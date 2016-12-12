using System;
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
            return db.Comments.Count(p => p.KoiID == koiID && p.CommentAnswer == null);
        }

        public int CountCommentByKoiFarmID(int koifarmID)
        {
            return db.Comments.Count(p => p.KoiFarmID == koifarmID && p.CommentAnswer == null);
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

        public bool CheckRatingKoi(int Memberid,int koiId)
        {
            try
            {
                var t = db.Comments.Count(p => p.MemberID == Memberid && p.KoiID == koiId);
                if (t > 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckRatingKoiFarm(int Memberid, int koifarmId)
        {
            try
            {
                var t = db.Comments.Count(p => p.MemberID == Memberid && p.KoiFarmID == koifarmId);
                if (t > 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool editComment(int commentID, string content)
        {
            try
            {
                var comt = db.Comments.Find(commentID);
                comt.Content = content;
                db.Comments.Attach(comt);
                db.Entry(comt).Property(x => x.Content).IsModified = true;
                int result = db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Comment GetCommentbyId(int CommentID)
        {
            return db.Comments.Find(CommentID);
        }

        public List<Comment> GetListCommentKoi(int KoiId)
        {
            return db.Comments.Where(p=>p.KoiID== KoiId&&p.Status && p.CommentAnswer == null).ToList();
        }

        public decimal getRate(int koiId)
        {
            decimal? rate = 0;
            var t = db.Comments.Where(p => p.KoiID == koiId&&p.CommentAnswer==null).ToList();
            if (t.Count > 0) { 
            foreach (var item in t)
            {
                if (item.Rating.HasValue)
                {
                    rate += item.Rating;
                }
            }
                var total = db.Comments.Count(p => p.KoiID == koiId && p.CommentAnswer == null);
            int t1= (int) Math.Round((decimal) (rate/ total));
            return t1;
            }
            return 0;
        }

        public decimal getRateKoiFarm(int koiFArmId)
        {
            decimal? rate = 0;
            var t = db.Comments.Where(p => p.KoiFarmID == koiFArmId && p.CommentAnswer == null).ToList();
            if (t.Count > 0)
            {
                foreach (var item in t)
                {
                    if (item.Rating.HasValue)
                    {
                        rate += item.Rating;
                    }
                }
                var total = db.Comments.Count(p => p.KoiID == koiFArmId && p.CommentAnswer == null);
                int t1 = (int)Math.Round((decimal)(rate / total));
                return t1;
            }
            return 0;
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
