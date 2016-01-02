using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class OrderByExpressionVisitor<T> : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter = Expression.Parameter(typeof (IEnumerable<T>));
        private MethodCallExpression _current;

        public Func<IEnumerable<T>, IOrderedEnumerable<T>> GetOrderByFunction()
        {
            return Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(_current, _parameter).CacheCompile();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof (Queryable))
            {
                Expression comparer;
                LambdaExpression expr;
                switch (node.Method.Name)
                {
                    case "OrderBy":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = typeof (Enumerable).GetMethod("OrderBy",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType),
                                    typeof (IComparer<>).MakeGenericType(expr.ReturnType)
                                });
                            _current = Expression.Call(method, _parameter, expr, comparer);
                        }
                        else
                        {
                            var method = typeof (Enumerable).GetMethod("OrderBy",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType)
                                });
                            _current = Expression.Call(method, _parameter, expr);
                        }
                        break;
                    case "OrderByDescending":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = typeof(Enumerable).GetMethod("OrderByDescending",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType),
                                    typeof (IComparer<>).MakeGenericType(expr.ReturnType)
                                });
                            _current = Expression.Call(method, _parameter, expr, comparer);
                        }
                        else
                        {
                            var method = typeof(Enumerable).GetMethod("OrderByDescending",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType)
                                });
                            _current = Expression.Call(method, _parameter, expr);
                        }
                        break;
                    case "ThenBy":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = typeof(Enumerable).GetMethod("ThenBy",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType),
                                    typeof (IComparer<>).MakeGenericType(expr.ReturnType)
                                });
                            _current = Expression.Call(method, _current, expr, comparer);
                        }
                        else
                        {
                            var method = typeof(Enumerable).GetMethod("ThenBy",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType)
                                });
                            _current = Expression.Call(method, _current, expr);
                        }
                        break;
                    case "ThenByDescending":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = typeof(Enumerable).GetMethod("ThenByDescending",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType),
                                    typeof (IComparer<>).MakeGenericType(expr.ReturnType)
                                });
                            _current = Expression.Call(method, _current, expr, comparer);
                        }
                        else
                        {
                            var method = typeof(Enumerable).GetMethod("ThenByDescending",
                                new[]
                                {
                                    typeof (Func<>).MakeGenericType(typeof (T), expr.ReturnType)
                                });
                            _current = Expression.Call(method, _current, expr);
                        }
                        break;
                }
            }
            return result;
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
    }
}