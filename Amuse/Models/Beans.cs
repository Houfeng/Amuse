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
                    throw new BeanExtistException(string.Format("名称为‘{0}’的 Bean 已经存在", bean.Name));
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
            XmlNodeList properyNodeList = beanNode.SelectNodes("./amuse:property", nsMgr);
            foreach (XmlNode properyNode in properyNodeList)
            {
                Property property = this.ParseProperty(properyNode);
                if (bean.Properties.Count > 0 && bean.Properties.Exists(p => p.Name == property.Name))
                {
                    throw new BeanExtistException(string.Format("‘{0}’的 Property ‘{1}’已经存在", bean.Name, property.Name));
                }
                bean.Properties.Add(property);
            }
            return bean;
        }
        private Property ParseProperty(XmlNode properyNode)
        {
            var property = new Property();
            foreach (XmlAttribute attr in properyNode.Attributes)
            {
                property.SetPropertyValue(ToTitleCase(attr.Name), attr.Value);
            }
            XmlNode valueNode = properyNode.SelectSingleNode("./amuse:value", nsMgr);
            if (valueNode != null)
            {
                property.Text = valueNode.InnerText;
            }
            return property;
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
