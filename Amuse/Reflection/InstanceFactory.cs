using System;
using System.Collections.Generic;

namespace Amuse.Reflection
{
    public static class InstanceFactory
    {
        public static object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        public static object GetInstance(Type type, object[] args)
        {
            return Activator.CreateInstance(type, args);
        }
    }

}
