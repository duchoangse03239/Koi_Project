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
    
    public partial class Variety
    {
        public Variety()
        {
            this.Kois = new HashSet<Koi>();
            this.Variety1 = new HashSet<Variety>();
        }
    
        public int VarietyID { get; set; }
        public string VarietyName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<int> VarietyDetailID { get; set; }
    
        public virtual ICollection<Koi> Kois { get; set; }
        public virtual ICollection<Variety> Variety1 { get; set; }
        public virtual Variety Variety2 { get; set; }
    }
}