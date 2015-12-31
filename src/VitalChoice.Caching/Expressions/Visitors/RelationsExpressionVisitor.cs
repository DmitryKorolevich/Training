using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class RelationsExpressionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<RelationCacheInfo, RelationInfo> _relationsUsed = new Dictionary<RelationCacheInfo, RelationInfo>();
        private RelationInfo _currentRelation;
        public ICollection<RelationInfo> Relations => _relationsUsed.Values;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions) && node.Method.Name == "Include")
            {
                var memberExpression = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                string name = memberExpression?.Member.Name;
                Type relationType = memberExpression?.Type;
                Type ownType = memberExpression?.Expression.Type;

                var elementType = relationType.TryGetElementType(typeof(ICollection<>));
                relationType = elementType ?? relationType;

                var searchKey = new RelationCacheInfo(name, relationType, ownType);

                if (!_relationsUsed.TryGetValue(searchKey, out _currentRelation))
                {
                    var relationInfo = new RelationInfo(name, relationType, ownType);
                    _relationsUsed.Add(searchKey, relationInfo);

                    _currentRelation = relationInfo;
                }
            }
            if (node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions) && node.Method.Name == "ThenInclude")
            {
                var memberExpression = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                var name = memberExpression?.Member.Name;
                var relationType = memberExpression?.Type;

                var elementType = relationType.TryGetElementType(typeof(ICollection<>));
                relationType = elementType ?? relationType;

                if (_currentRelation == null)
                    throw new InvalidOperationException("ThenInclude used before Include, need investigation");

                var searchKey = new RelationCacheInfo(name, relationType, _currentRelation.RelationType);
                RelationInfo newCurrent;
                if (!_currentRelation.RelationsDict.TryGetValue(searchKey, out newCurrent))
                {
                    var relationInfo = new RelationInfo(name, relationType, _currentRelation.RelationType);
                    _currentRelation.RelationsDict.Add(searchKey, relationInfo);
                    newCurrent = relationInfo;
                }
                _currentRelation = newCurrent;
            }
            return result;
        }
    }
}
