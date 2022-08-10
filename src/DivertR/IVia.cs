using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// Vias are used to create DivertR proxies and configure proxy behaviour.
    /// A Via instance has a target type and this is the type of the proxies it creates.
    /// When a proxy is created it can be given a root instance of its type. By default the proxy forwards all its calls to this root instance.
    /// Proxy behaviour is configured by inserting one or more <see cref="IRedirect" />s. Redirects can be added or removed at any time.
    /// A Via can create multiple proxies and its configured redirects are applied to all its proxies.
    /// </summary>
    public interface IVia
    {
        /// <summary>
        /// The Via identifier consisting of its type and optional name label.
        /// </summary>
        ViaId ViaId { get; }
        
        /// <summary>
        /// The <see cref="IViaSet"/> of this Via. The ViaSet contains a collection of Vias each with a unique ViaId.
        /// </summary>
        IViaSet ViaSet { get; }
        
        /// <summary>
        /// The Via <see cref="IRelay" /> that is used to access the chain of responsibility redirect pipeline of proxy calls.
        /// </summary>
        IRelay Relay { get; }
        
        /// <summary>
        /// The Via's <see cref="IRedirectRepository" /> for storing and managing the redirect configuration.
        /// </summary>
        IRedirectRepository RedirectRepository { get; }

        /// <summary>
        /// Creates a Via proxy object of the Via's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Via target type.</exception>
        /// <returns>The proxy instance.</returns>
        object Proxy(object? root);
        
        /// <summary>
        /// Creates a Via proxy object of the Via's target type with no provided root instance.
        /// </summary>
        /// <param name="withDummyRoot">Specifies if the proxy should be created with a dummy root or not.</param>
        /// <returns>The proxy instance.</returns>
        object Proxy(bool withDummyRoot);
        
        /// <summary>
        /// Creates a Via proxy object of the Via's target type with no provided root instance.
        /// The proxy is created with a dummy root or not as configured by the Via's <see cref="DiverterSettings.DefaultWithDummyRoot" />.
        /// </summary>
        /// <returns>The proxy instance.</returns>
        object Proxy();
        
        /// <summary>
        /// Inserts a redirect into this Via.
        /// </summary>
        /// <param name="redirect">The redirect instance.</param>
        /// <param name="optionsAction">Optional redirect options builder action.</param>
        /// <returns>This Via instance.</returns>
        IVia Redirect(IRedirect redirect, Action<IRedirectOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Reset the Via <see cref="IRedirectRepository" />.
        /// </summary>
        /// <returns>The current <see cref="IVia"/> instance.</returns>
        IVia Reset();

        /// <summary>
        /// Set strict mode on the Via.
        /// If strict is enabled on the Via and a call to its proxies does not hit a configured <see cref="IRedirect"/>
        /// then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Via instance.</returns>
        IVia Strict(bool? isStrict = true);
    }
    
    /// <summary>
    /// Strongly typed Via class used to create DivertR proxies of its type and to configure the proxy behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The Via type.</typeparam>
    public interface IVia<TTarget> : IVia where TTarget : class?
    {
        /// <summary>
        /// Reference to the Via <see cref="IRelay{TTarget}" /> chain of responsibility call pipeline.
        /// </summary>
        new IRelay<TTarget> Relay { get; }

        /// <summary>
        /// Creates a Via proxy instance of the Via's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        TTarget Proxy(TTarget? root);
        
        /// <summary>
        /// Creates a Via proxy instance of the Via's target type.
        /// </summary>
        /// <param name="root">Optional root instance the proxy will relay calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="root"/> is not the Via target type.</exception>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy(object? root);
        
        /// <summary>
        /// Creates a Via proxy instance of the Via's target type with no provided root instance.
        /// </summary>
        /// <param name="withDummyRoot">Specifies if the proxy should be created with a dummy root or not.</param>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy(bool withDummyRoot);
        
        /// <summary>
        /// Creates a Via proxy instance of the Via's target type with no provided root instance.
        /// The proxy is created with a dummy root or not as configured by the Via's <see cref="DiverterSettings.DefaultWithDummyRoot" />.
        /// </summary>
        /// <returns>The proxy instance.</returns>
        [return: NotNull]
        new TTarget Proxy();
        
        /// <summary>
        /// Insert a redirect into this Via.
        /// </summary>
        /// <param name="redirect">The redirect instance.</param>
        /// <param name="optionsAction">Optional redirect options builder action.</param>
        /// <returns>This Via instance.</returns>
        new IVia<TTarget> Redirect(IRedirect redirect, Action<IRedirectOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Reset the Via <see cref="IRedirectRepository" />.
        /// </summary>
        /// <returns>This Via instance.</returns>
        new IVia<TTarget> Reset();
        
        /// <summary>
        /// Set strict mode on the Via.
        /// If strict is enabled on the Via and a call to its proxies does not hit a configured <see cref="IRedirect"/>
        /// then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Via instance.</returns>
        new IVia<TTarget> Strict(bool? isStrict = true);

        /// <summary>
        /// Inserts a retarget redirect with no call constraints (therefore all calls will be redirected).
        /// </summary>
        /// <param name="target">The target instance to retarget calls to.</param>
        /// <param name="optionsAction">An optional builder action for configuring redirect options.</param>
        /// <returns>This Via instance.</returns>
        IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Inserts a redirect that captures incoming calls from all proxies.
        /// By default record redirects are configured to not satisfy strict calls if strict mode is enabled.
        /// </summary>
        /// <param name="optionsAction">An optional builder action for configuring redirect options.</param>
        /// <returns>An <see cref="IRecordStream{TTarget}"/> reference for retrieving and iterating the recorded calls.</returns>
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Creates a Redirect builder. />
        /// </summary>
        /// <param name="callConstraint">Optional call constraint <see cref="ICallConstraint{TTarget}"/>.</param>
        /// <returns>The builder instance.</returns>
        /// 
        IViaBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);

        /// <summary>
        /// Creates a builder that can be used to insert redirects for calls matching the <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The builder instance.</returns>
        IFuncViaBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression);
        
        /// <summary>
        /// Creates a builder that can be used to insert redirects for void calls matching the <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The builder instance.</returns>
        IActionViaBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates a builder that can be used to insert redirects for setter calls to matching the property <paramref name="memberExpression"/> expression
        /// and the <paramref name="constraintExpression"/> expression.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">Optional constraint expression on the setter input argument. If null, the constraint defaults to match any value</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IActionViaBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null);
    }
}
