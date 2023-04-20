using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Internal
{
    internal class ViaRedirectTypeCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly ViaRedirectCallHandler<TReturn> _callHandler;
        private readonly ViaRedirectTaskCallHandler<TReturn> _taskCallHandler;
        private readonly ViaRedirectValueTaskCallHandler<TReturn> _valueCallHandler;
        
        private readonly Type _returnType;
        private readonly Type _returnTaskType;
        private readonly Type _returnValueTaskType;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ViaRedirectTypeCallHandler(IRedirect<TReturn> redirect)
        {
            _callHandler = new ViaRedirectCallHandler<TReturn>(redirect);
            _taskCallHandler = new ViaRedirectTaskCallHandler<TReturn>(redirect);
            _valueCallHandler = new ViaRedirectValueTaskCallHandler<TReturn>(redirect);
            
            _returnType = typeof(TReturn);
            _returnTaskType = typeof(Task<>).MakeGenericType(_returnType);
            _returnValueTaskType = typeof(ValueTask<>).MakeGenericType(_returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            if (ReferenceEquals(call.CallInfo.Method.ReturnType, _returnType))
            {
                return _callHandler.Handle(call);
            }
            
            if (ReferenceEquals(call.CallInfo.Method.ReturnType, _returnTaskType))
            {
                return _taskCallHandler.Handle(call);
            }
            
            if (ReferenceEquals(call.CallInfo.Method.ReturnType, _returnValueTaskType))
            {
                return _valueCallHandler.Handle(call);
            }
            
            throw new InvalidOperationException($"Unexpected return type encountered: {call.CallInfo.Method.ReturnType}");
        }
    }
}