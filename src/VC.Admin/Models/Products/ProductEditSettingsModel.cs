using System.Collections.Generic;

namespace VC.Admin.Models.Products
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