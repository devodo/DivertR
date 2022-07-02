using System.Collections.Generic;
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
        void InsertRedirect(IRedirect redirect);
        
        void InsertRedirects(IEnumerable<IRedirect> redirects);
        void SetStrictMode(bool isStrict = true);
        void Reset();
    }
}