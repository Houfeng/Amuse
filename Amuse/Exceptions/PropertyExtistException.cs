using System;

namespace Amuse.Exceptions
{
    public class PropertyExtistException : Exception
    {
        public PropertyExtistException()
        {
        }

        public PropertyExtistException(string message)
            : base(message)
        {
        }

        public PropertyExtistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
