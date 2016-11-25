using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class AdminEditLockService : IAdminEditLockService
    {
        private readonly List<EditLockArea> _adminEditLockAreas;
        private readonly List<EditLockArea> _exportOrderEditLockAreas;
        private readonly ILogger _logger;

        private const int agentSecCount = 30;
        private const int exportOrderTimeoutSecCount = 120;

        public AdminEditLockService(ILoggerFactory loggerProvider)
        {
            _adminEditLockAreas = BaseAppConstants.ADMIN_EDIT_LOCK_AREAS != null
                ? BaseAppConstants.ADMIN_EDIT_LOCK_AREAS.Split(',').
                    Select(p => new EditLockArea(p)).ToList()
                : new List<EditLockArea>();

            _exportOrderEditLockAreas = _adminEditLockAreas.Where(p => BaseAppConstants.EXPORT_ORDERS_LOCK_AREAS.Split(',').Contains(p.Name)).ToList();

            _logger = loggerProvider.CreateLogger<AdminEditLockService>();
        }

        public bool AgentEditLockPing(EditLockPingModel model, string browserUserAgent)
        {
            var area = _adminEditLockAreas.FirstOrDefault(p => p.Name == model.AreaName);

            //not confugired
            if (area == null || model.Id == 0)
            {
                return true;
            }


            EditLockAreaItem currentStatus;
            lock (area.LockObject)
            {
                area.Items.TryGetValue(model.Id, out currentStatus);
            }

            if (currentStatus != null)
            {
                if (currentStatus.IdAgent == model.IdAgent && currentStatus.BrowserUserAgent == browserUserAgent)
                {
                    var now = DateTime.Now;
                    if (currentStatus.Expired <= now)
                    {
                        return false;
                    }
                    else
                    {
                        currentStatus.Expired = now.AddSeconds(agentSecCount);
                    }
                }
            }

            return true;
        }

        public EditLockRequestModel AgentEditLockRequest(EditLockRequestModel model, string browserUserAgent)
        {
            model.Avaliable = false;

            var area = _adminEditLockAreas.FirstOrDefault(p => p.Name == model.AreaName);

            //not confugired - allow edit
            if (area == null || model.Id == 0)
            {
                model.Avaliable = true;
                return model;
            }

            EditLockRequestModel toReturn;
            lock (area.LockObject)
            {
                var now = DateTime.Now;
                EditLockAreaItem currentStatus;
                if (!area.Items.TryGetValue(model.Id, out currentStatus))
                {
                    currentStatus = new EditLockAreaItem()
                    {
                        Agent = model.Agent,
                        AgentFirstName = model.AgentFirstName,
                        AgentLastName = model.AgentLastName,
                        IdAgent = model.IdAgent,
                        BrowserUserAgent = browserUserAgent,
                        LockMessageTitle = GetAgentLockMessageTitle(model.Agent, model.AgentFirstName, model.AgentLastName),
                        LockMessageBody = GetAgentLockMessageBody(model.Agent, model.AgentFirstName, model.AgentLastName),

                        Expired = now.AddSeconds(agentSecCount)
                    };
                    area.Items.Add(model.Id, currentStatus);
                }

                if (currentStatus.Expired <= now)
                {
                    currentStatus.Agent = model.Agent;
                    currentStatus.AgentFirstName = model.AgentFirstName;
                    currentStatus.AgentLastName = model.AgentLastName;
                    currentStatus.IdAgent = model.IdAgent;
                    currentStatus.BrowserUserAgent = browserUserAgent;
                    currentStatus.LockMessageTitle = GetAgentLockMessageTitle(model.Agent, model.AgentFirstName, model.AgentLastName);
                    currentStatus.LockMessageBody = GetAgentLockMessageBody(model.Agent, model.AgentFirstName, model.AgentLastName);

                    currentStatus.Expired = now.AddSeconds(agentSecCount);

                    model.Avaliable = true;
                    toReturn = model;
                }
                else
                {
                    //the same
                    if (currentStatus.IdAgent == model.IdAgent && currentStatus.BrowserUserAgent == browserUserAgent)
                    {
                        currentStatus.Expired = now.AddSeconds(agentSecCount);

                        model.Avaliable = true;
                        toReturn = model;
                    }
                    else
                    {
                        //different
                        toReturn = new EditLockRequestModel()
                        {
                            Agent = currentStatus.Agent,
                            AgentFirstName = currentStatus.AgentFirstName,
                            AgentLastName = currentStatus.AgentLastName,
                            IdAgent = currentStatus.IdAgent,
                            LockMessageTitle = currentStatus.LockMessageTitle,
                            LockMessageBody = currentStatus.LockMessageBody,

                            Avaliable = false,
                        };
                    }
                }
            }

            return toReturn;
        }

        public void ExportOrderEditLockRequest(int idOrder, string lockMessageTitle, string lockMessageBody)
        {
            EditLockAreaItem status = new EditLockAreaItem()
            {
                LockMessageTitle = lockMessageTitle,
                LockMessageBody = lockMessageBody,
                Expired = DateTime.Now.AddSeconds(exportOrderTimeoutSecCount)
            };

            foreach (var area in _exportOrderEditLockAreas)
            {
                lock (area.LockObject)
                {
                    area.Items.Remove(idOrder);
                    area.Items.Add(idOrder, status);
                }
            }
        }

        public void ExportOrderEditLockRelease(int idOrder)
        {
            foreach (var area in _exportOrderEditLockAreas)
            {
                lock (area.LockObject)
                {
                    area.Items.Remove(idOrder);
                }
            }
        }

        public bool GetIsOrderLocked(int idOrder)
        {
            foreach (var area in _exportOrderEditLockAreas)
            {
                lock (area.LockObject)
                {
                    EditLockAreaItem lockArea;
                    if (area.Items.TryGetValue(idOrder, out lockArea))
                    {
                        return lockArea.Expired > DateTime.Now;
                    }
                    return false;
                }
            }
            return false;
        }

        private string GetAgentLockMessageTitle(string agent, string agentFirstName, string agentLastName)
        {
            return $"This area is currently being viewed by {agentFirstName} {agentLastName} ({agent})";
        }

        private string GetAgentLockMessageBody(string agent, string agentFirstName, string agentLastName)
        {
            return $"This area is currently being viewed by {agentFirstName} {agentLastName} ({agent}). " +
                   $"You won't be able to save your changes. Wait a few minutes then refresh or contact {agentFirstName} {agentLastName} ({agent}).";
        }
    }
}
