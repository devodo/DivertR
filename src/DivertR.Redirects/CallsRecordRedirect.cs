using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class CallsRecordRedirect<TTarget> : IRedirect<TTarget>, ICallsRecord<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IRelay<TTarget> _relay;

        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public CallsRecordRedirect(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            _relay = _via.Relay;
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }

        public ICallConstraint<TTarget> CallConstraint { get; }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            var callReturn = new CallReturn();
            var recordedCall = new RecordedCall<TTarget>(callInfo, callReturn);
            _recordedCalls.Enqueue(recordedCall);
            
            var returnValue = _relay.CallNext(callInfo);
            callReturn.ReturnedObject = returnValue;

            return returnValue;
        }

        public IReadOnlyList<RecordedCall<TTarget>> All => Matching(TrueCallConstraint<TTarget>.Instance);

        public IReadOnlyList<RecordedCall<TTarget>> Matching(ICallConstraint<TTarget> callConstraint)
        {
            return Array.AsReadOnly(_recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToArray());
        }
        
        public IReadOnlyList<RecordedCall<TTarget, TReturn>> Matching<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            var callConstraint = _via.Redirect(lambdaExpression).CallConstraint;
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(x => new RecordedCall<TTarget, TReturn>(x.CallInfo, x.CallReturn))
                .ToArray();

            return Array.AsReadOnly(calls);
        }

        public IReadOnlyList<RecordedCall<TTarget>> Matching(Expression<Action<TTarget>> lambdaExpression)
        {
            return Matching(_via.Redirect(lambdaExpression).CallConstraint);
        }

        public IReadOnlyList<RecordedCall<TTarget>> MatchSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            return Matching(_via.RedirectSet(lambdaExpression, valueExpression).CallConstraint);
        }
    }
}