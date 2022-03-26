using System;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder : IRedirectOptionsBuilder
    {
        private readonly IVia _via;
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly List<Func<ICallHandler, ICallHandler>> _callHandlerDecorators =
            new List<Func<ICallHandler, ICallHandler>>();
        
        private readonly List<Func<ICallConstraint, ICallConstraint>> _callConstraintDecorators =
            new List<Func<ICallConstraint, ICallConstraint>>();

        public RedirectOptionsBuilder(IVia via)
        {
            _via = via;
        }

        public IRedirectOptionsBuilder OrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }

        public IRedirectOptionsBuilder OrderFirst()
        {
            return OrderWeight(int.MaxValue);
        }

        public IRedirectOptionsBuilder OrderLast()
        {
            return OrderWeight(int.MinValue);
        }

        public IRedirectOptionsBuilder DisableSatisfyStrict(bool disableStrict = true)
        {
            _disableSatisfyStrict = disableStrict;

            return this;
        }

        public IRedirectOptionsBuilder DecorateCallHandler(Func<ICallHandler, ICallHandler> decorator)
        {
            _callHandlerDecorators.Add(decorator);

            return this;
        }

        public IRedirectOptionsBuilder DecorateCallConstraint(Func<ICallConstraint, ICallConstraint> decorator)
        {
            _callConstraintDecorators.Add(decorator);

            return this;
        }

        public IRedirectOptionsBuilder Repeat(int repeatCount)
        {
            return DecorateCallHandler(callHandler => new RepeatCallHandler(_via, callHandler, repeatCount));
        }
        
        public IRedirectOptionsBuilder Skip(int skipCount)
        {
            return DecorateCallHandler(callHandler => new SkipCallHandler(_via, callHandler, skipCount));
        }

        public IRedirectOptionsBuilder AddSwitch(IRedirectSwitch redirectSwitch)
        {
            ICallConstraint switchConstraint = new SwitchCallConstraint(redirectSwitch);

            ICallConstraint Decorator(ICallConstraint callConstraint)
            {
                return CompositeCallConstraint.Empty.AddCallConstraints(new[] { switchConstraint, callConstraint });
            }

            return DecorateCallConstraint(Decorator);
        }

        public RedirectOptions Build()
        {
            return new RedirectOptions
            {
                OrderWeight = _orderWeight,
                DisableSatisfyStrict = _disableSatisfyStrict,
                CallHandlerDecorator = BuildCallHandlerDecorator,
                CallConstraintDecorator = BuildCallConstraintDecorator
            };
        }
        
        private ICallHandler BuildCallHandlerDecorator(ICallHandler callHandler)
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
        
        private ICallConstraint BuildCallConstraintDecorator(ICallConstraint callConstraint)
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