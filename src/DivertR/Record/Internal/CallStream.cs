using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class CallStream<TTarget> : ICallStream<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IReadOnlyCollection<RecordedCall<TTarget>> _recordedCalls;

        public CallStream(IVia<TTarget> via, IReadOnlyCollection<RecordedCall<TTarget>> recordedCalls)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            _recordedCalls = recordedCalls;
        }
        
        public IReadOnlyList<IRecordedCall<TTarget>> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint<TTarget>.Instance;
            
            return Array.AsReadOnly(_recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo)).ToArray());
        }
        
        public IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression?.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint<TTarget>();
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Cast<RecordedCall<TTarget, TReturn>>()
                .ToArray();

            return new FuncCallStream<TTarget, TReturn>(parsedCall, calls);
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
