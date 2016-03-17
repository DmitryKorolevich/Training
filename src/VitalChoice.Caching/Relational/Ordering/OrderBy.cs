using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Ordering
{
    public class OrderBy : IEquatable<OrderBy>
    {
        public bool Equals(OrderBy other)
        {
            if ((object)this == (object)other) return true;
            if ((object)other == null) return false;

            return OrderByItems.SimpleJoin(other.OrderByItems).All(pair => pair.Key == pair.Value);
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj) return true;
            if (obj == null) return false;
            var other = obj as OrderBy;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            int result = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in OrderByItems)
                result = (result*397) ^ item.GetHashCode();
            return result;
        }

        public static bool Equals(OrderBy left, OrderBy right)
        {
            return left?.Equals(right) ?? (object) right == null;
        }

        public static bool operator ==(OrderBy left, OrderBy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OrderBy left, OrderBy right)
        {
            return !Equals(left, right);
        }

        public List<OrderByItem> OrderByItems { get; } = new List<OrderByItem>();
    }
}