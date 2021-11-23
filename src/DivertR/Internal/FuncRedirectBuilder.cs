using System;
using System.Collections;
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
        
        public Redirect<TTarget> Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(() => instance, optionsAction);
        }
        
        public Redirect<TTarget> Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ParsedCallExpression.Validate(redirectDelegate);
            var callHandler = new DelegateCallHandler<TTarget>(callInfo => redirectDelegate.Invoke());

            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn>(Relay, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public Redirect<TTarget> Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }
        
        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Redirect(() => instance, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

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
        
        public new IFuncRecordCollection<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new FuncRecordCollection<TTarget, TReturn>(recordStream, ParsedCallExpression);
        }

        public IFuncRecordCollection<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }

        public ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var spyMapper = new SpyFuncMapper<TTarget, TReturn, TMap>(mapper);

            return base.Spy(spyMapper.Map, optionsAction);
        }
    }

    internal class FuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn>, IFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint, Relay<TTarget, TReturn> relay)
            : base(via, parsedCallExpression, callConstraint, relay)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), _valueTupleMapper.ArgumentTypes);
        }

        public Redirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new FuncRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, Relay, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public new IFuncRecordCollection<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);

            return new FuncRecordCollection<TTarget, TReturn, TArgs>(recordStream, ParsedCallExpression, _valueTupleMapper);
        }

        public ISpyCollection<TMap> Spy<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var spyMapper = new SpyFuncMapper<TTarget, TReturn, TArgs, TMap>(_valueTupleMapper, mapper);

            return base.Spy(spyMapper.Map, optionsAction);
        }
    }
}