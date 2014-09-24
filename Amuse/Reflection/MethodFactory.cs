using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public static class MethodFactory
    {
        private static object Locker = new object();
        private static Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();

        private static object listLocker = new object();
        private static Dictionary<Type, MethodInfo[]> methodListCache = new Dictionary<Type, MethodInfo[]>();

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
            if (methodCache.TryGetValue(cacheKey, out methodInfo))
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
                methodCache[cacheKey] = method;
                return method;
            }
        }

        public static MethodInfo[] GetMethodInfos(Type type)
        {
            MethodInfo[] methodList;
            if (methodListCache.TryGetValue(type, out methodList))
                return methodList;
            lock (listLocker)
            {
                methodListCache[type] = type.GetMethods();
            }
            return methodListCache[type];
        }
    }
}
