namespace DivertR.Internal
{
    internal class CallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ICallHandler _callHandler;

        public CallHandler(ICallHandler callHandler)
        {
            _callHandler = callHandler;
        }
        
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _callHandler.Handle(call);
        }
    }
}