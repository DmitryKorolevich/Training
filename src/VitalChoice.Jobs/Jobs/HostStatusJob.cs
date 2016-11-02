using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using VitalChoice.Business.Mailings;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Jobs.Jobs
{
    public class HostStatusJob : IJob
    {
        private readonly IOptions<AppOptions> _options;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public HostStatusJob(ILoggerFactory loggerFactory, IOptions<AppOptions> options, IEmailSender emailSender)
        {
            _options = options;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<HostStatusJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.LogWarning("Host Status Check started");
            try
            {
                if (!CheckHost(_options.Value.PublicHost))
                {
                    _logger.LogCritical("Restarting public app pool");
                    using (
                        var process =
                            Process.Start(new ProcessStartInfo(@"C:\Windows\system32\inetsrv\appcmd.exe", "stop apppool /apppool.name:public")))
                    {
                        process?.WaitForExit();
                    }
                    using (
                        var process =
                            Process.Start(new ProcessStartInfo(@"C:\Windows\system32\inetsrv\appcmd.exe", "start apppool /apppool.name:public")))
                    {
                        process?.WaitForExit();
                    }
                }

                if (!CheckHost(_options.Value.AdminHost))
                {
                    _logger.LogCritical("Restarting admin app pool");
                    using (
                        var process =
                            Process.Start(new ProcessStartInfo(@"C:\Windows\system32\inetsrv\appcmd.exe", "stop apppool /apppool.name:admin")))
                    {
                        process?.WaitForExit();
                    }
                    using (
                        var process =
                            Process.Start(new ProcessStartInfo(@"C:\Windows\system32\inetsrv\appcmd.exe", "start apppool /apppool.name:admin")))
                    {
                        process?.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            _logger.LogWarning("Host Status Check stopped");
        }

        private bool CheckHost(string host, int retryNumber = 0, HttpStatusCode previousStatus = HttpStatusCode.OK)
        {
            try
            {
                var request = WebRequest.Create($"https://{host}/");
                request.Method = HttpMethod.Get.Method;
                request.Timeout = 120000;
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var result = ProcessResponse(host, retryNumber, previousStatus, response);
                    response.GetResponseStream()?.Dispose();
                    return result;
                }
            }
            catch (WebException e)
            {
                var response = e.Response as HttpWebResponse;
                if (response == null)
                {
                    _logger.LogError(e.ToString());
                    return true;
                }
                return ProcessResponse(host, retryNumber, previousStatus, response);
            }
            catch (Exception e)
            {
                var errorBody = e.ToString();
                _logger.LogError(errorBody);
                SendAlertEmail("Vital Choice Host Failure", errorBody);
            }
            return true;
        }

        private bool ProcessResponse(string host, int retryNumber, HttpStatusCode previousStatus, HttpWebResponse response)
        {
            if ((int) response.StatusCode >= 500)
            {
                var errorBody = $"{host} Check Failure, Status: {response.StatusCode}";
                _logger.LogError(errorBody);
                SendAlertEmail("Vital Choice Host Failure", errorBody);
            }
            if (response.StatusCode == HttpStatusCode.BadGateway || response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                if (retryNumber > 1)
                {
                    return false;
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
                return CheckHost(host, retryNumber + 1);
            }
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                if (retryNumber > 1 && previousStatus == HttpStatusCode.ServiceUnavailable)
                {
                    return false;
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
                return CheckHost(host, retryNumber + 1, response.StatusCode);
            }
            return true;
        }

        private void SendAlertEmail(string subject, string body)
        {
            _emailSender.SendEmailAsync(_options.Value.MainSuperAdminEmail, subject, body, "Vital Choice Alert", isBodyHtml: false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
