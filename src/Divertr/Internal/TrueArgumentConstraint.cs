using DivertR.Core;

namespace DivertR.Internal
{
    internal class TrueArgumentConstraint : IArgumentConstraint
    {
        public static readonly TrueArgumentConstraint Instance = new TrueArgumentConstraint();
        
        public bool IsMatch(object? argument)
        {
            return true;
        }
    }
}