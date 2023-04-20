using System;
using System.Collections.Generic;

namespace DivertR
{
    /// <summary>
    /// The DivertR interface used for managing and registering a collection of <see cref="IRedirect"/> instances intended to be embedded into a dependency injection container.
    ///
    /// Use this interface to register the set of types intended to be redirected and a corresponding <see cref="IRedirect"/> is created for each and added to the underlying <see cref="IRedirectSet"/>.
    /// 
    /// The registered <see cref="IRedirect"/> instances are exposed for embedding into a dependency injection container as decorators added to the <see cref="IDiverterDecorator"/> collection.
    /// These decorators convert the original resolved instances into redirect proxies with the originals as their roots. General purpose decorators can also be registered on this interface.
    ///  
    /// The responsibility of embedding the decorators into the dependency injection container is left to the container specific implementation.
    /// For example <see cref="DependencyInjection.ServiceCollectionExtensions.Divert"/> is an extension method that does this for the <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection">IServiceCollection</see> container.
    ///
    /// This interface also acts as a facade around an underlying <see cref="IRedirectSet"/> for retrieving and managing <see cref="IRedirect"/> instances in this redirect collection.   
    /// </summary>
    public interface IDiverter
    {
        /// <summary>
        /// The underlying <see cref="IRedirectSet"/> containing the <see cref="IRedirect"/> collection of this <see cref="IDiverter"/> instance.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// Register a type to redirect. The redirect is added with default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="nestedRegisterAction">Optional nested register action.</param>
        /// <typeparam name="TTarget">The type to register.</typeparam>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <typeparamref name="TTarget"/> type with with default <see cref="RedirectId.Name" /> has already been registered.</exception>
        IDiverter Register<TTarget>(Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?;

        /// <summary>
        /// Register a type to redirect.
        /// </summary>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="nestedRegisterAction">Optional nested register action.</param>
        /// <typeparam name="TTarget">The type to register.</typeparam>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <typeparamref name="TTarget"/> type with matching <paramref name="name"/> has already been registered.</exception>
        IDiverter Register<TTarget>(string? name, Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?;

        /// <summary>
        /// Register a type to redirect.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the <paramref name="type"/> with matching <paramref name="name"/> has already been registered.</exception>
        IDiverter Register(Type type, string? name = null);
        
        /// <summary>
        /// Register multiple types to redirect.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with matching <paramref name="name"/> have already been registered.</exception>
        IDiverter Register(IEnumerable<Type> types, string? name = null);
        
        /// <summary>
        /// Register multiple types to redirect. The redirects are added with default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="types">The types to register.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with default <see cref="RedirectId.Name" /> have already been registered.</exception>
        IDiverter Register(params Type[] types);
        
        /// <summary>
        /// Register multiple types to redirect.
        /// </summary>
        /// <param name="name">The decorator group and the <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="types">The types to register.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        /// <exception cref="DiverterException">Thrown if any <paramref name="types"/> with matching <paramref name="name"/> have already been registered.</exception>
        IDiverter Register(string? name, params Type[] types);
        
        /// <summary>
        /// Register a service decorator. The decorator is added to the default group.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TService">The service type to decorate.</typeparam>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter Decorate<TService>(Func<TService, TService> decorator);
        
        /// <summary>
        /// Register a service decorator.
        /// </summary>
        /// <param name="name">The decorator group.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <typeparam name="TService">The service type to decorate.</typeparam>
        /// <returns></returns>
        IDiverter Decorate<TService>(string? name, Func<TService, TService> decorator);
        
        /// <summary>
        /// Register a service decorator. The decorator is added to the default group.
        /// </summary>
        /// <param name="serviceType">The service type to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <returns></returns>
        IDiverter Decorate(Type serviceType, Func<object, object> decorator);
        
        /// <summary>
        /// Register a service decorator.
        /// </summary>
        /// <param name="name">The decorator group.</param>
        /// <param name="serviceType">The service type to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <returns></returns>
        IDiverter Decorate(string? name, Type serviceType, Func<object, object> decorator);

        /// <summary>
        /// Gets the collection of currently registered decorators.
        /// This method is intended to be used by the process responsible for installing decorators into the dependency injection container.
        /// </summary>
        /// <param name="name">Optional decorator group name.</param>
        /// <returns>The <see cref="IDiverterDecorator"/> collection.</returns>
        IEnumerable<IDiverterDecorator> GetDecorators(string? name = null);
        
        /// <summary>
        /// Get the <see cref="IRedirect{TTarget}"/> from the underlying <see cref="IRedirectSet"/> by <see cref="RedirectId"/> key generated from <typeparamref name="TTarget"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <typeparam name="TTarget">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</typeparam>
        /// <returns>The <see cref="IRedirect{TTarget}" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> from the underlying <see cref="IRedirectSet"/> by <paramref name="redirectId"/>
        /// </summary>
        /// <param name="redirectId">The <see cref="IRedirect"/> key.</param>
        /// <returns>The <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect Redirect(RedirectId redirectId);
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> from the underlying <see cref="IRedirectSet"/> by <see cref="RedirectId"/> key generated from <paramref name="type"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="type">The <see cref="RedirectId.Type" /> of the <see cref="RedirectId"/>.</param>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="RedirectId"/>.</param>
        /// <returns>The <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect Redirect(Type type, string? name = null);
        
        /// <summary>
        /// Enable strict mode on all <see cref="IRedirect"/> instances in the underlying <see cref="IRedirectSet"/>.
        /// </summary>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter StrictAll();
        
        /// <summary>
        /// Enable strict mode on an <see cref="IRedirect"/> group in the underlying <see cref="IRedirectSet"/> with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter Strict(string? name = null);
        
        /// <summary>
        /// Reset all <see cref="IRedirect"/> instances in the underlying <see cref="IRedirectSet"/>.
        /// </summary>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter ResetAll();
        
        /// <summary>
        /// Reset an <see cref="IRedirect"/> group in the underlying <see cref="IRedirectSet"/> set with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter Reset(string? name = null);
    }
}