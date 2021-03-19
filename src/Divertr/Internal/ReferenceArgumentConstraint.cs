using DivertR.Core;

namespace DivertR.Internal
{
    internal class ReferenceArgumentConstraint : IArgumentConstraint
    {
        private readonly object _instance;

        public ReferenceArgumentConstraint(object instance)
        {
            _instance = instance;
        }
        
        public bool IsMatch(object? argument)
        {
            return ReferenceEquals(_instance, argument);
        }
    }
}