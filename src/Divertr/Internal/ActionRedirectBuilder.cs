using System;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<TTarget> : DelegateRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget> where TTarget : class
    {
        public ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression)
        {
        }
        
        public new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }

        public new IActionRedirectBuilder<TTarget> WithOrderWeight(int orderWeight)
        {
            base.WithOrderWeight(orderWeight);

            return this;
        }
        
        public new IActionRedirectBuilder<TTarget> AddRedirectDecorator(Func<IRedirect<TTarget>, IRedirect<TTarget>> decorator)
        {
            base.AddRedirectDecorator(decorator);

            return this;
        }

        public IRedirect<TTarget> Build(Action redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1>(Action<T1> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2>(Action<T1, T2> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]);
                return default;
            });
        }
        
        public IRedirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]);
                return default;
            });
        }
        
        public IVia<TTarget> To(Action redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public IVia<TTarget> To<T1>(Action<T1> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2>(Action<T1, T2> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]);
                return default;
            });
        }
        
        public IVia<TTarget> To<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]);
                return default;
            });
        }
    }
}
