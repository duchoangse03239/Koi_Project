using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using LinqToExcel;
using PagedList;

namespace KoiManagement.Controllers
{
    public class KoiFarmController : Controller
    {
        KoiManagementEntities db = new KoiManagementEntities();
        KoiDAO koiDao = new KoiDAO();
        MemberDAO memberDao = new MemberDAO();
        KoiFarmDAO koiFarmDao = new KoiFarmDAO();
        InfoDetailDAO DetailDao = new InfoDetailDAO();
        OwnerDAO ownerDao = new OwnerDAO();
        // GET: KoiFarm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult KoiFarmDetail(int? id, int? page, string orderby, string nameKoi, string username, string variety, string sizeFrom, string sizeTo, string gender, string owner, string AgeFrom, string AgeTo)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var listkoi = koiFarmDao.GetListKoiByKoiFarmId((int)id);
            var koifarm = koiFarmDao.GetKoiFarmDetail((int)id);
            if (koifarm == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.koiFarm = koifarm;
            //ViewBag.listKoi = listkoi;
            VarietyDAO varietyDao= new VarietyDAO();
            KoiDAO kDao = new KoiDAO();
            //list koi

                        if (!String.IsNullOrEmpty(variety)&&variety.Equals("0"))
            {
                variety ="";
            }
            //ViewBag.VarietyId = id;
            KoiFilterModel filter = new KoiFilterModel(orderby, nameKoi, username, variety, sizeFrom, sizeTo, gender, owner, AgeFrom, AgeTo);
            ViewBag.Filter = filter;
            ViewBag.listVariety = varietyDao.getListMainVariety();

            var koi = db.Kois.AsQueryable();

            koi = koiFarmDao.KoiFilterByKoiFarm(filter, (int)id);

            // phân trang 6 item 1trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Listkoi = koi.ToList().ToPagedList(pageNumber, pageSize);


            return View();
        }

