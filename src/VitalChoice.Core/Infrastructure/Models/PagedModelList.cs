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

		public int StartIndex { get; set; }

		public int? PageItemCount
		{
			get { return this._pageItemCount; }
			set
			{
				if (value.HasValue && value.Value < 0)
					throw new InvalidOperationException();

				this._pageItemCount = value;
			}
		}

		private int? _pageItemCount = null;

		public int? TotalItemCount { get; set; }

		public int PageIndex => StartIndex/PageItemCount ?? 0;
	}
}