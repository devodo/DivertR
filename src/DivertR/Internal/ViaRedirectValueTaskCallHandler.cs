using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Internal
{
    internal class ViaRedirectValueTaskCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly IRedirect<TReturn> _redirect;

        public ViaRedirectValueTaskCallHandler(IRedirect<TReturn> redirect)
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

            if (callReturn is not ValueTask<TReturn> valueTask)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {callReturn.GetType()}");
            }

            return ProxifyValueTaskAsync(valueTask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async ValueTask<TReturn> ProxifyValueTaskAsync(ValueTask<TReturn> valueTask)
        {
            var taskResult = await valueTask.ConfigureAwait(false);

            return _redirect.Proxy(taskResult);
        }
    }
}