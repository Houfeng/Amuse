using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amuse.Reflection
{
    public class TypeFactory
    {
        private static object Locker = new object();
        private static Dictionary<string, Type> Cache = new Dictionary<string, Type>();

        public static Type GetType(string typeFullName)
        {
            Type type;
            if (Cache.TryGetValue(typeFullName, out type))
            {
                return type;
            }
            lock (Locker)
            {
                Assembly[] allAssembly = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in allAssembly)
                {
                    type = ass.GetType(typeFullName);
                    if (type != null)
                        break;
                }
                Cache[typeFullName] = type;
                return type;
            }
        }
        public static Type GetType(string typeFullName, string assemblyFile)
        {
            if (string.IsNullOrEmpty(assemblyFile))
            {
                return GetType(typeFullName);
            }
            string cacheKey = string.Format("{0}::{1}", assemblyFile, typeFullName);
            Type type;
            if (Cache.TryGetValue(cacheKey, out type))
            {
                return type;
            }
            lock (Locker)
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
                Cache[cacheKey] = type;
                return type;
            }
        }
    }
}
