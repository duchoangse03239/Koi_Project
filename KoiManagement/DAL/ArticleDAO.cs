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
    public class ArticleDAO
    {
        KoiManagementEntities db;
         public ArticleDAO()
        {
            db = new KoiManagementEntities();
        }

        /// <summary>
        /// Handle to add article
        /// </summary>
         /// <param name="article">Article</param>
        /// <returns>Bool</returns>
        public bool AddArticle(Article article)
        {
            try
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Handle to edit article
        /// </summary>
        /// <param name="article">article</param>
        /// <returns>int</returns>
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

        /// <summary>
        /// Handle to get list of article
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Article> GetListArticle(int id)
        {
            return db.Articles.Where(p => p.TypeID == id&&p.Status).ToList();
        }

        /// <summary>
        /// Get max index of achievement id
        /// </summary>
        /// <returns></returns>
        public int GetMaxAchiId()
        {
            var count = db.Articles.Count();

            if (count == 0)
            {
                return count = 1;
            }
            return db.Articles.Max(g => g.ArticleID) + 1; ;
        }

        /// <summary>
        /// Handle to get list of article
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Article GetArticleDetail(int id)
        {
            return db.Articles.Find(id);
        }
    }
}