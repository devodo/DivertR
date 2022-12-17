using System;
using System.Collections.Generic;

namespace DivertR
{
    /// <summary>
    /// The primary DivertR interface for managing an <see cref="IRedirect"/> collection.
    /// </summary>
    public interface IDiverter
    {
        /// <summary>
        /// The <see cref="IRedirectSet"/> instance containing all <see cref="IRedirect"/> instances used by this <see cref="IDiverter"/>.
        /// </summary>
        IRedirectSet RedirectSet { get; }
        
        /// <summary>
        /// Register an <see cref="IRedirect{TTarget}"/> for a given type.
        /// </summary>
        /// <param name="name">Optional Redirect group name.</param>
        /// <typeparam name="TTarget">The Redirect type.</typeparam>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Register an <see cref="IRedirect"/> for a given type.
        /// </summary>
        /// <param name="type">The Redirect type.</param>
        /// <param name="name">Optional Redirect group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register(Type type, string? name = null);
        
        /// <summary>
        /// Register multiple <see cref="IRedirect"/>s for a given type collection.
        /// </summary>
        /// <param name="types">The Redirect types.</param>
        /// <param name="name">The Redirect group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register(IEnumerable<Type> types, string? name = null);
        
        /// <summary>
        /// Retrieve a group of registered <see cref="IRedirect"/>s.
        /// </summary>
        /// <param name="name">The Redirect group name.</param>
        /// <returns>The registered <see cref="IRedirect"/> collection.</returns>
        IEnumerable<IRedirect> RegisteredRedirects(string? name = null);
        
        /// <summary>
        /// Retrieve a registered <see cref="IRedirect{TTarget}" /> instance.
        /// </summary>
        /// <param name="name">Optional Redirect group name.</param>
        /// <typeparam name="TTarget">The Redirect type.</typeparam>
        /// <returns>The registered <see cref="IRedirect{TTarget}" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IRedirect{TTarget}" /> has not been registered.</exception>
        IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Retrieve a registered <see cref="IRedirect" /> instance.
        /// </summary>
        /// <param name="id">The <see cref="IRedirect" /> id.</param>
        /// <returns>The registered <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IRedirect" /> has not been registered.</exception>
        IRedirect Redirect(RedirectId id);
        
        /// <summary>
        /// Retrieve a registered <see cref="IRedirect" /> instance.
        /// </summary>
        /// <param name="targetType">The <see cref="IRedirect" /> type.</param>
        /// <param name="name">The Redirect group name.</param>
        /// <returns>The registered <see cref="IRedirect" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IRedirect" /> has not been registered.</exception>
        IRedirect Redirect(Type targetType, string? name = null);
        
        /// <summary>
        /// Enable strict mode on all registered <see cref="IRedirect" />s.
        /// </summary>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter StrictAll();
        
        /// <summary>
        /// Enable strict mode on group of registered <see cref="IRedirect" />s.
        /// </summary>
        /// <param name="name">The Redirect group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Strict(string? name = null);
        
        /// <summary>
        /// Reset all registered <see cref="IRedirect" />s.
        /// </summary>
        /// <param name="includePersistent">Optionally also reset persistent redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter ResetAll(bool includePersistent = false);
        
        /// <summary>
        /// Reset registered <see cref="IRedirect" /> group.
        /// </summary>
        /// <param name="name">The Redirect group name.</param>
        /// <param name="includePersistent">Optionally also reset persistent redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Reset(string? name = null, bool includePersistent = false);
    }
}