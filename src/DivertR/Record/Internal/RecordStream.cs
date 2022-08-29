using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class RecordStream<TTarget> : CallStream<IRecordedCall<TTarget>>, IRecordStream<TTarget> where TTarget : class?
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

            var callValidator = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = Calls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TTarget, TReturn>(call));

            return new FuncCallStream<TTarget, TReturn>(calls, callValidator);
        }

        public IActionCallStream<TTarget> To(Expression<Action<TTarget>> lambdaExpression)
        {
            if (lambdaExpression.Body == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var callValidator = CallExpressionParser.FromExpression(lambdaExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            var calls = Calls.Where(x => callConstraint.IsMatch(x.CallInfo));

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
            var calls = Calls.Where(x => callConstraint.IsMatch(x.CallInfo));

            return new ActionCallStream<TTarget>(calls, parsedCall);
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IRecordedCall<TTarget>> Verify(Action<IRecordedCall<TTarget>, CallArguments> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget>>> Verify(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
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

            var callValidator = CallExpressionParser.FromExpression(constraintExpression.Body);
            var callConstraint = callValidator.CreateCallConstraint();
            
            var calls = Calls
                .Where(x => callConstraint.IsMatch(x.CallInfo))
                .Select(call => new FuncRecordedCall<TReturn>(call));

            return new FuncCallStream<TReturn>(calls);
        }
        
        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(this.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IRecordedCall> Verify(Action<IRecordedCall, CallArguments> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IRecordedCall>> Verify(Func<IRecordedCall, CallArguments, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }
    }
}
