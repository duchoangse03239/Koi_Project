using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KoiManagement.Common;
using KoiManagement.Models;
using EntityFramework.BulkInsert.Extensions;

namespace KoiManagement.DAL
{
    public class KoiDAO
    {
        KoiManagementEntities db = null;
        public KoiDAO()
        {
            db = new KoiManagementEntities();
        }

        /// <summary>
        /// Lấy id lớn nhất +1 của koi id
        /// </summary>
        /// <returns></returns>
        public int GetMaxKoiID()
        {
            return  db.Kois.Max(g => g.KoiID) + 1;
        }

        /// <summary>
        /// Lấy danh sách cá koi theo memberid
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        public List<Koi> GetListKoiByMember(int memberID)
        {
             var Owner = db.Owners.Where(p => p.MemberID == memberID&&p.Status).ToList();
            if (Owner.Count > 0)
            {
                var ListKois = new List<Koi>();

                foreach (var item1 in Owner)
                {
                    var kois = db.Kois.Where(p => p.KoiID == item1.KoiID&&p.Status==1).ToList();

                    if (kois.Count > 0)
                    {
                        ListKois.Add(kois.ElementAt(0));
                    }
                }

           return ListKois;
            }
            return null;
        }
        /// <summary>
        /// lấy số lượng cá koi theo Member id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountKoibyOwnerId(int id)
        {
            var ow = db.Owners.Where(p=>p.MemberID== id&& p.Status);
            return ow.Include(p => p.Koi).Where(p=>p.Koi.Status==1).Count(); 
        }

        public Koi getKoiById(int id)
        {
            
            return db.Kois.Find(id);
        }

        public bool AddKoi(Koi koi)
        {
            try
            {
                db.Kois.Add(koi);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ImportKoi(List<KoiImport> ListKoiImport, int memberID, int koifarmId, List<string> imagedetail, List<List<Medium>> media)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var ListKoi = new List<Koi>();
                    //var Listowner = new List<Owner>();
                    //var ListDetail = new List<InfoDetail>();
                    VarietyDAO varietyDao = new VarietyDAO();
                    int detailIndex = 0;
                    foreach (var item in ListKoiImport)
                    {
                        //thêm koi
                        var koi = new Koi(varietyDao.getVarityIdByName(item.Variety), item.KoiName, item.DoB, CommonFunction.ReturnGenderKoiDb(item.Gender), "", "", item.Image.ElementAt(0), item.Origin, 1, true);
                        db.Kois.Add(koi);
                        db.SaveChanges();
                        var koinewID = koi.KoiID;
                        //thêm owner
                        db.Owners.Add(new Owner(memberID, koinewID, koifarmId, DateTime.Now, null, true));
                        db.SaveChanges();
                        //thêm infodetail
                        var Detail = new InfoDetail(koinewID, DateTime.Now,  item.Size, String.Empty, String.Empty, imagedetail.ElementAt(detailIndex), true);
                        db.InfoDetails.Add(Detail);
                        db.SaveChanges();
                        var DetailID = Detail.DetailID;
                        //thêm media

                        
                            for (int j = 0; j < media.ElementAt(detailIndex).Count; j++)
                            {
                                media.ElementAt(detailIndex).ElementAt(j).ModelId = DetailID;
                                db.Media.Add(media.ElementAt(detailIndex).ElementAt(j));
                                db.SaveChanges();
                            }

                        detailIndex++;
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                    return false;
                }
            }
            return true;
        }

        public bool AddKoi(Koi koi,int memberID, string image,decimal size,List<Medium> media )
        {
            // Transaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //thêm koi
                    db.Kois.Add(koi);
                    db.SaveChanges();
                    var koinewID = koi.KoiID;
                    //thêm owner
                    db.Owners.Add(new Owner(memberID, koinewID, null, DateTime.Now, null, true));
                    db.SaveChanges();
                    //thêm infodetail
                    var Detail = new InfoDetail(koinewID, DateTime.Now,  size, String.Empty, String.Empty, image, true);
                    db.InfoDetails.Add(Detail);
                    db.SaveChanges();
                    var DetailID = Detail.DetailID;
                    //thêm media

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
            }
            return true;
        }

