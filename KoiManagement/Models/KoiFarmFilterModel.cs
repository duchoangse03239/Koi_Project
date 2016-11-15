using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Models
{
    public class KoiFarmFilterModel
    {
        public string orderby { get; set; }
        public string farmname { get; set; }
        public string owner { get; set; }
        public string address { get; set; }

        public KoiFarmFilterModel() { }

        public KoiFarmFilterModel(string orderby, string farmname, string owner, string address)
        {
            this.orderby = orderby;
            this.farmname = farmname;
            this.owner = owner;
            this.address = address;
        }
    }
}