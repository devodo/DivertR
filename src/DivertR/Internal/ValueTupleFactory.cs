﻿using System;
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

            var factoryTypeDefinition = ValueTupleFactoryTypes[valueTupleType.GenericTypeArguments.Length];
            var factoryType = factoryTypeDefinition.MakeGenericType(valueTupleType.GenericTypeArguments);

            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
            var factory = (IValueTupleFactory) Activator.CreateInstance(factoryType, ActivatorFlags, null, Array.Empty<object>(), default);

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

        public Type[] ArgumentTypes => Array.Empty<Type>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return new ValueTuple();
        }
    }
    
    internal class ValueTupleFactory<T1> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1) };

        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return new ValueTuple<T1>((T1) args[0]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2> : IValueTupleFactory
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type[] ArgTypes;
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<CallArguments, object> CreateFunc;

        static ValueTupleFactory()
        {
            if (typeof(T2) == typeof(__))
            {
                ArgTypes = new[] { typeof(T1) };
                CreateFunc = CreatePartial;
            }
            else
            {
                ArgTypes = new[] { typeof(T1), typeof(T2) };
                CreateFunc = CreateFull;
            }
        }
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return CreateFunc(args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateFull(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1]);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreatePartial(CallArguments args)
        {
            return ((T1) args[0], __.Instance);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6, T7> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6]);
        }
    }
    
    internal class ValueTupleFactory<T1, T2, T3, T4, T5, T6, T7, T8> : IValueTupleFactory
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(CallArguments args)
        {
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6], (T8) args[7]);
        }
    }
}