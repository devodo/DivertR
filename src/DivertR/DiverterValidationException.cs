using System;

namespace DivertR
{
    public class DiverterValidationException : DiverterException
    {
        public DiverterValidationException()
        {
        }

        public DiverterValidationException(string message) : base(message)
        {
        }

        public DiverterValidationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}