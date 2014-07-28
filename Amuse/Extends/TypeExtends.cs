using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Extends
{
    public static class TypeExtends
    {
        public static T GetAttribute<T>(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(true);
            foreach (object att in attributes)
            {
                if (att is T)
                    return (T)att;
            }
            return default(T);
        }
        public static List<T> GetAttributes<T>(this MethodInfo method)
        {
            List<T> attrList = new List<T>();
            object[] attributes = method.GetCustomAttributes(true);
            foreach (object att in attributes)
            {
                if (att is T)
                {
                    attrList.Add((T)att);
                }
            }
            return attrList;
        }
    }
}
