﻿using System;

namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        private Func<string> _messageFactory;
        
        public Foo()
        {
        }

        public Foo(Func<string> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public Foo(string message) : this(() => message)
        {
        }

        public virtual string Message
        {
            get => _messageFactory.Invoke();
            set
            {
                _messageFactory = () => value;
            }
        } 
        
        public string GetMessage(string input)
        {
            return _messageFactory.Invoke() + input;
        }

        public string SetMessage(Wrapper<string> input)
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