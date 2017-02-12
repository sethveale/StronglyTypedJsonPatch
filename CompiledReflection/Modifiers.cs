using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace CompiledReflection
{
    /// <summary>
    ///     Helper for modifying (public) fields and properties of objects (i.e. not static members)
    /// </summary>
    public class Modifiers<T>
    {
        // The static member in generic is entirely intentional
        // ReSharper disable StaticMemberInGenericType

        private static readonly ConcurrentDictionary<string, object> ActionCache =
            new ConcurrentDictionary<string, object>();

        private static readonly ConcurrentDictionary<string, Expression> LambdaCache =
            new ConcurrentDictionary<string, Expression>();

        // ReSharper restore StaticMemberInGenericType

        /// <summary>
        ///     Provides an modifier to a member as a action
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Action<T, TValue> AsAction<TValue>(string name)
        {
            return (Action<T, TValue>) ActionCache.GetOrAdd(name, _ => AsLambda<TValue>(name).Compile());
        }

        /// <summary>
        ///     Provides an modifier to a member as a lambda
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Expression<Action<T, TValue>> AsLambda<TValue>(string name)
        {
            return (Expression<Action<T, TValue>>) LambdaCache.GetOrAdd(name, MakeLambda<TValue>(name));
        }

        private static Expression<Action<T, TValue>> MakeLambda<TValue>(string name)
        {
            var propertyOrField = Values<T>.AsMember<TValue>(name);
            var member = (MemberInfo) propertyOrField.Item1 ?? propertyOrField.Item2;
            Contract.Assume(member is PropertyInfo || member is FieldInfo);

            var objParam = Expression.Parameter(typeof(T));
            Contract.Assume(objParam != null);

            var valueParam = Expression.Parameter(typeof(TValue));

            return Expression.Lambda<Action<T, TValue>>(
                Expression.Assign(Expression.MakeMemberAccess(objParam, member), valueParam),
                objParam,
                valueParam
            );
        }
    }
}