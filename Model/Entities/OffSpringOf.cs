//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class OffSpringOf
    {
        public OffSpringOf()
        {
            this.KoiParents = new HashSet<KoiParent>();
        }
    
        public int OffspringID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public Nullable<int> KoiID { get; set; }
    
        public virtual Koi Koi { get; set; }
        public virtual ICollection<KoiParent> KoiParents { get; set; }
    }
}