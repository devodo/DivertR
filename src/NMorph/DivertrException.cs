using System;

namespace NMorph
{
    public class DivertrException : Exception
    {
        public DivertrException()
        {
        }

        public DivertrException(string message) : base(message)
        {
        }

        public DivertrException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}