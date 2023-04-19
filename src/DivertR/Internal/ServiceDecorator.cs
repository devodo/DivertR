using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ServiceDecorator : IDiverterDecorator
    {
        private readonly IDiverterDecorator _decorator;

        public ServiceDecorator(Type serviceType, Func<object, object> decorator)
        {
            ServiceType = serviceType;

            _decorator = serviceType.IsValueType
                ? new ServiceStructDecorator(serviceType, decorator)
                : new ServiceClassDecorator(serviceType, decorator);
        }
        
        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Decorate(object input)
        {
            return _decorator.Decorate(input);
        }

        public static IDiverterDecorator Create<TService>(Func<TService, TService> decorator)
        {
            return new ServiceDecorator(typeof(TService), x => decorator.Invoke((TService) x)!);
        }
    }

    internal class ServiceClassDecorator : IDiverterDecorator
    {
        private readonly ConditionalWeakTable<object, object> _decoratedCache = new();
        private readonly Func<object, object> _decorator;
        
        public ServiceClassDecorator(Type serviceType, Func<object, object> decorator)
        {
            ServiceType = serviceType;
            _decorator = decorator;
        }

        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Decorate(object input)
        {
            return _decoratedCache.GetValue(input, x => _decorator.Invoke(x));
        }
    }

    internal class ServiceStructDecorator : IDiverterDecorator
    {
        private readonly Func<object, object> _decorator;
        
        public ServiceStructDecorator(Type serviceType, Func<object, object> decorator)
        {
            ServiceType = serviceType;
            _decorator = decorator;
        }

        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Decorate(object input)
        {
            return _decorator.Invoke(input);
        }
    }
}