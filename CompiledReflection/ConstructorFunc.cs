using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace CompiledReflection
{
    /// <summary>
    ///     Finds and wraps constructors that match the type parameters of <see cref="Func{TResult}" />s
    /// </summary>
    public static class ConstructorFunc
    {
        /// <summary>
        ///     Creates a func that wraps a constructor for <typeparamref name="TResult" /> that all the given parameter types
        ///     (in order).
        /// </summary>
        public static Func<T1, TResult> From<T1, TResult>()
        {
            Contract.Ensures(Contract.Result<Func<T1, TResult>>() != null);

            return FindAndWrapConstructor(
                (body, args) => Expression.Lambda<Func<T1, TResult>>(body, args).Compile()
                );
        }

        /// <summary>
        ///     Creates a func that wraps a constructor for <typeparamref name="TResult" /> that all the given parameter types
        ///     (in order).
        /// </summary>
        public static Func<T1, T2, TResult> From<T1, T2, TResult>()
        {
            Contract.Ensures(Contract.Result<Func<T1, T2, TResult>>() != null);

            return FindAndWrapConstructor(
                (body, args) => Expression.Lambda<Func<T1, T2, TResult>>(body, args).Compile()
                );
        }

        /// <summary>
        ///     Creates a func that wraps a constructor for <typeparamref name="TResult" /> that all the given parameter types
        ///     (in order).
        /// </summary>
        public static Func<T1, T2, T3, TResult> From<T1, T2, T3, TResult>()
        {
            Contract.Ensures(Contract.Result<Func<T1, T2, T3, TResult>>() != null);

            return FindAndWrapConstructor(
                (body, args) => Expression.Lambda<Func<T1, T2, T3, TResult>>(body, args).Compile()
                );
        }

        /// <summary>
        ///     Creates a func that wraps a constructor for <typeparamref name="TResult" /> that all the given parameter types
        ///     (in order).
        /// </summary>
        public static Func<T1, T2, T3, T4, TResult> From<T1, T2, T3, T4, TResult>()
        {
            Contract.Ensures(Contract.Result<Func<T1, T2, T3, T4, TResult>>() != null);

            return FindAndWrapConstructor(
                (body, args) => Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(body, args).Compile()
                );
        }

        private static readonly ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

        private static TFunc FindAndWrapConstructor<TFunc>(Func<Expression, ParameterExpression[], TFunc> compile)
            where TFunc : class
        {
            var funcType = typeof(TFunc);
            var func = (TFunc) Cache.GetOrAdd(funcType, ft => FindAndWrapConstructor(ft, compile));
            Contract.Assume(func != null);
            return func;
        }

        private static TFunc FindAndWrapConstructor<TFunc>(Type funcType, Func<Expression, ParameterExpression[], TFunc> compile)
        {
            Contract.Requires(funcType != null);
            Contract.Requires(compile != null);
            Contract.Ensures(Contract.Result<TFunc>() != null);

            var typeArgs = funcType.GenericTypeArguments;
            Contract.Assume((typeArgs != null) && (typeArgs.Length > 0));
            Contract.Assume(Contract.ForAll(typeArgs, a => a != null));

            var parameters = typeArgs.Take(typeArgs.Length - 1).ToArray();
            var constructedType = typeArgs[typeArgs.Length - 1];

            var constructors = constructedType.GetConstructors();
            Contract.Assume(constructors.Any());

            var constructor = constructors.FirstOrDefault(
                c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameters)
            );
            Contract.Assume(constructor != null);

            var parameterExpressions = constructor.GetParameters().Select(
                p => Expression.Parameter(p.ParameterType, p.Name)
            ).ToArray();

            var lambda = compile(
                // ReSharper disable once CoVariantArrayConversion
                Expression.New(constructor, parameterExpressions),
                parameterExpressions
            );
            Contract.Assume(lambda != null);
            return lambda;
        }
    }
}