using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface ITypeConverter
    {
        Task<object> ConvertFromModelAsync(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        Task<object> ConvertToModelAsync(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
    }
}