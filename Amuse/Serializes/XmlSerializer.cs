using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Amuse.Serializes
{
    public class EasyXmlSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            //声明Xml序列化对象实例serializer
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            //执行序列化并将序列化结果输出到控制台
            StringBuilder buffer = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(buffer);
            serializer.Serialize(xmlWriter, obj);
            return buffer.ToString();
        }
        public T Deserialize<T>(string text)
        {
            //声明序列化对象实例serializer 
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            //反序列化，并将反序列化结果值赋给变量i
            XmlReader xmlReader = XmlReader.Create(new StringReader(text));
            return (T)serializer.Deserialize(xmlReader);
        }

        public object Deserialize(string text, Type type)
        {
            //声明序列化对象实例serializer 
            XmlSerializer serializer = new XmlSerializer(type);
            //反序列化，并将反序列化结果值赋给变量i
            XmlReader xmlReader = XmlReader.Create(new StringReader(text));
            return serializer.Deserialize(xmlReader);
        }
    }
}
