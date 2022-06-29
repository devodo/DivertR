namespace DivertR
{
    public interface IRedirectOptions
    {
        int? OrderWeight { get; }
        bool? DisableSatisfyStrict { get; }
    }
}