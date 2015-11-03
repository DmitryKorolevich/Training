using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IObjectMapper
    { 
        object ToModel(object obj, Type modelType);
        void UpdateModel(object obj, Type modelType, object model);
        object FromModel(Type modelType, object model);
        void UpdateObject(Type modelType, object model, object obj);
    }

    public interface IObjectMapper<TObject>
    {
        TModel ToModel<TModel>(TObject obj)
            where TModel : class, new();
        void UpdateModel<TModel>(TModel model, TObject obj);
        IDictionary<string, object> ToDictionary(TObject obj);
        void UpdateModel(TObject obj, IDictionary<string, object> model);
        TObject FromModel<TModel>(TModel model);
        void UpdateObject<TModel>(TModel model, TObject obj);
        TObject FromDictionary(IDictionary<string, object> model, bool caseSense = true);
        void UpdateObject(TObject obj, IDictionary<string, object> model, bool caseSense = true);
        void SecureObject(TObject obj);
    }
}