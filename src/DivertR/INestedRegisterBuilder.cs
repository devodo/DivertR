using System;

namespace DivertR
{
    /// <summary>
    /// A builder interface for providing nested register actions on parent registrations.
    /// Nested registrations can be used to intercept and redirect inner services created outside the dependency injection container e.g. from factories.
    /// </summary>
    public interface INestedRegisterBuilder
    {
        /// <summary>
        /// Add a nested registration to redirect calls from the parent registration with return type <typeparamref name="TReturn"/>
        /// by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// A corresponding <see cref="IRedirect{TReturn}"/> is created and added to the underlying <see cref="IRedirectSet"/>.
        /// The returned instances are proxied by inserting a <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/> that is persistent, i.e. remains after reset.
        /// </summary>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and default <see cref="RedirectId.Name" />.</exception>
        INestedRegisterBuilder ThenRegister<TReturn>(Action<INestedRegisterBuilder>? registerAction = null) where TReturn : class?;

        /// <summary>
        /// Add a nested registration to redirect calls from the parent registration with return type <typeparamref name="TReturn"/>
        /// by proxying instances returned from parent methods via an <see cref="IRedirect{TReturn}"/>.
        /// A corresponding <see cref="IRedirect{TReturn}"/> is created and added to the underlying <see cref="IRedirectSet"/>.
        /// The returned instances are proxied by inserting a <see cref="IVia"/> on the <see cref="IRedirect{TReturn}"/> that is persistent, i.e. remains after reset.
        /// </summary>
        /// <param name="name">Specify the <see cref="DivertR.RedirectId.Name" /> of the returned <see cref="IRedirect{TReturn}"/>.</param>
        /// <param name="registerAction">Optional nested register action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and <paramref name="name"/>.</exception>
        INestedRegisterBuilder ThenRegister<TReturn>(string? name, Action<INestedRegisterBuilder>? registerAction = null) where TReturn : class?;
        
        /// <summary>
        /// Register a decorator on call returns of type <typeparamref name="TReturn"/> on the nested redirect.
        /// The decorator is applied by inserting a <see cref="IVia"/> on the nested redirect that is persistent, i.e. remains after reset.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>This <see cref="INestedRegisterBuilder"/> instance.</returns>
        INestedRegisterBuilder ThenDecorate<TReturn>(Func<TReturn, TReturn> decorator);
    }
}