using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VC.Admin.Models.Product
{
    public class LookupViewModel
    {
        public string Name { get; set; }

        public int? Type { get; set; }

        public int DefaultValue { get; set; }

        public ICollection<LookupItem<int>> Items { get; set; }

        public LookupViewModel(string name, int? type, string defaultValue, Lookup lookup)
        {
            this.Name = name;
            this.Type = type;
            int res = 1;
            if(Int32.TryParse(defaultValue, out res))
            {
                DefaultValue = res;
            }
            if(lookup!=null)
            {
                Items = lookup.LookupVariants.Select(x => new LookupItem<int>
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            }
        }
    }
}