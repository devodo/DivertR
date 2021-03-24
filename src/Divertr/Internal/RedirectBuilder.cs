using System;
using System.Collections.Generic;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> Via;
        private readonly List<ICallConstraint> _callConstraints = new List<ICallConstraint>();

        public RedirectBuilder(IVia<T> via, ICallConstraint? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                _callConstraints.Add(callConstraint);
            }
        }
        
        public ICallConstraint BuildCallConstraint()
        {
            return _callConstraints.Count switch
            {
                0 => TrueCallConstraint.Instance,
                1 => _callConstraints[0],
                _ => new CompositeCallConstraint(_callConstraints)
            };
        }

        public IRedirect<T> BuildRedirect(T target)
        {
            return new TargetRedirect<T>(target, BuildCallConstraint());
        }
        
        public virtual IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            return new DelegateRedirect<T>(callInfo => 
                redirectDelegate.GetMethodInfo().ToDelegate(redirectDelegate.GetType()).Invoke(redirectDelegate, callInfo.CallArguments.InternalArgs), BuildCallConstraint());
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