using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReturnTypeCallConstraint<TReturn> : ICallConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            return callInfo.Method.ReturnType == typeof(TReturn);
        }
    }
}