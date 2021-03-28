﻿using System.Collections;
using System.Collections.Generic;

namespace DivertR.Core
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

        public object this[int index] => InternalArgs[index];
    }
}