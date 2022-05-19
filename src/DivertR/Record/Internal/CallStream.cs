using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallStream<T> : ICallStream<T>
    {
        protected readonly IEnumerable<T> Calls;

        public CallStream(IEnumerable<T> calls)
        {
            Calls = calls;
        }

        public ICallStream<TMap> Map<TMap>(Func<T, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(mapper.Invoke));
        }

        public IReplayResult Replay(Action<T> visitor)
        {
            return new ReplayResult(this.Select(call =>
            {
                visitor.Invoke(call);

                return call;
            }).Count());
        }

        public IReplayResult Replay(Action<T, int> visitor)
        {
            return new ReplayResult(this.Select((call, i) =>
            {
                visitor.Invoke(call, i);

                return call;
            }).Count());
        }
        
        public async Task<IReplayResult> Replay(Func<T, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call).ConfigureAwait(false);
                count++;
            }

            return new ReplayResult(count);
        }

        public async Task<IReplayResult> Replay(Func<T, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in this)
            {
                await visitor.Invoke(call, count++).ConfigureAwait(false);
            }

            return new ReplayResult(count);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return Calls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    internal class CallStream<TRecordedCall, TTarget> : CallStream<TRecordedCall>
        where TRecordedCall : IRecordedCall<TTarget>
        where TTarget : class
    {
        protected CallStream(IEnumerable<TRecordedCall> calls) : base(calls)
        {
        }

        public ICallStream<TMap> Map<TMap>(Func<TRecordedCall, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IReplayResult Replay(Action<TRecordedCall, CallArguments> visitor)
        {
            return new ReplayResult(Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count());
        }

        public IReplayResult Replay(Action<TRecordedCall, CallArguments, int> visitor)
        {
            return new ReplayResult(Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count());
        }

        public async Task<IReplayResult> Replay(Func<TRecordedCall, CallArguments, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return new ReplayResult(count);
        }

        public async Task<IReplayResult> Replay(Func<TRecordedCall, CallArguments, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return new ReplayResult(count);
        }
    }

    internal class CallStream<TRecordedCall, TTarget, TArgs> : CallStream<TRecordedCall>
        where TRecordedCall : IRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
        protected CallStream(IEnumerable<TRecordedCall> calls) : base(calls)
        {
        }

        public ICallStream<TMap> Map<TMap>(Func<TRecordedCall, TArgs, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IReplayResult Replay(Action<TRecordedCall, TArgs> visitor)
        {
            return new ReplayResult(Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count());
        }

        public IReplayResult Replay(Action<TRecordedCall, TArgs, int> visitor)
        {
            return new ReplayResult(Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count());
        }

        public async Task<IReplayResult> Replay(Func<TRecordedCall, TArgs, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return new ReplayResult(count);
        }

        public async Task<IReplayResult> Replay(Func<TRecordedCall, TArgs, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return new ReplayResult(count);
        }
    }
}