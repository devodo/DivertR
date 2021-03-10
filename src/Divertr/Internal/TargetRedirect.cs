using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class TargetRedirect<T> : IRedirect<T> where T : class
    {
        private readonly T _target;
        private readonly ICallCondition? _callCondition;
        public object? State { get; }

        public TargetRedirect(T target, object? state = null, ICallCondition? callCondition = null)
        {
            _callCondition = callCondition;
            _target = target;
            State = state;
        }

        public object? Invoke(MethodInfo methodInfo, object[] args)
        {
            if (_target == null)
            {
                throw new DiverterException("The redirect instance reference is null");
            }
            
            return methodInfo.ToDelegate(typeof(T)).Invoke(_target, args);
        }

        public bool IsMatch(ICall call)
        {
            return _callCondition?.IsMatch(call) ?? true;
        }
    }
}
