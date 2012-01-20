using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDNSPod.DNSPod.Api;
using DDNSPod.DNSPod.DDNS;

namespace DDNSPod
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetDomainList();
        }

        void GetDomainList()
        {
            DomainListApi api = new DomainListApi();

            string p = "type=all&offset=0&length=100";
            api.Excute(p);
            api.ParseXML();

            gvList.DataSource = api.DomainList;

            txtStatus.Text = api.GetStatus();

        }

        private void gvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = gvList.Rows[e.RowIndex];
            string id = row.Cells[0].Value.ToString();
            string name = row.Cells[1].Value.ToString();

            //MessageBox.Show(id);

            Domain dform = new Domain();
            dform.Show();
            dform.GetRecordList(id);

            dform.SetDomain(name, id);

        }

        private void dDNSItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DDNS f = new DDNS();
            f.Show();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUS f = new AboutUS();
            f.Show();
        }

        private void tmUpdateIP_Tick(object sender, EventArgs e)
        {
            DDNSController ctl = new DDNSController();
            ctl.UpdateLastIp();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                Hide();
            }
        }
    }
}
