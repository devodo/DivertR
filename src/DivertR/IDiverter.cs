using System;
using System.Collections.Generic;

namespace DivertR
{
    /// <summary>
    /// An interface for retrieving and managing a set of <see cref="IRedirect"/> instances.
    ///
    /// This interface also exposes the associated <see cref="IDiverterDecorator"/> decorators via the <see cref="IDiverter.GetDecorators"/> method that are intended to be embedded into a dependency injection container.
    /// The responsibility of installing the decorators is left to the container specific implementation.
    /// For example <see cref="DependencyInjection.ServiceCollectionExtensions.Divert"/> is an extension method that does this for the <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection">IServiceCollection</see> container.
    /// </summary>
    public interface IDiverter
    {
        /// <summary>
        /// The underlying <see cref="IRedirectSet"/> containing the <see cref="IRedirect"/> collection of this <see cref="IDiverter"/> instance.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// Get the <see cref="IRedirect{TTarget}"/> by <see cref="RedirectId"/> key generated from <typeparamref name="TTarget"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <typeparam name="TTarget">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</typeparam>
        /// <returns>The <see cref="IRedirect{TTarget}" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> by <paramref name="redirectId"/>
        /// </summary>
        /// <param name="redirectId">The <see cref="IRedirect"/> key.</param>
        /// <returns>The <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect Redirect(RedirectId redirectId);
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> by <see cref="RedirectId"/> key generated from <paramref name="type"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="type">The <see cref="RedirectId.Type" /> of the <see cref="RedirectId"/>.</param>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="RedirectId"/>.</param>
        /// <returns>The <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">Thrown if the underlying <see cref="IRedirectSet" /> does not contain a matching <see cref="IRedirect"/>.</exception>
        IRedirect Redirect(Type type, string? name = null);
        
        /// <summary>
        /// Enable strict mode on all <see cref="IRedirect"/> instances.
        /// </summary>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter StrictAll();
        
        /// <summary>
        /// Enable strict mode on an <see cref="IRedirect"/> group with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter Strict(string? name = null);
        
        /// <summary>
        /// Reset all <see cref="IRedirect"/> instances.
        /// </summary>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter ResetAll();
        
        /// <summary>
        /// Reset an <see cref="IRedirect"/> group with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IDiverter"/> instance.</returns>
        IDiverter Reset(string? name = null);
        
        /// <summary>
        /// Gets the collection of registered decorators.
        /// This method is intended to be used by the process responsible for installing decorators into a dependency injection container.
        /// </summary>
        /// <param name="name">Optional decorator group name.</param>
        /// <returns>The <see cref="IDiverterDecorator"/> collection.</returns>
        IEnumerable<IDiverterDecorator> GetDecorators(string? name = null);
    }
}