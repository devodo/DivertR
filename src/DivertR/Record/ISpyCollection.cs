﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivertR.Record
{
    public interface ISpyCollection<TMap> : IReadOnlyCollection<TMap>
    {
        int Scan(Action<TMap> visitor);
        int Scan(Action<TMap, int> visitor);
    }
}