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
        /// Insert a <see cref="IRedirect"/> instance />.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="redirectOptions">Optional redirect options.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertRedirect(IRedirect redirect, IRedirectOptions? redirectOptions = null);
        
        /// <summary>
        /// Insert a <see cref="IRedirectContainer"/> instance />.
        /// </summary>
        /// <param name="redirect">The redirect container.</param>
        /// <returns>This <see cref="IRedirectRepository"/> instance.</returns>
        IRedirectRepository InsertRedirect(IRedirectContainer redirect);
        
        IRedirectRepository SetStrictMode(bool isStrict = true);
        IRedirectRepository Reset();
    }
}