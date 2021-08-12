﻿using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class FooAlt : IFoo
    {
        private Func<string> _messageFactory;
        
        public FooAlt() : this("alternate")
        {
        }

        public FooAlt(string message)
        {
            _messageFactory = () => message;
        }
        
        public FooAlt(Func<string> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public virtual string Name
        {
            get => _messageFactory.Invoke();
            set => _messageFactory = () => value;
        }

        public async Task<string> GetNameAsync()
        {
            await Task.Yield();
            return _messageFactory.Invoke();
        }

        public string Echo(string input)
        {
            return input;
        }

        public string EchoGeneric<T>(T input)
        {
            return $"{input}";
        }

        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return input;
        }

        public async ValueTask<string> EchoValueAsync(string input)
        {
            await Task.Yield();
            return input;
        }

        public string SetName(Wrapper<string> input)
        {
            _messageFactory = () => input.Item;

            return _messageFactory.Invoke();
        }

        public IFoo GetFoo()
        {
            return this;
        }
    }
}