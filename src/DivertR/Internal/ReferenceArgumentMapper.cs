using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReferenceArgumentMapper<T> : IReferenceArgumentMapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToRef(object arg)
        {
            return new Ref<T>((T) arg);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object FromRef(object mappedArg)
        {
            var argRef = (Ref<T>) mappedArg;

            return argRef.Value!;
        }
    }
}