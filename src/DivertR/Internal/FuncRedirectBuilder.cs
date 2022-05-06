using System;
using System.Collections;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<TTarget, TReturn> : DelegateRedirectBuilder<TTarget>, IFuncRedirectBuilder<TTarget, TReturn> where TTarget : class
    {
        public FuncRedirectBuilder(IVia<TTarget> via, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(via, callValidator, callConstraint)
        {
        }
        
        public new IFuncRedirectBuilder<TTarget, TReturn> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            base.AddConstraint(callConstraint);

            return this;
        }
        
        public IRedirect<TTarget> Build(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => instance, optionsAction);
        }
        
        public IRedirect<TTarget> Build(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            return Build(call => redirectDelegate.Invoke(), optionsAction);
        }

        public IRedirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TTarget, TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var callHandler = new FuncArgsRedirectCallHandler<TTarget, TReturn>(redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public IRedirect<TTarget> Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public IRedirect<TTarget> Build<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Build(redirectDelegate, optionsAction);
        }

        public new IFuncRedirectBuilder<TTarget, TReturn> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
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

        public IFuncRedirectBuilder<TTarget, TReturn> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(instance, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Redirect(redirectDelegate, optionsAction);
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRedirectBuilder<TTarget, TReturn, TArgs>(Via, CallValidator, CallConstraint);
        }
        
        public new IFuncCallLog<TTarget, TReturn> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget>, IFuncRecordedCall<TTarget, TReturn>>(recordStream,
                call => new FuncRecordedCall<TTarget, TReturn>(call));

            return new FuncCallLog<TTarget, TReturn>(mappedCollection, CallValidator);
        }

        public IFuncCallLog<TTarget, TReturn, TArgs> Record<TArgs>(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Record(optionsAction);
        }
    }

    internal class FuncRedirectBuilder<TTarget, TReturn, TArgs> : FuncRedirectBuilder<TTarget, TReturn>, IFuncRedirectBuilder<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        
        public FuncRedirectBuilder(IVia<TTarget> via, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(via, callValidator, callConstraint)
        {
            _valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(_valueTupleMapper);
        }

        public IRedirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new FuncRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }
        
        public IRedirect<TTarget> Build(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new FuncArgsRedirectCallHandler<TTarget, TReturn, TArgs>(_valueTupleMapper, redirectDelegate);
            
            return base.Build(callHandler, optionsAction);
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(TReturn instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(instance, optionsAction);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public new IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            base.Redirect(redirectDelegate, optionsAction);

            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public IFuncRedirectBuilder<TTarget, TReturn, TArgs> Redirect(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        public new IFuncCallLog<TTarget, TReturn, TArgs> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordStream = ((RedirectBuilder<TTarget>) this).Record(optionsAction);
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget>, IFuncRecordedCall<TTarget, TReturn, TArgs>>(recordStream,
                call => new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs)));

            return new FuncCallLog<TTarget, TReturn, TArgs>(mappedCollection, CallValidator);
        }
    }
}