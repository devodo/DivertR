using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DivertR.Record.Internal
{
    internal class VerifySnapshot<T> : ReadOnlyCollection<T>, IVerifySnapshot<T>
    {
        public VerifySnapshot(IList<T> items) : base(items)
        {
        }

        public IVerifySnapshot<T> ForEach(Action<T> visitor)
        {
            foreach (var item in base.Items)
            {
                visitor.Invoke(item);
            }

            return this;
        }

        public IVerifySnapshot<T> ForEach(Action<T, int> visitor)
        {
            var count = 0;
            
            foreach (var item in base.Items)
            {
                visitor.Invoke(item, count++);
            }

            return this;
        }
    }
}