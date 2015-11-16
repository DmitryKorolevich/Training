using System;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models
{
	public class FileModel : BaseModel
	{
		public int Id { get; set; }

		public string FileName { get; set; }

		public DateTime UploadDate { get; set; }

		public string Description { get; set; }
	}
}