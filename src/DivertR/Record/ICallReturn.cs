using System;

namespace DivertR.Record
{
    public interface ICallReturn
    {
        public bool IsValue { get; }
        
        public bool IsException { get; }
        
        object? Value { get; }
        
        Exception? Exception { get; }
    }
    
    public interface ICallReturn<out TReturn> : ICallReturn
    {
        new TReturn Value { get; }
    }
}
