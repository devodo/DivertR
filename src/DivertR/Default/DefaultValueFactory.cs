using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DivertR.Default.Internal;

namespace DivertR.Default
{
    public class DefaultValueFactory : IDefaultValueFactory
    {
        private readonly Dictionary<Type, Func<Type, object?>> _valueFactories;

        public DefaultValueFactory()
        {
            var defaultTaskFactory = new DefaultAwaitableFactory(this);

            _valueFactories = new Dictionary<Type, Func<Type, object?>>
            {
                [typeof(Task)] = type => Task.CompletedTask,
                [typeof(Task<>)] = type => defaultTaskFactory.CreateTaskOf(type),
                [typeof(ValueTask<>)] = type => defaultTaskFactory.CreateValueTaskOf(type),
                [typeof(IEnumerable)] = CreateEnumerable,
                [typeof(IEnumerable<>)] = CreateEnumerableOf,
                [typeof(Array)] = CreateArray,
                [typeof(ValueTuple<>)] = CreateTupleOf,
                [typeof(ValueTuple<,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,,,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,,,,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,,,,,>)] = CreateTupleOf,
                [typeof(ValueTuple<,,,,,,,>)] = CreateTupleOf,
                [typeof(Tuple<>)] = CreateTupleOf,
                [typeof(Tuple<,>)] = CreateTupleOf,
                [typeof(Tuple<,,>)] = CreateTupleOf,
                [typeof(Tuple<,,,>)] = CreateTupleOf,
                [typeof(Tuple<,,,,>)] = CreateTupleOf,
                [typeof(Tuple<,,,,,>)] = CreateTupleOf,
                [typeof(Tuple<,,,,,,>)] = CreateTupleOf,
                [typeof(Tuple<,,,,,,,>)] = CreateTupleOf,
            };
        }

        public object? GetDefaultValue(Type type)
        {
            Type factoryType = type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type.IsArray
                    ? typeof(Array)
                    : type;

            if (_valueFactories.TryGetValue(factoryType, out var factory))
            {
                return factory.Invoke(type);
            }
            
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }
        
        private static object CreateArray(Type type)
        {
            var elementType = type.GetElementType();
            var lengths = new int[type.GetArrayRank()];
            return Array.CreateInstance(elementType!, lengths);
        }
        
        private static object CreateEnumerable(Type type)
        {
            return Array.Empty<object>();
        }

        private static object CreateEnumerableOf(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);
        }
        
        private object CreateTupleOf(Type type)
        {
            var itemTypes = type.GetGenericArguments();
            var items = new object?[itemTypes.Length];
            for (int i = 0, n = itemTypes.Length; i < n; ++i)
            {
                items[i] = this.GetDefaultValue(itemTypes[i]);
            }
            
            return Activator.CreateInstance(type, items);
        }
    }
}