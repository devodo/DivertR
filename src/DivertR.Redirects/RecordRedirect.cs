using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class RecordRedirect<TTarget> : IRedirect<TTarget>, ICallStream<TTarget> where TTarget : class
    {
        private static readonly ConcurrentDictionary<MethodInfo, RecordedCallFactory<TTarget>> CallFactories = new ConcurrentDictionary<MethodInfo, RecordedCallFactory<TTarget>>();
        
        private readonly IVia<TTarget> _via;
        private readonly IRelay<TTarget> _relay;

        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public RecordRedirect(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            _relay = _via.Relay;
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }

        public ICallConstraint<TTarget> CallConstraint { get; }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            var returnType = callInfo.Method.ReturnType;

            if (returnType == typeof(void))
            {
                returnType = typeof(object);
            }
            
            var callFactory = CallFactories.GetOrAdd(callInfo.Method, type =>
            {
                var factoryType = typeof(RecordedCallFactory<,,>).MakeGenericType(typeof(TTarget), returnType, callInfo.Method.GetParameters()[0].ParameterType);
                return (RecordedCallFactory<TTarget>) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);
            });
            
            var recordedCall = callFactory.Create(callInfo);
            _recordedCalls.Enqueue(recordedCall);
            object? returnValue;

            try
            {
                returnValue = _relay.CallNext(callInfo);
                recordedCall.SetReturned(returnValue);
            }
            catch (Exception ex)
            {
                recordedCall.SetException(ex);
                throw;
            }
            
            return returnValue;
        }

        public IReadOnlyList<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<TTarget>.Instance;
            
            return Array.AsReadOnly(_recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToArray());
        }
        
        public IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            var callConstraint = _via.To(lambdaExpression).CallConstraint;
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Cast<RecordedCall<TTarget, TReturn>>()
                .ToArray();

            return new FuncCallStream<TTarget, TReturn>(calls);
        }

        public IReadOnlyList<IRecordedCall<TTarget>> To(Expression<Action<TTarget>> lambdaExpression)
        {
            return To(_via.To(lambdaExpression).CallConstraint);
        }

        public IReadOnlyList<IRecordedCall<TTarget>> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            return To(_via.ToSet(lambdaExpression, valueExpression).CallConstraint);
        }
        
        public int Count => _recordedCalls.Count;

        public IEnumerator<IRecordedCall<TTarget>> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
