/*
 * 版本: 0.1
 * 描述: 针对数据访问层对一些对象的扩展方法。
 * 创建: Houfeng
 * 邮件: houzf@prolliance.cn
 * 
 * 修改记录:
 * 2011-11-7,Houfeng,添加文件说明，更新版本号为0.1
 */

using Amuse.Reflection;
using System;
using System.Reflection;

namespace Amuse.Extends
{
    public static class ObjectExtends
    {
        public static object ConvertTo(this object entity, Type pType)
        {
            return ConvertTo(entity, pType, entity);
        }
        public static object ConvertTo(this object entity, Type pType, object defaultReturn)
        {
            if (pType == typeof(DateTime) || pType == typeof(DateTime?))
                return Convert.ToDateTime(entity);
            else if (pType == typeof(int) || pType == typeof(int?))
                return Convert.ToInt32(Convert.ToDouble(entity));
            else if (pType == typeof(string))
                return Convert.ToString(entity);
            else if (pType == typeof(double) || pType == typeof(double?))
                return Convert.ToDouble(entity);
            else if (pType == typeof(decimal) || pType == typeof(decimal?))
                return Convert.ToDecimal(entity);
            else if (pType == typeof(bool) || pType == typeof(bool?))
                return Convert.ToBoolean(entity);
            else if (pType == typeof(float) || pType == typeof(float?))
                return (float)(entity);
            else if (pType == typeof(object))
                return entity;
            else
            {
                return defaultReturn;
            }
        }
        public static object InvokeMethod(this object entity, string methodName, object[] parameters)
        {
            try
            {
                MethodInfo methodInfo = MethodCache.GetMethodInfo(entity.GetType(), methodName);
                if (methodInfo != null)
                {
                    ParameterInfo[] pareameterInfos = ParameterCache.GetPropertyInfo(methodInfo);
                    return methodInfo.Invoke(entity, parameters);
                }
                else
                {
                    throw new Exception("没有找到指定调用方法");
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("调用指定调用方法时出错,{0}", e.Message));
            }
        }
        public static void SetPropertyValue(this object entity, string propertyName, object value)
        {
            PropertyInfo property = PropertyCache.GetPropertyInfo(entity.GetType(), propertyName);
            if (property != null)
            {
                property.SetValue(entity, value.ConvertTo(property.PropertyType), null);
            }
        }
        public static object GetPropertyValue(this object entity, string propertyName)
        {
            PropertyInfo property = PropertyCache.GetPropertyInfo(entity.GetType(), propertyName);
            if (property != null)
                return property.GetValue(entity, null);
            return null;
        }
        public static PropertyInfo[] GetProperties(this object entity)
        {
            return entity.GetType().GetProperties();
        }
        public static MethodInfo[] GetMethods(this object entity)
        {
            return entity.GetType().GetMethods();
        }
    }
}
