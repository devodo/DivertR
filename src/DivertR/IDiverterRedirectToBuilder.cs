using System;

namespace DivertR
{
    /// <summary>
    /// A constrained Diverter Redirect builder interface for configuring redirect behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    /// <typeparam name="TReturn">The constrained return type.</typeparam>
    public interface IDiverterRedirectToBuilder<TTarget, TReturn> where TTarget : class?
    {
        /// <summary>
        /// Append an additional filter to the existing constraint.
        /// </summary>
        /// <param name="callConstraint">The call constraint filter.</param>
        /// <returns>This instance.</returns>
        IDiverterRedirectToBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint);
        
        /// <summary>
        /// Register a redirect to proxy all calls matching matching this builders filter constraint via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder ViaRedirect(string? name = null);
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// The returned instances are proxied to the decorator by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate(Func<TReturn, TReturn> decorator);
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns matching this builders filter constraint.
        /// The returned instances are proxied to the decorator by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate(Func<TReturn, IDiverter, TReturn> decorator);
    }
}