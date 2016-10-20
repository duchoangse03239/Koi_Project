﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Common
{
    public static class SessionAccount
    {
           public const string SessionUserId = "userid";
           public const string SessionUserName = "username";
           public const string SessionName = "fullname";
            /// <summary>
           /// Session screen redirect to other screen
            /// </summary>
           public const string ScreenRedirect = "ScreenRedirect";
    }
}