using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class AsyncFoo : IAsyncFoo
    {
        private readonly Func<Task<string>> _messageFactory;

        public AsyncFoo(Func<Task<string>> messageFactory)
        {
            _messageFactory = messageFactory;
        }
        public AsyncFoo(string message) : this(async () =>
        {
            await Task.Yield();
            return message;
        })
        {
        }

        public Task<string> MessageAsync => _messageFactory.Invoke();
    }
}