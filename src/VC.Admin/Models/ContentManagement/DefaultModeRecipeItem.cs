using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public abstract class DefaultModeRecipeItem: BaseModel
    {
	    public bool InUse { get; set; }

	    public byte Number { get; set; }
    }
}
