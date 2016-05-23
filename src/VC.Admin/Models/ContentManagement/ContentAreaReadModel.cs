using VitalChoice.Infrastructure.Domain.Content;

namespace VC.Admin.Models.ContentManagement
{
    public class ContentAreaReadModel : ContentAreaUpdateModel
    {
        public string Name { get; set; }

        public ContentAreaReadModel(ContentArea item)
        {
            Id = item.Id;
            Template = item.Template;
            Name = item.Name;
        }
    }
}
