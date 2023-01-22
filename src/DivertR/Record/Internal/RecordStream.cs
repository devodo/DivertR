using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class RecordStream<TTarget> : CallStream<IRecordedCall<TTarget>>, IRecordStream, IRecordStream<TTarget> where TTarget : class?
    {
        public RecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls) : base(recordedCalls)
        {
        }
        
        public IRecordStream<TTarget> To(ICallConstraint<TTarget>? callConstraint = null)
        {
            if (callConstraint == null)
            {
                return this;
            }

            return new RecordStream<TTarget>(Calls.Where(x => callConstraint.IsMatch(x.CallInfo)));
        }
        
        public IFuncCallStream<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var callValidator = CallExpressionParser.FromExpression<TTarget>(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = Calls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TTarget, TReturn>(call));

            return new FuncCallStream<TTarget, TReturn>(calls, callValidator);
        }

        public IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var callValidator = CallExpressionParser.FromExpression<TTarget>(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = Calls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, callValidator);
        }

        public IActionCallStream<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> lambdaExpression, Expression<Func<TProperty>> valueExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));
            if (valueExpression.Body == null) throw new ArgumentNullException(nameof(valueExpression));

            if (lambdaExpression.Body is not MemberExpression propertyExpression)
            {
                throw new ArgumentException("Must be a property member expression", nameof(propertyExpression));
            }

            var parsedCall = CallExpressionParser.FromPropertySetter<TTarget>(propertyExpression, valueExpression.Body);
            var callConstraint = parsedCall.CreateCallConstraint();
            var calls = Calls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, parsedCall);
        }

        IEnumerator<IRecordedCall> IEnumerable<IRecordedCall>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        ICallStream<IRecordedCall> ICallStream<IRecordedCall>.Filter(Func<IRecordedCall, bool> predicate)
        {
            return (ICallStream<IRecordedCall>) base.Filter(predicate);
        }

        ICallStream<TMap> ICallStream<IRecordedCall>.Map<TMap>(Func<IRecordedCall, TMap> mapper)
        {
            return base.Map(mapper);
        }

        IVerifySnapshot<IRecordedCall> ICallStream<IRecordedCall>.Verify()
        {
            return base.Verify();
        }

        IVerifySnapshot<IRecordedCall> ICallStream<IRecordedCall>.Verify(Action<IRecordedCall> visitor)
        {
            return base.Verify();
        }

        async Task<IVerifySnapshot<IRecordedCall>> ICallStream<IRecordedCall>.Verify(Func<IRecordedCall, Task> visitor)
        {
            return await base.Verify(visitor);
        }

        IRecordStream IRecordStream.To(ICallConstraint? callConstraint)
        {
            if (callConstraint == null)
            {
                return this;
            }

            return new RecordStream(Calls.Where(x => callConstraint.IsMatch(x.CallInfo)));
        }

        IFuncCallStream<TReturn> IRecordStream.To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromProperty(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            var calls = Calls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TReturn>(call));

            return new FuncCallStream<TReturn>(calls);
        }
    }
    
    internal class RecordStream : CallStream<IRecordedCall>, IRecordStream
    {
        public RecordStream(IEnumerable<IRecordedCall> recordedCalls) : base(recordedCalls)
        {
        }

        public IRecordStream To(ICallConstraint? callConstraint = null)
        {
            if (callConstraint == null)
            {
                return this;
            }

            return new RecordStream(Calls.Where(x => callConstraint.IsMatch(x.CallInfo)));
        }
        
        public IFuncCallStream<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            if (constraintExpression.Body == null) throw new ArgumentNullException(nameof(constraintExpression));

            var callValidator = CallExpressionParser.FromProperty(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            var calls = Calls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TReturn>(call));

            return new FuncCallStream<TReturn>(calls);
        }
    }
}