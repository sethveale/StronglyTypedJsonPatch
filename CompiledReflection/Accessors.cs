﻿using System;
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
    public static class Accessors<T>
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
            var propertyOrField = Values<T>.AsMember<TValue>(name);
            var member = (MemberInfo) propertyOrField.Item1 ?? propertyOrField.Item2;
            Contract.Assume(member is PropertyInfo || member is FieldInfo);

            var objParam = Expression.Parameter(typeof(T));
            Contract.Assume(objParam != null);

            return Expression.Lambda<Func<T, TValue>>(
                Expression.MakeMemberAccess(objParam, member),
                objParam
            );
        }
    }
}