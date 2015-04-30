using System;
using System.Collections;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Files
{
    public class DirectoryInfoObject
    {
        public string Name { get; set; }

        public string FullRelativeName { get; set; }

        public ICollection<DirectoryInfoObject> Directories { get; set; }

        public DirectoryInfoObject()
        {
        }

        public DirectoryInfoObject(string name,string fullRelativeName)
        {
            Name = name;
            FullRelativeName = fullRelativeName;
            Directories = new List<DirectoryInfoObject>();
        }
    }
}