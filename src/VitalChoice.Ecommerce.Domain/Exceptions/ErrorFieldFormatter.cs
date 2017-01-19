namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public static class ErrorFieldFormatter
    {
        public static string FormatCollectionError(this string propertyName, string collectionName, int index)
        {
            if (!string.IsNullOrEmpty(collectionName))
            {
                return $"{collectionName}.i{index}.{propertyName}";
            }
            else
            {
                return $"i{index}.{propertyName}";
            }
        }

        public static string FormatErrorWithForm(this string fieldName, string formName)
        {
            if (!string.IsNullOrEmpty(formName))
                return $"{formName}::{fieldName}";
            return fieldName;
        }
    }
}