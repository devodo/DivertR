using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectPlan
    {
        IReadOnlyList<IViaContainer> Vias
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