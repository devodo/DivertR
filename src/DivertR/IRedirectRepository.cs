using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectRepository
    {
        IRedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        /// Insert a <see cref="IVia"/> instance />.
        /// </summary>
        /// <param name="via">The Via to insert.</param>
        /// <param name="viaOptions">Optional Via options.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertVia(IVia via, IViaOptions? viaOptions = null);
        
        /// <summary>
        /// Insert a <see cref="IConfiguredVia"/> instance />.
        /// </summary>
        /// <param name="configuredVia">The configured Via.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertVia(IConfiguredVia configuredVia);
        
        /// <summary>
        /// Set strict mode.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository SetStrictMode(bool isStrict = true);
        
        /// <summary>
        /// Reset this <see cref="IRedirectRepository" /> to its initial state.
        /// </summary>
        /// <param name="includePersistent">Optionally also reset persistent <see cref="IVia"/>s.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository Reset(bool includePersistent = false);
    }
}