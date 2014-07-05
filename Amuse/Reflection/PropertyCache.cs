/*
 * 版本: 0.1
 * 描述: 反射属性缓存类。
 * 创建: Houfeng
 * 邮件: houzf@prolliance.cn
 * 
 * 修改记录:
 * 2011-11-7,Houfeng,添加文件说明，更新版本号为0.1
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public static class PropertyCache
    {
        private static object m_mutex = new object();
        private static Dictionary<Type, PropertyInfo[]> piListCache = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> piCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            Dictionary<string, PropertyInfo> propertyCache;
            if (piCache.TryGetValue(type, out propertyCache))
            {
                PropertyInfo propertyInfo;
                if (propertyCache.TryGetValue(propertyName, out propertyInfo))
                    return propertyInfo;
            }
            //----
            lock (m_mutex)
            {
                if (!piCache.ContainsKey(type))
                    piCache[type] = new Dictionary<string, PropertyInfo>();
                PropertyInfo property = type.GetProperty(propertyName);
                piCache[type][propertyName] = property;
                return property;
            }
        }
        public static PropertyInfo[] GetPropertyInfo(Type type)
        {
            PropertyInfo[] piList;
            if (piListCache.TryGetValue(type, out piList))
                return piList;
            piListCache[type] = type.GetProperties();
            return piListCache[type];
        }
    }
}
