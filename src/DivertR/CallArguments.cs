using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallArguments : IReadOnlyList<object>
    {
        public CallArguments(object[] args)
        {
            InternalArgs = args;
        }
        
        internal object[] InternalArgs { get; }
        
        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object?>) InternalArgs).GetEnumerator()!;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => InternalArgs.Length;

        public object this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InternalArgs[index];
        }

        public static implicit operator CallArguments(object[] args) => new CallArguments(args);
    }
}