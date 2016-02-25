using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Cart
{
    public class CartCrossSellModel:BaseModel
    {
	    public string Title { get; set; }

	    public string ImageUrl { get; set; }

	    public string SkuCode { get; set; }

	    public decimal Price { get; set; }
    }
}
