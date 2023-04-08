using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ViaDecoratorCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly ConditionalWeakTable<TReturn, TReturn> _decorateCache = new();
        private readonly Func<TReturn, TReturn> _divertFunc;
        
        public ViaDecoratorCallHandler(Func<TReturn, TReturn> divertFunc)
        {
            _divertFunc = divertFunc;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var result = call.CallNext();

            if (result == null)
            {
                return null;
            }
            
            if (result is not TReturn callReturn)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {result.GetType()}");
            }
            
            return _decorateCache.GetValue(callReturn, x =>
            {
                var diverted = _divertFunc.Invoke(x);

                return diverted;
            });
        }
    }
}