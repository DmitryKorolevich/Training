using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class MethodInfoHelper
    {
        public static Delegate CompileStaticDelegateAccessor<TDelegate>(this MethodInfo method)
        {
            if (!(typeof (TDelegate).GetTypeInfo().IsSubclassOf(typeof (Delegate))))
                throw new ArgumentException("TDelegate should be a delegate");
            var delegateMethod = typeof (TDelegate).GetMethod("Invoke");
            return method.GetOrCompile(method.ReturnType, typeof (TDelegate),
                delegateMethod.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod.
        /// Static parameterless methods only, can be void result.
        /// </summary>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<TResult> CompileStaticAccessor<TResult>(this MethodInfo method)
        {
            return (Func<TResult>) method.GetOrCompile(typeof (TResult), typeof (Func<TResult>));
        }

        public static Action<T> CompileVoidAccessor<T>(this MethodInfo method)
        {
            return (Action<T>) method.GetOrCompile(typeof (void), typeof (Action<T>), typeof (T));
        }

        public static Action<T1, T2> CompileVoidAccessor<T1, T2>(this MethodInfo method)
        {
            return (Action<T1, T2>) method.GetOrCompile(typeof (void), typeof (Action<T1, T2>), typeof (T1), typeof (T2));
        }

        public static Action<T1, T2, T3> CompileVoidAccessor<T1, T2, T3>(this MethodInfo method)
        {
            return
                (Action<T1, T2, T3>) method.GetOrCompile(typeof (void), typeof (Action<T1, T2, T3>), typeof (T1), typeof (T2), typeof (T3));
        }

        public static Action<T1, T2, T3, T4> CompileVoidAccessor<T1, T2, T3, T4>(this MethodInfo method)
        {
            return
                (Action<T1, T2, T3, T4>)
                    method.GetOrCompile(typeof (void), typeof (Action<T1, T2, T3, T4>), typeof (T1), typeof (T2), typeof (T3), typeof (T4));
        }

        public static Action<T1, T2, T3, T4, T5> CompileVoidAccessor<T1, T2, T3, T4, T5>(this MethodInfo method)
        {
            return
                (Action<T1, T2, T3, T4, T5>)
                    method.GetOrCompile(typeof (void), typeof (Action<T1, T2, T3, T4, T5>), typeof (T1), typeof (T2), typeof (T3),
                        typeof (T4), typeof (T5));
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod
        /// </summary>
        /// <typeparam name="T">Either object to call method from or method parameter (depends weather method is static)</typeparam>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<T, TResult> CompileAccessor<T, TResult>(this MethodInfo method)
        {
            return (Func<T, TResult>) method.GetOrCompile(typeof (TResult), typeof (Func<T, TResult>), typeof (T));
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod
        /// </summary>
        /// <typeparam name="T1">Either object to call method from or method parameter (depends weather method is static)</typeparam>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<T1, T2, TResult> CompileAccessor<T1, T2, TResult>(this MethodInfo method)
        {
            return (Func<T1, T2, TResult>) method.GetOrCompile(typeof (TResult), typeof (Func<T1, T2, TResult>), typeof (T1), typeof (T2));
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod
        /// </summary>
        /// <typeparam name="T1">Either object to call method from or method parameter (depends weather method is static)</typeparam>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<T1, T2, T3, TResult> CompileAccessor<T1, T2, T3, TResult>(this MethodInfo method)
        {
            return
                (Func<T1, T2, T3, TResult>)
                    method.GetOrCompile(typeof (TResult), typeof (Func<T1, T2, T3, TResult>), typeof (T1), typeof (T2), typeof (T3));
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod
        /// </summary>
        /// <typeparam name="T1">Either object to call method from or method parameter (depends weather method is static)</typeparam>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<T1, T2, T3, T4, TResult> CompileAccessor<T1, T2, T3, T4, TResult>(this MethodInfo method)
        {
            return
                (Func<T1, T2, T3, T4, TResult>)
                    method.GetOrCompile(typeof (TResult), typeof (Func<T1, T2, T3, T4, TResult>), typeof (T1), typeof (T2), typeof (T3),
                        typeof (T4));
        }

        /// <summary>
        /// Emit and compile delegate using Accessor via DynamicMethod
        /// </summary>
        /// <typeparam name="T1">Either object to call method from or method parameter (depends weather method is static)</typeparam>
        /// <typeparam name="TResult">The result of calee</typeparam>
        /// <param name="method">MethodInfo to compile accessor to</param>
        /// <returns>Delegate to invoke method</returns>
        public static Func<T1, T2, T3, T4, T5, TResult> CompileAccessor<T1, T2, T3, T4, T5, TResult>(
            this MethodInfo method)
        {
            return
                (Func<T1, T2, T3, T4, T5, TResult>)
                    method.GetOrCompile(typeof (TResult), typeof (Func<T1, T2, T3, T4, T5, TResult>), typeof (T1), typeof (T2), typeof (T3),
                        typeof (T4), typeof (T5));
        }

        // ReSharper disable UnusedParameter.Local
        private static void ValidateParameters(MethodInfo method, Type[] typeParameters)
        {
            if (method.IsStatic && method.GetParameters().Length != typeParameters.Length ||
                !method.IsStatic && method.GetParameters().Length + 1 != typeParameters.Length)
                throw new ArgumentException("Method has different number of arguments");
        }
        // ReSharper enable UnusedParameter.Local

        private static DynamicMethod EmitDynamic(Type returnType, MethodInfo method, params Type[] typeParameters)
        {
            ValidateParameters(method, typeParameters);
            var dynamic = new DynamicMethod(method.Name, returnType, typeParameters,
                typeof(MethodInfoHelper), true);
            ILGenerator il = dynamic.GetILGenerator();
            il.EmitParameters(method, typeParameters);
            if (method.IsVirtual || method.IsAbstract)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }
            il.EmitCastIfNeeded(method.ReturnType, returnType);
            il.Emit(OpCodes.Ret);
            return dynamic;
        }

        private static void EmitCastIfNeeded(this ILGenerator il, Type one, Type toOther)
        {
            if (one == toOther)
                return;
            if (one.GetTypeInfo().IsValueType || toOther.GetTypeInfo().IsValueType)
            {
                if (one == typeof (object))
                {
                    il.Emit(OpCodes.Unbox_Any, toOther);
                    return;
                }
                if (toOther == typeof (object))
                {
                    il.Emit(OpCodes.Box, one);
                    return;
                }
                if (toOther.IsImplementGeneric(typeof (Nullable<>)) && !one.IsImplementGeneric(typeof(Nullable<>)))
                {
                    var constructor = toOther.GetConstructor(new[] {toOther.UnwrapNullable()});
                    if (constructor != null)
                    {
                        il.Emit(OpCodes.Newobj, constructor);
                    }
                    else
                    {
                        throw new InvalidOperationException($"{toOther} Type doesn't have parameterless constructor.");
                    }
                    return;
                }
                if (one.IsImplementGeneric(typeof(Nullable<>)) && !toOther.IsImplementGeneric(typeof(Nullable<>)))
                {
                    il.Emit(OpCodes.Call, one.GetProperty("Value").GetGetMethod());
                    return;
                }
                throw new InvalidOperationException("The ValueType parameters cast isn't allowed with this compiler");
            }
            il.Emit(OpCodes.Castclass, toOther);
        }

        private static void EmitParameters(this ILGenerator il, MethodInfo method, Type[] parameterTypes)
        {
            if (parameterTypes == null) throw new ArgumentNullException(nameof(parameterTypes));
            if (parameterTypes.Length == 0)
                return;
            int seed = method.IsStatic ? 0 : 1;
            var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            if (parameterTypes.Length > 0)
            {
                il.Emit(OpCodes.Ldarg_0);
                if (seed == 0)
                    il.EmitCastIfNeeded(parameterTypes[0], methodParameterTypes[0]);
            }
            if (parameterTypes.Length > 1)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCastIfNeeded(parameterTypes[1], methodParameterTypes[1 - seed]);
            }
            if (parameterTypes.Length > 2)
            {
                il.Emit(OpCodes.Ldarg_2);
                il.EmitCastIfNeeded(parameterTypes[2], methodParameterTypes[2 - seed]);
            }
            if (parameterTypes.Length > 3)
            {
                il.Emit(OpCodes.Ldarg_3);
                il.EmitCastIfNeeded(parameterTypes[3], methodParameterTypes[3 - seed]);
            }
            if (parameterTypes.Length > 4)
            {
                for (int i = 4; i < parameterTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i);
                    il.EmitCastIfNeeded(parameterTypes[i], methodParameterTypes[i - seed]);
                }
            }
        }

        private struct CacheItem : IEquatable<CacheItem>
        {
            public CacheItem(MethodInfo methodInfo, Type resultType, Type[] parameters)
            {
                if (methodInfo == null)
                    throw new ArgumentNullException(nameof(methodInfo));
                if (resultType == null)
                    throw new ArgumentNullException(nameof(resultType));

                _methodInfo = methodInfo;
                _resultType = resultType;
                _parameters = parameters;
            }

            private readonly MethodInfo _methodInfo;
            private readonly Type _resultType;
            private readonly Type[] _parameters;

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (_methodInfo.Name.GetHashCode() * 397) ^
                                   (_methodInfo.DeclaringType?.GetHashCode() ?? _methodInfo.ReturnType.GetHashCode());

                    hashCode = (hashCode * 397) ^ _resultType.GetHashCode();
                    // ReSharper disable once ForCanBeConvertedToForeach
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    if (_parameters != null)
                    {
                        for (int i = 0; i < _parameters.Length; i++)
                        {
                            var type = _parameters[i];
                            hashCode = (hashCode * 397) ^ type.GetHashCode();
                        }
                    }
                    return hashCode;
                }
            }

            public bool Equals(CacheItem other)
            {
                if (_parameters?.Length != other._parameters?.Length)
                    return false;

                var @equals = _resultType == other._resultType && _methodInfo == other._methodInfo;

                if (@equals && _parameters != null && other._parameters != null)
                {
                    for (var i = 0; i < _parameters.Length; i++)
                    {
                        @equals = @equals && _parameters[i] == other._parameters[i];
                    }
                    return @equals;
                }
                return @equals;
            }
        }

        private static readonly Dictionary<CacheItem, Delegate> CompiledCache = new Dictionary<CacheItem, Delegate>();

        private static Delegate GetOrCompile(this MethodInfo method, Type returnType, Type delegateType, params Type[] typeParameters)
        {
            CacheItem cacheKey = new CacheItem(method, returnType, typeParameters);
            lock (CompiledCache)
            {
                Delegate result;
                if (CompiledCache.TryGetValue(cacheKey, out result))
                {
                    return result;
                }
                var dynamic = EmitDynamic(returnType, method, typeParameters);
                result = dynamic.CreateDelegate(delegateType);
                CompiledCache.Add(cacheKey, result);
                return result;
            }
        }

    }
}