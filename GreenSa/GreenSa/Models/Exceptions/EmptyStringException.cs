using System;

namespace GreenSa.Models.Exceptions
{
    internal class EmptyStringException : Exception
    {
        public EmptyStringException()
        {
        }

        public EmptyStringException(string message) : base(message)
        {
        }

        public EmptyStringException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}