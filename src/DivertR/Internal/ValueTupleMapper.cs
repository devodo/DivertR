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
            return new ValueTuple<T1>((T1) args[0]);
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
    
    internal class ValueTupleMapperDiscard<T1> : IValueTupleMapper
    {
        private static readonly Type[] ArgTypes = { typeof(T1) };
        public Type[] ArgumentTypes => ArgTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            return ((T1) args[0], __.Instance);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            var valueTuple = (ValueTuple<T1, __>) boxedTuple;

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
            return ((T1) args[0], (T2) args[1]);
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
            return ((T1) args[0], (T2) args[1], (T3) args[2]);
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
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3]);
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
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4]);
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
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5]);
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
            return ((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6]);
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
            var rest = (TRest) NestedMapper.ToTuple(args.Slice(7));
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                (T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3], (T5) args[4], (T6) args[5], (T7) args[6], rest);
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
            NestedMapper.WriteBackReferences(args.Slice(7), valueTuple.Rest);
        }
    }
}