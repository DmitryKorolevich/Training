using System;
using System.Collections.Concurrent;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class EditLockRequestModel : EditLockPingModel
    {
        public bool Avaliable { get; set; }

        public string LockMessageTitle { get; set; }

        public string LockMessageBody { get; set; }
    }
}