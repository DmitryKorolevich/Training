using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        private static readonly List<SessionInfo> SessionPool = new List<SessionInfo>(20);

        private static int _currentSession;

        private readonly IEncryptedServiceBusHostClient _encryptedBusHost;
        private bool IsAuthenticated =>_encryptedBusHost.IsAuthenticatedClient(SessionId);
        public string ServerHostName => _encryptedBusHost.ServerHostName;
        public string LocalHostName => _encryptedBusHost.LocalHostName;

        protected virtual int MaxSessions => 10;

        protected virtual int ExpireTimeMinutes => 60;

        protected EncryptedServiceBusClient(IEncryptedServiceBusHostClient encryptedBusHost)
        {
            _encryptedBusHost = encryptedBusHost;
            SessionId = GetSession();
        }

        public Guid SessionId { get; private set; }

        protected async Task<T> SendCommand<T>(ServiceBusCommandWithResult command)
        {
            if (!await EnsureAuthenticated())
            {
                throw new Exception("Cannot authenticate export client");
            }

            return await _encryptedBusHost.ExecuteCommand<T>(command);
        }

        private async Task<bool> EnsureAuthenticated()
        {
            if (!IsAuthenticated)
            {
                var sessionId = SessionId;
                //double auth try to refresh broken/regenerated public key
                try
                {
                    if (!await _encryptedBusHost.AuthenticateClient(sessionId))
                    {
                        SetInvalid(sessionId);
                        sessionId = SessionId;
                        if (!await _encryptedBusHost.AuthenticateClient(sessionId))
                        {
                            return false;
                        }
                    }
                }
                catch (ApiException)
                {
                    SetInvalid(sessionId);
                    sessionId = SessionId;
                    if (!await _encryptedBusHost.AuthenticateClient(sessionId))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected async Task SendCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, ServiceBusCommandData> requestAcqureAction)
        {
            if (!await EnsureAuthenticated())
            {
                requestAcqureAction?.Invoke(command, new ServiceBusCommandData
                {
                    Error = "Cannot authenticate export client"
                });
            }

            _encryptedBusHost.ExecuteCommand(command, requestAcqureAction);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        private Guid GetSession()
        {
            lock (SessionPool)
            {
                if (_currentSession + 1 > SessionPool.Count)
                {
                    _currentSession = 0;
                    if (SessionPool.Count < MaxSessions)
                    {
                        SessionPool.Add(new SessionInfo
                        {
                            SessionId = Guid.NewGuid(),
                            CreationTime = DateTime.Now
                        });
                    }
                }
                var sessionInfo = SessionPool[_currentSession];
                if ((DateTime.Now - sessionInfo.CreationTime).TotalMinutes > ExpireTimeMinutes)
                {
                    sessionInfo = new SessionInfo
                    {
                        SessionId = Guid.NewGuid(),
                        CreationTime = DateTime.Now
                    };
                    SessionPool[_currentSession] = sessionInfo;
                }
                _currentSession++;
                return sessionInfo.SessionId;
            }
        }

        private void SetInvalid(Guid session)
        {
            lock (SessionPool)
            {
                if (SessionPool[_currentSession].SessionId == session)
                {
                    var sessionInfo = new SessionInfo
                    {
                        SessionId = Guid.NewGuid(),
                        CreationTime = DateTime.Now
                    };
                    SessionPool[_currentSession] = sessionInfo;
                    SessionId = sessionInfo.SessionId;
                    return;
                }
                for (var i = 0; i < SessionPool.Count; i++)
                {
                    if (SessionPool[i].SessionId == session)
                    {
                        var sessionInfo = new SessionInfo
                        {
                            SessionId = Guid.NewGuid(),
                            CreationTime = DateTime.Now
                        };
                        SessionPool[i] = sessionInfo;
                        SessionId = sessionInfo.SessionId;
                        return;
                    }
                }
                SessionId = GetSession();
            }
        }

        private struct SessionInfo
        {
            public Guid SessionId { get; set; }

            public DateTime CreationTime { get; set; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~EncryptedServiceBusClient()
        {
            Dispose(false);
        }
    }
}