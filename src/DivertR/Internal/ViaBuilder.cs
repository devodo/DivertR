﻿using System;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR.Internal
{
    internal class ViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        protected readonly IRedirectRepository RedirectRepository;
        private readonly IRedirectBuilder<TTarget> _redirectBuilder;

        public ViaBuilder(IRedirectRepository redirectRepository, IRedirectBuilder<TTarget> redirectBuilder)
        {
            RedirectRepository = redirectRepository ?? throw new ArgumentNullException(nameof(redirectRepository));
            _redirectBuilder = redirectBuilder;
        }

        public IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _redirectBuilder.AddConstraint(callConstraint);

            return this;
        }

        public IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return _redirectBuilder.Build(callHandler, optionsAction);
        }

        public IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(target, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        public IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            var redirect = _redirectBuilder.Build(recordHandler, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return recordHandler.RecordStream;
        }
    }
}
