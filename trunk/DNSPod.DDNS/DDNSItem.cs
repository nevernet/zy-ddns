using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDNSPod.DNSPod.DDNS
{
    public class DDNSItem
    {
        public string domain_id { get; set; }
        public string record_id { get; set; }
        public string domain_name { get; set; }
        public string sub_domain { get; set; }        
        public string record_line { get; set; }
    }
}
