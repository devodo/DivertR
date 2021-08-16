using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal class RecordCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private static readonly ConcurrentDictionary<MethodInfo, RecordedCallFactory<TTarget>> CallFactories = new ConcurrentDictionary<MethodInfo, RecordedCallFactory<TTarget>>();
        
        private readonly IRelay<TTarget> _relay;
        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public RecordCallHandler(IRelay<TTarget> relay)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
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

        public ICallStream<TTarget> CreateCallStream()
        {
            return new CallStream<TTarget>(_recordedCalls);
        }

        private static RecordedCallFactory<TTarget> GetRecordedCallFactory(MethodInfo callMethod)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            var callFactory = CallFactories.GetOrAdd(callMethod, method =>
            {
                var returnType = method.ReturnType;

                if (returnType == typeof(void))
                {
                    returnType = typeof(object);
                }

                var methodParameters = method.GetParameters();
                var genericTypes = new[] {typeof(TTarget), returnType}
                    .Concat(methodParameters
                        .Select(x => !x.ParameterType.IsByRef ? x.ParameterType : x.ParameterType.GetElementType()));

                var genericType = methodParameters.Length switch
                {
                    0 => typeof(RecordedCallFactory<,>),
                    1 => typeof(RecordedCallFactory<,,>),
                    2 => typeof(RecordedCallFactory<,,,>),
                    3 => typeof(RecordedCallFactory<,,,,>),
                    4 => typeof(RecordedCallFactory<,,,,,>),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                var factoryType = genericType.MakeGenericType(genericTypes.Take(8).ToArray());
                return (RecordedCallFactory<TTarget>) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);
            });

            return callFactory;
        }
    }
}
