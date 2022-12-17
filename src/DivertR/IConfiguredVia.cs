using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IConfiguredVia
    {
        IVia Via
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        IViaOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}