using System;

namespace NMorph
{
    public class MorphException : Exception
    {
        public MorphException()
        {
        }

        public MorphException(string message) : base(message)
        {
        }

        public MorphException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}