using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ReferenceArgumentMapper<T> : IReferenceArgumentMapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToRef(object? arg)
        {
            return arg != null ? new Ref<T>((T) arg) : new Ref<T>(default!);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object FromRef(object mappedArg)
        {
            var argRef = (Ref<T>) mappedArg;

            return argRef.Value!;
        }
    }
}