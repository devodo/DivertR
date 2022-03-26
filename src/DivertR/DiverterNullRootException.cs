using System;

namespace DivertR
{
    public class DiverterNullRootException : DiverterException
    {
        public DiverterNullRootException()
        {
        }

        public DiverterNullRootException(string message) : base(message)
        {
        }

        public DiverterNullRootException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}