using System;

namespace Amuse.Exceptions
{
    public class BeanExtistException : Exception
    {
        public BeanExtistException()
        {
        }

        public BeanExtistException(string message)
            : base(message)
        {
        }

        public BeanExtistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
