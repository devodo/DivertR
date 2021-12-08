using System;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly List<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerDecorators =
            new List<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>>();
        
        private readonly List<Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>> _callConstraintDecorators =
            new List<Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>>();

        public RedirectOptionsBuilder(IVia<TTarget> via)
        {
            _via = via;
        }

        public IRedirectOptionsBuilder<TTarget> OrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> OrderFirst()
        {
            return OrderWeight(int.MaxValue);
        }

        public IRedirectOptionsBuilder<TTarget> OrderLast()
        {
            return OrderWeight(int.MinValue);
        }

        public IRedirectOptionsBuilder<TTarget> DisableSatisfyStrict(bool disableStrict = true)
        {
            _disableSatisfyStrict = disableStrict;

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> DecorateCallHandler(Func<ICallHandler<TTarget>, ICallHandler<TTarget>> decorator)
        {
            _callHandlerDecorators.Add(decorator);

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> DecorateCallConstraint(Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>> decorator)
        {
            _callConstraintDecorators.Add(decorator);

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount)
        {
            return DecorateCallHandler(callHandler => new RepeatCallHandler<TTarget>(_via, callHandler, repeatCount));
        }
        
        public IRedirectOptionsBuilder<TTarget> Skip(int skipCount)
        {
            return DecorateCallHandler(callHandler => new SkipCallHandler<TTarget>(_via, callHandler, skipCount));
        }

        public IRedirectOptionsBuilder<TTarget> AddSwitch(IRedirectSwitch redirectSwitch)
        {
            ICallConstraint<TTarget> switchConstraint = new SwitchCallConstraint<TTarget>(redirectSwitch);

            ICallConstraint<TTarget> Decorator(ICallConstraint<TTarget> callConstraint)
            {
                return CompositeCallConstraint<TTarget>.Empty.AddCallConstraints(new[] { switchConstraint, callConstraint });
            }

            return DecorateCallConstraint(Decorator);
        }

        public RedirectOptions<TTarget> Build()
        {
            return new RedirectOptions<TTarget>
            {
                OrderWeight = _orderWeight,
                DisableSatisfyStrict = _disableSatisfyStrict,
                CallHandlerDecorator = BuildCallHandlerDecorator,
                CallConstraintDecorator = BuildCallConstraintDecorator
            };
        }
        
        private ICallHandler<TTarget> BuildCallHandlerDecorator(ICallHandler<TTarget> callHandler)
        {
            if (!_callHandlerDecorators.Any())
            {
                return callHandler;
            }
            
            foreach (var decorator in _callHandlerDecorators)
            {
                callHandler = decorator.Invoke(callHandler);
            }

            return callHandler;
        }
        
        private ICallConstraint<TTarget> BuildCallConstraintDecorator(ICallConstraint<TTarget> callConstraint)
        {
            if (!_callConstraintDecorators.Any())
            {
                return callConstraint;
            }
            
            foreach (var decorator in _callConstraintDecorators)
            {
                callConstraint = decorator.Invoke(callConstraint);
            }

            return callConstraint;
        }
    }
}