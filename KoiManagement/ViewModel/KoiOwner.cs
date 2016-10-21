using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.ViewModel
{
    public class KoiOwner
    {
        public Koi Koi { get; set; }
        public string OwnerName { get; set; }
    }
}