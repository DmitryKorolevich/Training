﻿using System;
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

        public override int GetHashCode()
        {
            return PairComparer.GetHashCode(this);
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

    public struct GenericTypePair : IEquatable<GenericTypePair>
    {
        public GenericTypePair(Type first, Type second)
        {
            First = first;
            Second = second;
        }

        public override int GetHashCode()
        {
            return PairComparer.GetHashCode(this);
        }

        private sealed class ModelTypeDynamicTypeEqualityComparer : IEqualityComparer<GenericTypePair>
        {
            public bool Equals(GenericTypePair x, GenericTypePair y)
            {
                return x.First == y.First && x.Second == y.Second;
            }

            public int GetHashCode(GenericTypePair obj)
            {
                unchecked
                {
                    return ((obj.First?.GetHashCode() ?? 0) * 397) ^ (obj.Second?.GetHashCode() ?? 0);
                }
            }
        }

        public static IEqualityComparer<GenericTypePair> PairComparer { get; } =
            new ModelTypeDynamicTypeEqualityComparer();

        public Type Second { get; }
        public Type First { get; }
        public bool Equals(GenericTypePair other)
        {
            return other.Second == Second && other.First == First;
        }
    }
}
