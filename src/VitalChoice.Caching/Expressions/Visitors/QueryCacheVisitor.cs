using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Ordering;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class QueryCacheVisitor : ExpressionVisitor
    {
        public QueryCacheVisitor(Type ownedType)
        {
            Relations = new RelationInfo(string.Empty, ownedType, ownedType, null);
        }

        private static readonly HashSet<string> MethodNames = new HashSet<string>
        {
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };

        public OrderBy OrderBy { get; private set; }
        public RelationInfo Relations { get; }

        private static readonly ConcurrentDictionary<RelationCacheInfo, RelationInfo> RelationsUsed = new ConcurrentDictionary<RelationCacheInfo, RelationInfo>();
        private RelationInfo _currentRelation;

        private LambdaExpression GetOrderExpression(ICollection<Expression> arguments, out Expression comparer)
        {
            if (arguments.Count == 2)
            {
                comparer = null;
                return (arguments.Last() as UnaryExpression)?.Operand as LambdaExpression;
            }
            comparer = arguments.Last();
            return (arguments.Skip(1).First() as UnaryExpression)?.Operand as LambdaExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions))
            {
                MemberExpression memberExpression;
                string name;
                Type relationType;
                Type elementType;
                RelationCacheInfo searchKey;
                switch (node.Method.Name)
                {
                    case "Include":
                        var lambda = (node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                        memberExpression = lambda?.Body as MemberExpression;
                        name = memberExpression?.Member.Name;
                        relationType = memberExpression?.Type;
                        Type ownType = memberExpression?.Expression.Type;

                        elementType = relationType.TryGetElementType(typeof (ICollection<>)) ?? relationType;

                        searchKey = new RelationCacheInfo(name, elementType, ownType);
                        _currentRelation = RelationsUsed.GetOrAdd(searchKey, _ => new RelationInfo(name, elementType, ownType, lambda));

                        if (!Relations.RelationsDict.ContainsKey(_currentRelation.Name))
                        {
                            Relations.RelationsDict.Add(_currentRelation.Name, _currentRelation);
                        }
                        break;
                    case "ThenInclude":
                        var lambdaExpression = (node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                        memberExpression = lambdaExpression?.Body as MemberExpression;
                        name = memberExpression?.Member.Name;
                        if (name == null)
                        {
                            throw new InvalidOperationException("ThenInclude contains invalid relation name");
                        }
                        relationType = memberExpression?.Type;

                        elementType = relationType.TryGetElementType(typeof(ICollection<>)) ?? relationType;

                        if (_currentRelation == null)
                            throw new InvalidOperationException("ThenInclude used before Include, need investigation");

                        RelationInfo newCurrent;
                        if (!_currentRelation.RelationsDict.TryGetValue(name, out newCurrent))
                        {
                            searchKey = new RelationCacheInfo(name, elementType, _currentRelation.RelationType);
                            var relationInfo = RelationsUsed.GetOrAdd(searchKey,
                                _ => new RelationInfo(name, elementType, _currentRelation.RelationType, lambdaExpression));
                            _currentRelation.RelationsDict.Add(name, relationInfo);
                            newCurrent = relationInfo;
                        }
                        _currentRelation = newCurrent;
                        break;
                }
            }
            if (node.Method.DeclaringType == typeof (Queryable) || node.Method.DeclaringType == typeof (Enumerable))
            {
                if (MethodNames.Contains(node.Method.Name))
                {
                    Expression comparer;
                    var expr = GetOrderExpression(node.Arguments, out comparer);
                    if (comparer == null)
                    {
                        AddOrderByItem(node.Method.Name, expr.ToString());
                    }
                    else
                    {
                        AddOrderByItem(node.Method.Name, expr.ToString(), comparer.ToString());
                    }
                }
            }
            return result;
        }

        private void AddOrderByItem(string methodName, string memberSelector, string comparer = null)
        {
            if (OrderBy == null)
            {
                OrderBy = new OrderBy();
            }
            OrderBy.OrderByItems.Add(new OrderByItem(methodName, memberSelector, comparer));
        }
    }
}
