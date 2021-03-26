using System.Collections.ObjectModel;
using System.Reflection;

namespace DivertR.Core
{
    public class CallInfo<T> where T : class
    {
        public CallInfo(T proxy, T? original, MethodInfo method, CallArguments callArguments)
        {
            Proxy = proxy;
            Original = original;
            Method = method;
            CallArguments = callArguments;
        }
        
        public CallInfo(T proxy, T? original, MethodInfo method, object[] args)
            : this(proxy, original, method, new CallArguments(args))
        {
        }

        public T Proxy { get; }
        public T? Original { get; }
        public MethodInfo Method { get; }

        public ReadOnlyCollection<object> Arguments => CallArguments.Arguments;

        internal CallArguments CallArguments { get; }
    }
}