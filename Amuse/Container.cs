using Amuse.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Amuse
{
    /// <summary>
    /// Amuse 容器
    /// </summary>
    public class Container
    {
        private BeanFactory BeanFactory { get; set; }
        private Container(string configFile)
        {
            this.BeanFactory = new BeanFactory(configFile);
        }

        #region 创建容器
        private static Dictionary<string, Container> ContainerCache = new Dictionary<string, Container>();
        private static Container CreateByFile(string configFile)
        {
            lock (ContainerCache)
            {
                if (!File.Exists(configFile))
                {
                    throw new ConfigNotFoundException(string.Format("‘{0}’ 容器配置文件没有找到,也没没有发现名为‘{0}’的 AppSetting 配置节", configFile));
                }
                if (!ContainerCache.ContainsKey(configFile))
                {
                    ContainerCache[configFile] = new Container(configFile);
                }
                return ContainerCache[configFile];
            }
        }
        /// <summary>
        /// 创建一个对象容器
        /// </summary>
        /// <param name="configFileOrAppSettingKey">“配置文件路径” 或 “AppSetting配置节 Key”</param>
        /// <returns>容器</returns>
        public static Container Create(string configFileOrAppSettingKey)
        {
            if (ConfigurationManager.AppSettings[configFileOrAppSettingKey] != null)
            {
                return CreateByFile(ConfigurationManager.AppSettings[configFileOrAppSettingKey]);
            }
            else
            {
                return CreateByFile(configFileOrAppSettingKey);
            }
        }
        /// <summary>
        /// 默认容器
        /// </summary>
        /// <returns></returns>
        public static Container Create()
        {
            return Create(string.Format("{0}\\Amuse.config", AppDomain.CurrentDomain.BaseDirectory));
        }
        #endregion

        #region 获取实例
        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <typeparam name="T">对应类型（一般为接口）</typeparam>
        /// <param name="beanName">Bean Name</param>
        /// <returns>实例</returns>
        public T Get<T>(string beanName)
        {
            return (T)this.Get(beanName);
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <param name="beanName">Bean Name</param>
        /// <returns>实例</returns>
        public object Get(string beanName)
        {
            return this.BeanFactory.Create(beanName);
        }
        /// <summary>
        /// 获取一组实例
        /// </summary>
        /// <param name="beanGroup">Bean Group Name</param>
        /// <returns>实例列表</returns>
        public List<object> GetByGroup(string beanGroup)
        {
            return this.BeanFactory.CreateByGroup(beanGroup);
        }
        /// <summary>
        /// 获取一组实例
        /// </summary>
        /// <typeparam name="T">对应类型（一般为接口）</typeparam>
        /// <param name="beanGroup">Bean Group Name</param>
        /// <returns>实例列表</returns>
        public List<T> GetByGroup<T>(string beanGroup)
        {
            List<object> srcInstanceList = this.BeanFactory.CreateByGroup(beanGroup);
            List<T> dstInstanceList = new List<T>();
            foreach (var srcInstance in srcInstanceList)
            {
                dstInstanceList.Add((T)srcInstance);
            }
            return dstInstanceList;
        }
        #endregion
    }
}
