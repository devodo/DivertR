using System;
using System.Linq.Expressions;

namespace DivertR
{
    /// <summary>
    /// A builder interface for nested Diverter builder actions.
    /// </summary>
    public interface INestedDiverterBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Redirect parent calls with return type <typeparamref name="TReturn"/> via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedDiverterBuilder{TTarget}"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and default <see cref="RedirectId.Name" />.</exception>
        INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect parent calls with return type <typeparamref name="TReturn"/> via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedDiverterBuilder{TTarget}"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and <paramref name="name"/>.</exception>
        INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(string? name, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect parent calls matching a constraint via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedDiverterBuilder{TTarget}"/> instance.</returns>
        INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect parent calls matching a constraint via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedDiverterBuilder{TTarget}"/> instance.</returns>
        INestedDiverterBuilder<TTarget> AddRedirect<TReturn>(string? name, Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedDiverterBuilder<TReturn>>? registerAction = null) where TReturn : class?;
    }
}