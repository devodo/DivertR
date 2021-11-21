namespace DivertR.Internal
{
    internal class ReferenceArgumentFactory<T> : IReferenceArgumentFactory
    {
        public object Create(object arg)
        {
            return new Ref<T>((T) arg);
        }

        public object GetRefValue(object mappedArg)
        {
            var argRef = (Ref<T>) mappedArg;

            return argRef.Value!;
        }
    }
}