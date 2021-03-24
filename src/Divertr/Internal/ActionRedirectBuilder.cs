﻿using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<T> : RedirectBuilder<T>, IActionRedirectBuilder<T> where T : class
    {
        private readonly ParsedCall _parsedCall;

        public ActionRedirectBuilder(IVia<T> via, ParsedCall parsedCall)
            : base(via, parsedCall.CreateCallConstraint())
        {
            _parsedCall = parsedCall;
        }
        
        public override IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);

            return base.BuildRedirect(redirectDelegate);
        }
        
        public IVia<T> To<T1>(Action<T1> redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            }, BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }
    }
}