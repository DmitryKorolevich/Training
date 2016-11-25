using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class AdminEditLockService : IAdminEditLockService
    {
        private readonly List<EditLockArea> _adminEditLockAreas;
        private readonly List<EditLockArea> _exportOrderEditLockAreas;

        private const int agentSecCount = 30;
        private const int exportOrderTimeoutSecCount = 120;

        public AdminEditLockService()
        {
            _adminEditLockAreas = BaseAppConstants.ADMIN_EDIT_LOCK_AREAS.Split(',').Select(p => new EditLockArea(p)).ToList();
            _exportOrderEditLockAreas = BaseAppConstants.EXPORT_ORDERS_LOCK_AREAS.Split(',').Select(p => new EditLockArea(p)).ToList();
        }

        private bool GetIsLocked(IEnumerable<EditLockArea> lockAreas, EditLockPingModel model,
            Func<EditLockArea, EditLockAreaItem, bool> lockCheck, out EditLockAreaItem item)
        {
            var area = lockAreas.FirstOrDefault(p => p.Name == model.AreaName);

            if (area == null || model.Id == 0)
            {
                item = null;
                return false;
            }

            lock (area.LockObject)
            {
                return area.Items.TryGetValue(model.Id, out item) && lockCheck(area, item);
            }
        }

        public bool AgentEditLockPing(EditLockPingModel model, string browserUserAgent)
        {
            var now = DateTime.Now;
            EditLockAreaItem lockItem;
            if (GetIsLocked(_exportOrderEditLockAreas, model, (area, item) => item.Expired <= now, out lockItem))
            {
                return false;
            }
            return !GetIsLocked(_adminEditLockAreas, model, (area, item) =>
            {
                if (item.IdAgent == model.IdAgent && item.BrowserUserAgent == browserUserAgent)
                {
                    item.Expired = now.AddSeconds(agentSecCount);
                    return false;
                }
                return true;
            }, out lockItem);
        }

        public EditLockRequestModel AgentEditLockRequest(EditLockRequestModel model, string browserUserAgent)
        {
            model.Avaliable = false;

            var now = DateTime.Now;

            EditLockAreaItem lockItem;
            if (GetIsLocked(_exportOrderEditLockAreas, model, (area, item) => item.Expired <= now, out lockItem))
            {
                return new EditLockRequestModel
                {
                    Agent = lockItem.Agent,
                    AgentFirstName = lockItem.AgentFirstName,
                    AgentLastName = lockItem.AgentLastName,
                    IdAgent = lockItem.IdAgent,
                    LockMessageTitle = lockItem.LockMessageTitle,
                    LockMessageBody = lockItem.LockMessageBody,
                    Avaliable = false,
                };
            }

            if (GetIsLocked(_adminEditLockAreas, model, (area, item) =>
            {
                if (item.IdAgent == model.IdAgent && item.BrowserUserAgent == browserUserAgent)
                {
                    item.Expired = now.AddSeconds(agentSecCount);
                    return false;
                }
                lockItem = new EditLockAreaItem
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
                area.Items.Add(model.Id, lockItem);
                model.Avaliable = true;
                return true;
            }, out lockItem))
            {
                return new EditLockRequestModel
                {
                    Agent = lockItem.Agent,
                    AgentFirstName = lockItem.AgentFirstName,
                    AgentLastName = lockItem.AgentLastName,
                    IdAgent = lockItem.IdAgent,
                    LockMessageTitle = lockItem.LockMessageTitle,
                    LockMessageBody = lockItem.LockMessageBody,
                    Avaliable = false,
                };
            }

            return model;
        }

        public void ExportOrderEditLockRequest(int idOrder, string lockMessageTitle, string lockMessageBody)
        {
            var status = new EditLockAreaItem
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