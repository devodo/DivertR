using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : DelegateRedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class
    {
        public FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression)
        {
        }
        
        public new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink)
        {
            base.ChainCallHandler(chainLink);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn> WithOrderWeight(int orderWeight)
        {
            base.WithOrderWeight(orderWeight);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn> WithExcludeStrict(bool excludeStrict = true)
        {
            base.WithExcludeStrict(excludeStrict);

            return this;
        }

        public Redirect<TTarget> Build(TReturn instance)
        {
            return Build(() => instance);
        }
        
        public Redirect<TTarget> Build(Func<TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo => redirectDelegate.Invoke());
        }

        public Redirect<TTarget> Build<T1>(Func<T1, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0]));
        }

        public Redirect<TTarget> Build<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]));
        }

        public Redirect<TTarget> Build<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]));
        }

        public Redirect<TTarget> Build<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]));
        }

        public Redirect<TTarget> Build<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]));
        }

        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]));
        }

        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]));
        }

        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]));
        }

        public IVia<TTarget> Redirect(TReturn instance)
        {
            return Redirect(() => instance);
        }
        
        public IVia<TTarget> Redirect(Func<TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo => redirectDelegate.Invoke());
        }
        
        public IVia<TTarget> Redirect<T1>(Func<T1, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0]));
        }

        public IVia<TTarget> Redirect<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3>(Func<T1, T2, T3, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]));
        }

        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]));
        }
    }
}
