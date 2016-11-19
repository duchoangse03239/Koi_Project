using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class ArticleDAO
    {
        KoiManagementEntities db = null;
         public ArticleDAO()
        {
            db = new KoiManagementEntities();
        }
        public bool AddArticle(Article Article)
        {
            try
            {
                db.Articles.Add(Article);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Article> GetListArticle(int id)
        {
            return db.Articles.Where(p => p.TypeID == id).ToList();
        }
    }
}