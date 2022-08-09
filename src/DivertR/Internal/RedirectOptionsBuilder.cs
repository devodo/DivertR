﻿using System;
using System.Collections.Concurrent;
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
        
    internal class RedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilder<TTarget> where TTarget : class?
    {
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly ConcurrentStack<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerDecorators =
            new ConcurrentStack<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>>();
        
        private readonly ConcurrentStack<Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>> _callConstraintDecorators =
            new ConcurrentStack<Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>>();

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

        public IRedirectOptionsBuilder<TTarget> DecorateCallConstraint(Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>> decorator)
        {
            _callConstraintDecorators.Push(decorator);

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

        public ICallConstraint<TTarget> BuildCallConstraint(ICallConstraint<TTarget> callConstraint)
        {
            if (!_callConstraintDecorators.Any())
            {
                return callConstraint;
            }
            
            foreach (var decorator in _callConstraintDecorators.Reverse())
            {
                callConstraint = decorator.Invoke(callConstraint);
            }

            return callConstraint;
        }
    }
}