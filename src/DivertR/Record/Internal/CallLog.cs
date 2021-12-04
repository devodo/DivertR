using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal class CallLog<TMap> : CallStream<TMap>, ICallLog<TMap>
    {
        public CallLog(IReadOnlyCollection<TMap> calls) : base(calls)
        {
        }

        protected new IReadOnlyCollection<TMap> Calls => (IReadOnlyCollection<TMap>) base.Calls;

        public int Count => Calls.Count;
    }
}