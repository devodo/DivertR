using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ValueTupleMapperFactory
    {
        private static readonly Type[] ValueTupleTypes =
        {
            typeof(ValueTuple), typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
        };
        
        private static readonly Type[] ValueTupleFactoryTypes =
        {
            typeof(ValueTupleMapper), typeof(ValueTupleMapper<>), typeof(ValueTupleMapper<,>), typeof(ValueTupleMapper<,,>), typeof(ValueTupleMapper<,,,>), typeof(ValueTupleMapper<,,,,>), typeof(ValueTupleMapper<,,,,,>), typeof(ValueTupleMapper<,,,,,,>), typeof(ValueTupleMapper<,,,,,,,>)
        };

        public static IValueTupleMapper Create<TArgs>()
        {
            var valueTupleType = typeof(TArgs);

            if (!IsValueTuple(valueTupleType))
            {
                throw new DiverterException($"Type {valueTupleType.Name} is not a ValueTuple type");
            }

            Type factoryType;
            if (valueTupleType.GenericTypeArguments.Length == 2 && valueTupleType.GenericTypeArguments[1] == typeof(__))
            {
                var factoryTypeDefinition = typeof(ValueTupleMapperDiscard<>);
                factoryType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments[0]);
            }
            else
            {
                var factoryTypeDefinition = ValueTupleFactoryTypes[valueTupleType.GenericTypeArguments.Length];
                factoryType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments);
            }
            
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
            var factory = (IValueTupleMapper) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);
            
            var refMappers = GetRefMappers(valueTupleType.GenericTypeArguments);

            if (refMappers.Length > 0)
            {
                factory = new RefValueTupleMapper(factory, refMappers);
            }

            return factory;
        }

        private static bool IsValueTuple(Type type)
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