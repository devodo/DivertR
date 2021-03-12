﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class Diverter : IDiverter
    {
        private readonly ViaStateRepository _viaStateRepository = new ViaStateRepository();
        private readonly ConcurrentDictionary<ViaId, IVia> _vias = new ConcurrentDictionary<ViaId, IVia>();

        public IVia<T> Via<T>(string? name = null) where T : class
        {
            return (IVia<T>) _vias.GetOrAdd(ViaId.From<T>(name),
                id => new Via<T>(id, _viaStateRepository));
        }
        
        public IVia Via(Type type, string? name = null)
        {
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return _vias.GetOrAdd(ViaId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Via<>).MakeGenericType(type);
                    return (IVia) Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _viaStateRepository}, default);
                });
        }
        
        public IDiverter ResetAll()
        {
            _viaStateRepository.ResetAll();
            return this;
        }
    }
}