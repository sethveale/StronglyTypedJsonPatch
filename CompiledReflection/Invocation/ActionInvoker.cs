using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace CompiledReflection.Invocation
{
    /// <summary>
    ///     Untyped base invoker for methods and properties that can be represented as a Action
    /// </summary>
    public abstract class ActionInvoker
    {
        /// <summary>
        ///     Creates a ActionInvoker of the appropriate signature for the <paramref name="method" />
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     if the <paramref name="method" /> isn't a valid <see cref="Action{TResult}" />
        /// </exception>
        public static ActionInvoker Create(MethodInfo method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<ActionInvoker>() != null);

            var types = ToActionInvokerTypes(method);

            var memberInvokerType = types.Item1;
            Contract.Assert(memberInvokerType != null);

            var action = Delegate.CreateDelegate(types.Item2, null, method);
            return (ActionInvoker)Activator.CreateInstance(memberInvokerType, action);
        }

        /// <summary>
        ///     Resolves the <see cref="CompiledReflection.Invocation.ActionInvoker" /> subtype and <see cref="Action{TResult}" /> type for the method
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     if the <paramref name="method" /> isn't a valid <see cref="Action{TResult}" />
        /// </exception>
        public static Tuple<Type, Type> ToActionInvokerTypes(MethodInfo method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<Tuple<Type, Type>>() != null);
            Contract.Ensures(Contract.Result<Tuple<Type, Type>>().Item1 != null &&
                             Contract.Result<Tuple<Type, Type>>().Item1 != null);

            var parameters = method.GetParameters();
            if (parameters.Any(p => p == null || p.IsRetval || p.IsOut))
                throw new NotSupportedException("All parameters must be input only, out and ref don't work in a action");

            if (method.ReturnType != typeof(void))
                throw new NotSupportedException("Must not have a return type to be a Action, use FuncInvoker instead");

            var typeParameters = method.IsStatic ? new Type[0] : new[] { method.DeclaringType };
            typeParameters = typeParameters.Concat(parameters.Select(p => p.ParameterType)).ToArray();

            Type invokerType;
            Type actionType;

            switch (typeParameters.Length)
            {
                case 1:
                    invokerType = typeof(ActionInvoker<>);
                    actionType = typeof(Action<>);
                    break;

                case 2:
                    invokerType = typeof(ActionInvoker<,>);
                    actionType = typeof(Action<,>);
                    break;

                case 3:
                    invokerType = typeof(ActionInvoker<,,>);
                    actionType = typeof(Action<,,>);
                    break;

                default:
                    throw new NotSupportedException("Methods with more than 3 parameters are not supported");
            }

            var result = Tuple.Create(
                invokerType.MakeType(typeParameters),
                actionType.MakeType(typeParameters)
            );
            Contract.Assume(result != null && result.Item1 != null && result.Item2 != null);
            return result;
        }

        /// <summary>
        ///     Invokes the delegate with the args, which must match the signature.
        /// </summary>
        public abstract void Invoke(params object[] args);
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Action
    /// </summary>
    public class ActionInvoker<T0> : ActionInvoker
    {
        private readonly Action<T0> _action;

        /// <param name="action">the action to be invoked</param>
        public ActionInvoker(Action<T0> action)
        {
            Contract.Requires(action != null);
            _action = action;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override void Invoke(params object[] args)
        {
            _action((T0)args[0]);
        }
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Action
    /// </summary>
    public class ActionInvoker<T0, T1> : ActionInvoker
    {
        private readonly Action<T0, T1> _action;

        /// <param name="action">the action to be invoked</param>
        public ActionInvoker(Action<T0, T1> action)
        {
            Contract.Requires(action != null);
            _action = action;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override void Invoke(params object[] args)
        {
            _action((T0)args[0], (T1)args[1]);
        }
    }

    /// <summary>
    ///     Invoker for methods and properties that can be represented as a Action
    /// </summary>
    public class ActionInvoker<T0, T1, T2> : ActionInvoker
    {
        private readonly Action<T0, T1, T2> _action;

        /// <param name="action">the action to be invoked</param>
        public ActionInvoker(Action<T0, T1, T2> action)
        {
            Contract.Requires(action != null);
            _action = action;
        }

        /// <inheritdoc />
        [ContractVerification(false)]
        public override void Invoke(params object[] args)
        {
            _action((T0)args[0], (T1)args[1], (T2)args[2]);
        }
    }
}
