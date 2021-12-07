using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class RedirectRepository<TTarget> where TTarget : class
    {
        private volatile RedirectPlan<TTarget> _redirectPlan = RedirectPlan<TTarget>.Empty;

        public RedirectPlan<TTarget> RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan;
        }

        public void InsertRedirect(Redirect<TTarget> redirect)
        {
            MutateRedirectPlan(original => original.InsertRedirect(redirect));
        }
        
        public void SetStrictMode(bool isStrict = true)
        {
            MutateRedirectPlan(original => original.SetStrictMode(isStrict));
        }

        public void Reset()
        {
            if (ReferenceEquals(_redirectPlan, RedirectPlan<TTarget>.Empty))
            {
                return;
            }
            
            Interlocked.Exchange(ref _redirectPlan, RedirectPlan<TTarget>.Empty);
        }

        private void MutateRedirectPlan(Func<RedirectPlan<TTarget>, RedirectPlan<TTarget>> mutateAction)
        {
            var lastRead = _redirectPlan;

            while (true)
            {
                var mutated = mutateAction(lastRead);
                var actual = Interlocked.CompareExchange(ref _redirectPlan, mutated, lastRead);

                if (ReferenceEquals(actual, lastRead))
                {
                    break;
                }

                lastRead = actual;
            }
        }
    }
}