﻿using System.Runtime.CompilerServices;

namespace DivertR
{
    public class TrueCallConstraint : ICallConstraint
    {
        public static readonly TrueCallConstraint Instance = new TrueCallConstraint();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            return true;
        }
    }
}
