using System.Reflection;

namespace DivertR.Core
{
    public class CallInfo<TTarget> where TTarget : class
    {
        public CallInfo(TTarget viaProxy, TTarget? original, MethodInfo method, CallArguments callArguments)
        {
            ViaProxy = viaProxy;
            Original = original;
            Method = method;
            Arguments = callArguments;
        }
        
        public CallInfo(TTarget viaProxy, TTarget? original, MethodInfo method, object[] args)
            : this(viaProxy, original, method, new CallArguments(args))
        {
        }

        public TTarget ViaProxy { get; }
        
        public TTarget? Original { get; }
        
        public MethodInfo Method { get; }

        public CallArguments Arguments { get; }
    }
}