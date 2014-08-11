using System;

namespace Amuse.Exceptions
{
    public class ObjectExtistException : Exception
    {
        public ObjectExtistException()
        {
        }

        public ObjectExtistException(string message)
            : base(message)
        {
        }

        public ObjectExtistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
