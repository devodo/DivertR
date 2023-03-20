using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// The Redirect class is used to create proxies of its target type and configure their behaviour.
    ///
    /// Proxy behaviour is configured by adding one or more <see cref="IVia"/>s to the Redirect.
    /// The inserted Vias are applied to all proxies created by the Redirect.
    /// Vias can be added or removed from the Redirect at any time allowing the proxy behaviour to be changed dynamically at runtime.
    /// 
    /// When a proxy is created it can be given a reference to a root instance of its type and by default it forwards all its call to this root, i.e. when no Vias are configured on the Redirect.
    /// If a root instance is not provided the proxy will be created with a dummy root that provides default return values on its members.
    /// Optionally a proxy can also be created with a null root but in this case the proxy behaviour must be defined to handle any call received else a <see cref="DiverterNullRootException"/> will be thrown.
    /// </summary>
    public interface IRedirect
    {
        /// <summary>
        /// The Redirect identifier that is a composite of the target type and an optional <see langword="string"/> group name.
        /// </summary>
        RedirectId RedirectId { get; }
        
        /// <summary>
        /// The <see cref="IRedirectSet"/> of this Redirect. The RedirectSet contains a collection of Redirects each with a unique <see cref="RedirectId"/>.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// The the call <see cref="IRelay" />.
        /// </summary>
        IRelay Relay { get; }
        
        /// <summary>
        /// The Redirect's <see cref="IRedirectRepository" /> for storing and managing the configuration that determines proxy behaviour.
        /// </summary>
        IRedirectRepository RedirectRepository { get; }

        /// <summary>
        /// Creates a Redirect proxy object of the Redirect's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Redirect target type.</exception>
        /// <returns>The proxy instance.</returns>
        object Proxy(object? root);
        
        /// <summary>
        /// Creates a Redirect proxy object of the Redirect's target type with no provided root instance.
        /// </summary>
        /// <param name="withDummyRoot">Specifies if the proxy should be created with a dummy root or not.</param>
        /// <returns>The proxy instance.</returns>
        object Proxy(bool withDummyRoot);
        
        /// <summary>
        /// Creates a Redirect proxy object of the Redirect's target type with no provided root instance.
        /// The proxy is created with a dummy root or not as configured by the Redirect's <see cref="DiverterSettings.DefaultWithDummyRoot" />.
        /// </summary>
        /// <returns>The proxy instance.</returns>
        object Proxy();
        
        /// <summary>
        /// Inserts an <see cref="IVia"/> into this Redirect.
        /// </summary>
        /// <param name="via">The Via instance.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Redirect instance.</returns>
        IRedirect Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Reset the Redirect's <see cref="IRedirectRepository" /> to its initial state.
        /// </summary>
        /// <returns>This Redirect instance.</returns>
        IRedirect Reset();

        /// <summary>
        /// Set strict mode on the Redirect.
        /// If strict is enabled and a call to its proxies does not match a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Redirect instance.</returns>
        IRedirect Strict(bool? isStrict = true);
    }
    
    /// <summary>
    /// Strongly typed generic extension of the <see cref="IRedirect"/> interface with target type defined.
    /// </summary>
    /// <typeparam name="TTarget">The proxy target type.</typeparam>
    public interface IRedirect<TTarget> : IRedirect where TTarget : class?
    {
        /// <summary>
        /// The the call <see cref="IRelay{TTarget}" />.
        /// </summary>
        new IRelay<TTarget> Relay { get; }

        /// <summary>
        /// Creates a Redirect proxy instance of the Redirect's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        TTarget Proxy(TTarget? root);
        
        /// <summary>
        /// Creates a Redirect proxy instance of the Redirect's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Redirect target type.</exception>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy(object? root);
        
        /// <summary>
        /// Creates a Redirect proxy instance of the Redirect's target type with no provided root instance.
        /// </summary>
        /// <param name="withDummyRoot">Specifies if the proxy should be created with a dummy root or not.</param>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy(bool withDummyRoot);

        /// <summary>
        /// Creates a Redirect proxy instance of the Redirect's target type with no provided root instance.
        /// The proxy is created with a dummy root or not as configured by the Redirect's <see cref="DiverterSettings.DefaultWithDummyRoot" />.
        /// </summary>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy();
        
        /// <summary>
        /// Insert a <see cref="IVia"/> into this Redirect.
        /// </summary>
        /// <param name="via">The Via instance to insert.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Redirect instance.</returns>
        new IRedirect<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Reset the Redirect's <see cref="IRedirectRepository" /> to its initial state.
        /// </summary>
        /// <returns>This Redirect instance.</returns>
        new IRedirect<TTarget> Reset();
        
        /// <summary>
        /// Set strict mode on the Redirect.
        /// If strict is enabled and a call to its proxies does not hit a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Redirect instance.</returns>
        new IRedirect<TTarget> Strict(bool? isStrict = true);

        /// <summary>
        /// Inserts a retarget <see cref="IVia"/> with no call constraints (therefore all calls will be matched and retargeted).
        /// </summary>
        /// <param name="target">The target instance to retarget calls to.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Redirect instance.</returns>
        IRedirect<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Inserts a record Via that captures incoming calls from all proxies.
        /// By default record Redirects are configured to not satisfy strict calls if strict mode is enabled.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An <see cref="IRecordStream{TTarget}"/> instance for retrieving and iterating the recorded calls.</returns>
        IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Creates an <see cref="IRedirectUpdater{TTarget}"/> instance for updating this Redirect. />
        /// </summary>
        /// <param name="callConstraint">Optional call constraint <see cref="ICallConstraint"/>.</param>
        /// <returns>The updater instance.</returns>
        IRedirectUpdater<TTarget> To(ICallConstraint? callConstraint = null);

        /// <summary>
        /// Creates an <see cref="IFuncRedirectUpdater{TTarget,TReturn}"/> instance for updating this Redirect for calls matching the <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The updater instance.</returns>
        IFuncRedirectUpdater<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression);
        
        /// <summary>
        /// Creates an <see cref="IActionRedirectUpdater{TTarget}"/> instance for updating this Redirect for void calls matching the <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The updater instance.</returns>
        IActionRedirectUpdater<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates an <see cref="IActionRedirectUpdater{TTarget}"/> instance for updating this Redirect for setter calls to matching the property <paramref name="memberExpression"/> expression
        /// and setter value <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">Optional constraint expression on the setter input argument. If null, the constraint defaults to match any value</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The updater instance.</returns>
        IActionRedirectUpdater<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null);
    }
}
