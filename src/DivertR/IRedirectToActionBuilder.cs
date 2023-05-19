using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// A Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with void return type.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IRedirectToActionBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget> Via(Action<IActionRedirectCall<TTarget>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls with ValueTuple arguments via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>A builder with ValueTuple arguments.</returns>
        IRedirectToActionBuilder<TTarget, TArgs> Via<TArgs>(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Specify ValueTuple arguments
        /// </summary>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>A builder with ValueTuple arguments.</returns>
        IRedirectToActionBuilder<TTarget, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Record calls to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls.</returns>
        IActionCallStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Record calls with ValueTuple arguments to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>An enumerable collection containing the recorded calls with ValueTuple arguments.</returns>
        IActionCallStream<TTarget, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
    }
    
    /// <summary>
    /// A Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with void return type.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
    public interface IRedirectToActionBuilder<TTarget, TArgs> : IRedirectToActionBuilder<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        new IRedirectToActionBuilder<TTarget, TArgs> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToActionBuilder<TTarget, TArgs> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToActionBuilder<TTarget, TArgs> Via(Action viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToActionBuilder<TTarget, TArgs> Via(Action<IActionRedirectCall<TTarget, TArgs>> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToActionBuilder<TTarget, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Record calls to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls with ValueTuple arguments.</returns>
        new IActionCallStream<TTarget, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
