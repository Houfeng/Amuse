using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public class TypeCache
    {
        private static object m_mutex = new object();
        private static Dictionary<string, Type> m_cache = new Dictionary<string, Type>();

        public static Type GetType(string typeFullName)
        {
            Type type;
            if (m_cache.TryGetValue(typeFullName, out type))
            {
                return type;
            }
            lock (m_mutex)
            {
                Assembly[] allAssembly = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in allAssembly)
                {
                    type = ass.GetType(typeFullName);
                    if (type != null)
                        break;
                }
                m_cache[typeFullName] = type;
                return type;
            }
        }
        public static Type GetType(string typeFullName, string assemblyFile)
        {
            if (string.IsNullOrWhiteSpace(assemblyFile))
            {
                return GetType(typeFullName);
            }
            string cacheKey = string.Format("{0}::{1}", assemblyFile, typeFullName);
            Type type;
            if (m_cache.TryGetValue(cacheKey, out type))
            {
                return type;
            }
            lock (m_mutex)
            {
                Assembly assembly = Assembly.LoadWithPartialName(assemblyFile);
                if (assembly == null)
                {
                    assembly = Assembly.Load(assemblyFile);
                }
                if (assembly == null)
                {
                    assembly = Assembly.LoadFrom(assemblyFile);
                }
                type = assembly.GetType(typeFullName);
                m_cache[cacheKey] = type;
                return type;
            }
        }
    }
}
