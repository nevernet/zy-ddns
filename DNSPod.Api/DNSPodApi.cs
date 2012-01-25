using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace DDNSPod.DNSPod.Api
{
    public abstract class DNSPodApi :IDNSPodAPI
    {
        const string APIBASEDOMAIN = "https://dnsapi.cn/";

        protected string returnString = "<dnspod></dnspod>";

        /// <summary>
        /// API method , example: Domain.List
        /// </summary>
        /// <returns></returns>
        public abstract string GetMethod();
        public abstract string ParseXML();
        public abstract string ParseJson();


        public string GetStatus()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(returnString);

            XmlNode node = doc.SelectSingleNode("//dnspod/status");
            if (node == null) return "";
            return node.InnerXml;

        }
         

        string GeneratePostParam()
        {
            XmlDocument doc = new XmlDocument();
            string file = Path.Combine(Application.StartupPath, "config.xml");

            doc.Load(file);

            XmlNodeList nodelist = doc.SelectNodes("//Config/*");

            string p = "";

            foreach (XmlNode node in nodelist)
            {
                p += "&" + node.Name + "=" + node.InnerText;
            }

            p = p.TrimStart(new char[] { '&' });
            return p;
        }

        public string Excute(string p)
        {
            try
            {
                string newparam = GeneratePostParam() + "&" + p;
                byte[] byteArray = Encoding.UTF8.GetBytes(newparam);

                string url = APIBASEDOMAIN + GetMethod();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                //request.UserAgent = "";
                request.Method = "POST";
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "ZY DDNS Client/1.0.0 (ysixin@gmail.com)";
                
                if (request.Method == "POST")
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                returnString = reader.ReadToEnd();

                return returnString;
            }
            catch (Exception exc)
            {
                //throw;
                return "";
            }
            
        }
    }
}
