using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectPlan<TTarget> where TTarget : class
    {
        IReadOnlyList<Redirect<TTarget>> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}