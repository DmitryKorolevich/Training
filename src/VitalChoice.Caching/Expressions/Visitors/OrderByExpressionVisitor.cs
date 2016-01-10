using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class OrderByExpressionVisitor<T> : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter = Expression.Parameter(typeof (IEnumerable<CacheResult<T>>));
        private MethodCallExpression _current;

        public bool Ordered => _current != null;

        public Func<IEnumerable<T>, IOrderedEnumerable<T>> GetOrderByFunction()
        {
            return
                Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(_current,
                    _parameter).CacheCompile();
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
                            var method = GetOrderMethodWithComparer(expr, "OrderBy");
                            _current = Expression.Call(method, _parameter, expr, comparer);
                        }
                        else
                        {
                            var method = GetOrderMethod(expr, "OrderBy");
                            _current = Expression.Call(method, _parameter, expr);
                        }
                        break;
                    case "OrderByDescending":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = GetOrderMethodWithComparer(expr, "OrderByDescending");
                            _current = Expression.Call(method, _parameter, expr, comparer);
                        }
                        else
                        {
                            var method = GetOrderMethod(expr, "OrderByDescending");
                            _current = Expression.Call(method, _parameter, expr);
                        }
                        break;
                    case "ThenBy":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = GetOrderMethodWithComparer(expr, "ThenBy");
                            _current = Expression.Call(method, _current, expr, comparer);
                        }
                        else
                        {
                            var method = GetOrderMethod(expr, "ThenBy");
                            _current = Expression.Call(method, _current, expr);
                        }
                        break;
                    case "ThenByDescending":
                        expr = GetOrderExpression(node.Arguments, out comparer);
                        if (comparer != null)
                        {
                            var method = GetOrderMethodWithComparer(expr, "ThenByDescending");
                            _current = Expression.Call(method, _current, expr, comparer);
                        }
                        else
                        {
                            var method = GetOrderMethod(expr, "ThenByDescending");
                            _current = Expression.Call(method, _current, expr);
                        }
                        break;
                }
            }
            return result;
        }

        private static MethodInfo GetOrderMethod(LambdaExpression expr, string name)
        {
            var method =
                typeof (Enumerable).GetMethods()
                    .Single(m => m.Name == name && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof (CacheResult<T>), expr.ReturnType);
            return method;
        }

        private static MethodInfo GetOrderMethodWithComparer(LambdaExpression expr, string name)
        {
            var method =
                typeof (Enumerable).GetMethods()
                    .Single(m => m.Name == name && m.GetParameters().Length == 3)
                    .MakeGenericMethod(typeof (CacheResult<T>), expr.ReturnType);
            return method;
        }

        //private static readonly Expression<Func<CacheResult<T>, T>> Converter = result => result.Entity;

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

        //private LambdaExpression CreateNewFunction(LambdaExpression expression)
        //{
        //    return ((IFnctionFactory)
        //        Activator.CreateInstance(typeof (FunctionFactory<>).MakeGenericType(typeof (T), expression.ReturnType)))
        //        .CreateNewFunction(expression);
        //}

        //private interface IFnctionFactory
        //{
        //    LambdaExpression CreateNewFunction(LambdaExpression initialExpression);
        //}

        //private class FunctionFactory<TResult> : IFnctionFactory
        //{
        //    public LambdaExpression CreateNewFunction(LambdaExpression initialExpression)
        //    {
        //        ParameterExpression param = Expression.Parameter(typeof (CacheResult<T>));
        //        return
        //            Expression.Lambda<Func<CacheResult<T>, TResult>>(Expression.Invoke(initialExpression,
        //                Expression.Invoke(Converter, param)), param);
        //    }
        //}
    }
}