using System;
using System.Collections;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ActionRedirectBuilder<TTarget> : DelegateRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget> where TTarget : class
    {
        public ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression, parsedCallExpression.ToCallConstraint<TTarget>())
        {
        }
        
        protected ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint)
            : base(via, parsedCallExpression, callConstraint)
        {
        }
        
        public new IActionRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }
        
        public Redirect<TTarget> Build(Action redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1>(Action<T1> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2>(Action<T1, T2> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]);
                return default;
            });
        }
        
        public Redirect<TTarget> Build<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate)
        {
            return Build(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect(Action redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke();
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1>(Action<T1> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2>(Action<T1, T2> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3>(Action<T1, T2, T3> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3, T4>(Action<T1, T2, T3, T4> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6]);
                return default;
            });
        }
        
        public IVia<TTarget> Redirect<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> redirectDelegate)
        {
            return InsertRedirect(redirectDelegate, callInfo =>
            {
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1], (T3) callInfo.Arguments[2], (T4) callInfo.Arguments[3], (T5) callInfo.Arguments[4], (T6) callInfo.Arguments[5], (T7) callInfo.Arguments[6], (T8) callInfo.Arguments[7]);
                return default;
            });
        }

        public IActionRedirectBuilder<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRedirectBuilder<TTarget, TArgs>(Via, ParsedCallExpression, CallConstraint);
        }

        public new IActionRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new ActionRecordStream<TTarget>(recordStream, ParsedCallExpression, skipValidation: true);
        }

        public IActionRecordStream<TTarget, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }
    }

    internal class ActionRedirectBuilder<TTarget, TArgs> : ActionRedirectBuilder<TTarget>, IActionRedirectBuilder<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleFactory _valueTupleFactory;
        
        public ActionRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint)
            : base(via, parsedCallExpression, callConstraint)
        {
            _valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();
            ParsedCallExpression.Validate(typeof(void), _valueTupleFactory.ArgumentTypes);
        }

        public IActionRedirectBuilder<TTarget, TArgs> Redirect(Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            object? CallHandler(CallInfo<TTarget> callInfo)
            {
                var args = (TArgs) _valueTupleFactory.Create(callInfo.Arguments);
                var redirectCall = new ActionRedirectCall<TTarget, TArgs>(callInfo, Via.Relay, args);
                redirectDelegate.Invoke(redirectCall);
                
                return default;
            }
            
            InsertRedirect(CallHandler, optionsAction);

            return this;
        }

        public new IActionRecordStream<TTarget, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new ActionRecordStream<TTarget, TArgs>(recordStream, ParsedCallExpression, skipValidation: true);
        }
    }
}