using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class AsyncNumber : IAsyncNumber
    {
        private readonly Func<int, Task<int>> _numberFactory;

        public AsyncNumber() : this(async i =>
        {
            await Task.Yield();
            return i;
        })
        {
        }

        public AsyncNumber(Func<int, Task<int>> numberFactory)
        {
            _numberFactory = numberFactory;
        }

        public Task<int> GetNumber(int input)
        {
            return _numberFactory.Invoke(input);
        }
    }
}