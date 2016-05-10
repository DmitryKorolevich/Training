using System.Threading.Tasks;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IModelConverter
    {
        Task DynamicToModelAsync(object model, object dynamic);
        Task ModelToDynamicAsync(object model, object dynamic);
    }

    public interface IModelConverter<in TModel, in TDynamic> : IModelConverter
    {
        Task DynamicToModelAsync(TModel model, TDynamic dynamic);
        Task ModelToDynamicAsync(TModel model, TDynamic dynamic);
    }
}