using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDNSPod.DNSPod.DDNS;

namespace DDNSPod
{
    public partial class DDNS : Form
    {
        public DDNS()
        {
            InitializeComponent();
            GetList();
        }

        void GetList()
        {
            DDNSController ctl = new DDNSController();
            gvList.DataSource = ctl.GetList();
        }

        private void gvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = gvList.Rows[e.RowIndex];
            string id = row.Cells[1].Value.ToString();

            string record = row.Cells[3].Value.ToString() + "." +row.Cells[2].Value.ToString();

            if (MessageBox.Show("Are you sure you want to delete <" + record + "> from DDNS?", "Delete Record", MessageBoxButtons.YesNo)
                == System.Windows.Forms.DialogResult.Yes)
            {
                DDNSController ctl = new DDNSController();
                ctl.DeleteItem(id);
                GetList();
            }
        }
    }
}
