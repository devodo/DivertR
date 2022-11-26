using System;

namespace DivertR
{
    public class RedirectOptions : IRedirectOptions
    {
        public static readonly RedirectOptions Default = new();

        public RedirectOptions(
            int orderWeight = 0,
            bool disableSatisfyStrict = false,
            bool isPersistent = false,
            Func<IRedirect, IRedirect>? redirectDecorator = null)
        {
            OrderWeight = orderWeight;
            DisableSatisfyStrict = disableSatisfyStrict;
            IsPersistent = isPersistent;
            RedirectDecorator = redirectDecorator;
        }

        public int OrderWeight { get; }
        public bool DisableSatisfyStrict { get; }
        public bool IsPersistent { get; }
        public Func<IRedirect, IRedirect>? RedirectDecorator { get; }
    }
}