using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CallRecordRedirect<TTarget> : IRedirect<TTarget>, ICallRecord<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;

        private readonly ConcurrentQueue<IRecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<IRecordedCall<TTarget>>();

        public CallRecordRedirect(IRelay<TTarget> relay, ICallConstraint<TTarget>? callConstraint = null)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
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
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);

            return Calls(parsedCall.ToCallConstraint<TTarget>());
        }
        
        public IReadOnlyList<IRecordedCall<TTarget>> Calls(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);

            return Calls(parsedCall.ToCallConstraint<TTarget>());
        }

        public IReadOnlyList<IRecordedCall<TTarget>> CallsSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Only property member expressions are valid input to CallsSet", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);
            return Calls(parsedCall.ToCallConstraint<TTarget>());
        }
    }
}