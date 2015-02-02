﻿using System;
using System.Reflection;

namespace VitalChoice.Domain.Entities.Localization
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class LocalizationGroupAttribute : Attribute
    {
        public int GroupId { get; private set; }
        public LocalizationGroupAttribute(int groupId)
        {
            GroupId = groupId;
        }
    }
}