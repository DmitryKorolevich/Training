using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace VitalChoice.Business.Services.Bronto
{
    public static class XmlSerializableServices
    {
        internal static readonly string ReadNodesMethodName = "ReadNodes";
        public static XmlNode[] ReadNodes(XmlReader xmlReader)
        {
            if (xmlReader == null)
                throw new Exception();
            XmlDocument doc = new XmlDocument();
            List<XmlNode> nodeList = new List<XmlNode>();
            if (xmlReader.MoveToFirstAttribute())
            {
                do
                {
                    if (IsValidAttribute(xmlReader))
                    {
                        XmlNode node = doc.ReadNode(xmlReader);
                        if (node == null)
                            throw new Exception();
                        nodeList.Add(node);
                    }
                } while (xmlReader.MoveToNextAttribute());
            }
            xmlReader.MoveToElement();
            if (!xmlReader.IsEmptyElement)
            {
                int startDepth = xmlReader.Depth;
                xmlReader.Read();
                while (xmlReader.Depth > startDepth && xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    XmlNode node = doc.ReadNode(xmlReader);
                    if (node == null)
                        throw new Exception();
                    nodeList.Add(node);
                }
            }
            return nodeList.ToArray();
        }

        private static bool IsValidAttribute(XmlReader xmlReader)
        {
            return xmlReader.NamespaceURI != "http://schemas.microsoft.com/2003/10/Serialization/" &&
                                   xmlReader.NamespaceURI != "http://www.w3.org/2001/XMLSchema-instance" &&
                                   xmlReader.Prefix != "xmlns" &&
                                   xmlReader.LocalName != "xmlns";
        }

        internal static string WriteNodesMethodName = "WriteNodes";
        public static void WriteNodes(XmlWriter xmlWriter, XmlNode[] nodes)
        {
            if (xmlWriter == null)
                throw new Exception();
            if (nodes != null)
                for (int i = 0; i < nodes.Length; i++)
                    if (nodes[i] != null)
                        nodes[i].WriteTo(xmlWriter);
        }
    }
}
