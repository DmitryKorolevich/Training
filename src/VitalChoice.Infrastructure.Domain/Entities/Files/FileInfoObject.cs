using System;

namespace VitalChoice.Infrastructure.Domain.Entities.Files
{
    public class FileInfoObject
    {
        public string Name { get; set; }

        public string FullRelativeName { get; set; }

        public string DirectoryFullRelativeName { get; set; }

        public string SizeMessage { get; set; }

        public long Size { get; set; }

        public DateTime Updated { get; set; }

        public FileInfoObject()
        { }

        public FileInfoObject(string name,string fullRelativeName, string directoryFullRelativeName, long sizeBytes, DateTime updated, string filesPrefix)
        {
            Name = name;
            FullRelativeName = filesPrefix + fullRelativeName;
            DirectoryFullRelativeName = directoryFullRelativeName;
            Size = sizeBytes;
            var kBytes = Math.Round((double)sizeBytes / 1024,1);
            if(kBytes<1)
            {
                kBytes = 1;
            }
            if(kBytes>1024)
            {
                var mBytes = Math.Round((double)sizeBytes / 1048576, 1);
                SizeMessage = mBytes + " mb";
            }
            else
            {
                SizeMessage = kBytes + " kb";
            }
            Updated = updated;
        }
    }
}