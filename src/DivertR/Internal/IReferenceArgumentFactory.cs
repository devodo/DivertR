namespace DivertR.Internal
{
    internal interface IReferenceArgumentFactory
    {
        object Create(object arg);

        object GetRefValue(object mappedArg);
    }
}