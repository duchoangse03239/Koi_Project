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
            return db.Messages.Where(p =>p.MemberID==memberID ).ToList();
        }

    }
}