using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class TrueArgumentConstraint : IArgumentConstraint
    {
        public static readonly TrueArgumentConstraint Instance = new TrueArgumentConstraint();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(object? argument)
        {
            return true;
        }
    }
}