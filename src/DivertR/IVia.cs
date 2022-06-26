using System;
using System.Linq.Expressions;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// A Via is the DivertR unit associated with a type. The Via is used to create proxies of its type and to configure the proxy behaviour.
    /// </summary>
    public interface IVia
    {
        /// <summary>
        /// The Via identifier consisting of its type and optional name label.
        /// </summary>
        ViaId ViaId { get; }
        
        /// <summary>
        /// The <see cref="IViaSet"/> of this Via
        /// </summary>
        IViaSet ViaSet { get; }
        
        /// <summary>
        /// Reference to the Via <see cref="IRelay" /> chain of responsibility call pipeline.
        /// </summary>
        IRelay Relay { get; }
        
        /// <summary>
        /// Retrieve the current proxy redirect configuration.
        /// </summary>
        IRedirectRepository RedirectRepository { get; }

        /// <summary>
        /// Create a Via proxy object without needing to specify the compile time Via type.
        /// </summary>
        /// <param name="root">Optional root instance to proxy calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Via type.</exception>
        /// <returns>The proxy instance.</returns>
        object ProxyObject(object? root);
        
        object ProxyObject(bool createDummyRoot = true);

        /// <summary>
        /// Reset the Via.
        /// </summary>
        /// <returns>The current <see cref="IVia"/> instance.</returns>
        IVia Reset();

        /// <summary>
        /// Set strict mode. If no argument, strict mode is enabled by default.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>The current <see cref="IVia"/> instance.</returns>
        IVia Strict(bool? isStrict = true);
    }
    
    /// <summary>
    /// Strongly typed Via class used to create DivertR proxies of its type and to configure the proxy behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The Via type.</typeparam>
    public interface IVia<TTarget> : IVia where TTarget : class
    {
        /// <summary>
        /// Reference to the Via <see cref="IRelay{TTarget}" /> chain of responsibility call pipeline.
        /// </summary>
        new IRelay<TTarget> Relay { get; }

        /// <summary>
        /// Create a Via proxy instance.
        /// </summary>
        /// <param name="root">Optional root instance to proxy calls to.</param>
        /// <returns>The proxy instance.</returns>
        TTarget Proxy(TTarget? root);
        
        TTarget Proxy(bool createDummyRoot = true);

        /// <summary>
        /// Reset the Via <see cref="IRedirectRepository" />.
        /// </summary>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        new IVia<TTarget> Reset();
        
        /// <summary>
        /// Enable strict mode.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        new IVia<TTarget> Strict(bool? isStrict = true);

        /// <summary>
        /// Create and insert a redirect (with no <see cref="ICallConstraint{TTarget}"/>) to the given <paramref name="target"/>
        /// into the Via <see cref="IRedirectRepository" />.
        /// </summary>
        /// <param name="target">The target instance to redirect call to.</param>
        /// <param name="optionsAction">An optional builder action for configuring redirect options.</param>
        /// <returns>The Redirect builder instance.</returns>
        IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        /// <summary>
        /// Inserts a redirect that captures incoming calls from all proxies.
        /// By default record redirects are configured to not satisfy strict calls if strict mode is enabled.
        /// </summary>
        /// <param name="optionsAction">An optional builder action for configuring redirect options.</param>
        /// <returns>An <see cref="IRecordStream{TTarget}"/> reference for retrieving and iterating the recorded calls.</returns>
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
        /// <summary>
        /// Creates a Redirect builder. />
        /// </summary>
        /// <param name="callConstraint">Optional redirect <see cref="ICallConstraint{TTarget}"/>.</param>
        /// <returns>The Redirect builder instance.</returns>
        /// 
        IViaBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);
        
        // Delegate required to coerce C# to allow the struct return type constrained To method overload below
        delegate TResult StructReturnFunc<in T, out TResult>(T args) where TResult : struct;

        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning struct <typeparam name="TReturn" />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<StructReturnFunc<TTarget, TReturn>> constraintExpression) where TReturn : struct;

        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning class <typeparam name="TReturn" />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IClassFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression) where TReturn : class;
        
        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning void />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The Redirect builder instance.</returns>
        IActionViaBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a property setter member of <typeparamref name="TTarget"/> />.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">The call constraint expression for the input value of the setter.</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IActionViaBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>> constraintExpression);
    }
}