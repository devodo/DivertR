using System;
using System.Collections;

namespace DivertR
{
    /// <summary>
    /// A Diverter Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with void return type.
    /// By default all <see cref="IVia"/> instances added by this builder are configured to be persistent and to not satisfy strict checks.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IDiverterRedirectToActionBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IDiverterRedirectToActionBuilder<TTarget> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls with ValueTuple arguments via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}