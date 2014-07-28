using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public static class MethodFactory
    {
        private static object Locker = new object();
        private static Dictionary<string, MethodInfo> Cache = new Dictionary<string, MethodInfo>();

        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            return GetMethodInfo(type, methodName, null);
        }
        public static MethodInfo GetMethodInfo(Type type, string methodName, Type[] argsTypes)
        {
            MethodInfo methodInfo;
            //
            string cacheKey = type.FullName + ":" + methodName;
            if (argsTypes != null)
            {
                foreach (Type argType in argsTypes)
                {
                    cacheKey += ":" + argType.FullName;
                }
            }
            //
            if (Cache.TryGetValue(cacheKey, out methodInfo))
            {
                return methodInfo;
            }
            //
            lock (Locker)
            {
                MethodInfo method = null;
                if (argsTypes != null)
                    method = type.GetMethod(methodName, argsTypes);
                else
                    method = type.GetMethod(methodName);
                Cache[cacheKey] = method;
                return method;
            }
        }

    }
}
