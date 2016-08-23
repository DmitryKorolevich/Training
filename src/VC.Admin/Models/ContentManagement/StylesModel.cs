using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public class StylesModel:BaseModel
    {
        public int Id { get; set; }

        public string CSS { get; set; }

        public StylesModel(CustomPublicStyle item)
        {
            Id = item.Id;
            CSS = item.Styles;
        }
    }
}
