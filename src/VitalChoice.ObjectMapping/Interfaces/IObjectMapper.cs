using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IObjectUpdater
    {
        Task<object> ToModelAsync(object obj, Type modelType);
        Task UpdateModelAsync(object obj, Type modelType, object model);
        Task UpdateObjectAsync(Type modelType, object model, object obj);
        IDictionary<string, object> ToDictionary(object obj);
        void UpdateModel(object obj, IDictionary<string, object> model);
        Task UpdateObjectAsync(object obj, IDictionary<string, object> model, bool caseSense = true);
        void SecureObject(object obj);
        bool IsObjectSecured(object obj);
    }

    public interface IObjectMapper : IObjectUpdater
    {
        Task<object> FromModelAsync(Type modelType, object model);
        Task<object> FromDictionaryAsync(IDictionary<string, object> model, bool caseSense = true);
    }

    public interface IObjectUpdater<in T> : IObjectUpdater
    {
        Task<TModel> ToModelAsync<TModel>(T obj)
            where TModel : class, new();

        Task UpdateModelAsync<TModel>(TModel model, T obj);
        Task UpdateObjectAsync<TModel>(TModel model, T obj);
        //void CloneInto<TBase>(T dest, T src);
    }

    public interface IObjectMapper<T> : IObjectMapper, IObjectUpdater<T>
        where T : class, new()
    {
        Task<T> FromModelAsync<TModel>(TModel model);
        T Clone<TBase>(T obj);
        T Clone<TBase>(T obj, Func<TBase, TBase> cloneBaseFunc);
    }
}