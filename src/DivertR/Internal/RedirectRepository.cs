using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository<TTarget> where TTarget : class
    {
        private readonly object _redirectLock = new object();
        private RedirectPlan<TTarget>? _redirectPlan;
        private List<Redirect<TTarget>> _insertedRedirects = new List<Redirect<TTarget>>();

        public RedirectPlan<TTarget>? RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                lock (_redirectLock)
                {
                    return _redirectPlan;
                }
            }
        }

        public void InsertRedirect(Redirect<TTarget> redirect)
        {
            lock (_redirectLock)
            {
                _insertedRedirects.Add(redirect);

                var redirects = _insertedRedirects
                    .Select((r, i) => (r, i))
                    .OrderByDescending(x => x, RedirectComparer.Instance)
                    .Select(x => x.r)
                    .ToArray();

                var isStrict = _redirectPlan?.IsStrictMode ?? false;
                _redirectPlan = new RedirectPlan<TTarget>(redirects, isStrict);
            }
        }
        
        public void SetStrictMode(bool isStrict = true)
        {
            lock (_redirectLock)
            {
                var redirects = _redirectPlan?.Redirects ?? Array.Empty<Redirect<TTarget>>();
                _redirectPlan = new RedirectPlan<TTarget>(redirects, isStrict);
            }
        }

        public void Reset()
        {
            lock (_redirectLock)
            {
                _insertedRedirects = new List<Redirect<TTarget>>();
                _redirectPlan = null;
            }
        }

        private class RedirectComparer : IComparer<(Redirect<TTarget> Redirect, int StackOrder)>
        {
            public static readonly RedirectComparer Instance = new RedirectComparer();
            
            public int Compare((Redirect<TTarget> Redirect, int StackOrder) x, (Redirect<TTarget> Redirect, int StackOrder) y)
            {
                var weightComparison = x.Redirect.OrderWeight.CompareTo(y.Redirect.OrderWeight);
                
                if (weightComparison != 0)
                {
                    return weightComparison;
                }

                return x.StackOrder.CompareTo(y.StackOrder);
            }
        }
    }
}
