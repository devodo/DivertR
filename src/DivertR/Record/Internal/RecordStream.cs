﻿using System;
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
        
        public IEnumerable<IRecordedCall<TTarget>> To(ICallConstraint? callConstraint = null)
        {
            callConstraint ??= TrueCallConstraint.Instance;
            
            return _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));
        }
        
        public IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint();
            var calls = _recordedCalls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TTarget, TReturn>(call));

            return new FuncCallStream<TTarget, TReturn>(calls, parsedCall);
        }

        public IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var parsedCall = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = parsedCall.ToCallConstraint();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, parsedCall);
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
            var callConstraint = parsedCall.ToCallConstraint();
            var calls = _recordedCalls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, parsedCall);
        }

        public ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper)
        {
            var mappedCalls = new MappedCollection<IRecordedCall<TTarget>, TMap>(this, mapper);
            return new CallLog<TMap>(mappedCalls);
        }

        public ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper)
        {
            var mappedCalls = new MappedCollection<IRecordedCall<TTarget>, TMap>(this, call => mapper.Invoke(call, call.Args));
            return new CallLog<TMap>(mappedCalls);
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
