using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : DelegateRedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class
    {
        protected readonly Relay<TTarget, TReturn> Relay;
        
        public FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression, parsedCallExpression.ToCallConstraint<TTarget>())
        {
            Relay = new Relay<TTarget, TReturn>(via.Relay);
        }
        
        protected FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint, Relay<TTarget, TReturn> relay)
            : base(via, parsedCallExpression, callConstraint)
        {
            Relay = relay;
        }
        
        public new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

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

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Redirect(() => instance, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            InsertRedirect(callInfo => redirectDelegate.Invoke(), optionsAction);
            
            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            object? CallHandler(CallInfo<TTarget> callInfo)
            {
                var redirectCall = new FuncRedirectCall<TTarget, TReturn>(callInfo, Relay);

                return redirectDelegate.Invoke(redirectCall);
            }
            
            InsertRedirect(CallHandler, optionsAction);

            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRedirectBuilder<TTarget, TReturn, TArgs>(Via, ParsedCallExpression, CallConstraint, Relay);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, ValueTuple<CallArguments, T1, T2>> Redirect<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            InsertRedirect(redirectDelegate, callInfo =>
                redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]));
            
            return new FuncRedirectBuilder<TTarget, TReturn, ValueTuple<CallArguments, T1, T2>>(Via, ParsedCallExpression, CallConstraint, Relay);
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

        public new IFuncRecordStream<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new FuncRecordStream<TTarget, TReturn>(recordStream, ParsedCallExpression);
        }

        public IFuncRecordStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }

        public IReadOnlyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            TMap Map(IRecordedCall<TTarget> recordedCall)
            {
                var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn>(recordedCall);

                return mapper.Invoke(funcRecordedCall);
            }

            return base.Spy(Map, optionsAction);
        }
    }

    internal class FuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn>, IFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleFactory _valueTupleFactory;
        
        public FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint, Relay<TTarget, TReturn> relay)
            : base(via, parsedCallExpression, callConstraint, relay)
        {
            _valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), _valueTupleFactory.ArgumentTypes);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleFactory, Relay, redirectDelegate);
            base.InsertRedirect(callHandler, optionsAction);

            return this;
        }

        public new IFuncRecordStream<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new FuncRecordStream<TTarget, TReturn, TArgs>(recordStream, ParsedCallExpression, _valueTupleFactory);
        }

        public IReadOnlyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            TMap Map(IRecordedCall<TTarget> recordedCall)
            {
                var args = (TArgs) _valueTupleFactory.Create(recordedCall.Args.InternalArgs);
                var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn, TArgs>(recordedCall, args);

                return mapper.Invoke(funcRecordedCall);
            }

            return base.Spy(Map, optionsAction);
        }
    }
}