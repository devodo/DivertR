using System;
using System.Linq.Expressions;

namespace DivertR
{
    /// <summary>
    /// A Diverter Redirect builder interface for configuring redirect behaviour.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IDiverterRedirectBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Register a redirect to proxy all calls with return type <typeparamref name="TReturn"/> via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>The parent builder.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and default <see cref="RedirectId.Name" />.</exception>
        IDiverterBuilder ViaRedirect<TReturn>(string? name = null) where TReturn : class?;
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// The returned instances are proxied to the decorator by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate<TReturn>(Func<TReturn, TReturn> decorator);
        
        /// <summary>
        /// Register a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// The returned instances are proxied to the decorator by inserting a persistent <see cref="IVia"/> on the parent <see cref="IRedirect{TTarget}"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate<TReturn>(Func<TReturn, IDiverter, TReturn> decorator);
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter on calls matching the given constraint expression for class types.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="_">Ignore, this is a discard parameter to coerce C# into allowing method overloads on the generic constraints.</param>
        /// <typeparam name="TReturn">The constraint expression return type.</typeparam>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : class?;
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter on calls matching the given constraint expression for struct types.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="_">Ignore, this is a discard parameter to coerce C# into allowing method overloads on the generic constraints.</param>
        /// <typeparam name="TReturn">The constraint expression return type.</typeparam>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : struct;
    }
}