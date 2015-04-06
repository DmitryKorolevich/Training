using System;
using System.Collections.Generic;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Core.Infrastructure.Models
{
	public class PagedModelList<TEntity> where TEntity : IModel
	{
		public PagedModelList()
		{
			Items = new List<TEntity>();
        }

		public IList<TEntity> Items { get; set; }
        public int Count { get; set; }
    }
}