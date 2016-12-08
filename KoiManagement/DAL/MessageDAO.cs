using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    /// <summary>
    /// Handle with database
    /// </summary>
    public class MessageDAO
    {
        KoiManagementEntities db;
         public MessageDAO()
        {
            db = new KoiManagementEntities();
        }

        /// <summary>
        /// Handle to add article
        /// </summary>
         /// <param name="article">Article</param>
        /// <returns>Bool</returns>
        public bool AddMessage(Message article)
        {
            try
            {
                db.Messages.Add(article);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Message> GetListMessage(int memberID)
        {
            return db.Messages.Where(p =>(p.MemberID==memberID||p.SenderID==memberID) &&p.ReplyID==null).ToList();
        }
        public List<List<Message>> GetListMessageDetail(int memberid)
        {
            List<List<Message>> ListComment = new List<List<Message>>();
            var comment = db.Messages.Where(p => p.MemberID == memberid && p.Status && p.ReplyID == null).ToList();
            foreach (var i in comment)
            {
                var b = db.Messages.Where(p =>  p.ReplyID == i.MessageID && p.Status).ToList();
                ListComment.Add(b);
            }
            return ListComment;
        }

        public List<Message> GetListMeDetail(int messageID)
        {
            return db.Messages.Where(p => p.ReplyID == messageID).OrderBy(p=>p.Datetime).ToList();
        }
        public Message getOwner(int messageID)
        {
            var m= db.Messages.Find(messageID);
            return m;
        }

    }
}