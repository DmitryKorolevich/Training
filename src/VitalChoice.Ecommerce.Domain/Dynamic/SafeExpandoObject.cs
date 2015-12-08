using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Ecommerce.Domain.Dynamic
{
    public sealed class SafeExpandoObject : IDynamicMetaObjectProvider, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictObj;
        private readonly IEnumerable<KeyValuePair<string, object>> _enumerable;

        public SafeExpandoObject(ExpandoObject obj)
        {
            _dictObj = obj;
            _enumerable = obj;
        }

        #region IDynamicMetaObjectProvider Members

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new MetaExpando(parameter, this);
        }

        #endregion

        #region IDictionary<string, object> Members

        ICollection<string> IDictionary<string, object>.Keys => _dictObj.Keys;

        ICollection<object> IDictionary<string, object>.Values => _dictObj.Values;

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                object value;
                if (!_dictObj.TryGetValue(key, out value))
                {
                    return null;
                }
                return value;
            }
            set { throw new ApiException("Object read only"); }
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            throw new ApiException("Object read only");
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return _dictObj.ContainsKey(key);
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            throw new ApiException("Object read only");
        }

        internal static bool ExpandoTryGetValue(object expando, string key, out object value)
        {
            value = null;
            return (expando as IDictionary<string, object>)?.TryGetValue(key, out value) ?? false;
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _dictObj.TryGetValue(key, out value);
        }

        #endregion

        #region ICollection<KeyValuePair<string, object>> Members 

        int ICollection<KeyValuePair<string, object>>.Count => _dictObj.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => true;

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            throw new ApiException("Object read only");
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            throw new ApiException("Object read only");
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            object value;
            if (!_dictObj.TryGetValue(item.Key, out value))
            {
                return false;
            }

            return object.Equals(value, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<string, object> item in _dictObj)
            {
                array[arrayIndex++] = item;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            throw new ApiException("Object read only");
        }

        #endregion

        #region IEnumerable<KeyValuePair<string, object>> Member 

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        #endregion

        #region MetaExpando

        private class MetaExpando : DynamicMetaObject
        {
            public MetaExpando(Expression expression, SafeExpandoObject value)
                : base(expression, BindingRestrictions.Empty, value)
            {
            }

            private DynamicMetaObject BindGetOrInvokeMember(string name, DynamicMetaObject fallback,
                Func<DynamicMetaObject, DynamicMetaObject> fallbackInvoke)
            {
                ParameterExpression value = Expression.Parameter(typeof (object), "value");

                Expression tryGetValue = Expression.Call(
                    typeof (SafeExpandoObject).GetTypeInfo().GetDeclaredMethod("ExpandoTryGetValue"),
                    Expression.Constant(Value),
                    Expression.Constant(name),
                    value
                    );

                var result = new DynamicMetaObject(value, BindingRestrictions.Empty);
                if (fallbackInvoke != null)
                {
                    result = fallbackInvoke(result);
                }

                result = new DynamicMetaObject(
                    Expression.Block(
                        new[] {value},
                        tryGetValue,
                        result.Expression
                        ),
                    result.Restrictions.Merge(fallback.Restrictions)
                    );

                return result;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return BindGetOrInvokeMember(
                    binder.Name,
                    binder.FallbackGetMember(this),
                    null
                    );
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                return BindGetOrInvokeMember(
                    binder.Name,
                    binder.FallbackInvokeMember(this, args),
                    value => binder.FallbackInvoke(value, args, null)
                    );
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                var value = Value as SafeExpandoObject;
                return value?._dictObj.Keys ?? Enumerable.Empty<string>();
            }

            /// <summary> 
            /// Returns our Expression converted to our known LimitType
            /// </summary> 
            private Expression GetLimitedSelf()
            {
#if DNX451 || NET451
                if (Expression.Type == LimitType || Expression.Type.IsEquivalentTo(LimitType))
#else
                if (Expression.Type == LimitType)
#endif
                {
                    return Expression;
                }
                return Expression.Convert(Expression, LimitType);
            }
        }

        #endregion
    }
}