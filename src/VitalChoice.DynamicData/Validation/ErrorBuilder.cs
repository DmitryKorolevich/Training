using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Validation.Abstractions;
using VitalChoice.Ecommerce.Domain.Helpers;

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
            if (Data == null)
                throw new ArgumentException("Object you are trying to compare is null");
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression) collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) collectionSelector;
                var innerCollectionValue = collectionExpression.CacheCompile().Invoke(Data);
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
            if (Data == null)
                throw new ArgumentException("Object you are trying to compare is null");
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression) collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) collectionSelector;
                var innerCollectionValue = collectionExpression.CacheCompile().Invoke(Data);
                return new CollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty>(innerCollectionValue, member.Member.Name);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public virtual IErrorResult Property(
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
                PropertyName = memberName;
                return this;
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}