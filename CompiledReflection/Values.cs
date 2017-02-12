using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace CompiledReflection
{
    /// <summary>
    ///     Wraps up ways to reflect on Properties and Fields for a type
    /// </summary>
    public static class Values<T>
    {
        /// <summary>
        ///     Finds the member matching a name and type
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Tuple<PropertyInfo, FieldInfo> AsMember<TValue>(string name)
        {
            Contract.Ensures(Contract.Result<Tuple<PropertyInfo, FieldInfo>>() != null);
            Contract.Ensures(Contract.Result<Tuple<PropertyInfo, FieldInfo>>().Item1 != null ||
                             Contract.Result<Tuple<PropertyInfo, FieldInfo>>().Item2 != null);

            var result = AsMember(name);

            if (result.Item1 != null && result.Item1.PropertyType != typeof(TValue) ||
                result.Item2 != null && result.Item2.FieldType != typeof(TValue))
                throw new MissingMemberException(typeof(TValue).Name, name);

            return result;
        }

        /// <summary>
        ///     Finds the member matching a name, regardless of type.
        /// </summary>
        /// <exception cref="MissingMemberException" />
        public static Tuple<PropertyInfo, FieldInfo> AsMember(string name)
        {
            Contract.Ensures(Contract.Result<Tuple<PropertyInfo, FieldInfo>>() != null);
            Contract.Ensures(Contract.Result<Tuple<PropertyInfo, FieldInfo>>().Item1 != null ||
                             Contract.Result<Tuple<PropertyInfo, FieldInfo>>().Item2 != null);

            var result = typeof(T).GetMember(name)
                .Select(m => Tuple.Create(m as PropertyInfo, m as FieldInfo))
                .FirstOrDefault(t => t.Item1 != null || t.Item2 != null);

            if (result == null)
                throw new MissingMemberException(typeof(T).Name, name);

            Contract.Assume(result.Item1 != null || result.Item2 != null);
            return result;
        }
    }
}