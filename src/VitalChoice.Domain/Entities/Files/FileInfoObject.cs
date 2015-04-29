using System;
using System.IO;

namespace VitalChoice.Domain.Entities.Files
{
    public class FileInfoObject
    {
        public string Name { get; set; }

        public string FullRelativeName { get; set; }

        public string SizeMessage { get; set; }
        public long Size { get; set; }

        public FileInfoObject(string name,string fullRelativeName, long sizeBytes)
        {
            Name = name;
            FullRelativeName =fullRelativeName;
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
        }
    }
}