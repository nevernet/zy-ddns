using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDNSPod.DNSPod.PO
{
    public class RecordListItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string line { get; set; }
        public string type { get; set; }
        public string ttl { get; set; }
        public string value { get; set; }
        public string mx { get; set; }
        public string enabled { get; set; }
        public string remark { get; set; }
        public string updated_on { get; set; }
    }
}
