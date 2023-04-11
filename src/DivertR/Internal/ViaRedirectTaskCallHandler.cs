using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Internal
{
    internal class ViaRedirectTaskCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly IRedirect<TReturn> _redirect;
        private readonly ConditionalWeakTable<Task<TReturn>, Task<TReturn>> _taskProxyCache = new();

        public ViaRedirectTaskCallHandler(IRedirect<TReturn> redirect)
        {
            _redirect = redirect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var callReturn = call.CallNext();

            if (callReturn == null)
            {
                return null;
            }
            
            if (callReturn is not Task<TReturn> task)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {callReturn.GetType()}");
            }

            return _taskProxyCache.GetValue(task, async x =>
            {
                var taskResult = await x.ConfigureAwait(false);
                var proxy = _redirect.Proxy(taskResult);

                return proxy;
            });
        }
    }
}