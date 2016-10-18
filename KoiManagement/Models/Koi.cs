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
        public Koi()
        {
            this.Achivements = new HashSet<Achivement>();
            this.Comments = new HashSet<Comment>();
            this.InfoDetails = new HashSet<InfoDetail>();
            this.KoiParents = new HashSet<KoiParent>();
            this.OffSpringOfs = new HashSet<OffSpringOf>();
            this.Owners = new HashSet<Owner>();
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
        public bool Status { get; set; }
        public string Privacy { get; set; }
        public bool Report { get; set; }
    
        public virtual ICollection<Achivement> Achivements { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<InfoDetail> InfoDetails { get; set; }
        public virtual Variety Variety { get; set; }
        public virtual ICollection<KoiParent> KoiParents { get; set; }
        public virtual ICollection<OffSpringOf> OffSpringOfs { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
    }
}