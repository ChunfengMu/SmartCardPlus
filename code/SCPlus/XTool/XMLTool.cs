using System;
using System.IO;
using System.Xml;
namespace xTool
{
    public class XMLTool
    {
        private XmlDocument doc;
        private string path;

        public XMLTool(string filename)
        {
            this.path = Directory.GetCurrentDirectory() + "\\" + filename;
            doc = new XmlDocument();
        }
        /// <summary>
        /// read node or attribute
        public string Read(string node, string attribute, bool removeSpace = false)
        {
            string value = "";
            if (File.Exists(path))
            {
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (xn == null)
                    return "";
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
                if (removeSpace)
                    return value.Replace(" ", "");
            }
            return value;
        }

        /// <summary>
        /// GET ROOT
        public XmlElement GetXmlDocumentRoot()
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("XML: \"" + path + "\" not founded, Plz check Config.");
            }
            doc.Load(path);
            return doc.DocumentElement;
        }


        public XmlNode CreateElement(string node, string text)
        {
            //doc.Load(path);
            XmlNode xn = (XmlNode)doc.CreateElement(node);
            xn.InnerText = text;

            return xn;
        }

        /// <summary>
        /// update node
        public void Update(string node, string attribute, string value)
        {
            //doc.Load(path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("XML: \"" + path + "\" not founded, Plz check Config.");
            }
            XmlNode xn = doc.SelectSingleNode(node);
            if (xn != null)
            {
                XmlElement xe = (XmlElement)xn;
                if (string.IsNullOrEmpty(attribute))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            else
                throw new FileNotFoundException("\"" + node + "\" not founded, Plz check xml file.");

        }

        public void XmlInsertBefore(XmlNode oldxn, XmlNode newxn, bool remove = true)
        {
            //doc.Load(path);
            XmlNode parent = oldxn.ParentNode;
            if (newxn.InnerText.Equals(oldxn.InnerText))
                return;
            else if (remove)
            {

                if (parent.HasChildNodes)
                {
                    XmlNode temp = parent.FirstChild;
                    while (temp != null)
                    {
                        if (temp.InnerText.Equals(newxn.InnerText, StringComparison.CurrentCultureIgnoreCase))
                        {
                            parent.RemoveChild(temp);
                            break;
                        }
                        temp = temp.NextSibling;
                    }
                }
            }
            parent.InsertBefore(newxn, oldxn);
            doc.Save(path);
        }

        public void XmlInsert(XmlNode newxn)
        {
            //doc.Load(path);
            GetXmlDocumentRoot().AppendChild(newxn);
            doc.Save(path);
        }

        public void RemoveLastNode(XmlNode root, int max)
        {
            int count = root.ChildNodes.Count;
            if (count > max)
                root.RemoveChild(root.LastChild);
            doc.Save(path);
        }

        public bool RemoveChildNode(XmlNode childnode)
        {
            try
            {
                XmlNode parentnode = childnode.ParentNode;
                parentnode.RemoveChild(childnode);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool RemoveChildAllNode(XmlNode xmlnode)
        {
            xmlnode.RemoveAll();
            if (xmlnode.ChildNodes.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
