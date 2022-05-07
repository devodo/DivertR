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

        void InsertRedirect(IRedirect redirect);
        void InsertRedirects(IEnumerable<IRedirect> redirects);
        void SetStrictMode(bool isStrict = true);
        void Reset();
    }
}