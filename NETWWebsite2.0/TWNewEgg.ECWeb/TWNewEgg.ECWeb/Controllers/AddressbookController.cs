using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Website.ECWeb.Service;
using System.Text.RegularExpressions;
using TWNewEgg.DB;
//using TWNewEgg.Website.ECWeb.Controllers.Api;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Account;
using System.IO;

namespace TWNewEgg.ECWeb.Controllers
{
    /// <summary>
    /// AddressBook畫面的展示與AddressBook的資料更新、修改與刪除
    /// </summary>
    public class AddressbookController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private IGetInfo checkID = new GetInfoRepository();
        // GET  /Addressbook/
        private AesCookies aesEnc = new AesCookies();

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>回傳View</returns>
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// 收件人紀錄本資料收集與重至預設值function
        /// </summary>
        /// <returns>秀出收件人紀錄本畫面</returns>
        public ActionResult RecordReceive()
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            //MyRecordBook myrecordbook = new MyRecordBook();
            List<Addressbook> addressbook = new List<Addressbook>();
            int accID = NEUser.ID;
            Account account = (from p in db_before.Account where p.ID == accID select p).FirstOrDefault();
            if (accID > 0 && account != null)
            {
                List<Addressbook> addressBooks = (from q in db_before.Addressbook where q.AccountID == account.ID select q).ToList();
                // 重新設置預設值時使用
                int defaultCount = addressBooks.Where(x => x.DefaultSetting == 1).ToList().Count;
                // 當預設值不只一個時才執行
                if (defaultCount > 1)
                {
                    // 重置所有預設值(0)
                    foreach (Addressbook subAddressbook in addressBooks)
                    {
                        subAddressbook.DefaultSetting = 0;
                    }
                    // 預設該收件人紀錄本中第一項收件人資訊為預設值
                    addressBooks[0].DefaultSetting = 1;
                    db_before.SaveChanges();
                }
                //myrecordbook.addressbook = addressBooks;
                //return PartialView(myrecordbook);
                return PartialView(addressBooks);
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "MyAccount", new { returnUrl = "/Addressbook/RecordReceive" });
            }
            else
            {
                return JavaScript("window.location = '" + Url.Action("Login", "MyAccount", new { returnUrl = "/Addressbook/RecordReceive" }) + "'");
            }
        }

        /// <summary>
        /// 新增收件人紀錄本
        /// </summary>
        /// <param name="_data">Addressbook所需相關資料</param>
        /// <param name="myNewegg">是否由myNewegg執行的動作</param>
        /// <returns>新增收件人紀錄本資訊</returns>
        //public ActionResult AddNewAddressBook(Addressbook _data, bool myNewegg = false)
        //{
        //    TWSqlDBContext db_before = new TWSqlDBContext();
        //    string returnNewUrl = "";
        //    if (myNewegg)
        //    {
        //        returnNewUrl = "/myNewegg/newAddressBook";
        //    }
        //    else
        //    {
        //        returnNewUrl = "/Addressbook/RecordReceive";
        //    }

        //    try
        //    {
        //        int accID = NEUser.ID;
        //        if (accID > 0)
        //        {
        //            Addressbook newAddressBook = new Addressbook();
        //            // Addressbook資料格式驗證
        //            string validationString = FormatValidation(_data);
        //            if (validationString.Length > 0)
        //            {
        //                // 若格式驗證失敗則回傳失敗訊息
        //                return Json(validationString);
        //            }
        //            // 新增收件人
        //            newAddressBook.AccountID = accID;
        //            newAddressBook.RecvSex = _data.RecvSex;
        //            newAddressBook.RecvName = _data.RecvName;
        //            newAddressBook.RecvLastName = _data.RecvLastName;
        //            newAddressBook.RecvFirstName = _data.RecvFirstName;
        //            string[] tel = _data.RecvTelDay.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        //            newAddressBook.RecvTelDay = tel.Length != 0 ? _data.RecvTelDay : "";
        //            newAddressBook.RecvMobile = _data.RecvMobile == null ? "" : _data.RecvMobile.Trim(new char[] { ' ', '-', '(', ')' });
        //            newAddressBook.DelivLOC = _data.DelivLOC;
        //            newAddressBook.DelivZip = _data.DelivZip;
        //            newAddressBook.DelivAddr = _data.DelivAddr;
        //            newAddressBook.CreateDate = DateTime.Now;
        //            newAddressBook.AccountEmail = (from p in db_before.Account where p.ID == accID select p.Email).FirstOrDefault();
        //            newAddressBook.Updated = 0;

        //            db_before.Addressbook.Add(newAddressBook);
        //            db_before.SaveChanges();
        //            return Json("[Message]新增成功!");
        //        }
        //        else if (!Request.IsAjaxRequest())
        //        {
        //            return RedirectToAction("Login", "MyAccount", new { returnUrl = returnNewUrl });
        //        }
        //        else
        //        {
        //            return JavaScript("window.location = '" + Url.Action("Login", "MyAccount", new { returnUrl = returnNewUrl }) + "'");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json("[Error]資料錯誤，新增失敗");
        //    }
        //}

        /// <summary>
        /// 收件人資訊更新
        /// </summary>
        /// <param name="_data">Addressbook所需相關資料</param>
        /// <param name="myNewegg">是否由myNewegg執行的動作</param>
        /// <returns>回傳是否成功的Jsno訊息</returns>
        //public ActionResult ModifyAddressBook(Addressbook _data, bool myNewegg = false)
        //{
        //    TWSqlDBContext db_before = new TWSqlDBContext();
        //    string returnNewUrl = "";
        //    if (myNewegg)
        //    {
        //        returnNewUrl = "/myNewegg/newAddressBook";
        //    }
        //    else
        //    {
        //        returnNewUrl = "/Addressbook/RecordReceive";
        //    }

        //    try
        //    {
        //        int accID = NEUser.ID;
        //        if (accID > 0)
        //        {
        //            ViewBag.tabOrder = 3;
        //            Addressbook updateAddressBook = new Addressbook();
        //            // Addressbook資料格式驗證
        //            string validationString = FormatValidation(_data);
        //            if (validationString.Length > 0)
        //            {
        //                // 若格式驗證失敗則回傳失敗訊息
        //                return Json(validationString);
        //            }
        //            // 更新收件人資訊
        //            updateAddressBook = db_before.Addressbook.Where(x => x.ID == _data.ID).FirstOrDefault();
        //            if (updateAddressBook != null)
        //            {
        //                Account account = (from p in db_before.Account where p.ID == accID select p).FirstOrDefault();
        //                updateAddressBook.RecvSex = _data.RecvSex;
        //                updateAddressBook.RecvName = _data.RecvName;
        //                updateAddressBook.RecvLastName = _data.RecvLastName;
        //                updateAddressBook.RecvFirstName = _data.RecvFirstName;
        //                string[] tel = _data.RecvTelDay.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        //                updateAddressBook.RecvTelDay = tel.Length != 0 ? _data.RecvTelDay : "";
        //                updateAddressBook.RecvMobile = _data.RecvMobile == null ? "" : _data.RecvMobile.Trim(new char[] { ' ', '-', '(', ')' });
        //                updateAddressBook.DelivLOC = _data.DelivLOC;
        //                updateAddressBook.DelivZip = _data.DelivZip;
        //                updateAddressBook.DelivAddr = _data.DelivAddr;
        //                updateAddressBook.Updated++;
        //                updateAddressBook.AccountEmail = account.Email;
        //                db_before.SaveChanges();
        //                return Json("[Message]修改成功");
        //            }

        //            if (myNewegg)
        //            {
        //                return RedirectToAction("MyNeweggAddressBook", "MyNewegg");
        //            }
        //            else
        //            {
        //                return RedirectToAction("RecordReceive", "Addressbook");
        //            }
        //        }
        //        else if (!Request.IsAjaxRequest())
        //        {
        //            return RedirectToAction("Login", "MyAccount", new { returnUrl = returnNewUrl });
        //        }
        //        else
        //        {
        //            return JavaScript("window.location = '" + Url.Action("Login", "MyAccount", new { returnUrl = returnNewUrl }) + "'");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json("[Error]資料錯誤，更新失敗");
        //    }
        //}

        /// <summary>
        /// 刪除單項收件人紀錄本資料
        /// </summary>
        /// <param name="_data">Addressbook所需相關資料</param>
        /// <param name="myNewegg">是否由myNewegg執行的動作</param>
        /// <returns>回傳是否刪除成功</returns>
        //public ActionResult DeleteAddressBook(Addressbook _data, bool myNewegg = false)
        //{
        //    string returnNewUrl = "";
        //    if (myNewegg)
        //    {
        //        returnNewUrl = "/myNewegg/newAddressBook";
        //    }
        //    else
        //    {
        //        returnNewUrl = "/Addressbook/RecordReceive";
        //    }

        //    TWSqlDBContext db_before = new TWSqlDBContext();
        //    try
        //    {
        //        int accID = NEUser.ID;
        //        if (accID > 0)
        //        {
        //            ViewBag.tabOrder = 3;
        //            Addressbook deleteData = db_before.Addressbook.Where(x => x.ID == _data.ID).FirstOrDefault();
        //            if (deleteData.AccountID != accID)
        //            {
        //                return RedirectToAction("RecordReceive");
        //            }

        //            if (deleteData != null)
        //            {
        //                db_before.Addressbook.Remove(deleteData);
        //                db_before.SaveChanges();
        //                return Json("[Message]刪除成功");
        //            }

        //            if (myNewegg)
        //            {
        //                return RedirectToAction("MyNeweggAddressBook", "MyNewegg");
        //            }
        //            else
        //            {
        //                return RedirectToAction("RecordReceive", "Addressbook");
        //            }
        //        }
        //        else if (!Request.IsAjaxRequest())
        //        {
        //            return RedirectToAction("Login", "MyAccount", new { returnUrl = returnNewUrl });
        //        }
        //        else
        //        {
        //            return JavaScript("window.location = '" + Url.Action("Login", "MyAccount", new { returnUrl = returnNewUrl }) + "'");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json("[Error]資料錯誤，刪除失敗");
        //    }
        //}

        /// <summary>
        /// 格式驗證
        /// </summary>
        /// <param name="_data">欲驗證的資訊</param>
        /// <returns>回傳驗證格式是否有問題</returns>
        //public string FormatValidation(Addressbook _data)
        //{
        //    TWSqlDBContext db_before = new TWSqlDBContext();
        //    try
        //    {
        //        string resultMessage = "";
        //        List<string> name = new List<string>();
        //        List<string> numberList = new List<string>();
        //        name.Add(_data.RecvName);
        //        bool nameResult = CheckRecName(name);
        //        if (nameResult == false)
        //        {
        //            return "[Error]姓名格式錯誤，需全部為中文字";
        //        }

        //        List<string> tel = _data.RecvTelDay.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
        //        if (CheckNumber(tel))
        //        {
        //            return "[Error]電話號碼格式錯誤，需全部為數字";
        //        }
        //        // 驗證電話號碼長度
        //        if (tel.Count > 0 && tel[0].Length + tel[1].Length != 9 && tel[0].Length + tel[1].Length != 10)
        //        {
        //            return "[Error]電話號碼長度錯誤";
        //        }

        //        numberList.Add(_data.RecvMobile);
        //        if (CheckNumber(numberList))
        //        {
        //            return "[Error]手機格式錯誤，必須全部為數字";
        //        }

        //        numberList.Clear();
        //        if (_data.RecvMobile.Length != 10)
        //        {
        //            return "[Error]手機長度錯誤，請重新確認";
        //        }

        //        if (_data.RecvMobile.Substring(0, 2) != "09")
        //        {
        //            return "[Error]手機格式錯誤，開頭必須是\"09\"(不支援+886)";
        //        }

        //        return resultMessage;
        //    }
        //    catch (Exception e)
        //    {
        //        return "[Error]驗證錯誤";
        //    }
        //}

        /// <summary>
        /// 檢驗收件人姓名是否為中文字
        /// </summary>
        /// <param name="nameCheck">欲檢查的資訊</param>
        /// <returns>回傳收件人中文姓名是否符合，是則回傳true否則回傳false</returns>
        //public bool CheckRecName(List<string> nameCheck)
        //{
        //    int range = 0;
        //    int j = 0;
        //    int dstringmax = Convert.ToInt32("9fff", 16);
        //    int dstringmin = Convert.ToInt32("4e00", 16);
        //    for (int i = 0; i < nameCheck[0].Length; i++)
        //    {
        //        range = Convert.ToInt32(Convert.ToChar(nameCheck[0].Substring(i, 1)));
        //        if (range >= dstringmin && range < dstringmax)
        //        {
        //            j++;
        //        }
        //    }

        //    if (j == nameCheck[0].Length)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// 檢驗字串是否為數字
        /// </summary>
        /// <param name="numberCheck">欲檢查的資訊</param>
        /// <returns>若該字串不為數字則回傳ture，反之回傳false</returns>
        //public bool CheckNumber(List<string> numberCheck)
        //{
        //    foreach (string subNumber in numberCheck)
        //    {
        //        int tryOut = 0;
        //        bool numCheck = int.TryParse(subNumber, out tryOut);
        //        // 若該字串不為數字則回傳ture
        //        if (numCheck == false)
        //        {
        //            return true;
        //        }
        //    }
        //    // 若該字串回數字則回傳false
        //    return false;
        //}

        /// <summary>
        /// 更新Address預設選項
        /// </summary>
        /// <param name="addressBookID">addressBookID</param>
        /// <returns>更新預設選項</returns>
        //public ActionResult ReSetAddressDefault(int addressBookID)
        //{
        //    TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
        //    try
        //    {
        //        int accID = NEUser.ID;
        //        List<Addressbook> addressbooks = db_before.Addressbook.Where(x => x.AccountID == accID).ToList();
        //        Addressbook searchAddressbook = db_before.Addressbook.Where(x => x.ID == addressBookID && x.AccountID == accID && x.RecvSex != 2).FirstOrDefault();
        //        foreach (Addressbook subAddressbook in addressbooks)
        //        {
        //            subAddressbook.DefaultSetting = 0;
        //        }

        //        if (searchAddressbook != null)
        //        {
        //            searchAddressbook.DefaultSetting = 1; // 設定為預設
        //            db_before.SaveChanges();
        //            return Json(new { Message = "[Success]" });
        //        }

        //        return Json(new { Message = "[Error]" });
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Info("ReSetAddressDefault Error [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
        //        return Json(new { Message = "[Error]" });
        //    }
        //}

        /// <summary>
        /// 修正AddressBook預設選項
        /// </summary>
        /// <param name="addressBookID">AddressBookID</param>
        /// <param name="memberChange">是否為MemberAddressBook</param>
        /// <returns>返回執行結果</returns>
        public JsonResult ChangeAddressBookDefault(int addressBookID, bool memberChange, string viewPath)
        {
            bool IsSuccess = false;
            int getAccID = NEUser.ID;
            try
            {
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "ChangeAddressBookDefault", getAccID, addressBookID, memberChange).results;
                if (IsSuccess)
                {
                    
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(viewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                    //return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                logger.Error("Email[" + NEUser.Email + "] 修改[" + viewPath + "] 失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddNewAddressBook(AddressBookVM addressBook)
        {
            bool IsSuccess = false;
            try
            {
                addressBook.AccountEmail = NEUser.Email;
                addressBook.AccountID = NEUser.ID;
                AddressBookDM addAddressBook = ModelConverter.ConvertTo<AddressBookDM>(addressBook);
                addAddressBook.DefaultSetting = 1;
                AddressBookVM resultAddressBook = Processor.Request<AddressBookVM, AddressBookDM>("GetMemberService", "AddAddressBook", addAddressBook).results;
                if (resultAddressBook != null)
                {
                    IsSuccess = true;
                }
                if (IsSuccess)
                {
                    int getAccID = NEUser.ID;
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(addressBook.ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditAddressBook(AddressBookVM addressBook)
        {
            bool IsSuccess = false;
            try
            {
                addressBook.AccountEmail = NEUser.Email;
                addressBook.AccountID = NEUser.ID;
                AddressBookDM updateAddressBook = ModelConverter.ConvertTo<AddressBookDM>(addressBook);
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "ModifyAddressBook", updateAddressBook).results;
                //return Json(new { IsSuccess = IsSuccess, addressBook = addressBook }, JsonRequestBehavior.AllowGet);
                if (IsSuccess)
                {
                    int getAccID = NEUser.ID;
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(addressBook.ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteAddressBook(int addressBookID, string viewPath)
        {
            //int defaultAddressBook = 0;
            bool IsSuccess = false;
            try
            {
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "DeleteAddressBook", addressBookID).results;
                if (IsSuccess)
                {
                    int getAccID = NEUser.ID;
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    //if (getCartMemberInfo != null)
                    //{
                    //    defaultAddressBook = getCartMemberInfo.MemberAddressBookList.Where(x => x.DefaultSetting == 1).FirstOrDefault().ID;
                    //}
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(viewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddNewCompanyBook(CompanyBookVM companyBook)
        {
            bool IsSuccess = false;
            try
            {
                int getAccID = NEUser.ID;
                companyBook.Accountid = getAccID;
                CompanyBookDM addCompanyBook = ModelConverter.ConvertTo<CompanyBookDM>(companyBook);
                addCompanyBook.DefaultSetting = 1;
                CompanyBookVM resultAddressBook = Processor.Request<CompanyBookVM, CompanyBookDM>("GetMemberService", "AddCompanyBook", addCompanyBook).results;
                if (resultAddressBook != null)
                {
                    IsSuccess = true;
                }
                if (IsSuccess)
                {
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(companyBook.ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditCompanyBook(CompanyBookVM companyBook)
        {
            bool IsSuccess = false;
            try
            {
                int getAccID = NEUser.ID;
                companyBook.Accountid = getAccID;
                CompanyBookDM updateCompanyBook = ModelConverter.ConvertTo<CompanyBookDM>(companyBook);
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "ModifyCompanyBook", updateCompanyBook).results;
                //return Json(new { IsSuccess = IsSuccess, addressBook = addressBook }, JsonRequestBehavior.AllowGet);
                if (IsSuccess)
                {
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(companyBook.ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteCompanyBook(int companyBookID, string ViewPath)
        {
            bool IsSuccess = false;
            try
            {
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "DeleteCompanyBook", companyBookID).results;
                if (IsSuccess)
                {
                    int getAccID = NEUser.ID;
                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChangeCompanyBookDefault(int companyBookID, string ViewPath)
        {
            bool IsSuccess = false;
            int getAccID = NEUser.ID;
            try
            {
                IsSuccess = Processor.Request<bool, bool>("GetMemberService", "ChangeCompanyBookDefault", getAccID, companyBookID).results;
                if (IsSuccess)
                {

                    CartMemberInfoVM getCartMemberInfo = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID).results;
                    ViewBag.GetCartMemberInfo = getCartMemberInfo;
                    // 將View轉成string
                    string viewResult = RenderView(ViewPath);
                    return Json(new { IsSuccess = IsSuccess, viewHtml = viewResult }, JsonRequestBehavior.AllowGet);
                    //return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                logger.Error("Email[" + NEUser.Email + "] 修改[CompanyBook] 失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                return Json(new { IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        public string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }
    }
}
