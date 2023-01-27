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
        
        ViaOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}