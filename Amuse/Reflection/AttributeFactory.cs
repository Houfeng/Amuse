using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Amuse.Reflection
{
    public class AttributeFactory
    {

        #region 有关类型特性
        private static object typeLocker = new object();
        private static Dictionary<Type, List<object>> typeAttrCache = new Dictionary<Type, List<object>>();

        public static List<object> GetCustomAttributes(Type type)
        {
            List<object> attrList;
            if (typeAttrCache.TryGetValue(type, out attrList))
            {
                return attrList;
            }
            object[] attributes = type.GetCustomAttributes(true);
            if (attributes != null)
            {
                lock (typeLocker)
                {
                    typeAttrCache[type] = attributes.ToList();
                }
            }
            return typeAttrCache[type];
        }

        public static List<T> GetAttributes<T>(Type type)
        {
            List<T> attrList = new List<T>();
            List<object> attributes = GetCustomAttributes(type);
            foreach (object att in attributes)
            {
                if (att is T)
                {
                    attrList.Add((T)att);
                }
            }
            return attrList;
        }

        public static T GetAttribute<T>(Type type)
        {
            List<T> attributes = GetAttributes<T>(type);
            if (attributes != null && attributes.Count > 0)
                return attributes[0];
            else
                return default(T);
        }
        #endregion

        #region 有关方法特性
        private static object methodLocker = new object();

        private static Dictionary<MethodInfo, List<object>> methodAttrCache = new Dictionary<MethodInfo, List<object>>();

        public static List<object> GetCustomAttributes(MethodInfo method)
        {
            List<object> attrList;
            if (methodAttrCache.TryGetValue(method, out attrList))
            {
                return attrList;
            }
            object[] attributes = method.GetCustomAttributes(true);
            if (attributes != null)
            {
                lock (methodLocker)
                {
                    methodAttrCache[method] = attributes.ToList();
                }
            }
            return methodAttrCache[method];
        }

        public static List<T> GetAttributes<T>(MethodInfo method)
        {
            List<T> attrList = new List<T>();
            List<object> attributes = GetCustomAttributes(method);
            foreach (object att in attributes)
            {
                if (att is T)
                {
                    attrList.Add((T)att);
                }
            }
            return attrList;
        }

        public static T GetAttribute<T>(MethodInfo method)
        {
            List<T> attributes = GetAttributes<T>(method);
            if (attributes != null && attributes.Count > 0)
                return attributes[0];
            else
                return default(T);
        }
        #endregion

        #region 有关属性特性
        private static object propertyLocker = new object();

        private static Dictionary<PropertyInfo, List<object>> propertyAttrCache = new Dictionary<PropertyInfo, List<object>>();

        public static List<object> GetCustomAttributes(PropertyInfo property)
        {
            List<object> attrList;
            if (propertyAttrCache.TryGetValue(property, out attrList))
            {
                return attrList;
            }
            object[] attributes = property.GetCustomAttributes(true);
            if (attributes != null)
            {
                lock (propertyLocker)
                {
                    propertyAttrCache[property] = attributes.ToList();
                }
            }
            return propertyAttrCache[property];
        }

        public static List<T> GetAttributes<T>(PropertyInfo property)
        {
            List<T> attrList = new List<T>();
            List<object> attributes = GetCustomAttributes(property);
            foreach (object att in attributes)
            {
                if (att is T)
                {
                    attrList.Add((T)att);
                }
            }
            return attrList;
        }

        public static T GetAttribute<T>(PropertyInfo property)
        {
            List<T> attributes = GetAttributes<T>(property);
            if (attributes != null && attributes.Count > 0)
                return attributes[0];
            else
                return default(T);
        }
        #endregion

    }
}
