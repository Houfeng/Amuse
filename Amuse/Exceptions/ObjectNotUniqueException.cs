using System;

namespace Amuse.Exceptions
{
    public class ObjectNotUniqueException : Exception
    {

        public ObjectNotUniqueException()
        {
        }

        public ObjectNotUniqueException(string message)
            : base(message)
        {
        }

        public ObjectNotUniqueException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
