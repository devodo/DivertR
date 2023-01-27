using System;

namespace DivertR
{
    public class ViaOptions
    {
        public static readonly ViaOptions Default = new();

        public ViaOptions(
            int orderWeight = 0,
            bool disableSatisfyStrict = false,
            bool isPersistent = false,
            Func<IVia, IVia>? viaDecorator = null)
        {
            OrderWeight = orderWeight;
            DisableSatisfyStrict = disableSatisfyStrict;
            IsPersistent = isPersistent;
            ViaDecorator = viaDecorator;
        }

        public int OrderWeight { get; }
        public bool DisableSatisfyStrict { get; }
        public bool IsPersistent { get; }
        public Func<IVia, IVia>? ViaDecorator { get; }
    }
}