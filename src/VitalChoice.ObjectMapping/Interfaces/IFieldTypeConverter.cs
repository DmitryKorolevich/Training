namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IFieldTypeConverter
    {
        object ConvertFrom(object obj);
        object DefaultValue { get; }
    }
}