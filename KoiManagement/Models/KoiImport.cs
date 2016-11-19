using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Models
{
    public class KoiImport
    {
        public KoiImport()
        {
            
        }
        public KoiImport(string Variety, string KoiName, DateTime? DoB, string Gender  ,decimal Size, string Origin, List<string> image)
        {
            this.Variety = Variety;
            this.KoiName = KoiName;
            this.DoB = DoB;
            this.Gender = Gender;
            this.Size = Size;
            this.Origin = Origin;
            this.Image.AddRange(image);
        }
        public string Variety { get; set; }
        public string KoiName { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DoB { get; set; }
        public decimal Size { get; set; }
        public string Origin { get; set; }
        public List<string> Image =new List<string>();
    }
}