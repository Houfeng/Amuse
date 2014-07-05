using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public static class MethodCache
    {
        private static object m_mutex = new object();
        private static Dictionary<Type, Dictionary<string, MethodInfo>> m_cache = new Dictionary<Type, Dictionary<string, MethodInfo>>();
        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            MethodInfo methodInfo;
            Dictionary<string, MethodInfo> methodCache;

            if (m_cache.TryGetValue(type, out methodCache))
            {
                if (methodCache.TryGetValue(methodName, out methodInfo))
                {
                    return methodInfo;
                }
            }

            lock (m_mutex)
            {
                if (!m_cache.ContainsKey(type))
                {
                    m_cache[type] = new Dictionary<string, MethodInfo>();
                }

                MethodInfo method = type.GetMethod(methodName);
                m_cache[type][methodName] = method;
                return method;
            }
        }
    }
}
