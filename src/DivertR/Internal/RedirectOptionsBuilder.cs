using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder : IRedirectOptionsBuilder
    {
        private int _orderWeight;
        private bool _disableSatisfyStrict;
        private bool _isPersistent;
        
        private readonly ConcurrentStack<Func<IRedirect, IRedirect>> _redirectDecorators = new();

        private RedirectOptionsBuilder(int orderWeight = 0, bool disableSatisfyStrict = false)
        {
            _orderWeight = orderWeight;
            _disableSatisfyStrict = disableSatisfyStrict;
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
        
        public IRedirectOptionsBuilder Persist(bool isPersistent = true)
        {
            _isPersistent = isPersistent;

            return this;
        }

        public IRedirectOptionsBuilder Decorate(Func<IRedirect, IRedirect> decorator)
        {
            _redirectDecorators.Push(decorator);

            return this;
        }

        public IRedirectOptionsBuilder Repeat(int repeatCount)
        {
            return Decorate(redirect => new RepeatRedirectDecorator(redirect, repeatCount));
        }
        
        public IRedirectOptionsBuilder Skip(int skipCount)
        {
            return Decorate(redirect => new SkipRedirectDecorator(redirect, skipCount));
        }
        
        public static IRedirectOptions Create(Action<IRedirectOptionsBuilder>? optionsAction, int orderWeight = 0, bool disableSatisfyStrict = false)
        {
            var builder = new RedirectOptionsBuilder(orderWeight, disableSatisfyStrict);
            optionsAction?.Invoke(builder);

            return builder.BuildOptions();
        }
        
        private IRedirectOptions BuildOptions()
        {
            return new RedirectOptions(_orderWeight, _disableSatisfyStrict, _isPersistent, BuildRedirectDecorator());
        }
        
        private Func<IRedirect, IRedirect>? BuildRedirectDecorator()
        {
            if (!_redirectDecorators.Any())
            {
                return null;
            }

            return redirect =>
            {
                foreach (var decorator in _redirectDecorators.Reverse())
                {
                    redirect = decorator.Invoke(redirect);
                }

                return redirect;
            };
        }
    }
}