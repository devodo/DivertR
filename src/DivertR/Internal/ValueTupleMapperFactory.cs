using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace DivertR.Internal
{
    internal static class ValueTupleMapperFactory
    {
        private static readonly Type[] ValueTupleTypes =
        {
            typeof(ValueTuple), typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
        };
        
        private static readonly Type[] ValueTupleMapperTypes =
        {
            typeof(ValueTupleMapper), typeof(ValueTupleMapper<>), typeof(ValueTupleMapper<,>), typeof(ValueTupleMapper<,,>), typeof(ValueTupleMapper<,,,>), typeof(ValueTupleMapper<,,,,>), typeof(ValueTupleMapper<,,,,,>), typeof(ValueTupleMapper<,,,,,,>), typeof(ValueTupleMapper<,,,,,,,>)
        };
        
        private static readonly ConcurrentDictionary<Type, IValueTupleMapper> MapperCache = new ConcurrentDictionary<Type, IValueTupleMapper>();

        public static IValueTupleMapper Create<TArgs>()
        {
            return MapperCache.GetOrAdd(typeof(TArgs), valueTupleType =>
            {
                if (valueTupleType == typeof(ValueTuple))
                {
                    return new ValueTupleMapper();
                }
                
                if (!IsGenericValueTuple(valueTupleType))
                {
                    throw new DiverterException($"Type {valueTupleType.Name} is not a ValueTuple type");
                }
                
                var factoryTypeDefinition = ValueTupleMapperTypes[valueTupleType.GenericTypeArguments.Length];
                var mapperType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments);
            
                const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
                var mapper = (IValueTupleMapper) Activator.CreateInstance(mapperType, ActivatorFlags, null, null, default);
            
                var refMappers = GetRefMappers(valueTupleType.GenericTypeArguments);

                if (refMappers.Length > 0)
                {
                    mapper = new RefValueTupleMapper(mapper, refMappers);
                }

                return mapper;
            });
        }

        private static bool IsGenericValueTuple(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == ValueTupleTypes[type.GenericTypeArguments.Length];
        }

        private static (IReferenceArgumentMapper, int)[] GetRefMappers(Type[] arguments)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            var factories = arguments
                .Select((arg, i) =>
                {
                    if (!arg.IsGenericType ||
                        arg.GenericTypeArguments.Length != 1 ||
                        arg.GetGenericTypeDefinition() != typeof(Ref<>))
                    {
                        return ((IReferenceArgumentMapper) null!, 0);
                    }
                    
                    var typeDefinition = typeof(ReferenceArgumentMapper<>);
                    var factoryType = typeDefinition.MakeGenericType(arg.GenericTypeArguments[0]);
                    var factory = (IReferenceArgumentMapper) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);

                    return (factory, i);
                })
                .Where(x => x.Item1 != null)
                .ToArray();

            return factories;
        }
    }
}