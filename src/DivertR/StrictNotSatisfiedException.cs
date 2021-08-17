using System;
using DivertR.Core;

namespace DivertR
{
    public class StrictNotSatisfiedException : DiverterException
    {
        public StrictNotSatisfiedException()
        {
        }

        public StrictNotSatisfiedException(string message) : base(message)
        {
        }

        public StrictNotSatisfiedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}