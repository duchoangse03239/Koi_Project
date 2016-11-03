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
    
    public partial class Koi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Koi()
        {
            this.Achievements = new HashSet<Achievement>();
            this.Comments = new HashSet<Comment>();
            this.InfoDetails = new HashSet<InfoDetail>();
            this.KoiParents = new HashSet<KoiParent>();
            this.OffSpringOfs = new HashSet<OffSpringOf>();
            this.Owners = new HashSet<Owner>();
            this.Reports = new HashSet<Report>();
        }

        public Koi(int VarietyID, string KoiName, DateTime? DoB, string Gender, string Temperament, string Certificate, string Image, string Origin, bool Status, bool Privacy)
        {
            this.Achievements = new HashSet<Achievement>();
            this.Comments = new HashSet<Comment>();
            this.InfoDetails = new HashSet<InfoDetail>();
            this.KoiParents = new HashSet<KoiParent>();
            this.OffSpringOfs = new HashSet<OffSpringOf>();
            this.Owners = new HashSet<Owner>();
            this.Reports = new HashSet<Report>();
            this.VarietyID = VarietyID;
            this.KoiName = KoiName;
            this.DoB = DoB;
            this.Gender = Gender;
            this.Temperament = Temperament;
            this.Certificate = Certificate;
            this.Image = Image;
            this.Origin = Origin;
            this.Status = Status;
            this.Privacy = Privacy;
        }

        public int KoiID { get; set; }
        public int VarietyID { get; set; }
        public string KoiName { get; set; }
        public Nullable<System.DateTime> DoB { get; set; }
        public string Gender { get; set; }
        public string Temperament { get; set; }
        public string Certificate { get; set; }
        public string Image { get; set; }
        public string Origin { get; set; }
        public Nullable<bool> IsDead { get; set; }
        public string DeadReason { get; set; }
        public bool Status { get; set; }
        public Nullable<bool> Privacy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Achievement> Achievements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InfoDetail> InfoDetails { get; set; }
        public virtual Variety Variety { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KoiParent> KoiParents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OffSpringOf> OffSpringOfs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Owner> Owners { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports { get; set; }
    }
}
