using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ValueTupleMapper : IValueTupleMapper
    {
        public Type[] ArgumentTypes => Array.Empty<Type>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return new ValueTuple();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            return Array.Empty<object?>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return new ValueTuple<T1>(args.Map<T1>(0));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var valueTuple = (ValueTuple<T1>) boxedTuple;

            return new object?[] { valueTuple.Item1 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }

    internal class ValueTupleMapper<T1, T2> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2) = (ValueTuple<T1, T2>) boxedTuple;

            return new object?[] { item1, item2 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3) = (ValueTuple<T1, T2, T3>) boxedTuple;

            return new object?[] { item1, item2, item3 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3, T4> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2), args.Map<T4>(3));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3, item4) = (ValueTuple<T1, T2, T3, T4>) boxedTuple;

            return new object?[] { item1, item2, item3, item4 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3, T4, T5> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2), args.Map<T4>(3), args.Map<T5>(4));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3, item4, item5) = (ValueTuple<T1, T2, T3, T4, T5>) boxedTuple;

            return new object?[] { item1, item2, item3, item4, item5 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3, T4, T5, T6> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2), args.Map<T4>(3), args.Map<T5>(4), args.Map<T6>(5));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3, item4, item5, item6) = (ValueTuple<T1, T2, T3, T4, T5, T6>) boxedTuple;

            return new object?[] { item1, item2, item3, item4, item5, item6 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3, T4, T5, T6, T7> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return (args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2), args.Map<T4>(3), args.Map<T5>(4), args.Map<T6>(5), args.Map<T7>(6));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3, item4, item5, item6, item7) = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>) boxedTuple;

            return new object?[] { item1, item2, item3, item4, item5, item6, item7 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
        }
    }
    
    internal class ValueTupleMapper<T1, T2, T3, T4, T5, T6, T7, TRest> : IValueTupleMapper where TRest : struct
    {
        // ReSharper disable once StaticMemberInGenericType (Static instance per generic type is intended here)
        private static readonly Type[] ArgTypes;
        
        // ReSharper disable once StaticMemberInGenericType (Static instance per generic type is intended here)
        private static readonly IValueTupleMapper NestedMapper;
        
        static ValueTupleMapper()
        {
            NestedMapper = ValueTupleMapperFactory.Create<TRest>();
            ArgTypes = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }
                .Concat(NestedMapper.ArgumentTypes)
                .ToArray();
        }
        
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            var argsRest = args.Length > 7 ? args.Slice(7) : Span<object>.Empty;
            var rest = (TRest) NestedMapper.ToTuple(argsRest);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                args.Map<T1>(0), args.Map<T2>(1), args.Map<T3>(2), args.Map<T4>(3), args.Map<T5>(4), args.Map<T6>(5), args.Map<T7>(6), rest);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var (item1, item2, item3, item4, item5, item6, item7) = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>) boxedTuple;

            return new object?[] { item1, item2, item3, item4, item5, item6, item7 };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
            var valueTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>) boxedTuple;
            var argsRest = args.Length > 7 ? args.Slice(7) : Span<object>.Empty;
            NestedMapper.WriteBackReferences(argsRest, valueTuple.Rest);
        }
    }
    
    internal static class MapTypeOrDiscardExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Map<T>(this Span<object> args, int index)
        {
            if (typeof(T) == typeof(__))
            {
                return (T) (object) __.Instance;
            }

            return (T) args[index];
        }
    }
}