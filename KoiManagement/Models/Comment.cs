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
    
    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            this.Comment1 = new HashSet<Comment>();
        }
    
        public int CommentID { get; set; }
        public Nullable<int> MemberID { get; set; }
        public Nullable<int> KoiID { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<int> KoiFarmID { get; set; }
        public decimal Rating { get; set; }
        public string Content { get; set; }
        public Nullable<int> CommentAnswer { get; set; }
        public bool Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment1 { get; set; }
        public virtual Comment Comment2 { get; set; }
        public virtual Koi Koi { get; set; }
        public virtual KoiFarm KoiFarm { get; set; }
        public virtual Member Member { get; set; }
    }
}
