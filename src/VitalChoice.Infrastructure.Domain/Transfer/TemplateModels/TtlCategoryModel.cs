﻿using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlCategoryModel
    {
	    public TtlCategoryModel()
	    {
			SubCategories = new List<TtlCategoryModel>();
			SideMenuItems = new List<TtlSidebarMenuItemModel>();
			Products = new List<TtlCategoryProductModel>();
			BreadcrumbOrderedItems = new List<TtlBreadcrumbItemModel>();
        }

	    public string Name { get; set; }

		public string Url { get; set; }

		public int Order { get; set; }

		public string FileImageSmallUrl { get; set; }

		public string FileImageLargeUrl { get; set; }

		public string LongDescription { get; set; }

		public string LongDescriptionBottom { get; set; }

	    public IList<TtlCategoryModel> SubCategories { get; set; }

	    public IList<TtlCategoryProductModel> Products { get; set; }

	    public IList<TtlSidebarMenuItemModel> SideMenuItems { get; set; }

	    public IList<TtlBreadcrumbItemModel> BreadcrumbOrderedItems { get; set; }
    }
}