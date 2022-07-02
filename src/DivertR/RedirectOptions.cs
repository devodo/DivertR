namespace DivertR
{
    public class RedirectOptions : IRedirectOptions
    {
        public static readonly RedirectOptions Default = new RedirectOptions();

        public RedirectOptions(
            int? orderWeight = null,
            bool? disableSatisfyStrict = null)
        {
            OrderWeight = orderWeight;
            DisableSatisfyStrict = disableSatisfyStrict;
        }

        public int? OrderWeight { get; }
        public bool? DisableSatisfyStrict { get; }
    }
}