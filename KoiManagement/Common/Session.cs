﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.Common
{
    public static class SessionAccount
    {
           public const string SessionUserId = "userid";
           public const string SessionUserName = "username";
           public const string SessionName = "fullname";
           public const string SessionImage = "Image";

        public static string SessionGetUserId = "";
        /// <summary>
        /// Session screen redirect to other screen
        /// </summary>
        public const string ScreenRedirect = "ScreenRedirect";
       

    }

    public static class SessionInfoDetail
    {
        public static List<Medium> listRemoveImage = new List<Medium>();
    }
}