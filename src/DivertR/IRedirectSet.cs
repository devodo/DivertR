using System;

namespace DivertR
{
    public interface IRedirectSet
    {
        DiverterSettings Settings { get; }
        
        /// <summary>
        /// Get or create a Redirect in this set for a given target type.
        /// </summary>
        /// <param name="name">Optional Redirect group name.</param>
        /// <typeparam name="TTarget">The Redirect target type.</typeparam>
        /// <returns>The existing or created Redirect.</returns>
        IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get or create a Redirect in this set for a given target type.
        /// </summary>
        /// <param name="targetType">The Redirect target type.</param>
        /// <param name="name">Optional Redirect group name.</param>
        /// <returns>The existing or created Redirect.</returns>
        IRedirect Redirect(Type targetType, string? name = null);

        /// <summary>
        /// Reset the specified group of <see cref="IRedirect" />s in this set.
        /// </summary>
        /// <param name="name">The Redirect group name.</param>
        /// <param name="includePersistent">Optionally also reset persistent Redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IRedirectSet Reset(string? name = null, bool includePersistent = false);
        
        /// <summary>
        /// Reset all <see cref="IRedirect" />s in this set.
        /// </summary>
        /// <param name="includePersistent">Optionally also reset persistent Redirects.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IRedirectSet ResetAll(bool includePersistent = false);
        
        /// <summary>
        /// Enable strict mode on the specified group of <see cref="IRedirect" />s in this set.
        /// </summary>
        /// <param name="name">The Redirect group name.</param>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IRedirectSet Strict(string? name = null);
        
        /// <summary>
        /// Enable strict mode on all <see cref="IRedirect" />s in this set.
        /// </summary>
        /// <returns>The current <see cref="IDiverter"/> instance.</returns>
        IRedirectSet StrictAll();
    }
}