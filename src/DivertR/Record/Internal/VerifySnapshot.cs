using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DivertR.Record.Internal
{
    internal class VerifySnapshot<T> : ReadOnlyCollection<T>, IVerifySnapshot<T>
    {
        public VerifySnapshot(IList<T> items) : base(items)
        {
        }
    }
}