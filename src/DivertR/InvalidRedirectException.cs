using System;

namespace DivertR
{
    public class InvalidRedirectException : DiverterException
    {
        public InvalidRedirectException()
        {
        }

        public InvalidRedirectException(string message) : base(message)
        {
        }

        public InvalidRedirectException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}