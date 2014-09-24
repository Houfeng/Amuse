using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public class ParameterFactory
    {
        private static object locker = new object();
        private static Dictionary<MethodInfo, ParameterInfo[]> parameterInfoListCache = new Dictionary<MethodInfo, ParameterInfo[]>();
        public static ParameterInfo[] GetParameterInfos(MethodInfo methodInfo)
        {
            ParameterInfo[] pList;
            if (parameterInfoListCache.TryGetValue(methodInfo, out pList))
                return pList;
            lock (locker)
            {
                parameterInfoListCache[methodInfo] = methodInfo.GetParameters();
            }
            return parameterInfoListCache[methodInfo];
        }

    }
}
