using System.Reflection;

namespace DivertR
{
    public class CallInfo<TTarget> where TTarget : class
    {
        public CallInfo(TTarget proxy, TTarget? original, MethodInfo method, CallArguments args)
        {
            Proxy = proxy;
            Original = original;
            Method = method;
            Arguments = args;
        }
        
        public CallInfo(TTarget proxy, TTarget? original, MethodInfo method, object[] args)
            : this(proxy, original, method, new CallArguments(args))
        {
        }

        public TTarget Proxy { get; }
        
        public TTarget? Original { get; }
        
        public MethodInfo Method { get; }

        public CallArguments Arguments { get; }
    }
}