using System;

namespace DivertR.Core
{
    public interface ITypeConstraint
    {
        bool IsMatch(Type type);
    }
}