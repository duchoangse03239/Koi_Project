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
        public string Username { get; set; }
        public string VarietyId { get; set; }
        public string SizeFrom { get; set; }
        public string SizeTo { get; set; }
        public string Gender { get; set; }
        public string Owner { get; set; }
        public string AgeFrom { get; set; }
        public string AgeTo { get; set; }
        public KoiFilterModel() { }

        public KoiFilterModel(string orderby ,string koiName, string username, string varietyId, string sizeFrom, string sizeTo,string gender,string owner, string AgeFrom, string AgeTo)
        {
            this.orderby= orderby;
            this.KoiName = koiName;
            this.Username = username;
            this.VarietyId = varietyId;
            this.SizeFrom = sizeFrom;
            this.SizeTo = sizeTo;
            this.Gender = gender;
            this.Owner = owner;
            this.AgeFrom = AgeFrom;
            this.AgeTo = AgeTo;
        }
    }
}