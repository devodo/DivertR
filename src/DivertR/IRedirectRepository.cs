using System.Runtime.CompilerServices;

namespace DivertR
{
    /// <summary>
    /// A repository for storing and updating an <see cref="IRedirect"/> configuration.
    /// </summary>
    public interface IRedirectRepository
    {
        /// <summary>
        /// The current <see cref="IRedirect"/> configuration.
        /// </summary>
        IRedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        /// Inserts the given <paramref name="via"/>.
        /// </summary>
        /// <param name="via">The Via to insert.</param>
        /// <param name="viaOptions">The Via options.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertVia(IVia via, ViaOptions? viaOptions = null);
        
        /// <summary>
        /// Inserts the given <paramref name="configuredVia"/>.
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
        /// Reset the <see cref="RedirectPlan" /> to its initial state.
        /// </summary>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository Reset();

        /// <summary>
        /// Reset the <see cref="RedirectPlan" /> and atomically insert the given <paramref name="via"/>.
        /// </summary>
        /// <param name="via">The Via instance to insert after reset.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository ResetAndInsert(IVia via);

        /// <summary>
        /// Resets the <see cref="RedirectPlan" /> and atomically insert the given <paramref name="via"/>.
        /// </summary>
        /// <param name="via">The Via instance to insert after reset.</param>
        /// <param name="viaOptions">The Via options.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository ResetAndInsert(IVia via, ViaOptions viaOptions);

        /// <summary>
        /// Reset the <see cref="RedirectPlan" /> and atomically insert the given <paramref name="configuredVia"/>.
        /// </summary>
        /// <param name="configuredVia">The configured Via instance to insert after reset.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository ResetAndInsert(IConfiguredVia configuredVia);
    }
}