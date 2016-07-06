using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        private static readonly List<SessionInfo> SessionPool = new List<SessionInfo>(20);

        private static int _currentSession;

        private readonly IEncryptedServiceBusHostClient _encryptedBusHost;
        private readonly IObjectEncryptionHost _encryptionHost;
        private bool IsAuthenticated => _session.Authenticated;

        public Guid SessionId => _session.SessionId;

        public string ServerHostName => _encryptedBusHost.ServerHostName;
        public string LocalHostName => _encryptedBusHost.LocalHostName;

        protected virtual int MaxSessions => 10;

        protected virtual int ExpireTimeMinutes => 60;

        protected EncryptedServiceBusClient(IEncryptedServiceBusHostClient encryptedBusHost, IObjectEncryptionHost encryptionHost)
        {
            _encryptedBusHost = encryptedBusHost;
            _encryptionHost = encryptionHost;
            _session = GetSession();
        }

        private SessionInfo _session;

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
                var sessionId = _session.SessionId;
                //double auth try to refresh broken/regenerated public key
                if (!await _encryptedBusHost.AuthenticateClient(sessionId))
                {
                    SetInvalid(sessionId);
                    sessionId = _session.SessionId;
                    if (!await _encryptedBusHost.AuthenticateClient(sessionId))
                    {
                        SetInvalid(sessionId);
                        return false;
                    }
                }
                _session.Authenticated = true;
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
                return;
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

        private SessionInfo GetSession()
        {
            lock (SessionPool)
            {
                if (_currentSession + 1 > SessionPool.Count)
                {
                    _currentSession = 0;
                    if (SessionPool.Count < MaxSessions)
                    {
                        var newInfo = new SessionInfo
                        {
                            SessionId = Guid.NewGuid(),
                            CreationTime = DateTime.Now
                        };
                        CreateAndRegisterSession(newInfo);
                        SessionPool.Add(newInfo);
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
                    CreateAndRegisterSession(sessionInfo);
                    SessionPool[_currentSession] = sessionInfo;
                }
                _currentSession++;
                return sessionInfo;
            }
        }

        private void CreateAndRegisterSession(SessionInfo sessionInfo)
        {
            var keys = _encryptionHost.CreateSession(sessionInfo.SessionId);
            _encryptionHost.RegisterSession(sessionInfo.SessionId, keys);
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
                    CreateAndRegisterSession(sessionInfo);
                    SessionPool[_currentSession] = sessionInfo;
                    _session = sessionInfo;
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
                        CreateAndRegisterSession(sessionInfo);
                        SessionPool[i] = sessionInfo;
                        _session = sessionInfo;
                        return;
                    }
                }
                _session = GetSession();
            }
        }

        private class SessionInfo
        {
            public Guid SessionId { get; set; }

            public DateTime CreationTime { get; set; }

            public bool Authenticated { get; set; }
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