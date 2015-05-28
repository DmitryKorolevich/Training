using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.ContentManagement
{
    public class CategoryTreeItemModel : Model<ContentCategory, IMode>
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }

        public ContentType Type { get; set; }
        
        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<CategoryTreeItemModel> SubItems { get; set; }

        public CategoryTreeItemModel(ContentCategory item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                Type = item.Type;
                StatusCode = item.StatusCode;
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(CategoryTreeItemModel model, ContentCategory category)
        {
            var subModels = new List<CategoryTreeItemModel>();
            foreach(var subCategory in category.SubCategories)
            {
                CategoryTreeItemModel subModel = new CategoryTreeItemModel(subCategory);
                subModels.Add(subModel);
            }
            model.SubItems = subModels;
        }

        public override ContentCategory Convert()
        {
            return Convert(null);
        }

        public ContentCategory Convert(int? parentId)
        {
            ContentCategory toReturn = new ContentCategory();
            toReturn.Id=Id;
            toReturn.ParentId = parentId;
            toReturn.Name = Name;
            toReturn.Url = Url;
            toReturn.Type = Type;
            var subItems = new List<ContentCategory>();
            if (SubItems!=null)
            {
                foreach(var subItem in SubItems)
                {
                    subItems.Add(subItem.Convert(toReturn.Id));
                }
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}