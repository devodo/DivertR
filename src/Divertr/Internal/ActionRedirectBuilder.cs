using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<T> : DelegateRedirectBuilder<T>, IActionRedirectBuilder<T> where T : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionRedirectBuilder(IVia<T> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression.ToCallConstraint<T>())
        {
            _parsedCallExpression = parsedCallExpression;
        }
        
        public override IRedirect<T> Build(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return base.Build(redirectDelegate);
        }
        
        public IVia<T> To(Action redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public IVia<T> To<T1>(Action<T1> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2>(Action<T1, T2> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]);
                return default;
            });
        }
        
        public IVia<T> To<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]);
                return default;
            });
        }

        private IVia<T> AddRedirect(Func<CallInfo<T>, object?> redirectDelegate)
        {
            var redirect = new DelegateRedirect<T>(redirectDelegate, BuildCallConstraint());

            return Via.InsertRedirect(redirect);
        }
    }
}