using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class FuncCallLog<TTarget, TReturn> : CallLog<IFuncRecordedCall<TTarget, TReturn>>, IFuncCallLog<TTarget, TReturn> where TTarget : class
    {
        public FuncCallLog(IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn>> recordedCalls) : base(recordedCalls)
        {
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn>, TMap>(Calls, mapper);
            
            return new CallLog<TMap>(mappedCollection);
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn>, TMap>(Calls, call => mapper.Invoke(call, call.Args));
            
            return new CallLog<TMap>(mappedCollection);
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
    }

    internal class FuncCallLog<TTarget, TReturn, TArgs> : CallLog<IFuncRecordedCall<TTarget, TReturn, TArgs>>, IFuncCallLog<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public FuncCallLog(IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>> recordedCalls) : base(recordedCalls)
        {
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap>(Calls, mapper);
            
            return new CallLog<TMap>(mappedCollection);
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap>(Calls,
                call => mapper.Invoke(call, call.Args));
            
            return new CallLog<TMap>(mappedCollection);
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