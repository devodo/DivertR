using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Dummy.Internal
{
    internal class DummyValueFactory
    {
        private readonly Dictionary<Type, Func<Type, DummyValueFactory, object?>> _valueFactories;

        public DummyValueFactory()
        {
            var defaultTaskFactory = new TaskValueFactory();

            _valueFactories = new Dictionary<Type, Func<Type, DummyValueFactory, object?>>
            {
                [typeof(Task)] = (type, factory) => Task.CompletedTask,
                [typeof(Task<>)] = (type, factory) => defaultTaskFactory.CreateTaskOf(type, factory),
                [typeof(ValueTask<>)] = (type, factory) => defaultTaskFactory.CreateValueTaskOf(type, factory),
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

        public object? Create(Type type)
        {
            if (_valueFactories.TryGetValue(type, out var factory))
            {
                return factory.Invoke(type, this); 
            }

            Type factoryType = type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type.IsArray
                    ? typeof(Array)
                    : type;

            if (_valueFactories.TryGetValue(factoryType, out factory))
            {
                return factory.Invoke(type, this);
            }
            
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        private static object CreateArray(Type type, DummyValueFactory dummyValueFactory)
        {
            var elementType = type.GetElementType();
            var lengths = new int[type.GetArrayRank()];
            return Array.CreateInstance(elementType!, lengths);
        }

        private static object CreateEnumerable(Type type, DummyValueFactory dummyValueFactory)
        {
            return Array.Empty<object>();
        }

        private static object CreateEnumerableOf(Type type, DummyValueFactory dummyValueFactory)
        {
            var elementType = type.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);
        }
        
        private static object CreateTupleOf(Type type, DummyValueFactory dummyValueFactory)
        {
            var itemTypes = type.GetGenericArguments();
            var items = new object?[itemTypes.Length];
            for (int i = 0, n = itemTypes.Length; i < n; ++i)
            {
                items[i] = dummyValueFactory.Create(itemTypes[i]);
            }
            
            return Activator.CreateInstance(type, items);
        }
    }
}