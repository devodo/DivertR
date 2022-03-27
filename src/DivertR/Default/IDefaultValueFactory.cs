using System;

namespace DivertR.Default
{
    public interface IDefaultValueFactory
    {
        object? GetDefaultValue(Type type);
    }
}