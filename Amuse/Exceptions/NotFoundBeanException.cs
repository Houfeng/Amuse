using System;

namespace Amuse.Exceptions
{
    public class NotFoundBeanException : Exception
    {
        public NotFoundBeanException()
        {
        }

        public NotFoundBeanException(string message)
            : base(message)
        {
        }

        public NotFoundBeanException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
