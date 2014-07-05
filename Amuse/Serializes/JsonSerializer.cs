/*
 * 版本: 0.1
 * 描述: json序列化类。
 * 创建: Houfeng
 * 邮件: houzf@prolliance.cn
 * 
 * 修改记录:
 * 2011-11-7,Houfeng,添加文件说明，更新版本号为0.1
 */

using System;
using System.Web.Script.Serialization;

namespace Amuse.Serializes
{
    public class JsonSerializer : ISerializer
    {
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public string Serialize(object obj)
        {
            return serializer.Serialize(obj);
        }
        public T Deserialize<T>(string text)
        {
            return serializer.Deserialize<T>(text);
        }
        public object Deserialize(string text, Type type)
        {
            return this.serializer.Deserialize(text, type);
        }
    }
}
