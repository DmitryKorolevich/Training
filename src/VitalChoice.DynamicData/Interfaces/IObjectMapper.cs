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
        void ToModel(object obj, Type modelType, object model);
        object FromModel(Type modelType, object model);
        void FromModel(Type modelType, object model, object obj);
    }

    public interface IObjectMapper<TObject>
    {
        TModel ToModel<TModel>(TObject obj)
            where TModel : class, new();
        void ToModel<TModel>(TObject obj, TModel model);
        IDictionary<string, object> ToDictionary(TObject obj);
        void ToDictionary(TObject obj, IDictionary<string, object> model);
        TObject FromModel<TModel>(TModel model);
        void FromModel<TModel>(TModel model, TObject obj);
        TObject FromDictionary(IDictionary<string, object> model);
        void FromDictionary(IDictionary<string, object> model, TObject obj);
        void SecureObject(TObject obj);
    }
}