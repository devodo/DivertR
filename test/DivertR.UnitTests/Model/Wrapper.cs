using System.Collections.Generic;

namespace DivertR.UnitTests.Model
{
    public class Wrapper<T>
    {
        public T Item { get; set; }

        public Wrapper(T item)
        {
            Item = item;
        }

        private bool Equals(Wrapper<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Item, other.Item);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Wrapper<T>) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode (Used for testing)
            return EqualityComparer<T>.Default.GetHashCode(Item!);
        }
    }
}