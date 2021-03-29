using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<T, TReturn> : RedirectBuilder<T>, IFuncRedirectBuilder<T, TReturn> where T : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncRedirectBuilder(IVia<T> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression.ToCallConstraint<T>())
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public override IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return base.BuildRedirect(redirectDelegate);
        }

        public IVia<T> To(TReturn instance)
        {
            return To(() => instance);
        }

        public IVia<T> To(Func<TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            return AddRedirect(callInfo => redirectDelegate.Invoke());
        }

        public IVia<T> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            return AddRedirect(callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0]));
        }

        public IVia<T> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]));
        }

        public IVia<T> To<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]));
        }

        public IVia<T> To<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]));
        }

        public IVia<T> To<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            
            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]));
        }

        public IVia<T> To<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]));
        }

        public IVia<T> To<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]));
        }

        public IVia<T> To<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return AddRedirect(callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]));
        }

        private IVia<T> AddRedirect(Func<CallInfo<T>, object?> redirectDelegate)
        {
            var redirect = new DelegateRedirect<T>(redirectDelegate, BuildCallConstraint());

            return Via.AddRedirect(redirect);
        }
    }
}