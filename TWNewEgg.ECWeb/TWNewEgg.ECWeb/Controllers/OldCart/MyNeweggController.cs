using Newegg.Mobile.MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Redeem.Service;
using TWNewEgg.InternalSendMail.Service;
using System.Data;
using System.Data.SqlClient;
using System.Web.Util;
using TWNewEgg.GetConfigData.Service;
using TWNewEgg.DB;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.Website.ECWeb.Controllers
{
    /// <summary>
    /// 我的帳戶相關
    /// </summary>
    public class MyNeweggController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
        /// <summary>
        /// amount of order to show in each page for recentOrders
        /// </summary>
        // If this arg set to 1, then it will count the wrong total page value in paging function****************************** 2013/12/24 Bill Wu.
        const int recentOrderSpan = 4;

        /// <summary>
        /// object used to encrypt cookies
        /// </summary>
        AesCookies aesenc;

        // private const string neweggRequestUsername = "po_netw@newegg.com";
        // private const string neweggRequestPassword = "newegg61705";

        private TWSqlDBContext dbbefore = new TWSqlDBContext();
        private TWBackendDBContext dbafter = new TWBackendDBContext();
        private TWNewEgg.DB.TWBackendDBContext TWBdbafter = new DB.TWBackendDBContext();
        private TWNewEgg.DB.TWSqlDBContext TWBdbbefore = new DB.TWSqlDBContext();
        private IGetInfo CheckID = new GetInfoRepository();
        // string NewLinkTitle = "https://localhost:44300"; // 工程師測試信件用
        string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
        string ECDomain2 = System.Configuration.ConfigurationManager.AppSettings["ECSSLDomain"];
        // Mail
        string GP_TEAM_MKPL_LOGISTICS_TW = System.Configuration.ConfigurationManager.AppSettings["GP_TEAM_MKPL_LOGISTICS_TW"];
        string GP_TEAM_MKPL_CS_TW = System.Configuration.ConfigurationManager.AppSettings["GP_TEAM_MKPL_CS_TW"];
        string GP_TEAM_MKPL_CASHFLOW_TW = System.Configuration.ConfigurationManager.AppSettings["GP_TEAM_MKPL_CASHFLOW_TW"];

        //使用者登入資訊
        private string mstrAccountId = "";

        /// <summary>
        /// Used to check login status of present user from cookies encrypted by AesCookies.
        /// Called before every action.
        /// </summary>
        /// <returns>
        /// Return account id under this controller if the login status is valid otherwise 0.
        /// </returns>
        private int CheckAccount()
        {
            // 判斷Cookies是否存在;
            if (Request.Cookies["Accountid"] != null && Request.Cookies["LoginStatus"] != null)
            {
                string[] plainText = CheckID.Decoder(Request.Cookies["Accountid"].Value, false);
                if (plainText.Length < 2)
                {
                    return 0;
                }
                int loginStatus = 0;
                int accID = int.Parse(plainText[0]);
                plainText = CheckID.Decoder(Request.Cookies["LoginStatus"].Value, false);
                if (plainText.Length < 2)
                {
                    return 0;
                }
                int.TryParse(plainText[0], out loginStatus);
                bool? checkLogin = dbbefore.Account.FirstOrDefault(x => x.ID == accID && loginStatus == 1).LoginStatus == 1;
                if (checkLogin != null)
                {
                    this.mstrAccountId = Convert.ToString(accID);
                    return accID;
                }
            }
            return 0;
        }

        /// <summary>
        /// 我的帳戶頁
        /// </summary>
        /// <param name="err">回傳錯誤訊息</param>
        /// <returns></returns>
        public ActionResult myAccount(string err)
        {
            if (err != "")
            {
                ViewBag.err = err;
            }
            else { ViewBag.err = ""; }
            int accID = CheckAccount();
            if (accID > 0)
            {
                ViewBag.Name = dbbefore.Account.Where(i => i.ID == accID).Select(i => i.Name).First();
                return View();
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/myAccount" });
        }

        public ActionResult CompanyBook()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                List<CompanyBook> books = dbbefore.CompanyBook.Where(a => a.Accountid == accID).ToList();
                if (Request.IsAjaxRequest())
                {
                    ViewData.Model = books;
                    string partialView = "Partial_CompanyBook";
                    return Json(renderView(partialView), JsonRequestBehavior.AllowGet);
                }
                ViewBag.tabOrder = 4;
                ViewBag.Content = "Partial_CompanyBook";
                return PartialView("myAccountContent", books);
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" }) + "'");
        }

        private string renderView(string partialView)
        {
            string result;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }
            return result;
        }

        /// <summary>
        /// Delete specific CompanyBook.
        /// </summary>
        /// <param name="id">CompanyBook_id of addressbook to delete</param>
        /// <returns>redirect to CompanyBook page</returns>
        public ActionResult deleteCompany(int id)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                ViewBag.tabOrder = 4;

                CompanyBook book = dbbefore.CompanyBook.FirstOrDefault(i => i.ID == id);
                if (book.Accountid != accID)
                {
                    return RedirectToAction("Companybook");
                }
                if (book != null)
                {
                    dbbefore.CompanyBook.Remove(book);
                    dbbefore.SaveChanges();
                }
                List<CompanyBook> books = dbbefore.CompanyBook.Where(a => a.Accountid == accID).ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Partial_CompanyBook", books);
                }
                else if (ControllerContext.IsChildAction)
                {
                    ViewBag.Content = "Partial_CompanyBook";
                    return PartialView("myAccountContent", books);
                }
                else
                {
                    return RedirectToAction("CompanyBook", "myNewegg");
                }
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" }) + "'");
        }

        /// <summary>
        /// Update specific CompanyBook with parameter 'modefied'.
        /// The addressbook_id of target is that of 'modified'.
        /// </summary>
        /// <param name="modified">addressbook to update</param>
        /// <returns></returns>
        public ActionResult editCompany(CompanyBook modified)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                ViewBag.tabOrder = 4;

                CompanyBook book = dbbefore.CompanyBook.FirstOrDefault(a => a.ID == modified.ID);
                if (book.Accountid != accID)
                {
                    return RedirectToAction("Companybook");
                }
                if (book != null)
                {
                    book.Number = modified.Number;
                    book.Title = modified.Title;
                    dbbefore.SaveChanges();
                    return RedirectToAction("CompanyBook");
                }
                List<CompanyBook> books = dbbefore.CompanyBook.Where(a => a.Accountid == accID).ToList();

                if (Request.IsAjaxRequest())
                {
                    return PartialView("Partial_CompanyBook", books);
                }
                else if (ControllerContext.IsChildAction)
                {
                    ViewBag.Content = "Partial_CompanyBook";
                    return PartialView("myAccountContent", books);
                }
                else
                {
                    return RedirectToAction("CompanyBook", "myNewegg");
                }
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" }) + "'");
        }

        /// <summary>
        /// Insert parameter 'inserted' to CompanyBook.
        /// </summary>
        /// <param name="inserted">new CompanyBook to add to the database</param>
        /// <returns></returns>
        public ActionResult insertCompany(CompanyBook inserted)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                inserted.Createdate = DateTime.Now;
                inserted.Accountid = accID;
                inserted.Updated = 0;
                inserted.Delivaddr = "";
                inserted.Delivloc = "";
                inserted.Delivzip = "";
                if (ModelState.IsValid)
                {
                    dbbefore.CompanyBook.Add(inserted);
                    dbbefore.SaveChanges();
                }
                List<CompanyBook> books = dbbefore.CompanyBook.Where(a => a.Accountid == accID).ToList();

                if (Request.IsAjaxRequest())
                {
                    return PartialView("Partial_CompanyBook", books);
                }
                else if (ControllerContext.IsChildAction)
                {
                    ViewBag.Content = "Partial_CompanyBook";
                    return PartialView("myAccountContent", books);
                }
                else
                {
                    return RedirectToAction("CompanyBook", "myNewegg");
                }

            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/CompanyBook" }) + "'");
        }

        /// <summary>
        /// Set up email and epaper subscription status
        /// </summary>
        /// <returns>
        /// ViewBag.email is uesd to setup the default value of user email address.
        /// ViewBag.subscribe is the present subscription status.
        /// </returns>
        public ActionResult EPaper()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {

                Account acc = dbbefore.Account.FirstOrDefault(a => a.ID == accID);
                ViewBag.email = acc.Email;
                if (acc.MessagePaper == 1)
                {
                    ViewBag.Subscribe = true;
                    ViewBag.NotSubscribe = false;
                }
                else
                {
                    ViewBag.Subscribe = false;
                    ViewBag.NotSubscribe = true;
                }
                if (acc.ReceiveEDM == 1)
                {
                    ViewBag.ReceiveEDM = true;
                    ViewBag.NotReceiveEDM = false;
                }
                else
                {
                    ViewBag.ReceiveEDM = false;
                    ViewBag.NotReceiveEDM = true;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/EPaper" });
            }
        }

        /// <summary>
        /// Setup email and epaper subscription in database.
        /// </summary>
        /// <param name="subscribeOrNot">subscription status to set</param>
        /// <param name="emailAddress">email address to set</param>
        /// <returns> 
        /// If subscribeOrNot is null or false set subscribe to 1, which means no subscription.
        /// If subscribeOrNot is true then set subscribe to 0.
        /// </returns>
        [HttpPost]
        public ActionResult EPaper(bool subscribeOrNot, bool receiveEDMOrNot)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {

                aesenc = new AesCookies();
                Account account = dbbefore.Account.FirstOrDefault(a => a.ID == accID);
                int newMessagePaper = (subscribeOrNot == true) ? 1 : 0;
                int newReceiveEDM = (receiveEDMOrNot == true) ? 1 : 0;
                // 若設定有變更則顯示不同字句
                ViewBag.MessagePaper = (account.MessagePaper == newMessagePaper) ? false : true;
                ViewBag.ReceiveEDM = (account.ReceiveEDM == newReceiveEDM) ? false : true;

                account.MessagePaper = newMessagePaper;
                account.ReceiveEDM = newReceiveEDM;
                dbbefore.SaveChanges();

                return PartialView("EPaperSubmit");
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/EPaper" });
        }

        /// <summary>
        /// Set up member infomation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MemberInfo()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {

                TWSqlDBContext db = new TWSqlDBContext();

                Account acc = dbbefore.Account.FirstOrDefault(a => a.ID == accID);

                var result = (from ac in db.Account
                              where ac.ID == accID
                              join me in db.Member on ac.ID equals me.AccID
                              into memb
                              from me in memb.DefaultIfEmpty()
                              select new
                              {
                                  AccID = ac.ID,
                                  Sex = me.Sex,
                                  Firstname = me.Firstname,
                                  Lastname = me.Lastname,
                                  Nickname = me.Nickname,
                                  Firstname_en = me.Firstname_en,
                                  Lastname_en = me.Lastname_en,
                                  Birthday = me.Birthday,
                                  Mobile = me.Mobile,
                                  TelZip = me.TelZip,
                                  TelDay = me.TelDay,
                                  TelExtension = me.TelExtension,
                                  Loc = me.Loc,
                                  Zip = me.Zip,
                                  Zipname = me.Zipname,
                                  Address = me.Address,
                                  Address_en = me.Address_en
                              }).ToList();

                List<Member> mem = result.ToList().ConvertAll<Member>(item => new Member()
                {
                    AccID = item.AccID,
                    Sex = item.Sex,
                    Firstname = item.Firstname,
                    Lastname = item.Lastname,
                    Nickname = item.Nickname,
                    Firstname_en = item.Firstname_en,
                    Lastname_en = item.Lastname_en,
                    Birthday = item.Birthday,
                    Mobile = item.Mobile,
                    TelZip = item.TelZip,
                    TelDay = item.TelDay,
                    TelExtension = item.TelExtension,
                    Loc = item.Loc,
                    Zip = item.Zip,
                    Zipname = item.Zipname,
                    Address = item.Address,
                    Address_en = item.Address_en
                });

                return View(mem[0]);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/MemberInfo" });
            }
        }

        [HttpPost]
        public ActionResult MemberInfo(Member sMember)
        {
            int accID = CheckAccount();

            try
            {
                if (accID > 0)
                {
                    Member mem = dbbefore.Member.FirstOrDefault(a => a.AccID == accID);
                    if (mem != null)
                    {
                        mem.Sex = sMember.Sex;
                        mem.Firstname = sMember.Firstname;
                        mem.Lastname = sMember.Lastname;
                        mem.Nickname = sMember.Nickname;
                        mem.Firstname_en = sMember.Firstname_en;
                        mem.Lastname_en = sMember.Lastname_en;
                        mem.Birthday = sMember.Birthday;
                        mem.Mobile = sMember.Mobile;
                        mem.TelZip = sMember.TelZip;
                        mem.TelDay = sMember.TelDay;
                        mem.TelExtension = sMember.TelExtension;
                        mem.Loc = sMember.Loc;
                        mem.Zip = sMember.Zip;
                        mem.Zipname = sMember.Zipname;
                        mem.Address = sMember.Address;
                        mem.Address_en = sMember.Address_en;
                        mem.ModifyDate = DateTime.Now;

                        dbbefore.SaveChanges();
                    }
                    else
                    {
                        sMember.AccID = accID;
                        sMember.CreateDate = DateTime.Now;
                        sMember.ModifyDate = DateTime.Now;

                        dbbefore.Member.Add(sMember);
                        dbbefore.SaveChanges();
                    }
                }

                TempData["Message"] = "會員基本資料修改成功!";

                Member memO = dbbefore.Member.FirstOrDefault(a => a.AccID == accID);

                return View(memO);

            }
            catch (Exception e)
            {
                string errstr = e.Message;

                TempData["Message"] = "會員基本資料修改失敗!";

                Member memO = dbbefore.Member.FirstOrDefault(a => a.AccID == accID);

                return View(memO);

            }
        }

        /// <summary>
        /// List the addressbooks of present account.
        /// </summary>
        /// <returns></returns>
        //public ActionResult AddressBook()
        //{
        //    int accID = CheckAccount();
        //    if (accID > 0)
        //    {

        //        List<Addressbook> books = dbbefore.Addressbook.Where(a => a.AccountID == accID).OrderBy(a => a.ID).ToList();

        //        if (Request.IsAjaxRequest())
        //        {
        //            ViewData.Model = books;
        //            string partialView = "Partial_AddressBook";
        //            return Json(renderView(partialView), JsonRequestBehavior.AllowGet);
        //        }
        //        ViewBag.tabOrder = 3;
        //        ViewBag.Content = "Partial_AddressBook";
        //        return PartialView("myAccountContent", books);
        //    }
        //    else if (!Request.IsAjaxRequest())
        //    {
        //        return RedirectToAction("Login", "account", new { returnUrl = "/myNewegg/AddressBook" });
        //    }
        //    else
        //        return JavaScript("window.location = '" + Url.Action("Login", "account", new { returnUrl = "/myNewegg/AddressBook" }) + "'");
        //}

        /// <summary>
        /// Delete specific addressbook.
        /// </summary>
        /// <param name="id">addressbook_id of addressbook to delete</param>
        /// <returns>redirect to addressbook page</returns>
        //public ActionResult deleteAddress(int id)
        //{
        //    int accID = CheckAccount();
        //    if (accID > 0)
        //    {
        //        ViewBag.tabOrder = 3;

        //        Addressbook book = dbbefore.Addressbook.FirstOrDefault(i => i.ID == id);
        //        if (book.AccountID != accID)
        //        {
        //            return RedirectToAction("AddressBook");
        //        }
        //        if (book != null)
        //        {
        //            dbbefore.Addressbook.Remove(book);
        //            dbbefore.SaveChanges();
        //        }
        //        return RedirectToAction("AddressBook");
        //    }
        //    else if (!Request.IsAjaxRequest())
        //    {
        //        return RedirectToAction("Login", "account", new { returnUrl = "/myNewegg/AddressBook" });
        //    }
        //    else
        //        return JavaScript("window.location = '" + Url.Action("Login", "account", new { returnUrl = "/myNewegg/AddressBook" }) + "'");
        //}

        /// <summary>
        /// Update specific addressbook to parameter 'modefied'.
        /// The addressbook_id of target is the id of 'modified' addressbook.
        /// The recvtelday of 'modefied' is combined with parameter 'recvtelday0' and parameter 'recvtelday2' to retain the complete phone number.
        /// Remove the special characters in phone number of addressbook.
        /// </summary>
        /// <param name="modified">addressbook to update</param>
        /// <param name="recvtelday0">Phone number prefixes. For example, 02 in 02-8006-8010.</param>
        /// <param name="recvtelday2">Ext of Phone number. For example, 61721 in 02-8006-8010#61721 .</param>
        /// <returns></returns>
        //public ActionResult editAddress(Addressbook modified, string recvtelday0, string recvtelday2, string delivzipTag)
        //{
        //    int accID = CheckAccount();
        //    if (accID > 0)
        //    {
        //        ViewBag.tabOrder = 3;

        //        Addressbook book = dbbefore.Addressbook.FirstOrDefault(a => a.ID == modified.ID);
        //        if (book.AccountID != accID)
        //        {
        //            return RedirectToAction("AddressBook");
        //        }
        //        if (book != null)
        //        {
        //            modified.DelivAddr = delivzipTag + modified.DelivAddr;
        //            book.RecvSex = modified.RecvSex;
        //            book.RecvLastName = modified.RecvLastName;
        //            book.RecvFirstName = modified.RecvFirstName;
        //            book.RecvName = modified.RecvLastName + modified.RecvFirstName;
        //            book.DelivLOC = modified.DelivLOC;
        //            book.DelivZip = modified.DelivZip;
        //            book.DelivAddr = modified.DelivAddr;
        //            book.RecvTelDay = (recvtelday0.Length + (modified.RecvTelDay == null ? 0 : modified.RecvTelDay.Length)) > 0 ? (string.IsNullOrEmpty(recvtelday2)) ? ('(' + recvtelday0 + ')' + modified.RecvTelDay) : ('(' + recvtelday0 + ')' + modified.RecvTelDay + "#" + recvtelday2) : "";
        //            book.RecvTelDay = book.RecvTelDay.Trim(new char[] { ' ', '-' });
        //            book.RecvMobile = (modified.RecvMobile == null) ? "" : modified.RecvMobile.Trim(new char[] { ' ', '-', '(', ')' });

        //            dbbefore.SaveChanges();
        //            return RedirectToAction("AddressBook");
        //        }
        //        return RedirectToAction("AddressBook");
        //    }
        //    else if (!Request.IsAjaxRequest())
        //    {
        //        return RedirectToAction("Login", "account", new { returnUrl = "/myNewegg/AddressBook" });
        //    }
        //    else
        //        return JavaScript("window.location = '" + Url.Action("Login", "account", new { returnUrl = "/myNewegg/AddressBook" }) + "'");
        //}

        /// <summary>
        /// Insert parameter 'inserted' to addressbook.
        /// The recvtelday of 'inserted' is combined with parameter 'recvtelday0' and parameter 'recvtelday2' to retain the complete phone number.
        /// Remove the special characters in phone number of addressbook.
        /// </summary>
        /// <param name="inserted">new addressbook to add to the database</param>
        /// <param name="recvtelday0">Phone number prefixes. For example, 02 in 02-8006-8010.</param>
        /// <param name="recvtelday2">Ext of Phone number. For example, 61721 in 02-8006-8010#61721 .</param>
        /// <returns></returns>
        //public ActionResult insertAddress(Addressbook inserted, string recvtelday0, string recvtelday2, string delivzipTag)
        //{
        //    int accID = CheckAccount();
        //    if (accID > 0)
        //    {
        //        try
        //        {
        //            inserted.DelivAddr = delivzipTag + inserted.DelivAddr;
        //            inserted.RecvName = inserted.RecvLastName + inserted.RecvFirstName;
        //            inserted.RecvTelDay = (recvtelday0.Length + (inserted.RecvTelDay == null ? 0 : inserted.RecvTelDay.Length)) > 0 ? (string.IsNullOrEmpty(recvtelday2)) ? ('(' + recvtelday0 + ')' + inserted.RecvTelDay) : ('(' + recvtelday0 + ')' + inserted.RecvTelDay + "#" + recvtelday2) : "";
        //            inserted.RecvTelDay = inserted.RecvTelDay.Trim(new char[] { ' ', '-' });
        //            inserted.RecvMobile = (inserted.RecvMobile == null) ? "" : inserted.RecvMobile.Trim(new char[] { ' ', '-', '(', ')' });
        //            inserted.CreateDate = DateTime.Now;
        //            inserted.AccountID = accID;
        //            inserted.Updated = 0;
        //            inserted.UpdateDate = DateTime.Now;
        //            inserted.AccountEmail = "";

        //            dbbefore.Addressbook.Add(inserted);
        //            dbbefore.SaveChanges();
        //        }
        //        catch { }
        //        return RedirectToAction("AddressBook");
        //    }
        //    else if (!Request.IsAjaxRequest())
        //    {
        //        return RedirectToAction("Login", "account", new { returnUrl = "/myNewegg/AddressBook" });
        //    }
        //    else
        //        return JavaScript("window.location = '" + Url.Action("Login", "account", new { returnUrl = "/myNewegg/AddressBook" }) + "'");
        //}

        /// <summary>
        /// Get the salesorder content and all the salesorderitem of the salesOrder with salesOrder_code == parameter 'salesOrderCode' 
        /// </summary>
        /// <param name="salesOrderCode">salesorder_code of the salesorder to return</param>
        /// <returns></returns>
        public ActionResult orderContent(string salesOrderCode)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                SalesOrder so = dbbefore.SalesOrder.Single(s => s.Code == salesOrderCode);
                if (so.AccountID == accID)
                {
                    int salesOrderGroupID = (int)so.SalesOrderGroupID;
                    List<SalesOrder> salesorders = dbbefore.SalesOrder.Where(s => s.SalesOrderGroupID == salesOrderGroupID).ToList();
                    List<Cart> carts = dbafter.Cart.Where(c => c.SalesorderGroupID == salesOrderGroupID).ToList();
                    List<RecentOrderItem> ROI;
                    Cart cart = carts.FirstOrDefault(c => c.ID == salesOrderCode);
                    InvoiceList invoiceList = dbafter.InvoiceList.FirstOrDefault(i => i.SONumber == salesOrderCode);

                    if (cart != null)
                    {
                        if (salesorders.Any(s => s.DelivType == 3) && carts.Any(c => !string.IsNullOrWhiteSpace(c.InvoiceNO)) && invoiceList != null)
                        {
                            cart.InvoiceNO = invoiceList.InvoiceNumber;
                        }

                        ViewBag.so = new SalesOrder(cart);
                    }
                    else
                    {
                        ViewBag.so = so;
                    }
                    ROI = setROI(accID, salesorders);
                    List<Deliver> deliver = dbafter.Deliver.ToList();

                    PurchaseOrderTWBACK po = dbafter.PurchaseOrderTWBACK.FirstOrDefault(p => p.SalesorderCode == salesOrderCode);
                    List<PurchaseOrderitemTWBACK> POIs = (po != null) ? dbafter.PurchaseOrderitemTWBACK.Where(p => p.PurchaseorderCode == po.Code).ToList() : new List<PurchaseOrderitemTWBACK>();
                    foreach (var item in ROI)
                    {
                        PurchaseOrderitemTWBACK poi = POIs.FirstOrDefault(p => p.ItemID == item.ItemID);
                        if (poi != null)
                        {
                            switch (item.salesorder_delivtype)
                            {
                                case 4:

                                    break;
                                case 6:
                                    decimal? taxFee = new decimal();

                                    if (item.ItemlistID == 0)
                                    {
                                        taxFee = dbbefore.Item.Where(x => x.ID == item.ItemID).Select(y => y.Taxfee).FirstOrDefault();
                                    }

                                    if (taxFee != null)
                                    {
                                        item.tax = taxFee;
                                    }
                                    break;
                                default:
                                    item.tax = 0;
                                    break;
                            }
                        }
                        else
                        {
                            switch (item.salesorder_delivtype)
                            {
                                case 4:

                                    break;
                                case 6:
                                    decimal? taxFee = new decimal();
                                    if (item.ItemlistID == 0)
                                    {
                                        taxFee = dbbefore.Item.Where(x => x.ID == item.ItemID).Select(y => y.Taxfee).FirstOrDefault();
                                    }

                                    if (taxFee != null)
                                    {
                                        item.tax = taxFee;
                                    }
                                    break;
                                default:
                                    item.tax = 0;
                                    break;
                            }
                        }
                        item.salesorder_delivtype = salesorders.Where(x => x.Code == item.SalesorderCode).First().DelivType;

                        if (salesorders[0].DelivType == 3 && po != null)
                        {
                            item.TrackNo = po.ForwardNO;
                            item.delivnoLink = "http://web.ucf.com.tw/wiznet/querycwb.php";

                        }
                        else
                        {
                            item.TrackNo = item.process_delivno;
                            try
                            {
                                if (string.IsNullOrEmpty(cart.Deliv4NO) == true)
                                {
                                    item.delivnoLink = deliver.Where(x => x.code == item.process_deliver).First().WebSite;
                                    item.process_delivno = deliver.Where(x => x.code == item.process_deliver).First().Name;
                                }
                                else
                                {

                                    item.delivnoLink = deliver.Where(x => x.code == cart.O2OForwarder).First().WebSite;
                                    item.process_delivno = deliver.Where(x => x.code == cart.O2OForwarder).First().Name;
                                }
                            }
                            catch { item.delivnoLink = "http://www.hct.com.tw/SearchGoods.aspx"; }
                        }
                    }
                    int? paytype = salesorders[0].PayType;
                    string cardbank = salesorders[0].CardBank;
                    PayType pay = dbbefore.PayType.FirstOrDefault(p => p.PayType0rateNum == paytype) ?? new PayType();
                    Bank bank = dbbefore.Bank.FirstOrDefault(b => b.Code == cardbank) ?? new Bank();

                    if ((pay.PayType0rateNum > 1 && pay.PayType0rateNum < 25) || (pay.PayType0rateNum > 101 && pay.PayType0rateNum < 125))
                    {
                        if (bank.Code == "021")
                        {
                            ViewBag.payType = bank.Name + (pay.Name ?? "");
                        }
                        else
                        {
                            ViewBag.payType = "信用卡" + (pay.Name ?? "");
                        }
                    }
                    else
                    {
                        ViewBag.payType = pay.Name;
                    }

                    return View(ROI);
                }
                else
                {
                    return RedirectToAction("recentOrders");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/recentOrders" });
            }
        }


        /// <summary>
        /// If not logged in yet, redirect to the login page.
        /// If it's ajax request, return PartialView. Else return myAccouontContent.
        /// </summary>
        /// <param name="length">total count of salesorder_grouop of this account, calculated when this action called for the first time</param>
        /// <param name="start">the order of requested data order by salesorder_groupid</param>
        /// <returns></returns>
        public ActionResult recentOrders(int? length, int start = 0)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                int count = length ?? dbbefore.SalesOrder.Where(s => s.AccountID == accID).Select(s => s.SalesOrderGroupID).Distinct().Count();
                start = start / recentOrderSpan * recentOrderSpan;
                if (start >= count)
                {
                    start = (count - 1) / recentOrderSpan * recentOrderSpan;
                }
                if (start < 0)
                {
                    start = 0;
                }
                ViewBag.tabOrder = 0;
                ViewBag.length = count;
                ViewBag.span = recentOrderSpan;
                ViewBag.start = start;
                bool ajax = Request.IsAjaxRequest();

                List<SalesOrder> salesOrdersGroup = dbbefore.SalesOrder.Where(s => s.AccountID == accID).GroupBy(s => s.SalesOrderGroupID)
                    .OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).Skip(start).Take(recentOrderSpan).SelectMany(s => s).ToList();

                var model = setROI(accID, salesOrdersGroup).OrderByDescending(r => r.salesorder_groupid).ThenBy(r => r.seller_country).ThenBy(r => r.status).ThenByDescending(r => r.TrackNo).ThenByDescending(r => r.SalesorderCode).ToList();

                ViewBag.tabOrder = 0;

                if (Request.IsAjaxRequest())
                {
                    ViewData.Model = model;
                    string partialView = "Partial_RecentOrder";
                    return Json(renderView(partialView), JsonRequestBehavior.AllowGet);
                }
                ViewBag.Content = "Partial_RecentOrder";
                return View("myAccountContent", model);
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/recentOrders" });
            }
            else
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/recentOrders" }) + "'");
        }


        private List<RecentOrderItem> setROI(int accID, List<SalesOrder> salesOrdersGroup)
        {
            DateTime dateTimeNow = DateTime.Now;
            List<SalesOrder> fromCartGroup = new List<SalesOrder>();
            List<List<IGrouping<int?, SalesOrder>>> allGroup = new List<List<IGrouping<int?, SalesOrder>>>();
            List<Item> items = new List<Item>();
            List<string> eachSOCode = new List<string>();
            List<SalesOrderItem> salesorderitems;
            List<Process> processes;
            List<RecentOrderItem> ROItems = new List<RecentOrderItem>();
            string accIDstring = accID.ToString();

            var groupIDs = salesOrdersGroup.Select(s => s.SalesOrderGroupID).Distinct().ToList();
            var carts = dbafter.Cart.Where(c => groupIDs.Contains(c.SalesorderGroupID)).OrderBy(c => c.ID).ToList();
            var fromSOGroup = salesOrdersGroup.Except((from c in carts
                                                       join s in salesOrdersGroup on c.ID equals s.Code into a
                                                       from b in a
                                                       select b)).ToList();

            foreach (var cart in carts)
            {
                fromCartGroup.Add(new SalesOrder(cart));
            }

            allGroup.Add(fromSOGroup.GroupBy(s => s.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
            allGroup.Add(fromCartGroup.GroupBy(c => c.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
            eachSOCode = allGroup.SelectMany(g => g).SelectMany(s => s).Where(s => s.Note != "國際運費" && s.Note != "服務費").Select(s => s.Code).ToList();
            salesorderitems = dbbefore.SalesOrderItem.Where(s => eachSOCode.Contains(s.SalesorderCode)).ToList();
            processes = dbafter.Process.Where(p => eachSOCode.Contains(p.CartID)).ToList();
            var productIDs = salesorderitems.Select(s => (s.ProductID > 0) ? s.ProductID : s.ProductlistID).ToList();
            var products = dbbefore.Product.Where(p => productIDs.Contains(p.ID)).ToList();
            var sellerIDs = products.Select(p => p.SellerID).ToList();
            var sellers = dbbefore.Seller.Where(s => sellerIDs.Contains(s.ID)).ToList();
            var countryIds = sellers.Select(s => s.CountryID).ToList();
            var countrys = dbbefore.Country.Where(s => countryIds.Contains(s.ID)).ToList();

            var POs = dbafter.PurchaseOrderTWBACK.Where(p => eachSOCode.Contains(p.SalesorderCode)).ToList();

            var PONumbers = POs.Select(p => p.Code).ToList();
            var POIs = dbafter.PurchaseOrderitemTWBACK.Where(p => PONumbers.Contains(p.PurchaseorderCode)).ToList();

            var payTypes = dbbefore.PayType.ToList();

            for (int k = 0; k < allGroup.Count(); k++)
            {
                foreach (var SOGroup in allGroup[k])
                {

                    bool isCart = k > 0;
                    var socodes = SOGroup.Select(s => s.Code).ToList();
                    List<RecentOrderItem> ros = new List<RecentOrderItem>();
                    if (isCart)
                    {
                        List<Process> tempProc = processes.Where(p => socodes.Contains(p.CartID)).ToList();
                        foreach (var proc in tempProc)
                        {
                            ros.Add(new RecentOrderItem(proc));
                        }
                    }
                    else
                    {
                        List<SalesOrderItem> tempSOI = salesorderitems.Where(p => socodes.Contains(p.SalesorderCode)).ToList();
                        foreach (var soi in tempSOI)
                        {
                            ros.Add(new RecentOrderItem(soi));
                        }
                    }


                    for (int i = 0; i < ros.Count(); i++)
                    {

                        var r = ros[i];

                        Product product = products.Single(p => p.ID == r.ProductID);

                        Cart cart = carts.FirstOrDefault(c => c.ID == r.SalesorderCode);
                        r.product_sellerid = product.SellerID;

                        r.seller_country = sellers.Single(s => s.ID == product.SellerID).CountryID ?? 0;

                        r.country_name = countrys.Single(c => c.ID == r.seller_country).countryName;

                        r.salesorder_delivtype = SOGroup.FirstOrDefault(g => g.Code == r.SalesorderCode).DelivType;

                        r.salesorder_groupid = SOGroup.First(g => g.Code == r.SalesorderCode).SalesOrderGroupID ?? 0;

                        r.fromCart = isCart;
                        var purchaseOrder = POs.FirstOrDefault(p => p.SalesorderCode == r.SalesorderCode) ?? new PurchaseOrderTWBACK();
                        var purchaseOrderItem = POIs.Where(p => p.PurchaseorderCode == purchaseOrder.Code).ToList() ?? new List<PurchaseOrderitemTWBACK>();
                        if (r.salesorder_delivtype == 3)
                        {
                            r.TrackNo = purchaseOrder.ForwardNO;
                        }
                        else if (cart != null)
                        {
                            r.O2OShopDeliveryDate = cart.O2OShopDeliveryDate.ToString() ?? "";
                            r.TrackNo = string.IsNullOrEmpty(cart.Deliv4NO) ? cart.DelivNO : cart.Deliv4NO;
                            r.process_delivno = string.IsNullOrEmpty(cart.Deliv4NO) ? cart.DelivNO : cart.Deliv4NO;
                        }


                        SalesOrder so = SOGroup.Single(g => g.Code == r.SalesorderCode);
                        // 若有訂單存在
                        if (isCart)
                        {
                            // 訂單正常(Status = 0 || Status = 99)進入下列判斷,其他則判斷定單狀態
                            if (so.Status == 0 || so.Status == 99)
                            {
                                if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                                {
                                    // 待可自動回填客戶收貨日期補上條件,已送達
                                    r.status = 5;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                                {
                                    // 已出貨
                                    r.status = 4;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.待出貨)
                                {
                                    // 商品押上發票號碼,出貨中
                                    r.status = 3;
                                }
                                else if (cart != null && (r.salesorder_delivtype == 0 || r.salesorder_delivtype == 2 || r.salesorder_delivtype == 7) && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立)
                                {
                                    // 切貨商品初始狀態,已成立
                                    r.status = 1;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.空運中)
                                {
                                    // 空運中
                                    r.status = 2;
                                }
                                else if (cart != null && r.salesorder_delivtype == 6 && (cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.初始狀態 || cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立))
                                {
                                    // 海外切貨商品初始狀態,空運中
                                    r.status = 2;
                                }
                                else if (purchaseOrder.DelvStatus == 1 || purchaseOrder.DelvStatus == 0)
                                {
                                    // 空運中
                                    r.status = 2;
                                }
                                else if (purchaseOrder.DelvStatus == 999)
                                {
                                    // 拋單成功 Tracking#2生成,上飛機, WH60尚未收貨前
                                    if (!string.IsNullOrEmpty(purchaseOrder.ForwardNO))
                                    {
                                        // 空運中
                                        r.status = 2;
                                    }
                                    else if (!purchaseOrderItem.Any(p => string.IsNullOrEmpty(p.SellerorderCode)))
                                    {
                                        // 訂單成立
                                        r.status = 1;
                                    }
                                    else if (purchaseOrderItem.Any(p => string.IsNullOrEmpty(p.SellerorderCode)))
                                    {
                                        // 確認中
                                        r.status = 0;
                                    }
                                    else if (!string.IsNullOrEmpty(purchaseOrder.DELIVNO) || !products.Single(p => p.ID == r.ProductID).SellerProductID.StartsWith("9SI"))
                                    {
                                        // 回押tracking#1或為自營商品,訂單成立
                                        r.status = 1;
                                    }
                                }
                                else
                                {
                                    // status = 0,確認中->統統改成訂單成立
                                    r.status = 1;
                                }

                            }
                            else if (so.Status == 5)
                            {
                                // 已退貨
                                r.status = 6;
                                if (cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                                {
                                    // 無法配達，需要已出貨並且已退貨的狀態 (只針對貨到付款)
                                    r.status = -3;
                                }
                            }
                            else if (so.Status == 7 && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                            {
                                // 已配達
                                r.status = 5;
                            }
                            else if (so.Status == 7)
                            {
                                // 已完成但未壓配達, 所以不能申請退貨, 狀態為已出貨
                                r.status = 4;
                            }
                            else if (so.Status == 1 || so.Status == 2)
                            {
                                // 已取消
                                r.status = -1;
                            }

                            // Find Invoice Number from backend DB
                            var invoiceInfo = dbafter.InvoiceList.Where(x => x.SONumber == r.SalesorderCode).FirstOrDefault();
                            if (invoiceInfo != null)
                            {
                                r.InvoiceNumber = invoiceInfo.InvoiceNumber;
                                r.InvoiceInDate = invoiceInfo.InDate;
                            }
                            else
                            {
                                r.InvoiceNumber = "";
                                r.InvoiceInDate = null;
                            }

                        }
                        else
                        {
                            // check salesorder status after 6 mins.
                            if (DateTime.Compare(so.CreateDate.AddMinutes(6), dateTimeNow) < 0)
                            {
                                if (so.Status == 99)
                                {
                                    // paid fail, and cancel.
                                    r.status = -4;
                                }
                                else if (so.Status == 0)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = 34;
                                }
                                else if (so.Status == 1)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -1;
                                }
                                else if (so.Status == 2)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -4;
                                }
                                else if (so.Status == 3)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -1;
                                }
                                else
                                {
                                    // other reason, then so error.
                                    r.status = 34;
                                }
                            }
                            else
                            {
                                if (so.Status == 99)
                                {
                                    // confirm this salesorder in db, paid failed, show cancel this order.
                                    r.status = 33;
                                }
                                else if (so.Status == 0)
                                {
                                    // r.status = 30; paid success.
                                    if (payTypes.Where(x => x.PayType0rateNum == so.PayType).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == 31).FirstOrDefault())
                                    {
                                        // paid success, but backend db didn't create cart and process.
                                        r.status = 31;
                                    }
                                    else
                                    {
                                        // paid success, but backend db didn't create cart and process.
                                        r.status = 30;
                                    }
                                }
                            }
                        }
                        ROItems.Add(r);
                    }
                }
            }

            // 已退貨的resultCart加上retStatus
            var canceled = ROItems.Where(r => r.status == 6).ToList();
            var retCartProcIds = canceled.Select(p => new { salesorderitem_salesordercode = p.SalesorderCode, salesorderitem_code = p.Code }).ToList();

            Dictionary<Retgood, string> retgoodDic = new Dictionary<Retgood, string>();
            var returnedProcID = retCartProcIds.Select(r => r.salesorderitem_code).ToList();
            var retgoods = dbafter.Retgood.Where(r => returnedProcID.Contains(r.ProcessID)).ToList();
            foreach (var ret in retgoods)
            {
                retgoodDic.Add(ret, retCartProcIds.Single(r => r.salesorderitem_code == ret.ProcessID).salesorderitem_salesordercode);
            }
            for (int i = 0; i < canceled.Count(); i++)
            {
                var retstatus = retgoodDic.Where(r => r.Value == canceled[i].SalesorderCode).LastOrDefault().Key.Status;
                canceled[i].retStatus = retstatus;
            }

            // 已進入退款的resultCart, retStatus看refund
            var refunding = ROItems.Where(r => r.retStatus == 99).ToList();
            var refCartProcIds = canceled.Select(p => new { salesorderitem_salesordercode = p.SalesorderCode, salesorderitem_code = p.Code }).ToList();

            Dictionary<refund2c, string> refundDic = new Dictionary<refund2c, string>();
            var refundingProcID = refCartProcIds.Select(r => r.salesorderitem_code).ToList();
            var refunds = dbafter.refund2c.Where(r => refundingProcID.Contains(r.ProcessID)).ToList();
            foreach (var rf in refunds)
            {
                refundDic.Add(rf, refCartProcIds.Single(r => r.salesorderitem_code == rf.ProcessID).salesorderitem_salesordercode);
            }
            for (int i = 0; i < refunding.Count(); i++)
            {
                var retstatus = refundDic.Single(r => r.Value == refunding[i].SalesorderCode).Key.Status;
                refunding[i].retStatus = retstatus;
            }

            var prob = dbbefore.Problem.Where(p => eachSOCode.Contains(p.BlngCode)).Select(p => new { prblm_blngcode = p.BlngCode, prblm_code = p.Code }).ToList();
            foreach (var code in prob)
            {
                var temp = ROItems.Where(r => r.SalesorderCode == code.prblm_blngcode).ToList();
                foreach (var item in temp)
                {
                    item.prblm_prblmcode = code.prblm_code;
                }
            }

            return ROItems;
        }

        private void GroupResultCart(IGrouping<int?, SalesOrder> SOGroup, bool isCart, out List<ResultCart> singleCheckout, out List<RecentOrderItem> soItems)
        {
            soItems = getSalesOrderItem(SOGroup.ToList(), isCart);
            try
            {
                singleCheckout = SetResultCart(SOGroup.ToList(), soItems).OrderByDescending(s => s.itemSONumber).OrderBy(s => s.itemCountry).ThenBy(s => s.itemSellerID).ToList();
            }
            catch (NullReferenceException e)
            {
                Thread.Sleep(1000);
                singleCheckout = SetResultCart(SOGroup.ToList(), soItems).OrderByDescending(s => s.itemSONumber).OrderBy(s => s.itemCountry).ThenBy(s => s.itemSellerID).ToList();
            }

            for (int i = singleCheckout.Count() - 1; i >= 0; i--)
            {
                if (singleCheckout[i].itemList != null)
                {
                    foreach (var itemlist in singleCheckout[i].itemList)
                    {
                        itemlist.itemID = 0;
                    }
                    singleCheckout.InsertRange(i + 1, singleCheckout[i].itemList);
                }
            }
        }

        /// <summary>
        /// take salesorderitem corresponding to the sogroup form either process or salesorderitem 
        /// </summary>
        /// <param name="sogroup">訂單SO List</param>
        /// <param name="isCart">是否有訂單存在</param>
        /// <returns>soItems</returns>
        private List<RecentOrderItem> getSalesOrderItem(List<SalesOrder> SOGroup, bool isCart = false)
        {
            List<RecentOrderItem> soItems = new List<RecentOrderItem>();
            List<string> SOCodes = SOGroup.Select(s => s.Code).ToList();
            if (isCart)
            {
                var Processes = dbafter.Process.Where(p => SOCodes.Contains(p.CartID) && p.OrderNote != "國際運費" && p.OrderNote != "服務費").ToList();
                foreach (var p in Processes)
                {
                    soItems.Add(new RecentOrderItem(p));
                }
            }
            else
            {
                var SOIs = dbbefore.SalesOrderItem.Where(soi => SOCodes.Contains(soi.SalesorderCode) && soi.Note != "國際運費" && soi.Note != "服務費").ToList();
                foreach (var item in SOIs)
                {
                    soItems.Add(new RecentOrderItem(item));
                }
            }
            if (soItems.Count() == 0)
            {
                throw new Exception("Can't match cart and process");
            }
            return soItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sogroup"></param>
        /// <param name="soItems"></param>
        /// <returns></returns>
        private List<ResultCart> SetResultCart(List<SalesOrder> SOGroup, List<RecentOrderItem> soItems)
        {
            var ResultsOri = SOGroup.Join(soItems, s => s.Code, i => i.SalesorderCode, (s, i) => new { so = s, soi = i }).ToList();

            var allSalesOrder = ResultsOri.Select(x => new { salesorder_code = x.so.Code, salesorderitem_itemid = x.soi.ItemID }).Distinct().ToList();

            List<int> allSalesOrderItemID = allSalesOrder.Select(x => x.salesorderitem_itemid).Distinct().ToList();
            var itemIDSellerID = dbbefore.Item.Where(y => allSalesOrderItemID.Contains(y.ID)).Select(z => new { item_sellerid = z.SellerID, item_id = z.ID }).ToList(); //Get and Find all itemIDs in one SellerID.
            List<int> allSellerID = itemIDSellerID.Select(x => x.item_sellerid).ToList(); //Read all SellerID.
            List<Seller> allSellers = dbbefore.Seller.Where(x => allSellerID.Contains(x.ID)).ToList(); //Get all SellerID's Detail Data.
            List<Nullable<int>> allCountryID = allSellers.Select(y => y.CountryID).ToList(); //Read all CountryID.
            List<Country> allCountries = dbbefore.Country.Where(x => allCountryID.Contains(x.ID)).ToList(); //Get all CountryID's Detail Data.

            List<InsertSalesOrdersBySellerOutput> ResultsTemp;
            ResultsService CallService = new ResultsService();
            List<ResultCart> Result = new List<ResultCart>();
            List<BuyingItems> ResultItems = new List<BuyingItems>();
            List<int> sameSellerItem;
            List<string> sameSellerCode;

            foreach (Seller seller in allSellers) //Caculate all item in each seller.
            {
                sameSellerItem = itemIDSellerID.Where(x => x.item_sellerid == seller.ID).Select(y => y.item_id).ToList(); //Find all items which in same seller.
                sameSellerCode = allSalesOrder.Where(x => sameSellerItem.Contains(x.salesorderitem_itemid)).Select(y => y.salesorder_code).ToList(); //Find the items in same seller's salesorder_code.
                var toSet = ResultsOri.Where(x => sameSellerCode.Contains(x.so.Code)).ToList();
                ResultsTemp = addSalesOrderExp(toSet.Select(a => a.so).ToList(), toSet.Select(a => a.soi).ToList()); //Using salesorder_code to Find Result's Item.
                CallService.SetInResults(ResultsTemp, Result, sameSellerCode, seller, new Dictionary<string, int>(), ResultItems, allCountries); //Start to Caculate and find this Result's Data.
            }

            ICarts CountShipping = new CartsRepository();
            TWNewEgg.ItemService.Models.ShipTaxService TaxShip = CountShipping.ShippingCosts(ResultItems); //Call and Get Local price, tax and service fee from stored procedure in DB.
            CallService.SetResultsItem(Result, TaxShip.ShippingTaxCost, TaxShip.ShippingCost); //Combine SP and Result. Also count total service fee.

            return Result;
        }

        private List<InsertSalesOrdersBySellerOutput> addSalesOrderExp(List<SalesOrder> inputSO, List<RecentOrderItem> inputSOI)
        {
            List<InsertSalesOrdersBySellerOutput> results = new List<InsertSalesOrdersBySellerOutput>();
            for (int i = 0; i < inputSO.Count(); i++)
            {
                results.Add(new InsertSalesOrdersBySellerOutput()
                {
                    salesorder_code = inputSO[i].Code,
                    salesorder_delivtype = inputSO[i].DelivType,
                    salesorderitem_salesordercode = inputSOI[i].Code,
                    salesorderitem_attribs = inputSOI[i].Attribs,
                    salesorderitem_itemlistid = inputSOI[i].ItemlistID,
                    salesorderitem_productid = inputSOI[i].ProductID,
                    salesorderitem_productlistid = inputSOI[i].ProductlistID,
                    salesorderitem_itemid = inputSOI[i].ItemID,
                    salesorderitem_name = inputSOI[i].Name,
                    salesorderitem_price = (int)inputSOI[i].Price,
                    salesorderitem_date = inputSOI[i].Date,

                });
            }
            return results;
        }

        /// <summary>
        /// 取消訂單頁面
        /// </summary>
        /// <param name="salesorder_code">訂單號碼</param>
        /// <returns>頁面</returns>
        public ActionResult Reset_form(string salesorder_code)
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回首頁
            if (accID > 0)
            {
                // 獲取訂單資料
                Cart cartTemp = dbafter.Cart.Where(x => x.ID == salesorder_code).FirstOrDefault();

                // 若訂單存在
                if (cartTemp != null)
                {
                    // 訂單帳戶編號
                    int cartACCID = int.Parse(cartTemp.UserID);

                    // 判斷帳號是否與訂單號碼相同,若相同則顯示取消頁面;若不同則返回首頁
                    if (accID != cartACCID)
                    {
                        return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                    }
                    else
                    {
                        // 判斷訂單狀態是否可以取消
                        if (cartTemp.Status == 0 && (cartTemp.DelvStatus == (int)Cart.cartstatus.初始狀態 || cartTemp.DelvStatus == (int)Cart.cartstatus.已成立))
                        {
                            return View();
                        }
                        else
                        {
                            // 不能取消返回首頁
                            return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                        }
                    }
                }
                else
                {
                    // 訂單不存在,回首頁
                    return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                }
            }
            else
            {
                // 返回首頁
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/reset_form" });
            }
        }

        /// <summary>
        /// 獲取訂單取消
        /// </summary>
        /// <param name="salesOrder_code">訂單編碼</param>
        /// <returns>回傳Json顯示執行結果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reset_formapi(string salesOrder_code)
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();
            if (realid <= 0)
            {
                // 沒有登入,回饋錯誤
                return Json("error");
            }
            else
            {
                // 訂單SO資料
                Cart carttemp = new Cart();
                carttemp = dbafter.Cart.Where(x => x.ID == salesOrder_code).FirstOrDefault();
                List<object> returnData = new List<object>();
                //判斷訂單是否存在
                if (carttemp != null)
                {
                    // 訂購人ID
                    int userID = Convert.ToInt32(carttemp.UserID);
                    // 若訂購人ID不符合,回饋錯誤
                    if (userID != realid)
                    {
                        // 回饋錯誤
                        return Json("error");
                    }
                    // 若訂單狀態不為成功,回饋錯誤
                    if (carttemp.Status != 7 && carttemp.Status != 0)
                    {
                        // 回饋錯誤
                        return Json("error");
                    }
                    else
                    {
                        // 若訂單狀態不為成功且配送狀態已改變,則不能取消,回饋錯誤
                        if (!(carttemp.Status == 0 && (carttemp.DelvStatus == (int)Cart.cartstatus.初始狀態 || carttemp.DelvStatus == (int)Cart.cartstatus.已成立)))
                        {
                            // 回饋錯誤
                            return Json("error");
                        }
                        else
                        {
                            // 訂單SO名稱
                            string salesOrderitemname = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == salesOrder_code).Select(x => x.Name).FirstOrDefault();
                            // 回饋Json字串
                            var qnaanstext = "";
                            qnaanstext += "[{\"SalesOrder_code\" : \"" + salesOrder_code.ToString() + "\", \"SalesOrder_createdate\" : \"" + carttemp.CreateDate.Value.Year.ToString("0000") + "/" + carttemp.CreateDate.Value.Month.ToString("00") + "/" + carttemp.CreateDate.Value.Day.ToString("00") + "　　" + carttemp.CreateDate.Value.ToLongTimeString().ToString() + "\",";
                            qnaanstext += " \"SalesOrder_name\" : \"" + carttemp.Username.ToString() + "\", \"email\" : \"" + carttemp.Email.ToString() + "\", \"SalesOrder_mobile\" : \"" + carttemp.Mobile + "\", \"SalesOrder_address1\" : \"" + carttemp.Location.ToString() + "\", \"SalesOrder_address2\" : \"" + carttemp.Zipcode.ToString() + "\", \"SalesOrder_address3\" : \"" + carttemp.ADDR.ToString() + "\",";
                            qnaanstext += " \"SalesOrder_itemname\" : \"" + Server.HtmlEncode(salesOrderitemname.ToString()) + "\",\"SalesOrder_paytype\" : \"" + carttemp.PayType.ToString() + "\",\"SalesOrder_tel\" : \"" + carttemp.TelDay.ToString() + "\",";
                            qnaanstext += " \"salesorder_recvname\" : \"" + carttemp.Receiver.ToString() + "\",\"salesorder_recvtelday\" : \"" + carttemp.Phone.ToString() + "\",\"salesorder_recvmobile\" : \"" + carttemp.RecvMobile.ToString() + "\"}]";
                            returnData.Add(new
                            {
                                SalesOrder_code = salesOrder_code.ToString(),
                                SalesOrder_createdate = carttemp.CreateDate.Value.Year.ToString("0000") + "/" + carttemp.CreateDate.Value.Month.ToString("00") + "/" + carttemp.CreateDate.Value.Day.ToString("00") + "　　" + carttemp.CreateDate.Value.ToLongTimeString().ToString(),
                                SalesOrder_name = carttemp.Username.ToString(),
                                SalesOrder_mobile = carttemp.Mobile,
                                SalesOrder_address1 = carttemp.Location.ToString(),
                                SalesOrder_address2 = carttemp.Zipcode.ToString(),
                                SalesOrder_address3 = carttemp.ADDR.ToString(),
                                SalesOrder_itemname = Server.HtmlEncode(salesOrderitemname.ToString()),
                                SalesOrder_paytype = carttemp.PayType.ToString(),
                                SalesOrder_tel = carttemp.TelDay.ToString(),
                                salesorder_recvname = carttemp.Receiver.ToString(),
                                salesorder_recvtelday = carttemp.Phone.ToString(),
                                salesorder_recvmobile = carttemp.RecvMobile.ToString(),
                            });
                            return Json(returnData);
                        }
                    }
                }
                else
                {
                    return Json("error");
                }
            }
        }

        /// <summary>
        /// 取消訂單用API
        /// </summary>
        /// <param name="salesorder_code">訂單編號</param>
        /// <param name="reset_reasonval">取消原因</param>
        /// <param name="reset_reasontext">取消原因描述</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">銀行分行</param>
        /// <param name="bankaccount">銀行帳號</param>
        /// <param name="passive">??</param>
        /// <param name="password">密碼輸入??</param>
        /// <returns>Json成功失敗</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Resetpost(string salesorder_code, int? reset_reasonval, string reset_reasontext, string bankid, string branches, string bankaccount, bool passive = false, string password = "")
        {
            Thread.Sleep(1000);
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            // 判斷字串取消原因描述是否過長,若過長則自動截斷
            if (reset_reasontext.Length > 240)
            {
                reset_reasontext = reset_reasontext.Substring(0, 239);
            }

            // 判斷是否登入,登入則回AccountID,否則回0
            int accID = CheckAccount();
            // 若登入則繼續,沒登入則Json回饋上味登入訊息
            if (accID > 0)
            {
                // 撈取取消單資料
                List<SalesOrderCancel> salesOrderCancellist = dbbefore.SalesOrderCancel.Where(x => x.SalesorderCode == salesorder_code).ToList();
                // 撈取SO資料
                Cart cartTemp = dbafter.Cart.Where(x => x.ID == salesorder_code).FirstOrDefault();
                // 撈取Group底下SO狀態為正常且成立,
                List<Cart> cartList = dbafter.Cart.Where(x => x.SalesorderGroupID == (cartTemp.SalesorderGroupID ?? 0) && x.Status == 0 && (x.DelvStatus == (int)Cart.cartstatus.初始狀態 || x.DelvStatus == (int)Cart.cartstatus.已成立)).ToList();

                // 判斷cartList是否有訂單可以取消
                if (cartList.Count == 0)
                {
                    return Json("error");
                }
                // 判斷取消訂單與登入帳戶為同一人
                if (accID != int.Parse(cartList.FirstOrDefault().UserID))
                {
                    return Json("AccidError");
                }
                // 判斷訂單是否已經取消過在salesOrderCancellist有資料
                if (salesOrderCancellist.Count > 0)
                {
                    return Json("canceled");
                }
                //// 前台SO資料
                //SalesOrder salesOrder = dbbefore.SalesOrder.Single(x => x.Code == salesorder_code);
                // 訂單帳戶資料
                Account account = dbbefore.Account.Where(x => x.ID == accID).FirstOrDefault();
                // Logger取消訂單開始
                logger.Debug("start grepping data from db, salesorder " + salesorder_code);

                // SOGroupID
                int? sogroup = cartTemp.SalesorderGroupID;
                // SOGroupID底下前台SO訂單
                List<SalesOrder> salesOrders = dbbefore.SalesOrder.Where(s => s.SalesOrderGroupID == sogroup).ToList();
                // SalesorderCodeList SOGroupID底下前台SOIDList
                List<string> salesorderCodes = salesOrders.Select(s => s.Code).ToList();
                // SalesOrderItemList SOGroupID底下f前台SOitemList
                List<SalesOrderItem> salesOrderItems = dbbefore.SalesOrderItem.Where(p => salesorderCodes.Contains(p.SalesorderCode)).ToList();
                // SalesorderCodeList SOGroupID底下前台SOListIDList
                List<string> salesOrderItemsIDstring = salesOrderItems.Select(s => s.Code).ToList();
                // CattList SOGroupID底下後台SO訂單
                List<Cart> carts = dbafter.Cart.Where(x => x.SalesorderGroupID == sogroup).ToList();
                // CattIDList SOGroupID底下後台SOIDList
                List<string> cartsIDs = carts.Select(c => c.ID).ToList();
                // POCodesList SOGroupID底下後台POList
                List<string> pocodes = dbbefore.PurchaseOrder.Where(p => salesorderCodes.Contains(p.SalesorderCode)).Select(p => p.Code).ToList();
                // POItemsList SOGroupID底下後台POitemList
                List<PurchaseOrderItem> poitems = dbbefore.PurchaseOrderItem.Where(p => pocodes.Contains(p.PurchaseorderCode) && p.SellerOrderCode != null && p.SellerOrderCode.Trim() != "").ToList();
                // processesList SOGroupID底下後台SOitemList
                List<Process> processes = dbafter.Process.Where(p => cartsIDs.Contains(p.CartID)).ToList();
                // CouponsList
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> coupons;
                // CouponIDList
                List<string> couponIDListString;
                // eachCouponIDList
                List<string> eachcouponIDListString;
                // CouponIDList to Int
                List<int> couponIDListInt = new List<int>();

                // 判斷SOitemList數量(是否轉單)
                if (processes.Count > 0)
                {
                    couponIDListString = processes.Select(x => x.Coupons).ToList();
                }
                else
                {
                    couponIDListString = salesOrderItems.Select(x => x.Coupons).ToList();
                }

                // 為改變coupons狀態為使用取消,將使用過的coupons撈出
                foreach (var couponIDListStringtemp in couponIDListString)
                {
                    // coupons可以為兩張以上,使用','分開
                    eachcouponIDListString = couponIDListStringtemp.Split(',').ToList();
                    foreach (var eachCouponIDListStringtemp in eachcouponIDListString)
                    {
                        // 判斷是否有coupons存在
                        if (eachCouponIDListStringtemp != null && eachCouponIDListStringtemp.Trim() != "")
                        {
                            couponIDListInt.Add((Int32.Parse(eachCouponIDListStringtemp)));
                        }
                    }
                }
                // 將所有使用的coupons撈出
                //coupons = dbbefore.Coupon.Where(x => couponIDListInt.Contains(x.id)).ToList();
                coupons = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.EventReponsitory", "GetCouponByIdList", couponIDListInt).results;

                logger.Debug("end of data query from db, salesorder " + salesorder_code);

                // 在正式環境對美蛋拋送訂單取消
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    //NeweggRequestCancel(salesorder_code, logger, poitems);
                }

                // 取消單生成退款單 SO Group 為單位
                if (carts != null && carts.Count() > 0)
                {
                    logger.Debug("start generating refund2c, salesorder " + salesorder_code);
                    // 判斷是否產生退款單,貨到付款取消不產生
                    if (salesOrders[0].PayType != 31)
                    {
                        // 退款單
                        refund2c refund2c = new refund2c();
                        // 信用卡付款資料
                        CreditAuth creditAuth = new CreditAuth();
                        // SOID
                        string salesOrdersIDtemp = salesOrders[0].Code;
                        // 將信用卡刷卡資料撈出
                        creditAuth = dbafter.CreditAuth.Where(x => x.OrderNO == salesOrdersIDtemp || x.SalesOrderGroupID == (sogroup ?? 0)).FirstOrDefault();

                        // 若信用卡資訊存在
                        if (creditAuth != null)
                        {
                            // 判斷是否付款成功
                            if (creditAuth.SuccessFlag == "1")
                            {
                                // 產生退款單
                                refund2c = Creatrefund2c(carts, processes, bankid, branches, bankaccount, "取消-" + reset_reasontext,reset_reasonval);
                            }
                        }
                        else
                        {
                            // 若信用卡資訊不存,產生退款單
                            refund2c = Creatrefund2c(carts, processes, bankid, branches, bankaccount, "取消-" + reset_reasontext,reset_reasonval);
                        }

                    }
                }

                // 產生取消單
                List<SalesOrderCancel> CreatSalesOrderList = CreatSalesOrderCancel(carts, processes, bankid, branches, bankaccount, reset_reasontext);
                //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => pocodes.Contains(x.SalesOrderItemCode)).ToList();
                List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", pocodes).results;

                // 計算退款通知信寄出嘗試次數
                int count = 0;
                // 給客服人員的訂單取消通知信
                // PO拋單異常造成失敗
                logger.Info("In t0 Parsing.....GeneraterViewPage：" + CreatSalesOrderList.FirstOrDefault().SalesorderCode);
                if (passive)
                {
                    var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                    TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", carts, salesOrders, "拋單失敗異常訂單取消", reset_reasonval);
                    //while (!Mail_InnerCancelSO(salesOrders, "拋單失敗異常訂單取消", bankid, branches, bankaccount))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                }
                else
                {
                    var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                    var GeneraterViewPageResult = TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", carts, salesOrders, reset_reasontext, reset_reasonval);
                    TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "CancelSO", carts, salesOrders, reset_reasontext,reset_reasonval);
                    //// 使用者取消訂單
                    //List<SalesOrder> SalesOrdertemp = salesOrders.Where(x => x.DelivType == 2 || x.DelivType == 7 || x.DelivType == 8 || x.DelivType == 9).ToList();
                    //// 給SellerVendor客服的訂單取消通知信
                    //if (SalesOrdertemp.Count > 0 && SalesOrdertemp != null)
                    //{
                    //    while (!Mail_VendorCancelSO(SalesOrdertemp, reset_reasontext, bankid, branches, bankaccount))
                    //    {
                    //        count++;
                    //        if (count >= 3)
                    //        {
                    //            logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //            break;
                    //        }
                    //    }
                    //}
                    //// 給客服的訂單取消通知信
                    //while (!Mail_InnerCancelSO(salesOrders, reset_reasontext, bankid, branches, bankaccount))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                    //// 給使用者的訂單取消通知信
                    //while (!Mail_CancelSO(salesOrders))
                    //{
                    //    count++;
                    //    if (count >= 3)
                    //    {
                    //        logger.Error("Mail_CancelSO not sent, salesorder " + salesorder_code);
                    //        break;
                    //    }
                    //}
                }

                logger.Debug("Emails have been sent, salesorder " + salesorder_code);

                //Avoid the difference time cause database error,check again
                carts = dbafter.Cart.Where(x => x.SalesorderGroupID == sogroup).ToList();
                // 將前後台主單取消
                foreach (var tempitem in carts)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                foreach (var tempitem in salesOrders)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                // 取消coupons使用狀態
                UpDataCoupon(salesOrderItemsIDstring, accID);

                // 送sellerprotal取消信
                string Environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    try
                    {
                        string sellerprotalIP = "";
                        try
                        {
                            sellerprotalIP = System.Configuration.ConfigurationManager.AppSettings["SellerProtalIP_" + Environment];
                        }
                        catch
                        {

                        }
                        if (sellerprotalIP != "")
                        {
                            TWNewEgg.Website.ECWeb.Service.NeweggRequest ProcessVoidOrderNumMailurlReq = new TWNewEgg.Website.ECWeb.Service.NeweggRequest();
                            string OrderNum = salesorder_code;
                            string ProcessVoidOrderNumMailurlReqtemp = ProcessVoidOrderNumMailurlReq.Post<string>(sellerprotalIP + "/shiftOrderNum/ProcessVoidOrderNumMail?OrderNum=" + salesorder_code, null);
                        }
                    }
                    catch { }
                }
                //if (coupons.Count() > 0)
                //{
                //    foreach (var Couponstemp in coupons)
                //    {
                //        Couponstemp.usestatus = (int)Coupon.CouponUsedStatusOption.UsedButCancel;
                //        Couponstemp.updatedate = DateTime.Now;
                //        Couponstemp.updateuser = Couponstemp.accountid;
                //        Couponstemp.note = Couponstemp.note + " " + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                //    }
                //}
                //if (PromotionGiftRecordsList.Count() > 0)
                //{
                //    foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                //    {
                //        PromotionGiftRecordtemp.UsedStatus = (int)PromotionGiftRecords.UsedStatusOption.CancelUsed;
                //        PromotionGiftRecordtemp.UpdateDate = DateTime.Now;
                //        PromotionGiftRecordtemp.UpdateUser = accID.ToString();
                //    }
                //}
            }
            try
            {
                dbafter.SaveChanges();
                dbbefore.SaveChanges();
                // 回傳成功
                return Json("1");
            }
            catch (Exception e)
            {
                logger.Error("exception caught", e);
                return Json("error");
            }
        }

        /// <summary>
        /// 產生取消單
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">分行ID</param>
        /// <param name="bankaccount">退款帳號</param>
        /// <param name="reset_reasontext">取消原因</param>
        /// <returns>SalesOrderCancel model</returns>
        public List<SalesOrderCancel> CreatSalesOrderCancel(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> carts, List<TWNewEgg.DB.TWBACKENDDB.Models.Process> processes, string bankid, string branches, string bankaccount, string reset_reasontext)
        {
            // 取消單List
            List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> SalesOrderCancelList = new List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>();
            // 產生取消單
            for (int i = 0; i < carts.Count(); i++)
            {
                int itemid = (processes.Where(s => s.CartID == carts[i].ID).Select(s => s.StoreID).FirstOrDefault() ?? 0);
                TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel SalesOrderCancel = new TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel()
                {
                    // CartID
                    SalesorderCode = carts[i].ID,
                    // 賣場ID
                    ItemID = itemid,
                    // 取消原因
                    CauseNote = reset_reasontext,
                    // 使用者名稱
                    AccountName = carts[i].Username,
                    // 銀行ID
                    BankID = bankid,
                    // 分行ID
                    BankBranch = branches,
                    // 退款帳戶
                    AccountNO = bankaccount,
                    // 產生時間
                    CreateDate = DateTime.Now
                };

                TWBdbbefore.SalesOrderCancel.Add(SalesOrderCancel);
                SalesOrderCancelList.Add(SalesOrderCancel);
            }
            TWBdbbefore.SaveChanges();
            return SalesOrderCancelList;
        }

        /// <summary>
        /// 取消訂單用API
        /// </summary>
        /// <param name="salesorder_code">訂單編號</param>
        /// <param name="reset_reasontext">取消原因描述</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">銀行分行</param>
        /// <param name="bankaccount">銀行帳號</param>
        /// <param name="passive">??</param>
        /// <param name="password">密碼輸入??</param>
        /// <returns>Json成功失敗</returns>
        public int CreatCancel(List<Cart> CartList, List<Process> Processes, List<SalesOrder> SalesOrders, List<SalesOrderItem> SalesOrderItems, string reset_reasontext,int? reset_reasonval, string bankid, string branches, string bankaccount)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            List<string> ProcessesIDstringList = Processes.Select(x => x.ID).ToList();
            refund2c refund2ctemp = dbafter.refund2c.Where(x => ProcessesIDstringList.Contains(x.ProcessID)).FirstOrDefault();

            if (refund2ctemp == null)
            {
                // 判斷字串取消原因描述是否過長,若過長則自動截斷
                if (reset_reasontext.Length > 240)
                {
                    reset_reasontext = reset_reasontext.Substring(0, 239);
                }

                // 帳號
                string AccountID = CartList.FirstOrDefault().UserID;
                // 撈取取消單資料
                string SalesorderCodetemp = CartList.FirstOrDefault().ID;
                List<SalesOrderCancel> salesOrderCancellist = dbbefore.SalesOrderCancel.Where(x => x.SalesorderCode == SalesorderCodetemp).ToList();
                // SOGroupID
                int? sogroup = CartList.FirstOrDefault().SalesorderGroupID;
                // POCodesList SOGroupID底下後台POList
                List<string> pocodes = dbbefore.PurchaseOrder.Where(p => p.SalesorderCode == SalesorderCodetemp).Select(p => p.Code).ToList();
                // POItemsList SOGroupID底下後台POitemList
                List<PurchaseOrderItem> poitems = dbbefore.PurchaseOrderItem.Where(p => pocodes.Contains(p.PurchaseorderCode) && p.SellerOrderCode != null && p.SellerOrderCode.Trim() != "").ToList();
                // SalesorderCodeList SOGroupID底下前台SOIDList
                List<string> salesorderCodes = SalesOrders.Select(s => s.Code).ToList();
                // CouponsList
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> coupons;
                // CouponIDList
                List<string> couponIDListString;
                // eachCouponIDList
                List<string> eachcouponIDListString;
                // CouponIDList to Int
                List<int> couponIDListInt = new List<int>();
                bool boolExec = false;

                // 判斷SOitemList數量(是否轉單)
                if (Processes.Count > 0)
                {
                    couponIDListString = Processes.Select(x => x.Coupons).ToList();
                }
                else
                {
                    couponIDListString = SalesOrderItems.Select(x => x.Coupons).ToList();
                }

                // 為改變coupons狀態為使用取消,將使用過的coupons撈出
                foreach (var couponIDListStringtemp in couponIDListString)
                {
                    // coupons可以為兩張以上,使用','分開
                    eachcouponIDListString = couponIDListStringtemp.Split(',').ToList();
                    foreach (var eachCouponIDListStringtemp in eachcouponIDListString)
                    {
                        // 判斷是否有coupons存在
                        if (eachCouponIDListStringtemp != null && eachCouponIDListStringtemp.Trim() != "")
                        {
                            couponIDListInt.Add((Int32.Parse(eachCouponIDListStringtemp)));
                        }
                    }
                }
                // 將所有使用的coupons撈出
                //coupons = dbbefore.Coupon.Where(x => couponIDListInt.Contains(x.id)).ToList();
                coupons = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByIdList", couponIDListInt).results;

                logger.Debug("end of data query from db, salesorder " + SalesorderCodetemp);

                // 在正式環境對美蛋拋送訂單取消
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    NeweggRequestCancel(SalesorderCodetemp, logger, poitems);
                }

                // 取消單生成退款單 SO Group 為單位
                if (CartList != null && CartList.Count() > 0)
                {
                    logger.Debug("start generating refund2c, salesorder " + SalesorderCodetemp);
                    // 判斷是否產生退款單,貨到付款取消不產生
                    if (SalesOrders[0].PayType != 31)
                    {
                        // 退款單
                        refund2c refund2c = new refund2c();
                        // 信用卡付款資料
                        CreditAuth creditAuth = new CreditAuth();
                        // SOID
                        string salesOrdersIDtemp = SalesOrders[0].Code;
                        // 將信用卡刷卡資料撈出
                        creditAuth = dbafter.CreditAuth.Where(x => x.OrderNO == salesOrdersIDtemp || x.SalesOrderGroupID == (sogroup ?? 0)).FirstOrDefault();

                        // 若信用卡資訊存在
                        if (creditAuth != null)
                        {
                            // 判斷是否付款成功
                            if (creditAuth.SuccessFlag == "1")
                            {
                                // 產生退款單
                                refund2c = Creatrefund2c(CartList, Processes, bankid, branches, bankaccount, "取消-" + reset_reasontext,reset_reasonval);
                            }
                        }
                        else
                        {
                            // 若信用卡資訊不存,產生退款單
                            refund2c = Creatrefund2c(CartList, Processes, bankid, branches, bankaccount, "取消-" + reset_reasontext,reset_reasonval);
                        }

                    }
                }

                // 產生取消單
                List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> CreatSalesOrderList = CreatSalesOrderCancel(CartList, Processes, bankid, branches, bankaccount, reset_reasontext);
                //List<PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => pocodes.Contains(x.SalesOrderItemCode)).ToList();
                List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", pocodes).results;

                // 計算退款通知信寄出嘗試次數
                int count = 0;
                List<SalesOrder> SalesOrdertemp = SalesOrders.Where(x => x.DelivType == 2 || x.DelivType == 7 || x.DelivType == 8 || x.DelivType == 9).ToList();
                var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "InnerCancelSO", CartList, SalesOrders, reset_reasontext, reset_reasonval);
                TestXMLEXport.GeneraterViewPage(CreatSalesOrderList, "CancelSO", CartList, SalesOrders, reset_reasontext, reset_reasonval);

                //// 給SellerVendor客服的訂單取消通知信
                //if (SalesOrdertemp.Count > 0 && SalesOrdertemp != null)
                //{
                //    while (!Mail_VendorCancelSO(SalesOrdertemp, reset_reasontext, bankid, branches, bankaccount))
                //    {
                //        count++;
                //        if (count >= 3)
                //        {
                //            logger.Error("Mail_InnerCancelSO not sent, salesorder " + SalesorderCodetemp);
                //            break;
                //        }
                //    }
                //}
                //// 給客服的訂單取消通知信
                //while (!Mail_InnerCancelSO(SalesOrders, reset_reasontext, bankid, branches, bankaccount))
                //{
                //    count++;
                //    if (count >= 3)
                //    {
                //        logger.Error("Mail_InnerCancelSO not sent, salesorder " + SalesorderCodetemp);
                //        break;
                //    }
                //}
                //// 給使用者的訂單取消通知信
                //while (!Mail_CancelSO(SalesOrders))
                //{
                //    count++;
                //    if (count >= 3)
                //    {
                //        logger.Error("Mail_CancelSO not sent, salesorder " + SalesorderCodetemp);
                //        break;
                //    }
                //}

                logger.Debug("Emails have been sent, salesorder " + SalesorderCodetemp);



                // 將前後台主單取消
                foreach (var tempitem in CartList)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                foreach (var tempitem in SalesOrders)
                {
                    tempitem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.取消;
                }
                // 取消coupons使用狀態
                if (coupons.Count() > 0)
                {
                    foreach (var Couponstemp in coupons)
                    {
                        Couponstemp.usestatus = (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel;
                        Couponstemp.updatedate = DateTime.Now;
                        Couponstemp.updateuser = Couponstemp.accountid;
                        Couponstemp.note = Couponstemp.note + " " + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                    }
                }
                //修改使用狀態
                if (PromotionGiftRecordsList.Count() > 0)
                {
                    foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                    {
                        PromotionGiftRecordtemp.UsedStatus = (int)TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords.UsedStatusOption.CancelUsed;
                        PromotionGiftRecordtemp.UpdateDate = DateTime.Now;
                        PromotionGiftRecordtemp.UpdateUser = AccountID.ToString();
                    }

                    boolExec = Processor.Request<bool, bool>("Service.PromotionGiftService.PromotionGiftRecordRepository", "UpdatePromotionGiftRecordByList", PromotionGiftRecordsList).results;
                }

                try
                {
                    dbafter.SaveChanges();
                    dbbefore.SaveChanges();
                    // 回傳成功
                    return 1;
                }
                catch (Exception e)
                {
                    logger.Error("exception caught", e);
                    return 0;
                }
            }
            else
            {
                logger.Error("exception caught:退款單已產生過");
                return 0;
            }
        }

        /// <summary>
        /// 建立退款單
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <param name="bankid">銀行ID</param>
        /// <param name="branches">分行ID</param>
        /// <param name="bankaccount">退款帳號</param>
        /// <param name="reset_reasontext">取消原因</param>
        /// <returns>refund2c model</returns>
        public refund2c Creatrefund2c(List<Cart> carts, List<Process> processes, string bankid, string branches, string bankaccount, string reset_reasontext,int? reset_reasonval)
        {
            refund2c refund2c = new refund2c();
            List<string> cartsIDs = carts.Select(x => x.ID).ToList();

            // 將子單金額加總
            refund2c.Amount = CalculationAmount(carts, processes);
            // 退款單編號
            refund2c.Code = GetRegoodNO("Refund2c");
            refund2c.AccountName = carts[0].Username;
            refund2c.BankID = bankid;
            refund2c.BankName = bankid;
            refund2c.SubBankName = branches;
            refund2c.AccountNO = bankaccount;
            refund2c.PayDate = carts[0].CreateDate;
            refund2c.Status = 98;
            refund2c.StatusDate = DateTime.Now;
            refund2c.Cause = reset_reasonval;
            refund2c.CauseNote = reset_reasontext;
            refund2c.ApplyDate = DateTime.Now;
            refund2c.CreateDate = DateTime.Now;
            refund2c.Date = DateTime.Now;
            refund2c.UpdateNote = DateTime.Now + "　退款單成立　;<br>";
            refund2c.ProcessID = processes.OrderBy(p => p.ID).First().ID;
            refund2c.CartID = carts[0].ID;
            // 判斷是否開立發票
            if (carts.FirstOrDefault().InvoiceNO == null || carts.FirstOrDefault().InvoiceNO == "")
            {
                refund2c.InvoiceResult = (int)TWNewEgg.DB.TWBACKENDDB.Models.refund2c.InvoiceResult_Status.未開立發票;
            }

            // 判斷金額大於0產生退款單
            if (refund2c.Amount > 0)
            {
                List<refund2c> refund2ctemp = dbafter.refund2c.Where(x => cartsIDs.Contains(x.CartID)).ToList();
                // 判斷使否產生過退款單
                if (refund2ctemp.Count == 0)
                {
                    dbafter.refund2c.Add(refund2c);
                    dbafter.SaveChanges();
                }
                else
                {
                    refund2c = null;
                }
            }
            return refund2c;
        }

        /// <summary>
        /// 計算退款金額
        /// </summary>
        /// <param name="carts">cartsList</param>
        /// <param name="Processes">processesList</param>
        /// <returns>退款金額</returns>
        public int CalculationAmount(List<Cart> carts, List<Process> processes)
        {
            int Amount = 0;
            for (int i = 0; i < carts.Count(); i++)
            {
                //if (!new List<int>() { 31, 32 }.Contains(carts[i].PayType ?? 0))
                //{
                List<Process> Processlist = processes.Where(x => x.CartID == carts[i].ID).ToList();
                foreach (var proc in Processlist)
                {
                    if (proc.ProductID != 13189 && proc.ProductID != 13190)
                    {
                        Amount += (int)(proc.Price ?? 0);
                        Amount += (int)(proc.ShippingExpense ?? 0);
                        Amount += (int)(proc.ServiceExpense ?? 0);
                        Amount += (int)(proc.InstallmentFee);
                        Amount -= (int)(Math.Abs(proc.Pricecoupon ?? 0));
                        Amount -= (int)(Math.Abs(proc.ApportionedAmount));
                    }
                }
                //}
            }
            return Amount;
        }

        /// <summary>
        /// 訂單退貨頁面
        /// </summary>
        /// <param name="salesorder_code">訂單號碼</param>
        /// <returns>頁面</returns>
        public ActionResult Return_form(string salesorder_code)
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回首頁
            if (accID > 0)
            {
                // 獲取訂單資料
                Cart cartTemp = dbafter.Cart.Where(x => x.ID == salesorder_code).FirstOrDefault();

                // 若訂單存在
                if (cartTemp != null)
                {
                    // 訂單配達狀態
                    int delvStatus = cartTemp.DelvStatus.Value;

                    // 訂單帳戶編號
                    int cartACCID = int.Parse(cartTemp.UserID);

                    // 判斷登入帳戶是否與訂單帳戶相同,若不同返回首頁
                    if (accID != cartACCID)
                    {
                        return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                    }
                    else
                    {
                        // 判斷定單是否配達,沒有配達則返回首頁
                        if (delvStatus == (int)Cart.cartstatus.配達)
                        {
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
                }
            }
            else
            {
                // 沒有登入返回首頁
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/return_form" });
            }
        }

        /// <summary>
        /// 獲取訂單資料
        /// </summary>
        /// <param name="salesOrder_code">訂單編號</param>
        /// <returns>Json頁面資料</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Return_formapi(string salesOrder_code)
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();
            if (realid <= 0)
            {
                // 沒有登入,回饋錯誤
                return Json("error");
            }
            else
            {
                // 訂單SO資料
                Cart Carttemp = new Cart();
                Carttemp = dbafter.Cart.Where(x => x.ID == salesOrder_code).FirstOrDefault();
                int SalesorderGroupID = Carttemp.SalesorderGroupID ?? 0;
                List<Cart> CartList = dbafter.Cart.Where(x => x.SalesorderGroupID == SalesorderGroupID).ToList();
                List<string> CartCodeList = CartList.Select(x => x.ID).ToList();
                List<Process> ProcessList = dbafter.Process.Where(x => CartCodeList.Contains(x.CartID)).ToList();
                decimal ApportionedAmount = ProcessList.Sum(x => x.ApportionedAmount);
                string ApportionedAlert = "";
                List<object> returnData = new List<object>();
                //判斷訂單是否存在
                if (Carttemp != null)
                {
                    // 訂購人ID
                    int USERID = Convert.ToInt32(Carttemp.UserID);
                    // 若訂購人ID不符合,回饋錯誤
                    if (USERID != realid && USERID != 43)
                    {
                        // 回饋錯誤
                        return Json("error");
                    }
                    // 若訂單狀態不為成功,回饋錯誤
                    if (Carttemp.Status != 7)
                    {
                        return Json("error");
                    }
                    else
                    {
                        // 若訂單狀態不為配達,回饋錯誤
                        if (Carttemp.DelvStatus != 2)
                        {
                            return Json("error");
                        }
                        else
                        {
                            // 訂單商品名稱
                            string SalesOrderitemname = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == salesOrder_code).Select(x => x.Name).FirstOrDefault();
                            if (ApportionedAmount != 0m)
                            {
                                SalesOrderitemname = "";
                                salesOrder_code = "";
                                foreach (var Cartitem in CartList)
                                {
                                    decimal ApportionedAmounttemp = ProcessList.Where(x => x.CartID == Cartitem.ID).Sum(x => x.ApportionedAmount);
                                    if (ApportionedAmounttemp != 0m)
                                    {
                                        if (salesOrder_code != "")
                                        {
                                            ApportionedAlert = "Y";
                                            salesOrder_code = salesOrder_code + "<br>";
                                            SalesOrderitemname = SalesOrderitemname + "<br>";
                                        }
                                        salesOrder_code = salesOrder_code + Cartitem.ID.ToString();
                                        SalesOrderitemname = SalesOrderitemname + Server.HtmlEncode(ProcessList.Where(x => x.CartID == Cartitem.ID).Select(x => x.Title).FirstOrDefault().ToString());
                                    }
                                }

                            }
                            // 回饋Json字串
                            var qnaanstext = "";
                            qnaanstext += "[{\"SalesOrder_code\" : \"" + salesOrder_code.ToString() + "\", \"SalesOrder_createdate\" : \"" + Carttemp.CreateDate.Value.Year.ToString("0000") + "/" + Carttemp.CreateDate.Value.Month.ToString("00") + "/" + Carttemp.CreateDate.Value.Day.ToString("00") + "　　" + Carttemp.CreateDate.Value.ToLongTimeString().ToString() + "\",";
                            qnaanstext += " \"SalesOrder_name\" : \"" + Carttemp.Username.ToString() + "\", \"email\" : \"" + Carttemp.Email.ToString() + "\", \"SalesOrder_mobile\" : \"" + Carttemp.Mobile + "\", \"SalesOrder_address1\" : \"" + Carttemp.Location.ToString() + "\", \"SalesOrder_address2\" : \"" + Carttemp.Zipcode.ToString() + "\", \"SalesOrder_address3\" : \"" + Carttemp.ADDR.ToString() + "\",";
                            qnaanstext += " \"SalesOrder_itemname\" : \"" + SalesOrderitemname.ToString() + "\",\"SalesOrder_paytype\" : \"" + Carttemp.PayType.ToString() + "\",\"SalesOrder_tel\" : \"" + Carttemp.TelDay.ToString() + "\",";
                            qnaanstext += " \"salesorder_recvname\" : \"" + Carttemp.Receiver.ToString() + "\",\"salesorder_recvtelday\" : \"" + Carttemp.Phone.ToString() + "\",\"salesorder_recvmobile\" : \"" + Carttemp.RecvMobile.ToString() + "\",\"ApportionedAlert\" : \"" + ApportionedAlert.ToString() + "\"}]";
                            returnData.Add(new
                            {
                                SalesOrder_code = salesOrder_code.ToString(),
                                SalesOrder_createdate = Carttemp.CreateDate.Value.Year.ToString("0000") + "/" + Carttemp.CreateDate.Value.Month.ToString("00") + "/" + Carttemp.CreateDate.Value.Day.ToString("00") + "　　" + Carttemp.CreateDate.Value.ToLongTimeString().ToString(),
                                SalesOrder_name = Carttemp.Username.ToString(),
                                email = Carttemp.Email.ToString(),
                                SalesOrder_mobile = Carttemp.Mobile,
                                SalesOrder_address1 = Carttemp.Location.ToString(),
                                SalesOrder_address2 = Carttemp.Zipcode.ToString(),
                                SalesOrder_address3 = Carttemp.ADDR.ToString(),
                                SalesOrder_itemname = Server.HtmlEncode(SalesOrderitemname.ToString()),
                                SalesOrder_paytype = Carttemp.PayType.ToString(),
                                SalesOrder_tel = Carttemp.TelDay.ToString(),
                                salesorder_recvname = Carttemp.Receiver.ToString(),
                                salesorder_recvtelday = Carttemp.Phone.ToString(),
                                salesorder_recvmobile = Carttemp.RecvMobile.ToString(),
                                ApportionedAlert = ApportionedAlert.ToString(),
                            });
                            return Json(returnData);
                        }
                    }
                }
                else
                {
                    // 訂單不存在,回饋錯誤
                    return Json("error");
                }
            }
        }

        /// <summary>
        /// 訂單退貨Post, status = 5 for the return
        /// </summary>
        /// <param name="salesorder_code">訂單編碼</param>
        /// <param name="return_reasonval">退貨原因</param>
        /// <param name="return_reasontext">退貨描述</param>
        /// <param name="item_status">商品狀態</param>
        /// <param name="name">商品名稱</param>
        /// <param name="email">訂購人email</param>
        /// <param name="address1">訂購人郵遞區號</param>
        /// <param name="address2">訂購人縣市</param>
        /// <param name="address3">訂購人地址</param>
        /// <param name="tel">訂購人電話</param>
        /// <param name="mobile">訂購人手機</param>
        /// <param name="bankid">退款銀行ID</param>
        /// <param name="branches">退款銀行分行</param>
        /// <param name="bankaccount">退款銀行帳號</param>
        /// <returns>Json回傳執行結果</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Returnpost(string salesorder_code, int? return_reasonval, string return_reasontext, string item_status, string name, string email, string address1, string address2, string address3, string tel, string mobile, string bankid, string branches, string bankaccount)
        {
            Thread.Sleep(500);
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            aesenc = new AesCookies();

            // retgoodList
            List<Retgood> retgoodList = new List<Retgood>();
            // 判斷是否登入,沒登入返回登入頁面
            int accID = CheckAccount();
            // RetgoodMODEL
            Retgood retgood = new Retgood();
            // SalesOrderMODEL
            SalesOrder salesOrder = dbbefore.SalesOrder.Where(x => x.Code == salesorder_code).FirstOrDefault();
            // SalesOrderitemMODEL
            Process salesOrderitem = dbafter.Process.Where(x => x.CartID == salesorder_code).FirstOrDefault();
            // CartMODEL
            Cart cart = dbafter.Cart.Where(x => x.ID == salesorder_code).FirstOrDefault();
            // CartMODEL
            List<Cart> CartList = new List<Cart>();
            List<SalesOrder> SalesOrderList = new List<SalesOrder>();
            CartList.Add(cart);
            // SO子單List
            List<Process> ProcessList = dbafter.Process.Where(x => x.CartID == salesorder_code && x.ProductID != 13189 && x.ProductID != 13190).ToList();
            // 退貨單
            retgood = dbafter.Retgood.Where(x => x.ProcessID == salesOrderitem.ID).FirstOrDefault();
            // 判斷此訂單是否退訂過
            string salesorder_codeold = dbafter.Retgood.Where(x => x.CartID == salesorder_code).Select(x => x.CartID).FirstOrDefault();
            int count = 0;

            // 判斷此退貨單是否存在
            if (retgood != null || salesorder_codeold != null)
            {
                return Json("Has_returned");
            }

            // 判斷登入帳號是否正確
            if (salesOrder.AccountID != accID)
            {
                return Json("AccidError");
            }

            // 是否配達
            if (cart.DelvStatus != 2)
            {
                return Json("error");
            }
            else
            {
                if (ProcessList.Sum(x => x.ApportionedAmount) == 0m)
                {
                    // 判斷訂單是否存在,若存在,將狀態改為退貨
                    if (salesOrder != null)
                    {
                        salesOrder.Status = (int)TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status.退貨;
                    }

                    // 判斷後台訂單是否存在,若存在,將狀態改為退貨
                    if (cart != null)
                    {
                        cart.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;
                    }
                    // 產生退貨單
                    retgoodList = CreatRetgood(CartList, ProcessList, return_reasonval, return_reasontext, item_status, name, email, address1, address2, address3, tel, mobile, bankid, branches, bankaccount);
                }
                else
                {
                    int SalesorderGroupID = cart.SalesorderGroupID ?? 0;
                    CartList = dbafter.Cart.Where(x => x.SalesorderGroupID == SalesorderGroupID).ToList();
                    SalesOrderList = dbbefore.SalesOrder.Where(x => x.SalesOrderGroupID == SalesorderGroupID).ToList();
                    List<string> CartCodeList = CartList.Select(x => x.ID).ToList();
                    ProcessList = dbafter.Process.Where(x => CartCodeList.Contains(x.CartID)).ToList();
                    foreach (var Cartitem in CartList)
                    {
                        List<Retgood> retgoodListtemp = new List<Retgood>();
                        List<Process> ProcessListtemp = ProcessList.Where(x => x.CartID == Cartitem.ID).ToList();
                        List<Cart> CartListtemp = new List<Cart>();
                        CartListtemp.Add(Cartitem);
                        if (ProcessList.Sum(x => x.ApportionedAmount) != 0m)
                        {
                            if (Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨 && (Cartitem.DelvStatus == 1 || Cartitem.DelvStatus == 2 || Cartitem.DelvStatus == 5))
                            {
                                retgoodListtemp = CreatRetgood(CartListtemp, ProcessListtemp, return_reasonval, return_reasontext, item_status, name, email, address1, address2, address3, tel, mobile, bankid, branches, bankaccount);
                                Cartitem.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;
                                SalesOrderList.Where(x => x.Code == Cartitem.ID).FirstOrDefault().Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨;

                                retgoodList.Add(retgoodListtemp.FirstOrDefault());
                            }
                            else if (Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨 && Cartitem.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消)
                            {
                                string CartitemCode = Cartitem.ID;
                                List<SalesOrder> SalesOrderListtemp = dbbefore.SalesOrder.Where(x => x.Code == CartitemCode).ToList();
                                List<SalesOrderItem> SalesOrderItemListtemp = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == CartitemCode).ToList();
                                CreatCancel(CartListtemp, ProcessListtemp, SalesOrderListtemp, SalesOrderItemListtemp, "其它共同活動參與商品退貨-" + return_reasontext,return_reasonval, bankid, branches, bankaccount);
                                Cartitem.Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消;
                                SalesOrderList.Where(x => x.Code == Cartitem.ID).FirstOrDefault().Status = (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消;
                            }
                        }

                    }

                }

                List<string> ProcessListString = ProcessList.Select(x => x.ID).ToList();
                UpDataCoupon(ProcessListString, accID);

                Retgood retgoodmain = new Retgood();
                retgoodmain = retgoodList.FirstOrDefault();

                try
                {
                    dbbefore.SaveChanges();
                    dbafter.SaveChanges();
                }
                catch (InvalidCastException e)
                {
                    logger.Info("sellercode:" + salesorder_code + "Error:" + e);
                    return Json("Error");
                }

                // 收件人
                string Recipient = "";
                Recipient = aesenc.AESdecrypt(Request.Cookies["em"].Value) + "," + retgoodmain.FrmEmail;

                // 判斷交易模式寄信給物流
                if (cart.ShipType == 2 || cart.ShipType == 7 || cart.ShipType == 8 || cart.ShipType == 9)
                {
                    Recipient = "Gretchen.H.Yeh@newegg.com, Teresa.S.Li@newegg.com, Dolcee.J.Chang@newegg.com, Joyce.H.Hsiao@newegg.com, gp.team.mkpl.sourcing.tw@newegg.com";
                }
                else
                {
                    Recipient = "Gretchen.H.Yeh@newegg.com, Grace.c.hsiao@newegg.com, Steven.c.mao@newegg.com, Jasmine.C.Hsieh@newegg.com, Jessie.Y.Tseng@newegg.com, Dolcee.J.Chang@newegg.com, Joyce.H.Hsiao@newegg.com, gp.team.mkpl.sourcing.tw@newegg.com";
                }

                var TestXMLEXport = new TWNewEgg.InternalSendMail.Service.GeneratorView();
                TestXMLEXport.GeneraterViewPage(retgoodList, "retgood", CartList, SalesOrderList, return_reasontext, null);

                //foreach (var MailRetgooditem in retgoodList)
                //{
                //    List<Retgood> MailRetgooditemtemp = new List<Retgood>();
                //    MailRetgooditemtemp.Add(MailRetgooditem);
                //    // 寄信給消費者
                //    while (!Mail_RetgoodMessage(MailRetgooditemtemp, Recipient, "", false))
                //    {
                //        count++;
                //        if (count >= 3)
                //            break;
                //    }
                //}
                return Json("Success");

            }
        }

        private bool Mail_InnerCancelSO(SalesOrder SalesOrdertemp, string return_reasontext, string bankid, string branches, string bankaccount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 建立退貨單
        /// </summary>
        /// <param name="carts"></param>
        /// <param name="Processes"></param>
        /// <param name="bankid"></param>
        /// <param name="branches"></param>
        /// <param name="bankaccount"></param>
        /// <param name="reset_reasontext"></param>
        /// <returns></returns>
        public List<Retgood> CreatRetgood(List<Cart> carts, List<Process> processes, int? return_reasonval, string return_reasontext, string item_status, string name, string email, string address1, string address2, string address3, string tel, string mobile, string bankid, string branches, string bankaccount)
        {
            // retgoodList
            List<Retgood> retgoodList = new List<Retgood>();
            Retgood retgoodmain = new Retgood();
            // 退貨人
            retgoodmain.AccountName = carts.Select(x => x.Username).FirstOrDefault();
            // 退貨原因編碼
            retgoodmain.Cause = (int)return_reasonval;
            // 退貨原因
            retgoodmain.CauseNote = return_reasontext;
            // 商品狀態
            retgoodmain.DealNote = item_status;
            // 商品屬性0一般, 10屬性, 20贈品
            retgoodmain.RetgoodType = 0;
            // 收件人
            retgoodmain.FrmName = name;
            // 收件人email
            retgoodmain.FrmEmail = email;
            // 收件人電話
            retgoodmain.FrmPhone = tel;
            retgoodmain.FrmMobile = mobile;
            // 收件人地址
            retgoodmain.FrmLocation = address1;
            retgoodmain.FrmZipcode = address2.Split(' ')[0];
            retgoodmain.FrmADDR = (address2.Split(' ')[1] ?? "") + address3;
            // 退貨單號
            retgoodmain.Code = GetRegoodNO("Regood");
            retgoodmain.Price = Convert.ToDecimal(CalculationAmount(carts, processes));
            // 退貨品項
            retgoodmain.ProductID = processes.Where(x => x.ProductID != 13189 && x.ProductID != 13190).FirstOrDefault().ProductID;
            // 退貨SO
            retgoodmain.CartID = carts.FirstOrDefault().ID;
            // 退貨SOitem
            retgoodmain.ProcessID = processes.Where(x => x.ProductID != 13189 && x.ProductID != 13190).FirstOrDefault().ID;
            // 退貨數量
            retgoodmain.Qty = processes.Select(x => x.Qty ?? 0).Sum();
            // 退貨庫存代號
            retgoodmain.StockOutItemID = dbbefore.ItemStock.Where(x => x.ProductID == retgoodmain.ProductID).Select(x => x.ID).FirstOrDefault();
            // 退貨seller
            retgoodmain.SupplierID = dbbefore.Product.Where(x => x.ID == retgoodmain.ProductID).FirstOrDefault().SellerID;
            retgoodmain.Date = DateTime.Now;
            // 退貨初始狀態
            retgoodmain.Status = 0;
            // 退款銀行代號
            retgoodmain.BankName = bankid;
            // 退款分行
            retgoodmain.BankBranch = branches;
            // 退款帳號
            retgoodmain.AccountNO = bankaccount.ToString();
            retgoodmain.UpdateNote = DateTime.Now + "　退貨單成立　;<br>";
            retgoodList.Add(retgoodmain);
            dbafter.Retgood.Add(retgoodmain);
            dbafter.SaveChanges();

            return retgoodList;
        }

        /// <summary>
        /// 折價券狀態改變
        /// </summary>
        /// <param name="processesList"></param>
        /// <returns></returns>
        public void UpDataCoupon(List<string> processesList, int accID)
        {
            //List<TWNewEgg.Models.ViewModels.Redeem.Coupon> CouponList = dbbefore.Coupon.Where(x => processesList.Contains(x.ordcode)).ToList();
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> CouponList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByOrdCode", processesList).results;
            //List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = dbbefore.PromotionGiftRecords.Where(x => processesList.Contains(x.SalesOrderItemCode)).ToList();
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> PromotionGiftRecordsList = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", processesList).results;
            bool boolExec = false;

            if (CouponList.Count() > 0)
            {
                foreach (var Couponstemp in CouponList)
                {
                    Couponstemp.usestatus = (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel;
                    Couponstemp.updatedate = DateTime.Now;
                    Couponstemp.updateuser = Couponstemp.accountid;
                    Couponstemp.note = Couponstemp.note + " " + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "客戶取消訂單折價券作廢;";
                }
            }
            if (PromotionGiftRecordsList.Count() > 0)
            {
                foreach (var PromotionGiftRecordtemp in PromotionGiftRecordsList)
                {
                    PromotionGiftRecordtemp.UsedStatus = (int)TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords.UsedStatusOption.CancelUsed;
                    PromotionGiftRecordtemp.UpdateDate = DateTime.Now;
                    PromotionGiftRecordtemp.UpdateUser = accID.ToString();
                }

                boolExec = Processor.Request<bool, bool>("Service.PromotionGiftService.PromotionGiftRecordRepository", "UpdatePromotionGiftRecordByList", PromotionGiftRecordsList).results;
            }
            dbbefore.SaveChanges();
        }

        /// <summary>
        /// 問與答中心訂單相關
        /// </summary>
        /// <returns>頁面</returns>
        public ActionResult faqcenter()
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                // 使用者email
                string Accountemail = "";
                // 問題List
                List<Problem> Problemlist = new List<Problem>();

                Accountemail = dbbefore.Account.Where(x => x.ID == accID).Select(x => x.Email).FirstOrDefault();
                Problemlist = dbbefore.Problem.Where(x => x.AccountID == accID && x.Source == 1).OrderByDescending(x => x.Code).ToList();

                return View();
            }
            else
            {
                // 沒登入返回登入頁面
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/faqcenter" });
            }
        }

        /// <summary>
        /// 問與答中心其他相關
        /// </summary>
        /// <returns></returns>
        public ActionResult faqcenterother()
        {
            // List<DB.TWSQLDB.Models.DrawingList> DrawingList = new List<DB.TWSQLDB.Models.DrawingList>();
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                // 使用者email
                string Accountemail = "";
                // 問題List
                List<Problem> Problemlist = new List<Problem>();
                DateTime NowDate = DateTime.Now.Date;

                Accountemail = dbbefore.Account.Where(x => x.ID == accID).Select(x => x.Email).FirstOrDefault();
                Problemlist = dbbefore.Problem.Where(x => x.AccountID == accID && x.Source == 1).OrderByDescending(x => x.Code).ToList();

                return View();
            }
            else
            {
                // 沒登入返回登入頁面
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/faqcenterother" });
            }
        }

        /// <summary>
        /// 問與答詳細頁面
        /// </summary>
        /// <param name="prblm_prblmcode">問題編號</param>
        /// <returns></returns>
        public ActionResult FaQrecord(string prblm_prblmcode)
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                return View();
            }
            else
            {
                // 沒登入返回登入頁面
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/FaQrecord?prblm_prblmcode=" + prblm_prblmcode });
            }
        }

        /// <summary>
        /// 問與答其他詳細頁面
        /// </summary>
        /// <param name="prblm_prblmcode">問題編號</param>
        /// <returns></returns>
        public ActionResult FaQrecordother(string prblm_prblmcode)
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                return View();
            }
            else
            {
                // 沒登入返回登入頁面
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/FaQrecordother?prblm_prblmcode" + prblm_prblmcode });
            }
        }

        /// <summary>
        /// 問與答訂單相關問答頁面
        /// </summary>
        /// <param name="salesOrder_code">訂單SO編碼</param>
        /// <returns></returns>
        public ActionResult faq_form(string SalesOrder_code)
        {
            // 判斷是否登入
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                // 判斷訂單SO編碼是否輸入,若沒有輸入則以最後一張訂單為準
                if (SalesOrder_code == null)
                {
                    // 帳號資料
                    Account Account = new Account();
                    Account = dbbefore.Account.Where(x => x.ID == accID).FirstOrDefault();
                    // 客戶名稱
                    ViewBag.Viewname = Account.ACTName;
                    // 客戶email
                    ViewBag.Viewmobile = Account.Mobile;
                    return View();
                }
                else
                {
                    // SO訂單資料
                    SalesOrder SalesOrder = new SalesOrder();
                    // SOitem訂單資料
                    SalesOrderItem SalesOrderItem = new SalesOrderItem();
                    SalesOrder = dbbefore.SalesOrder.Where(x => x.Code == SalesOrder_code).FirstOrDefault();
                    SalesOrderItem = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == SalesOrder_code).FirstOrDefault();
                    // 客戶名稱
                    ViewBag.Viewname = SalesOrder.Name;
                    // 客戶電話
                    ViewBag.Viewmobile = SalesOrder.Mobile;
                    // 客戶購買物品
                    ViewBag.ViewSalesOrder_codeitem = SalesOrderItem.ItemID;
                    return View();
                }
            }
            else
            {
                // 返回登入頁面
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/FaQ_form" });
            }
        }

        /// <summary>
        /// 會員中心
        /// </summary>
        /// <returns></returns>
        public ActionResult myNewegg()
        {
            // 判斷是否登入,沒登入返回登入頁面
            int accID = CheckAccount();

            // 若登入則繼續,沒登入返回登入頁面
            if (accID > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/myNewegg" });
            }
        }

        public ActionResult myProcess()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                return View();
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/myNewegg" });
        }

        /// <summary>
        /// 得到帳戶email
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getAccount()
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();

            // 帳戶email
            string Accountemail = "";
            List<Account> Accountlist = new List<Account>();
            Accountemail = dbbefore.Account.Where(x => x.ID == realid).Select(x => x.Email).FirstOrDefault();
            return Json(Accountemail);

        }

        /// <summary>
        /// 問與答訂單頁面
        /// </summary>
        /// <param name="page">第N頁</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faqcenterapi(int page)
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();

            // 判斷是否登入,沒登入則回饋空白
            if (realid <= 0)
            {
                return Json("");
            }

            // 帳戶email
            string Accountemail = "";
            var i = 0;
            // 只撈取過去三個月的資料
            DateTime startday = DateTime.Now.AddMonths(-3);
            // 帳戶List
            List<Account> Accountlist = new List<Account>();
            // 問題List
            List<Problem> Problemlist = new List<Problem>();
            // Soitem List
            List<SalesOrderItem> SalesOrderItem = new List<SalesOrderItem>();
            Account Account = new Account();
            Accountemail = dbbefore.Account.Where(x => x.ID == realid).Select(x => x.Email).FirstOrDefault();
            //前台只顯示使用者詢問的信件
            Problemlist = dbbefore.Problem.Where(x => x.AccountID == realid && x.Source == 1).OrderByDescending(x => x.CreateDate).ToList();
            Problemlist = Problemlist.Where(x => x.CreateDate >= startday).ToList();
            Problemlist = Problemlist.Where(x => x.BlngCode != "").OrderByDescending(x => x.CreateDate).ToList();
            var ProblemlistGroup = Problemlist.GroupBy(x => x.BlngCode).ToList();

            // Json回饋字串
            string qnakey = "";

            for (i = 0; i < ProblemlistGroup.Count; i++)
            {
                // 判斷是否空字串,若不為空字串則加,
                if (qnakey != "")
                {
                    qnakey += ",";
                }

                string prblm_source = ProblemlistGroup[i].ToList()[0].BlngCode.ToString();
                string problem_prblmcode = "";
                int j;
                for (j = 0; j < ProblemlistGroup[i].Count(); j++)
                {
                    problem_prblmcode = problem_prblmcode + ProblemlistGroup[i].ToList()[j].Code.ToString() + "<br>";
                }

                string SalesOrderItem_name = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == prblm_source).Select(x => x.Name).FirstOrDefault();
                qnakey += "[{\"problem_prblmcode\" : \"" + ProblemlistGroup[i].ToList()[0].Code.ToString() + "\", \"prblm_createdate\" : \"" + ProblemlistGroup[i].ToList()[0].CreateDate.Value.Year.ToString("0000") + "/" + ProblemlistGroup[i].ToList()[0].CreateDate.Value.Month.ToString("00") + "/" + ProblemlistGroup[i].ToList()[0].CreateDate.Value.Day.ToString("00") + "\", \"prblm_source\" : \"" + prblm_source + "\", \"prblm_itemname\" : \"" + SalesOrderItem_name + "\", \"prblm_class\" : \"" + ProblemlistGroup[i].ToList()[0].IntClass.ToString() + "\"}]";

            }

            return Json(qnakey);

        }

        /// <summary>
        /// 問與答期它頁面
        /// </summary>
        /// <param name="page">第N頁</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faqcenterotherapi(int page)
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();

            // 判斷是否登入,沒登入則回饋空白
            if (realid <= 0)
            {
                return Json("");
            }

            // 帳戶email
            string Accountemail = "";
            int i = 0;

            // 只撈取過去三個月的資料
            DateTime startday = DateTime.Now.AddMonths(-3);
            // 帳戶List
            List<Account> Accountlist = new List<Account>();
            // 問題List
            List<Problem> Problemlist = new List<Problem>();
            // Soitem List
            List<SalesOrderItem> SalesOrderItem = new List<SalesOrderItem>();
            Account Account = new Account();
            Accountemail = dbbefore.Account.Where(x => x.ID == realid).Select(x => x.Email).FirstOrDefault();
            //前台只顯示使用者的信件
            Problemlist = dbbefore.Problem.Where(x => x.AccountID == realid && x.Source == 1).OrderByDescending(x => x.CreateDate).ToList();
            Problemlist = Problemlist.Where(x => x.CreateDate >= startday).ToList();
            Problemlist = Problemlist.Where(x => x.BlngCode == "").OrderByDescending(x => x.CreateDate).ToList();

            // Json回饋字串
            string qnakey = "";

            for (i = 0; i < Problemlist.Count; i++)
            {
                // 判斷是否空字串,若不為空字串則加,
                if (qnakey != "")
                {
                    qnakey += ",";
                }

                int? prblm_source = Problemlist[i].ItemID;
                string problem_prblmcode = "";
                int j;
                for (j = 0; j < Problemlist.Count; j++)
                {
                    problem_prblmcode = problem_prblmcode + Problemlist[i].Code.ToString() + "<br>";
                }

                string SalesOrderItem_name = dbbefore.Item.Where(x => x.ID == prblm_source).Select(x => x.Name).FirstOrDefault();

                // 判斷商品名稱是否存在
                if (SalesOrderItem_name == null)
                {
                    SalesOrderItem_name = "-----";
                }

                SalesOrderItem_name = Server.HtmlEncode(SalesOrderItem_name);
                qnakey += "[{\"problem_prblmcode\" : \"" + Problemlist[i].Code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[i].CreateDate.Value.Year.ToString("0000") + "/" + Problemlist[i].CreateDate.Value.Month.ToString("00") + "/" + Problemlist[i].CreateDate.Value.Day.ToString("00") + "\", \"prblm_source\" : \"" + Problemlist[i].ItemID + "\", \"prblm_itemname\" : \"" + SalesOrderItem_name + "\", \"prblm_class\" : \"" + Problemlist[i].IntClass.ToString() + "\"}]";

            }

            return Json(qnakey);

        }

        /// <summary>
        /// 問與答,問答獲取
        /// </summary>
        /// <param name="prblm_sourcecode">訂單編號</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faqanswerorderapi(string prblm_sourcecode)
        {
            // 判斷是否登入,沒登入返回登入頁面
            int realid = CheckAccount();
            // 問題List
            List<Problem> Problemlist = new List<Problem>();
            // 訂單List
            List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
            Problemlist = dbbefore.Problem.Where(x => x.BlngCode == prblm_sourcecode).ToList();
            // 訂單編碼
            string prblm_blngcode = Problemlist[0].BlngCode.ToString();
            SalesOrderlist = dbbefore.SalesOrder.Where(x => x.Code == prblm_blngcode).ToList();
            // 判斷定單是否存在,若不存在回"SalesOrderzero"
            if (SalesOrderlist.Count != 0)
            {
                // 回饋Json字串
                string qnaanstext = "";
                // 問題編碼
                string prblm_source = Problemlist[0].BlngCode.ToString();
                // 商品名稱
                string SalesOrderItem_name = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == prblm_source).Select(x => x.Name).FirstOrDefault();
                // 判斷商品名稱是否存在
                if (SalesOrderItem_name == null)
                {
                    SalesOrderItem_name = "---";
                }
                DateTime SalesOrderItem_date = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == prblm_source).Select(x => x.CreateDate).FirstOrDefault();
                int SalesOrderItem_itemid = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == prblm_source).Select(x => x.ItemID).FirstOrDefault();

                if (SalesOrderlist[0].CardADDR.Length > 5)
                {
                    SalesOrderlist[0].CardADDR = SalesOrderlist[0].CardADDR.Substring(0, SalesOrderlist[0].CardADDR.Length - 5).ToString() + "*****";
                }
                else
                {
                    SalesOrderlist[0].CardADDR = SalesOrderlist[0].CardADDR.Substring(0, SalesOrderlist[0].CardADDR.Length - Convert.ToInt32(Math.Ceiling(SalesOrderlist[0].CardADDR.Length / 1.5))).ToString() + "*****";
                }

                qnaanstext += "[{\"SalesOrder_code\" : \"" + Problemlist[0].BlngCode.ToString() + "\", \"SalesOrder_createdate\" : \"" + SalesOrderItem_date.Year.ToString("0000") + "/" + SalesOrderItem_date.Month.ToString("00") + "/" + SalesOrderItem_date.Day.ToString("00") + "　　" + SalesOrderItem_date.ToLongTimeString().ToString() + "\", \"SalesOrder_itemname\" : \"" + SalesOrderItem_name + "\", \"SalesOrder_itemid\" : \"" + SalesOrderItem_itemid.ToString() + "\",";
                qnaanstext += " \"SalesOrder_name\" : \"" + EscapeStringForJS(SalesOrderlist[0].Name).ToString() + "\", \"email\" : \"" + EscapeStringForJS(SalesOrderlist[0].Email).ToString() + "\", \"SalesOrder_mobile\" : \"" + SalesOrderlist[0].Mobile + "\", \"SalesOrder_address\" : \"" + SalesOrderlist[0].CardLOC.ToString() + SalesOrderlist[0].CardADDR + "\",";
                qnaanstext += " \"problem_cont\" : \"" + EscapeStringForJS(Problemlist[0].Cont).ToString() + "\",\"problem_problemcode\" : \"" + Problemlist[0].Code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[0].CreateDate.Value.Year.ToString("0000") + "/" + Problemlist[0].CreateDate.Value.Month.ToString("00") + "/" + Problemlist[0].CreateDate.Value.Day.ToString("00") + "　" + Problemlist[0].CreateDate.Value.ToLongTimeString().ToString() + "\", \"prblm_itemname\" : \"" + EscapeStringForJS(SalesOrderItem_name) + "\", \"prblm_class\" : \"" + Problemlist[0].IntClass.ToString() + "\"}]";

                return Json(qnaanstext);
            }
            else
            {
                //若不存在回"SalesOrderzero"
                return Json("SalesOrderzero");
            }
        }






        public ActionResult returnList()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                return RedirectToAction("recentOrders");
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/recentOrderFs" });
            }
        }
        /// <summary>
        /// 申請退貨通知信
        /// </summary>
        /// <param name="retgoodList"></param>
        public bool Mail_RetgoodMessage(List<Retgood> RetgoodList, string Recipient, string RecipientBcc, bool Bankinfo)
        {
            aesenc = new AesCookies();
            string path = Server.MapPath("~/Log/Mail/");
            //try
            //{
            ViewBag.Bankinfo = Bankinfo; // 是否能看到銀行資訊
            ViewBag.RetgoodList = RetgoodList;
            ViewBag.NewLinkTitle = NewLinkTitle;
            ViewBag.LoginDate = Convert.ToString(DateTime.Now); // 登錄日期
            ViewBag.retgood_causenote = ""; // 退貨原因
            //int Convert Retgood.reason 退貨原因
            if (RetgoodList.FirstOrDefault().Cause != null) 
            {
             ViewBag.retgood_causenote = (Retgood.reason)RetgoodList.FirstOrDefault().Cause;
            }

            ViewBag.retgood_toaddr = RetgoodList.FirstOrDefault().FrmZipcode + RetgoodList.FirstOrDefault().FrmLocation + RetgoodList.FirstOrDefault().FrmADDR; // 地址
            int? retgood_productid = RetgoodList.FirstOrDefault().ProductID; // 商品productid
            int? retgood_qty = RetgoodList.FirstOrDefault().Qty; // 數量
            ViewBag.retgood_itemname = ""; // table的retgood_productid為int 
            string ProcessID = RetgoodList.FirstOrDefault().ProcessID;
            int itenID = (int)dbafter.Process.Where(x => x.ID == ProcessID).Select(x => x.StoreID).FirstOrDefault();
            ViewBag.retgood_itemname = (from p in dbbefore.Item where p.ID == itenID select p.Name).FirstOrDefault();
            if (ViewBag.retgood_itemname == null || ViewBag.retgood_itemname == "")
            {
                ViewBag.retgood_itemname = (from p in dbbefore.Product where p.ID == retgood_productid select p.NameTW).FirstOrDefault();
                if (ViewBag.retgood_itemname == null || ViewBag.retgood_itemname == "")
                {
                    ViewBag.retgood_itemname = (from p in dbbefore.Product where p.ID == retgood_productid select p.Name).FirstOrDefault();
                }
            }
            string saleorderitemtemp = RetgoodList.FirstOrDefault().ProcessID;
            ViewBag.retgood_salesorderCODE = dbafter.Process.Where(x => x.ID == saleorderitemtemp).Select(x => x.CartID).FirstOrDefault();
            ViewBag.Itemlist = ""; //配件
            for (int i = 1; i < RetgoodList.Count; i++)
            {
                int? productid = RetgoodList[i].ProductID;
                ViewBag.Itemlist += (from p in dbbefore.Product where p.ID == productid select p.Name).FirstOrDefault();
                switch (RetgoodList[i].RetgoodType)
                {
                    case 0: ViewBag.Itemlist += "(配件)";
                        break;
                    case 20: ViewBag.Itemlist += "(贈品)";
                        break;
                }
                ViewBag.Itemlist += "*" + RetgoodList[i].Qty + "&nbsp;&nbsp;&nbsp;";
            }
            string Messageresult = "";

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_RetgoodMail");

                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                Messageresult = sw.GetStringBuilder().ToString();
            }

            //Recipient = AesEnc.AESdecrypt(Request.Cookies["em"].Value)+ ", grace.c.hsiao@newegg.com, david.y.chou@newegg.com";
            LogtoFileWrite(path, "Newegg" + WebSiteData.SiteName + "通知-申請退貨通知信！", Messageresult);

            return send_email(Messageresult, "Newegg" + WebSiteData.SiteName + "通知-申請退貨通知信！", Recipient, RecipientBcc);
            //}
            //catch { return false; }
        }

        public void LogtoFileWrite(string path, string MessageType, string writeStringendtoFile)
        {
            string filename = path + string.Format("{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd_HH-mm}_" + MessageType + ".txt", DateTime.Now);
            //string filename = path + string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd HH:mm:ss}_" + MessageType + ".txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getorderlist(string salesorder_code)
        {
            int i = 0;
            SalesOrder SalesOrder = new SalesOrder();
            SalesOrderItem SalesOrderItem = new SalesOrderItem();
            Cart Cart = new Cart();

            SalesOrder = dbbefore.SalesOrder.Where(x => x.Code == salesorder_code).FirstOrDefault();
            int CarttempGroup = SalesOrder.SalesOrderGroupID ?? 0;
            List<Cart> CartList = dbafter.Cart.Where(x => x.SalesorderGroupID == CarttempGroup && x.Status == 0 && (x.DelvStatus == (int)Cart.cartstatus.初始狀態 || x.DelvStatus == (int)Cart.cartstatus.已成立)).ToList();
            if (SalesOrder.Status != 0)
            {
                //return JavaScript("window.location = '" + Url.Action("Login", "account", new { returnUrl = "/myNewegg/CompanyBook" }) + "'");
                return Json("此訂單不能取消");
            }
            if (CartList.Count == 0)
            {
                return Json("此訂單不能取消");
            }
            List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
            List<SalesOrderItem> SalesOrderItemlist = new List<SalesOrderItem>();
            SalesOrderlist = dbbefore.SalesOrder.Where(x => x.SalesOrderGroupID == SalesOrder.SalesOrderGroupID).ToList();
            int itemid = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == salesorder_code).Select(x => x.ItemID).FirstOrDefault();
            int sellerid = dbbefore.Item.Where(x => x.ID == itemid).Select(x => x.SellerID).FirstOrDefault();
            int itemidtemp;
            int selleridtemp;
            string ordertext = "";
            string salesorder_codetemp = "";
            string salesorder_nametemp = "";

            List<object> returnData = new List<object>();

            if (SalesOrderlist.Count != 0)
            {
                for (i = 0; i < SalesOrderlist.Count; i++)
                {
                    SalesOrderItem SalesOrderitemext = new SalesOrderItem();
                    salesorder_codetemp = SalesOrderlist[i].Code;
                    SalesOrderitemext = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == salesorder_codetemp).FirstOrDefault();
                    salesorder_nametemp = Server.HtmlEncode(SalesOrderitemext.Name);
                    itemidtemp = dbbefore.SalesOrderItem.Where(x => x.SalesorderCode == salesorder_codetemp).Select(x => x.ItemID).FirstOrDefault();
                    selleridtemp = dbbefore.Item.Where(x => x.ID == itemidtemp).Select(x => x.SellerID).FirstOrDefault();
                    if (selleridtemp == sellerid)
                    {

                        if (salesorder_nametemp == "國際運費" || salesorder_nametemp == "服務費")
                        { }
                        else
                        {
                            if (ordertext != "")
                            { ordertext += ","; }
                            ordertext += "[{\"Salesorder_code\" : \"" + SalesOrderlist[i].Code.ToString() + "\", \"Salesorder_name\" : \"" + salesorder_nametemp.Replace("\"", "") + "\"}]";
                            ordertext += "[{\"Salesorder_code\" : \"" + SalesOrderlist[i].Code.ToString() + "\", \"Salesorder_name\" : \"" + salesorder_nametemp.Replace("\r", "").Replace("\n", "").Replace("\\", "") + "\"}]";
                            returnData.Add(new { Salesorder_code = SalesOrderlist[i].Code.ToString(), Salesorder_name = salesorder_nametemp });
                        }
                    }
                    else { }
                }
                return Json(returnData);
            }
            else
            {
                return Json("noproblem");
            }
        }

        /// <summary>
        /// 拋送訂單取消
        /// </summary>
        /// <param name="salesorder_code"></param>
        /// <param name="logger"></param>
        /// <param name="POItems"></param>
        private void NeweggRequestCancel(string salesorder_code, log4net.ILog logger, List<PurchaseOrderItem> POItems)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PRD")
                {
                    NeweggRequest neweggRequest = new NeweggRequest();
                    neweggRequest.Login();
                    foreach (var poi in POItems)
                    {
                        Message<UIOrderHistoryContent> OrderHistory;
                        logger.Debug("start taking OrderHistory of purchaseorderitem " + poi.Code + ", salesorder " + salesorder_code);
                        try
                        {
                            OrderHistory = neweggRequest.GetOrderHistory(poi.SellerOrderCode, 1);
                            if (OrderHistory == null || OrderHistory.Body == null || OrderHistory.Body.OrderSummaryList == null)
                            {
                                logger.Debug("can't find " + poi.Code + " with NeweggRequest, who's sellerordercode is " + poi.SellerOrderCode);
                                continue;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            logger.Error("error when taking OrderHistroy with NeweggRequest, Salesordercode = " + salesorder_code + ", sellerordercode is " + poi.SellerOrderCode, e);
                            throw new Exception("error when taking OrderHistroy with NeweggRequest, Salesordercode = " + salesorder_code);
                        }

                        var order = OrderHistory.Body.OrderSummaryList.SingleOrDefault(s => s.SONumber == Convert.ToInt32(poi.SellerOrderCode));
                        if (order == null)
                        {
                            logger.Debug("can't find " + poi.Code + " with NeweggRequest, who's sellerordercode is " + poi.SellerOrderCode);
                        }
                        else
                        {
                            logger.Debug("got ordersumarryList of " + poi.Code);
                            //不做是否可取消的預先判斷，一律拋送取消請求，交由API判斷取消是否成功
                            /*if (order.IsShowCancelButton)
                            {*/
                            logger.Debug("start sending CancelOrder request of " + poi.Code + " with NeweggRequest");
                            var result = neweggRequest.CancelOrder(Convert.ToInt32(poi.SellerOrderCode));
                            if (result != null)
                            {
                                //PO中寫入註記 (時間+ResponseCode)
                                try
                                {
                                    //update TWSQLDB
                                    DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                                    var po = db.PurchaseOrder.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                    po.Note2 += DateTime.UtcNow.AddHours(8).ToString() + "+" + result.Code;
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e.Message);
                                }
                                try
                                {
                                    //update TWBACKENDDB
                                    DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
                                    var po = backendDB.PurchaseOrderTWBACK.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                    po.Note2 += DateTime.UtcNow.AddHours(8).ToString() + "+" + result.Code;
                                    backendDB.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e.Message);
                                }

                                //判斷是否取消成功
                                if (result.Code != "000")
                                {
                                    logger.Debug("Purchaseorderitem_code " + poi.Code + " NeweggRequest CancelOrder " + result.Code);
                                }
                                else
                                {
                                    logger.Debug("Purchaseorderitem_code " + poi.Code + " NeweggRequest CancelOrder 000");
                                    //取消成功_更新PO狀態
                                    try
                                    {
                                        //update TWSQLDB
                                        DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                                        var po = db.PurchaseOrder.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                        if (po != null)
                                        {
                                            po.Status = (int)DB.TWSQLDB.Models.PurchaseOrder.status.取消;
                                        }
                                        var tempPOI = db.PurchaseOrderItem.Where(x => x.Code == poi.Code).FirstOrDefault();
                                        if (tempPOI != null)
                                        {
                                            tempPOI.Status = (int)DB.TWSQLDB.Models.PurchaseOrderItem.status.取消;
                                        }
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e.Message);
                                    }
                                    try
                                    {
                                        //update TWBACKENDDB
                                        DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
                                        var po = backendDB.PurchaseOrderTWBACK.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
                                        if (po != null)
                                        {
                                            po.Status = (int)DB.TWBACKENDDB.Models.PurchaseOrderTWBACK.status.取消;
                                        }
                                        var tempPOI = backendDB.PurchaseOrderitemTWBACK.Where(x => x.Code == poi.Code).FirstOrDefault();
                                        if (tempPOI != null)
                                        {
                                            tempPOI.Status = (int)DB.TWBACKENDDB.Models.PurchaseOrderitemTWBACK.status.取消;
                                        }
                                        backendDB.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e.Message);
                                    }
                                }
                            }
                            /*}
                            else
                            {
                                logger.Debug("Purchaseorderitem_code " + poi.Code + " doesn' show Cancel Button");
                            }*/
                        }
                    }
                }

                if (POItems.Count() == 0)
                {
                    logger.Debug("no purchaseorderitem cancelOrder requests have been sent, salesorder " + salesorder_code);
                }
                else
                {
                    logger.Debug("all the purchaseorderitem cancelOrder requests have been sent, salesorder " + salesorder_code);
                }
            }
            catch (Exception e)
            {
                logger.Debug("NeweggRequestCancel_Bad");
            }
        }

        public bool Mail_VendorCancelSO(List<SalesOrder> SalesOrders, string status, string BankName, string Branches, string Bankaccount)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                ViewBag.SalesOrders = SalesOrders;
                string SalesOrderList = "";
                foreach (var so in SalesOrders)
                {
                    SalesOrderList += so.Code + "，";
                }
                SalesOrderList = SalesOrderList.Substring(0, SalesOrderList.Length - 1);

                ViewBag.SalesOrderList = SalesOrderList;
                ViewBag.NewLinkTitle = NewLinkTitle;
                ViewBag.status = status; // 退貨原因
                ViewBag.CancelDate = DateTime.Now;
                int paytype = (int)SalesOrders.First().PayType;
                string Salesorder_paytype = "";
                string BankCode = SalesOrders.First().CardBank;
                string bankname = (from p in dbbefore.Bank where p.Code == BankCode select p.Name).FirstOrDefault();
                Salesorder_paytype += bankname + " ";
                switch (paytype)
                {
                    case 1:
                        Salesorder_paytype += "信用卡付款 (一次付清)"; // 信用卡
                        break;
                    case 3:
                        Salesorder_paytype += "信用卡分期付款 3期0利率";
                        break;
                    case 6:
                        Salesorder_paytype += "信用卡分期付款 6期0利率";
                        break;
                    case 10:
                        Salesorder_paytype += "信用卡分期付款 10期0利率";
                        break;
                    case 18:
                        Salesorder_paytype += "信用卡分期付款 18期0利率";
                        break;
                    case 24:
                        Salesorder_paytype += "信用卡分期付款 24期0利率";
                        break;
                    case 112:
                        Salesorder_paytype += "信用卡分期付款 12期";
                        break;
                    case 124:
                        Salesorder_paytype += "信用卡分期付款 24期";
                        break;
                    case 30:
                        Salesorder_paytype += "網路ATM";
                        break;
                    case 31:
                        Salesorder_paytype += "貨到付款"; // 新竹貨運
                        break;
                    case 32:
                        Salesorder_paytype += "超商付款";
                        break;
                    case 501:
                        Salesorder_paytype += "歐付寶";
                        break;
                    default:
                        break;
                }
                ViewBag.Paytype = Salesorder_paytype;
                ViewBag.BankName = BankName;
                ViewBag.Branches = Branches;
                ViewBag.Bankaccount = Bankaccount;
                string Messageresult = "";

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InnerCancelSO");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }
                string Recipient = "Gretchen.H.Yeh@newegg.com, Teresa.S.Li@newegg.com, Dolcee.J.Chang@newegg.com, Joyce.H.Hsiao@newegg.com";
                LogtoFileWrite(path, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Messageresult);
                return send_email(Messageresult, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Recipient, "");
            }
            catch { return false; }
        }

        public bool Mail_InnerCancelSO(List<SalesOrder> SalesOrders, string status, string BankName, string Branches, string Bankaccount)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                ViewBag.SalesOrders = SalesOrders;
                string SalesOrderList = "";
                foreach (var so in SalesOrders)
                {
                    SalesOrderList += so.Code + "-";
                    switch (so.DelivType)
                    {
                        case 0:
                            SalesOrderList += "切貨 ，";
                            break;
                        case 1:
                            SalesOrderList += "間配 ，";
                            break;
                        case 2:
                            SalesOrderList += "直配 ，";
                            break;
                        case 3:
                            SalesOrderList += "三角 ，";
                            break;
                        case 4:
                            SalesOrderList += "借賣網 ，";
                            break;
                        case 5:
                            SalesOrderList += "自貿區 ，";
                            break;
                        case 6:
                            SalesOrderList += "海外切貨 ，";
                            break;
                        case 7:
                            SalesOrderList += "B2C直配 ，";
                            break;
                        case 8:
                            SalesOrderList += "MKPL寄倉 ，";
                            break;
                        case 9:
                            SalesOrderList += "B2C寄倉 ，";
                            break;
                        default:
                            SalesOrderList += "其他未定義 ，";
                            break;
                    }
                }
                SalesOrderList = SalesOrderList.Substring(0, SalesOrderList.Length - 1);

                ViewBag.SalesOrderList = SalesOrderList;
                ViewBag.NewLinkTitle = NewLinkTitle;
                ViewBag.status = status; // 退貨原因
                ViewBag.CancelDate = DateTime.Now;
                int paytype = (int)SalesOrders.First().PayType;
                string Salesorder_paytype = "";
                string BankCode = SalesOrders.First().CardBank;
                string bankname = (from p in dbbefore.Bank where p.Code == BankCode select p.Name).FirstOrDefault();
                Salesorder_paytype += bankname + " ";
                switch (paytype)
                {
                    case 1:
                        Salesorder_paytype += "信用卡付款 (一次付清)"; // 信用卡
                        break;
                    case 3:
                        Salesorder_paytype += "信用卡分期付款 3期0利率";
                        break;
                    case 6:
                        Salesorder_paytype += "信用卡分期付款 6期0利率";
                        break;
                    case 10:
                        Salesorder_paytype += "信用卡分期付款 10期0利率";
                        break;
                    case 18:
                        Salesorder_paytype += "信用卡分期付款 18期0利率";
                        break;
                    case 24:
                        Salesorder_paytype += "信用卡分期付款 24期0利率";
                        break;
                    case 112:
                        Salesorder_paytype += "信用卡分期付款 12期";
                        break;
                    case 124:
                        Salesorder_paytype += "信用卡分期付款 24期";
                        break;
                    case 30:
                        Salesorder_paytype += "網路ATM";
                        break;
                    case 31:
                        Salesorder_paytype += "貨到付款"; // 新竹貨運
                        break;
                    case 32:
                        Salesorder_paytype += "超商付款";
                        break;
                    case 501:
                        Salesorder_paytype += "歐付寶";
                        break;
                    default:
                        break;
                }
                ViewBag.Paytype = Salesorder_paytype;
                ViewBag.BankName = BankName;
                ViewBag.Branches = Branches;
                ViewBag.Bankaccount = Bankaccount;
                string Messageresult = "";

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InnerCancelSO");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }

                string Recipient = "Gretchen.H.Yeh@newegg.com, Grace.C.Hsiao@newegg.com, Jasmine.C.Hsieh@newegg.com, Jessie.Y.Tseng@newegg.com, Steven.c.mao@newegg.com, Dolcee.J.Chang@newegg.com,Joyce.H.Hsiao@newegg.com";
                LogtoFileWrite(path, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Messageresult);
                return send_email(Messageresult, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Recipient, "");
            }
            catch { return false; }
        }

        public bool Mail_CancelSO(List<SalesOrder> SalesOrders)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                ViewBag.SalesOrders = SalesOrders;

                // salesorders 除國際運費及服務費的訂單
                string SOList_SOCode = "";
                foreach (var so in SalesOrders)
                {
                    if (so.Note != "國際運費" && so.Note != "服務費")
                    {
                        SOList_SOCode += so.Code + "，";
                    }
                }

                SOList_SOCode = SOList_SOCode.Substring(0, SOList_SOCode.Length - 1);
                ViewBag.SalesOrderCodes = SOList_SOCode;
                // salesorders 除國際運費及服務費的訂單
                ViewBag.NewLinkTitle = NewLinkTitle;
                ViewBag.CancelDate = DateTime.Now;
                int paytype = (int)SalesOrders.First().PayType;
                string Salesorder_paytype = "";
                switch (paytype)
                {
                    case 1:
                        Salesorder_paytype = "信用卡付款 (一次付清)"; // 信用卡
                        break;
                    case 3:
                        Salesorder_paytype = "信用卡分期付款 3期0利率";
                        break;
                    case 6:
                        Salesorder_paytype = "信用卡分期付款 6期0利率";
                        break;
                    case 10:
                        Salesorder_paytype = "信用卡分期付款 10期0利率";
                        break;
                    case 18:
                        Salesorder_paytype = "信用卡分期付款 18期0利率";
                        break;
                    case 24:
                        Salesorder_paytype = "信用卡分期付款 24期0利率";
                        break;
                    case 112:
                        Salesorder_paytype = "信用卡分期付款 12期";
                        break;
                    case 124:
                        Salesorder_paytype = "信用卡";
                        break;
                    case 30:
                        Salesorder_paytype = "WebATM";
                        break;
                    case 31:
                        Salesorder_paytype = "貨到付款"; // 新竹貨運
                        break;
                    case 32:
                        Salesorder_paytype = "超商付款";
                        break;
                    case 501:
                        Salesorder_paytype = "歐付寶";
                        break;
                    default:
                        Salesorder_paytype = "";
                        break;
                }
                ViewBag.Paytype = Salesorder_paytype;

                string Messageresult = "";

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_CancelSO");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }
                // 固定使用訂購者信箱
                int accountid = SalesOrders.First().AccountID;
                string Recipient = (from p in dbbefore.Account where p.ID == accountid select p.Email).FirstOrDefault();
                LogtoFileWrite(path, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Messageresult);
                return send_email(Messageresult, "Newegg" + WebSiteData.SiteName + "通知-訂單取消通知信！", Recipient, "");
            }
            catch { return false; }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faqanswerapi(string prblm_prblmcode, string prblm_sourcecode)
        {
            int realid = CheckAccount();

            int i = 0;
            int j = 0;
            //string realid = id.AESdecrypt(accountID);
            List<Problem> Problemlist = new List<Problem>();
            List<Answer> Answerlist = new List<Answer>();
            List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
            Account Account = new Account();
            Problemlist = dbbefore.Problem.Where(x => x.BlngCode == prblm_sourcecode && x.Source == 1).ToList();
            Answerlist = dbbefore.Answer.Where(x => x.PrblmCode == prblm_prblmcode).ToList();

            if (Problemlist.Count != 0)
            {
                string prblm_blngcode = Problemlist[0].BlngCode.ToString();
                SalesOrderlist = dbbefore.SalesOrder.Where(x => x.Code == prblm_blngcode).ToList();

                string qnaanstext = "";

                /*for (i = 0; i < Answerlist.Count; i++)
                {
                    if (qnaanstext != "")
                    { qnaanstext += ","; }

                    qnaanstext += "[{\"answer_date\" : \"" + Answerlist[i].answr_date.Value.Year.ToString("0000") + "/" + Answerlist[i].answr_date.Value.Month.ToString("00") + "/" + Answerlist[i].answr_date.Value.Day.ToString("00") + "　" + Answerlist[i].answr_date.Value.ToLongTimeString().ToString() + "\",\"answer_cont\" : \"" + Answerlist[i].answr_cont.ToString() + "\"}";

                    if (i == 0)
                    {
                        string prblm_source = Problemlist[0].prblm_blngcode.ToString();
                        string SalesOrderItem_name = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_name).FirstOrDefault();
                        if (SalesOrderItem_name == null)
                        { SalesOrderItem_name = "---"; }
                        DateTime SalesOrderItem_date = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_createdate).FirstOrDefault();
                        int SalesOrderItem_itemid = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_itemid).FirstOrDefault();
                        qnaanstext += ",{\"salesOrder_code\" : \"" + Problemlist[0].prblm_blngcode.ToString() + "\", \"SalesOrder_createdate\" : \"" + SalesOrderItem_date.Year.ToString("0000") + "/" + SalesOrderItem_date.Month.ToString("00") + "/" + SalesOrderItem_date.Day.ToString("00") + "　　" + SalesOrderItem_date.ToLongTimeString().ToString() + "\", \"SalesOrder_itemname\" : \"" + SalesOrderItem_name + "\", \"SalesOrder_itemid\" : \"" + SalesOrderItem_itemid.ToString() + "\",";
                        qnaanstext += " \"SalesOrder_name\" : \"" + SalesOrderlist[0].salesorder_name.ToString() + "\", \"email\" : \"" + SalesOrderlist[0].salesorder_email.ToString() + "\", \"SalesOrder_mobile\" : \"" + SalesOrderlist[0].salesorder_mobile + "\", \"SalesOrder_address\" : \"" + SalesOrderlist[0].salesorder_cardloc.ToString() + SalesOrderlist[0].salesorder_cardaddr.ToString() + "*****\",";
                        qnaanstext += " \"problem_cont\" : \"" + Problemlist[0].prblm_cont.ToString() + "\",\"problem_problemcode\" : \"" + Problemlist[0].prblm_code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[0].prblm_createdate.Value.Year.ToString("0000") + "/" + Problemlist[0].prblm_createdate.Value.Month.ToString("00") + "/" + Problemlist[0].prblm_createdate.Value.Day.ToString("00") + "　" + Problemlist[0].prblm_createdate.Value.ToLongTimeString().ToString() + "\", \"prblm_itemname\" : \"" + SalesOrderItem_name + "\", \"prblm_class\" : \"" + Problemlist[0].prblm_intclass.ToString() + "\"}]";
                    }
                    else
                    {
                        qnaanstext += "]";
                    }

                }
                qnaanstext = "";*/
                for (i = 0; i < Problemlist.Count; i++)
                {
                    prblm_prblmcode = Problemlist[i].Code.ToString();
                    Answerlist = dbbefore.Answer.Where(x => x.PrblmCode == prblm_prblmcode).ToList();
                    if (qnaanstext != "")
                    { qnaanstext += ","; }

                    qnaanstext += "[{\"problem_problemcode\" : \"" + Problemlist[i].Code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[i].CreateDate.Value.Year.ToString("0000") + "/" + Problemlist[i].CreateDate.Value.Month.ToString("00") + "/" + Problemlist[i].CreateDate.Value.Day.ToString("00") + "　" + Problemlist[i].CreateDate.Value.ToLongTimeString().ToString() + "\", \"prblm_class\" : \"" + Problemlist[i].IntClass.ToString() + "\",\"problem_cont\" : \"" + EscapeStringForJS(Problemlist[i].Cont).ToString() + "\"}";
                    if (Answerlist.Count != 0)
                    {
                        for (j = 0; j < Answerlist.Count; j++)
                        {
                            if (Answerlist[j].Cont.Trim() != "" && Answerlist[j].Cont.Trim() != null)
                            {
                                qnaanstext += ",{\"answer_date\" : \"" + Answerlist[j].Date.Value.Year.ToString("0000") + "/" + Answerlist[j].Date.Value.Month.ToString("00") + "/" + Answerlist[j].Date.Value.Day.ToString("00") + "　" + Answerlist[j].Date.Value.ToLongTimeString().ToString() + "\",\"answer_cont\" : \"" + EscapeStringForJS(Answerlist[j].Cont).ToString() + "\"}";
                            }
                        }
                        if (j == Answerlist.Count)
                        {
                            qnaanstext += "]";
                        }

                    }
                    else
                    {
                        qnaanstext += ",{\"answer_date\" : \"" + "" + "\",\"answer_cont\" : \"" + "尚無回應" + "\"}]";
                    }
                }
                return Json(qnaanstext);
            }
            else
            {
                return Json("noproblem");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faqanswerotherapi(string prblm_prblmcode, string prblm_sourcecode)
        {
            int realid = CheckAccount();

            int i = 0;
            int j = 0;
            //string realid = id.AESdecrypt(accountID);
            List<Problem> Problemlist = new List<Problem>();
            List<Answer> Answerlist = new List<Answer>();
            List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
            Account Account = new Account();
            Problemlist = dbbefore.Problem.Where(x => x.Code == prblm_prblmcode && x.Source == 1).ToList();
            Answerlist = dbbefore.Answer.Where(x => x.PrblmCode == prblm_prblmcode).ToList();
            string prblm_blngcode = Problemlist[0].BlngCode.ToString();
            SalesOrderlist = dbbefore.SalesOrder.Where(x => x.Code == prblm_blngcode).ToList();

            string qnaanstext = "";

            /*for (i = 0; i < Answerlist.Count; i++)
            {
                if (qnaanstext != "")
                { qnaanstext += ","; }

                qnaanstext += "[{\"answer_date\" : \"" + Answerlist[i].answr_date.Value.Year.ToString("0000") + "/" + Answerlist[i].answr_date.Value.Month.ToString("00") + "/" + Answerlist[i].answr_date.Value.Day.ToString("00") + "　" + Answerlist[i].answr_date.Value.ToLongTimeString().ToString() + "\",\"answer_cont\" : \"" + Answerlist[i].answr_cont.ToString() + "\"}";

                if (i == 0)
                {
                    string prblm_source = Problemlist[0].prblm_blngcode.ToString();
                    string SalesOrderItem_name = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_name).FirstOrDefault();
                    if (SalesOrderItem_name == null)
                    { SalesOrderItem_name = "---"; }
                    DateTime SalesOrderItem_date = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_createdate).FirstOrDefault();
                    int SalesOrderItem_itemid = dbbefore.SalesOrderItem.Where(x => x.salesorderitem_salesordercode == prblm_source).Select(x => x.salesorderitem_itemid).FirstOrDefault();
                    qnaanstext += ",{\"salesOrder_code\" : \"" + Problemlist[0].prblm_blngcode.ToString() + "\", \"SalesOrder_createdate\" : \"" + SalesOrderItem_date.Year.ToString("0000") + "/" + SalesOrderItem_date.Month.ToString("00") + "/" + SalesOrderItem_date.Day.ToString("00") + "　　" + SalesOrderItem_date.ToLongTimeString().ToString() + "\", \"SalesOrder_itemname\" : \"" + SalesOrderItem_name + "\", \"SalesOrder_itemid\" : \"" + SalesOrderItem_itemid.ToString() + "\",";
                    qnaanstext += " \"SalesOrder_name\" : \"" + SalesOrderlist[0].salesorder_name.ToString() + "\", \"email\" : \"" + SalesOrderlist[0].salesorder_email.ToString() + "\", \"SalesOrder_mobile\" : \"" + SalesOrderlist[0].salesorder_mobile + "\", \"SalesOrder_address\" : \"" + SalesOrderlist[0].salesorder_cardloc.ToString() + SalesOrderlist[0].salesorder_cardaddr.ToString() + "*****\",";
                    qnaanstext += " \"problem_cont\" : \"" + Problemlist[0].prblm_cont.ToString() + "\",\"problem_problemcode\" : \"" + Problemlist[0].prblm_code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[0].prblm_createdate.Value.Year.ToString("0000") + "/" + Problemlist[0].prblm_createdate.Value.Month.ToString("00") + "/" + Problemlist[0].prblm_createdate.Value.Day.ToString("00") + "　" + Problemlist[0].prblm_createdate.Value.ToLongTimeString().ToString() + "\", \"prblm_itemname\" : \"" + SalesOrderItem_name + "\", \"prblm_class\" : \"" + Problemlist[0].prblm_intclass.ToString() + "\"}]";
                }
                else
                {
                    qnaanstext += "]";
                }

            }
            qnaanstext = "";*/
            var ss = Server.HtmlEncode("\\");
            var aa = Server.HtmlEncode("'");
            var s1 = Server.HtmlEncode("\\\\");
            for (i = 0; i < Problemlist.Count; i++)
            {
                prblm_prblmcode = Problemlist[i].Code.ToString();
                Answerlist = dbbefore.Answer.Where(x => x.PrblmCode == prblm_prblmcode).ToList();
                Answerlist = Answerlist.Where(x => x.Cont != null && x.Cont != "").ToList();

                for (var Answerlisti = 0; Answerlisti < Answerlist.Count; Answerlisti++)
                {
                    if (Answerlist[Answerlisti].Cont.Trim() == "")
                    {
                        Answerlist.Remove(Answerlist[Answerlisti]);
                        Answerlisti--;
                    }
                }

                if (qnaanstext != "")
                { qnaanstext += ","; }

                qnaanstext += "[{\"problem_problemcode\" : \"" + Problemlist[i].Code.ToString() + "\", \"prblm_createdate\" : \"" + Problemlist[i].CreateDate.Value.Year.ToString("0000") + "/" + Problemlist[i].CreateDate.Value.Month.ToString("00") + "/" + Problemlist[i].CreateDate.Value.Day.ToString("00") + "　" + Problemlist[i].CreateDate.Value.ToLongTimeString().ToString() + "\", \"prblm_class\" : \"" + Problemlist[i].IntClass.ToString() + "\",\"problem_cont\" : \"" + EscapeStringForJS(Problemlist[i].Cont).ToString() + "\"}";
                if (Answerlist.Count != 0)
                {
                    for (j = 0; j < Answerlist.Count; j++)
                    {
                        if (Answerlist[j].Cont != null && Answerlist[j].Cont.Trim() != "")
                        {
                            qnaanstext += ",{\"answer_date\" : \"" + Answerlist[j].Date.Value.Year.ToString("0000") + "/" + Answerlist[j].Date.Value.Month.ToString("00") + "/" + Answerlist[j].Date.Value.Day.ToString("00") + "　" + Answerlist[j].Date.Value.ToLongTimeString().ToString() + "\",\"answer_cont\" : \"" + EscapeStringForJS(Answerlist[j].Cont).ToString() + "\"}";
                        }
                    }
                    if (j == Answerlist.Count)
                    {
                        qnaanstext += "]";
                    }

                }
                else
                {
                    qnaanstext += ",{\"answer_date\" : \"" + "" + "\",\"answer_cont\" : \"" + "尚無回應" + "\"}]";
                }
            }
            return Json(qnaanstext);
        }

        public static string EscapeStringForJS(string s)
        {
            //REF: http://www.javascriptkit.com/jsref/escapesequence.shtml
            return s.Replace("\f", @" ")
                    .Replace("\n", @"<br>")
                    .Replace("\0", @" ")
                    .Replace("\r", @"<br>")
                    .Replace("\t", @" ")
                    .Replace("\v", @" ")
                    .Replace("\"", @"&quot;")
                    .Replace("'", @"&lsquo;")
                    .Replace(@"\", @"\\");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult faq_form(string SalesOrder_code, int? itemid, string name, string mobile, string email, short? faqtypeval, string maintext)
        {
            int realid = CheckAccount();

            if (SalesOrder_code != "")
            {
                List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
                SalesOrderlist = dbbefore.SalesOrder.Where(x => x.Code == SalesOrder_code).ToList();


                if (SalesOrderlist.Count > 0)
                {
                    if (SalesOrderlist.FirstOrDefault().AccountID == realid)
                    {
                        Problem Problem = new Problem();
                        Problem.Code = dbbefore.GetAutoSN("PR");
                        Problem.AccountID = realid;
                        Problem.BlngCode = SalesOrder_code;
                        Problem.ItemID = itemid;
                        Problem.Name = name;
                        Problem.TEL = mobile;
                        Problem.Email = email;
                        Problem.IntClass = faqtypeval;
                        Problem.Cont = maintext.Replace("\n", "<br>");
                        Problem.Source = (int)TWNewEgg.DB.TWSQLDB.Models.Problem.pbsource.信件;
                        Problem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Problem.pbstatus.未處理;
                        Problem.StatusDate = DateTime.Now;
                        Problem.CreateDate = DateTime.Now;

                        dbbefore.Problem.Add(Problem);
                        dbbefore.SaveChanges();
                        return Json("1");
                    }
                    else
                    {
                        return Json("SalesOrder_Account_error");
                    }
                }
                else
                {
                    return Json("SalesOrder_code_error");
                }
            }
            else
            {
                Problem Problem = new Problem();
                Problem.Code = dbbefore.GetAutoSN("PR");
                Problem.AccountID = realid;
                Problem.BlngCode = SalesOrder_code;
                Problem.ItemID = itemid;
                Problem.Name = name;
                Problem.TEL = mobile;
                Problem.Email = email;
                Problem.IntClass = faqtypeval;
                Problem.Cont = maintext.Replace("\n", "<br>");
                Problem.Source = (int)TWNewEgg.DB.TWSQLDB.Models.Problem.pbsource.信件;
                Problem.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Problem.pbstatus.未處理;
                Problem.StatusDate = DateTime.Now;
                Problem.CreateDate = DateTime.Now;

                dbbefore.Problem.Add(Problem);
                dbbefore.SaveChanges();
                return Json("1");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SOCheck(string SalesOrder_code)
        {
            int realid = CheckAccount();

            if (SalesOrder_code != "" && SalesOrder_code != null)
            {
                List<SalesOrder> SalesOrderlist = new List<SalesOrder>();
                SalesOrderlist = dbbefore.SalesOrder.Where(x => x.Code == SalesOrder_code).ToList();

                if (SalesOrderlist.Count > 0)
                {
                    if (SalesOrderlist.FirstOrDefault().AccountID == realid)
                    {
                        return Json("1");
                    }
                    else
                    {
                        return Json("SalesOrder_Account_error");
                    }
                }
                else
                {
                    return Json("SalesOrder_code_error");
                }
            }
            else
            {
                return Json("1");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult get_bankinfo()
        {

            SalesOrder SalesOrder = new SalesOrder();
            List<Bank> Banklist = new List<Bank>();
            List<Models.Bankinfo> Bankinfolist = new List<Models.Bankinfo>();
            Banklist = dbbefore.Bank.Where(x => x.Code != "***").OrderBy(x => x.Code).ToList();
            Banklist = Banklist.Where(x => x.Code != "978").OrderBy(x => x.Code).ToList();

            var j = 0;
            //Banklist.ForEach(x => Banklist = x.bank_code );
            for (var i = 0; i < Banklist.Count; i++)
            {
                if (Banklist[i].Code.Length <= 3)
                {
                    Bankinfo Bankinfo = new Models.Bankinfo();
                    Bankinfolist.Add(Bankinfo);
                    Bankinfolist[j].Bank_id = Banklist[i].Code;
                    Bankinfolist[j].Bank_name = Banklist[i].Name;
                    j++;
                }
                else { }
            }
            //Bankinfolist. = 

            return Json(Bankinfolist);
        }

        /// <summary>
        /// emailManage page
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        public ActionResult EmailManage(string newEmail, string Message)
        {
            aesenc = new AesCookies();
            ViewBag.Success = "";
            if (Request.Cookies["em"] == null)
            {
                AccountController accController = new AccountController();
                accController.ClearCookie(Request, Response); // 清除cookies
                return RedirectToAction("Login", "Account");
            }
            if (newEmail == null)
            {
                ViewBag.Email = aesenc.AESdecrypt(Request.Cookies["em"].Value);
                return View();
            }
            else
            {
                TWSqlDBContext db = new TWSqlDBContext();
                var checkNewMail = (from p in db.Account where p.Email == newEmail select p).FirstOrDefault();
                if (checkNewMail != null)
                {
                    ViewBag.Message = "此帳號已存在請再次確認";
                    ViewBag.Email = aesenc.AESdecrypt(Request.Cookies["em"].Value);
                    return View();
                    //return RedirectToAction("EmailManage", "MyNewegg", new { Message = "此帳號已存在請再次確認" });
                }
                else
                {
                    Account account = new Account();
                    string emailNow = aesenc.AESdecrypt(Request.Cookies["em"].Value);
                    account = (from p in db.Account where p.Email == emailNow select p).FirstOrDefault();
                    if (account != null)
                    {
                        ViewBag.Success = "";
                        string EmailCheck = account.Email;
                        ViewBag.Email = aesenc.AESdecrypt(Request.Cookies["em"].Value);
                        if (Request.Cookies[EmailCheck + "e"] == null)
                        {
                            int rand;
                            char pd;
                            string newlinks = String.Empty;
                            // 生成重設密碼用的連結路徑  
                            System.Random random = new Random();
                            for (int i = 0; i < 15; i++)
                            {
                                rand = random.Next();
                                if (rand % 3 == 0)
                                {
                                    pd = (char)('A' + (char)(rand % 26));
                                }
                                else if (rand % 3 == 1)
                                {
                                    pd = (char)('a' + (char)(rand % 26));
                                }
                                else
                                {
                                    pd = (char)('0' + (char)(rand % 10));
                                }

                                newlinks += pd.ToString();
                            }
                            Response.Cookies["NewLinks"].Value = newlinks;
                            account.NewLinks = newlinks;
                            account.Email2 = newEmail;
                            db.SaveChanges();

                            if (Mail_ReSetEmail(account))
                            {
                                Response.Cookies[EmailCheck].Value = HttpUtility.UrlEncode(System.DateTime.Now.AddMinutes(3).ToString());
                                Response.Cookies[EmailCheck].Expires = System.DateTime.Now.AddMinutes(3);
                                Response.Cookies[EmailCheck + "e"].Value = HttpUtility.UrlEncode(System.DateTime.Now.AddMinutes(3).ToString());
                                Response.Cookies[EmailCheck + "e"].Expires = System.DateTime.Now.AddMinutes(3);
                                ViewBag.Success = "Success";
                                ViewBag.NewMessage = newEmail;
                                //ViewBag.err = "帳號修改鏈結已發送至您會員設定信箱中";
                            }
                            else
                            {
                                ViewBag.Success = "Failure";
                                ViewBag.NewMessage = "帳號修改鏈結發送失敗，請重新設定";
                            }
                        }
                        else
                        {
                            try
                            {
                                var MailDate = Convert.ToDateTime(HttpUtility.UrlDecode(Request.Cookies[EmailCheck + "e"].Value));
                                var NowDate = DateTime.Now;
                                var timeDiff = MailDate.Subtract(NowDate).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
                                if (timeDiff.Minutes > 0)
                                {
                                    ViewBag.Message = "重複發信，請在" + timeDiff.Minutes + "分鐘" + timeDiff.Seconds + "秒後再嘗試";
                                }
                                else
                                {
                                    ViewBag.Message = "重複發信，請在" + timeDiff.Seconds + "秒後再嘗試";
                                }
                            }
                            catch { }
                        }

                        //RedirectToAction("myAccount", "myNewegg");
                        return View();
                    }
                    else
                    {
                        // 回傳系統錯誤
                        return RedirectToAction("myAccount", "myNewegg", new { err = "系統錯誤，請重新設定" });
                    }
                }
            }
            //return View("myAccount");
        }
        /// <summary>
        /// 帳號修改信
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool Mail_ReSetEmail(Account account)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                //var tt = System.Configuration.ConfigurationManager.AppSettings["Images"];
                ViewBag.NewLinkTitle = NewLinkTitle;
                string Messageresult = "";
                // 將URL與信件做連結
                ViewBag.url = "https://" + ECDomain2 + "/MyNewegg/ReSetMailAddr?NewLinks=" + account.NewLinks + "&Email=" + account.Email2; // 重設新密碼的路徑
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_changeMail");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }

                string Recipient = account.Email2;
                return send_email(Messageresult, " Newegg" + WebSiteData.SiteName + "通知-請立刻按[確認]－啟動您修改後的新會員帳號！", Recipient, "");
            }
            catch { return false; }
        }
        /// <summary>
        /// 通知信發送函式
        /// </summary>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool send_email(string MailMessage, string mysubject, string Recipient, string RecipientBcc) // 信件訊息，信件主旨，收件人，密件收件人
        {
            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收件者
                msg.To.Add(Recipient);
                // msg.CC.Add("c@msn.com"); // 副本
                if (RecipientBcc != "")
                {
                    msg.Bcc.Add(RecipientBcc); // 密件副本
                }
                // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
                msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
                msg.Subject = mysubject; // 郵件主旨
                msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
                msg.Body = MailMessage; // 郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
                msg.IsBodyHtml = true; // 是否為HTML郵件
                msg.Priority = MailPriority.Normal; // 郵件優先等級
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                //SmtpClient MySmtp = new SmtpClient("172.22.5.55", 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 修改帳號
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ActionResult ReSetMailAddr(Account account)
        {
            TWSqlDBContext db = new TWSqlDBContext();
            aesenc = new AesCookies();
            // 清除所有曾經寫入過的資料
            int checkaccount = 0;

            //FormsAuthentication.SignOut();
            /*
            if (Request.Cookies["newEgg_Login"] != null)
            {
                HttpCookie rCookie = new HttpCookie("newEgg_Login");
                rCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(rCookie);
            }*/
            string mainDomain = "";
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (environment == "DEV")
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
            }
            else
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
            }
            if (Request.Cookies["Accountid"] != null)
            {
                IGetInfo CheckAccount = new GetInfoRepository();
                string[] plainText = CheckAccount.Decoder(Request.Cookies["Accountid"].Value, false);
                if (plainText.Length < 2)
                {
                    return RedirectToAction("Index", "Home");
                }
                checkaccount = Convert.ToInt32(plainText[0]);
                //var LocalAccountid = (from p in db.account where p.account_id == checkaccount select p).FirstOrDefault();
                //LocalAccountid.account_loginstatus = 0;
                ////LocalAccountid.account_newlinks = null;
                //
                //db.SaveChanges();
            }
            if (Request.Cookies["Accountid"] != null)
            {
                HttpCookie accountidcookie = new HttpCookie("Accountid");
                accountidcookie.Domain = mainDomain;
                accountidcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(accountidcookie);
            }
            if (Request.Cookies["LoginStatus"] != null)
            {
                HttpCookie loginstatus = new HttpCookie("LoginStatus");
                loginstatus.Domain = mainDomain;
                loginstatus.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(loginstatus);
            }
            if (Request.Cookies["IE"] != null)
            {
                HttpCookie IE = new HttpCookie("IE");
                IE.Domain = mainDomain;
                IE.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(IE);
            }
            if (Request.Cookies["em"] != null)
            {
                HttpCookie emailcookie = new HttpCookie("em");
                emailcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(emailcookie);
            }
            if (Request.Cookies["ex"] != null)
            {
                HttpCookie expirescookie = new HttpCookie("ex");
                expirescookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(expirescookie);
            }
            if (Request.Cookies["NewLinks"] != null)
            {
                HttpCookie newlinkscookie = new HttpCookie("NewLinks");
                newlinkscookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(newlinkscookie);

            }
            /*
            if (Request.Cookies["ValidateCode"] != null)
            {
                HttpCookie validatecodecookie = new HttpCookie("ValidateCode");
                validatecodecookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(validatecodecookie);
            }
            */
            // 清除所有曾經寫入過的資料
            // 更新新帳號

            if (account.NewLinks != "" && account.Email != "")
            {
                var checkAccount = (from p in db.Account where p.NewLinks == account.NewLinks && p.Email2 == account.Email select p).FirstOrDefault();
                if (checkAccount != null && checkAccount.Email2 != null)
                {
                    try
                    {
                        checkAccount.Email = checkAccount.Email2;
                        checkAccount.Email2 = null;
                        if (checkAccount.Istosap == 1)
                        {
                            checkAccount.ActionCode = "U";
                            checkAccount.Istosap = 0;
                        }
                        Response.Cookies["em"].Value = aesenc.AESenprypt(checkAccount.Email);
                        Response.Cookies["NewLinks"].Value = account.NewLinks;
                        Response.Cookies["ex"].Value = aesenc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
                        //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
                        //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(1440);
                        //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);
                        Response.Cookies["Password"].Value = checkAccount.PWD;
                        db.SaveChanges();
                    }
                    catch
                    {
                        return RedirectToAction("Login", "Account", new { account_confirm = "驗證錯誤，請重新至管理信箱設定" });
                    }
                    return RedirectToAction("AutoLogin", "Account", new { Message = "帳號更新成功", newmail = checkAccount.Email, pwd = checkAccount.PWD });
                    //return RedirectToAction("AutoLogin", "account", new { model = checkAccount, Message = "帳號更新成功" });
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { account_confirm = "驗證錯誤，請重新至管理信箱設定" });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult passwordChange(string now_account_pwd, string new_account_pwd, string account_confirmpassword)
        {
            //檢查登入
            if (Request.Cookies["em"] == null)
            {
                AccountController accController = new AccountController();
                accController.ClearCookie(Request, Response); // 清除cookies
                return RedirectToAction("Login", "Account");
            }

            /* ------ 驗證密碼用的物件宣告 ------ */
            AccountVerify objAccountVertify = null;
            Account objAccount = null;
            string email = "";
            bool boolExec = false;//修改密碼的結果
            /* ------ 修改密碼折價券活動用的物件宣告 ------ */
            int numEventId = 0;
            
            TWNewEgg.Models.ViewModels.Redeem.Event objActiveEvent = null;
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon = null;
            string strCouponMessage = "";
            Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption objGetCouponStatus;

            /* ------ start of 修改密碼 ------ */
            // 判斷輸入密碼是否一致
            if ((now_account_pwd == null || new_account_pwd == null || account_confirmpassword == null) || !(new_account_pwd.Equals(account_confirmpassword)))
            {
                return View();
            }

            objAccountVertify = new AccountVerify();
            aesenc = new AesCookies();
            email = aesenc.AESdecrypt(Request.Cookies["em"].Value);

            //判斷密碼格式
            if (objAccountVertify.VerifyAccountRule(new_account_pwd, email).IndexOf("NoPass") >= 0)
            {
                ViewBag.err = "密碼格式錯誤，請重新輸入!";
                return View();
            }

            //驗證舊密碼登入
            objAccount = objAccountVertify.VerifyAccountLogin(email, now_account_pwd, true);
            if (objAccount == null)
            {
                //若登入失敗
                return View();
            }

            //以上三個判斷皆通過, 進行修改密碼
            boolExec = objAccountVertify.UpdateAccountPassword(email, now_account_pwd, new_account_pwd);
            //修改密碼成功
            if (boolExec)
            {
                /* ------ start of 修改密碼給予折價券 ------ */
                //依環境判斷活動ID, GQC為41, 正式環境為178
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"].ToUpper().Equals("GQC"))
                    numEventId = 41; //活動Id
                else
                    numEventId = 178; //活動Id
                
                //objActiveEvent = objEventService.GetActiveEventById(numEventId);
                objActiveEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.EventReponsitory", "GetActiveEventById", numEventId).results;
                //2014/9/10前註冊的會員才能參加這次的活動
                if (objAccount.Registeron != null && DateTime.Compare(Convert.ToDateTime(objAccount.Registeron), Convert.ToDateTime("2014/9/10")) <= 0 && objActiveEvent != null)
                {
                    //listCoupon = objCouponService.GetCouponByAccountIdAndEventId(objAccount.ID.ToString(), numEventId);
                    listCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByAccountIdAndEventId", objAccount.ID.ToString(), numEventId).results;
                    if (listCoupon == null || listCoupon.Count <= 0)
                    {
                        //objGetCouponStatus = objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, objAccount.ID.ToString());
                        objGetCouponStatus = Processor.Request<Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption, Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", numEventId, objAccount.ID.ToString()).results;
                        if (objGetCouponStatus.Equals(Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption.發送成功))
                        {
                            strCouponMessage = "折價券領取成功!";
                        }
                        else if (!objGetCouponStatus.Equals(Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption.User已領過))
                        {
                            //非「已領過」,其他原因都顯示失敗
                            strCouponMessage = "折價券領取失敗!";
                        }//end if
                    }
                    else
                    {
                        //"已領過", 則不顯示任何訊息
                        strCouponMessage = "";
                    }
                }
                else
                {
                    //不顯示任何Coupon訊息
                    strCouponMessage = "";
                }
                /* ------ end of 修改密碼給予折價券 ------ */

                return RedirectToAction("myAccount", "myNewegg", new { err = "密碼更新成功!" + strCouponMessage });
            }
            else
            {
                //修改密碼失敗
                return RedirectToAction("myAccount", "myNewegg", new { err = "密碼更新失敗!" });
            }
        }

        public ActionResult suggestItem(int type, int count)
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                Account AccountGroup = (from p in dbbefore.Account where p.ID == accID select p).FirstOrDefault();
                int AG = AccountGroup.Subscribe == null ? 0 : (int)AccountGroup.Subscribe;
                List<int> SuggestItemIDs = new List<int>();
                List<MyNewEgg> SuggestItems = new List<MyNewEgg>();
                SuggestItems = (from i in dbbefore.MyNewEgg
                                where i.Type == type && i.MyNeweggOrder != 0
                                    && i.DateStart <= DateTime.Now && i.DateEnd >= DateTime.Now && i.GroupID == AG
                                select i).OrderBy(x => x.MyNeweggOrder).ToList();
                // 額外，order為0
                List<MyNewEgg> SIs = (from i in dbbefore.MyNewEgg
                                      where i.Type == type && i.MyNeweggOrder == 0
                                          && i.DateStart <= DateTime.Now && i.DateEnd >= DateTime.Now && i.GroupID == AG
                                      select i).ToList();
                if (SIs.Count > 0)
                {
                    foreach (var row in SIs)
                    {
                        SuggestItems.Add(row);
                    }
                }
                foreach (var row in SuggestItems)
                {
                    SuggestItemIDs.Add(row.ItemID);
                }
                // 額外，order為0 -- end
                List<Item> Suggest = dbbefore.Item.Where(i => SuggestItemIDs.Contains(i.ID)).ToList();
                List<ItemPreview> model = new List<ItemPreview>();
                foreach (var id in SuggestItemIDs)
                {
                    try
                    {
                        model.Add(new ItemPreview(Suggest.Single(r => r.ID == id)));
                    }
                    catch (InvalidOperationException e)
                    {
                        continue;
                    }
                }
                model = model.Take(count).ToList();
                // 進入到已寫好的PartialView中
                return PartialView("recentViewed", model);
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/myNewegg" });
        }

        public string GetRegoodNO(string Retype)
        {
            //List<GetItemTaxDetail> ItemTaxDetail = new List<GetItemTaxDetail>();
            TWSqlDBContext db_SP = new TWSqlDBContext();


            db_SP.Database.Initialize(force: false);
            var cmd = db_SP.Database.Connection.CreateCommand();

            if (Retype == "Regood")
            {
                cmd.CommandText = "exec [dbo].[UP_EC_RegoodGetNumber]";
            }
            else if (Retype == "Refund2c")
            {
                cmd.CommandText = "exec [dbo].[UP_EC_Refund2cGetNumber]";
            }

            try
            {
                db_SP.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                TWNewEgg.Website.ECWeb.Models.DbQuery nDb = null;
                DataSet NdsResult = null; //getSalesOrderNumByDate
                nDb = new TWNewEgg.Website.ECWeb.Models.DbQuery();

                NdsResult = nDb.Query(cmd.CommandText);

                DataTable NdtItem = null; //getSalesOrderNumByDate
                string RetypeNO = null;
                if (NdsResult != null && NdsResult.Tables.Count > 0)
                {
                    NdtItem = NdsResult.Tables[0];

                    RetypeNO = Convert.ToString(NdtItem.Rows[0][0]);
                    /*foreach (DataRow dr in NdtItem.Rows)
                    {
                        RetypeNO = Convert.ToString(dr[0]);
                    }//end foreach*/
                }//end if (dsResult != null && dsResult.Tables.Count > 0)

                return RetypeNO;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ActionResult myCouponIndex()
        {

            int accID = CheckAccount();
            if (accID == 0)
            {
                return RedirectToAction("Logout", "Account");
            }

            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listActiveCoupon = null;   //已生效可使用的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listUsedCoupon = null; //已經使用過的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listExpiredCoupon = null; //過期的Coupon
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listWaitingForActivecoupon = null; //待生效的Coupon
            List<SalesOrderItem> listSalesOrderItem = null;
            List<string> listSalesOrderItemCode = null;
            SalesOrderItem oSalesOrderItem = null;
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>> oDictResult = null;
            DateTime dateTimeNow = DateTime.Now;

            if (CheckAccount() == 0)
            {
                return View(oDictResult);
            }


            oDictResult = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>>();
            // 將Coupon分類
            // 已生效但未使用
            //listActiveCoupon = oCouponService.getActiveCouponListByAccount(this.mstrAccountId);
            listActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", this.mstrAccountId).results;

            // 修改Categories的說明
            if (listActiveCoupon != null)
            {
                this.getCouponCategoriesDesc(ref listActiveCoupon);
            }
            oDictResult.Add(1, listActiveCoupon);

            // 已使用:僅列出3個月內的消費記錄
            //listUsedCoupon = oCouponService.getUsedCouponIn3MonthListByAccount(this.mstrAccountId);
            listUsedCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getUsedCouponIn3MonthListByAccount", this.mstrAccountId).results;
            if (listUsedCoupon != null && listUsedCoupon.Count > 0)
            {
                // 置換x.ordcode為顯示商品名稱, 而非消費者看不懂的FBS序號
                listSalesOrderItemCode = listUsedCoupon.Select(x => x.ordcode).ToList();
                oDb = new DB.TWSqlDBContext();
                listSalesOrderItem = oDb.SalesOrderItem.Where(x => listSalesOrderItemCode.Contains(x.Code)).ToList();
                if (listSalesOrderItem != null)
                {
                    foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listUsedCoupon)
                    {
                        oSalesOrderItem = listSalesOrderItem.FirstOrDefault(x => x.Code == oSubCoupon.ordcode);
                        if (oSalesOrderItem != null)
                            oSubCoupon.ordcode = "<div style='display:none;'>" + oSubCoupon.ordcode + "</div>" + oSalesOrderItem.Name;
                        // 判斷coupon的Used狀況, 若是「使用後訂單取消」也要特別顯示
                        if (oSubCoupon.usestatus == (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel)
                            oSubCoupon.ordcode += "<br><span style='color:red'>此項商品已取消訂購</span>";
                    }// end foreach
                }
                oDb.Dispose();
                // 修改Categories的說明
                this.getCouponCategoriesDesc(ref listUsedCoupon);
            }
            oDictResult.Add(2, listUsedCoupon);
            // 未使用但過期
            //listExpiredCoupon = oCouponService.getExpiredCouponListByAccount(this.mstrAccountId);
            listExpiredCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getExpiredCouponListByAccount", this.mstrAccountId).results;
            // 修改Categories的說明
            if (listExpiredCoupon != null)
            {
                this.getCouponCategoriesDesc(ref listExpiredCoupon);
            }
            oDictResult.Add(3, listExpiredCoupon);
            // 待生效 : Event設定可以使用coupon
            //listWaitingForActivecoupon = oCouponService.getWaitingActiveCouponByAccount(this.mstrAccountId);
            listWaitingForActivecoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getWaitingActiveCouponByAccount", this.mstrAccountId).results;
            // 修改Categories的說明
            if (listWaitingForActivecoupon != null)
            {
                this.getCouponCategoriesDesc(ref listWaitingForActivecoupon);
            }
            oDictResult.Add(4, listWaitingForActivecoupon);

            //釋放記憶體
            if (oDb != null)
                oDb.Dispose();
            listSalesOrderItem = null;
            listSalesOrderItemCode = null;
            oSalesOrderItem = null;

            //回傳值
            return View(oDictResult);
        }
        [HttpGet]
        public ActionResult Member_Specification(string Message)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Member_Specification(int? AgreePaper)
        {
            int accID = CheckAccount();
            if (accID == 0)
            {
                return RedirectToAction("Logout", "Account");
            }
            if (AgreePaper == null)
            {
                return RedirectToAction("Member_Specification", "MyNewegg");
            }
            if (AgreePaper.Value == 1)
            {
                var agreeStatus = dbbefore.Account.Where(x => x.ID == accID).FirstOrDefault();
                if (agreeStatus != null)
                {
                    agreeStatus.MemberAgreement = 1;
                    try
                    {
                        dbbefore.SaveChanges();
                        return RedirectToAction("Member_Only", "MyNewegg");
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Member_Specification", "MyNewegg");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }

            }

            return View();
        }
        public ActionResult Member_Only()
        {
            int accID = CheckAccount();
            if (accID == 0)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/Member_Only" });
            }
            var agreeStatus = dbbefore.Account.Where(x => x.ID == accID).Select(x => x.MemberAgreement).FirstOrDefault();
            if (agreeStatus == null || agreeStatus != 1)
            {
                return RedirectToAction("Member_Specification", "MyNewegg");
            }
            return View();
        }

        /// <summary>
        ///修改Coupon顯示的可使用類別
        /// </summary>
        /// <param name="CouponNumber"></param>
        /// <returns></returns>
        private void getCouponCategoriesDesc(ref List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon)
        {
            List<int> listEventId = null;
            List<TWNewEgg.Models.ViewModels.Redeem.Event> listEvent = null;
            TWNewEgg.Models.ViewModels.Redeem.Event oEvent = null;

            if (listCoupon == null)
                return;

            listEventId = listCoupon.Select(x => x.eventid).Distinct().ToList<int>();

            //listEvent = oDb.Event.Where(x => listEventId.Contains(x.id)).ToList<Event>();
            listEvent = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Event>, List<TWNewEgg.Models.DomainModels.Redeem.Event>>("Service.CouponService.EventReponsitory", "GetEvents", listEventId).results;

            if (listEvent != null)
            {
                foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listCoupon)
                {
                    oEvent = listEvent.FirstOrDefault(x => x.id == oSubCoupon.eventid);
                    if (oEvent == null)
                        continue;

                    if (oEvent.limitdescription.Length > 0)
                        oSubCoupon.categories = oEvent.limitdescription;
                    else if (oSubCoupon.categories.Equals(";0;"))
                        oSubCoupon.categories = "全館";
                    else
                        oSubCoupon.categories = "部份商品";
                }//end foreach
            }

            //釋放記憶體
            listEvent = null;
            listEventId = null;
        }

        /// <summary>
        /// 取得Item的最高Category類別ID
        /// </summary>
        /// <param name="arg_nCategoryId"></param>
        /// <param name="arg_nGate">避免陷入無限迴圈的控制</param>
        /// <param name="listResult">該Category的遍歷Category</param>
        /// <returns>該Category最上層的祖先</returns>
        private int getRootCategoryId(int arg_nCategoryId, int arg_nGate, ref List<int> listResult, ref List<Category> listAllCategory)
        {
            if (arg_nCategoryId <= 0 || arg_nGate >= 20)
                return -1;

            arg_nGate++;

            Category oCategory = null;
            int nParentCategoryId = -1;

            oCategory = listAllCategory.Where(x => x.ID == arg_nCategoryId).SingleOrDefault();

            if (oCategory == null)
                return -1;
            nParentCategoryId = oCategory.ParentID;
            oCategory = null;
            if (nParentCategoryId == 0)
            {
                return arg_nCategoryId;
            }
            else
            {
                listResult.Add(nParentCategoryId);
                return this.getRootCategoryId(nParentCategoryId, arg_nGate, ref listResult, ref listAllCategory);
            }

        }

        /// <summary>
        /// 地址簿
        /// </summary>
        /// <returns></returns>
        public ActionResult MyNeweggAddressBook()
        {
            int accID = CheckAccount();
            if (accID > 0)
            {
                List<Addressbook> books = dbbefore.Addressbook.Where(a => a.AccountID == accID).OrderBy(a => a.ID).ToList();
                if (Request.IsAjaxRequest())
                {
                    ViewData.Model = books;
                    string partialView = "Partial_MyNeweggAddressBook";
                    return Json(renderView(partialView), JsonRequestBehavior.AllowGet);
                }
                ViewBag.tabOrder = 3;
                ViewBag.Content = "Partial_MyNeweggAddressBook";
                return PartialView("myAccountContent", books);
            }
            else if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/myNewegg/MyNeweggAddressBook" });
            }
            else
            {
                return JavaScript("window.location = '" + Url.Action("Login", "Account", new { returnUrl = "/myNewegg/MyNeweggAddressBook" }) + "'");
            }
        }


    }//end class
}//end namespace
