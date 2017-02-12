using System;
using System.Linq.Expressions;

namespace CompiledReflection
{
    /// <summary>
    ///     Converts to and from dereference <see cref="Func{TResult}" />s and <see cref="Expression{TDelegate}" />s
    /// </summary>
    public static class DeReference
    {
        /// <summary>
        ///     Converts a sequence of (public) property and field dereferences into a lambda.
        /// </summary>
        /// <typeparam name="TObject">Type of the origin object</typeparam>
        /// <typeparam name="TResult">Type of the final dereference</typeparam>
        /// <param name="deReferenceSequence">The sequential (left to right) names of the properties and fields</param>
        /// <exception cref="MissingMemberException" />
        public static Expression<Func<TObject, TResult>> AsLambda<TObject, TResult>(params string[] deReferenceSequence)
        {
            return null;
        }
    }
}