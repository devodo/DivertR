using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : DelegateRedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression.ToCallConstraint<TTarget>())
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public override IRedirect<TTarget> Build(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return base.Build(redirectDelegate);
        }

        public IRedirect<TTarget> Build(TReturn instance)
        {
            return Build(() => instance);
        }
        
        public IRedirect<TTarget> Build(Func<TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo => redirectDelegate.Invoke());
        }

        public IVia<TTarget> To(TReturn instance)
        {
            return To(() => instance);
        }

        public IVia<TTarget> To(Func<TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo => redirectDelegate.Invoke());
        }

        public IVia<TTarget> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0]));
        }

        public IVia<TTarget> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]));
        }

        public IVia<TTarget> To<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]));
        }

        public IVia<TTarget> To<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]));
        }

        public IVia<TTarget> To<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]));
        }

        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]));
        }

        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]));
        }

        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate)
        {
            return AddRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]));
        }
        
        private IRedirect<TTarget> Build(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            _parsedCallExpression.Validate(inputDelegate);
            
            return new DelegateRedirect<TTarget>(mappedRedirect, BuildCallConstraint());
        }

        private IVia<TTarget> AddRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return InsertRedirect(redirect);
        }
    }
}