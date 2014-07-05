using System;

namespace Amuse.Exceptions
{
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException()
        {
        }

        public MethodNotFoundException(string message)
            : base(message)
        {
        }

        public MethodNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
