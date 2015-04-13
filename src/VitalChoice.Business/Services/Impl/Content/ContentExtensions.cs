using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Impl.Content
{
    internal static class ContentExtensions
    {
        internal static void CreateSubCategoriesList(this ContentCategory root, IEnumerable<ContentCategory> heapCategories)
        {
            if (root.SubCategories == null)
            {
                var subCategories = heapCategories.Where(p => p.ParentId == root.Id);
                heapCategories.ToList().RemoveAll(p => p.ParentId == root.Id);
                foreach (var subCategory in subCategories)
                {
                    subCategory.Parent = root;
                }
                root.SubCategories = subCategories.OrderBy(p=>p.Order).ToList();
            }

            foreach (var subCategory in root.SubCategories)
            {
                CreateSubCategoriesList(subCategory, heapCategories);
            }
        }

        internal static void SetSubCategoriesOrder(this ContentCategory root)
        {
            if (root.SubCategories != null)
            {
                var subCategories = root.SubCategories.ToList();
                for (int i=0;i< subCategories.Count; i++)
                {
                    subCategories[i].Order = i;
                }
                root.SubCategories = subCategories;
            }

            foreach (var subCategory in root.SubCategories)
            {
                SetSubCategoriesOrder(subCategory);
            }
        }
    }
}