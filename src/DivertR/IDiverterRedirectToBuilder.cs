using System;

namespace DivertR
{
    /// <summary>
    /// A Diverter Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour.
    /// By default all <see cref="IVia"/> instances added by this builder are configured to be persistent and to not satisfy strict checks.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IDiverterRedirectToBuilder<in TTarget> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IDiverterRedirectToBuilder<TTarget> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Redirect calls via the given <paramref name="callHandler"/>.
        /// </summary>
        /// <param name="callHandler">The call handler.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Retarget calls to a given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target to retarget calls too.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}