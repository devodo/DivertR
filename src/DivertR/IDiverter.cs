using System;
using System.Collections.Generic;

namespace DivertR
{
    /// <summary>
    /// The primary DivertR interface for managing an <see cref="IVia"/> collection.
    /// </summary>
    public interface IDiverter
    {
        /// <summary>
        /// The <see cref="IViaSet"/> instance containing all <see cref="IVia"/> instances used by this <see cref="IDiverter"/>.
        /// </summary>
        IViaSet ViaSet { get; }
        
        /// <summary>
        /// Register an <see cref="IVia{TTarget}"/> for a given type.
        /// </summary>
        /// <param name="name">Optional Via group name.</param>
        /// <typeparam name="TTarget">The Via type.</typeparam>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register<TTarget>(string? name = null) where TTarget : class;
        
        /// <summary>
        /// Register an <see cref="IVia"/> for a given type.
        /// </summary>
        /// <param name="type">The Via type.</param>
        /// <param name="name">Optional Via group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register(Type type, string? name = null);
        
        /// <summary>
        /// Register multiple <see cref="IVia"/>s for a given type collection.
        /// </summary>
        /// <param name="types">The Via types.</param>
        /// <param name="name">The Via group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Register(IEnumerable<Type> types, string? name = null);
        
        /// <summary>
        /// Retrieve a group of registered <see cref="IVia"/>s.
        /// </summary>
        /// <param name="name">The Via group name.</param>
        /// <returns>The registered <see cref="IVia"/> collection.</returns>
        IEnumerable<IVia> RegisteredVias(string? name = null);
        
        /// <summary>
        /// Retrieve a registered <see cref="IVia{TTarget}" /> instance.
        /// </summary>
        /// <param name="name">Optional Via group name.</param>
        /// <typeparam name="TTarget">The Via type.</typeparam>
        /// <returns>The registered <see cref="IVia{TTarget}" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IVia{TTarget}" /> has not been registered.</exception>
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class;
        
        /// <summary>
        /// Retrieve a registered <see cref="IVia" /> instance.
        /// </summary>
        /// <param name="id">The <see cref="IVia" /> id.</param>
        /// <returns>The registered <see cref="IVia" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IVia" /> has not been registered.</exception>
        IVia Via(ViaId id);
        
        /// <summary>
        /// Retrieve a registered <see cref="IVia" /> instance.
        /// </summary>
        /// <param name="targetType">The <see cref="IVia" /> type.</param>
        /// <param name="name">The Via group name.</param>
        /// <returns>The registered <see cref="IVia" /> instance.</returns>
        /// <exception cref="DiverterException">If the <see cref="IVia" /> has not been registered.</exception>
        IVia Via(Type targetType, string? name = null);
        
        /// <summary>
        /// Enable strict mode on all registered <see cref="IVia" />s.
        /// </summary>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter StrictAll();
        
        /// <summary>
        /// Enable strict mode on group of registered <see cref="IVia" />s.
        /// </summary>
        /// <param name="name">The Via group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Strict(string? name = null);
        
        /// <summary>
        /// Reset all registered <see cref="IVia" />s.
        /// </summary>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter ResetAll();
        
        /// <summary>
        /// Reset registered <see cref="IVia" /> group.
        /// </summary>
        /// <param name="name">The Via group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IDiverter Reset(string? name = null);
    }
}