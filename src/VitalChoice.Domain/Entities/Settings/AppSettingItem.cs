using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Settings
{
    public class AppSettingItem : Entity
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
