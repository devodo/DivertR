namespace DivertR
{
    public class Ref<T>
    {
        private T _value;

        public Ref(T value)
        {
            _value = value;
        }

        public ref T Value()
        {
            return ref _value;
        }
    }
}