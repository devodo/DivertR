﻿using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsMatch(ICallInfo callInfo);
    }
}