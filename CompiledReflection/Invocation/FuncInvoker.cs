using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace CompiledReflection.Invocation
{
    /// <summary>
    ///     Untyped base invoker for methods and properties that can be represented as a Func
    /// </summary>
    public abstract class FuncInvoker
    {
        /// <summary>
        ///     Creates a FuncInvoker of the appropriate signature for the <paramref name="method" />
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     if the <paramref name="method" /> isn't a valid <see cref="Func{TResult}" />
        /// </exception>
        public static FuncInvoker Create(MethodInfo method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<FuncInvoker>() != null);

            var types = ToFuncInvokerTypes(method);

            var memberInvokerType = types.Item1;
            Contract.Assert(memberInvokerType != null);

            var func = Delegate.CreateDelegate(types.Item2, null, method);
            return (FuncInvoker) Activator.CreateInstance(memberInvokerType, func);
        }

        /// <summary>
        ///     Resolves the <see cref="FuncInvoker" /> subtype and <see cref="Func{TResult}" /> type for the method
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     if the <paramref name="method" /> isn't a valid <see cref="Func{TResult}" />
        /// </exception>
        public static Tuple<Type, Type> ToFuncInvokerTypes(MethodInfo method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<Tuple<Type, Type>>() != null);
            Contract.Ensures(Contract.Result<Tuple<Type, Type>>().Item1 != null &&
                             Contract.Result<Tuple<Type, Type>>().Item1 != null);

            var parameters = method.GetParameters();
            if (parameters.Any(p => p == null || p.IsRetval || p.IsOut))
                throw new NotSupportedException("All parameters must be input only, out and ref don't work in a func");

            if (method.ReturnType == typeof(void))
                throw new NotSupportedException("Must have a return type to be a Func, use MemberActionInvoker instead");

            var typeParameters = method.IsStatic ? new Type[0] : new[] { method.DeclaringType };
            typeParameters = typeParameters.Concat(parameters.Select(p => p.ParameterType))
                .Concat(new[] { method.ReturnType })
                .ToArray();

            Type invokerType;
            Type funcType;

            switch (typeParameters.Length)
            {
                case 1:
                    invokerType = typeof(FuncInvoker<>);
                    funcType = typeof(Func<>);
                    break;

                case 2:
                    invokerType = typeof(FuncInvoker<,>);
                    funcType = typeof(Func<,>);
                    break;

                case 3:
                    invokerType = typeof(FuncInvoker<,,>);
                    funcType = typeof(Func<,,>);
                    break;

                case 4:
                    invokerType = typeof(FuncInvoker<,,,>);
                    funcType = typeof(Func<,,,>);
                    break;

                default:
                    throw new NotSupportedException("Methods with more than 3 parameters are not supported");
            }

            var result = Tuple.Create(
                invokerType.MakeType(typeParameters),
                funcType.MakeType(typeParameters)
            );
            Contract.Assume(result != null && result.Item1 != null && result.Item2 != null);
            return result;
        }

        /// <summary>
        ///     Invokes the delegate with the args, which must match the signature.
        /// </summary>
        public abstract object Invoke(params object[] args);
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Func
    /// </summary>
    public class FuncInvoker<TResult> : FuncInvoker
    {
        private readonly Func<TResult> _func;

        /// <param name="func">the func to be invoked</param>
        public FuncInvoker(Func<TResult> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override object Invoke(params object[] args)
        {
            return _func();
        }
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Func
    /// </summary>
    public class FuncInvoker<T0, TResult> : FuncInvoker
    {
        private readonly Func<T0, TResult> _func;

        /// <param name="func">the func to be invoked</param>
        public FuncInvoker(Func<T0, TResult> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override object Invoke(params object[] args)
        {
            return _func((T0) args[0]);
        }
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Func
    /// </summary>
    public class FuncInvoker<T0, T1, TResult> : FuncInvoker
    {
        private readonly Func<T0, T1, TResult> _func;

        /// <param name="func">the func to be invoked</param>
        public FuncInvoker(Func<T0, T1, TResult> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override object Invoke(params object[] args)
        {
            return _func((T0) args[0], (T1) args[1]);
        }
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Func
    /// </summary>
    public class FuncInvoker<T0, T1, T2, TResult> : FuncInvoker
    {
        private readonly Func<T0, T1, T2, TResult> _func;

        /// <param name="func">the func to be invoked</param>
        public FuncInvoker(Func<T0, T1, T2, TResult> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override object Invoke(params object[] args)
        {
            return _func((T0) args[0], (T1) args[1], (T2) args[2]);
        }
    }
}