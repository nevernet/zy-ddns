using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDNSPod.DNSPod.Api;
using System.IO;
using System.Xml;

namespace DDNSPod
{
    public partial class Domain : Form
    {
        public Domain()
        {
            InitializeComponent();
        }

        public void GetRecordList(string domainId)
        {
            RecordListApi api = new RecordListApi();

            string p = "domain_id="+domainId+"&offset=0&length=100";
            api.Excute(p);
            api.ParseXML();

            gvList.DataSource = api.DomainList;


            textBox1.Text = api.GetStatus();

        }

        public void SetDomain(string domain, string id)
        {
            lblDomain.Text = domain;
            lblId.Text = id;
        }

        private void gvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = gvList.Rows[e.RowIndex];
            string id = row.Cells[0].Value.ToString();
            string name = row.Cells[1].Value.ToString();
            string line = row.Cells[2].Value.ToString();

            string domain_id = lblId.Text;

            if (MessageBox.Show("Are you add "+name+" to DDNS?", "Add DDNS", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                //Save
                XmlDocument doc = new XmlDocument();
                
                string file = Path.Combine(Application.StartupPath, "records.xml");
                XmlNode root;
                if (!File.Exists(file))
                {

                    root = doc.CreateElement("Records");
                    doc.AppendChild(root);

                    //doc.Save(file);
                }
                else
                {
                    doc.Load(file);
                    root = doc.SelectSingleNode("//Records");
                }

                string path = "//record[record_id=" + id + "]";

                XmlNode record = doc.SelectSingleNode(path);
                if (record == null)
                {
                    record = doc.CreateElement("record");
                    root.AppendChild(record);
                }

                XmlNode domain_idnode = doc.SelectSingleNode(path + "/domain_id");
                if (domain_idnode == null)
                {
                    domain_idnode = doc.CreateElement("domain_id");
                    record.AppendChild(domain_idnode);
                }
                domain_idnode.InnerText = domain_id;

                XmlNode domain_namenode = doc.SelectSingleNode(path + "/domain_name");
                if (domain_namenode == null)
                {
                    domain_namenode = doc.CreateElement("domain_name");
                    record.AppendChild(domain_namenode);
                }
                domain_namenode.InnerText = lblDomain.Text;

                XmlNode record_idNode = doc.SelectSingleNode(path + "/record_id");
                if (record_idNode == null)
                {
                    record_idNode = doc.CreateElement("record_id");
                    record.AppendChild(record_idNode);
                }
                record_idNode.InnerText = id;

                XmlNode sub_domainnode = doc.SelectSingleNode(path + "/sub_domain");
                if (sub_domainnode == null)
                {
                    sub_domainnode = doc.CreateElement("sub_domain");
                    record.AppendChild(sub_domainnode);
                }
                sub_domainnode.InnerText = name;

                XmlNode record_linenode = doc.SelectSingleNode(path + "/record_line");
                if (record_linenode == null)
                {
                    record_linenode = doc.CreateElement("record_line");
                    
                    record.AppendChild(record_linenode);
                }
                XmlCDataSection cdata = doc.CreateCDataSection(line);
                record_linenode.AppendChild(cdata);

                doc.Save(file);

                MessageBox.Show("The record has been added successfully.");
            }
        }
    }
}
