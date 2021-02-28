using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class ValueTaskFoo : IValueTaskFoo
    {
        private readonly Func<ValueTask<string>> _messageFactory;

        public ValueTaskFoo(Func<ValueTask<string>> messageFactory)
        {
            _messageFactory = messageFactory;
        }
        public ValueTaskFoo(string message) : this( () => new ValueTask<string>(message))
        {
        }

        public ValueTask<string> MessageAsync => _messageFactory.Invoke();
    }
}