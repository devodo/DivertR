using System;
using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal class CallLog<T> : CallStream<T>, ICallLog<T>
    {
        public CallLog(IReadOnlyCollection<T> calls) : base(calls)
        {
        }
        
        public new ICallLog<TMap> Map<TMap>(Func<T, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<T, TMap>(Calls, mapper);
            
            return new CallLog<TMap>(mappedCollection);
        }

        private new IReadOnlyCollection<T> Calls => (IReadOnlyCollection<T>) base.Calls;

        public int Count => Calls.Count;
    }
}