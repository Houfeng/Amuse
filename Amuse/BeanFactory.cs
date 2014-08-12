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
                throw new ArgumentNullException(string.Format("‘Bean 名称’ 不能是一个空值。"));
            }
            //找出名称为 beanName 的 Bean 对象
            var foundBeans = this.Beans.Where(b => b.Name == beanName).ToList();
            if (foundBeans == null || foundBeans.Count < 1)
            {
                throw new ObjectNotFoundException(string.Format("没有找到名称为 ‘{0}’ 的 Bean 配置。", beanName));
            }
            var bean = foundBeans[0];
            //查检缓存中是否存在（配置成单例模式的 Bean 会被缓存）
            object instance = null;
            if (singletonCache.TryGetValue(beanName, out instance))
            {
                return instance;
            }
            else
            {
                injectionLink.Add(beanName);
                instance = this.CreateInstance(bean, injectionLink);
                instance = this.PropertyInjection(instance, bean, injectionLink);
                instance = this.MethodInjection(instance, bean, injectionLink);
                //如果单例模式，就放入缓存，下次从缓存中获取，实现单例
                if (string.IsNullOrWhiteSpace(bean.Mode)
                    || bean.Mode == ModeType.Singleton)
                {
                    singletonCache[beanName] = instance;
                }
                return instance;
            }
        }
        private Type ParseType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }
            string[] typeInfo = typeName.Split(',');
            Type type = typeInfo.Length > 1 ? TypeFactory.GetType(typeInfo[0], typeInfo[1]) : TypeFactory.GetType(typeInfo[0]);
            return type;
        }
        private object CreateInstance(Bean bean, List<string> injectionLink)
        {
            if (bean == null) return null;
            Type beanType = this.ParseType(bean.Type);
            if (beanType == null)
            {
                throw new ObjectNotFoundException(string.Format("‘{0}’ 没有找到。", bean.Type));
            }
            //开始尝试创建实例
            if (!string.IsNullOrWhiteSpace(bean.FactoryMethod))
            {
                //使用静态工厂方法创建实例
                MethodInfo methodInfo = MethodFactory.GetMethodInfo(beanType, bean.FactoryMethod);
                if (methodInfo == null)
                {
                    throw new ObjectNotFoundException(string.Format(" ‘{0}’ 的 ‘{1}’ 没有找到。", bean.Type, bean.FactoryMethod));
                }
                return methodInfo.Invoke(null, null);
            }
            else if (bean.Constructor != null)
            {
                //使用指定构造创建实例
                List<object> parameterValues = new List<object>();
                List<Type> parameterTypes = new List<Type>();
                ConstructorInfo[] constructorInfoList = beanType.GetConstructors();
                foreach (ConstructorInfo constructorInfo in constructorInfoList)
                {
                    parameterValues.Clear();
                    ParameterInfo[] parameterInfoList = constructorInfo.GetParameters();
                    if (bean.Constructor.Parameters.Count() == parameterInfoList.Count())
                    {
                        foreach (ParameterInfo parameterInfo in parameterInfoList)
                        {
                            Parameter parameter = bean.Constructor.Parameters
                                .FirstOrDefault(p => p.Name == parameterInfo.Name);
                            //如果一个参数无法匹配说明这个构造方法不匹配，测试跳出参数循环，开始尝试下一个构造函数
                            if (parameter == null) break;
                            Type parameterType = this.ParseType(parameter.Type);
                            parameterType = parameterType != null ? parameterType : parameterInfo.ParameterType;
                            parameterTypes.Add(parameterType);
                            if (!string.IsNullOrWhiteSpace(parameter.Ref))
                            {
                                if (injectionLink.Contains(parameter.Ref))
                                {
                                    injectionLink.Add(parameter.Ref);
                                    var linkString = string.Join(">", injectionLink.ToArray());
                                    throw new CircularDependencyException(string.Format("在 ‘{0}’ 的构造方法的 ‘{1}’ 参数，发现循环依赖，依赖链: ‘{2}’ 。", bean.Name, parameter.Name, linkString));
                                }
                                var deptInstance = this.FindAndCreate(parameter.Ref, injectionLink);
                                parameterValues.Add(deptInstance);
                            }
                            else
                            {
                                var parameterValue = parameter.Value.ConvertTo(parameterType, null);
                                if (parameterValue == null)
                                {
                                    parameterValue = this.Serializer.Deserialize(parameter.Value, parameterType);
                                }
                                parameterValues.Add(parameterValue);
                            }
                        }
                    }
                }
                return Activator.CreateInstance(beanType, parameterValues.ToArray());
            }
            else
            {
                //使用默认构造创建实例
                return Activator.CreateInstance(beanType, true);
            }
        }
        private object PropertyInjection(object instance, Bean bean, List<string> injectionLink)
        {
            if (instance == null || bean == null || bean.Properties == null || bean.Properties.Count < 1)
            {
                return instance;
            }
            foreach (Property property in bean.Properties)
            {
                PropertyInfo propertyInfo = instance.GetProperties()
                        .FirstOrDefault(p => p.Name == property.Name);
                if (propertyInfo == null)
                {
                    throw new ObjectNotFoundException(string.Format(" ‘{0}’ 的属性 ‘{1}’ 没有找到。", bean.Type, property.Name));
                }
                if (!string.IsNullOrWhiteSpace(property.Ref))
                {
                    if (injectionLink.Contains(property.Ref))
                    {
                        injectionLink.Add(property.Ref);
                        var linkString = string.Join(">", injectionLink.ToArray());
                        throw new CircularDependencyException(string.Format("在 ‘{0}’ 的属性 ‘{1}’ 发现循环依赖，依赖链: ‘{2}’ 。", bean.Name, property.Name, linkString));
                    }
                    var deptInstance = this.FindAndCreate(property.Ref, injectionLink);
                    instance.SetPropertyValue(property.Name, deptInstance);
                }
                else
                {
                    Type propertyType = this.ParseType(property.Type);
                    propertyType = propertyType != null ? propertyType : propertyInfo.PropertyType;
                    var propertyValue = property.Value.ConvertTo(propertyType, null);
                    if (propertyValue == null)
                    {
                        propertyValue = this.Serializer.Deserialize(property.Value, propertyType);
                    }
                    instance.SetPropertyValue(property.Name, propertyValue);
                }
            }
            return instance;
        }
        private object MethodInjection(object instance, Bean bean, List<string> injectionLink)
        {
            if (instance == null || bean == null || bean.Properties == null || bean.Methods.Count < 1)
            {
                return instance;
            }
            foreach (Method method in bean.Methods)
            {
                List<MethodInfo> methodInfoList = instance.GetMethods()
                    .Where(m => m.Name == method.Name)
                    .ToList();
                if (methodInfoList == null || methodInfoList.Count < 1)
                {
                    throw new ObjectNotFoundException(string.Format(" ‘{0}’ 的方法 ‘{1}’ 没有找到。", bean.Type, method.Name));
                }
                //--
                //使用指定构造创建实例
                List<object> parameterValues = new List<object>();
                List<Type> parameterTypes = new List<Type>();
                foreach (MethodInfo methodInfo in methodInfoList)
                {
                    parameterValues.Clear();
                    ParameterInfo[] parameterInfoList = methodInfo.GetParameters();
                    if (method.Parameters.Count() == parameterInfoList.Count())
                    {
                        foreach (ParameterInfo parameterInfo in parameterInfoList)
                        {
                            Parameter parameter = method.Parameters
                                .FirstOrDefault(p => p.Name == parameterInfo.Name);
                            //如果一个参数无法匹配说明这个构造方法不匹配，测试跳出参数循环，开始尝试下一个构造函数
                            if (parameter == null) break;
                            Type parameterType = this.ParseType(parameter.Type);
                            parameterType = parameterType != null ? parameterType : parameterInfo.ParameterType;
                            parameterTypes.Add(parameterType);
                            if (!string.IsNullOrWhiteSpace(parameter.Ref))
                            {
                                if (injectionLink.Contains(parameter.Ref))
                                {
                                    injectionLink.Add(parameter.Ref);
                                    var linkString = string.Join(">", injectionLink.ToArray());
                                    throw new CircularDependencyException(string.Format("在 ‘{0}’ 的构造方法的 ‘{1}’ 参数，发现循环依赖，依赖链: ‘{2}’ 。", bean.Name, parameter.Name, linkString));
                                }
                                var deptInstance = this.FindAndCreate(parameter.Ref, injectionLink);
                                parameterValues.Add(deptInstance);
                            }
                            else
                            {
                                var parameterValue = parameter.Value.ConvertTo(parameterType, null);
                                if (parameterValue == null)
                                {
                                    parameterValue = this.Serializer.Deserialize(parameter.Value, parameterType);
                                }
                                parameterValues.Add(parameterValue);
                            }
                        }
                    }
                }
                MethodInfo willInvokeMethodInfo = instance.GetType().GetMethod(method.Name, parameterTypes.ToArray());
                if (willInvokeMethodInfo == null)
                {
                    throw new ObjectNotFoundException(string.Format(" ‘{0}’ 的方法 ‘{1}’ 没有找到。", bean.Type, method.Name));
                }
                willInvokeMethodInfo.Invoke(instance, parameterValues.ToArray());
            }
            return instance;
        }
    }
}
