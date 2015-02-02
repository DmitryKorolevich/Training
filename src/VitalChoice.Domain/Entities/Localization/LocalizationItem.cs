﻿using System;
using VitalChoice.Domain;

namespace VitalChoice.Domain.Entities.Localization
{
	public class LocalizationItem : Entity
	{
        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public string GroupName { get; set; }

        public string ItemName { get; set; }
    }
}
