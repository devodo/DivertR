using System;
using System.Linq.Expressions;

namespace DivertR
{
    /// <summary>
    /// A builder interface for providing nested register actions on parent registrations.
    /// Nested registrations can be used to intercept and redirect inner services created outside the dependency injection container e.g. from factories.
    /// </summary>
    public interface INestedRegisterBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// Redirect calls from the parent registration with return type <typeparamref name="TReturn"/> by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder{TTarget}"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and default <see cref="RedirectId.Name" />.</exception>
        INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect calls from the parent registration with return type <typeparamref name="TReturn"/> by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder{TTarget}"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and <paramref name="name"/>.</exception>
        INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(string? name, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect calls matching a constraint from the parent registration by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder{TTarget}"/> instance.</returns>
        INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Redirect calls matching a constraint from the parent registration by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// The returned instances are proxied by inserting a persistent <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/>.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder{TTarget}"/> instance.</returns>
        INestedRegisterBuilder<TTarget> ThenRedirect<TReturn>(string? name, Expression<Func<TTarget, TReturn>> constraintExpression, Action<INestedRegisterBuilder<TReturn>>? registerAction = null) where TReturn : class?;
    }
}