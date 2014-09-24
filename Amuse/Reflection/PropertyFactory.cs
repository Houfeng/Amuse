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
    public static class PropertyFactory
    {
        private static object Locker = new object();
        private static object listLocker = new object();
        private static Dictionary<Type, PropertyInfo[]> propertyListCache = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> typePropertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            Dictionary<string, PropertyInfo> propertyCache;
            if (typePropertyCache.TryGetValue(type, out propertyCache))
            {
                PropertyInfo propertyInfo;
                if (propertyCache.TryGetValue(propertyName, out propertyInfo))
                    return propertyInfo;
            }
            //----
            lock (Locker)
            {
                if (!typePropertyCache.ContainsKey(type))
                    typePropertyCache[type] = new Dictionary<string, PropertyInfo>();
                PropertyInfo property = type.GetProperty(propertyName);
                typePropertyCache[type][propertyName] = property;
                return property;
            }
        }
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            PropertyInfo[] piList;
            if (propertyListCache.TryGetValue(type, out piList))
                return piList;
            lock (listLocker)
            {
                propertyListCache[type] = type.GetProperties();
            }
            return propertyListCache[type];
        }
    }
}
