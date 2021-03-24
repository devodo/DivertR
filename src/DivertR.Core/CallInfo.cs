using System.Collections.ObjectModel;
using System.Reflection;

namespace DivertR.Core
{
    public class CallInfo
    {
        public CallInfo(MethodInfo method, CallArguments callArguments)
        {
            Method = method;
            CallArguments = callArguments;
        }
        
        public CallInfo(MethodInfo method, object[] args)
            : this(method, new CallArguments(args))
        {
        }
        
        public MethodInfo Method { get; }

        public ReadOnlyCollection<object> Arguments => CallArguments.Arguments;

        internal CallArguments CallArguments { get; }
    }
}