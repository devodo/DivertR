using System;
using System.Diagnostics.CodeAnalysis;

namespace DivertR.Record
{
    public interface ICallReturn
    {
        object? Value { get; }
        
        Exception? Exception { get; }
    }
    
    public interface ICallReturn<out TReturn> : ICallReturn
    {
        [AllowNull]
        new TReturn Value { get; }
    }
}
