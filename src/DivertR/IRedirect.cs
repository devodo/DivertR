using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IRedirect
    {
        int OrderWeight { get; }

        bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(ICallInfo callInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object? Handle(IRedirectCall call);
    }

    public interface IRedirect<TTarget> : IRedirect where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        ICallHandler<TTarget> CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}