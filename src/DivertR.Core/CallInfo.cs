using System.Reflection;

namespace DivertR.Core
{
    public class CallInfo<T> where T : class
    {
        public CallInfo(T viaProxy, T? original, MethodInfo method, CallArguments callArguments)
        {
            ViaProxy = viaProxy;
            Original = original;
            Method = method;
            Arguments = callArguments;
        }
        
        public CallInfo(T viaProxy, T? original, MethodInfo method, object[] args)
            : this(viaProxy, original, method, new CallArguments(args))
        {
        }

        public T ViaProxy { get; }
        
        public T? Original { get; }
        
        public MethodInfo Method { get; }

        public CallArguments Arguments { get; }
    }
}