﻿using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<T, TReturn> : RedirectBuilder<T>, IFuncRedirectBuilder<T, TReturn> where T : class
    {
        private readonly ParsedCall _parsedCall;

        public FuncRedirectBuilder(IVia<T> via, ParsedCall parsedCall)
            : base(via, parsedCall.CreateCallConstraint())
        {
            _parsedCall = parsedCall;
        }

        public override IVia<T> To(Delegate redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);

            return base.To(redirectDelegate);
        }

        public IVia<T> To(TReturn instance)
        {
            return To(() => instance);
        }
        
        public IVia<T> To(Func<TReturn> redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(args => redirectDelegate.Invoke(), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }

        public IVia<T> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(args => redirectDelegate.Invoke((T1) args[0]), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }

        public IVia<T> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(args => redirectDelegate.Invoke((T1) args[0], (T2) args[1]), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }
    }
}