using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace VitalChoice.Domain.Helpers
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
            string expressionBody = expression.Body.ToString();
            lock (_lambdaCache)
            {
                if (_lambdaCache.TryGetValue(expressionBody, out result))
                {
                    return result;
                }
            }
            T newCompiled = expression.Compile();
            lock (_lambdaCache)
            {
                if (_lambdaCache.TryGetValue(expressionBody, out result))
                {
                    return result;
                }
                _lambdaCache.Add(expressionBody, newCompiled);
                return newCompiled;
            }

        }
    }

    public static class LambdaCompiler
    {
        public static T CacheCompile<T>(this Expression<T> expression)
        {
            return LambdaCompiler<T>.CacheCompile(expression);
        }
    }
}