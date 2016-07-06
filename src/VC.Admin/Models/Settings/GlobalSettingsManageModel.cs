using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using System.Reflection;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VC.Admin.Models.Setting
{
    public class GlobalSettingsManageModel : BaseModel
    {
        public decimal? GlobalPerishableThreshold { get; set; }

        public bool CreditCardAuthorizations { get; set; }
    }
}