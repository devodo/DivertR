﻿using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Divertr
{
    public class DiverterSet : IDiverterSet
    {
        private readonly DiversionState _diversionState = new DiversionState();
        private readonly ConcurrentDictionary<DiverterId, object> _diverters = new ConcurrentDictionary<DiverterId, object>();

        public IDiverter<T> Get<T>(string name = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            return (IDiverter<T>) _diverters.GetOrAdd(DiverterId.From<T>(name),
                id => new Diverter<T>(id, _diversionState));
        }
        
        public IDiverter Get(Type type, string name = null)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", nameof(type));
            }
            
            const BindingFlags activatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            return (IDiverter) _diverters.GetOrAdd(DiverterId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diverter<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, activatorFlags, null, new object[] {id, _diversionState}, default(CultureInfo));
                });
        }
        
        public void ResetAll()
        {
            _diversionState.Reset();
        }
    }
}