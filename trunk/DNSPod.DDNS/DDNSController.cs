using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net;
using DDNSPod.DNSPod.Api;
using System.Text.RegularExpressions;

namespace DDNSPod.DNSPod.DDNS
{
    public class DDNSController
    {
        public List<DDNSItem> GetList()
        {
            XmlDocument doc = new XmlDocument();
            string file = Path.Combine(Application.StartupPath, "records.xml");

            if (!File.Exists(file))
            {
                return new List<DDNSItem>();
            }

            doc.Load(file);
            string xpath = "//record";
            XmlNodeList nodes = doc.SelectNodes(xpath);

            List<DDNSItem> list = new List<DDNSItem>();

            foreach (XmlNode node in nodes)
            {
                DDNSItem item = new DDNSItem();
                foreach (XmlNode child in node.ChildNodes)
                {
                    Type t = item.GetType();
                    PropertyInfo pi = t.GetProperty(child.Name);
                    if (pi != null)
                    {
                        pi.SetValue(item, child.InnerText, null);
                    }
                }

                list.Add(item);
            }

            return list;
        }

        public void DeleteItem(string id)
        {
            XmlDocument doc = new XmlDocument();
            string file = Path.Combine(Application.StartupPath, "records.xml");

            if (File.Exists(file))
            {
                doc.Load(file);
                string xpath = "//record[record_id=" + id + "]";

                XmlNode record = doc.SelectSingleNode(xpath);
                XmlNode root = doc.SelectSingleNode("//Records");

                root.RemoveChild(record);

                doc.Save(file);
            }
        }

        string GetLatestIp()
        {
            string ip = "1.1.1.1";

            try
            {
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://myip.zhiyusoft.com/");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://iframe.ip138.com/ic.asp");
                request.Credentials = CredentialCache.DefaultCredentials;
                //request.UserAgent = "";
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "ZY DDNS Client/1.0.3 (ysixin@gmail.com)";
                request.Timeout = 1000 * 10;

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                string results = reader.ReadToEnd();

                Regex reg = new Regex(@"\[(.*?)\]", RegexOptions.Multiline);
                Match match = reg.Match(results);
                if (match.Success)
                {
                    ip = match.Groups[1].Value;
                }
            }
            catch (Exception exc)
            {

                string path = Path.Combine(Application.StartupPath, "log");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = "exc_" + DateTime.Now.ToString("yyyy-MM-dd")+".log";
                string fullfile = Path.Combine(path, filename);

                using (StreamWriter w = File.AppendText(fullfile))
                {
                    Logger.Log(exc.Message + "\r\n"+exc.StackTrace, w);
                    // Close the writer and underlying file.
                    w.Close();
                }
            }

            return ip;
        }

        public void UpdateLastIp()
        {

            string path = Path.Combine(Application.StartupPath, "log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filename = "ddns_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string fullfile = Path.Combine(path, filename);

            XmlDocument doc = new XmlDocument();
            string file = Path.Combine(Application.StartupPath, "lastip.xml");

            XmlNode root;
            if (!File.Exists(file))
            {
                root = doc.CreateElement("Dnspod");
                doc.AppendChild(root);
            }
            else
            {
                doc.Load(file);
                root = doc.SelectSingleNode("/Dnspod");
            }

            XmlNode lastipNode = root.SelectSingleNode("lastip");
            if (lastipNode == null)
            {
                lastipNode = doc.CreateElement("lastip");
                lastipNode.InnerText = "1.1.1.1";
                root.AppendChild(lastipNode);
            }


            string lastip = lastipNode.InnerText;
            string lastestip = GetLatestIp();

            string logmessage = "";

            if (lastestip != lastip)
            {
                //update server record and write to log

                List<DDNSItem> list = GetList();
                foreach (DDNSItem item in list)
                {
                    string p = "domain_id=" + item.domain_id + "&record_id=" + item.record_id + "&sub_domain=" + item.sub_domain + "&record_line=" + item.record_line;
                    logmessage += p + "\r\n";
                    RecordDdnsApi api = new RecordDdnsApi();
                    logmessage += "updating " + item.sub_domain + "." + item.domain_name + "...";
                    api.Excute(p);
                    api.ParseXML();

                    if (api.DomainList.Count > 0)
                    {
                        if (api.DomainList[0].value == lastestip)
                        {
                            logmessage += " successfully. ";
                        }
                        else
                        {
                            logmessage += api.GetStatus();
                        }
                    }
                    else
                    {
                        logmessage += api.GetStatus();
                    }

                    logmessage += " [Finished]\r\n";

                }

                //write to lastip.xml
                lastipNode.InnerText = lastestip;
                doc.Save(file);

                using (StreamWriter w = File.AppendText(fullfile))
                {
                    Logger.Log(logmessage, w);
                    // Close the writer and underlying file.
                    w.Close();
                }
            }
        }
    }
}
