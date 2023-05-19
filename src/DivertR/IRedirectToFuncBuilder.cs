using System;
using System.Collections;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// A Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with return type <typeparamref name="TReturn"/>.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    /// <typeparam name="TReturn">The constrained return type.</typeparam>
    public interface IRedirectToFuncBuilder<TTarget, TReturn> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls to return the given instance.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Redirect calls with ValueTuple arguments via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>A builder with ValueTuple arguments.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Specify ValueTuple arguments
        /// </summary>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>A builder with ValueTuple arguments.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Args<TArgs>()
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Record calls to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls.</returns>
        IFuncCallStream<TTarget, TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Record calls with ValueTuple arguments to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
        /// <returns>An enumerable collection containing the recorded calls with ValueTuple arguments.</returns>
        IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable;
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn> Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
    }
    
    /// <summary>
    /// A Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour on methods with return type <typeparamref name="TReturn"/>.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    /// <typeparam name="TReturn">The constrained return type.</typeparam>
    /// <typeparam name="TArgs">The ValueTuple type that call arguments are mapped to.</typeparam>
    public interface IRedirectToFuncBuilder<TTarget, TReturn, TArgs> : IRedirectToFuncBuilder<TTarget, TReturn>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls to return the given instance.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Record calls to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls with ValueTuple arguments.</returns>
        new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
