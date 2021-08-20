using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal class RecordCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private static readonly ConcurrentDictionary<Type, RecordedCallFactory<TTarget>> CallFactories = new ConcurrentDictionary<Type, RecordedCallFactory<TTarget>>();
        
        private readonly IRelay<TTarget> _relay;
        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public ICallStream<TTarget> CallStream { get; }

        public RecordCallHandler(IRelay<TTarget> relay)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            CallStream = new CallStream<TTarget>(_recordedCalls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var callFactory = GetRecordedCallFactory(callInfo.Method);
            var recordedCall = callFactory.Create(callInfo);
            _recordedCalls.Enqueue(recordedCall);
            object? returnValue;

            try
            {
                returnValue = _relay.CallNext();
                recordedCall.SetReturned(returnValue);
            }
            catch (Exception ex)
            {
                recordedCall.SetException(ex);
                throw;
            }
            
            return returnValue;
        }

        private static RecordedCallFactory<TTarget> GetRecordedCallFactory(MethodInfo callMethod)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            var callFactory = CallFactories.GetOrAdd(callMethod.ReturnType, returnType =>
            {
                if (returnType == typeof(void))
                {
                    returnType = typeof(object);
                }
                
                var genericTypes = new[] {typeof(TTarget), returnType};
                var factoryType = typeof(RecordedCallFactory<,>).MakeGenericType(genericTypes);
                return (RecordedCallFactory<TTarget>) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);
            });

            return callFactory;
        }
    }
}
