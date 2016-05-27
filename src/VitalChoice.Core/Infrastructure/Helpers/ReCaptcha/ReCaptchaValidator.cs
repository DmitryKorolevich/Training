using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Core.Infrastructure.Helpers.ReCaptcha
{
    public class ReCaptchaValidator
    {
        public const string DefaultPostParamName = "g-Recaptcha-Response";

	    private readonly GoogleCaptcha reCaptcha;

	    public ReCaptchaValidator(IOptions<AppOptions> options)
	    {
		    reCaptcha = options.Value.GoogleCaptcha;
	    }

	    public async Task<bool> Validate(string userResponse)
	    {
		    using (var client = new HttpClient())
		    {
				var googleReply = await client.PostAsync($"{reCaptcha.VerifyUrl}?secret={reCaptcha.SecretKey}&response={userResponse}", null);

				googleReply.EnsureSuccessStatusCode();

				var res = await googleReply.Content.ReadAsStringAsync();

				var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptcha>(res);

			    bool valid;
			    return bool.TryParse(captchaResponse.Success?.ToLower(), out valid) && valid;
		    }
		}
    }
}
