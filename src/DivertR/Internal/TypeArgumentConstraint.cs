using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TypeArgumentConstraint<TArgument> : IArgumentConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            if (argument == null)
            {
                return true;
            }

            return argument is TArgument;
        }
    }
}