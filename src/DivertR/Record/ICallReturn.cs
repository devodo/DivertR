using System;

namespace DivertR.Record
{
    public interface ICallReturn
    {
        object? Value { get; }
        
        Exception? Exception { get; }
    }
    
    public interface ICallReturn<out TReturn> : ICallReturn
    {
        new TReturn Value { get; }
    }
}
