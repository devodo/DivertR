using System;
using System.Collections.Generic;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> Via;
        private readonly List<ICallConstraint<T>> _callConstraints = new List<ICallConstraint<T>>();

        public RedirectBuilder(IVia<T> via, ICallConstraint<T>? callConstraint = null)
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

        public IRedirect<T> BuildRedirect(T target)
        {
            return new TargetRedirect<T>(target, BuildCallConstraint());
        }
        
        public virtual IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            var fastDelegate = redirectDelegate.GetMethodInfo().ToDelegate(redirectDelegate.GetType());
            return new DelegateRedirect<T>(callInfo => fastDelegate.Invoke(redirectDelegate, callInfo.Arguments.InternalArgs), BuildCallConstraint());
        }

        public IVia<T> To(T target)
        {
            var redirect = BuildRedirect(target);
            Via.AddRedirect(redirect);
            
            return Via;
        }
        
        public IVia<T> To(Delegate redirectDelegate)
        {
            var redirect = BuildRedirect(redirectDelegate);
            
            return Via.AddRedirect(redirect);
        }
    }
}