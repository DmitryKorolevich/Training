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
                root.SubCategories = subCategories.ToList();
            }

            foreach (var subCategory in root.SubCategories)
            {
                CreateSubCategoriesList(subCategory, heapCategories);
            }
        }
    }
}