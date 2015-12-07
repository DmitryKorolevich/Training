using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Product
{
    public class ReviewListItemModel
    {
		public string Title { get; set; }

		public string Review { get; set; }

		public string CustomerName { get; set; }

		public DateTime DateCreated { get; set; }

		public int Rating { get; set; }
	}
}
