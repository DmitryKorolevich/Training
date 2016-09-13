using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace VitalChoice.CustomerFiles.Entities
{
    public class VCustomerOldFile
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public string FilePath =>
            $"./attachments/{UploadDate.Year.ToString(CultureInfo.InvariantCulture)}/{UploadDate.Month.ToString(CultureInfo.InvariantCulture)}/{UploadDate.Day.ToString(CultureInfo.InvariantCulture)}/{Id}.dat"
            ;

        public string OriginalFileName { get; set; }

        public int IdCustomer { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
