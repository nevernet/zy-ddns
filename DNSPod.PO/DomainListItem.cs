using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDNSPod.DNSPod.PO
{
    public class DomainListItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string grade { get; set; }
        public string status { get; set; }
        public string ext_status { get; set; }
        public string records { get; set; }
        public string group_id { get; set; }
        public string is_mark { get; set; }
        public string remark { get; set; }
        public string is_vip { get; set; }
        public string updated_on { get; set; }
    }
}