        public ActionResult CreateKoiFarm()
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CreateKoiFarm(string farmName, string address, string description)
        {
            ViewBag.Message = string.Empty;
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            if (string.IsNullOrWhiteSpace(farmName) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(description) || farmName.Length > 100)
            {
                return View();

            }
            try
            {
                KoiFarm koiFarm = new KoiFarm();
                koiFarm.FarmName = farmName;
                koiFarm.Address = address;
                koiFarm.Description = description;
                koiFarm.MemberID = id;
                koiFarm.Status = 1;
                if (koiFarmDao.AddKoiFarm(koiFarm))
                {
                    ViewBag.Message = "Bạn đã thêm trang trại thành công, vui lòng chờ người quản trị xác nhận thông tin";
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " +
                                       validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        public ActionResult ListKoiFarm(int? page, string orderby, string farmname, string owner, string address)
        {
            //lấy trường lọc
            KoiFarmFilterModel koiFarmFilter = new KoiFarmFilterModel(orderby, farmname, owner, address);
            ViewBag.Filter = koiFarmFilter;
            //Lấy list koifarm
            var ListKoiFarm = db.KoiFarms.AsQueryable();
            ListKoiFarm = koiFarmDao.KoiFarmFilter(koiFarmFilter);
            // var ListKoiFarm = koiFarmDao.GetListKoiFarm();
            // ViewBag.ListKoiFarm = ListKoiFarm;
            Dictionary<int, string> dict = new Dictionary<int, string>();
            List<string> listVariety = new List<string>();
            string t = "";
            foreach (var koifarm in ListKoiFarm)
            {
                t = "";
                dict.Clear();
                var db1 =
                    db.Kois.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().KoiFarmID == koifarm.KoifarmID);
                foreach (var item in db1)
                {
                    if (!dict.ContainsKey(item.Variety.VarietyID) && !item.Variety.VarietyName.Equals("Chưa rõ"))
                        dict.Add(item.Variety.VarietyID, item.Variety.VarietyName);
                }
                foreach (var v in dict)
                {
                    t += v.Value + ", ";
                }
                t = CommonFunction.Trim2LastCharacter(t);
                if (string.IsNullOrEmpty(t))
                {
                    t = " ";
                }
                listVariety.Add(t);
            }
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.listVariety = listVariety;
            ViewBag.ListKoiFarm = ListKoiFarm.ToList().ToPagedList(pageNumber, pageSize);

            return View();
        }

        public ActionResult EditKoiFarm(int? id)
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //check tồn tại id
            if (id == null)
            {
                return RedirectToAction("ListKoiFarm", "KoiFarm");
            }

            // lấy id người đang đăng nhập
            int Mid = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(Mid);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(Mid);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(Mid);

            ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail((int)id);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditKoiFarm(string farmName, string address, string description, string KoifarmID)
        {
            ViewBag.Message = string.Empty;
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            if (string.IsNullOrWhiteSpace(farmName) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(description) || farmName.Length > 100)
            {
                return View();

            }
            try
            {
                KoiFarm koiFarm = new KoiFarm();
                koiFarm.FarmName = farmName;
                koiFarm.Address = address;
                koiFarm.Description = description;
                koiFarm.Status = 1;
                koiFarm.KoifarmID = int.Parse(KoifarmID);
                if (koiFarmDao.EditKoiFarm(koiFarm) > 0)
                {
                    ViewBag.Message = "Bạn đã sửa thông tin thành công, vui lòng chờ người quản trị xác nhận thông tin";
                    ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail(int.Parse(KoifarmID));
                }
                else
                {
                    ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail(int.Parse(KoifarmID));
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " +
                                       validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        public ActionResult KoifarmOwner(int id = 0)
        {
            //kiểm tra đăng nhập
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            // lấy id người đang đăng nhập
            //id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            var koifarm = koiFarmDao.GetListKoiFarmByMemberId(id);
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            return View(koifarm);
        }

        public ActionResult AddKoiToKoiFarm(int? id, int? page)
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            var check = memberDao.GetMemberbyID(id.Value);
            if (check == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var ad = db.Kois.Where(cu => cu.Owners.Any(c => c.MemberID == id&&c.KoiFarmID==null));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.ListKoi = ad.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.KoiFarmID = id;
            return View();
        }


        /// <summary>  
        /// This function is used to download excel format.  
        /// </summary>  
        /// <param name="Path"></param>  
        /// <returns>file</returns>  
        public FileResult DownloadExcel()
        {
            string path = "~/Content/FileImport/Koi.xlsx";
            return File(path, "application/vnd.ms-excel", "Koi.xlsx");
        }

        public ActionResult Import()
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Import( HttpPostedFileBase FileUpload)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();

            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }

            //session
            var memberid = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            var koifarmId = ownerDao.getKoiFarmbyMember(memberid);
            if (koifarmId == 0)
            {
                obj.Message = "Bạn không phải là chủ trang trại, xin hãy đăng kí trang trại.";
                obj.Status = 0;
                return Json(obj);
            }
            List<string> data = new List<string>();
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = null;
            if(files.Count>0)
            FileUpload = files[0];
            if (FileUpload != null)
            {
                //  tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    
                    int ImageCount = files.Count-1;

                    //Lấy file execel validate
                    string filenameExcel = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Content/FileImport");
                    FileUpload.SaveAs(targetpath + filenameExcel);
                    string pathToExcelFile = targetpath + filenameExcel;
                    var connectionString = "";
                    if (filenameExcel.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filenameExcel.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }


                    
                    var adapter = new OleDbDataAdapter("SELECT * FROM [ImportData$]", connectionString);


                    var ds = new DataSet();
                    //if (!ds.Tables.Cast<DataTable>().Any(x => x.DefaultView.Count > 0))
                    //{
                    //    obj.Message = "Tập tin không đúng mẫu.";
                    //    obj.Status = 0;
                    //    return Json(obj);
                    //}
                    try
                    {
                        adapter.Fill(ds, "ExcelTable");
                    }
                    catch
                    {
                        obj.Message = "Tập tin không đúng mẫu.";
                        obj.Status = 0;
                        return Json(obj);
                    }
                    DataTable dataTableImport = ds.Tables["ExcelTable"];

                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    if (dataTableImport.Columns.Count != 11)
                    {
                        obj.Message = "Tập tin không đúng mẫu.";
                        obj.Status = 0;
                        return Json(obj);
                    }
                    if (dataTableImport.Rows.Count == 0)
                    {
                        obj.Message = "Xin hãy nhập dữ liệu.";
                        obj.Status = 0;
                        return Json(obj);
                    }
                    // check format file
                    String error = ValidateFileExcel(dataTableImport, ImageCount);
                    if (!String.IsNullOrEmpty(error))
                    {
                        obj.Message = error;
                        obj.Status = 0;
                        return Json(obj);
                    }
                    //thêm data từ excel to list
                    var ListImport = new List<KoiImport>();
                    
                    // List link anh để so sánh
                    var ListImage = new List<string>();
                    var ListImageCompare = new List<string>();
                    for (int i = 0; i < dataTableImport.Rows.Count; i++)
                    {
                        ListImage.Clear();
                        
                        //thêm list ảnh
                        if (!string.IsNullOrWhiteSpace(dataTableImport.Rows[i][6].ToString()))
                        {
                            ListImage.Add(dataTableImport.Rows[i][6].ToString());
                        }
                        if (!string.IsNullOrWhiteSpace(dataTableImport.Rows[i][7].ToString()))
                        {
                            ListImage.Add(dataTableImport.Rows[i][7].ToString());
                        }
                        if (!string.IsNullOrWhiteSpace(dataTableImport.Rows[i][8].ToString()))
                        {
                            ListImage.Add(dataTableImport.Rows[i][8].ToString());
                        }
                        if (!string.IsNullOrWhiteSpace(dataTableImport.Rows[i][9].ToString()))
                        {
                            ListImage.Add(dataTableImport.Rows[i][9].ToString());
                        }
                        if (!string.IsNullOrWhiteSpace(dataTableImport.Rows[i][10].ToString()))
                        {
                            ListImage.Add(dataTableImport.Rows[i][10].ToString());
                        }
                        string datetime = Validate.ConverDateExcel(string.Concat(dataTableImport.Rows[i][3].ToString().TakeWhile((c) => c != ' ')));

                        ListImport.Add(new KoiImport(dataTableImport.Rows[i][1].ToString(), dataTableImport.Rows[i][0].ToString(), Validate.ConverDateTime(datetime), dataTableImport.Rows[i][2].ToString(), decimal.Parse(dataTableImport.Rows[i][4].ToString()), dataTableImport.Rows[i][5].ToString()
                            , ListImage));
                        ListImageCompare.AddRange(ListImage);
                    }
                    // So sánh ảnh var difference = list1.Except(list2).ToList();  2 count = nhau
                    //Save image to server
                    //List tên ảnh gôc để so sanh
                    List<string> ListOriginImage = new List<string>();
                    Dictionary<int, string> Listdictionary =new Dictionary<int, string>();

                    for (int i = 1; i < files.Count; i++)
                    {
                        file = files[i];
                        var fileName = file.FileName;
                        fileName = fileName.Remove(fileName.LastIndexOf("."));
                        Listdictionary.Add(i, fileName);
                        ListOriginImage.Add(fileName);
                    }
                    var difference = ListImageCompare.Except(ListOriginImage).ToList();
                    if (difference.Count > 0)
                    {
                        obj.Status = 0;
                        if (difference.Count > 3)
                        {
                            obj.Message = "Không tìm thấy ảnh " + difference.ElementAt(0) + " " +
                                          difference.ElementAt(1) + " " + difference.ElementAt(2) + "...";
                        }
                        else
                        {
                            obj.Message = "Không tìm thấy ảnh" ;
                            for (int i=0;i< difference.Count;i++)
                            {
                                obj.Message = obj.Message + " " + difference.ElementAt(i);
                            }
                           
                        }

                        return Json(obj);
                    }

                    // List ảnh Dùng khi thêm không thành công thì xóa
                    List<string> fullpath = new List<string>();
                    // List link anh
                     List<List<Medium>> ListMedia = new List<List<Medium>>();
                    // lấy id max đặt tên file ảnh
                    var maxKoiId = koiDao.GetMaxKoiID();
                    var maxDetailId = DetailDao.GetMaxDetailID();
                    string imageKoiname;
                    //List ảnh detail
                    List<string> listImageDetailname = new List<string>();

                    int indexImage = 0;
                    int indexDetailName = 0;
                    //Trường hợp có nhiều file ảnh, file đầu tiên là file excel
                    for (var koiIndex=0; koiIndex < ListImport.Count; koiIndex ++)
                    {
                        var listIamge = new List<Medium>();

                        for (var image = 0; image < ListImport.ElementAt(koiIndex).Image.Count; image++)
                        {
                            var indeximage = Listdictionary.FirstOrDefault(x => x.Value == ListImport.ElementAt(koiIndex).Image.ElementAt(image)).Key;
                            Listdictionary.Remove(indeximage);
                            file = files[indeximage];
                            if (image == 0)
                            {
                         //thêm ảnh cho koi
                            imageKoiname = Path.GetFileName("Koi" + maxKoiId + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                            //Lấy ảnh đầu tiên làm avatar
                            ListImport.ElementAt(koiIndex).Image[0] = imageKoiname;
                            fullpath.Add(Server.MapPath("~/Content/Image/Koi/" + imageKoiname));
                            var pathKoi = Path.Combine(Server.MapPath("~/Content/Image/Koi"), imageKoiname);
                            file.SaveAs(pathKoi);
                            //thêm ảnh cho koi detail
                            listImageDetailname.Add(Path.GetFileName("Detail" + maxDetailId + file.FileName.Substring(file.FileName.LastIndexOf('.'))));

                            fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + listImageDetailname.ElementAt(indexDetailName)));
                            var pathDetail = Path.Combine(Server.MapPath("~/Content/Image/Detail"), listImageDetailname.ElementAt(indexDetailName));
                            file.SaveAs(pathDetail);
                            }
                            else if (file != null)
                            {
                                var filename = Path.GetFileName("Detail" + maxDetailId + "." + image + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                                fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                                var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                                file.SaveAs(path);
                                Medium a = new Medium();
                                a.LinkImage = filename;
                                a.Status = true;
                                listIamge.Add(a);
                            }
                        }
                        indexDetailName++;
                        ListMedia.Add(listIamge);

                        maxKoiId++;
                        maxDetailId++;
                    }

                    //Import To database
                    if (koiDao.ImportKoi(ListImport, memberid, koifarmId, listImageDetailname, ListMedia))
                    {
                        //success
                        obj.Status = 1;
                        obj.Message = "Bạn đã thêm koi thành công";
                        obj.RedirectTo = Url.Action("KoiUser/" + Session[SessionAccount.SessionUserId], "Koi");
                    }
                    else
                    {
                        //Nếu fail xoa anh da them
                        foreach (var image in fullpath)
                        {
                            if (System.IO.File.Exists(image))
                            {
                                System.IO.File.Delete(image);
                            }
                        }
                    }

                }
                else
                {

                    obj.Status = 0;
                    obj.Message = "Chỉ chấp nhập tập tin excel.";
                    return Json(obj);
                }
            }
            else
            {
                obj.Status = 0;
                obj.Message = "Xin hãy chọn tập tin excel.";
                return Json(obj);
            }
            return Json(obj);
        }

        public string ValidateFileExcel(DataTable ListKoi, int image)
        {
            int count = 0;
            for (int i = 0; i < ListKoi.Rows.Count; i++)
            {
                int row = i + 2;
                if (String.IsNullOrWhiteSpace(ListKoi.Rows[i][0].ToString()))
                {
                    return "Tên cá Koi không được trống tại hàng " + row + " cột A.";
                }
                else if (String.IsNullOrWhiteSpace(ListKoi.Rows[i][1].ToString()))
                {
                    return "Chủng loại không được trống tại hàng " + row + " cột B.";
                }
                string datetime = string.Concat(ListKoi.Rows[i][3].ToString().TakeWhile((c) => c != ' '));
                if (!String.IsNullOrWhiteSpace(datetime))
                {
                    if (!Validate.ValidateDateExcel(datetime))
                    {
                        return "Xin hãy nhập đúng định dạng ngày tháng tại hàng " + row + " cột D.";
                    }
                }
                else if (String.IsNullOrWhiteSpace(ListKoi.Rows[i][4].ToString()))
                {
                    return "Kích thước không được trống tại hàng " + row + " cột E.";
                }

                else if (!Validate.CheckIsDouble(ListKoi.Rows[i][4].ToString()))
                {
                    return "Xin hãy nhập kích thước kiểu số tại hàng " + row + " cột E.";
                }
                decimal size = decimal.Parse(ListKoi.Rows[i][4].ToString());
                if (size < 0)
                {
                    return "Xin hãy nhập số dương cho kích thước tại hàng " + row + " cột E.";
                }
                if (size > 150)
                {
                    return "Kích thước phải nhỏ hơn 150cm tại hàng " + row + " cột E.";
                }

                else if (string.IsNullOrWhiteSpace(ListKoi.Rows[i][6].ToString()))
                {
                    return "Hãy nhập ít nhất 1 ảnh cho Koi tại hàng " + row + " cột G.";
                }
                 if (!string.IsNullOrWhiteSpace(ListKoi.Rows[i][6].ToString()))
                 {
                     count++;
                 }
                if (!string.IsNullOrWhiteSpace(ListKoi.Rows[i][7].ToString()))
                {
                    count++;
                }
                if (!string.IsNullOrWhiteSpace(ListKoi.Rows[i][8].ToString()))
                {
                    count++;
                }
                if (!string.IsNullOrWhiteSpace(ListKoi.Rows[i][9].ToString()))
                {
                    count++;
                }
                if (!string.IsNullOrWhiteSpace(ListKoi.Rows[i][10].ToString()))
                {
                    count++;
                }
            }
            //so sanh số lượng ảnh
            if (count != image)
            {
                return "Xin hãy chọn đủ số lượng "+ count + " ảnh tương ứng với tập tin excel.";
            }
            return "";
        }


        

         public ActionResult DeleteKoiFarm(string KoiFarmId)
        {
            KoiFarm koiFarm = db.KoiFarms.Find(int.Parse(KoiFarmId));
            koiFarm.Status = -1;
            db.KoiFarms.Attach(koiFarm);
            db.Entry(koiFarm).Property(x => x.Status).IsModified = true;
            int result = db.SaveChanges();
            //return View();
            if (result == 1)
            {
                return Json(new { result = true });
            }
            else
            {
                return Json(new
                {
                    result = false
                });
            }
        }

        [HttpPost]
        public JsonResult AddToKoiFarm(string KoiID, int koiFarmId)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            var myListKoi = KoiID.Split(',').Select(x => Int32.Parse(x)).ToArray();

            if (ownerDao.AddListKoiToKoiFarm(myListKoi, koiFarmId))
            {
                obj.Status = 1;
                obj.Message = "Thêm thành công.";
                return Json(obj);
            }
            return Json(obj);
        }
        [HttpPost]
        public JsonResult Rating(int koifarmID, string RateNum, string content)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();

            try
            {
                CommentDAO commentDao = new CommentDAO();
                if (Session[SessionAccount.SessionUserId] == null)
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy đăng nhập để đánh giá.";
                    return Json(obj);
                }
                var UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                if (string.IsNullOrWhiteSpace(content))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập nội dung đánh giá.";
                    return Json(obj);
                }
                if (!Validate.CheckIsDouble(RateNum))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy chọn sao đánh giá.";
                    return Json(obj);
                }
                if (!commentDao.CheckRatingKoiFarm(UserID, koifarmID))
                {
                    obj.Status = 3;
                    obj.Message = "Bạn đã đánh giá cá Koi này rồi.";
                    return Json(obj);
                }
                decimal sao = decimal.Parse(RateNum);
                Comment c = new Comment(int.Parse(Session[SessionAccount.SessionUserId].ToString()),null, DateTime.Now, koifarmID, sao, content, null, true);
                if (commentDao.addComment(c))
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã gửi đánh giá thành công!";
                    return Json(obj);
                }

            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

    }
}