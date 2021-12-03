using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class ActionCallLog<TTarget> : CallLog<IRecordedCall<TTarget>>, IActionCallLog<TTarget> where TTarget : class
    {
        public ActionCallLog(IReadOnlyCollection<IRecordedCall<TTarget>> calls) : base(calls)
        {
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
        
        public int Replay(Action<IRecordedCall<TTarget>, CallArguments> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IRecordedCall<TTarget>, CallArguments, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }
    }

    internal class ActionCallLog<TTarget, TArgs> : CallLog<IRecordedCall<TTarget, TArgs>>, IActionCallLog<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ActionCallLog(IReadOnlyCollection<IRecordedCall<TTarget, TArgs>> recordedCalls)
            : base(recordedCalls)
        {
        }

        public ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget, TArgs>, TMap>(Calls, mapper);
            return new CallLog<TMap>(mappedCollection);
        }

        public ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget, TArgs>, TMap>(Calls, call => mapper.Invoke(call, call.Args));
            return new CallLog<TMap>(mappedCollection);
        }
        
        public int Replay(Action<IRecordedCall<TTarget>, TArgs> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, TArgs, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor)
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