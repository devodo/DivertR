using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DivertR.Default.Internal
{
    internal class ListValueFactory
    {
        private readonly ConcurrentDictionary<Type, Type> _iListTypes = new ConcurrentDictionary<Type, Type>();

        public object? CreateListOf(Type iListType, IDefaultValueFactory _)
        {
            var type = _iListTypes.GetOrAdd(iListType, type =>
            {
                var elementType = type.GetGenericArguments()[0];
                
                return typeof(List<>).MakeGenericType(elementType);
            });

            return Activator.CreateInstance(type);
        }
    }
}