        public int EditKoi(Koi koi)
        {
            try
            {
                db.Kois.Attach(koi);
                var entry = db.Entry(koi);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.Privacy).IsModified = false;
                entry.Property(e => e.DeadReason).IsModified = false;
                entry.Property(e => e.IsDead).IsModified = false;

                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        public IQueryable<Koi> KoiFilter(KoiFilterModel searchModel)
        {
            var koi = db.Kois.AsQueryable();
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.KoiName))
                {
                    string koiname = CommonFunction.convertToNormalString(searchModel.KoiName);
                    koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                }
                if (!string.IsNullOrEmpty(searchModel.Username))
                {
                    koi = koi.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().Member.UserName.Equals(searchModel.Username));
                }
                if (!string.IsNullOrEmpty(searchModel.VarietyId))
                {
                    int? variety = CommonFunction.ToNullableInt(searchModel.VarietyId);
                    koi = koi.Where(p => p.VarietyID== variety);
                }
                if (!string.IsNullOrEmpty(searchModel.SizeFrom))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeFrom);
                    koi = koi.Where(p => p.InfoDetails.OrderBy(u => u.Date).FirstOrDefault().Size >= size);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.SizeTo))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeTo);
                    koi = koi.Where(p => p.InfoDetails.OrderBy(u => u.Date).FirstOrDefault().Size <= size);
                }
                if (!string.IsNullOrEmpty(searchModel.Gender))
                {
                    koi = koi.Where(p => p.Gender== searchModel.Gender);
                }
                if (!string.IsNullOrEmpty(searchModel.Owner))
                {
                    //@@
                   // koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                    koi = koi.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().Member.Name.Contains(searchModel.Owner)) ;
                }
                if (!string.IsNullOrEmpty(searchModel.AgeFrom))
                {
                    int? age = CommonFunction.ToNullableInt(searchModel.AgeFrom);
                    if (age.HasValue)
                    {
                        DateTime? Month = CommonFunction.getDateFromMonth(age.Value);
                        if (Month != null)
                        {
                                 koi = koi.Where(p => p.DoB >= Month);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchModel.AgeTo))
                {
                    int? age = CommonFunction.ToNullableInt(searchModel.AgeTo);
                    if (age.HasValue)
                    {
                        DateTime? Month = CommonFunction.getDateFromMonth(age.Value);
                        if (Month != null)
                        {
                            koi = koi.Where(p => p.DoB <= Month);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchModel.orderby))
                {
                    switch (searchModel.orderby)
                    {
                        case "newest":
                            koi = koi.OrderBy(p => p.InfoDetails.OrderBy(v=>v.Date).Select(v=>v.DetailID).FirstOrDefault());
                            break;
                        case "review":
                            koi = koi.Include(p => p.Comments).OrderByDescending(p => p.Comments.Count);
                            break;
                        case "date_desc":
                            break;
                        default:  // Name ascending 
                            break;
                    }
                }
                // kiểm tra tồn tại koi
                koi = koi.Where(p => p.Status==1);
            }
            else { return null;}
            return koi;
        }

        public int ToDead(int koiId,string deadReason)
        {
            var koi = db.Kois.Find(koiId);
            try
            {
                koi.IsDead = true;
                koi.Status = 1;
                koi.DeadReason = deadReason;
                db.Kois.Attach(koi);
                var entry = db.Entry(koi);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.IsDead).IsModified = true;
                entry.Property(e => e.DeadReason).IsModified = true;
                entry.Property(e => e.Status).IsModified = true;

                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }
        public int AddParent(int koiSonId, int koMomId)
        {
            var koi = db.Kois.Find(koiSonId);
            try
            {
                koi.KoiMom = koMomId;
                db.Kois.Attach(koi);
                var entry = db.Entry(koi);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.KoiMom).IsModified = true;
                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        public bool AddNewParent(Koi koi, int koiSonId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //thêm koi mẹ
                    db.Kois.Add(koi);
                    db.SaveChanges();
                    //lấy id koi mẹ
                    var koiMomID = koi.KoiID;
                   // thêm koi con
                    var koiSon = db.Kois.Find(koiSonId);
                    if (koiSon == null)
                        return false;
                    koiSon.KoiMom = koiMomID;
                    db.Kois.Attach(koiSon);
                    var entry = db.Entry(koiSon);
                    entry.State = EntityState.Modified;
                    // Set column not change
                    entry.Property(e => e.KoiMom).IsModified = true;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                    return false;
                }
            }
        }

        public List<Koi> getListKoiSonByKoiID(int koiID)
        {
            return db.Kois.Where(p => p.KoiMom == koiID && p.Status==1).Take(20).ToList();
        }

        public int? GetKoimombykoiID(int koiid)
        {
            var t = db.Kois.Find(koiid).KoiMom;
            if (t.HasValue)
            {
                return t.Value;
            }
            return null;
        }


    }
}
