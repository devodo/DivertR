using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectBuilderOptions<TTarget> where TTarget : class
    {
        public List<ICallConstraint<TTarget>> CallConstraints { get; } = new List<ICallConstraint<TTarget>>();

        public int OrderWeight { get; set; }
    }
}