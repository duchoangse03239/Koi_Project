using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class MediaDAO
    {
        KoiManagementEntities db = null;
        public MediaDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool addMedia(Medium  medi)
        {
            try
            {
                db.Media.Add(medi);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
