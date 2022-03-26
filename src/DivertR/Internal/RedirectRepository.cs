using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class RedirectRepository
    {
        private volatile RedirectPlan _redirectPlan = RedirectPlan.Empty;

        public RedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan;
        }

        public void InsertRedirect(Redirect redirect)
        {
            MutateRedirectPlan(original => original.InsertRedirect(redirect));
        }
        
        public void SetStrictMode(bool isStrict = true)
        {
            MutateRedirectPlan(original => original.SetStrictMode(isStrict));
        }

        public void Reset()
        {
            if (ReferenceEquals(_redirectPlan, RedirectPlan.Empty))
            {
                return;
            }
            
            Interlocked.Exchange(ref _redirectPlan, RedirectPlan.Empty);
        }

        private void MutateRedirectPlan(Func<RedirectPlan, RedirectPlan> mutateAction)
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