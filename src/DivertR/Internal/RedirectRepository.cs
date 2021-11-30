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
            Interlocked.Exchange(ref _redirectPlan, RedirectPlan<TTarget>.Empty);
        }

        private void MutateRedirectPlan(Func<RedirectPlan<TTarget>, RedirectPlan<TTarget>> mutateAction)
        {
            RedirectPlan<TTarget> original, mutated;
            
            do
            {
                original = _redirectPlan;
                mutated = mutateAction(original);
            } while (original != Interlocked.CompareExchange(ref _redirectPlan, mutated, original));
        }
    }
}