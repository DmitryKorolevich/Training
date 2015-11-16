using VitalChoice.Validation.Models;

namespace VC.Admin.Models
{
	public class DeleteFileModel : BaseModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

		public string PublicId { get; set; }
	}
}