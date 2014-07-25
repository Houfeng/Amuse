using Amuse.Exceptions;
using Amuse.Extends;
using Amuse.Models;
using Amuse.Reflection;
using Amuse.Serializes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amuse
{
    //TODO: 1)解析 Bean 的 File 节，以支持外部 DLL; 2)Property Value 是否调整为 XML
    internal class BeanFactory
    {
        private Beans Beans { get; set; }
        private JsonSerializer Serializer { get; set; }
        public BeanFactory(string configFile)
        {
            this.Beans = new Beans(configFile);
            this.Serializer = new JsonSerializer();
        }
        private static Dictionary<string, object> singletonCache = new Dictionary<string, object>();
        public object Create(string beanName)
        {
            return this.FindAndCreate(beanName);
        }
        public List<object> CreateByGroup(string groupName)
        {
            List<Bean> beanList = this.Beans.Where(b => b.Group == groupName).ToList();
            List<object> instanceList = new List<object>();
            foreach (var bean in beanList)
            {
                instanceList.Add(this.Create(bean.Name));
            }
            return instanceList;
        }
        private object FindAndCreate(string beanName)
        {
            return this.FindAndCreate(beanName, new List<string>());
        }
        private object FindAndCreate(string beanName, List<string> injectionLink)
        {
            if (string.IsNullOrWhiteSpace(beanName))
            {
                throw new ArgumentNullException(string.Format("‘Bean 名称’ 不能是一个空值"));
            }
            var foundBeans = this.Beans.Where(b => b.Name == beanName).ToList();
            if (foundBeans == null || foundBeans.Count < 1)
            {
                throw new NotFoundBeanException(string.Format("没有找到名称为‘{0}’的 Bean 配置", beanName));
            }
            var bean = foundBeans[0];
            object instance = null;
            if (singletonCache.TryGetValue(beanName, out instance))
            {
                return instance;
            }
            else
            {
                instance = this.CreateInstance(bean);
                injectionLink.Add(beanName);
                instance = this.PropertyInjection(instance, bean, injectionLink);
                //如果不是多例模式，就放入缓存，下次从缓存中获取，实现单例
                if (bean.Mode != "multiton")
                {
                    singletonCache[beanName] = instance;
                }
                return instance;
            }
        }
        private object CreateInstance(Bean bean)
        {
            bean.Class = bean.Class ?? "";
            string[] beanClassInfo = bean.Class.Split(',');
            Type beanType = beanClassInfo.Length > 1 ? TypeCache.GetType(beanClassInfo[0], beanClassInfo[1]) : TypeCache.GetType(beanClassInfo[0]);
            if (beanType == null)
            {
                throw new TypeNotFoundException(string.Format("‘{0}’ 没有找到", bean.Class));
            }
            if (string.IsNullOrWhiteSpace(bean.FactoryMethod))
            {
                return Activator.CreateInstance(beanType);
            }
            else
            {
                MethodInfo methodInfo = MethodCache.GetMethodInfo(beanType, bean.FactoryMethod);
                if (methodInfo == null)
                {
                    throw new MethodNotFoundException(string.Format("‘{0}’的‘{1}’没有找到", bean.Class, bean.FactoryMethod));
                }
                return methodInfo.Invoke(null, null);
            }
        }
        private object PropertyInjection(object instance, Bean bean, List<string> injectionLink)
        {
            foreach (Property property in bean.Properties)
            {
                if (property.Ref != null)
                {
                    if (injectionLink.Contains(property.Ref))
                    {
                        injectionLink.Add(property.Ref);
                        var linkString = string.Join(">", injectionLink.ToArray());
                        throw new CircularDependencyException(string.Format("在‘{0}.{1}’发现循环依赖，依赖链:‘{2}’", bean.Name, property.Name, linkString));
                    }
                    var deptInstance = this.FindAndCreate(property.Ref, injectionLink);
                    instance.SetPropertyValue(property.Name, deptInstance);
                }
                else if (property.Text != null)
                {
                    PropertyInfo propertyInfo = instance.GetProperties()
                        .FirstOrDefault(p => p.Name == property.Name);
                    if (propertyInfo == null)
                    {
                        throw new PropertyNotFoundException(string.Format("‘{0}’的‘{1}’没有找到", bean.Class, property.Name));
                    }
                    var propertyValue = property.Text.ConvertTo(propertyInfo.PropertyType, null);
                    if (propertyValue == null)
                    {
                        propertyValue = this.Serializer.Deserialize(property.Text, propertyInfo.PropertyType);
                    }
                    instance.SetPropertyValue(property.Name, propertyValue);
                }
            }
            return instance;
        }
    }
}
