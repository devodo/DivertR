using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirectUntargeted : IRedirect
    {
        ICallConstraint CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        ICallHandler CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}