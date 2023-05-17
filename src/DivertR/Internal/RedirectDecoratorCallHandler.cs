using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectDecoratorCallHandler<TReturn> : ICallHandler
    {
        private readonly ICallHandler _callHandler;
        
        public RedirectDecoratorCallHandler(IDiverter diverter, Func<TReturn, IDiverter, TReturn> decorator)
        {
            var callHandlerType = typeof(TReturn).IsValueType
                ? typeof(RedirectDecoratorStructCallHandler<>).MakeGenericType(typeof(TReturn))
                : typeof(RedirectDecoratorClassCallHandler<>).MakeGenericType(typeof(TReturn));

            _callHandler = (ICallHandler) Activator.CreateInstance(callHandlerType, diverter, decorator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _callHandler.Handle(call);
        }
    }
    
    internal class RedirectDecoratorClassCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly ConditionalWeakTable<TReturn, TReturn> _decoratedCache = new();
        private readonly IDiverter _diverter;
        private readonly Func<TReturn, IDiverter, TReturn> _decorator;

        public RedirectDecoratorClassCallHandler(IDiverter diverter, Func<TReturn, IDiverter, TReturn> decorator)
        {
            _diverter = diverter;
            _decorator = decorator;
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var result = call.CallNext();

            if (result is null)
            {
                return _decorator.Invoke(default!, _diverter);
            }

            if (result is not TReturn callReturn)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {result.GetType()}");
            }

            return _decoratedCache.GetValue(callReturn, x => _decorator.Invoke(x, _diverter));
        }
    }
    
    internal class RedirectDecoratorStructCallHandler<TReturn> : ICallHandler where TReturn : struct
    {
        private readonly IDiverter _diverter;
        private readonly Func<TReturn, IDiverter, TReturn> _decorator;

        public RedirectDecoratorStructCallHandler(IDiverter diverter, Func<TReturn, IDiverter, TReturn> decorator)
        {
            _diverter = diverter;
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

            return _decorator.Invoke(callReturn, _diverter);
        }
    }
}