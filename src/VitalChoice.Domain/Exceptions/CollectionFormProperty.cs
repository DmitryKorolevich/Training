namespace VitalChoice.Domain.Exceptions
{
    public static class CollectionFormProperty
    {
        public static string GetFullName(string collectionName, int index, string propertyName)
        {
            return $"{collectionName}.i{index}.{propertyName}";
        }
    }
}