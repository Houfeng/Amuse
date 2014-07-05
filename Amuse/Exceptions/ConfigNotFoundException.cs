using System;

namespace Amuse.Exceptions
{
    public class ConfigNotFoundException : Exception
    {

        public ConfigNotFoundException()
        {
        }

        public ConfigNotFoundException(string message)
            : base(message)
        {
        }

        public ConfigNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
