using System;

namespace DivertR.Default
{
    public interface IDefaultValueFactory
    {
        object? Create(Type type);
    }
}