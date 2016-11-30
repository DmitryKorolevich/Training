using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class EditLockArea
    {
        public EditLockArea(string name)
        {
            Name = name;
            Items = new Dictionary<int, EditLockAreaItem>();
            LockObject = new object();
        }

        public string Name { get; set; }

        public object LockObject { get; set; }

        public Dictionary<int, EditLockAreaItem> Items { get; set; }
    }
}