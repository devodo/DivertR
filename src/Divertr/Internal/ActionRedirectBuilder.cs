using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<T> : RedirectBuilder<T>, IActionRedirectBuilder<T> where T : class
    {
        private readonly ParsedCall<T> _parsedCall;

        public ActionRedirectBuilder(IVia<T> via, ParsedCall<T> parsedCall)
            : base(via, parsedCall.CallConstraint)
        {
            _parsedCall = parsedCall;
        }
        
        public override IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);

            return base.BuildRedirect(redirectDelegate);
        }
        
        public IVia<T> To(Action redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public IVia<T> To<T1>(Action<T1> redirectDelegate)
        {
            _parsedCall.Validate(redirectDelegate);
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }

        private IVia<T> AddRedirect(Func<CallInfo<T>, object?> redirectDelegate)
        {
            var redirect = new DelegateRedirect<T>(redirectDelegate, BuildCallConstraint());

            return Via.AddRedirect(redirect);
        }
    }
}