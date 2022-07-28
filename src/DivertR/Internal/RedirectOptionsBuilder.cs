using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder : IRedirectOptionsBuilder
    {
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
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

        public IRedirectOptions BuildOptions()
        {
            return new RedirectOptions(_orderWeight, _disableSatisfyStrict);
        }
    }
        
    internal class RedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly ConcurrentStack<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerDecorators =
            new ConcurrentStack<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>>();
        
        private readonly ConcurrentBag<ICallConstraint<TTarget>> _callConstraints = new ConcurrentBag<ICallConstraint<TTarget>>();

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
            _callHandlerDecorators.Push(decorator);

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> AddCallConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _callConstraints.Add(callConstraint);

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount)
        {
            return DecorateCallHandler(callHandler => new RepeatCallHandler<TTarget>(callHandler, repeatCount));
        }
        
        public IRedirectOptionsBuilder<TTarget> Skip(int skipCount)
        {
            return DecorateCallHandler(callHandler => new SkipCallHandler<TTarget>(callHandler, skipCount));
        }

        public IRedirectOptionsBuilder<TTarget> AddSwitch(IRedirectSwitch redirectSwitch)
        {
            var switchConstraint = new SwitchCallConstraint<TTarget>(redirectSwitch);

            return AddCallConstraint(switchConstraint);
        }

        public IRedirectOptions BuildOptions()
        {
            return new RedirectOptions(_orderWeight, _disableSatisfyStrict);
        }
        
        public ICallHandler<TTarget> BuildCallHandler(ICallHandler<TTarget> callHandler)
        {
            if (!_callHandlerDecorators.Any())
            {
                return callHandler;
            }
            
            foreach (var decorator in _callHandlerDecorators.Reverse())
            {
                callHandler = decorator.Invoke(callHandler);
            }

            return callHandler;
        }

        public ICallConstraint<TTarget> BuildCallConstraint(IEnumerable<ICallConstraint<TTarget>> callConstraints)
        {
            return new CompositeCallConstraint<TTarget>(callConstraints.Concat(_callConstraints));
        }
    }
}