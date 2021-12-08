﻿using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DivertR
{
    public class ViaSet : IViaSet
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, IVia>> _viaGroups =
            new ConcurrentDictionary<string, ConcurrentDictionary<Type, IVia>>();
        
        public ViaSet(DiverterSettings? settings = null)
        {
            Settings = settings ?? DiverterSettings.Global;
        }

        public DiverterSettings Settings { get; }

        public IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class
        {
            var viaId = ViaId.From<TTarget>(name);
            var viaGroup = GetViaGroup(viaId.Name);
            var via = viaGroup.GetOrAdd(viaId.Type, _ => new Via<TTarget>(viaId, this));

            return (IVia<TTarget>) via;
        }
        
        public IVia Via(Type targetType, string? name = null)
        {
            var viaId = ViaId.From(targetType, name);
            
            IVia CreateVia(Type type)
            {
                const BindingFlags ActivatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                
                var diverterType = typeof(Via<>).MakeGenericType(type);
                var constructorParams = new object[] { viaId, this };
                var via = (IVia) Activator.CreateInstance(diverterType, ActivatorFlags, null, constructorParams,
                    default);

                return via;
            }
            
            var viaGroup = GetViaGroup(viaId.Name);
            
            return viaGroup.GetOrAdd(viaId.Type, CreateVia);
        }

        public IVia? Reset(ViaId viaId)
        {
            var viaGroup = GetViaGroup(viaId.Name);
            
            return viaGroup.TryGetValue(viaId.Type, out var via) ? via.Reset() : null;
        }

        public IViaSet Reset(string? name = null)
        {
            var viaGroup = GetViaGroup(name);
            foreach (var via in viaGroup.Values)
            {
                via.Reset();
            }

            return this;
        }

        public IViaSet ResetAll()
        {
            foreach (var viaGroup in _viaGroups.Values)
            {
                foreach (var via in viaGroup.Values)
                {
                    via.Reset();
                }
            }

            return this;
        }

        public IViaSet Strict(string? name = null)
        {
            var viaGroup = GetViaGroup(name);
            foreach (var via in viaGroup.Values)
            {
                via.Strict();
            }

            return this;
        }

        public IViaSet StrictAll()
        {
            foreach (var viaGroup in _viaGroups.Values)
            {
                foreach (var via in viaGroup.Values)
                {
                    via.Strict();
                }
            }

            return this;
        }

        internal void AddVia(IVia via)
        {
            var viaGroup = GetViaGroup(via.ViaId.Name);
            if (!viaGroup.TryAdd(via.ViaId.Type, via))
            {
                throw new DiverterException("Via already exists in ViaSet");
            }
        }

        private ConcurrentDictionary<Type, IVia> GetViaGroup(string? name = null)
        {
            return _viaGroups.GetOrAdd(name ?? string.Empty, _ => new ConcurrentDictionary<Type, IVia>());
        }
    }
}