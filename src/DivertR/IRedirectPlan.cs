using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectPlan
    {
        /// <summary>
        /// The ordered list of configured vias.
        /// </summary>
        IReadOnlyList<IConfiguredVia> Vias
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        /// <summary>
        /// Boolean flag to indicate if strict mode is enabled or disabled.
        /// </summary>
        bool IsStrictMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}