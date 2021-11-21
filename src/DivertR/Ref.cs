using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Ref<T>
    {
        private T _value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ref(T value)
        {
            _value = value;
        }

        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _value;
        }
    }
}