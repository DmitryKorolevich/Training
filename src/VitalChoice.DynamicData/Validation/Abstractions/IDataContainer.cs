namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IDataContainer<out TProperty>
    {
        TProperty Data { get; }
    }
}