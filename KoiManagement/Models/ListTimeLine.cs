using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Models
{
    public class ListTimeLine
    {
        public List<InfoDetail> ListInfo{ get; set; }
        public List<Achivement> ListIAchi { get; set; }

        public ListTimeLine()
        {
            ListInfo = new List<InfoDetail>();
            ListIAchi = new List<Achivement>();
        }
        public DateTime date { get; set; }

    }
}