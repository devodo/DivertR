﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : CallStream<IFuncRecordedCall<TTarget, TReturn>>, IFuncCallStream<TTarget, TReturn>
        where TTarget : class
    {
        protected readonly ParsedCallExpression ParsedCallExpression;
        
        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls)
        {
            ParsedCallExpression = parsedCallExpression;
        }
        
        public IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);
            
            return new FuncCallStream<TTarget, TReturn, TArgs>(mappedCall, ParsedCallExpression);
        }
        
        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }

        internal static IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> MapCalls<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
        {
            return calls.Select(call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }

    internal class FuncCallStream<TTarget, TReturn, TArgs> : CallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>, IFuncCallStream<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        protected readonly ParsedCallExpression ParsedCallExpression;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls)
        {
            ParsedCallExpression = parsedCallExpression;
        }

        public IFuncCallStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = FuncCallStream<TTarget, TReturn>.MapCalls<TNewArgs>(Calls, valueTupleFactory);
            
            return new FuncCallStream<TTarget, TReturn, TNewArgs>(mappedCall, ParsedCallExpression);
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, TArgs> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, TArgs, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }
    }
}