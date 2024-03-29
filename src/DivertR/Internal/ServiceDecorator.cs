﻿using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ServiceDecorator : IServiceDecorator
    {
        private readonly IServiceDecorator _decorator;

        private ServiceDecorator(Type serviceType, Func<object?, IDiverter, IServiceProvider, object?> decorator)
        {
            ServiceType = serviceType;

            _decorator = serviceType.IsValueType
                ? new ServiceStructDecorator(serviceType, decorator!)
                : new ServiceClassDecorator(serviceType, decorator);
        }
        
        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Decorate(object? input, IDiverter diverter, IServiceProvider provider)
        {
            return _decorator.Decorate(input, diverter, provider);
        }

        public static IServiceDecorator Create<TService>(Func<TService, TService> decorator)
        {
            return new ServiceDecorator(typeof(TService), (input, _, _) => decorator.Invoke((TService) input!));
        }
        
        public static IServiceDecorator Create<TService>(Func<TService, IDiverter, TService> decorator)
        {
            return new ServiceDecorator(typeof(TService), (input, diverter, _) => decorator.Invoke((TService) input!, diverter));
        }
        
        public static IServiceDecorator Create<TService>(Func<TService, IDiverter, IServiceProvider, TService> decorator)
        {
            return new ServiceDecorator(typeof(TService), (input, diverter, provider) => decorator.Invoke((TService) input!, diverter, provider)!);
        }
        
        public static IServiceDecorator Create(Type serviceType, Func<object?, object?> decorator)
        {
            return new ServiceDecorator(serviceType, (input, _, _) => decorator.Invoke(input));
        }
        
        public static IServiceDecorator Create(Type serviceType, Func<object?, IDiverter, object?> decorator)
        {
            return new ServiceDecorator(serviceType, (input, diverter, _) => decorator.Invoke(input, diverter));
        }
        
        public static IServiceDecorator Create(Type serviceType, Func<object?, IDiverter, IServiceProvider, object?> decorator)
        {
            return new ServiceDecorator(serviceType, decorator);
        }
    }

    internal class ServiceClassDecorator : IServiceDecorator
    {
        private readonly ConditionalWeakTable<object, object?> _decoratedCache = new();
        private readonly Func<object?, IDiverter, IServiceProvider, object?> _decorator;
        
        public ServiceClassDecorator(Type serviceType, Func<object?, IDiverter, IServiceProvider, object?> decorator)
        {
            ServiceType = serviceType;
            _decorator = decorator;
        }

        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Decorate(object? input, IDiverter diverter, IServiceProvider provider)
        {
            if (input is null)
            {
                return _decorator(input, diverter, provider);
            }
            
            return _decoratedCache.GetValue(input, inner => _decorator(inner, diverter, provider));
        }
    }

    internal class ServiceStructDecorator : IServiceDecorator
    {
        private readonly Func<object, IDiverter, IServiceProvider, object> _decorator;
        
        public ServiceStructDecorator(Type serviceType, Func<object, IDiverter, IServiceProvider, object> decorator)
        {
            ServiceType = serviceType;
            _decorator = decorator;
        }

        public Type ServiceType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Decorate(object? input, IDiverter diverter, IServiceProvider provider)
        {
            return _decorator.Invoke(input!, diverter, provider);
        }
    }
}