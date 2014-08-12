using Amuse.Exceptions;
using Amuse.Extends;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Amuse.Models
{
    internal class Beans : List<Bean>
    {
        private XmlNamespaceManager nsMgr { get; set; }

        public Beans(string configFile)
        {
            string xmlBuffer = File.ReadAllText(configFile);
            XmlDocument xmlDoc = new XmlDocument();
            this.nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("amuse", "http://houfeng.net/Amuse.xsd");
            xmlDoc.LoadXml(xmlBuffer);
            this.Parse(xmlDoc);
        }

        private void Parse(XmlDocument xmlDoc)
        {
            XmlNodeList beanNodeList = xmlDoc.SelectNodes("/amuse:beans/amuse:bean", nsMgr);
            foreach (XmlNode beanNode in beanNodeList)
            {
                Bean bean = this.ParseBean(beanNode);
                if (this.Count > 0 && this.Exists(b => b.Name == bean.Name))
                {
                    throw new ObjectExtistException(string.Format("名称为 ‘{0}’ 的 Bean 已经存在。", bean.Name));
                }
                this.Add(bean);
            }
        }

        private Bean ParseBean(XmlNode beanNode)
        {
            var bean = new Bean();
            foreach (XmlAttribute attr in beanNode.Attributes)
            {
                bean.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            this.FindPropertyNode(ref beanNode, ref bean);
            this.FindConstructorNode(ref beanNode, ref bean);
            this.FindMethodNode(ref beanNode, ref bean);
            return bean;
        }

        private void FindPropertyNode(ref XmlNode beanNode, ref Bean bean)
        {
            //遍历属性节点
            XmlNodeList properyNodeList = beanNode.SelectNodes("./amuse:property", nsMgr);
            foreach (XmlNode properyNode in properyNodeList)
            {
                Property property = this.ParseProperty(properyNode, bean);
                if (bean.Properties.Count > 0 && bean.Properties.Exists(p => p.Name == property.Name))
                {
                    throw new ObjectExtistException(string.Format("‘{0}’ 的 Property ‘{1}’ 已经存在。", bean.Name, property.Name));
                }
                bean.Properties.Add(property);
            }
        }

        private void FindMethodNode(ref XmlNode beanNode, ref Bean bean)
        {
            //遍历方法节点
            XmlNodeList methodNodeList = beanNode.SelectNodes("./amuse:method", nsMgr);
            foreach (XmlNode methodNode in methodNodeList)
            {
                Method method = this.ParseMethod(methodNode, bean);
                if (bean.Methods.Count > 0 && bean.Properties.Exists(p => p.Name == method.Name))
                {
                    throw new ObjectExtistException(string.Format("‘{0}’ 的 Method ‘{1}’ 已经存在。", bean.Name, method.Name));
                }
                bean.Methods.Add(method);
            }
        }

        private void FindConstructorNode(ref XmlNode beanNode, ref Bean bean)
        {
            //查找构造节点
            XmlNodeList constructorNodeList = beanNode.SelectNodes("./amuse:constructor", nsMgr);
            if (constructorNodeList == null || constructorNodeList.Count < 1)
            {
                return;
            }
            if (constructorNodeList != null && constructorNodeList.Count > 1)
            {
                throw new ObjectNotUniqueException(string.Format("‘{0}’ 只能存在一个 Constructor 配置节。", bean.Name));
            }
            Constructor constructor = this.ParseConstructor(constructorNodeList[0], bean);
            constructor.Name = bean.Name;
            bean.Constructor = constructor;
        }

        private Property ParseProperty(XmlNode propertyNode, Bean bean)
        {
            var property = new Property(bean);
            foreach (XmlAttribute attr in propertyNode.Attributes)
            {
                property.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            property.Value = propertyNode.InnerText ?? "";
            if (string.IsNullOrWhiteSpace(property.Trim)
                || property.Trim == TrimType.Both)
            {
                property.Value = property.Value.Trim();
            }
            return property;
        }

        private Method ParseMethod(XmlNode methodNode, Bean bean)
        {
            var method = new Method(bean);
            foreach (XmlAttribute attr in methodNode.Attributes)
            {
                method.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            this.FindMethodParameterNode(ref methodNode, ref method);
            return method;
        }

        private Constructor ParseConstructor(XmlNode constructorNode, Bean bean)
        {
            var constructor = new Constructor(bean);
            constructor.Name = "";
            foreach (XmlAttribute attr in constructorNode.Attributes)
            {
                constructor.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            this.FindConstructorParameterNode(ref constructorNode, ref constructor);
            return constructor;
        }

        private void FindMethodParameterNode(ref XmlNode methodNode, ref Method method)
        {
            XmlNodeList parameterNodeList = methodNode.SelectNodes("./amuse:parameter", nsMgr);
            foreach (XmlNode parameterNode in parameterNodeList)
            {
                Parameter parameter = this.ParseParameter(parameterNode, method);
                if (method.Parameters.Count > 0 && method.Parameters.Exists(p => p.Name == parameter.Name))
                {
                    throw new ObjectExtistException(string.Format("‘{0}’ 的 ‘{1}’ 方法的参数 ‘{2}’ 已经存在。", parameter.Method.Bean.Name, parameter.Method.Name, parameter.Name));
                }
                method.Parameters.Add(parameter);
            }
        }

        private void FindConstructorParameterNode(ref XmlNode constructorNode, ref Constructor constructor)
        {
            XmlNodeList parameterNodeList = constructorNode.SelectNodes("./amuse:parameter", nsMgr);
            foreach (XmlNode parameterNode in parameterNodeList)
            {
                Parameter parameter = this.ParseParameter(parameterNode, constructor);
                if (constructor.Parameters.Count > 0 && constructor.Parameters.Exists(p => p.Name == parameter.Name))
                {
                    throw new ObjectExtistException(string.Format("‘{0}’ 的 constructor 方法的参数 ‘{1}’ 已经存在。", parameter.Method.Bean.Name, parameter.Name));
                }
                constructor.Parameters.Add(parameter);
            }
        }

        private Parameter ParseParameter(XmlNode parameterNode, Method method)
        {
            var parameter = new Parameter(method);
            foreach (XmlAttribute attr in parameterNode.Attributes)
            {
                parameter.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            parameter.Value = parameterNode.InnerText ?? "";
            if (string.IsNullOrWhiteSpace(parameter.Trim)
                || parameter.Trim == TrimType.Both)
            {
                parameter.Value = parameter.Value.Trim();
            }
            return parameter;
        }

        private static string ToTitleCase(string text)
        {
            if (text != null && (text.Contains("-") || text.Contains("_")))
            {
                var textParts = text.Replace("_", "|").Replace("-", "|").Split('|');
                var buffer = new StringBuilder();
                foreach (var part in textParts)
                {
                    buffer.Append(ToTitleCase(part));
                }
                return buffer.ToString();
            }
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text);
        }
    }
}
