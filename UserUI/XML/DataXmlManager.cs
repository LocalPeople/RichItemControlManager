using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using UserUI.Data;

namespace UserUI.XML
{
    class DataXmlManager
    {
        string savePath;
        XmlDocument writer;
        XElement reader;

        public DataXmlManager(Section rootSection, string directory)
        {
            writer = new XmlDocument();
            XmlNode nodeTop = writer.CreateElement("Root");
            writer.AppendChild(nodeTop);
            InitXmlDocument(rootSection, nodeTop, false);
            savePath = Path.Combine(directory, "项目配置.xml");
            writer.Save(savePath);
            reader = XElement.Load(savePath);
        }

        public DataXmlManager(string directory)
        {
            writer = new XmlDocument();
            savePath = Path.Combine(directory, "项目配置.xml");
            writer.Load(savePath);
            reader = XElement.Load(savePath);
        }

        public static bool Exist(string directory)
        {
            return File.Exists(Path.Combine(directory, "项目配置.xml"));
        }

        private void InitXmlDocument(Section section, XmlNode node, bool isLeapGroup)
        {
            if (!isLeapGroup)
            {
                foreach (Section childSection in section.Group)
                {
                    XmlElement child = writer.CreateElement("Branch");
                    child.SetAttribute("Name", childSection.Name);
                    node.AppendChild(child);
                    InitXmlDocument(childSection, child, childSection.Group[0].IsLeap);
                }
            }
            else
            {
                XmlElement leap = writer.CreateElement("Leap");
                leap.SetAttribute("Name", section.Group[0].Name);
                node.AppendChild(leap);
            }
        }

        public string GetLeapName(Section section)
        {
            string[] relation = section.GetRelation();
            XElement target = reader;
            foreach (string next in relation)
            {
                target = target.Elements().First(elem => elem.Attribute("Name").Value == next);
            }
            return target.Elements().First().Attribute("Name").Value;
        }

        public void SetLeapName(Section section, string name)
        {
            string[] relation = section.GetRelation();
            XmlNode elem = writer.SelectSingleNode("Root");
            foreach (string next in relation)
            {
                foreach (XmlNode child in elem.ChildNodes)
                {
                    if (child.Attributes["Name"].Value == next)
                    {
                        elem = child;
                        break;
                    }
                }
            }
            elem.FirstChild.Attributes["Name"].Value = name;
            writer.Save(savePath);
        }
    }
}
