﻿namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        public Foo() {}
        public Foo(string message)
        {
            Message = message;
        }
        
        public string Message { get; init; }
    }
}