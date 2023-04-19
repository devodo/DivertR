using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ViaDecoratorCallHandler<TReturn> : ICallHandler
    {
        private readonly ICallHandler _callHandler;
        
        public ViaDecoratorCallHandler(Func<TReturn, TReturn> decorator)
        {
            var callHandlerType = typeof(TReturn).IsValueType
                ? typeof(ViaDecoratorStructCallHandler<>).MakeGenericType(typeof(TReturn))
                : typeof(ViaDecoratorClassCallHandler<>).MakeGenericType(typeof(TReturn));

            _callHandler = (ICallHandler) Activator.CreateInstance(callHandlerType, decorator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _callHandler.Handle(call);
        }
    }
    
    internal class ViaDecoratorClassCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly ConditionalWeakTable<TReturn, TReturn> _decoratedCache = new();
        private readonly Func<TReturn, TReturn> _decorator;

        public ViaDecoratorClassCallHandler(Func<TReturn, TReturn> decorator)
        {
            _decorator = decorator;
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var result = call.CallNext();

            if (result is null)
            {
                return _decorator.Invoke(default!);
            }

            if (result is not TReturn callReturn)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {result.GetType()}");
            }

            return _decoratedCache.GetValue(callReturn, x => _decorator.Invoke(x));
        }
    }
    
    internal class ViaDecoratorStructCallHandler<TReturn> : ICallHandler where TReturn : struct
    {
        private readonly Func<TReturn, TReturn> _decorator;

        public ViaDecoratorStructCallHandler(Func<TReturn, TReturn> decorator)
        {
            _decorator = decorator;
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Handle(IRedirectCall call)
        {
            var result = call.CallNext();

            if (result is not TReturn callReturn)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {result?.GetType()}");
            }

            return _decorator.Invoke(callReturn);
        }
    }
}