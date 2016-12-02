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

        public int EditArticle(Article article)
        {
            try
            {
                db.Articles.Attach(article);
                var entry = db.Entry(article);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.MemberID).IsModified = false;
                return db.SaveChanges();
            }
            catch
            {
                return 0;
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