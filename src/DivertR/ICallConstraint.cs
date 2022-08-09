using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(ICallInfo callInfo);
    }
    
    public interface ICallConstraint<in TTarget> where TTarget : class?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(ICallInfo<TTarget> callInfo);
    }
}