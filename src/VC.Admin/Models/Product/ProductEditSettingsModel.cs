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
using System.Dynamic;

namespace VC.Admin.Models.Product
{
    public class ProductEditSettingsModel
    {
        public ICollection<LookupViewModel> Lookups { get; set; }

        public Dictionary<int, Dictionary<string,string>> DefaultValues { get; set; }

        public ProductEditSettingsModel()
        {
            DefaultValues = new Dictionary<int, Dictionary<string, string>>();
        }
    }
}