using System;

namespace DivertR.Core
{
    [Serializable]
    public class DiverterException : Exception
    {
        public DiverterException()
        {
        }

        public DiverterException(string message) : base(message)
        {
        }

        public DiverterException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}