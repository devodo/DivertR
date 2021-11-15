using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class RecordStream<TTarget> : IRecordStream<TTarget> where TTarget : class
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
        
        public IFuncRecordStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint<TTarget>();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new FuncRecordStream<TTarget, TReturn>(calls, parsedCall);
        }

        public IActionRecordStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint<TTarget>();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionRecordStream<TTarget>(calls, parsedCall);
        }

        public IActionRecordStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression?.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (!(lambdaExpression.Body is MemberExpression propertyExpression))
            {
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter(propertyExpression, valueExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint<TTarget>();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionRecordStream<TTarget>(calls, parsedCall);
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
