using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DDNSPod.DNSPod.PO;
using System.Reflection;

namespace DDNSPod.DNSPod.Api
{
    public class RecordDdnsApi: DNSPodApi
    {

        public List<RecordDdnsItem> DomainList { get; set; }

        public override string GetMethod()
        {
            return "Record.Ddns";
        }

        public override string ParseXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(returnString);

            XmlNodeList nodes = doc.SelectNodes("//record");

            DomainList = new List<RecordDdnsItem>();

            foreach (XmlNode node in nodes)
            {
                RecordDdnsItem dlitem = new RecordDdnsItem();

                foreach (XmlNode child in node.ChildNodes)
                {
                    string name = child.Name;

                    Type t = dlitem.GetType();
                    PropertyInfo pi = t.GetProperty(name);
                    if (pi != null)
                    {
                        pi.SetValue(dlitem, child.InnerText, null);
                    }
                }

                DomainList.Add(dlitem);
            }

            return "";
        }

        public override string ParseJson()
        {
            return "";
        }
    }
}
