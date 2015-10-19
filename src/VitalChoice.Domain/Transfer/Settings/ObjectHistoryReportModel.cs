using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.eCommerce;

namespace VitalChoice.Domain.Transfer.Settings
{
    public class ObjectHistoryReportModel
    {
        public ObjectHistoryLogListItemModel Main { get; set; }

        public ObjectHistoryLogListItemModel Before { get; set; }

        public ObjectHistoryReportModel()
        {
        }
    }
}