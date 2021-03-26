using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class CallCaptureRedirect<T> : IRedirect<T>, ICallCapture<T> where T : class
    {
        private readonly IRelay<T> _relay;
        private readonly ICallConstraint<T> _callConstraint;
        private readonly object _lockObject = new object();

        private readonly List<CapturedCall<T>> _calls = new List<CapturedCall<T>>();

        public CallCaptureRedirect(IRelay<T> relay, ICallConstraint<T>? callConstraint = null)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _callConstraint = callConstraint ?? TrueCallConstraint<T>.Instance;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            var returnValue = _relay.CallNext(callInfo);
            
            lock (_lockObject)
            {
                _calls.Add(new CapturedCall<T>(callInfo, returnValue));
            }

            return returnValue;
        }
        
        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }

        public List<CapturedCall<T>> Calls(ICallConstraint<T>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<T>.Instance;

            lock (_lockObject)
            {
                return _calls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToList();
            }
        }
        
        public List<CapturedCall<T>> Calls<TReturn>(Expression<Func<T, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }

            var parsedCall = CallExpressionParser<T>.FromExpression(lambdaExpression.Body);
            return Calls(parsedCall.CallConstraint);
        }
        
        public List<CapturedCall<T>> Calls(Expression<Action<T>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }
            
            var parsedCall = CallExpressionParser<T>.FromExpression(lambdaExpression.Body);
            return Calls(parsedCall.CallConstraint);
        }
    }
}