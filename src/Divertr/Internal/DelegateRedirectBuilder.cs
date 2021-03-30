using System;
using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirectBuilder<T> : IDelegateRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> Via;
        private readonly List<ICallConstraint<T>> _callConstraints = new List<ICallConstraint<T>>();

        public DelegateRedirectBuilder(IVia<T> via, ICallConstraint<T>? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                _callConstraints.Add(callConstraint);
            }
        }
        
        public ICallConstraint<T> BuildCallConstraint()
        {
            return _callConstraints.Count switch
            {
                0 => TrueCallConstraint<T>.Instance,
                1 => _callConstraints[0],
                _ => new CompositeCallConstraint<T>(_callConstraints)
            };
        }
        
        public IRedirect<T> Build(T target)
        {
            return new TargetRedirect<T>(target, BuildCallConstraint());
        }

        public virtual IRedirect<T> Build(Delegate redirectDelegate)
        {
            var fastDelegate = redirectDelegate.Method.ToDelegate(redirectDelegate.GetType());
            return new DelegateRedirect<T>(callInfo => fastDelegate.Invoke(redirectDelegate, callInfo.Arguments.InternalArgs), BuildCallConstraint());
            //return new DelegateRedirect<T>(callInfo => redirectDelegate.DynamicInvoke(callInfo.Arguments.InternalArgs), BuildCallConstraint());
        }

        public IVia<T> To(T target)
        {
            var redirect = Build(target);
            
            return Via.InsertRedirect(redirect);
        }
        
        public IVia<T> To(Delegate redirectDelegate)
        {
            var redirect = Build(redirectDelegate);
            
            return Via.InsertRedirect(redirect);
        }

        protected IVia<T> InsertRedirect(IRedirect<T> redirect)
        {
            return Via.InsertRedirect(redirect);
        }
    }
}