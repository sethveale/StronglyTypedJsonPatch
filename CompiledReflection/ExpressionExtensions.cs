using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace CompiledReflection
{
    /// <summary>
    ///     Helpers for expressions
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Feeds the result of <paramref name="firstResult" /> into <paramref name="secondResult" /> to make a single lambda.
        /// </summary>
        /// <exception cref="NotSupportedException">if either expression is not a lambda</exception>
        public static Expression<Func<TOrigin, TSecondResult>> Concat<TOrigin, TFirstResult, TSecondResult>(
            this Expression<Func<TOrigin, TFirstResult>> firstResult,
            Expression<Func<TFirstResult, TSecondResult>> secondResult)
        {
            Contract.Requires(firstResult != null);
            Contract.Requires(secondResult != null);
            Contract.Ensures(Contract.Result<Expression<Func<TOrigin, TSecondResult>>>() != null);

            if (firstResult.NodeType != ExpressionType.Lambda || secondResult.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Only supports lambda expressions");

            var secondFromFirst = secondResult.Body.Substitute(e => e is ParameterExpression, firstResult.Body);
            Contract.Assume(secondFromFirst != null);

            return Expression.Lambda<Func<TOrigin, TSecondResult>>(secondFromFirst, firstResult.Parameters);
        }

        /// <summary>
        ///     Substitutes all <see cref="Expression" />s in <paramref name="expression" /> that match
        ///     <paramref name="shouldReplace" /> with <paramref name="substitution" />
        /// </summary>
        public static Expression Substitute(
            this Expression expression,
            Func<Expression, bool> shouldReplace,
            Expression substitution)
        {
            Contract.Requires(expression == null || shouldReplace != null);

            if (expression == null)
                return null;

            if (shouldReplace(expression))
                return substitution;

            var method = expression as MethodCallExpression;
            if (method != null)
                return method.Update(
                    method.Object.Substitute(shouldReplace, substitution),
                    method.Arguments.Select(e => e.Substitute(shouldReplace, substitution))
                );

            var binary = expression as BinaryExpression;
            if (binary != null)
                return binary.Update(
                    binary.Left.Substitute(shouldReplace, substitution),
                    binary.Conversion,
                    binary.Right.Substitute(shouldReplace, substitution)
                );

            var unary = expression as UnaryExpression;
            if (unary != null)
                return unary.Update(unary.Operand.Substitute(shouldReplace, substitution));

            var newUp = expression as NewExpression;
            if (newUp != null)
                return newUp.Update(
                    newUp.Arguments.Select(e => e.Substitute(shouldReplace, substitution))
                );

            var conditional = expression as ConditionalExpression;
            if (conditional != null)
                return conditional.Update(
                    conditional.Test.Substitute(shouldReplace, substitution),
                    conditional.IfTrue.Substitute(shouldReplace, substitution),
                    conditional.IfFalse.Substitute(shouldReplace, substitution)
                );

            var index = expression as IndexExpression;
            if (index != null)
            {
                var args = index.Arguments;
                Contract.Assume(args != null);
                return index.Update(
                    index.Object.Substitute(shouldReplace, substitution),
                    args.Select(e => e.Substitute(shouldReplace, substitution))
                );
            }

            var listInit = expression as ListInitExpression;
            if (listInit != null)
                return listInit.Update(
                    (NewExpression) listInit.NewExpression.Substitute(shouldReplace, substitution),
                    listInit.Initializers.Select(
                        e => e.Update(e.Arguments.Select(a => a.Substitute(shouldReplace, substitution)))
                    )
                );

            var member = expression as MemberExpression;
            if (member != null)
                return member.Update(member.Expression.Substitute(shouldReplace, substitution));

            var newArray = expression as NewArrayExpression;
            if (newArray != null)
                return newArray.Update(
                    newArray.Expressions.Select(e => e.Substitute(shouldReplace, substitution))
                );

            var memberInit = expression as MemberInitExpression;
            if (memberInit != null)
                return memberInit.Update(
                    (NewExpression) memberInit.NewExpression.Substitute(shouldReplace, substitution),
                    memberInit.Bindings
                );

            var switcher = expression as SwitchExpression;
            if (switcher != null)
            {
                var cases = switcher.Cases;
                Contract.Assume(cases != null);
                return switcher.Update(
                    switcher.SwitchValue.Substitute(shouldReplace, substitution),
                    cases.Select(c => c.Update(
                        c.TestValues.Select(e => e.Substitute(shouldReplace, substitution)),
                        c.Body.Substitute(shouldReplace, substitution)
                    )),
                    switcher.DefaultBody.Substitute(shouldReplace, substitution)
                );
            }

            var typeIs = expression as TypeBinaryExpression;
            if (typeIs != null)
                return typeIs.Update(typeIs.Expression.Substitute(shouldReplace, substitution));

            return expression;
        }
    }
}