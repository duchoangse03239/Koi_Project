//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KoiManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public int NoID { get; set; }
        public Nullable<int> MemberID { get; set; }
        public Nullable<int> SenderID { get; set; }
        public Nullable<int> ModelID { get; set; }
        public Nullable<System.DateTime> Datetime { get; set; }
        public string url { get; set; }
        public string Content { get; set; }
        public bool isRead { get; set; }
        public bool status { get; set; }
        public Notification()
        {
        }
        public Notification(int MemberID, int SenderID, int ModelID, DateTime Datetime, string Content, bool isRead, bool Status)
        {
            this.MemberID = MemberID;
            this.SenderID = SenderID;
            this.ModelID = ModelID;
            this.Datetime = Datetime;
            this.Content = Content;
            this.isRead = isRead;
            this.status = Status;
        }
        public virtual Member Member { get; set; }
        public virtual Member Member1 { get; set; }
    }
}