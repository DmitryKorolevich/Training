namespace VitalChoice.Domain.Exceptions
{
    public static class ErrorFieldFormatter
    {
        public static string Collection(string collectionName, int index, string propertyName)
        {
            return $"{collectionName}.i{index}.{propertyName}";
        }

        public static string Form(string formName, string fieldName)
        {
            if (!string.IsNullOrEmpty(formName))
                return $"{formName}::{fieldName}";
            return fieldName;
        }
    }
}