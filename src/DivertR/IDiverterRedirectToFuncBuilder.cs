using System;
using System.Collections;

namespace DivertR
{
    /// <summary>
    /// A Diverter Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with return type <typeparamref name="TReturn"/>.
    /// By default all <see cref="IVia"/> instances added by this builder are configured to be persistent and to not satisfy strict checks.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    /// <typeparam name="TReturn">The constrained return type.</typeparam>
    public interface IDiverterRedirectToFuncBuilder<TTarget, TReturn> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IDiverterRedirectToFuncBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls to return the given instance.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        
        /// <summary>
        /// Redirect calls with ValueTuple arguments via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Add a <see cref="IRedirect{TReturn}"/> and proxy all calls matching this builders filter constraint.
        /// The added <see cref="IRedirect{TReturn}"/> has default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder ViaRedirect(Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Add a <see cref="IRedirect{TReturn}"/> and proxy all calls matching this builders filter constraint.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder ViaRedirect(string? name, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate(Func<TReturn, IDiverter, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}