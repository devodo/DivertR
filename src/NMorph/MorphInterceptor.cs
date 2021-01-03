using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class MorphInterceptor<T> : IInterceptor where T : class
    {
        private readonly AlterationStore _alterationStore;
        private readonly T _originTarget;
        private readonly string _morphGroup;

        public MorphInterceptor(AlterationStore alterationStore, T originTarget, string morphGroup)
        {
            _alterationStore = alterationStore;
            _originTarget = originTarget;
            _morphGroup = morphGroup;
        }

        public void Intercept(IInvocation invocation)
        {
            var alteration = _alterationStore.GetAlteration<T>(_morphGroup);
            var morphTarget = alteration?.Substitute;

            if (morphTarget == null)
            {
                invocation.Proceed();
                return;
            }
            
            var invocationContext = new InvocationContext<T>(invocation, _originTarget);
            alteration.InvocationStack.Push(invocationContext);

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(morphTarget);
                invocation.Proceed();
            }
            finally
            {
                var poppedContext = alteration.InvocationStack.Pop();
                
                if (poppedContext == null)
                {
                    throw new Exception("Pop returned unexpected null invocation context");
                }

                if (!ReferenceEquals(poppedContext, invocationContext))
                {
                    throw new Exception("Pop returned unexpected invocation context");
                }
            }
            
        }
    }
}