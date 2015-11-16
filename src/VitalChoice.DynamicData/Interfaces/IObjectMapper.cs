using System;
using System.Collections.Generic;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IObjectMapper
    { 
        object ToModel(object obj, Type modelType);
        void UpdateModel(object obj, Type modelType, object model);
        object FromModel(Type modelType, object model);
        void UpdateObject(Type modelType, object model, object obj);
        IDictionary<string, object> ToDictionary(object obj);
        void UpdateModel(object obj, IDictionary<string, object> model);
        object FromDictionary(IDictionary<string, object> model, bool caseSense = true);
        void UpdateObject(object obj, IDictionary<string, object> model, bool caseSense = true);
        void SecureObject(object obj);
    }

    public interface IObjectMapper<TObject> : IObjectMapper
    {
        TModel ToModel<TModel>(TObject obj)
            where TModel : class, new();
        void UpdateModel<TModel>(TModel model, TObject obj);
        TObject FromModel<TModel>(TModel model);
        void UpdateObject<TModel>(TModel model, TObject obj);
        TObject Clone<TBase>(TObject obj);
    }
}