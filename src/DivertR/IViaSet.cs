using System;

namespace DivertR
{
    public interface IViaSet
    {
        DiverterSettings Settings { get; }
        
        /// <summary>
        /// Get or create a Via in this set for a given target type.
        /// </summary>
        /// <param name="name">Optional Via group name.</param>
        /// <typeparam name="TTarget">The Via target type.</typeparam>
        /// <returns>The existing or created Via.</returns>
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get or create a Via in this set for a given target type.
        /// </summary>
        /// <param name="targetType">The Via target type.</param>
        /// <param name="name">Optional Via group name.</param>
        /// <returns>The existing or created Via.</returns>
        IVia Via(Type targetType, string? name = null);

        /// <summary>
        /// Reset the specified group of <see cref="IVia" />s in this set.
        /// </summary>
        /// <param name="name">The Via group name.</param>
        /// <param name="includePersistent">Optionally also reset persistent redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IViaSet Reset(string? name = null, bool includePersistent = false);
        
        /// <summary>
        /// Reset all <see cref="IVia" />s in this set.
        /// </summary>
        /// <param name="includePersistent">Optionally also reset persistent redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IViaSet ResetAll(bool includePersistent = false);
        
        /// <summary>
        /// Enable strict mode on the specified group of <see cref="IVia" />s in this set.
        /// </summary>
        /// <param name="name">The Via group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IViaSet Strict(string? name = null);
        
        /// <summary>
        /// Enable strict mode on all <see cref="IVia" />s in this set.
        /// </summary>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IViaSet StrictAll();
    }
}