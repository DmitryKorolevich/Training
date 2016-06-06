namespace VitalChoice.Ecommerce.Domain.Entities.VeraCore
{
    public class VeraCoreFile : VeraCoreFileInfo
    {
        public string Data { get; set; }

        public VeraCoreFile(VeraCoreFileInfo info, string data)
        {
            FileDate = info.FileDate;
            FileName = info.FileName;
            FileSize = info.FileSize;
            Data = data;
        }
    }
}
