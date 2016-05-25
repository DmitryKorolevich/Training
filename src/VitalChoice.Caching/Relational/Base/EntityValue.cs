using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public class EntityValue<TInfo> : IEquatable<EntityValue<TInfo>>
        where TInfo: EntityValueInfo
    {
        public EntityValue(TInfo valueInfo, object value)
        {

            if (valueInfo == null)
                throw new ArgumentNullException(nameof(valueInfo));

            ValueInfo = valueInfo;
            Value = CreateValue(value);
        }

        public bool Equals(EntityValue<TInfo> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj) return true;
            var entityKey = obj as EntityValue<TInfo>;
            return entityKey != null && Equals(entityKey);
        }

        public override string ToString()
        {
            return $"{ValueInfo.Name}: {Value}";
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool Equals(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return left?.Equals(right) ?? (object) right == null;
        }

        public static bool operator ==(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return !Equals(left, right);
        }

        public TInfo ValueInfo { get; }

        public IValue Value { get; }

        private static IValue CreateValue(object value)
        {
            if (value == null)
            {
                return new NullValue();
            }
            return Construct((dynamic) value);
        }

        private static IValue Construct(bool value)
        {
            return new BoolValue(value);
        }

        private static IValue Construct(Guid value)
        {
            return new GuidValue(value);
        }

        private static IValue Construct(char value)
        {
            return new CharValue(value);
        }

        private static IValue Construct(byte value)
        {
            return new ByteValue(value);
        }

        private static IValue Construct(sbyte value)
        {
            return new ByteValue((byte)value);
        }

        private static IValue Construct(short value)
        {
            return new ShortValue(value);
        }

        private static IValue Construct(ushort value)
        {
            return new ShortValue((short)value);
        }

        private static IValue Construct(int value)
        {
            return new IntValue(value);
        }

        private static IValue Construct(uint value)
        {
            return new IntValue((int)value);
        }

        private static IValue Construct(Enum value)
        {
            var type = Enum.GetUnderlyingType(value.GetType());
            var obj = Convert.ChangeType(value, type);
            return Construct((dynamic)obj);
        }

        private static IValue Construct(long value)
        {
            return new LongValue(value);
        }

        private static IValue Construct(ulong value)
        {
            return new LongValue((long)value);
        }

        private static IValue Construct(decimal value)
        {
            return new DecimalValue(value);
        }

        private static IValue Construct(DateTime value)
        {
            return new DateTimeValue(value);
        }

        private static IValue Construct(string value)
        {
            return new StringValue(value);
        }
    }
}