using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Extends
{
    public static class PropertyExtends
    {
        public static T GetAttribute<T>(this PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(true);
            foreach (object att in attributes)
            {
                if (att is T)
                    return (T)att;
            }
            return default(T);
        }
        public static List<T> GetAttributes<T>(this PropertyInfo property)
        {
            List<T> attrList = new List<T>();
            object[] attributes = property.GetCustomAttributes(true);
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
