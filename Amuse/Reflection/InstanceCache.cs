using System;
using System.Collections.Generic;

namespace Amuse.Reflection
{
    public static class InstanceCache
    {
        private static object m_mutex = new object();
        private static Dictionary<Type, object> m_cache = new Dictionary<Type, object>();

        public static object GetInstance(Type type)
        {
            object ins;
            if (m_cache.TryGetValue(type, out ins))
            {
                return ins;
            }

            lock (m_mutex)
            {
                ins = type.Assembly.CreateInstance(type.FullName);
                m_cache[type] = ins;
                return ins;
            }
        }

    }

}
