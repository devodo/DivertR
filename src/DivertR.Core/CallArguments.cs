using System;
using System.Collections.ObjectModel;

namespace DivertR.Core
{
    public class CallArguments
    {
        public CallArguments(object[] args)
        {
            Arguments = Array.AsReadOnly(args);
            InternalArgs = args;
        }

        public ReadOnlyCollection<object> Arguments { get; }

        internal object[] InternalArgs { get; }
    }
}