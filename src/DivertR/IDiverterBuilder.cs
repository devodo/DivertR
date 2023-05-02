using System;
using System.Collections.Generic;

namespace DivertR
{
    /// <summary>
    /// Interface used for building an <see cref="IDiverter"/> instance by registering <see cref="IRedirect"/> and general decorators that are intended to be installed into a dependency injection container.
    ///
    /// General decorators are added with 'Decorate' methods and are functions that take as input the original resolved instance and return a decorated instance.
    /// Redirect decorators are added with 'Register' methods and specialisations that decorate the original resolved instances as <see cref="IRedirect{TTarget}"/> proxies.
    /// </summary>
    public interface IDiverterBuilder
    {
        /// <summary>
        /// The underlying <see cref="IRedirectSet"/> containing the set of registered <see cref="IRedirect"/> instances.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// Register a type to redirect. The redirect is added with default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="nestedRegisterAction">Optional nested register action.</param>
        /// <typeparam name="TTarget">The type to register.</typeparam>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <typeparamref name="TTarget"/> type with with default <see cref="RedirectId.Name" /> has already been registered.</exception>
        IDiverterBuilder Register<TTarget>(Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?;

        /// <summary>
        /// Register a type to redirect.
        /// </summary>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="nestedRegisterAction">Optional nested register action.</param>
        /// <typeparam name="TTarget">The type to register.</typeparam>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <typeparamref name="TTarget"/> type with matching <paramref name="name"/> has already been registered.</exception>
        IDiverterBuilder Register<TTarget>(string? name, Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?;

        /// <summary>
        /// Register a type to redirect.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <paramref name="type"/> with matching <paramref name="name"/> has already been registered.</exception>
        IDiverterBuilder Register(Type type, string? name = null);
        
        /// <summary>
        /// Register multiple types to redirect.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with matching <paramref name="name"/> have already been registered.</exception>
        IDiverterBuilder Register(IEnumerable<Type> types, string? name = null);
        
        /// <summary>
        /// Register multiple types to redirect. The redirects are added with default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="types">The types to register.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with default <see cref="RedirectId.Name" /> have already been registered.</exception>
        IDiverterBuilder Register(params Type[] types);
        
        /// <summary>
        /// Register multiple types to redirect.
        /// </summary>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="types">The types to register.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with matching <paramref name="name"/> have already been registered.</exception>
        IDiverterBuilder Register(string? name, params Type[] types);
        
        /// <summary>
        /// Register a service decorator. The decorator is added to the default group.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TService">The service type to decorate.</typeparam>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        IDiverterBuilder Decorate<TService>(Func<TService, TService> decorator);
        
        /// <summary>
        /// Register a service decorator.
        /// </summary>
        /// <param name="name">The decorator group.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TService">The service type to decorate.</typeparam>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        IDiverterBuilder Decorate<TService>(string? name, Func<TService, TService> decorator);
        
        /// <summary>
        /// Register a service decorator. The decorator is added to the default group.
        /// </summary>
        /// <param name="serviceType">The service type to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        IDiverterBuilder Decorate(Type serviceType, Func<object, object> decorator);
        
        /// <summary>
        /// Register a service decorator.
        /// </summary>
        /// <param name="name">The decorator group.</param>
        /// <param name="serviceType">The service type to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <returns>This <see cref="IDiverterBuilder"/> instance.</returns>
        IDiverterBuilder Decorate(string? name, Type serviceType, Func<object, object> decorator);
        
        /// <summary>
        /// Create an <see cref="IDiverter"/> instance from the current registrations.
        /// </summary>
        /// <returns>The created instance.</returns>
        IDiverter Create();
    }
}