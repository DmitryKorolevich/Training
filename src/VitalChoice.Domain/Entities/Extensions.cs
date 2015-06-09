using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Products;

namespace VitalChoice.Domain.Entities
{
    public static class Extensions
    {
        public static void CreateSubCategoriesList(this ContentCategory root,
            ICollection<ContentCategory> heapCategories)
        {
            if (root.SubCategories == null)
            {
                var subCategories = heapCategories.Where(p => p.ParentId == root.Id).ToArray();
                foreach (var subCategory in subCategories)
                {
                    subCategory.Parent = root;
                }
                root.SubCategories = subCategories.OrderBy(p => p.Order).ToArray();
            }

            foreach (var subCategory in root.SubCategories)
            {
                CreateSubCategoriesList(subCategory, heapCategories);
            }
        }

        public static void SetSubCategoriesOrder(this ContentCategory root)
        {
            if (root.SubCategories != null)
            {
                var subCategories = root.SubCategories.ToArray();
                for (int i = 0; i < subCategories.Length; i++)
                {
                    subCategories[i].Order = i;
                }
                root.SubCategories = subCategories;
            }

            if (root.SubCategories != null)
            {
                foreach (var subCategory in root.SubCategories)
                {
                    SetSubCategoriesOrder(subCategory);
                }
            }
        }

        public static void CreateSubCategoriesList(this ProductCategory root,
            ICollection<ProductCategory> heapCategories)
        {
            if (root.SubCategories == null)
            {
                var subCategories = heapCategories.Where(p => p.ParentId == root.Id).ToArray();
                foreach (var subCategory in subCategories)
                {
                    subCategory.Parent = root;
                }
                root.SubCategories = subCategories.OrderBy(p => p.Order).ToArray();
            }

            foreach (var subCategory in root.SubCategories)
            {
                CreateSubCategoriesList(subCategory, heapCategories);
            }
        }

        public static void SetSubCategoriesOrder(this ProductCategory root)
        {
            if (root.SubCategories != null)
            {
                var subCategories = root.SubCategories.ToArray();
                for (int i = 0; i < subCategories.Length; i++)
                {
                    subCategories[i].Order = i;
                }
                root.SubCategories = subCategories;
            }

            if (root.SubCategories != null)
            {
                foreach (var subCategory in root.SubCategories)
                {
                    SetSubCategoriesOrder(subCategory);
                }
            }
        }
    }
}