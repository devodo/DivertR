using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class RecordStream<TTarget> : IRecordStream<TTarget> where TTarget : class?
    {
        private readonly IReadOnlyCollection<RecordedCall<TTarget>> _recordedCalls;

        public RecordStream(IReadOnlyCollection<RecordedCall<TTarget>> recordedCalls)
        {
            _recordedCalls = recordedCalls ?? throw new ArgumentNullException(nameof(recordedCalls));
        }
        
        public IEnumerable<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<TTarget>.Instance;
            
            return _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));
        }
        
        public IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var callValidator = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TTarget, TReturn>(call));

            return new FuncCallStream<TTarget, TReturn>(calls, callValidator);
        }

        public IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var callValidator = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, callValidator);
        }

        public IActionCallStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, parsedCall);
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(mapper));
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(call => mapper.Invoke(call, call.Args)));
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
    
    internal class RecordStream : IRecordStream
    {
        private readonly IReadOnlyCollection<RecordedCall> _recordedCalls;

        public RecordStream(IReadOnlyCollection<RecordedCall> recordedCalls)
        {
            _recordedCalls = recordedCalls ?? throw new ArgumentNullException(nameof(recordedCalls));
        }

        public IEnumerable<IRecordedCall> To(ICallConstraint? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint.Instance;
            
            return _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));
        }
        
        public IFuncCallStream<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TReturn>(call));

            return new FuncCallStream<TReturn>(calls);
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(mapper));
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(call => mapper.Invoke(call, call.Args)));
        }

        public int Count => _recordedCalls.Count;

        public IEnumerator<IRecordedCall> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
