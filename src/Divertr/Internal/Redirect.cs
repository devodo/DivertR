using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class Redirect<T> : IRedirect<T> where T : class
    {
        private readonly ICallCondition? _callCondition;
        public T Target { get; }
        public object? State { get; }

        public Redirect(T target, object? state = null, ICallCondition? callCondition = null)
        {
            _callCondition = callCondition;
            Target = target;
            State = state;
        }

        public bool IsMatch(ICall call)
        {
            return _callCondition?.IsMatch(call) ?? true;
        }
    }
}
