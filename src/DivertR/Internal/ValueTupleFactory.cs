using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ValueTupleFactory : IValueTupleFactory
    {
        private static readonly Type[] ValueTupleTypes =
        {
            typeof(ValueTuple), typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
        };
        
        private static readonly Type[] ValueTupleFactoryTypes =
        {
            typeof(ValueTupleFactory), typeof(ValueTupleFactory<>), typeof(ValueTupleFactory<,>), typeof(ValueTupleFactory<,,>), typeof(ValueTupleFactory<,,,>), typeof(ValueTupleFactory<,,,,>), typeof(ValueTupleFactory<,,,,,>), typeof(ValueTupleFactory<,,,,,,>), typeof(ValueTupleFactory<,,,,,,,>)
        };

        public static IValueTupleFactory CreateFactory<TArgs>()
        {
            var valueTupleType = typeof(TArgs);

            if (!IsValueTuple(valueTupleType))
            {
                throw new DiverterException($"Type {valueTupleType.Name} is not a ValueTuple type");
            }

            Type factoryType;
            if (valueTupleType.GenericTypeArguments.Length == 2 && valueTupleType.GenericTypeArguments[1] == typeof(__))
            {
                var factoryTypeDefinition = typeof(ValueTupleFactoryDiscard<>);
                factoryType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments[0]);
            }
            else
            {
                var factoryTypeDefinition = ValueTupleFactoryTypes[valueTupleType.GenericTypeArguments.Length];
                factoryType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments);
            }
            
            var refMapper = GetRefMapper(valueTupleType.GenericTypeArguments);

            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
            var factory = (IValueTupleFactory) Activator.CreateInstance(factoryType, ActivatorFlags, null, new object[] { refMapper! }, default);
            
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

        private static ReferenceArgumentMapper? GetRefMapper(Type[] arguments)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            var factories = arguments
                .Select((arg, i) =>
                {
                    if (!arg.IsGenericType ||
                        arg.GenericTypeArguments.Length != 1 ||
                        arg.GetGenericTypeDefinition() != typeof(Ref<>))
                    {
                        return ((IReferenceArgumentFactory) null!, 0);
                    }
                    
                    var typeDefinition = typeof(ReferenceArgumentFactory<>);
                    var factoryType = typeDefinition.MakeGenericType(arg.GenericTypeArguments[0]);
                    var factory = (IReferenceArgumentFactory) Activator.CreateInstance(factoryType, ActivatorFlags, null, null, default);

                    return (factory, i);
                })
                .Where(x => x.Item1 != null)
                .ToArray();

            return factories.Length > 0 ? new ReferenceArgumentMapper(factories) : null;
        }

        public Type[] ArgumentTypes => Array.Empty<Type>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return new ValueTuple();
        }

        public ReferenceArgumentMapper? GetRefMapper() => null;
    }
    
    internal class ValueTupleFactory<T1> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1) };
        private readonly ReferenceArgumentMapper? _refMapper;
        
        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return new ValueTuple<T1>((T1) args[0]);
        }

        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactoryDiscard<T1> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactoryDiscard(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], __.Instance);
        }

        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6, T7> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6]);
        }
        
        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6, T7, TRest> : IValueTupleFactory where TRest : struct
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type[] ArgTypes;
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IValueTupleFactory NestedFactory;
        
        static ValueTupleFactory()
        {
            NestedFactory = ValueTupleFactory.CreateFactory<TRest>();
            ArgTypes = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }
                .Concat(NestedFactory.ArgumentTypes)
                .ToArray();
        }
        
        private readonly ReferenceArgumentMapper? _refMapper;

        public ValueTupleFactory(ReferenceArgumentMapper? refMapper)
        {
            _refMapper = refMapper;
        }

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Span<object> args)
        {
            var rest = (TRest) NestedFactory.Create(args.Slice(7));
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                (T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6], rest);
        }

        public ReferenceArgumentMapper? GetRefMapper() => _refMapper;
    }
}