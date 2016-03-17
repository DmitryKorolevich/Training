using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class LambdaCompiler<T>
    {
        private static Dictionary<string, T> _lambdaCache;

        public static T CacheCompile(Expression<T> expression)
        {
            if (_lambdaCache == null)
            {
                Interlocked.CompareExchange(ref _lambdaCache, new Dictionary<string, T>(), null);
            }
            T result;

            var key = expression.AsString();

            lock (_lambdaCache)
            {
                if (_lambdaCache.TryGetValue(key, out result))
                {
                    return result;
                }
            }
            T newCompiled = expression.Compile();
            lock (_lambdaCache)
            {
                if (_lambdaCache.TryGetValue(key, out result))
                {
                    return result;
                }
                _lambdaCache.Add(key, newCompiled);
                return newCompiled;
            }

        }
    }
    public static class LambdaCompiler
    {
        public static string AsString(this Expression expression)
        {
            var cachableExpression = Evaluator.PartialEval(expression);
            cachableExpression = LocalCollectionExpander.Rewrite(cachableExpression);
            string key = cachableExpression.ToString();
            return key;
        }

        public static T CacheCompile<T>(this Expression<T> expression)
        {
            return LambdaCompiler<T>.CacheCompile(expression);
        }
    }
}