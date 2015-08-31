using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Base
{
    public struct TypePair : IEquatable<TypePair>
    {
        public TypePair(Type modelType, Type dynamicType)
        {
            DynamicType = dynamicType;
            ModelType = modelType;
        }

        private sealed class ModelTypeDynamicTypeEqualityComparer : IEqualityComparer<TypePair>
        {
            public bool Equals(TypePair x, TypePair y)
            {
                return x.ModelType == y.ModelType && x.DynamicType == y.DynamicType;
            }

            public int GetHashCode(TypePair obj)
            {
                unchecked
                {
                    return ((obj.ModelType?.GetHashCode() ?? 0)*397) ^ (obj.DynamicType?.GetHashCode() ?? 0);
                }
            }
        }

        public static IEqualityComparer<TypePair> PairComparer { get; } =
            new ModelTypeDynamicTypeEqualityComparer();

        public Type ModelType { get; }
        public Type DynamicType { get; }
        public bool Equals(TypePair other)
        {
            return other.ModelType == ModelType && other.DynamicType == DynamicType;
        }
    }
}
