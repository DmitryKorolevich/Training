using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class QueriableExpressionVisitor<T> : ExpressionVisitor
    {
        private bool _inWhereExpression;
        private readonly Dictionary<RelationCacheInfo, RelationInfo> _relationsUsed = new Dictionary<RelationCacheInfo, RelationInfo>();
        private RelationInfo _currentRelation;

        public WhereExpression<T> WhereExpression { get; private set; }
        public ICollection<RelationInfo> Relations => _relationsUsed.Values;

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (_inWhereExpression && typeof (T1) == typeof (Func<T, bool>))
            {
                LambdaExpressionVisitor lambdaVisitor = new LambdaExpressionVisitor();
                lambdaVisitor.Visit(node);

                if (WhereExpression != null)
                    throw new InvalidOperationException("Where clause used twice, need investigation");

                WhereExpression = new WhereExpression<T>
                {
                    Expression = (Expression<Func<T, bool>>) (object) node,
                    Conditions = lambdaVisitor.CurrentConditions,
                    Operations = lambdaVisitor.CurrentOperations
                };
                return node;
            }
            return base.VisitLambda(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression result;
            if (node.Method.DeclaringType == typeof (Queryable) && node.Method.Name == "Where")
            {
                if (!_inWhereExpression)
                {
                    _inWhereExpression = true;
                    result = base.VisitMethodCall(node);
                    _inWhereExpression = false;
                    return result;
                }
            }
            result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions) && node.Method.Name == "Include")
            {
                var memberExpression = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                string name = memberExpression?.Member.Name;
                Type relationType = memberExpression?.Type;

                var elementType = relationType.TryGetElementType(typeof (ICollection<>));
                relationType = elementType ?? relationType;

                var searchKey = new RelationCacheInfo(name, relationType);

                if (!_relationsUsed.TryGetValue(searchKey, out _currentRelation))
                {
                    var relationInfo = new RelationInfo(name, relationType);
                    _relationsUsed.Add(searchKey, relationInfo);

                    _currentRelation = relationInfo;
                }
            }
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions) && node.Method.Name == "ThenInclude")
            {
                var memberExpression = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                var name = memberExpression?.Member.Name;
                var relationType = memberExpression?.Type;

                var elementType = relationType.TryGetElementType(typeof (ICollection<>));
                relationType = elementType ?? relationType;

                if (_currentRelation == null)
                    throw new InvalidOperationException("ThenInclude used before Include, need investigation");

                var searchKey = new RelationCacheInfo(name, relationType);
                RelationInfo newCurrent;
                if (!_currentRelation.Relations.TryGetValue(searchKey, out newCurrent))
                {
                    var relationInfo = new RelationInfo(name, relationType);
                    _currentRelation.Relations.Add(searchKey, relationInfo);
                    newCurrent = relationInfo;
                }
                _currentRelation = newCurrent;
            }
            return result;
        }
    }
}