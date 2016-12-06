using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Models
{
    public class UserComment
    {


        public int MemberID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        public string Content { get; set; }
        public UserComment()
        {

        }
        public UserComment(int MemberID, string Name, string Image, DateTime Date, decimal Rate, string Content)
        {
            this.MemberID = MemberID;
            this.Name = Name;
            this.Image = Image;
            this.Date = Date;
            this.Rate = Rate;
            this.Content = Content;
        }

    }
}