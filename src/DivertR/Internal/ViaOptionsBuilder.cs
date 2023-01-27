using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DivertR.Internal
{
    internal class ViaOptionsBuilder : IViaOptionsBuilder
    {
        private int _orderWeight;
        private bool _disableSatisfyStrict;
        private bool _isPersistent;
        
        private readonly ConcurrentStack<Func<IVia, IVia>> _viaDecorators = new();

        private ViaOptionsBuilder(int orderWeight = 0, bool disableSatisfyStrict = false)
        {
            _orderWeight = orderWeight;
            _disableSatisfyStrict = disableSatisfyStrict;
        }

        public IViaOptionsBuilder OrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }

        public IViaOptionsBuilder OrderFirst()
        {
            return OrderWeight(int.MaxValue);
        }

        public IViaOptionsBuilder OrderLast()
        {
            return OrderWeight(int.MinValue);
        }

        public IViaOptionsBuilder DisableSatisfyStrict(bool disableStrict = true)
        {
            _disableSatisfyStrict = disableStrict;

            return this;
        }
        
        public IViaOptionsBuilder Persist(bool isPersistent = true)
        {
            _isPersistent = isPersistent;

            return this;
        }

        public IViaOptionsBuilder Decorate(Func<IVia, IVia> decorator)
        {
            _viaDecorators.Push(decorator);

            return this;
        }

        public IViaOptionsBuilder Repeat(int repeatCount)
        {
            return Decorate(via => new RepeatViaDecorator(via, repeatCount));
        }
        
        public IViaOptionsBuilder Skip(int skipCount)
        {
            return Decorate(via => new SkipViaDecorator(via, skipCount));
        }
        
        public static ViaOptions Create(Action<IViaOptionsBuilder>? optionsAction, int orderWeight = 0, bool disableSatisfyStrict = false)
        {
            var builder = new ViaOptionsBuilder(orderWeight, disableSatisfyStrict);
            optionsAction?.Invoke(builder);

            return builder.BuildOptions();
        }
        
        private ViaOptions BuildOptions()
        {
            return new ViaOptions(_orderWeight, _disableSatisfyStrict, _isPersistent, BuildViaDecorator());
        }
        
        private Func<IVia, IVia>? BuildViaDecorator()
        {
            if (!_viaDecorators.Any())
            {
                return null;
            }

            return via =>
            {
                foreach (var decorator in _viaDecorators.Reverse())
                {
                    via = decorator.Invoke(via);
                }

                return via;
            };
        }
    }
}