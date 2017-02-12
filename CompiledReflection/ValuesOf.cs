using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CompiledReflection
{
    /// <summary>
    ///     Helper for accessing (public) fields and properties of objects (i.e. not static members)
    /// </summary>
    public static class ValuesOf<T>
    {
        // The static member in generic is entirely intentional
        // ReSharper disable StaticMemberInGenericType

        private static readonly ConcurrentDictionary<string, object> FuncCache =
            new ConcurrentDictionary<string, object>();

        private static readonly ConcurrentDictionary<string, Expression> LambdaCache =
            new ConcurrentDictionary<string, Expression>();

        // ReSharper restore StaticMemberInGenericType

        /// <summary>
        ///     Provides an accessor to a member as a func
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Func<T, TValue> AsFunc<TValue>(string name)
        {
            return (Func<T, TValue>) FuncCache.GetOrAdd(name, _ => AsLambda<TValue>(name).Compile());
        }

        /// <summary>
        ///     Provides an accessor to a member as a lambda
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Expression<Func<T, TValue>> AsLambda<TValue>(string name)
        {
            return (Expression<Func<T, TValue>>) LambdaCache.GetOrAdd(name, MakeLambda<TValue>(name));
        }

        private static Expression<Func<T, TValue>> MakeLambda<TValue>(string name)
        {
            var propertyOrField = typeof(T).GetMember(name)
                .Select(m => Tuple.Create(m as PropertyInfo, m as FieldInfo))
                .FirstOrDefault(t =>
                        ((t.Item1 != null) && (t.Item1.PropertyType == typeof(TValue))) ||
                        ((t.Item2 != null) && (t.Item2.FieldType == typeof(TValue)))
                );

            if (propertyOrField == null)
                throw new MissingMemberException(typeof(T).Name, name);

            var objParam = Expression.Parameter(typeof(T));
            Contract.Assume(objParam != null);

            var member = (MemberInfo) propertyOrField.Item1 ?? propertyOrField.Item2;
            Contract.Assume(member != null);

            return Expression.Lambda<Func<T, TValue>>(
                Expression.MakeMemberAccess(objParam, member),
                objParam
            );
        }
    }
}