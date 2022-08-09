using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallHandler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Handle(IRedirectCall call);
    }
    
    public interface ICallHandler<TTarget> where TTarget : class?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Handle(IRedirectCall<TTarget> call);
    }
}