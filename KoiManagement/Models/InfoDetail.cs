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
    
    public partial class InfoDetail
    {
        public int DetailID { get; set; }
        public Nullable<int> KoiID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Size { get; set; }
        public string HeathyStatus { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public InfoDetail(){}

        public InfoDetail(int KoiID, DateTime Date, decimal Weight,decimal Size,string HeathyStatus, string Description,string Image)
        {
            this.KoiID = KoiID;
            this.Date = Date;
            this.Weight = Weight;
            this.Size = Size;
            this.HeathyStatus = HeathyStatus;
            this.Description = Description;
            this.Image = Image;
        }
        public virtual Koi Koi { get; set; }
    }
}
