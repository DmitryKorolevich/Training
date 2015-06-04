using System;

namespace VitalChoice.DynamicData.Attributes
{
    public class MapAttribute : Attribute
    {
        public string Name { get; }

        public MapAttribute(string name = null)
        {
            Name = name;
        }
    }
}