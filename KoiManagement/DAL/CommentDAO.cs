using System.Collections.Generic;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class CommentDAO
    {
        KoiManagementEntities db = null;
        public CommentDAO()
        {
            db = new KoiManagementEntities();
        }

        public int CountCommentByKoiID(int koiID)
        {
            return  db.Kois.Find(koiID).Comments.Count();
        }

        public bool addComment(Comment com)
        {
            try
            {
                db.Comments.Add(com);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Comment> GetListCommentKoi(int KoiId)
        {
            return db.Comments.Where(p=>p.KoiID== KoiId&&p.Status && p.CommentAnswer == null).ToList();
        }
        public List<List<Comment>> GetListCommentKoiDetail(int KoiId)
        {
            List<List<Comment>> ListComment = new List<List<Comment>>();
            var t= db.Comments.Where(p => p.KoiID == KoiId && p.Status&&p.CommentAnswer==null).ToList();
            foreach (var i in t)
            {
                var b = db.Comments.Where(p => p.KoiID == KoiId&&p.CommentAnswer==i.CommentID && p.Status).ToList();
                ListComment.Add(b);
            }
            return ListComment;
        }
    }
}
