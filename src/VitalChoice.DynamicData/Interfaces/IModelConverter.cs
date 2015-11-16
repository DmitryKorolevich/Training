namespace VitalChoice.DynamicData.Interfaces
{
    public interface IModelConverter
    {
        void DynamicToModel(object model, object dynamic);
        void ModelToDynamic(object model, object dynamic);
    }

    public interface IModelConverter<in TModel, in TDynamic> : IModelConverter
    {
        void DynamicToModel(TModel model, TDynamic dynamic);
        void ModelToDynamic(TModel model, TDynamic dynamic);
    }
}