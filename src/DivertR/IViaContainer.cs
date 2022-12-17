using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IViaContainer
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