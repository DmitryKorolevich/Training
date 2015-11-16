using System.Linq;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public class CategoryTreeItemModel : BaseModel
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
            var subModels =
                category.SubCategories.Select(subCategory => new CategoryTreeItemModel(subCategory)).ToList();
            model.SubItems = subModels;
        }

        public ContentCategory Convert()
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
                subItems.AddRange(SubItems.Select(subItem => subItem.Convert(toReturn.Id)));
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}