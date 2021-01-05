using Castle.DynamicProxy;

namespace NMorph
{
    internal class MorphInterceptor<T> : IInterceptor where T : class
    {
        private readonly AlterationStore _alterationStore;
        private readonly T _originTarget;
        private readonly string _morphGroup;

        public MorphInterceptor(AlterationStore alterationStore, T originTarget, string morphGroup = null)
        {
            _alterationStore = alterationStore;
            _originTarget = originTarget;
            _morphGroup = morphGroup;
        }

        public void Intercept(IInvocation invocation)
        {
            var alteration = _alterationStore.GetAlteration<T>(_morphGroup);
            var invocationState = alteration?.CreateInvocationState(_originTarget, invocation);
            var substitution = invocationState?.Previous();

            if (substitution == null)
            {
                invocation.Proceed();
                return;
            }
            
            alteration.InvocationStack.Push(invocationState);

            try
            {
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(substitution.Substitute);
                invocation.Proceed();
            }
            finally
            {
                var poppedContext = alteration.InvocationStack.Pop();
                
                if (poppedContext == null)
                {
                    throw new MorphException("Fatal error: Encountered an unexpected null invocation state");
                }

                if (!ReferenceEquals(poppedContext, invocationState))
                {
                    throw new MorphException("Fatal error: Encountered an unexpected invocation state");
                }
            }
            
        }
    }
}