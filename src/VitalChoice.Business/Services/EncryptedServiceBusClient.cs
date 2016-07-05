using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public Guid SessionId { get; }

        protected async Task<T> SendCommand<T>(ServiceBusCommandWithResult command)
        {
            if (!IsAuthenticated)
            {
                //double auth try to refresh broken/regenerated public key
                try
                {
                    if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                    {
                        return default(T);
                    }
                }
                catch (ApiException)
                {
                    if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                    {
                        return default(T);
                    }
                }
            }

            return await _encryptedBusHost.ExecuteCommand<T>(command);
        }

        protected async Task SendCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction)
        {
            if (!IsAuthenticated)
            {
                //double auth try to refresh broken/regenerated public key
                if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                {
                    if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                    {
                        return;
                    }
                }
            }

            _encryptedBusHost.ExecuteCommand(command, requestAcqureAction);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _encryptedBusHost.RemoveClient(SessionId);
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
                    SessionPool[_currentSession] = new SessionInfo
                    {
                        SessionId = Guid.NewGuid(),
                        CreationTime = DateTime.Now
                    };
                }
                _currentSession++;
                return sessionInfo.SessionId;
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