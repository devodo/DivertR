using System;

namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        private readonly Func<string> _messageFactory;

        public Foo(Func<string> messageFactory)
        {
            _messageFactory = messageFactory;
        }
        public Foo(string message) : this(() => message)
        {
        }

        public string Message => _messageFactory.Invoke();
    }
}