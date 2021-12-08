using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RefValueTupleMapper : IValueTupleMapper
    {
        private readonly IValueTupleMapper _innerMapper;
        private readonly (IReferenceArgumentMapper Mapper, int Index)[] _refMappers;

        public RefValueTupleMapper(IValueTupleMapper innerMapper, (IReferenceArgumentMapper Mapper, int Index)[] refMappers)
        {
            _innerMapper = innerMapper;
            _refMappers = refMappers;
        }

        public Type[] ArgumentTypes => _innerMapper.ArgumentTypes;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToTuple(Span<object> args)
        {
            var mappedArgs = MapToReferences(args);

            return _innerMapper.ToTuple(mappedArgs);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object?[] ToObjectArray(object boxedTuple)
        {
            return _innerMapper.ToObjectArray(boxedTuple);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBackReferences(Span<object> args, object boxedTuple)
        {
            var items = _innerMapper.ToObjectArray(boxedTuple);

            foreach (var (mapper, index) in _refMappers)
            {
                args[index] = mapper.FromRef(items[index]!);
            }
            
            _innerMapper.WriteBackReferences(args, boxedTuple);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object[] MapToReferences(Span<object> args)
        {
            var mappedArgs = new object[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                mappedArgs[i] = args[i];
            }

            foreach (var (refFactory, index) in _refMappers)
            {
                mappedArgs[index] = refFactory.ToRef(args[index]);
            }

            return mappedArgs;
        }
    }
}