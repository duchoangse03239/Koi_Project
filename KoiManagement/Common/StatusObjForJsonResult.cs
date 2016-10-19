using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Common
{
    public class StatusObjForJsonResult
    {
        public int Status;
        public String Message;
        public String RedirectTo;
        public String PartName;
        public Object JsonObject;
        public List<String> ResultList;
    }
}