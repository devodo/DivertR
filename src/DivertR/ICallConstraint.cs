using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(CallInfo callInfo);
    }
    
    public interface ICallConstraint<TTarget> where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(CallInfo<TTarget> callInfo);
    }
}