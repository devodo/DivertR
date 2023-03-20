using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallHandler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Handle(IRedirectCall call);
    }
}