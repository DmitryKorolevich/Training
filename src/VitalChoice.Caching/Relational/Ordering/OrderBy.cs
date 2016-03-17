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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return OrderByItems.SimpleJoin(other.OrderByItems).All(pair => pair.Key == pair.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as OrderBy;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var item in OrderByItems)
                result = (result*397) ^ item.GetHashCode();
            return result;
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