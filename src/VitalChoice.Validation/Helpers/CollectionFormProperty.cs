﻿namespace VitalChoice.Validation.Helpers
{
    public static class CollectionFormProperty
    {
        public static string GetFullName(string collectionName, int index, string propertyName)
        {
            return $"{collectionName}.i{index}.{propertyName}";
        }
    }
}