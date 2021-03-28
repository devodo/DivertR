using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RecordCallRedirect<T> : IRedirect<T>, ICallRecord<T> where T : class
    {
        private readonly IRelay<T> _relay;
        private readonly ICallConstraint<T> _callConstraint;

        private readonly ConcurrentQueue<IRecordedCall<T>> _recordedCalls = new ConcurrentQueue<IRecordedCall<T>>();

        public RecordCallRedirect(IRelay<T> relay, ICallConstraint<T>? callConstraint = null)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _callConstraint = callConstraint ?? TrueCallConstraint<T>.Instance;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            var recordedCall = new RecordedCall<T>(callInfo);
            _recordedCalls.Enqueue(recordedCall);
            
            var returnValue = _relay.CallNext(callInfo);
            recordedCall.ReturnValue = returnValue;

            return returnValue;
        }
        
        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }

        public ReadOnlyCollection<IRecordedCall<T>> Calls(ICallConstraint<T>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<T>.Instance;

            return Array.AsReadOnly(_recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToArray());
        }
        
        public ReadOnlyCollection<IRecordedCall<T>> Calls<TReturn>(Expression<Func<T, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return Calls(parsedCall.ToCallConstraint<T>());
        }
        
        public ReadOnlyCollection<IRecordedCall<T>> Calls(Expression<Action<T>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }
            
            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            return Calls(parsedCall.ToCallConstraint<T>());
        }
    }
}