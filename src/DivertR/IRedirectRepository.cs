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
        /// <param name="redirect">The redirect to insert.</param>
        /// <param name="redirectOptions">Optional redirect options.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertVia(IVia redirect, IViaOptions? redirectOptions = null);
        
        /// <summary>
        /// Insert a <see cref="IViaContainer"/> instance />.
        /// </summary>
        /// <param name="redirect">The redirect container.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertVia(IViaContainer redirect);
        
        IRedirectRepository SetStrictMode(bool isStrict = true);
        IRedirectRepository Reset(bool includePersistent = false);
    }
}