namespace DivertR.Internal
{
    internal interface IReferenceArgumentMapper
    {
        object ToRef(object arg);

        object FromRef(object mappedRef);
    }
}