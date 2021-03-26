using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueCallConstraint<T> : ICallConstraint<T> where T : class
    {
        public static readonly TrueCallConstraint<T> Instance = new TrueCallConstraint<T>();
        
        public bool IsMatch(CallInfo<T> callInfo)
        {
            return true;
        }
    }
}
