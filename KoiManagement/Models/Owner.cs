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
    
    public partial class Owner
    {
        public int OwnerID { get; set; }
        public int MemberID { get; set; }
        public int KoiID { get; set; }
        public Nullable<int> KoiFarmID { get; set; }
        public System.DateTime DateOwerFrom { get; set; }
        public Nullable<System.DateTime> DateOwerTo { get; set; }
        public bool Status { get; set; }
        public Owner() { }
        public Owner(int MemberID, int KoiID, int? KoiFarmID, DateTime DateOwerFrom, DateTime? DateOwerTo, bool Status)
        {
            this.MemberID = MemberID;
            this.KoiID = KoiID;
            this.KoiFarmID = KoiFarmID;
            this.DateOwerFrom = DateOwerFrom;
            this.DateOwerTo = DateOwerTo;
            this.Status = Status;
        }
        public virtual Koi Koi { get; set; }
        public virtual KoiFarm KoiFarm { get; set; }
        public virtual Member Member { get; set; }
    }
}
