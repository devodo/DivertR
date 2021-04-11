using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Redirects
{
    internal class CallRecordRedirect<TTarget> : IRedirect<TTarget>, ICallRecord<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IRelay<TTarget> _relay;

        private readonly ConcurrentQueue<IRecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<IRecordedCall<TTarget>>();

        public CallRecordRedirect(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            _relay = _via.Relay;
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }

        public ICallConstraint<TTarget> CallConstraint { get; }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            var recordedCall = new RecordedCall<TTarget>(callInfo);
            _recordedCalls.Enqueue(recordedCall);
            
            var returnValue = _relay.CallNext(callInfo);
            recordedCall.ReturnValue = returnValue;

            return returnValue;
        }

        public IReadOnlyList<IRecordedCall<TTarget>> Calls(ICallConstraint<TTarget>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<TTarget>.Instance;

            return Array.AsReadOnly(_recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToArray());
        }
        
        public IReadOnlyList<IRecordedCall<TTarget>> Calls<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            return Calls(_via.Redirect(lambdaExpression).CallConstraint);
        }
        
        public IReadOnlyList<IRecordedCall<TTarget>> Calls(Expression<Action<TTarget>> lambdaExpression)
        {
            return Calls(_via.Redirect(lambdaExpression).CallConstraint);
        }

        public IReadOnlyList<IRecordedCall<TTarget>> CallsSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            return Calls(_via.RedirectSet(lambdaExpression, valueExpression).CallConstraint);
        }
    }
}