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
    }
}
