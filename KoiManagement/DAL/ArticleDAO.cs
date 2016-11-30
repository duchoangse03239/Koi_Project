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

        public int GetMaxAchiID()
        {
            var count = db.Articles.Count();

            if (count == 0)
            {
                return count = 1;
            }
            return db.Articles.Max(g => g.ArticleID) + 1; ;
        }

        public Article GetArticleDetail(int id)
        {
            return db.Articles.Find(id);
        }
    }
}