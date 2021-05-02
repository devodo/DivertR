using System;

namespace DivertR.Redirects
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
