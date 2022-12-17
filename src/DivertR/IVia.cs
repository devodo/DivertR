using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IVia
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(ICallInfo callInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Handle(IRedirectCall call);
    }
}