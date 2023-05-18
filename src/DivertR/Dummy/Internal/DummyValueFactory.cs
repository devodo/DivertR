using System;
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
                [typeof(void)] = (_, _) => null,
                [typeof(Task)] = (_, _) => Task.CompletedTask,
                [typeof(Task<>)] = (type, factory) => defaultTaskFactory.CreateTaskOf(type, factory),
                [typeof(ValueTask<>)] = (type, factory) => defaultTaskFactory.CreateValueTaskOf(type, factory),
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
                [typeof(Tuple<,,,,,,,>)] = CreateTupleOf
            };
        }

        public object? Create(Type type)
        {
            var factoryType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            
            if (_valueFactories.TryGetValue(factoryType, out var factory))
            {
                return factory.Invoke(type, this);
            }
            
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
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