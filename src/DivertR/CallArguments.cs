using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallArguments : IReadOnlyList<object>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallArguments(params object[] args)
        {
            InternalArgs = args;
        }

        internal object[] InternalArgs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object?>) InternalArgs).GetEnumerator()!;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InternalArgs.Length;
        }

        public object this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InternalArgs[index];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator CallArguments(object[] args) => new(args);
    }
}