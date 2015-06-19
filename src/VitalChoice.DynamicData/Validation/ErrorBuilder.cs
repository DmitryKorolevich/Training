using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class ErrorBuilder<TProperty> : ErrorBuilderBase<TProperty>, IErrorBuilder<TProperty>
    {

        public ErrorBuilder(TProperty obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
        }

        public IErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index)
        {
            return Collection(collectionExpression, new[] {index});
        }

        public virtual IErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes)
        {
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression) collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) collectionSelector;
                var innerCollectionValue = collectionExpression.Compile().Invoke(Data);
                var itemIndexes = indexes.ToArray();
                TResultProperty innerItem = innerCollectionValue.Skip(itemIndexes.FirstOrDefault()).FirstOrDefault();
                return new ErrorBuilder<TResultProperty>(innerItem, member.Member.Name, itemIndexes);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }



        public virtual ICollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression) 
            where TResultProperty : class
        {
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression) collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) collectionSelector;
                var innerCollectionValue = collectionExpression.Compile().Invoke(Data);
                return new CollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty>(innerCollectionValue, member.Member.Name);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public virtual IErrorResult<TProperty> Property(
            Expression<Func<TProperty, object>> propertyExpression)
        {
            Expression fieldSelector = propertyExpression;
            // ReSharper disable once UseNullPropagation
            if (fieldSelector is LambdaExpression)
            {
                fieldSelector = ((LambdaExpression) fieldSelector).Body;
            }
            if (fieldSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) fieldSelector;
                var memberName = member.Member.Name;
                return new ErrorResult<TProperty>(CollectionName, Indexes, memberName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }

    public class CollectionErrorBuilder<TCollection, TProperty> : ErrorBuilderBase<TCollection>, ICollectionErrorBuilder<TCollection, TProperty>
        where TProperty : class
        where TCollection : ICollection<TProperty>
    {
        public CollectionErrorBuilder(TCollection obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
        }

        public IErrorResult<TProperty> Property<TEntity>(
            Expression<Func<TProperty, object>> propertySelector, ICollection<TEntity> values,
            Expression<Func<TEntity, object>> valueSelector)
        {
            Expression fieldSelector = propertySelector;
            // ReSharper disable once UseNullPropagation
            if (fieldSelector is LambdaExpression)
            {
                fieldSelector = ((LambdaExpression)fieldSelector).Body;
            }
            if (fieldSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression)fieldSelector;
                var dynamicFieldName = member.Member.Name;
                var getter = propertySelector.Compile();
                var entityGetter = valueSelector.Compile();
                var valueSet = new HashSet<object>(values.Select(v => entityGetter.Invoke(v)));
                int index = 0;
                List<int> indexes = new List<int>();
                if (Data != null)
                {
                    foreach (var item in Data)
                    {
                        if (valueSet.Contains(getter.Invoke(item)))
                        {
                            indexes.Add(index);
                        }
                        index++;
                    }
                }
                return new ErrorResult<TProperty>(CollectionName, indexes.ToArray(), dynamicFieldName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}