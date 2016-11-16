using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using KoiManagement.Common;
using KoiManagement.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace KoiManagement.Hub
{
    public class NotificationsHub : Microsoft.AspNet.SignalR.Hub 
    {

        public void Hello()
        {
            Clients.All.hello();
        }

        public void SendNotification(string userid, string author, string message)
        {
             Clients.All.broadcastNotification(userid, author, message);
        }
        public void JoinGroup(string groupName)
        {

        }
        public override Task OnConnected()
        {
            var us = new UserConnection();
            //var m = AppSession.getSession(SessionAccount.SessionUserId);
            //var m = HttpContext.Current.Session[SessionAccount.SessionUserId];
            us.UserName = SessionAccount.SessionGetUserId;
            us.ConnectionID = Context.ConnectionId;
            Constants._uList.Add(us);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            //for (int i=0; i<Constants._uList.Count;i++)
            //{
            //    if (Constants._uList.ElementAt(i).UserName.Equals(SessionAccount.SessionGetUserName))
            //    {
            //        Constants._uList.RemoveAt(i);
            //    }
            //}
            return base.OnDisconnected(stopCalled);
        }
        private void SignalConnectionId(string signalConnectionId)
        {
            Clients.Client(signalConnectionId).signalConnectionId(signalConnectionId);
        }

    }

    public class UserConnection
    {
  public string UserName { set; get; }
    public string ConnectionID { set; get; }
}




}