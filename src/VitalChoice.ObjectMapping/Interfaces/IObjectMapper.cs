using System;
using System.Collections.Generic;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IObjectUpdater
    {
        object ToModel(object obj, Type modelType);
        void UpdateModel(object obj, Type modelType, object model);
        void UpdateObject(Type modelType, object model, object obj);
        IDictionary<string, object> ToDictionary(object obj);
        void UpdateModel(object obj, IDictionary<string, object> model);
        void UpdateObject(object obj, IDictionary<string, object> model, bool caseSense = true);
        void SecureObject(object obj);
        bool IsObjectSecured(object obj);
    }

    public interface IObjectMapper : IObjectUpdater
    {
        object FromModel(Type modelType, object model);
        object FromDictionary(IDictionary<string, object> model, bool caseSense = true);
    }

    public interface IObjectUpdater<in T> : IObjectUpdater
    {
        TModel ToModel<TModel>(T obj)
            where TModel : class, new();

        void UpdateModel<TModel>(TModel model, T obj);
        void UpdateObject<TModel>(TModel model, T obj);
        void CloneInto<TBase>(T dest, T src);
    }

    public interface IObjectMapper<T> : IObjectMapper, IObjectUpdater<T>
        where T : class, new()
    {
        T FromModel<TModel>(TModel model);
        T Clone<TBase>(T obj);
        T Clone<TBase>(T obj, Func<TBase, TBase> cloneBaseFunc);
    }
}