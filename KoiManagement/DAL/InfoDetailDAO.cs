using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class InfoDetailDAO
    {
        KoiManagementEntities db = null;
        public InfoDetailDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool AddInfoDetail(InfoDetail info, List<Medium> media)
        {
            // Transaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.InfoDetails.Add(info);
                    db.SaveChanges();
                    var DetailID = info.DetailID;
                    foreach (var item in media)
                    {
                        item.ModelId = DetailID;
                        db.Media.Add(item);
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                     return false;
                }
                return true;
            }
        }

        public List<InfoDetail> GetlistInfoDetails(int koiId)
        {
            return db.InfoDetails.Where(p => p.KoiID == koiId&&p.Status).ToList();
        }
        public int GetMaxDetailID()
        {
            return db.InfoDetails.Max(g => g.DetailID) + 1;
        }

        public InfoDetail GetDetailByid(int id)
        {
            return db.InfoDetails.Find(id);
        }
        public string GetLastSize(int KoiID)
        {
            var koiDeatail = db.InfoDetails.Where(p => p.KoiID == KoiID&&p.Status).OrderBy(p => p.Date).FirstOrDefault();
            if (koiDeatail == null)
            {
                return string.Empty;
            }
            return koiDeatail.Size.ToString();
        }

        public bool UpdateDetail(InfoDetail infoDetail, List<Medium> ListMediaRemove, List<Medium> ListMediaAdd, string avatar)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {


                    //set default value
                    db.InfoDetails.Attach(infoDetail);
                    var entry = db.Entry(infoDetail);
                    entry.State = EntityState.Modified;
                    // Set column not change
                    entry.Property(e => e.KoiID).IsModified = false;
                    db.SaveChanges();
                    var DetailID = infoDetail.DetailID;
                    foreach (var image in ListMediaRemove)
                    {
                        image.Status = false;
                        db.Media.Attach(image);
                        var entry1 = db.Entry(image);
                        entry1.State = EntityState.Modified;
                        // Set column not change
                        entry1.Property(e => e.Status).IsModified = true;
                        db.SaveChanges();
                    }
                    foreach (var item in ListMediaAdd)
                    {
                        item.ModelId = DetailID;
                        db.Media.Add(item);
                        db.SaveChanges();
                    }
                    if (!avatar.Equals("change") && !infoDetail.Image.Equals(avatar))
                    {
                        var listImage = db.Media.FirstOrDefault(p => p.ModelId == infoDetail.DetailID && p.LinkImage == avatar);
                        var oldImage = infoDetail.Image;
                        if (listImage != null)
                        {
                            infoDetail.Image = listImage.LinkImage;

                            listImage.LinkImage = oldImage;
                            db.Media.Attach(listImage);
                            var entry3 = db.Entry(listImage);
                            entry3.State = EntityState.Modified;
                            // Set column not change
                            entry3.Property(e => e.LinkImage).IsModified = true;
                            db.SaveChanges();
                        }


                        //set default value
                        db.InfoDetails.Attach(infoDetail);
                        var entry2 = db.Entry(infoDetail);
                        entry2.State = EntityState.Modified;
                        // Set column not change
                        entry2.Property(e => e.KoiID).IsModified = false;
                        db.SaveChanges();
                    }

                    dbContextTransaction.Commit();

                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    return false;
                }
            }
            return true;
        }
    }
}
