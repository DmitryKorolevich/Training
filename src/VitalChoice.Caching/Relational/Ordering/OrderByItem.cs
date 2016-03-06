using System;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Caching.Relational.Ordering
{
    public class OrderByItem : IEquatable<OrderByItem>
    {
        public bool Equals(OrderByItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Comparer, other.Comparer) && string.Equals(MethodName, other.MethodName) && string.Equals(MemberSelector, other.MemberSelector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as OrderByItem;
            return other != null && Equals(other);
        }

        private int? _hashCode;

        public override int GetHashCode()
        {
            unchecked
            {
                if (!_hashCode.HasValue)
                {
                    var hashCode = MethodName.GetHashCode();
                    hashCode = (hashCode*397) ^ MemberSelector.GetHashCode();
                    if (Comparer != null)
                        hashCode = (hashCode*397) ^ Comparer.GetHashCode();
                    _hashCode = hashCode;
                }
                return _hashCode.Value;
            }
        }

        public static bool operator ==(OrderByItem left, OrderByItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OrderByItem left, OrderByItem right)
        {
            return !Equals(left, right);
        }

        public OrderByItem(string methodName, string memberSelector, string comparer)
        {
            MethodName = methodName;
            MemberSelector = memberSelector;
            Comparer = comparer;
        }

        public string MethodName { get; }
        public string Comparer { get; }
        public string MemberSelector { get; }
    }
}