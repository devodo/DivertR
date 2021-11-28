﻿using System;
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

        public IViaSet ViaSet { get; }
        
        /// <summary>
        /// Create a Via proxy object without needing to specify the compile time Via type.
        /// </summary>
        /// <param name="original">Optional original instance to proxy calls to.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="original"/> is not the Via type.</exception>
        /// <returns>The proxy instance.</returns>
        object ProxyObject(object? original = null);
        
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
    /// A Via is the DivertR unit associated with a type. The Via is used to create proxies of its type and to configure the proxy behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The Via type.</typeparam>
    public interface IVia<TTarget> : IVia where TTarget : class
    {
        /// <summary>
        /// Reference to the Via <see cref="IRelay{TTarget}" /> chain of responsibility call pipeline.
        /// </summary>
        IRelay<TTarget> Relay { get; }
        
        /// <summary>
        /// Retrieve the current proxy redirect configuration.
        /// </summary>
        RedirectPlan<TTarget> RedirectPlan { get; }
        
        /// <summary>
        /// Create a Via proxy instance.
        /// </summary>
        /// <param name="original">Optional original instance to proxy calls to.</param>
        /// <returns>The proxy instance.</returns>
        TTarget Proxy(TTarget? original = null);
        
        /// <summary>
        /// Insert a <see cref="Redirect{TTarget}"/> instance into the Via <see cref="RedirectPlan{TTarget}" />.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect);
        
        /// <summary>
        /// Reset the Via <see cref="RedirectPlan{TTarget}" />.
        /// </summary>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        new IVia<TTarget> Reset();

        /// <summary>
        /// Create and insert a redirect (with no <see cref="ICallConstraint{TTarget}"/>) to the given <paramref name="target"/>
        /// into the Via <see cref="RedirectPlan{TTarget}" />.
        /// </summary>
        /// <param name="target">The target instance to redirect call to.</param>
        /// <param name="optionsAction">An optional builder action for configuring redirect options.</param>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        
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
        /// <returns>The Redirect builder.</returns>
        /// 
        IRedirectBuilder<TTarget> To(ICallConstraint<TTarget>? callConstraint = null);

        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning <typeparam name="TReturn" />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression) where TReturn : struct;
        
        // Delegate required to coerce C# to allow the To overload below
        delegate TResult ClassReturnMatch<in T, out TResult>(T args) where TResult : class;
        
        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning <typeparam name="TReturn" />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <typeparam name="TReturn">The Expression return type</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IClassFuncRedirectBuilder<TTarget, TReturn> To<TReturn>(Expression<ClassReturnMatch<TTarget, TReturn>> constraintExpression) where TReturn : class;
        
        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a member of <typeparamref name="TTarget"/> returning void />.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The Redirect builder instance.</returns>
        IActionRedirectBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates a Redirect builder from an Expression with a call constraint that matches a property setter member of <typeparamref name="TTarget"/> />.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">The call constraint expression for the input value of the setter.</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The Redirect builder instance.</returns>
        IActionRedirectBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>> constraintExpression);

        /// <summary>
        /// Enable strict mode.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>The current <see cref="IVia{TTarget}"/> instance.</returns>
        new IVia<TTarget> Strict(bool? isStrict = true);
    }
}