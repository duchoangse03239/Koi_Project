using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Models
{
    public class KoiFilterModel
    {
        public string orderby { get; set; }
        public string KoiName { get; set; }
        public string VarietyId { get; set; }
        public string SizeFrom { get; set; }
        public string SizeTo { get; set; }
        public string Gender { get; set; }
        public string Owner { get; set; }
        public KoiFilterModel() { }

        public KoiFilterModel(string orderby ,string koiName, string varietyId, string sizeFrom, string sizeTo,string gender,String owner)
        {
            this.orderby= orderby;
            this.KoiName = koiName;
            this.VarietyId = varietyId;
            this.SizeFrom = sizeFrom;
            this.SizeTo = sizeTo;
            this.Gender = gender;
            this.Owner = owner;
        }
    }
}