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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InfoDetail()
        {
            this.Media = new HashSet<Medium>();
        }


        public InfoDetail(int KoiID, DateTime Date, decimal Weight, decimal Size, string HeathyStatus, string Description, string Image, bool staus)
        {
            this.Media = new HashSet<Medium>();
            this.KoiID = KoiID;
            this.Date = Date;
            this.Weight = Weight;
            this.Size = Size;
            this.HeathyStatus = HeathyStatus;
            this.Description = Description;
            this.Image = Image;
            this.Status = staus;
        }

        public int DetailID { get; set; }
        public Nullable<int> KoiID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Size { get; set; }
        public string Image { get; set; }
        public string HeathyStatus { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    
        public virtual Koi Koi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Medium> Media { get; set; }
    }
}
