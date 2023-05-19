using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// The Redirect interface is used to create proxies of its target type. It also includes a fluent interface for configuring proxy behaviour and call handling.
    /// 
    /// When creating a proxy a root instance is provided and by default all calls are forwarded to this.
    /// If a root instance is not provided the proxy will be created with a dummy root that provides default return values on its members.
    /// A proxy can be created with a null root but in this case if no handlers are configured for a call a <see cref="DiverterNullRootException"/> will be thrown.
    ///
    /// Proxy behaviour is configured by the fluent interface by adding <see cref="IVia"/> call handlers to the Redirect.
    /// An inserted <see cref="IVia"/> is applied to all proxies created by the Redirect.
    /// Vias can be added or removed from the Redirect at any time allowing the proxy behaviour to be changed dynamically at runtime.
    /// </summary>
    public interface IRedirect
    {
        /// <summary>
        /// The Redirect identifier.
        /// </summary>
        RedirectId RedirectId { get; }
        
        /// <summary>
        /// The <see cref="IRedirectSet"/> this Redirect belongs to.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// The call <see cref="IRelay" />.
        /// </summary>
        IRelay Relay { get; }
        
        /// <summary>
        /// The repository for storing and managing configurations that determine proxy behaviour.
        /// </summary>
        IRedirectRepository RedirectRepository { get; }

        /// <summary>
        /// Creates an <see cref="IRedirect"/> proxy object of the Redirect's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Redirect target type.</exception>
        /// <returns>The created proxy.</returns>
        object Proxy(object? root);
        
        /// <summary>
        /// Creates an <see cref="IRedirect"/> proxy object of the Redirect's target type with no root instance provided.
        /// </summary>
        /// <param name="withDummyRoot">
        /// Specifies if the proxy should be created with a dummy root or not.
        /// Dummy roots are created using the <see cref="Dummy.IDummyFactory"/> configured at <see cref="DiverterSettings.DummyFactory"/>.
        /// </param>
        /// <returns>The created proxy.</returns>
        object Proxy(bool withDummyRoot = true);

        /// <summary>
        /// Inserts an <see cref="IVia"/> into this Redirect.
        /// </summary>
        /// <param name="via">The Via instance.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirect Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Resets proxy configuration.
        /// All non-persistent <see cref="IVia"/>s are removed from the Redirect.
        /// </summary>
        /// <returns>This instance.</returns>
        IRedirect Reset();

        /// <summary>
        /// Sets strict mode on the Redirect.
        /// If strict is enabled and a call to its proxies does not match a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This instance.</returns>
        IRedirect Strict(bool? isStrict = true);
        
        /// <summary>
        /// Adds an <see cref="IRedirect{TReturn}"/> and proxies all calls with return type <typeparamref name="TReturn"/> via the added redirect.
        /// The added <see cref="IRedirect{TReturn}"/> has default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <returns>The added <see cref="IRedirect{TReturn}"/>.</returns>
        IRedirect<TReturn> ViaRedirect<TReturn>(Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?;
        
        /// <summary>
        /// Adds an <see cref="IRedirect{TReturn}"/> and proxies all calls with return type <typeparamref name="TReturn"/> via the added redirect.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <returns>The added <see cref="IRedirect{TReturn}"/>.</returns>
        IRedirect<TReturn> ViaRedirect<TReturn>(string? name, Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?;
        
        /// <summary>
        /// Add a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <returns>This instance.</returns>
        IRedirect Decorate<TReturn>(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
    }
    
    /// <summary>
    /// A strongly typed extension of the <see cref="IRedirect"/> interface with a generic type defining its proxy target type.
    /// </summary>
    /// <typeparam name="TTarget">The proxy target type.</typeparam>
    public interface IRedirect<TTarget> : IRedirect where TTarget : class?
    {
        /// <summary>
        /// The the call <see cref="IRelay{TTarget}" />.
        /// </summary>
        new IRelay<TTarget> Relay { get; }

        /// <summary>
        /// Creates an <see cref="IRedirect{TTarget}"/> proxy.
        /// </summary>
        /// <param name="root">The root instance the proxy will relay calls to.</param>
        /// <returns>The created proxy.</returns>
        [return: NotNull]
        TTarget Proxy(TTarget? root);
        
        /// <summary>
        /// Creates an <see cref="IRedirect{TTarget}"/> proxy.
        /// </summary>
        /// <param name="root">The root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Redirect target type.</exception>
        /// <returns>The created proxy.</returns>
        [return: NotNull]
        new TTarget Proxy(object? root);
        
        /// <summary>
        /// Creates an <see cref="IRedirect{TTarget}"/> proxy with no root instance provided.
        /// </summary>
        /// <param name="withDummyRoot">
        /// Specifies if the proxy should be created with a dummy root or not.
        /// Dummy roots are created using the <see cref="Dummy.IDummyFactory"/> configured at <see cref="DiverterSettings.DummyFactory"/>.
        /// </param>
        /// <returns>The created proxy.</returns>
        [return: NotNull]
        new TTarget Proxy(bool withDummyRoot = true);

        /// <summary>
        /// Insert a <see cref="IVia"/> into this Redirect.
        /// </summary>
        /// <param name="via">The Via instance to insert.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirect<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Resets proxy configuration.
        /// All non-persistent <see cref="IVia"/>s are removed from the Redirect.
        /// </summary>
        /// <returns>This instance.</returns>
        new IRedirect<TTarget> Reset();
        
        /// <summary>
        /// Set strict mode on the Redirect.
        /// If strict is enabled and a call to its proxies does not hit a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This instance.</returns>
        new IRedirect<TTarget> Strict(bool? isStrict = true);

        /// <summary>
        /// Retarget all <see cref="IRedirect{TTarget}"/> calls to the given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target instance to retarget calls to.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirect<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Records all proxy calls to the returned stream.
        /// An <see cref="IVia"/> is inserted that captures incoming calls from all proxies.
        /// This is configured to not satisfy strict calls.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls.</returns>
        IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Add a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <returns>This instance.</returns>
        new IRedirect<TTarget> Decorate<TReturn>(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Creates and returns a builder for configuring proxy behaviour with a filter allowing calls matching the given <paramref name="callConstraint"/>.
        /// If <paramref name="callConstraint"/> is null then all calls are matched.
        /// </summary>
        /// <param name="callConstraint">Optional call constraint <see cref="ICallConstraint"/>.</param>
        /// <returns>The created builder.</returns>
        IRedirectToBuilder<TTarget> To(ICallConstraint? callConstraint = null);

        /// <summary>
        /// Creates and returns a builder for configuring proxy behaviour with a filter allowing calls matching the given <paramref name="constraintExpression"/>.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The created builder.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression);
        
        /// <summary>
        /// Creates and returns a builder for configuring proxy behaviour with a filter allowing void returning calls matching the given <paramref name="constraintExpression"/>.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The created builder.</returns>
        IRedirectToActionBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates and returns a builder for configuring proxy behaviour with a filter allowing property setter calls matching the given <paramref name="memberExpression"/> and <paramref name="constraintExpression"/>.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">Optional constraint expression on the setter input argument. If null, the constraint defaults to match any value</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The created builder.</returns>
        IRedirectToActionBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null);
    }
}
