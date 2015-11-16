using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public class ContentAreaUpdateModel: BaseModel
    {
	    public int Id { get; set; }

	    public string Template { get; set; }
	}
}
