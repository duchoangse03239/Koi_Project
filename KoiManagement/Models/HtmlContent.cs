using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Models
{
    public class HtmlContent
    {
        // This attributes allows your HTML Content to be sent up  
        [AllowHtml]
        public string Content { get; set; }

        public HtmlContent()
        {

        }
    }
}