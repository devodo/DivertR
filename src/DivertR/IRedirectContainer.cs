using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectContainer
    {
        IRedirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        IRedirectOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}