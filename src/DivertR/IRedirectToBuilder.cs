using System;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// A Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IRedirectToBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IRedirectToBuilder<TTarget> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToBuilder<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToBuilder<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Redirect calls via the given call delegate handler.
        /// </summary>
        /// <param name="viaDelegate">The call handler delegate.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToBuilder<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This instance.</returns>
        IRedirectToBuilder<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Record calls to the returned stream.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>An enumerable collection containing the recorded calls.</returns>
        IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
