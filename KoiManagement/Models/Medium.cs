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
    
    public partial class Medium
    {
        public int MediaID { get; set; }
        public string LinkImage { get; set; }
        public string LinkVideo { get; set; }
        public int ModelId { get; set; }
        public bool Status { get; set; }
        public Medium() { }
        public Medium(string LinkImage, string LinkVideo, int ModelId, bool Status)
        {
            this.LinkImage = LinkImage;
            this.LinkVideo = LinkVideo;
            this.ModelId = ModelId;
            this.Status = Status;
        }
        public virtual InfoDetail InfoDetail { get; set; }
    }
}
