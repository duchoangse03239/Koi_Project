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
    
    public partial class Article
    {
        public int ArticleID { get; set; }
        public int TypeID { get; set; }
        public string Title { get; set; }
        public string ShortDes { get; set; }
        public string Image { get; set; }
        public System.DateTime Date { get; set; }
        public int MemberID { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Type Type { get; set; }
    }
}
