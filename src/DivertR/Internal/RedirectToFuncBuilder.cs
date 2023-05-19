using System;
using System.Collections;
using System.Linq;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class RedirectToFuncBuilder<TTarget, TReturn> : RedirectToBuilder<TTarget>, IRedirectToFuncBuilder<TTarget, TReturn> where TTarget : class?
    {
        private readonly FuncViaBuilder<TTarget, TReturn> _viaBuilder;
        
        public RedirectToFuncBuilder(Redirect<TTarget> redirect, FuncViaBuilder<TTarget, TReturn> viaBuilder)
            : base(redirect, viaBuilder)
        {
            _viaBuilder = viaBuilder;
        }
        
        public new IRedirectToFuncBuilder<TTarget, TReturn> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(callHandler, optionsAction);

            return this;
        }

        public IRedirectToFuncBuilder<TTarget, TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return Via(() => instance, optionsAction);
        }

        public IRedirectToFuncBuilder<TTarget, TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectToFuncBuilder<TTarget, TReturn> Via(Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via<TArgs>(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Via(viaDelegate, optionsAction);
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var builder = _viaBuilder.Args<TArgs>();
            
            return new RedirectToFuncBuilder<TTarget, TReturn, TArgs>(Redirect, builder);
        }
        
        public new IFuncCallStream<TTarget, TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var recordStream = base.Record(optionsAction);
            
            var calls = recordStream.Select(call => new FuncRecordedCall<TTarget, TReturn>(call));
            var callStream = new FuncCallStream<TTarget, TReturn>(calls, _viaBuilder.CallValidator);

            return callStream;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> Record<TArgs>(Action<IViaOptionsBuilder>? optionsAction = null) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Record(optionsAction);
        }

        public IRedirectToFuncBuilder<TTarget, TReturn> Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new ViaDecoratorCallHandler<TReturn>(decorator);

            return Via(callHandler, optionsAction);
        }
    }

    internal class RedirectToFuncBuilder<TTarget, TReturn, TArgs> : RedirectToFuncBuilder<TTarget, TReturn>, IRedirectToFuncBuilder<TTarget, TReturn, TArgs>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly FuncViaBuilder<TTarget, TReturn, TArgs> _viaBuilder;
        
        public RedirectToFuncBuilder(Redirect<TTarget> redirect, FuncViaBuilder<TTarget, TReturn, TArgs> viaBuilder)
            : base(redirect, viaBuilder)
        {
            _viaBuilder = viaBuilder;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Filter(ICallConstraint callConstraint)
        {
            base.Filter(callConstraint);

            return this;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(callHandler, optionsAction);

            return this;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(instance, optionsAction);

            return this;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(viaDelegate, optionsAction);

            return this;
        }

        public IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Via(Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            var via = _viaBuilder.Build(viaDelegate);
            InsertVia(via, optionsAction);

            return this;
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }

        public new IFuncCallStream<TTarget, TReturn, TArgs> Record(Action<IViaOptionsBuilder>? optionsAction = null)
        {
            return base.Record(optionsAction).Args<TArgs>();
        }

        public new IRedirectToFuncBuilder<TTarget, TReturn, TArgs> Decorate(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Decorate(decorator, optionsAction);

            return this;
        }
    }
}