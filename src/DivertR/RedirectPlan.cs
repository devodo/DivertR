using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class RedirectPlan<TTarget> where TTarget : class
    {
        public static readonly RedirectPlan<TTarget> Empty = new RedirectPlan<TTarget>(Array.Empty<Redirect<TTarget>>(), false);
        
        public RedirectPlan(IReadOnlyList<Redirect<TTarget>> redirects, bool isStrictMode)
        {
            Redirects = redirects;
            IsStrictMode = isStrictMode;
        }

        public IReadOnlyList<Redirect<TTarget>> Redirects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}