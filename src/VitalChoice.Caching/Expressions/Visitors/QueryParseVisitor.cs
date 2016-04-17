using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Ordering;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class QueryParseVisitor<T> : ExpressionVisitor
    {
        public QueryParseVisitor()
        {
            Relations = new RelationInfo(string.Empty, typeof(T), typeof(T));
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly HashSet<string> MethodNames = new HashSet<string>
        {
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };

        public bool NonCached { get; private set; }
        public bool Tracking { get; private set; } = true;
        public OrderBy OrderBy { get; private set; }
        public RelationInfo Relations { get; }

        
        private RelationInfo _currentRelation;

        public WhereExpression<T> WhereExpression { get; private set; }


        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (typeof(T1) != typeof(Func<T, bool>))
                return base.VisitLambda(node);

            LambdaExpressionVisitor<T> lambdaVisitor = new LambdaExpressionVisitor<T>();
            lambdaVisitor.Visit(node.Body);

            if (WhereExpression != null)
            {
                ExpressionSwapVisitor swapVisitor = new ExpressionSwapVisitor(WhereExpression.Expression.Parameters[0], node.Parameters[0]);
                var newExpression = (Expression<Func<T, bool>>)swapVisitor.Visit(WhereExpression.Expression);
                WhereExpression.Expression =
                    Expression.Lambda<Func<T, bool>>(Expression.AndAlso(newExpression.Body, node.Body), node.Parameters);
                WhereExpression.Condition = new BinaryCondition(ExpressionType.AndAlso, newExpression)
                {
                    Left = WhereExpression.Condition,
                    Right = lambdaVisitor.Condition
                };
            }
            else
            {
                WhereExpression = new WhereExpression<T>((Expression<Func<T, bool>>)(object)node)
                {
                    Condition = lambdaVisitor.Condition
                };
            }
            return node;
        }

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
            if (node.Method.DeclaringType == typeof (QueryableExtension))
            {
                switch (node.Method.Name)
                {
                    case "AsNonCached":
                        NonCached = true;
                        return node.Arguments[0];
                }
            }
            if (node.Method.DeclaringType == typeof (RelationalQueryableExtensions))
            {
                if (node.Method.Name == "FromSql")
                {
                    NonCached = true;
                    return node;
                }
            }
            if (node.Method.DeclaringType == typeof(Enumerable))
            {
                if (node.Method.Name == "Select" || node.Method.Name == "SelectMany")
                {
                    NonCached = true;
                    return node;
                }
            }
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                if (node.Method.Name == "Select" || node.Method.Name == "SelectMany")
                {
                    NonCached = true;
                    return node;
                }
            }
            var result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions))
            {
                MemberExpression memberExpression;
                string name;
                Type relationType;
                Type elementType;
                switch (node.Method.Name)
                {
                    case "Include":
                        var lambda = (node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                        memberExpression = lambda?.Body as MemberExpression;
                        name = memberExpression?.Member.Name;
                        relationType = memberExpression?.Type;
                        Type ownType = memberExpression?.Expression.Type;

                        elementType = relationType.TryGetElementType(typeof (ICollection<>)) ?? relationType;

                        if (name == null)
                        {
                            throw new InvalidOperationException("Include contains invalid relation name");
                        }

                        if (!Relations.RelationsDict.TryGetValue(name, out _currentRelation))
                        {
                            _currentRelation = CompiledRelationsCache.GetRelation(name, elementType, ownType, lambda);
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
                        relationType = memberExpression.Type;

                        elementType = relationType.TryGetElementType(typeof(ICollection<>)) ?? relationType;

                        if (_currentRelation == null)
                            throw new InvalidOperationException("ThenInclude used before Include, need investigation");

                        RelationInfo newCurrent;
                        if (!_currentRelation.RelationsDict.TryGetValue(name, out newCurrent))
                        {
                            var relationInfo = CompiledRelationsCache.GetRelation(name, elementType, _currentRelation.RelationType,
                                lambdaExpression);
                            _currentRelation.RelationsDict.Add(name, relationInfo);
                            newCurrent = relationInfo;
                        }
                        _currentRelation = newCurrent;
                        break;
                    case "AsNoTracking":
                        Tracking = false;
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
