using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
//using Newegg.Framework.Web;
//using Newegg.Framework.Web.Cookie;
using System.Text.RegularExpressions;
using TWNewEgg.Website.ECWeb.Service;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using TWNewEgg.DB;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.InternalSendMail.Model;
using TWNewEgg.Redeem.Service.ActivityCheckService;
using TWNewEgg.InternalSendMail.Service;
using TWNewEgg.InternalSendMail.Model.SendMailModel;
using System.Threading;
using TWNewEgg.GetConfigData.Service;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.Website.ECWeb.Controllers
{
    public class AccountController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);

    
        //[Authorize]
        /*
        // GET: /Login/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("Login",new Models.Base.User());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(Models.Base.User data)
        {
            //Models.DatabaseContext db = new Models.DatabaseContext();
            //Models.Base.User user = db.User.Where(x=>x.username==data.username && x.password==data.password).FirstOrDefault<Models.Base.User>();
            //if(user != null) 
            if (true)
            {
                Session.RemoveAll();
                
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    "Tester",
                    DateTime.Now,
                    DateTime.Now.AddMinutes(10),
                    false,
                    "Test...",
                    FormsAuthentication.FormsCookiePath);

                string encodeTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encodeTicket));
            }
            return RedirectToAction("Index", "Home");
        }*/
        private AesCookies AesEnc = new AesCookies();//Bill
        //private OutputMessage OMessage = new OutputMessage(); //Bill
        TWSqlDBContext db_before = new TWSqlDBContext();
        string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
        string ECDomain2 = System.Configuration.ConfigurationManager.AppSettings["ECSSLDomain"];
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);


        /*
        [HttpPost]
        public ActionResult CheckDup(string Email)
        {
            var member = db.Members.Where(p => p.Email == Email).FirstOrDefault;

            if (member != null)
                return Json(false);
            else
                return Json(true);
        }
        */
        /*
        private void SendAuthCodeToMember(Member member)
        {
            string mailBody = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/MemberRegisterEMailTemplate.htm"));

            mailBody = mailBody.Replace("{{Name}}", member.Name);
            mailBody = mailBody.Replace("{{RegisterOn}}", member.RegisterOn.ToString("F"));
            var auth_url = new UriBuilder(Request.Url)
            {
                Path = Url.Action("ValidateRegister", new { id = member.AuthCode }),
                Query = ""
            };
            mailBody = mailBody.Replace("{{AUTH_URL}}", auth_url.ToString());
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("YourGmailAccount", "password");
                SmtpServer.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.Form = new MailAddress("YourGmailAccount@gmail.com");
                mail.To.Add(member.Email);
                mail.Subject = "[我的電子商務網站]會員註冊確認信";
                mail.Body = mailBody;
                mail.IsBodyHtml = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
                // 發生郵件寄送失敗，需記錄進資料庫備查，以免有會員無法登入
            }
            //throw new NotImplementedException();
        }
        */

        // 顯示會員登入頁面
        //[AllowAnonymous]
        //[RequireSSL]
        //  public class test()
        // {
        // string decode="";
        //decode = AesEnc.AESdecrypt(InnerUid);
        //}
        public ActionResult test(string InnerUid)
        {

            InnerUid = "pB0tLe5whORv38IXam6Ijg==";
            string decode = AesEnc.AESdecrypt(InnerUid);
            return View();

        }
        public ActionResult Login(string returnUrl, string account_confirm = "")
        {

            if (Request.Cookies["em"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //ClearCookie(Request, Response); // 清除cookies
            //FormsAuthentication.SignOut();
            if (account_confirm != "")
            {
                ViewBag.account_confirm = account_confirm;
            }
            else
            {
                ViewBag.account_confirm = "";
            }
            ModelState.Clear();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // 執行會員登入
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public ActionResult Login(Account model, string googlecaptchaM, string returnUrl, string LoginType, string Uid, string account_confirm = "")
        {
            if (Request.Cookies["em"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            //ClearCookie(Request, Response); // 清除cookies
            if (account_confirm != "")
            {
                ViewBag.account_confirm = account_confirm;
            }
            else
            {
                ViewBag.account_confirm = "";
            }
            ViewBag.pwd_confirm = "";
            ViewBag.ValicodeCheck = "";
            Account oSearch = null;
            string oPwd = "";
            bool bFbLogin = false;

            AccountVerify oAccountService = new AccountVerify();
            bool bIsLogin = false;
            //logger.Info("Step0");
            //新蛋登入
            string loginEmail = String.Empty;
            if (model.Email != null && model.Email.Length > 0)
            {
                loginEmail = model.Email.Trim();
            }
            //logger.Info("googlecaptchaM[" + googlecaptchaM + "]");
            if (loginEmail != null && model.PWD != null)
            {
                //logger.Info("正常登入");
                //logger.Info("loginEmail[" + loginEmail + "] model.PWD.count[" + model.PWD.Count() + "]");
                //檢查帳號是否存在
                //oSearch = (from x in this.db_before.Account where x.Email == loginEmail && x.PWD == oPwd select x).FirstOrDefault();
                oSearch = oAccountService.VerifyAccountLogin(loginEmail, model.PWD, true);
                if (oSearch == null)
                {
                    ViewBag.account_confirm = "帳號或密碼錯誤，請重新輸入";
                    return View();
                }
                oPwd = AesEnc.AESenprypt(model.PWD);
            }
            //facebook登入
            else if (loginEmail != null && LoginType.ToLower().Equals("facebook"))
            {
                //logger.Info("facebook登入");
                //logger.Info("loginEmail[" + loginEmail + "] LoginType.ToLower()[" + LoginType.ToLower() + "]");
                //oSearch = (from x in this.db.account where x.account_email == model.account_email && x.account_facebookuid == Uid select x).FirstOrDefault();
                //oSearch = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
                oSearch = oAccountService.GetAccountByEmail(loginEmail, false);
                //logger.Info("Facebook UID:" + Uid + " eMail:" + loginEmail);
                if (oSearch == null)
                {
                    //logger.Info("被判斷為尚未註冊");
                    //此facebook帳號尚未註冊為新蛋會員
                    return Json(new { type = "success", newegg = "false", first = "true", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl=" + returnUrl });
                }
                else
                {
                    //比對facebook uid
                    if (oSearch.FacebookUID == null)
                    {
                        //facebook uid不存在: 更新uid (下方會有登入時間的更動, 到時一起db.SaveChanges)
                        logger.Info("UID不存在 更新UID");
                        oSearch.FacebookUID = Uid;
                        bFbLogin = true;
                    }
                    else if (!oSearch.FacebookUID.Equals(Uid))
                    {
                        //facebook uid不符合
                        logger.Info("UID不符合");
                        return Json(new { type = "uid", newegg = "true", first = "false", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl" + returnUrl });
                    }
                    else if (oSearch.FacebookUID.Equals(Uid))
                        bFbLogin = true;
                }
            }
            //非經由正確管道登入
            else
            {
                logger.Info("非經由正確管道登入");
                return View();
            }
            //logger.Info("Step2");
            //var member_account = (from p in db.account where p.account_email == model.account_email select p.account_email).FirstOrDefault();
            //var member_pwd = (from p in db.account where p.account_email == model.account_email select p.account_pwd).FirstOrDefault();
            /*
            DateTime expiration = DateTime.Now.AddSeconds(30);
            if(RememberMe)
            {
                expiration = DateTime.Now.AddMinutes(2);
            }
            FormsAuthentication.Initialize();
            FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, email, DateTime.Now, expiration, RememberMe, FormsAuthentication.FormsCookiePath);
            HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
            ck.Path = FormsAuthentication.FormsCookiePath;
            response.Cookies.Add(ck);
            */

            if (!bFbLogin)
            {
                //logger.Info("Step3");
                if (googlecaptchaM == null || googlecaptchaM == "")
                {
                    ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                    return View();
                }
                //logger.Info("Step4");
                TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService httpRequest = new Service.CommonService.HttpWebRequestService();

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("secret", TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService.GOOGLE_SECRET_KEY);
                parameters.Add("response", googlecaptchaM);
                //logger.Info("Step4-1");
                var result = httpRequest.Get("https://www.google.com/recaptcha/api/siteverify", parameters);

                if (!httpRequest.CheckGooglereCaptChaMessage(result))
                {
                    ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                    //logger.Info("Step4-2");
                    return View();
                }
                if (loginEmail != null && model.PWD != null && Request.Cookies["ValidateCode"] != null)
                {
                    //logger.Info("Step4-2-1");
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    //logger.Info("CheckValidateCode[" + CheckValidateCode + "]");
                    if (model.ValidateCode == null || model.ValidateCode == "")
                    {
                        //logger.Info("Step4-3");
                        return View();
                    }
                    //logger.Info("Step4-3-1");
                    if (CheckValidateCode != model.ValidateCode)
                    {
                        ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                        //logger.Info("Step4-4");
                        return View();
                    }
                    //logger.Info("Step4-4-1");
                }
                else
                {
                    //logger.Info("Step4-5");
                    ViewBag.ValicodeCheck = "系統錯誤，請重新登入";
                    return View();
                }
            }//end if(!bFbLogin)
            //logger.Info("Step5");
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

            //---------------Rememberme-----------------//
            //Example Code from Stack Overflow
            //int timeout = rememberMe ? 525600 : 30; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(userName, rememberMe, timeout);

            int timeout = (model.RememberMe.Value == 1) ? 129600 : 0; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(model.account_email, (model.account_rememberme.Value == 1) ? true : false, timeout);
            //string encrypted = FormsAuthentication.Encrypt(ticket);
            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            //cookie.Expires = System.DateTime.Now.AddMinutes(timeout);// Not my line
            //cookie.HttpOnly = true; // cookie not available in javascript.
            //Response.Cookies.Add(cookie);

            //var lockemail = (from p in db.account where p.account_email == model.account_email && p.account_pwd == model.account_pwd select p).FirstOrDefault();
            //lockemail.account_loginon = DateTime.Now;
            //lockemail.account_loginstatus = 1;

            oSearch.Loginon = DateTime.Now;
            oSearch.LoginStatus = 1;

            oSearch.RememberMe = model.RememberMe;
            oSearch.NewLinks = newlinks;
            //logger.Info("Step5-1");
            //db_before.SaveChanges();
            oAccountService.UpdateAccount(oSearch);
            //logger.Info("Step5-2");
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
            Response.Cookies["em"].Value = AesEnc.AESenprypt(oSearch.Email);
            Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            Response.Cookies["NewLinks"].Value = newlinks;
            Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["Accountid"].Domain = mainDomain;
            Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["LoginStatus"].Domain = mainDomain;
            Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            Response.Cookies["IE"].Domain = mainDomain;
            /*
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomainFlag").ToUpper().Equals("ON"))
            {
                Response.Cookies["em"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
                Response.Cookies["ex"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
            }
            */
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_loginstatus).ToString() + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginstatus) + "_" + Convert.ToString(oSearch.account_loginon)) + "e";
            if (timeout != 0)
            {
                Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(timeout);
            }
            //---------------Rememberme-----------------//
            //logger.Info("Step6");
            /*      */
            ICarts repository = new CartsRepository();
            //repository.SetTrackAll(oSearch.account_id, oSearch.account_loginon.ToString());//Set accid 
            repository.SetTrackAll(oSearch.ID, oSearch.CreateDate.ToString());//Set accid 
            repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    //logger.Info("Step6-1");
                    string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
                    //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart);//Trans it to model


                    bool checkadd = new bool();
                    checkadd = true;

                    foreach (var cookieCart in cookieCarts) // Add all cookie's item into DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    //logger.Info("Step6-2");
                }
                else
                {
                    //logger.Info("Step6-3");
                }
            }
            catch (Exception e)
            {
                //logger.Info("Step7");
            }
            //logger.Info("Step8");
            string newUrl = "";
            //Check returnURL is null or not
            if (string.IsNullOrEmpty(returnUrl))
            {
                newUrl = "/Home/Index";
            }
            else
            {
                returnUrl = returnUrl.Replace(@"""", ""); //replace it

                if (returnUrl != "" && returnUrl != "\"\"") //detect if url is ""
                {
                    //if (returnUrl.IndexOf("JoinUSIntroduction", StringComparison.OrdinalIgnoreCase) >= 0)
                    //{
                    //    returnUrl = "/Activity/JoinUSIntroduction";
                    //}

                    Response.Cookies["lastView"].Value = ""; //clear it
                    newUrl = returnUrl;
                    //return Redirect(newUrl);
                }
                else
                {
                    newUrl = "/Home/Index";
                }
            }
            //logger.Info("Step9");
            if (loginEmail != null && LoginType != null && LoginType.ToLower().Equals("facebook"))
            {
                //logger.Info("Step10");
                return Json(new { type = "success", newegg = "true", first = "false", returnUrl = newUrl, redirect = "/Account/Register_Facebook" });
            }
            else
            {
                //logger.Info("Step11");
                return Redirect(newUrl);
            }
            //logger.Info("Step12");
            /*  if (ModelState.IsValid && WebSecurity.Login(email, password, persistCookie: model.RememberMe))
              {
                  return RedirectToLocal(returnUrl);
              }*/
            //ViewBag.pwd_confirm = "密碼錯誤，請您輸入新蛋全球生活網會員帳號的密碼，或按忘記密碼取得新密碼";
            //ModelState.AddModelError("", "您輸入的帳號或密碼錯誤或不存在");
            return View();
        }

        /// <summary>
        /// 執行內部會員登入
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <param name="InnerUid"></param>
        /// <returns></returns>
        //[RequireSSL]
        public ActionResult fekl(Account model, string returnUrl, string InnerUid)
        {
            //return RedirectToAction("Login", "account");
            return RedirectToAction("InnerLogin", new { model = model, returnUrl = returnUrl, InnerUid = InnerUid });
        }

        //[RequireSSL]
        public ActionResult InnerLogin(Account model, string returnUrl, string InnerUid)
        {
            ClearCookie(Request, Response); // 清除cookies
            Account oSearch = null;
            //int timeout = 1440; // Timeout in minutes, 1440 = 1 day, 525600 = 365 days.
            string decode = "";
            string shortName = ""; // 短名
            string createDate = ""; // 帳號創建時間
            string checkCreateDate = "";
            string HostName = Dns.GetHostName();
            InnerUid = InnerUid.Replace(" ", "+");
            string IP = "";
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    // 從IP地址列表中篩選出IPv4類型的IP地址
                    // AddressFamily.InterNetwork表示此IP為IPv4,
                    // AddressFamily.InterNetworkV6表示此地址為IPv6類型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        IP = IpEntry.AddressList[i].ToString();
                    }
                }

                //if (Request.ServerVariables["HTTP_VIA"] == null)
                //{
                //    IP = Request.ServerVariables["REMOTE_ADDR"].ToString();
                //}
                //else
                //{
                //    IP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                //}

                if (IP.Split('.')[0] == "172" || IP.Split('.')[0] == "10")
                { }
                else
                {
                    // 執行排除非內部登入的動作
                    ViewBag.errMessage = "驗證失敗請重新登入";
                    return RedirectToAction("Index", "Home");
                }

                oSearch = (from x in this.db_before.Account where x.Email == model.Email select x).FirstOrDefault();
                if (oSearch == null)
                {
                    // 執行搜尋不到的動作
                    ViewBag.errMessage = "驗證失敗請重新登入";
                    return RedirectToAction("Index", "Home");
                }

                decode = AesEnc.AESdecrypt(InnerUid);
                shortName = decode.Split('_')[0]; // 短名
                createDate = decode.Split('_')[1]; // 帳號創建時間
                checkCreateDate = ((DateTime)oSearch.CreateDate).ToString("yyyy-MM-dd HH:mm:ss");
                if (createDate != checkCreateDate)
                {
                    // 執行驗證失敗的動作
                    ViewBag.errMessage = "驗證失敗請重新登入";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.errMessage = "驗證失敗請重新登入";
                return RedirectToAction("Index", "Home");
            }

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
            //var lockemail = (from p in db.account where p.account_email == model.account_email && p.account_pwd == model.account_pwd select p).FirstOrDefault();
            //lockemail.account_loginon = DateTime.Now;
            //lockemail.account_loginstatus = 1;
            oSearch.Loginon = DateTime.Now;
            oSearch.LoginStatus = 1;
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

            Response.Cookies["ServerUser"].Value = shortName;
            Response.Cookies["em"].Value = AesEnc.AESenprypt(model.Email);
            Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            Response.Cookies["NewLinks"].Value = newlinks;
            Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["Accountid"].Domain = mainDomain;
            Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["LoginStatus"].Domain = mainDomain;
            Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            Response.Cookies["IE"].Domain = mainDomain;
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_loginstatus).ToString() + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginstatus) + "_" + Convert.ToString(oSearch.account_loginon)) + "e";
            oSearch.NewLinks = newlinks;

            db_before.SaveChanges();

            //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(timeout);
            //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(timeout);
            //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(timeout);
            //Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(timeout);
            //Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(timeout);
            //Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(timeout);

            ICarts repository = new CartsRepository();
            //repository.SetTrackAll(oSearch.account_id, oSearch.account_loginon.ToString());//Set accid 
            repository.SetTrackAll(oSearch.ID, oSearch.CreateDate.ToString());//Set accid 
            repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
                    //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart);//Trans it to model


                    bool checkadd = new bool();
                    checkadd = true;

                    foreach (var cookieCart in cookieCarts) // Add all cookie's item into DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            string newUrl = "";

            // Check returnURL is null or not
            if (string.IsNullOrEmpty(returnUrl))
            {
                newUrl = "/Home/Index";
            }
            else
            {
                returnUrl = returnUrl.Replace(@"""", ""); //replace it
                if (returnUrl != "" && returnUrl != "\"\"") //detect if url is ""
                {
                    Response.Cookies["lastView"].Value = ""; //clear it
                    newUrl = returnUrl;
                    //return Redirect(newUrl);
                }
                else
                    newUrl = "/Home/Index";
            }
            return RedirectToAction("recentOrders", "MyNewegg");
            //return RedirectToAction("Index", "Home");
        }

        private List<CookieCart> findShippingCart(string shippingCart)
        {
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"66\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"62\",\"buyingNumber\":\"2\"}]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"76\",\"buyingNumber\":\"6\"}]}]";
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]}]";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<CookieCart> shippingCarts = new List<CookieCart>();
            string newshippingCart = shippingCart.Replace("null", "");
            try
            {
                shippingCarts = serializer.Deserialize<List<CookieCart>>(newshippingCart);
                return shippingCarts;
            }
            catch (Exception e)
            {
                shippingCarts.Add(new CookieCart { itemID = new List<int>() { 0 }, itemlistID = new List<int>() { 0 } });
                return shippingCarts;
            }

        }

        //[AllowAnonymous]
        private bool ValidateUser(string email, string pwd)
        {

            //var hash_pw = FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + password, "SHA1");
            var member = (from p in db_before.Account where p.Email == email && p.PWD == pwd select p.Email).FirstOrDefault();
            //throw new NotImplementedException();
            return (member != null);
        }
        /*
        // 顯示會員自動登入頁面
        public ActionResult AutoLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
        */
        // 自動執行會員登入
        // [HttpPost]
        //[RequireSSL]
        public ActionResult AutoLogin(Account model, string Message, string newmail, string pwd, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //FormsAuthentication.SignOut();
            ViewBag.Message = Message; // 成功或失敗訊息
            Account LockLoginStatus = new Account();
            // 修改信箱的自動登入路徑
            if (Message != null && newmail != null && pwd != null && Message != "" && newmail != "" && pwd != "")
            {
                Response.Cookies["em"].Value = AesEnc.AESenprypt(newmail);

                LockLoginStatus = (from p in db_before.Account where p.Email == newmail && p.PWD == pwd select p).FirstOrDefault();
            }
            else
            {

                // 一般註冊、忘記密碼的自動登入路徑
                model.Email = AesEnc.AESdecrypt(Request.Cookies["em"].Value);
                model.PWD = Request.Cookies["Password"].Value;
                LockLoginStatus = (from p in db_before.Account where p.Email == model.Email && p.PWD == model.PWD select p).FirstOrDefault();
            }

            if (LockLoginStatus != null)
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
                LockLoginStatus.NewLinks = newlinks;
                LockLoginStatus.Loginon = DateTime.Now;
                LockLoginStatus.LoginStatus = 1;
                db_before.SaveChanges();
                //FormsAuthentication.RedirectFromLoginPage(model.account_email, false); // 登入
                //User.Identity.IsAuthenticated = true;
            }
            else
            {
                //LockLoginStatus.account_loginstatus = 0;
                //db.SaveChanges();
                ViewBag.errMessage = "系統錯誤";
                return RedirectToAction("Index", "Home");
            }
            // 移除Password
            HttpCookie passwordcookie = new HttpCookie("Password");
            passwordcookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(passwordcookie);

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
            //FormsAuthentication.SetAuthCookie(newmail, false);
            Response.Cookies["em"].Value = Request.Cookies["em"].Value;
            Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.ID) + "_" + Convert.ToString(LockLoginStatus.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["Accountid"].Domain = mainDomain;
            Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.LoginStatus) + "_" + Convert.ToString(LockLoginStatus.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["LoginStatus"].Domain = mainDomain;
            Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.ID) + "_" + Convert.ToString(LockLoginStatus.LoginStatus) + "_" + Convert.ToString(LockLoginStatus.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            Response.Cookies["IE"].Domain = mainDomain;
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.account_id) + "_" + Convert.ToString(LockLoginStatus.account_loginon));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.account_loginstatus) + "_" + Convert.ToString(LockLoginStatus.account_loginon));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(LockLoginStatus.account_id) + "_" + Convert.ToString(LockLoginStatus.account_loginstatus) + "_" + Convert.ToString(LockLoginStatus.account_loginon)) + "e";

            //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
            //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);
            //Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(1440);
            //Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(1440);
            //Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(1440);
            /*      */
            ICarts repository = new CartsRepository();
            //repository.SetTrackAll(LockLoginStatus.account_id, LockLoginStatus.account_loginon.ToString());//Set accid 
            repository.SetTrackAll(LockLoginStatus.ID, LockLoginStatus.CreateDate.ToString());//Set accid 
            repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
                    //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart);//Trans it to model


                    bool checkadd = new bool();
                    checkadd = true;

                    foreach (var cookieCart in cookieCarts)  // Add all cookie's item into DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }
            /*      */

            return RedirectToAction("Index", "Home");
        }

        // 讓WordPress執行會員登入
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public JsonResult WLogin(Account model, string LoginType, string Uid)
        {
            Dictionary<string, string> returnCookies = new Dictionary<string, string>();
            if (Request.Cookies["em"] != null)
            {
                ClearCookie(Request, Response); // 清除cookies
            }


            Account oSearch = null;
            string oPwd = "";
            bool bFbLogin = false;

            AccountVerify oAccountService = new AccountVerify();
            bool bIsLogin = false;
            //logger.Info("Step0");
            //新蛋登入
            string loginEmail = String.Empty;
            if (model.Email != null && model.Email.Length > 0)
            {
                loginEmail = model.Email.Trim();
            }
            //logger.Info("googlecaptchaM[" + googlecaptchaM + "]");
            if (loginEmail != null && model.PWD != null)
            {
                //logger.Info("正常登入");
                //logger.Info("loginEmail[" + loginEmail + "] model.PWD.count[" + model.PWD.Count() + "]");
                //檢查帳號是否存在
                //oSearch = (from x in this.db_before.Account where x.Email == loginEmail && x.PWD == oPwd select x).FirstOrDefault();
                oSearch = oAccountService.VerifyAccountLogin(loginEmail, model.PWD, true);
                if (oSearch == null)
                {
                    ViewBag.account_confirm = "帳號或密碼錯誤，請重新輸入";
                    return Json(returnCookies);
                }
                oPwd = AesEnc.AESenprypt(model.PWD);
            }
            //facebook登入
            else if (loginEmail != null && LoginType.ToLower().Equals("facebook"))
            {
                //logger.Info("facebook登入");
                //logger.Info("loginEmail[" + loginEmail + "] LoginType.ToLower()[" + LoginType.ToLower() + "]");
                //oSearch = (from x in this.db.account where x.account_email == model.account_email && x.account_facebookuid == Uid select x).FirstOrDefault();
                //oSearch = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
                oSearch = oAccountService.GetAccountByEmail(loginEmail, false);
                //logger.Info("Facebook UID:" + Uid + " eMail:" + loginEmail);
                if (oSearch == null)
                {
                    //logger.Info("被判斷為尚未註冊");
                    //此facebook帳號尚未註冊為新蛋會員
                    return Json(returnCookies);
                }
                else
                {
                    //比對facebook uid
                    if (oSearch.FacebookUID == null)
                    {
                        //facebook uid不存在: 更新uid (下方會有登入時間的更動, 到時一起db.SaveChanges)
                        logger.Info("UID不存在 更新UID");
                        oSearch.FacebookUID = Uid;
                        bFbLogin = true;
                    }
                    else if (!oSearch.FacebookUID.Equals(Uid))
                    {
                        //facebook uid不符合
                        logger.Info("UID不符合");
                        return Json(returnCookies);
                    }
                    else if (oSearch.FacebookUID.Equals(Uid))
                        bFbLogin = true;
                }
            }
            //非經由正確管道登入
            else
            {
                logger.Info("非經由正確管道登入");
                return Json(returnCookies);
            }
            //logger.Info("Step2");
            //var member_account = (from p in db.account where p.account_email == model.account_email select p.account_email).FirstOrDefault();
            //var member_pwd = (from p in db.account where p.account_email == model.account_email select p.account_pwd).FirstOrDefault();
            /*
            DateTime expiration = DateTime.Now.AddSeconds(30);
            if(RememberMe)
            {
                expiration = DateTime.Now.AddMinutes(2);
            }
            FormsAuthentication.Initialize();
            FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, email, DateTime.Now, expiration, RememberMe, FormsAuthentication.FormsCookiePath);
            HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
            ck.Path = FormsAuthentication.FormsCookiePath;
            response.Cookies.Add(ck);
            */

            //logger.Info("Step5");
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

            //---------------Rememberme-----------------//
            //Example Code from Stack Overflow
            //int timeout = rememberMe ? 525600 : 30; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(userName, rememberMe, timeout);

            int timeout = (model.RememberMe.Value == 1) ? 129600 : 0; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(model.account_email, (model.account_rememberme.Value == 1) ? true : false, timeout);
            //string encrypted = FormsAuthentication.Encrypt(ticket);
            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            //cookie.Expires = System.DateTime.Now.AddMinutes(timeout);// Not my line
            //cookie.HttpOnly = true; // cookie not available in javascript.
            //Response.Cookies.Add(cookie);

            //var lockemail = (from p in db.account where p.account_email == model.account_email && p.account_pwd == model.account_pwd select p).FirstOrDefault();
            //lockemail.account_loginon = DateTime.Now;
            //lockemail.account_loginstatus = 1;

            oSearch.Loginon = DateTime.Now;
            oSearch.LoginStatus = 1;

            oSearch.RememberMe = model.RememberMe;
            oSearch.NewLinks = newlinks;
            //logger.Info("Step5-1");
            //db_before.SaveChanges();
            oAccountService.UpdateAccount(oSearch);
            var accountMember = db_before.Member.Where(x => x.AccID == oSearch.ID).FirstOrDefault();
            //logger.Info("Step5-2");
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
            //Response.Cookies["em"].Value = AesEnc.AESenprypt(oSearch.Email);
            returnCookies.Add("em", AesEnc.AESenprypt(oSearch.Email));
            //Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            returnCookies.Add("ex", System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            //Response.Cookies["NewLinks"].Value = newlinks;
            returnCookies.Add("NewLinks", newlinks);
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            //Response.Cookies["Accountid"].Domain = mainDomain;
            returnCookies.Add("Accountid", AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            //Response.Cookies["LoginStatus"].Domain = mainDomain;
            returnCookies.Add("LoginStatus", AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            //Response.Cookies["IE"].Domain = mainDomain;
            returnCookies.Add("IE", "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e");
            if (accountMember != null)
            {
                returnCookies.Add("LastName", accountMember.Lastname ?? "");
                returnCookies.Add("FirstName", accountMember.Firstname ?? "");
            }
            /*
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomainFlag").ToUpper().Equals("ON"))
            {
                Response.Cookies["em"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
                Response.Cookies["ex"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
            }
            */
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_loginstatus).ToString() + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginstatus) + "_" + Convert.ToString(oSearch.account_loginon)) + "e";

            /*
            if (timeout != 0)
            {
                Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(timeout);
            }
            */

            //---------------Rememberme-----------------//
            //logger.Info("Step6");
            /*      
            ICarts repository = new CartsRepository();
            //repository.SetTrackAll(oSearch.account_id, oSearch.account_loginon.ToString());//Set accid 
            repository.SetTrackAll(oSearch.ID, oSearch.CreateDate.ToString());//Set accid 
            repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    //logger.Info("Step6-1");
                    string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
                    //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart);//Trans it to model


                    bool checkadd = new bool();
                    checkadd = true;

                    foreach (var cookieCart in cookieCarts) // Add all cookie's item into DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    //logger.Info("Step6-2");
                }
                else
                {
                    //logger.Info("Step6-3");
                }
            }
            catch (Exception e)
            {
                //logger.Info("Step7");
            }
            */
            if (loginEmail != null && LoginType != null && LoginType.ToLower().Equals("facebook"))
            {
                //logger.Info("Step10");
                return Json(returnCookies);
            }
            else
            {
                //logger.Info("Step11");
                return Json(returnCookies);
            }
        }

        // 讓WordPress執行會員登出
        //[AllowAnonymous]
        public bool WLogout()
        {
            ClearCookie(Request, Response); // 清除cookies
            return true;
        }

        // 顯示忘記密碼頁面
        //[AllowAnonymous]
        //[RequireSSL]
        public ActionResult ReSetPassword(string returnUrl)
        {
            //ClearCookie(Request, Response);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // 忘記密碼
        //[RequireSSL]
        [HttpPost]
        public ActionResult ReSetPassword(Account model, string returnUrl)
        {
            ClearCookie(Request, Response);
            ViewBag.account_confirm = "";
            // 驗證帳號是否存在
            Account member = (from p in db_before.Account where p.Email == model.Email select p).FirstOrDefault();
            if (member == null)
            {
                ViewBag.account_confirm = "無此會員帳號，請您輸入" + WebSiteData.SiteName + "的會員帳號或註冊一個新的會員帳號";
                return View();
            }
            //FormsAuthentication.SetAuthCookie(model.Email, false);
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

            //Response.Cookies["em"].Value = AesEnc.AESenprypt(model.account_email);
            //Response.Cookies["NewLinks"].Value = newlinks;

            var LockEmail = (from p in db_before.Account where p.Email == model.Email select p).FirstOrDefault();
            LockEmail.NewLinks = newlinks;

            db_before.SaveChanges();

            // 將URL與信件做連結
            //ViewBag.url = "https://" + ECDomain2 + "/account/NewPassword?NewLinks=" + model.NewLinks + "&Email=" + model.Email; // 重設新密碼的路徑

            ViewBag.Success = "";
            string EmailCheck = model.Email;

            if (Request.Cookies[EmailCheck + "p"] == null)
            {
                if (Mail_Message(LockEmail, "Forgot_Password"))
                {
                    Response.Cookies[EmailCheck].Value = HttpUtility.UrlEncode(System.DateTime.Now.AddMinutes(3).ToString());
                    Response.Cookies[EmailCheck].Expires = System.DateTime.Now.AddMinutes(3);
                    Response.Cookies[EmailCheck + "p"].Value = HttpUtility.UrlEncode(System.DateTime.Now.AddMinutes(3).ToString());
                    Response.Cookies[EmailCheck + "p"].Expires = System.DateTime.Now.AddMinutes(3);
                    ViewBag.Success = "更新密碼鏈結已發送至您會員設定信箱中";
                }
                else
                {
                    ViewBag.Success = "更新密碼鏈結發送失敗";
                }
            }
            else
            {
                var MailDate = Convert.ToDateTime(HttpUtility.UrlDecode(Request.Cookies[EmailCheck + "p"].Value));
                var NowDate = DateTime.Now;
                var timeDiff = MailDate.Subtract(NowDate).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
                if (timeDiff.Minutes > 0)
                {
                    ViewBag.Success = "重複發信，請在" + timeDiff.Minutes + "分鐘" + timeDiff.Seconds + "秒後再嘗試";
                }
                else
                {
                    ViewBag.Success = "重複發信，請在" + timeDiff.Seconds + "秒後再嘗試";
                }
            }

            return View();

        }

        //[AllowAnonymous]
        private bool ValidateUser_ReSetPassword(string email)
        {
            var member = (from p in db_before.Account where p.Email == email select p).FirstOrDefault();
            return (member != null);
        }

        /// <summary>
        /// RedirectToLocal???
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        //[RequireSSL]
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // 執行會員登出
        public ActionResult Logout()
        {
            // 清除表單驗證的 cookies
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
                var LocalAccountid = (from p in db_before.Account where p.ID == checkaccount select p).FirstOrDefault();
                if (LocalAccountid != null)
                {
                    LocalAccountid.RememberMe = 0;
                    //LocalAccountid.account_loginstatus = 0;
                    ////LocalAccountid.account_newlinks = null;
                    db_before.SaveChanges();
                }
                //-----------------------------------------//
                HttpCookie accountidcookie = new HttpCookie("Accountid");
                accountidcookie.Domain = mainDomain;
                accountidcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(accountidcookie);

                HttpCookie accountidcookieNoD = new HttpCookie("Accountid");
                accountidcookieNoD.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(accountidcookieNoD);
            }
            if (Request.Cookies["LoginStatus"] != null)
            {
                HttpCookie loginstatus = new HttpCookie("LoginStatus");
                loginstatus.Domain = mainDomain;
                loginstatus.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(loginstatus);

                HttpCookie loginstatusNoD = new HttpCookie("LoginStatus");
                loginstatusNoD.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(loginstatusNoD);
            }
            if (Request.Cookies["IE"] != null)
            {
                HttpCookie IE = new HttpCookie("IE");
                IE.Domain = mainDomain;
                IE.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(IE);

                HttpCookie IENoD = new HttpCookie("IE");
                IENoD.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(IENoD);
            }
            if (Request.Cookies["cartNumberDetail"] != null)
            {
                HttpCookie cartNumberDetailcookie = new HttpCookie("cartNumberDetail");
                cartNumberDetailcookie.Domain = mainDomain;
                cartNumberDetailcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cartNumberDetailcookie);

                HttpCookie cartNumberDetailcookieNoD = new HttpCookie("cartNumberDetail");
                cartNumberDetailcookieNoD.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cartNumberDetailcookieNoD);
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
            if (Request.Cookies["ValidateCode"] != null)
            {
                HttpCookie validatecodecookie = new HttpCookie("ValidateCode");
                validatecodecookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(validatecodecookie);
            }
            if (Request.Cookies["Password"] != null)
            {
                HttpCookie passwordcookie = new HttpCookie("Password");
                passwordcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(passwordcookie);
            }
            if (Request.Cookies["ServerUser"] != null)
            {
                HttpCookie serverusercookie = new HttpCookie("ServerUser");
                serverusercookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(serverusercookie);
            }
            // 清除所有曾經寫入過的 Session 資料
            //Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // 執行會員登出
        public void ClearCookie(HttpRequestBase Request, HttpResponseBase Response)
        {
            // 清除表單驗證的 cookies
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
            if (Request.Cookies["Password"] != null)
            {
                HttpCookie passwordcookie = new HttpCookie("Password");
                passwordcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(passwordcookie);
            }
            if (Request.Cookies["ServerUser"] != null)
            {
                HttpCookie serverusercookie = new HttpCookie("ServerUser");
                serverusercookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(serverusercookie);
            }
            // 清除所有曾經寫入過的 Session 資料
            //Session.Clear();
        }

        // Get
        //[Authorize]  // 必須登入才能進入
        //[AllowAnonymous]
        //[RequireSSL]
        public ActionResult NewPassword(Account model, string Date)
        {
            ViewBag.datastatus = "";
            Account LoginStart = null;
            try
            {
                ClearCookie(Request, Response); // 清除cookies
                // TempMail
                Response.Cookies["tm"].Value = AesEnc.AESenprypt(model.Email);
                Response.Cookies["NewLinks"].Value = model.NewLinks;
                ViewBag.Email = model.Email;
                //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
                //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(1440);
                //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);
                //string loginstart = Request.Cookies["em"].Value;
                //string newlinks = Request.Cookies["NewLinks"].Value;
                string loginstart = model.Email;
                string newlinks = model.NewLinks;
                LoginStart = (from p in db_before.Account where p.Email == loginstart && p.NewLinks == newlinks select p).FirstOrDefault();

                LoginStart.LoginStatus = 1;
                db_before.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Logout", "Account");
                //ViewBag.datastatus = "資料儲存錯誤";
            }

            return View(LoginStart);
        }

        // HttpPost
        //[RequireSSL]
        [HttpPost]
        //[Authorize]   // 必須登入才能進入     
        public ActionResult NewPassword(Account model)
        {
            ViewBag.datastatus = "";
            Account checkdata = null;
            if (Request.Cookies["tm"] != null)
            {
                string checkemail = AesEnc.AESdecrypt(Request.Cookies["tm"].Value);
                AccountVerify accountVerify = null;
                checkdata = (from p in db_before.Account where p.Email == checkemail && p.NewLinks == model.NewLinks select p).FirstOrDefault();

                // 移除TempMail
                HttpCookie tpcookie = new HttpCookie("tm");
                tpcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(tpcookie);
                accountVerify = new AccountVerify();
                if (accountVerify.UpdateAccountPassword(checkdata.Email, checkdata.PWD, model.PWD, false))
                {
                    Response.Cookies["Password"].Value = AesEnc.AESenprypt(model.PWD);
                    Response.Cookies["em"].Value = AesEnc.AESenprypt(model.Email);
                    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));

                    return RedirectToAction("AutoLogin", "Account");
                }

                ViewBag.datastatus = "資料儲存錯誤";
            }

            return View(checkdata);
        }

        //[AllowAnonymous]
        //[RequireSSL]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(6); //設定驗證碼出現多少個字數
            Response.Cookies["ValidateCode"].Value = code;
            ViewBag.ValidateCode = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        // 註冊成功通知信(註冊成功通知信)
        public bool Mail_RegisterMessage(Account account)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                string Messageresult = "";
                ViewBag.NewLinkTitle = NewLinkTitle;
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_Register");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }

                string Recipient = account.Email;
                bool Success = send_email(Messageresult, "親愛的" + WebSiteData.SiteName + "會員您好: +" + WebSiteData.SiteName + "通知-會員帳號註冊成功通知信", Recipient);

                LogtoFileWrite(path, "RegisterMessage", Messageresult);
                return Success;
            }
            catch { return false; }
        }

        // 註冊成功通知信、密碼修改通知信(忘記密碼)
        public bool Mail_Message(Account account, string MailType)
        {
            string path = Server.MapPath("~/Log/Mail/");
            try
            {
                string Messageresult = "";
                ViewBag.NewLinkTitle = NewLinkTitle;
                string Recipient = account.Email;
                bool Success = false;
                switch (MailType)
                {
                    case "RegisterMessage":
                        using (StringWriter sw = new StringWriter())
                        {
                            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_Register");

                            ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                            viewResult.View.Render(viewContext, sw);
                            Messageresult = sw.GetStringBuilder().ToString();
                        }
                        Success = send_email(Messageresult, "親愛的" + WebSiteData.SiteName + "會員您好: " + WebSiteData.SiteName + "通知-會員帳號註冊成功通知信", Recipient);
                        LogtoFileWrite(path, "RegisterMessage", Messageresult);
                        break;
                    case "Forgot_Password":
                        ViewBag.url = "https://" + ECDomain2 + "/Account/NewPassword?NewLinks=" + account.NewLinks + "&Email=" + account.Email; // 重設新密碼的路徑
                        using (StringWriter sw = new StringWriter())
                        {
                            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_reSetPwd");

                            ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                            viewResult.View.Render(viewContext, sw);
                            Messageresult = sw.GetStringBuilder().ToString();
                        }
                        Success = send_email(Messageresult, "NewEgg" + WebSiteData.SiteName + "通知-請立刻按[確認]－設定您Newegg帳號的新密碼！", Recipient);
                        LogtoFileWrite(path, "Forgot_Password", Messageresult);
                        break;
                }
                return Success;
            }
            catch { return false; }
        }

        public void LogtoFileWrite(string path, string MessageType, string writeStringendtoFile)
        {

            string filename = path + string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd_HH-mm}_" + MessageType + ".txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);

        }

        public bool send_email(string MailMessage, string mysubject, string address)
        {
            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收價者
                msg.To.Add(address);
                // msg.CC.Add("c@msn.com"); // 副本
                // msg.Bcc.Add("d@msn.com"); // 密件副本
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

        /*
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["myCookie"] != null)
                {
                    HttpCookie cookie = Request.Cookies.Get("myCookie");
                    txtUserName.Text = cookie.Values["username"];
                    txtPassword.Text = cookie.Values["password"];
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                objImpl = new WebImplIPhone();
                bool IsAvailable = false;
                HttpCookie myCookie = new HttpCookie("myCookie");
                bool IsRemember = chkRememberMe.Checked;

                IsAvailable = objImpl.CheckUserLogin(txtUserName.Text, txtPassword.Text);
                if (IsAvailable)
                {
                    DataTable dtUserName = objImpl.ReadUserIdbyUserName(txtUserName.Text);
                    if (dtUserName != null)
                    {
                        if (dtUserName.Rows.Count == 1)
                        {
                            Session["UserId"] = dtUserName.Rows[0].ItemArray[0].ToString();
                        }
                    }
                    if (IsRemember)
                    {
                        myCookie.Values.Add("username", txtUserName.Text);
                        myCookie.Values.Add("password", txtPassword.Text);
                        myCookie.Expires = DateTime.Now.AddDays(15);
                    }
                    else
                    {
                        myCookie.Values.Add("username", string.Empty);
                        myCookie.Values.Add("password", string.Empty);
                        myCookie.Expires = DateTime.Now.AddMinutes(5);
                    }
                    Response.Cookies.Add(myCookie);
                    //FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, chkRememberMe.Checked);
                    Response.Redirect("ApplicationList.aspx");
                }
                else
                {
                    lblError.Text = "Invalid UserName or Password or else your Username blocked";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Home.aspx");
            }
        }
        */

        //[HttpPost]
        //public ActionResult GuestLogin()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public ActionResult GuestLogin(Account accData)
        //{
        //    Account myAccount = new Account();
        //    string password = String.Empty;
        //    if (accData.Email == null || accData.PWD == null)
        //    {
        //        return Json("請輸入Email與Password!");
        //    }
        //    else 
        //    {
        //        password = AesEnc.AESenprypt(accData.PWD);
        //    }
        //    if (accData != null)
        //    {
        //        myAccount.Email = accData.Email;
        //        myAccount.PWD = password;
        //        myAccount.PWDtxt = password;
        //        myAccount.AgreePaper = accData.AgreePaper;
        //        myAccount.CreateDate = DateTime.Now;
        //        db_before.Account.Add(myAccount);
        //        db_before.SaveChanges();
        //    }
        //    return View();
        //}

        // 顯示會員登入頁面
        //[AllowAnonymous]
        //[RequireSSL]
        public ActionResult GuestLogin(string returnUrl, string account_confirm = "")
        {
            if (Request.Cookies["em"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //ClearCookie(Request, Response); // 清除cookies
            //FormsAuthentication.SignOut();
            if (account_confirm != "")
            {
                ViewBag.account_confirm = account_confirm;
            }
            else
            {
                ViewBag.account_confirm = "";
            }
            ModelState.Clear();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // 執行會員登入
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public ActionResult GuestLogin(Account model, string googlecaptchaM, string returnUrl, string LoginType, string Uid, string account_confirm = "")
        {
            if (Request.Cookies["em"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //ClearCookie(Request, Response); // 清除cookies
            if (account_confirm != "")
            {
                ViewBag.account_confirm = account_confirm;
            }
            else
            {
                ViewBag.account_confirm = "";
            }

            TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService httpRequest = new TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("secret", TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService.GOOGLE_SECRET_KEY);
            parameters.Add("response", googlecaptchaM);

            var result = httpRequest.Get("https://www.google.com/recaptcha/api/siteverify", parameters);

            if (!httpRequest.CheckGooglereCaptChaMessage(result))
            {
                ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                return View();
            }
            ViewBag.pwd_confirm = "";
            ViewBag.ValicodeCheck = "";
            Account oSearch = null;
            Account oAccount = null;
            string oPwd = "";
            bool bFbLogin = false;
            //新蛋登入
            string loginEmail = String.Empty;
            if (model.Email != null && model.Email.Length > 0)
            {
                loginEmail = model.Email.Trim();
            }
            if (loginEmail != null && model.PWD != null)
            {
                oPwd = AesEnc.AESenprypt(model.PWD);
                oAccount = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
                if (oAccount == null)
                {
                    ViewBag.account_confirm = "無此會員帳號，請您輸入" + WebSiteData.SiteName + "的會員帳號或註冊一個新的會員帳號";
                    return View();
                }
                oSearch = (from x in this.db_before.Account where x.Email == loginEmail && x.PWD == oPwd select x).FirstOrDefault();
                if (oSearch == null)
                {
                    ViewBag.account_confirm = "帳號或密碼錯誤，請重新輸入";
                    return View();
                }
            }
            else if (loginEmail != null && LoginType.ToLower().Equals("facebook"))
            {
                //facebook登入
                //oSearch = (from x in this.db.account where x.account_email == model.account_email && x.account_facebookuid == Uid select x).FirstOrDefault();
                oSearch = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
                logger.Info("Facebook UID:" + Uid + " eMail:" + loginEmail);
                if (oSearch == null)
                {
                    logger.Info("被判斷為尚未註冊");
                    //此facebook帳號尚未註冊為新蛋會員
                    return Json(new { type = "success", newegg = "false", first = "true", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl=" + returnUrl });
                }
                else
                {
                    //比對facebook uid
                    if (oSearch.FacebookUID == null)
                    {
                        //facebook uid不存在: 更新uid (下方會有登入時間的更動, 到時一起db.SaveChanges)
                        logger.Info("UID不存在 更新UID");
                        oSearch.FacebookUID = Uid;
                        bFbLogin = true;
                    }
                    else if (!oSearch.FacebookUID.Equals(Uid))
                    {
                        //facebook uid不符合
                        logger.Info("UID不符合");
                        return Json(new { type = "uid", newegg = "true", first = "false", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl" + returnUrl });
                    }
                    else if (oSearch.FacebookUID.Equals(Uid))
                    {
                        bFbLogin = true;
                    }
                }
            }
            else
            {
                //非經由正確管道登入
                return View();
            }

            //var member_account = (from p in db.account where p.account_email == model.account_email select p.account_email).FirstOrDefault();
            //var member_pwd = (from p in db.account where p.account_email == model.account_email select p.account_pwd).FirstOrDefault();
            /*
            DateTime expiration = DateTime.Now.AddSeconds(30);
            if(RememberMe)
            {
                expiration = DateTime.Now.AddMinutes(2);
            }
            FormsAuthentication.Initialize();
            FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, email, DateTime.Now, expiration, RememberMe, FormsAuthentication.FormsCookiePath);
            HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
            ck.Path = FormsAuthentication.FormsCookiePath;
            response.Cookies.Add(ck);
            */

            if (!bFbLogin)
            {
                if (loginEmail != null && model.PWD != null && Request.Cookies["ValidateCode"] != null)
                {
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    if (model.ValidateCode == null || model.ValidateCode == "")
                    { return View(); }
                    if (CheckValidateCode != model.ValidateCode)
                    {
                        ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ValicodeCheck = "系統錯誤，請重新登入";
                    return View();
                }
            }//end if(!bFbLogin)

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

            //---------------Rememberme-----------------//
            //Example Code from Stack Overflow
            //int timeout = rememberMe ? 525600 : 30; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(userName, rememberMe, timeout);

            int timeout = (model.RememberMe.Value == 1) ? 129600 : 0; // Timeout in minutes, 525600 = 365 days.
            //var ticket = new FormsAuthenticationTicket(model.account_email, (model.account_rememberme.Value == 1) ? true : false, timeout);
            //string encrypted = FormsAuthentication.Encrypt(ticket);
            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            //cookie.Expires = System.DateTime.Now.AddMinutes(timeout);// Not my line
            //cookie.HttpOnly = true; // cookie not available in javascript.
            //Response.Cookies.Add(cookie);

            //var lockemail = (from p in db.account where p.account_email == model.account_email && p.account_pwd == model.account_pwd select p).FirstOrDefault();
            //lockemail.account_loginon = DateTime.Now;
            //lockemail.account_loginstatus = 1;

            oSearch.Loginon = DateTime.Now;
            oSearch.LoginStatus = 1;

            oSearch.RememberMe = model.RememberMe;
            oSearch.NewLinks = newlinks;

            db_before.SaveChanges();
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
            Response.Cookies["em"].Value = AesEnc.AESenprypt(oSearch.Email);
            Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            Response.Cookies["NewLinks"].Value = newlinks;
            Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["Accountid"].Domain = mainDomain;
            Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["LoginStatus"].Domain = mainDomain;
            Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            Response.Cookies["IE"].Domain = mainDomain;
            /*
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomainFlag").ToUpper().Equals("ON"))
            {
                Response.Cookies["em"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
                Response.Cookies["ex"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
            }
            */
            //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_loginstatus).ToString() + "_" + Convert.ToString(oSearch.account_loginon));
            //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginstatus) + "_" + Convert.ToString(oSearch.account_loginon)) + "e";
            if (timeout != 0)
            {
                Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(timeout);
                Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(timeout);
            }
            //---------------Rememberme-----------------//

            /*      */
            ICarts repository = new CartsRepository();
            //repository.SetTrackAll(oSearch.account_id, oSearch.account_loginon.ToString());//Set accid 
            repository.SetTrackAll(oSearch.ID, oSearch.CreateDate.ToString());//Set accid 
            repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
                    //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart);//Trans it to model


                    bool checkadd = new bool();
                    checkadd = true;

                    foreach (var cookieCart in cookieCarts) // Add all cookie's item into DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            string newUrl = "";

            if (string.IsNullOrEmpty(returnUrl)) //Check returnURL is null or not
            {
                newUrl = "/Home/Index";
            }
            else
            {
                returnUrl = returnUrl.Replace(@"""", ""); //replace it
                if (returnUrl != "" && returnUrl != "\"\"") //detect if url is ""
                {
                    Response.Cookies["lastView"].Value = ""; //clear it
                    newUrl = returnUrl;
                    //return Redirect(newUrl);
                }
                else
                {
                    newUrl = "/Home/Index";
                }
            }

            if (loginEmail != null && LoginType != null && LoginType.ToLower().Equals("facebook"))
            {
                return Json(new { type = "success", newegg = "true", first = "false", returnUrl = newUrl, redirect = "/Account/Register_Facebook" });
            }
            else
            {
                return Redirect(newUrl);
            }


            /*  if (ModelState.IsValid && WebSecurity.Login(email, password, persistCookie: model.RememberMe))
              {
                  return RedirectToLocal(returnUrl);
              }*/
            //ViewBag.pwd_confirm = "密碼錯誤，請您輸入新蛋全球生活網會員帳號的密碼，或按忘記密碼取得新密碼";
            //ModelState.AddModelError("", "您輸入的帳號或密碼錯誤或不存在");
            return View();
        }

        // 會員註冊頁面
        //[AllowAnonymous]
        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult GuestRegister()
        {
            ClearCookie(Request, Response);
            return View();
        }

        // 寫入會員資料
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public ActionResult GuestRegister(Account accModel)
        {
            ClearCookie(Request, Response);
            if (accModel.AgreePaper != 1)
            {
                //ViewBag.account_confirm = "請閱讀及勾選同意會員條款";
                return View();
            }

            ViewBag.account_confirm = "";
            ViewBag.ValicodeCheck = "";
            TWSqlDBContext db = new TWSqlDBContext();
            string newEmail = String.Empty;
            if (accModel.Email != null && accModel.Email.Length > 0)
            {
                newEmail = accModel.Email.Trim();
            }
            if (newEmail != null && accModel.PWD != null && newEmail != "" && accModel.PWD != "")
            {
                // 檢查會員是否已經存在
                var chk_member = (from p in db.Account where p.Email == newEmail select p.Email).FirstOrDefault();
                if (chk_member != null)
                {
                    ViewBag.account_confirm = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請直接做會員登入或使用新e-mail(於非會員處)註冊成為會員";
                    return View();
                }
                else
                {
                    if (accModel.PWD.Length < 6 || accModel.PWD.Length > 16)
                    {
                        ViewBag.account_confirm = "密碼格式輸入錯誤";
                        return View();
                    }

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
                    ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;

                    Response.Cookies["em"].Value = AesEnc.AESenprypt(accModel.Email);
                    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
                    Response.Cookies["NewLinks"].Value = newlinks;
                    //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(1440);

                    Response.Cookies["Password"].Value = AesEnc.AESenprypt(accModel.PWD);
                    // 會員註冊時間
                    accModel.ValidateCode = "000000";
                    accModel.Registeron = DateTime.Now;
                    accModel.CreateDate = DateTime.Now;
                    accModel.Loginon = DateTime.Now;
                    accModel.PWD = AesEnc.AESenprypt(accModel.PWD);
                    accModel.PWDtxt = accModel.PWD;
                    accModel.NewLinks = newlinks;
                    accModel.LoginStatus = 1;
                    accModel.RememberMe = 0;
                    accModel.ActionCode = "C";
                    accModel.Istosap = 0;
                    accModel.GuestLogin = 1;
                    db.Account.Add(accModel);
                    db.SaveChanges();

                    /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    //DateTime objDateNow = DateTime.Now;
                    ////if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                    //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                    //{
                    //    TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository objCouponService = null;
                    //    int numEventId = 20;//鎖定活動使用20
                    //    objCouponService = new Redeem.Service.CouponService.CouponServiceRepository();

                    //    objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, accModel.ID.ToString());

                    //    objCouponService = null;
                    //}
                    /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */

                    Mail_Message(accModel, "RegisterMessage"); // 註冊成功通知信

                    //FormsAuthentication.SetAuthCookie(model.account_email, false);
                    Account searchAccount = new Account();
                    searchAccount = (from p in db.Account where p.Email == newEmail select p).FirstOrDefault();
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
                    Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["Accountid"].Domain = mainDomain;
                    Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["LoginStatus"].Domain = mainDomain;
                    Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
                    Response.Cookies["IE"].Domain = mainDomain;
                    //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_id) + "_" + Convert.ToString(SelectAccount.account_loginon));
                    //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_loginstatus) + "_" + Convert.ToString(SelectAccount.account_loginon));
                    //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_id) + "_" + Convert.ToString(SelectAccount.account_loginstatus) + "_" + Convert.ToString(SelectAccount.account_loginon)) + "e";

                    ICarts repository = new CartsRepository();
                    //repository.SetTrackAll(SelectAccount.account_id, SelectAccount.account_loginon.ToString()); //Set accid 
                    repository.SetTrackAll(searchAccount.ID, searchAccount.CreateDate.ToString()); //Set accid 
                    repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.
                    try
                    {
                        if (Request.Cookies["cart"] != null)
                        {
                            string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                            shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                            //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                            List<CookieCart> cookieCarts = new List<CookieCart>();
                            cookieCarts = findShippingCart(shippingCart); //Trans it to model

                            bool checkadd = new bool();
                            checkadd = true;
                            foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                            {
                                if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                                {
                                    //checkadd = true;
                                    //add cart success;
                                }
                                else
                                {
                                    checkadd = false;
                                    //add cart fail
                                }
                            }
                            if (checkadd)
                            {
                                Response.Cookies["cart"].Value = "";
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                            else
                            {
                                Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    //SendAuthCodeToMember(member);
                    return RedirectToAction("AutoLogin", "Account");
                }
            }

            return View();
        }

        //public string Test(string SONumber, string JMSONumber)
        //{
        //    JieMaiService JieMaiServiceAPI = new JieMaiService();
        //    string kk = JieMaiServiceAPI.TestJMSO(SONumber, JMSONumber);
        //    return "";
        //}

        /*
        //獲取IP地理位置 頁面
        public ActionResult Test()
        {
            string UserIP = Request.ServerVariables["Remote_Addr"].ToString();
            string obtaintxt = IpLocation(UserIP);
            ViewData["obtaintxt"] = obtaintxt;
            return View();
        }


        /// <summary>
        /// 新建IpLocation實例以獲得IP地理位置
        /// </summary>
        /// <param name="ipAddress"></param>
        public string IpLocation(string ipAddress)
        {
            string m_Location = "", m_IpAddress = "", m_Response = "";
            m_IpAddress = ipAddress.Trim();
            string[] ip = ipAddress.Split('.');

            ipAddress = ip[0] + "." + ip[1] + ".1.1";
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("GB2312");

            string url = "HTTP://www.ip138.com/ips.asp";
            string post = "ip=" + ipAddress + "&action=2";

            // 指定表單向伺服器提交的編碼類型,預設就上這個
            client.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
            // 向"HTTP://www.ip138.com/ips.asp";頁面post資料
            string response = client.UploadString(url, post);
            m_Response = response;

            // 用於解析結果的正則運算式
            string p = @"<li>參考資料二：(?<location>[^<>]+?)</li>";

            // 正則解析網頁的返回內容
            Match match = Regex.Match(response, p);

            // 取出匹配內容
            m_Location = match.Groups["location"].Value.Trim();
            return m_Location;
        }*/

        // 執行會員登入
        //[AllowAnonymous]
        //[RequireSSL]
        //[HttpPost]
        //public ActionResult Login(Account model, string returnUrl, string LoginType, string Uid, string test, string account_confirm = "")
        //{

        //    if (Request.Cookies["em"] != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    //ClearCookie(Request, Response); // 清除cookies
        //    if (account_confirm != "")
        //    {
        //        ViewBag.account_confirm = account_confirm;
        //    }
        //    else
        //    {
        //        ViewBag.account_confirm = "";
        //    }
        //    ViewBag.pwd_confirm = "";
        //    ViewBag.ValicodeCheck = "";
        //    Account oSearch = null;
        //    Account oAccount = null;
        //    string oPwd = "";
        //    bool bFbLogin = false;
        //    //新蛋登入
        //    string loginEmail = String.Empty;
        //    if (model.Email != null && model.Email.Length > 0)
        //    {
        //        loginEmail = model.Email.Trim();
        //    }
        //    if (loginEmail != null && model.PWD != null)
        //    {
        //        oPwd = AesEnc.AESenprypt(model.PWD);
        //        oAccount = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
        //        if (oAccount == null)
        //        {
        //            ViewBag.account_confirm = "無此會員帳號，請您輸入" + WebSiteData.SiteName + "的會員帳號或註冊一個新的會員帳號";
        //            return View();
        //        }
        //        oSearch = (from x in this.db_before.Account where x.Email == loginEmail && x.PWD == oPwd select x).FirstOrDefault();
        //        if (oSearch == null)
        //        {
        //            ViewBag.account_confirm = "帳號或密碼錯誤，請重新輸入";
        //            return View();
        //        }
        //    }
        //    //facebook登入
        //    else if (loginEmail != null && LoginType.ToLower().Equals("facebook"))
        //    {
        //        //oSearch = (from x in this.db.account where x.account_email == model.account_email && x.account_facebookuid == Uid select x).FirstOrDefault();
        //        oSearch = (from x in this.db_before.Account where x.Email == loginEmail select x).FirstOrDefault();
        //        logger.Info("Facebook UID:" + Uid + " eMail:" + loginEmail);
        //        if (oSearch == null)
        //        {
        //            logger.Info("被判斷為尚未註冊");
        //            //此facebook帳號尚未註冊為新蛋會員
        //            return Json(new { type = "success", newegg = "false", first = "true", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl=" + returnUrl });
        //        }
        //        else
        //        {
        //            //比對facebook uid
        //            if (oSearch.FacebookUID == null)
        //            {
        //                //facebook uid不存在: 更新uid (下方會有登入時間的更動, 到時一起db.SaveChanges)
        //                logger.Info("UID不存在 更新UID");
        //                oSearch.FacebookUID = Uid;
        //                bFbLogin = true;
        //            }
        //            else if (!oSearch.FacebookUID.Equals(Uid))
        //            {
        //                //facebook uid不符合
        //                logger.Info("UID不符合");
        //                return Json(new { type = "uid", newegg = "true", first = "false", returnUrl = returnUrl, redirect = "/Account/Register_Facebook?Uid=" + Uid + "&Email=" + loginEmail + "&returnUrl" + returnUrl });
        //            }
        //            else if (oSearch.FacebookUID.Equals(Uid))
        //                bFbLogin = true;
        //        }
        //    }
        //    //非經由正確管道登入
        //    else
        //    {
        //        return View();
        //    }

        //    //var member_account = (from p in db.account where p.account_email == model.account_email select p.account_email).FirstOrDefault();
        //    //var member_pwd = (from p in db.account where p.account_email == model.account_email select p.account_pwd).FirstOrDefault();
        //    /*
        //    DateTime expiration = DateTime.Now.AddSeconds(30);
        //    if(RememberMe)
        //    {
        //        expiration = DateTime.Now.AddMinutes(2);
        //    }
        //    FormsAuthentication.Initialize();
        //    FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, email, DateTime.Now, expiration, RememberMe, FormsAuthentication.FormsCookiePath);
        //    HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
        //    ck.Path = FormsAuthentication.FormsCookiePath;
        //    response.Cookies.Add(ck);
        //    */

        //    if (!bFbLogin)
        //    {
        //        if (loginEmail != null && model.PWD != null && Request.Cookies["ValidateCode"] != null)
        //        {
        //            string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
        //            if (model.ValidateCode == null || model.ValidateCode == "")
        //            { return View(); }
        //            if (CheckValidateCode != model.ValidateCode)
        //            {
        //                ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.ValicodeCheck = "系統錯誤，請重新登入";
        //            return View();
        //        }
        //    }//end if(!bFbLogin)

        //    int rand;
        //    char pd;
        //    string newlinks = String.Empty;
        //    // 生成重設密碼用的連結路徑  
        //    System.Random random = new Random();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        rand = random.Next();
        //        if (rand % 3 == 0)
        //        {
        //            pd = (char)('A' + (char)(rand % 26));
        //        }
        //        else if (rand % 3 == 1)
        //        {
        //            pd = (char)('a' + (char)(rand % 26));
        //        }
        //        else
        //        {
        //            pd = (char)('0' + (char)(rand % 10));
        //        }
        //        newlinks += pd.ToString();
        //    }

        //    //---------------Rememberme-----------------//
        //    //Example Code from Stack Overflow
        //    //int timeout = rememberMe ? 525600 : 30; // Timeout in minutes, 525600 = 365 days.
        //    //var ticket = new FormsAuthenticationTicket(userName, rememberMe, timeout);

        //    int timeout = (model.RememberMe.Value == 1) ? 129600 : 0; // Timeout in minutes, 525600 = 365 days.
        //    //var ticket = new FormsAuthenticationTicket(model.account_email, (model.account_rememberme.Value == 1) ? true : false, timeout);
        //    //string encrypted = FormsAuthentication.Encrypt(ticket);
        //    //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
        //    //cookie.Expires = System.DateTime.Now.AddMinutes(timeout);// Not my line
        //    //cookie.HttpOnly = true; // cookie not available in javascript.
        //    //Response.Cookies.Add(cookie);

        //    //var lockemail = (from p in db.account where p.account_email == model.account_email && p.account_pwd == model.account_pwd select p).FirstOrDefault();
        //    //lockemail.account_loginon = DateTime.Now;
        //    //lockemail.account_loginstatus = 1;

        //    oSearch.Loginon = DateTime.Now;
        //    oSearch.LoginStatus = 1;

        //    oSearch.RememberMe = model.RememberMe;
        //    oSearch.NewLinks = newlinks;

        //    db_before.SaveChanges();
        //    string mainDomain = "";
        //    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        //    if (environment == "DEV")
        //    {
        //        mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
        //    }
        //    else
        //    {
        //        mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
        //    }
        //    Response.Cookies["em"].Value = AesEnc.AESenprypt(oSearch.Email);
        //    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
        //    Response.Cookies["NewLinks"].Value = newlinks;
        //    Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
        //    Response.Cookies["Accountid"].Domain = mainDomain;
        //    Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.LoginStatus).ToString() + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
        //    Response.Cookies["LoginStatus"].Domain = mainDomain;
        //    Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.ID) + "_" + Convert.ToString(oSearch.LoginStatus) + "_" + Convert.ToString(oSearch.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
        //    Response.Cookies["IE"].Domain = mainDomain;
        //    /*
        //    if (System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomainFlag").ToUpper().Equals("ON"))
        //    {
        //        Response.Cookies["em"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
        //        Response.Cookies["ex"].Domain = System.Configuration.ConfigurationManager.AppSettings.Get("CookieDomain");
        //    }
        //    */
        //    //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginon));
        //    //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(oSearch.account_loginstatus).ToString() + "_" + Convert.ToString(oSearch.account_loginon));
        //    //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(oSearch.account_id) + "_" + Convert.ToString(oSearch.account_loginstatus) + "_" + Convert.ToString(oSearch.account_loginon)) + "e";
        //    if (timeout != 0)
        //    {
        //        Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //        Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //        Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //        Response.Cookies["Accountid"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //        Response.Cookies["IE"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //        Response.Cookies["LoginStatus"].Expires = System.DateTime.Now.AddMinutes(timeout);
        //    }
        //    //---------------Rememberme-----------------//

        //    /*      */
        //    ICarts repository = new CartsRepository();
        //    //repository.SetTrackAll(oSearch.account_id, oSearch.account_loginon.ToString());//Set accid 
        //    repository.SetTrackAll(oSearch.ID, oSearch.CreateDate.ToString());//Set accid 
        //    repository.CheckTrackCreateDate();//Del this accid's track item where didn't update in 30 days.
        //    try
        //    {
        //        if (Request.Cookies["cart"] != null)
        //        {
        //            string shippingCart = Request.Cookies["cart"].Value;//Get cookie's value
        //            shippingCart = HttpUtility.UrlDecode(shippingCart);//URIDecode
        //            //shippingCart = HttpUtility.HtmlEncode(shippingCart);

        //            List<CookieCart> cookieCarts = new List<CookieCart>();
        //            cookieCarts = findShippingCart(shippingCart);//Trans it to model


        //            bool checkadd = new bool();
        //            checkadd = true;

        //            foreach (var cookieCart in cookieCarts) // Add all cookie's item into DB
        //            {
        //                if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
        //                {
        //                    //checkadd = true;
        //                    //add cart success;
        //                }
        //                else
        //                {
        //                    checkadd = false;
        //                    //add cart fail
        //                }
        //            }
        //            if (checkadd)
        //            {
        //                Response.Cookies["cart"].Value = "";
        //                Response.Cookies["cart"].Domain = mainDomain;
        //                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
        //                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
        //            }
        //            else
        //            {
        //                Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
        //                Response.Cookies["cart"].Domain = mainDomain;
        //                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
        //                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
        //            }

        //        }
        //        else
        //        {

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //    }

        //    string newUrl = "";

        //    if (string.IsNullOrEmpty(returnUrl)) //Check returnURL is null or not
        //        newUrl = "/Home/Index";
        //    else
        //    {
        //        returnUrl = returnUrl.Replace(@"""", ""); //replace it
        //        if (returnUrl != "" && returnUrl != "\"\"") //detect if url is ""
        //        {
        //            Response.Cookies["lastView"].Value = ""; //clear it
        //            newUrl = returnUrl;
        //            //return Redirect(newUrl);
        //        }
        //        else
        //            newUrl = "/Home/Index";
        //    }

        //    if (loginEmail != null && LoginType != null && LoginType.ToLower().Equals("facebook"))
        //    {
        //        return Json(new { type = "success", newegg = "true", first = "false", returnUrl = newUrl, redirect = "/Account/Register_Facebook" });
        //    }
        //    else
        //    {
        //        return Redirect(newUrl);
        //    }


        //    /*  if (ModelState.IsValid && WebSecurity.Login(email, password, persistCookie: model.RememberMe))
        //      {
        //          return RedirectToLocal(returnUrl);
        //      }*/
        //    //ViewBag.pwd_confirm = "密碼錯誤，請您輸入新蛋全球生活網會員帳號的密碼，或按忘記密碼取得新密碼";
        //    //ModelState.AddModelError("", "您輸入的帳號或密碼錯誤或不存在");
        //    return View();
        //}

        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult Register()
        {

            // new 2015-03-23 Rex
            if (TempData["Message"] == "" || TempData["Message"] == null)
            {
                ViewBag.Message = "";
            }
            else
            {
                ViewBag.Message = TempData["Message"];
            }

            //活動關閉
            ViewBag.ActivityMessage = "";
            ViewBag.ActivityFlag = false;
            // 進行中的活動名稱
            string ongoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            // 驗證活動是否在有效活動時間內
            ActivityOnlineCheckService activityCheckService = null;
            ActivityData activityData = null;
            TWNewEgg.Models.ViewModels.Redeem.Event objEvent = null;
            switch (ongoingActivitiesName.ToLower())
            {
                case "coupon500":
                    #region coupon500
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                    //TWNewEgg.Redeem.Service.CouponService.EventReponsitory objEventService = new Redeem.Service.CouponService.EventReponsitory();
                    int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
                    // True: 活動進行中； False: 活動結束
                    bool ActivityIngOrNot = EvenStatus(numEventId);
                    DateTime nowDateTime = new DateTime();
                    nowDateTime = DateTime.Now;
                    //objEvent = objEventService.getEventById(numEventId);
                    objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.EventReponsitory", "getEventById", numEventId).results;
                    // 當下時間是否還在活動期間內
                    if (nowDateTime >= objEvent.datestart && nowDateTime <= objEvent.dateend)
                    {
                        TWNewEgg.ECWeb.Controllers.Api.CouponController apiController = new TWNewEgg.ECWeb.Controllers.Api.CouponController();
                        string str_CouponNumber = apiController.GetCouponNumber(numEventId).ToString();
                        // 判斷是否coupon券還有
                        if (str_CouponNumber != "0")
                        {
                            ViewBag.ActivityFlag = true;
                            ViewBag.ActivityMessage = "";
                        }
                        else
                        {
                            ViewBag.ActivityFlag = false;
                            ViewBag.ActivityMessage = "親愛的蛋友 您好，非常感謝大家的熱烈支持與愛護！新蛋迎新會首購折價券加碼又加碼，現已經全部發放完畢。這波來不及參與的朋友請放心，" + WebSiteData.SiteName + "將不定期推出優惠活動，請持續密切關注，謝謝。";
                        }
                    }
                    else
                    {
                        ViewBag.ActivityMessage = "";
                        ViewBag.ActivityFlag = false;
                    }

                    if (ActivityIngOrNot)
                    {
                        return RedirectToAction("RegisterActivity", "Account");
                    }
                    #endregion
                    break;
                case "omusic":
                    #region omusic
                    // 驗證Omusic活動是否在有效活動時間內
                    activityCheckService = new ActivityOnlineCheckService();
                    activityData = new ActivityData();
                    activityData = activityCheckService.GetActivityStatus("Omusic");
                    // 若Omusic活動在有效活動時間內則導向Omusic專屬頁面
                    if (activityData.IsEffective == true)
                    {
                        return RedirectToAction("RegisterActivity_Omusic", "Account");
                    }
                    #endregion
                    break;
                default:
                    break;
            }

            return View();

            //string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            //TWNewEgg.Redeem.Service.CouponService.EventReponsitory objEventService = new Redeem.Service.CouponService.EventReponsitory();
            //int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
            //string EvenIngOrNot = (System.Configuration.ConfigurationManager.AppSettings["MobileVerification"]).ToString();
            //// True: 活動進行中； False: 活動結束
            //bool ActivityIngOrNot = EvenStatus(numEventId);
            //// 檢查Omusic活動是否啟用
            //string isOmusicEnable = System.Configuration.ConfigurationManager.AppSettings["IsOmusicEnable"];
            //// 活動啟用則
            //if (isOmusicEnable.ToLower() == "on")
            //{
            //    // 驗證Omusic是否在有效活動時間內
            //    ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
            //    ActivityData activityData = activityCheckService.GetActivityStatus("Omusic");
            //    // 若Omusic活動在有效活動時間內則導向Omusic專屬頁面
            //    if (activityData.IsEffective == true)
            //    {
            //        return RedirectToAction("RegisterActivity_Omusic", "Account");
            //    }
            //}

            //if (TempData["Message"] == "" || TempData["Message"] == null)
            //{
            //    ViewBag.Message = "";
            //}
            //else
            //{
            //    ViewBag.Message = TempData["Message"];
            //}
            //// 活動開關 on: 啟動； off: 關閉
            //if (EvenIngOrNot == "on")
            //{
            //    DateTime nowDateTime = new DateTime();
            //    nowDateTime = DateTime.Now;
            //    var objEvent = objEventService.getEventById(numEventId);
            //    // 當下時間是否還在活動期間內
            //    if (nowDateTime >= objEvent.datestart && nowDateTime <= objEvent.dateend)
            //    {
            //        Api.CouponController apiController = new Api.CouponController();
            //        string str_CouponNumber = apiController.GetCouponNumber(numEventId).ToString();
            //        // 判斷是否coupon券還有
            //        if (str_CouponNumber != "0")
            //        {
            //            ViewBag.ActivityFlag = true;
            //            ViewBag.ActivityMessage = "";
            //        }
            //        else
            //        {
            //            ViewBag.ActivityFlag = false;
            //            ViewBag.ActivityMessage = "親愛的蛋友 您好，非常感謝大家的熱烈支持與愛護！新蛋迎新會首購折價券加碼又加碼，現已經全部發放完畢。這波來不及參與的朋友請放心，" + WebSiteData.SiteName + "將不定期推出優惠活動，請持續密切關注，謝謝。";
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.ActivityMessage = "";
            //        ViewBag.ActivityFlag = false;
            //    }
            //}
            //else
            //{
            //    //活動關閉
            //    ViewBag.ActivityMessage = "";
            //    ViewBag.ActivityFlag = false;
            //}

            //if (ActivityIngOrNot)
            //{
            //    return RedirectToAction("RegisterActivity", "Account");
            //}
            //else
            //{
            //    return View();
            //}
        }

        //[RequireSSL]
        [HttpPost]
        public ActionResult Register(Account account, string googlecaptchaM)
        {
            TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService httpRequest = new Service.CommonService.HttpWebRequestService();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("secret", TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService.GOOGLE_SECRET_KEY);
            parameters.Add("response", googlecaptchaM);

            var result = httpRequest.Get("https://www.google.com/recaptcha/api/siteverify", parameters);

            if (!httpRequest.CheckGooglereCaptChaMessage(result))
            {
                ViewBag.Register_Status = "您輸入的驗證碼錯誤";
                return View(account);
            }
            ClearCookie(Request, Response);

            Account objAccount = new Account();
            AccountVerify oAccountService = new AccountVerify();
            int numNewAccountId = 0;
            Member memberlist = new Member();
            TWSqlDBContext db = new TWSqlDBContext();
            ViewBag.Register_Status = "";
            if (string.IsNullOrEmpty(account.Email))
            {
                ViewBag.Register_Status = "請輸入帳號";
                return View(account);
            }
            string newEmail = account.Email.Trim();
            bool isEmail = Regex.IsMatch(account.Email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);

            if (isEmail == false)
            {
                ViewBag.Register_Status = "請輸入正確的帳號格式";
                return View(account);
            }
            if (oAccountService.HasAccountEmail(newEmail, out objAccount))
            {
                ViewBag.Register_Status = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請直接做會員登入或使用新e-mail註冊成為會員";
                return View(account);
            }
            else
            {
                if (Request.Cookies["ValidateCode"] == null)
                {
                    ViewBag.Register_Status = "系統錯誤，請重新註冊";
                    return View(account);
                }
                else
                {
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    if (string.IsNullOrEmpty(account.PWD))
                    {
                        ViewBag.Register_Status = "請輸入密碼";
                        return View(account);
                    }
                    string strAccVerify = oAccountService.VerifyAccountRule(account.PWD, account.Email);
                    if (string.IsNullOrEmpty(account.PWDtxt))
                    {
                        ViewBag.Register_Status = "請輸入確認密碼";
                        return View(account);
                    }
                    else if (account.PWD.Length < 8)
                    {
                        ViewBag.Register_Status = "密碼長度不可小於八";
                        return View(account);
                    }
                    else if (strAccVerify.IndexOf("NoPass") >= 0)
                    {
                        ViewBag.Register_Status = "密碼格式輸入錯誤";
                        return View(account);
                    }
                    if (account.PWD != account.PWDtxt)
                    {
                        ViewBag.Register_Status = "密碼與確認密碼不相同";
                        return View(account);
                    }
                    if (account.Sex == -1)
                    {
                        ViewBag.Register_Status = "請選擇稱謂";
                        return View(account);
                    }
                    else if (string.IsNullOrEmpty(account.Nickname))
                    {
                        ViewBag.Register_Status = "請輸入姓氏與名字";
                        return View(account);
                    }
                    try
                    {
                        string[] resultStringName = Regex.Split(account.Name, account.Nickname, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        ViewBag.Register_Status = "請填寫姓氏與姓名";
                        return View(account);
                    }

                    if (string.IsNullOrEmpty(account.Birthday))
                    {
                        ViewBag.Register_Status = "請選擇您的生日";
                        return View(account);
                    }
                    bool isBirthday = Regex.IsMatch(account.Birthday, @"^[0-9]{4}\/(((0[13578]|(10|12))\/(0[1-9]|[1-2][0-9]|3[0-1]))|(02\/(0[1-9]|[1-2][0-9]))|((0[469]|11)\/(0[1-9]|[1-2][0-9]|30)))$", RegexOptions.IgnoreCase);
                    if (isBirthday == false)
                    {
                        ViewBag.Register_Status = "生日格式錯誤，請重新選擇您的生日";
                        return View(account);
                    }
                    if (CheckValidateCode != account.ValidateCode)
                    {
                        ViewBag.Register_Status = "您輸入的驗證碼錯誤";
                        return View(account);
                    }
                    if (string.IsNullOrEmpty(account.Mobile))
                    {
                        ViewBag.Register_Status = "請您輸入手機號碼";
                        return View(account);
                    }
                    bool isMobile = System.Text.RegularExpressions.Regex.IsMatch(account.Mobile, @"^(09)([0-9]{8})$");
                    if (isMobile == false)
                    {
                        ViewBag.Register_Status = "手機格式錯誤，請重新輸入";
                        return View(account);
                    }
                }
                int rand;
                char pd;
                string newlinks = String.Empty;
                //生成重設密碼用的連結路徑
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
                ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;
                string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                Response.Cookies["em"].Value = AesEnc.AESenprypt(newEmail);
                Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
                Response.Cookies["NewLinks"].Value = newlinks;
                Response.Cookies["Password"].Value = AesEnc.AESenprypt(account.PWD);
                //會員註冊時間
                account.Registeron = DateTime.Now;
                account.CreateDate = DateTime.Now;
                account.Loginon = DateTime.Now;
                account.NewLinks = newlinks;
                account.LoginStatus = 1;
                account.RememberMe = 0;
                account.ActionCode = "C";
                account.Istosap = 0;

                // 判斷需要新增還是修改
                if (objAccount == null)
                {
                    numNewAccountId = oAccountService.CreateAccount(account);
                }
                else
                {
                    numNewAccountId = objAccount.ID;
                    account.ID = numNewAccountId;
                    oAccountService.UpdateAccount(account, true);
                }
                //新增會員資料
                memberlist.AccID = numNewAccountId;
                memberlist.Mobile = account.Mobile;
                string[] resultString = Regex.Split(account.Name, account.Nickname, RegexOptions.IgnoreCase);
                memberlist.Firstname = account.Nickname;
                memberlist.Lastname = resultString[0];
                memberlist.Birthday = account.Birthday;
                memberlist.CreateDate = DateTime.Now;
                memberlist.Sex = account.Sex;
                db.Member.Add(memberlist);
                db.SaveChanges();


                /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                DateTime objDateNow = DateTime.Now;
                //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                //Convert.ToDateTime("2014/6/9 14:00:00")-->改成configure
                //int numEventId = 20;-->改成configure
                int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
                var ActivityDate = db.Event.Where(p => p.id == numEventId).FirstOrDefault();


                bool ActivityIngOrNot = EvenStatus(numEventId);
                //利用EvenId獲得開始與結束時間
                if (ActivityIngOrNot == true)
                {
                    if (DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.datestart)) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.dateend)) <= 0)
                    {
                        TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository objCouponService = null;
                        numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]); ;//鎖定活動使用20
                        //objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, account.ID.ToString());
                        Processor.Request<Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption, Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", numEventId, account.ID.ToString());
                        
                        objCouponService = null;
                    }
                }
                /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                Mail_Message(account, "RegisterMessage"); // 註冊成功通知信
                //FormsAuthentication.SetAuthCookie(model.account_email, false);
                Account searchAccount = new Account();
                searchAccount = (from p in db.Account where p.Email == newEmail select p).FirstOrDefault();
                string mainDomain = string.Empty;

                if (environment == "DEV")
                {
                    mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
                }
                else
                {
                    mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
                }
                Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                Response.Cookies["Accountid"].Domain = mainDomain;
                Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                Response.Cookies["LoginStatus"].Domain = mainDomain;
                Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
                Response.Cookies["IE"].Domain = mainDomain;


                ICarts repository = new CartsRepository();
                repository.SetTrackAll(searchAccount.ID, searchAccount.CreateDate.ToString()); //Set accid 
                repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.
                try
                {
                    if (Request.Cookies["cart"] != null)
                    {
                        string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                        shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                        List<CookieCart> cookieCarts = new List<CookieCart>();
                        cookieCarts = findShippingCart(shippingCart); //Trans it to model

                        bool checkadd = new bool();
                        checkadd = true;
                        foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                        {
                            if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                            {
                                //checkadd = true;
                                //add cart success;
                            }
                            else
                            {
                                checkadd = false;
                                //add cart fail
                            }
                        }
                        if (checkadd)
                        {
                            Response.Cookies["cart"].Value = "";
                            Response.Cookies["cart"].Domain = mainDomain;
                            Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                            Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                        }
                        else
                        {
                            Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                            Response.Cookies["cart"].Domain = mainDomain;
                            Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                            Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                        }
                    }
                    else { }

                }
                catch (Exception e)
                {
                }

                return RedirectToAction("AutoLogin", "Account");
            }

        }

        // 會員註冊頁面
        //[AllowAnonymous]
        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult RegisterActivity()
        {
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
            bool ActivityIngOrNot = EvenStatus(numEventId);
            if (ActivityIngOrNot == false)
            {
                TempData["Message"] = "活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                return RedirectToAction("Register", "Account");
            }
            ClearCookie(Request, Response);
            return View();
        }

        // 寫入會員資料
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public ActionResult RegisterActivity(Account model)
        {
            ClearCookie(Request, Response);
            if (model.AgreePaper != 1)
            {
                //ViewBag.account_confirm = "請閱讀及勾選同意會員條款";
                return View();
            }

            /*
            oCookie = new HttpCookie("newEgg_Login");
            oCookie.Name = "newEgg_Login";
            oCookie.Expires = System.DateTime.Now.AddMinutes(1440);
            // 初始欄位設定
            oCookie.Values.Add("em", "");
            oCookie.Values.Add("NewLinks", "");
            oCookie.Values.Add("Expires", "");
            */
            //FormsAuthentication.SignOut();
            ViewBag.account_confirm = "";
            ViewBag.ValicodeCheck = "";
            TWSqlDBContext db = new TWSqlDBContext();
            string newEmail = String.Empty;
            Member memberlist = new Member();
            Accountactcheck accountcheck = new Accountactcheck();
            //Account驗證相關使用的變數
            AccountVerify oAccountService = null;
            Account objAccount = new Account();
            string strAccVerify = "";
            int numNewAccountId = 0;

            oAccountService = new AccountVerify();


            if (model.Email != null && model.Email.Length > 0)
            {
                newEmail = model.Email.Trim();
            }
            if (newEmail != null && model.PWD != null && newEmail != "" && model.PWD != "")
            {
                // 檢查會員是否已經存在

                //var chk_member = (from p in db.Account where p.Email == newEmail select p.Email).FirstOrDefault();
                if (oAccountService.HasAccountEmail(newEmail, out objAccount))
                {
                    ViewBag.account_confirm = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請直接做會員登入或使用新e-mail註冊成為會員";
                    return View();
                }
                else
                {
                    if (Request.Cookies["ValidateCode"] == null)
                    {
                        ViewBag.ValicodeCheck = "系統錯誤，請重新註冊";
                        return View();
                    }
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    if (model.ValidateCode == null || model.ValidateCode == "")
                    { return View(); }
                    if (CheckValidateCode != model.ValidateCode)
                    {
                        ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                        return View();
                    }

                    //驗證密碼格式
                    strAccVerify = oAccountService.VerifyAccountRule(model.PWD, model.Email);
                    if (strAccVerify.IndexOf("NoPass") >= 0)
                    {
                        ViewBag.account_confirm = "密碼格式輸入錯誤";
                        return View();
                    }

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
                    ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                    Response.Cookies["em"].Value = AesEnc.AESenprypt(newEmail);
                    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
                    Response.Cookies["NewLinks"].Value = newlinks;
                    //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(1440);

                    Response.Cookies["Password"].Value = AesEnc.AESenprypt(model.PWD);
                    accountcheck = db.Accountactcheck.Where(x => x.Phone == model.Mobile).FirstOrDefault();
                    if (accountcheck == null)
                    {
                        ViewBag.ValicodeCheck = "手機未證過";
                        return View();
                    }
                    if (accountcheck.Status != (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success)
                    {
                        ViewBag.ValicodeCheck = "手機驗證失敗";
                        return View();
                    }

                    if (accountcheck.Status == (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success && string.IsNullOrWhiteSpace(accountcheck.User_id) == false)
                    {
                        ViewBag.ValicodeCheck = "手機註冊過了";
                        return View();
                    }
                    // 會員註冊時間
                    model.Registeron = DateTime.Now;
                    model.CreateDate = DateTime.Now;
                    model.Loginon = DateTime.Now;
                    //model.PWD = AesEnc.AESenprypt(model.PWD);
                    //model.PWDtxt = model.PWD;
                    model.NewLinks = newlinks;
                    model.LoginStatus = 1;
                    model.RememberMe = 0;
                    model.ActionCode = "C";
                    model.Istosap = 0;

                    // 判斷需要新增還是修改
                    if (objAccount == null)
                    {
                        numNewAccountId = oAccountService.CreateAccount(model);
                    }
                    else
                    {
                        numNewAccountId = objAccount.ID;
                        model.ID = numNewAccountId;
                        oAccountService.UpdateAccount(model, true);
                    }

                    //新增會員資料
                    memberlist.AccID = numNewAccountId;
                    memberlist.Mobile = model.Mobile;
                    string[] resultString = Regex.Split(model.Name, model.Nickname, RegexOptions.IgnoreCase);
                    memberlist.Firstname = model.Nickname;
                    memberlist.Lastname = resultString[0];
                    memberlist.Birthday = model.Birthday;
                    memberlist.CreateDate = DateTime.Now;
                    memberlist.Sex = model.Sex;
                    db.Member.Add(memberlist);
                    //回存Accountactcheck部分資料


                    accountcheck.Firstname = model.Nickname;
                    accountcheck.Lastname = resultString[0];
                    accountcheck.User_id = numNewAccountId.ToString();
                    db.SaveChanges();

                    /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    DateTime objDateNow = DateTime.Now;
                    //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                    //Convert.ToDateTime("2014/6/9 14:00:00")-->改成configure
                    //int numEventId = 20;-->改成configure
                    int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
                    var EvenDate = db.Event.Where(p => p.id == numEventId).FirstOrDefault();


                    bool ActivityIngOrNot = EvenStatus(numEventId);
                    if (ActivityIngOrNot == true)
                    {
                        if (DateTime.Compare(objDateNow, Convert.ToDateTime(EvenDate.datestart)) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime(EvenDate.dateend)) <= 0)
                        {
                            TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository objCouponService = null;
                            //鎖定活動使用20
                            //objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, model.ID.ToString());
                            Processor.Request<Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption, Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", numEventId, model.ID.ToString());

                        }
                    }

                    /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */

                    Mail_Message(model, "RegisterMessage"); // 註冊成功通知信

                    //FormsAuthentication.SetAuthCookie(model.account_email, false);
                    Account searchAccount = new Account();
                    searchAccount = (from p in db.Account where p.Email == newEmail select p).FirstOrDefault();
                    string mainDomain = "";

                    if (environment == "DEV")
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
                    }
                    else
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
                    }
                    Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["Accountid"].Domain = mainDomain;
                    Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["LoginStatus"].Domain = mainDomain;
                    Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
                    Response.Cookies["IE"].Domain = mainDomain;
                    //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_id) + "_" + Convert.ToString(SelectAccount.account_loginon));
                    //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_loginstatus) + "_" + Convert.ToString(SelectAccount.account_loginon));
                    //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(SelectAccount.account_id) + "_" + Convert.ToString(SelectAccount.account_loginstatus) + "_" + Convert.ToString(SelectAccount.account_loginon)) + "e";

                    /*      */
                    ICarts repository = new CartsRepository();
                    //repository.SetTrackAll(SelectAccount.account_id, SelectAccount.account_loginon.ToString()); //Set accid 
                    repository.SetTrackAll(searchAccount.ID, searchAccount.CreateDate.ToString()); //Set accid 
                    repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.
                    try
                    {
                        if (Request.Cookies["cart"] != null)
                        {
                            string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                            shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                            //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                            List<CookieCart> cookieCarts = new List<CookieCart>();
                            cookieCarts = findShippingCart(shippingCart); //Trans it to model


                            bool checkadd = new bool();
                            checkadd = true;
                            foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                            {
                                if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                                {
                                    //checkadd = true;
                                    //add cart success;
                                }
                                else
                                {
                                    checkadd = false;
                                    //add cart fail
                                }
                            }
                            if (checkadd)
                            {
                                Response.Cookies["cart"].Value = "";
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                            else
                            {
                                Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }

                        }
                        else
                        {

                        }
                    }
                    catch (Exception e)
                    {
                    }
                    /*      */

                    //SendAuthCodeToMember(member);
                    return RedirectToAction("AutoLogin", "Account");
                }
            }

            return View();
        }

        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult Register_Facebook(string Uid, string Email)
        {
            #region 2015-03-23 by Rex
            Account oAccount = new Account();
            oAccount.Email = Email;
            oAccount.FacebookUID = Uid;

            //活動關閉
            ViewBag.ActivityMessage = "";
            ViewBag.ActivityFlag = false;
            // 進行中的活動名稱
            string ongoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            switch (ongoingActivitiesName.ToLower())
            {
                case "coupon500":
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                    int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
                    // True: 活動進行中； False: 活動結束
                    bool ActivityIngOrNot = EvenStatus(numEventId);
                    DateTime nowDateTime = new DateTime();
                    nowDateTime = DateTime.Now;
                    //var objEvent = objEventService.getEventById(numEventId);
                    TWNewEgg.Models.ViewModels.Redeem.Event objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.EventReponsitory", "getEventById", numEventId).results;
                    // 當下時間是否還在活動期間內
                    if (objEvent != null && nowDateTime >= objEvent.datestart && nowDateTime <= objEvent.dateend)
                    {
                        TWNewEgg.ECWeb.Controllers.Api.CouponController apiController = new TWNewEgg.ECWeb.Controllers.Api.CouponController();
                        string str_CouponNumber = apiController.GetCouponNumber(numEventId).ToString();
                        // 判斷是否coupon券還有
                        if (str_CouponNumber != "0")
                        {
                            ViewBag.ActivityFlag = true;
                            ViewBag.ActivityMessage = "";
                        }
                        else
                        {
                            ViewBag.ActivityFlag = false;
                            ViewBag.ActivityMessage = "親愛的蛋友 您好，非常感謝大家的熱烈支持與愛護！新蛋迎新會首購折價券加碼又加碼，現已經全部發放完畢。這波來不及參與的朋友請放心，" + WebSiteData.SiteName + "將不定期推出優惠活動，請持續密切關注，謝謝。";
                        }
                    }
                    else
                    {
                        ViewBag.ActivityMessage = "";
                        ViewBag.ActivityFlag = false;
                    }

                    if (ActivityIngOrNot)
                    {
                        return RedirectToAction("Register_FacebookActivity", "Account", oAccount);
                    }
                    break;
                case "omusic":
                    // 驗證Omusic是否在有效活動時間內
                    ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
                    ActivityData activityData = activityCheckService.GetActivityStatus("Omusic");
                    // 若Omusic活動在有效活動時間內則導向Omusic專屬頁面
                    if (activityData.IsEffective == true)
                    {
                        return RedirectToAction("RegisterActivity_Facebook_Omusic", "Account", oAccount);
                    }
                    break;
                case "apriljoinus":
                    break;
                default:
                    break;
            }

            return View(oAccount);
            #endregion

            #region 2015-03-23 before
            //string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            //int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
            //bool ActivityIngOrNot = EvenStatus(numEventId);
            //string EvenIngOrNot = (System.Configuration.ConfigurationManager.AppSettings["MobileVerification"]).ToString();
            //TWNewEgg.Redeem.Service.CouponService.EventReponsitory objEventService = new Redeem.Service.CouponService.EventReponsitory();

            //// Omusic活動是否啟用
            //string isOmusicEnable = System.Configuration.ConfigurationManager.AppSettings["IsOmusicEnable"];
            //// 若為啟動狀態則執行下列動作
            //if (isOmusicEnable.ToLower() == "on")
            //{
            //    // 驗證是否在Omusic有效活動時間內
            //    ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
            //    ActivityData activityData = activityCheckService.GetActivityStatus("Omusic");
            //    // 若是則導向Facebook Omusic專用頁面中
            //    if (activityData.IsEffective == true)
            //    {
            //        Account oAccount = new Account();
            //        oAccount.Email = Email;
            //        oAccount.FacebookUID = Uid;
            //        return RedirectToAction("RegisterActivity_Facebook_Omusic", "Account", oAccount);
            //    }
            //}

            //if (TempData["Message"] == "" || TempData["Message"] == null)
            //{
            //    ViewBag.MessageRegister = "";
            //}
            //else
            //{
            //    ViewBag.MessageRegister = TempData["Message"];
            //}

            //if (EvenIngOrNot == "on")
            //{
            //    DateTime nowDateTime = new DateTime();
            //    nowDateTime = DateTime.Now;
            //    var objEvent = objEventService.getEventById(numEventId);
            //    if (nowDateTime >= objEvent.datestart && nowDateTime <= objEvent.dateend)
            //    {
            //        Api.CouponController apiController = new Api.CouponController();
            //        string str_CouponNumber = apiController.GetCouponNumber(numEventId).ToString();
            //        if (str_CouponNumber != "0")
            //        {
            //            ViewBag.ActivityFlag = true;
            //            ViewBag.ActivityMessage = "";
            //        }
            //        else
            //        {
            //            ViewBag.ActivityFlag = false;
            //            ViewBag.ActivityMessage = "親愛的蛋友 您好，非常感謝大家的熱烈支持與愛護！新蛋迎新會首購折價券加碼又加碼，現已經全部發放完畢。這波來不及參與的朋友請放心，" + WebSiteData.SiteName + "將不定期推出優惠活動，請持續密切關注，謝謝。";
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.ActivityMessage = "";
            //        ViewBag.ActivityFlag = false;
            //    }
            //}
            //else//活動關閉
            //{
            //    ViewBag.ActivityMessage = "";
            //    ViewBag.ActivityFlag = false;
            //}
            //if (!ActivityIngOrNot)
            //{
            //    Account oAccount = new Account();
            //    oAccount.Email = Email;
            //    oAccount.FacebookUID = Uid;
            //    return View(oAccount);
            //}
            //else
            //{
            //    return RedirectToAction("Register_FacebookActivity", "Account");
            //}
            #endregion
        }

        //[RequireSSL]
        [HttpPost]
        public ActionResult Register_Facebook(Account model, string googlecaptchaM)
        {
            TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService httpRequest = new Service.CommonService.HttpWebRequestService();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("secret", TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService.GOOGLE_SECRET_KEY);
            parameters.Add("response", googlecaptchaM);

            var result = httpRequest.Get("https://www.google.com/recaptcha/api/siteverify", parameters);

            if (!httpRequest.CheckGooglereCaptChaMessage(result))
            {
                ViewBag.FacebookError = "驗證碼錯誤";
                return View(model);
            }
            ViewBag.account_confirm = "";
            ViewBag.ValicodeCheck = "";
            TWNewEgg.DB.TWSqlDBContext db = new TWSqlDBContext();
            //TWNewEgg.DB.TWSqlDBContext DBO = new TWSqlDBContext();
            //Account驗證相關使用的變數
            AccountVerify oAccountService = null;
            Account objAccount = new Account();
            int numNewAccountId = 0;
            Member memberlist = new Member();
            oAccountService = new AccountVerify();

            //檢查Mail
            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.PWD))
            {
                string newEmail = model.Email.Trim();
                //檢查帳號是否已經存在
                if (oAccountService.HasAccountEmail(model.Email, out objAccount))
                {
                    ViewBag.FacebookError = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請直接做會員登入或使用新e-mail註冊成為會員";
                    return View(model);
                }
                //帳號不存在
                else
                {
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    string strAccVerify = oAccountService.VerifyAccountRule(model.PWD, model.Email);
                    //檢查密碼
                    if (model.PWD == "" || model.PWD == null)
                    {
                        ViewBag.FacebookError = "請填寫您的密碼";
                        return View(model);
                    }
                    else if (model.PWD.Length < 8)
                    {
                        ViewBag.FacebookError = "密碼長度不可小於八";
                        return View(model);
                    }
                    else if (strAccVerify.IndexOf("NoPass") >= 0)
                    {
                        ViewBag.FacebookError = "密碼格式輸入錯誤";
                        return View(model);
                    }
                    if (model.PWD != model.PWDtxt)
                    {
                        ViewBag.FacebookError = "密碼與確認密碼不相同";
                        return View(model);
                    }
                    //檢查姓名
                    string[] resultString;
                    if (model.Sex == -1)
                    {
                        ViewBag.FacebookError = "請選擇稱謂";
                        return View(model);
                    }
                    if (model.Nickname == "" || model.Nickname == null)
                    {
                        ViewBag.FacebookError = "請填寫姓氏";
                        return View(model);
                    }
                    try
                    {
                        resultString = Regex.Split(model.Name, model.Nickname, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        ViewBag.FacebookError = "請填寫姓氏與姓名";
                        return View(model);
                    }
                    if (model.Birthday == "" || model.Birthday == null)
                    {
                        ViewBag.FacebookError = "請選擇生日";
                        return View(model);
                    }
                    bool isBirthday = Regex.IsMatch(model.Birthday, @"^[0-9]{4}\/(((0[13578]|(10|12))\/(0[1-9]|[1-2][0-9]|3[0-1]))|(02\/(0[1-9]|[1-2][0-9]))|((0[469]|11)\/(0[1-9]|[1-2][0-9]|30)))$");
                    if (isBirthday == false)
                    {
                        ViewBag.FacebookError = "生日格式錯誤，請重新選擇";
                        return View(model);
                    }
                    //檢查驗證碼
                    if (model.ValidateCode == "" || model.ValidateCode == null)
                    {
                        ViewBag.FacebookError = "請填寫驗證碼";
                        return View(model);
                    }
                    else if (model.ValidateCode != CheckValidateCode)
                    {
                        ViewBag.FacebookError = "驗證碼錯誤";
                        return View(model);
                    }
                    bool isMobile = System.Text.RegularExpressions.Regex.IsMatch(model.Mobile, @"^(09)([0-9]{8})$");
                    if (model.Mobile == "" || model.Mobile == null)
                    {
                        ViewBag.FacebookError = "請填手機號碼";
                        return View(model);
                    }
                    else if (isMobile == false)
                    {
                        ViewBag.FacebookError = "手機格式錯誤";
                        return View(model);
                    }
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
                    ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;
                    Response.Cookies["em"].Value = AesEnc.AESenprypt(model.Email);
                    Response.Cookies["NewLinks"].Value = newlinks;
                    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));

                    Response.Cookies["Password"].Value = AesEnc.AESenprypt(model.PWD);
                    // 會員註冊時間
                    model.Registeron = DateTime.Now;
                    model.Loginon = DateTime.Now;
                    model.NewLinks = newlinks;
                    model.LoginStatus = 1;
                    model.RememberMe = 0;
                    model.ActionCode = "C";
                    model.Istosap = 0;

                    // 判斷需要新增還是修改
                    if (objAccount == null)
                    {
                        numNewAccountId = oAccountService.CreateAccount(model);
                    }
                    else
                    {
                        numNewAccountId = objAccount.ID;
                        model.ID = numNewAccountId;
                        oAccountService.UpdateAccount(model, true);
                    }
                    memberlist.AccID = numNewAccountId;
                    memberlist.Mobile = model.Mobile;
                    resultString = Regex.Split(model.Name, model.Nickname, RegexOptions.IgnoreCase);
                    memberlist.Firstname = model.Nickname;
                    memberlist.Lastname = resultString[0];
                    memberlist.Birthday = model.Birthday;
                    memberlist.CreateDate = DateTime.Now;
                    memberlist.Sex = model.Sex;
                    db.Member.Add(memberlist);

                    db.SaveChanges();
                    /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    DateTime objDateNow = DateTime.Now;
                    //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                    int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);


                    bool ActivityIngOrNot = EvenStatus(numEventId);
                    var ActivityDate = db.Event.Where(p => p.id == numEventId).FirstOrDefault();
                    //DateTime.Compare(objDateNow, Convert.ToDateTime("2014/11/28 00:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/12/11 00:00:00")) <= 0
                    if (ActivityIngOrNot == true)
                    {
                        if (DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.datestart)) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.dateend)) <= 0)
                        {
                            //int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);//鎖定活動使用20;//鎖定活動使用20
                            //objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, model.ID.ToString());
                            Processor.Request<Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption, Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", numEventId, model.ID.ToString());
                        }
                    }
                    /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    model = (from p in db.Account where p.Email == model.Email select p).FirstOrDefault();
                    string mainDomain = "";
                    if (environment == "DEV")
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
                    }
                    else
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
                    }
                    Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(model.ID) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["Accountid"].Domain = mainDomain;
                    Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(model.LoginStatus) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["LoginStatus"].Domain = mainDomain;
                    Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(model.ID) + "_" + Convert.ToString(model.LoginStatus) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
                    Response.Cookies["IE"].Domain = mainDomain;
                    ICarts repository = new CartsRepository();
                    //repository.SetTrackAll(model.account_id, model.account_loginon.ToString()); //Set accid 
                    repository.SetTrackAll(model.ID, model.CreateDate.ToString()); //Set accid 
                    repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.

                    try
                    {
                        if (Request.Cookies["cart"] != null)
                        {
                            string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                            shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                            //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                            List<CookieCart> cookieCarts = new List<CookieCart>();
                            cookieCarts = findShippingCart(shippingCart); //Trans it to model


                            bool checkadd = new bool();
                            checkadd = true;

                            foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                            {
                                if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                                {
                                    //checkadd = true;
                                    //add cart success;
                                }
                                else
                                {
                                    checkadd = false;
                                    //add cart fail
                                }
                            }
                            if (checkadd)
                            {
                                Response.Cookies["cart"].Value = "";
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                            else
                            {
                                Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                        }
                        else { }



                    }
                    catch (Exception e)
                    {
                    }
                    return RedirectToAction("AutoLogin", "Account", model);
                }
            }
            return View();
        }

        // 會員註冊頁面
        //[AllowAnonymous]
        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult Register_FacebookActivity(string Uid, string Email)
        {


            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
            bool ActivityIngOrNot = EvenStatus(numEventId);
            if (ActivityIngOrNot == false)
            {
                TempData["Message"] = "活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                return RedirectToAction("Register_Facebook", "Account");
            }
            else
            {
                Account oAccount = new Account();
                oAccount.Email = Email;
                oAccount.FacebookUID = Uid;
                return View(oAccount);
            }

        }

        // 寫入會員資料
        //[AllowAnonymous]
        //[RequireSSL]
        [HttpPost]
        public ActionResult Register_FacebookActivity(Account model)
        {
            //ClearCookie(Request, Response);

            //FormsAuthentication.SignOut();
            ViewBag.account_confirm = "";
            ViewBag.ValicodeCheck = "";
            TWNewEgg.DB.TWSqlDBContext db = new TWSqlDBContext();
            TWNewEgg.DB.TWSqlDBContext DBO = new TWSqlDBContext();
            //Account驗證相關使用的變數
            AccountVerify oAccountService = null;
            int numNewAccountId = 0;
            Member memberlist = new Member();
            Accountactcheck accountcheck = new Accountactcheck();
            Account objAccount = new Account();

            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.PWD))
            {
                oAccountService = new AccountVerify();
                // 檢查會員是否已經存在
                //var chk_member = (from p in db.Account where p.Email == model.Email select p.Email).FirstOrDefault();

                if (oAccountService.HasAccountEmail(model.Email, out objAccount))
                {
                    ViewBag.account_confirm = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請做會員登入或使用另一個新的e-mail做新會員帳號註冊";
                    //ModelState.AddModelError("", "您輸入的 Email 已經有人註冊過了!");
                    return View();
                }
                else
                {
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    if (CheckValidateCode != model.ValidateCode)
                    {
                        ViewBag.ValicodeCheck = "您輸入的驗證碼錯誤";
                        return View();
                    }
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
                    ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;

                    Response.Cookies["em"].Value = AesEnc.AESenprypt(model.Email);
                    Response.Cookies["NewLinks"].Value = newlinks;
                    Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
                    //Response.Cookies["em"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["NewLinks"].Expires = System.DateTime.Now.AddMinutes(1440);
                    //Response.Cookies["ex"].Expires = System.DateTime.Now.AddMinutes(1440);

                    Response.Cookies["Password"].Value = AesEnc.AESenprypt(model.PWD);

                    // 會員註冊時間
                    model.Registeron = DateTime.Now;
                    model.Loginon = DateTime.Now;
                    //model.PWDtxt = AesEnc.AESenprypt(model.PWD);
                    //model.PWD = AesEnc.AESenprypt(model.PWD);
                    model.NewLinks = newlinks;
                    model.LoginStatus = 1;
                    model.RememberMe = 0;
                    model.ActionCode = "C";
                    model.Istosap = 0;

                    // 判斷需要新增還是修改
                    if (objAccount == null)
                    {
                        numNewAccountId = oAccountService.CreateAccount(model);
                    }
                    else
                    {
                        numNewAccountId = objAccount.ID;
                        model.ID = numNewAccountId;
                        oAccountService.UpdateAccount(model, true);
                    }
                    accountcheck = DBO.Accountactcheck.Where(x => x.Phone == model.Mobile).FirstOrDefault();
                    if (accountcheck == null)
                    {
                        ViewBag.ValicodeCheck = "手機未證過";
                        return View();
                    }
                    if (accountcheck.Status != (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success)
                    {
                        ViewBag.ValicodeCheck = "手機驗證失敗";
                        return View();
                    } if (accountcheck.Status == (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success && string.IsNullOrWhiteSpace(accountcheck.User_id) == false)
                    {
                        ViewBag.ValicodeCheck = "已註冊過了";
                        return View();
                    }
                    //新增會員資料
                    memberlist.AccID = numNewAccountId;
                    memberlist.Mobile = model.Mobile;
                    string[] resultString = Regex.Split(model.Name, model.Nickname, RegexOptions.IgnoreCase);
                    memberlist.Firstname = model.Nickname;
                    memberlist.Lastname = resultString[0];
                    memberlist.Birthday = model.Birthday;
                    memberlist.CreateDate = DateTime.Now;
                    memberlist.Sex = model.Sex;

                    db.Member.Add(memberlist);
                    //回存Accountactcheck部分資料

                    accountcheck.Firstname = model.Nickname;
                    accountcheck.Lastname = resultString[0];
                    accountcheck.User_id = numNewAccountId.ToString();
                    db.SaveChanges();
                    DBO.SaveChanges();



                    /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    DateTime objDateNow = DateTime.Now;
                    //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                    int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
                    var EvenDate = DBO.Event.Where(p => p.id == numEventId).FirstOrDefault();


                    bool ActivityIngOrNot = EvenStatus(numEventId);
                    if (ActivityIngOrNot == true)
                    {
                        if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/11/28 00:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/12/26 00:00:00")) <= 0)
                        {
                            //int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);//鎖定活動使用20;//鎖定活動使用20
                            //objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, model.ID.ToString());
                            Processor.Request<Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption, Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByEventIdAndUserAccount", numEventId, model.ID.ToString());
                        }
                        /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
                    }
                    model = (from p in db.Account where p.Email == model.Email select p).FirstOrDefault();
                    //FormsAuthentication.SetAuthCookie(model.account_email, false);
                    string mainDomain = "";

                    if (environment == "DEV")
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
                    }
                    else
                    {
                        mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
                    }
                    Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(model.ID) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["Accountid"].Domain = mainDomain;
                    Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(model.LoginStatus) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
                    Response.Cookies["LoginStatus"].Domain = mainDomain;
                    Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(model.ID) + "_" + Convert.ToString(model.LoginStatus) + "_" + Convert.ToString(model.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
                    Response.Cookies["IE"].Domain = mainDomain;
                    //Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(model.account_id) + "_" + Convert.ToString(model.account_loginon));
                    //Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(model.account_loginstatus) + "_" + Convert.ToString(model.account_loginon));
                    //Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(model.account_id) + "_" + Convert.ToString(model.account_loginstatus) + "_" + Convert.ToString(model.account_loginon)) + "e";

                    /*      */
                    ICarts repository = new CartsRepository();
                    //repository.SetTrackAll(model.account_id, model.account_loginon.ToString()); //Set accid 
                    repository.SetTrackAll(model.ID, model.CreateDate.ToString()); //Set accid 
                    repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.
                    try
                    {
                        if (Request.Cookies["cart"] != null)
                        {
                            string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                            shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                            //shippingCart = HttpUtility.HtmlEncode(shippingCart);

                            List<CookieCart> cookieCarts = new List<CookieCart>();
                            cookieCarts = findShippingCart(shippingCart); //Trans it to model


                            bool checkadd = new bool();
                            checkadd = true;
                            foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                            {
                                if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                                {
                                    //checkadd = true;
                                    //add cart success;
                                }
                                else
                                {
                                    checkadd = false;
                                    //add cart fail
                                }
                            }
                            if (checkadd)
                            {
                                Response.Cookies["cart"].Value = "";
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }
                            else
                            {
                                Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cart"].Domain = mainDomain;
                                Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                                Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                            }

                        }
                        else
                        {

                        }
                    }
                    catch (Exception e)
                    {
                    }
                    return RedirectToAction("AutoLogin", "Account", model);
                }
            }
            return View();
        }

        /// <summary>
        /// 活動是否進行中
        /// </summary>
        /// <param name="Evenid">事件的活動編號</param>
        /// <returns>T:活動進行中；F:活動結束</returns>
        public bool EvenStatus(int Evenid)
        {
            string OngoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            //string EvenIngOrNot = (System.Configuration.ConfigurationManager.AppSettings["MobileVerification"]).ToString();
            TWNewEgg.DB.TWSqlDBContext db = new TWSqlDBContext();

            if (OngoingActivitiesName.ToLower() != "coupon500")//活動關閉
            {
                return false;
            }
            else
            {
                TWNewEgg.ECWeb.Controllers.Api.CouponController apiController = new TWNewEgg.ECWeb.Controllers.Api.CouponController();
                string str_CouponNumber = apiController.GetCouponNumber(Evenid).ToString();
                if (str_CouponNumber == "0")//張數發完
                {
                    return false;
                }
                else
                {//張數還沒發完，現在時間是否在活動時間範圍區間內 
                    DateTime nowDateTime = new DateTime();
                    nowDateTime = DateTime.Now;
                    TWNewEgg.Models.ViewModels.Redeem.Event objEvent;
                    //objEvent = objEventService.getEventById(Evenid);
                    objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.EventReponsitory", "getEventById", Evenid).results;
                    var EvenData = db.Event.Where(p => p.id == Evenid).First();
                    if (nowDateTime >= objEvent.datestart && nowDateTime <= objEvent.dateend)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        // 會員註冊頁面
        //[AllowAnonymous]
        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult RegisterActivity_Omusic()
        {
            // Omusic活動是否啟用，驗證開關
            string ongoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            if (ongoingActivitiesName.ToLower() != "omusic")
            {
                TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                return RedirectToAction("Register", "Account");
            }
            //// Omusic活動是否啟用，驗證開關
            //string isOmusicEnable = System.Configuration.ConfigurationManager.AppSettings["IsOmusicEnable"];
            //if (isOmusicEnable.ToLower() != "on")
            //{
            //    TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
            //    return RedirectToAction("Register", "Account");
            //}
            // 驗證是否則有效活動時間內
            ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
            ActivityData activityData = activityCheckService.GetActivityStatus("Omusic");
            if (activityData.IsEffective != true)
            {
                TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                return RedirectToAction("Register", "Account");
            }

            ClearCookie(Request, Response);
            return View();
        }


        [HttpGet]
        //[RequireSSL]
        [OutputCache(Duration = 0)]
        public ActionResult Register_Introduction(string oldAccount, string newAccount)
        {
            Account _account = new Account();
            _account.Email = string.Empty;
            ViewBag.oldAccount = string.Empty;
            #region 活動是否啟用，驗證開關
            // apriljoinus活動是否啟用，驗證開關
            string ongoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            if (ongoingActivitiesName.ToLower() != "apriljoinus")
            {
                ViewBag.ErrorMsg = "活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                ViewBag.ReturnUrl = "../Home/Index";
                //TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                //return RedirectToAction("Register", "Account");
                return View(_account);
            }
            ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
            ActivityData activityData = activityCheckService.GetActivityStatus("apriljoinus");
            if (activityData.IsEffective != true)
            {
                ViewBag.ErrorMsg = "活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                ViewBag.ReturnUrl = "../Home/Index";
                //TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                //return RedirectToAction("Register", "Account");
                return View(_account);
            }
            #endregion

            #region 檢查新舊會員EMAIL不可為空
            if (string.IsNullOrEmpty(oldAccount))
            {
                ViewBag.ErrorMsg = "錯誤";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            if (string.IsNullOrEmpty(newAccount))
            {
                ViewBag.ErrorMsg = "錯誤";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            #endregion
            #region 檢查推薦人與被推薦人MAIL格式是否正確
            bool oldAccountCheck = Regex.IsMatch(oldAccount, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            if (oldAccountCheck == false)
            {
                ViewBag.ErrorMsg = "推薦人信箱格式錯誤";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            bool NewAccountCheck = Regex.IsMatch(newAccount, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            if (NewAccountCheck == false)
            {
                ViewBag.ErrorMsg = "被推薦人信箱格式錯誤";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            #endregion
            TWNewEgg.DB.TWSqlDBContext db = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.Account _accountCheck = new Account();
            #region 檢查推薦人與被推薦人是否為新蛋之會員
            _accountCheck = db.Account.Where(p => p.Email == oldAccount).FirstOrDefault();
            if (_accountCheck == null)
            {
                ViewBag.ErrorMsg = "推薦人不為" + WebSiteData.SiteName + "之會員";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            _accountCheck = null;
            _accountCheck = db.Account.Where(p => p.Email == newAccount).FirstOrDefault();
            if (_accountCheck != null)
            {
                ViewBag.ErrorMsg = "被推薦人已經為" + WebSiteData.SiteName + "之會員";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            #endregion
            AccountJoinGroup _bindingAccount = db.AccountJoinGroup.Where(p => p.Old_Account == oldAccount && p.New_Account == newAccount).FirstOrDefault();
            if (_bindingAccount == null)
            {
                ViewBag.ErrorMsg = "「請從e-mail中的連結到註冊頁」";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(_account);
            }
            _account.Email = newAccount;
            ViewBag.oldAccount = oldAccount;
            return View(_account);
        }

        //[RequireSSL]
        [HttpPost]
        public ActionResult Register_Introduction(Account account, string googlecaptchaM, string OldAccount)
        {
            #region 活動是否啟用，驗證開關
            // apriljoinus活動是否啟用，驗證開關
            string ongoingActivitiesName = System.Configuration.ConfigurationManager.AppSettings["OngoingActivitiesName"];
            if (ongoingActivitiesName.ToLower() != "apriljoinus")
            {
                return RedirectToAction("Index", "Home");
                //TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                //return RedirectToAction("Register", "Account");
            }
            ActivityOnlineCheckService activityCheckService = new ActivityOnlineCheckService();
            ActivityData activityData = activityCheckService.GetActivityStatus("apriljoinus");
            if (activityData.IsEffective != true)
            {
                //TempData["Message"] = "新蛋會員 x Omusic活動已結束，如有任何問題請洽詢客服人員，謝謝您";
                //return RedirectToAction("Register", "Account");
                return RedirectToAction("Index", "Home");
            }
            //if (Request.Cookies["em"] == null)
            //{
            //    return RedirectToAction("Login", "Account", new { returnUrl = "/Activity/JoinUSIntroduction" });
            //}
            #endregion
            ViewBag.NewAccount = account.Email;
            ViewBag.oldAccount = OldAccount;
            TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService httpRequest = new Service.CommonService.HttpWebRequestService();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("secret", TWNewEgg.Website.ECWeb.Service.CommonService.HttpWebRequestService.GOOGLE_SECRET_KEY);
            parameters.Add("response", googlecaptchaM);

            var result = httpRequest.Get("https://www.google.com/recaptcha/api/siteverify", parameters);

            if (!httpRequest.CheckGooglereCaptChaMessage(result))
            {
                ViewBag.Register_Status = "您輸入的驗證碼錯誤";
                return View(account);
            }
            //ClearCookie(Request, Response);
            Account objAccount = new Account();
            AccountVerify oAccountService = new AccountVerify();
            int numNewAccountId = 0;
            Member memberlist = new Member();
            AccountJoinGroup accountJoinGroup = new AccountJoinGroup();
            TWSqlDBContext db = new TWSqlDBContext();
            ViewBag.Register_Status = string.Empty;

            #region 檢查推薦人EMAIL 格式 預防前端被擅改程式碼
            if (string.IsNullOrEmpty(OldAccount))
            {
                ViewBag.Register_Status = "Error";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(account);
            }
            bool isEmailOldAccount = Regex.IsMatch(OldAccount, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            if (isEmailOldAccount == false)
            {
                ViewBag.Register_Status = "Error";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(account);
            }
            #endregion

            if (string.IsNullOrEmpty(account.Email))
            {
                ViewBag.Register_Status = "Error";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(account);
            }
            string newEmail = account.Email.Trim();
            bool isEmail = Regex.IsMatch(account.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            if (isEmail == false)
            {
                ViewBag.Register_Status = "Error";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(account);
            }
            if (oAccountService.HasAccountEmail(newEmail, out objAccount))
            {
                ViewBag.Register_Status = "您輸入的e-mail已是" + WebSiteData.SiteName + "會員，請直接做會員登入或使用新e-mail註冊成為會員";

                return View(account);
            }
            else
            {
                if (Request.Cookies["ValidateCode"] == null)
                {
                    ViewBag.Register_Status = "系統錯誤，請重新註冊";

                    return View(account);
                }
                else
                {
                    string CheckValidateCode = Request.Cookies["ValidateCode"].Value;
                    if (string.IsNullOrEmpty(account.PWD))
                    {
                        ViewBag.Register_Status = "請輸入密碼";

                        return View(account);
                    }
                    string strAccVerify = oAccountService.VerifyAccountRule(account.PWD, account.Email);
                    if (string.IsNullOrEmpty(account.PWDtxt))
                    {
                        ViewBag.Register_Status = "請輸入確認密碼";

                        return View(account);
                    }
                    else if (account.PWD.Length < 8)
                    {
                        ViewBag.Register_Status = "密碼長度不可小於八";

                        return View(account);
                    }
                    else if (strAccVerify.IndexOf("NoPass") >= 0)
                    {
                        ViewBag.Register_Status = "密碼格式輸入錯誤";

                        return View(account);
                    }
                    if (account.PWD != account.PWDtxt)
                    {
                        ViewBag.Register_Status = "密碼與確認密碼不相同";

                        return View(account);
                    }
                    if (account.Sex == -1)
                    {
                        ViewBag.Register_Status = "請選擇稱謂";

                        return View(account);
                    }
                    else if (string.IsNullOrEmpty(account.Nickname))
                    {
                        ViewBag.Register_Status = "請輸入姓氏與名字";

                        return View(account);
                    }
                    try
                    {
                        string[] resultStringName = Regex.Split(account.Name, account.Nickname, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        ViewBag.Register_Status = "請填寫姓氏與姓名";

                        return View(account);
                    }

                    if (string.IsNullOrEmpty(account.Birthday))
                    {
                        ViewBag.Register_Status = "請選擇您的生日";

                        return View(account);
                    }
                    bool isBirthday = Regex.IsMatch(account.Birthday, @"^[0-9]{4}\/(((0[13578]|(10|12))\/(0[1-9]|[1-2][0-9]|3[0-1]))|(02\/(0[1-9]|[1-2][0-9]))|((0[469]|11)\/(0[1-9]|[1-2][0-9]|30)))$", RegexOptions.IgnoreCase);
                    if (isBirthday == false)
                    {
                        ViewBag.Register_Status = "生日格式錯誤，請重新選擇您的生日";

                        return View(account);
                    }
                    if (CheckValidateCode != account.ValidateCode)
                    {
                        ViewBag.Register_Status = "您輸入的驗證碼錯誤";

                        return View(account);
                    }
                    if (string.IsNullOrEmpty(account.Mobile))
                    {
                        ViewBag.Register_Status = "請您輸入手機號碼";

                        return View(account);
                    }
                    bool isMobile = System.Text.RegularExpressions.Regex.IsMatch(account.Mobile, @"^(09)([0-9]{8})$");
                    if (isMobile == false)
                    {
                        ViewBag.Register_Status = "手機格式錯誤，請重新輸入";

                        return View(account);
                    }
                    if (account.AgreePaper != 1)
                    {
                        ViewBag.Register_Status = "請勾選\"我已閱讀完畢，並同意會員條款\"";

                        return View(account);
                    }
                }
            }
            #region 判斷推薦人跟被推薦人是正確的
            accountJoinGroup = db.AccountJoinGroup.Where(p => p.Old_Account == OldAccount && p.New_Account == account.Email).FirstOrDefault();
            if (accountJoinGroup == null)
            {
                ViewBag.Register_Status = "Error";
                ViewBag.ReturnUrl = "../Home/Index";
                return View(account);
            }
            #endregion
            int rand;
            char pd;
            string newlinks = String.Empty;
            //生成重設密碼用的連結路徑
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
            ViewBag.ValidateCode = Request.Cookies["ValidateCode"].Value;
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            Response.Cookies["em"].Value = AesEnc.AESenprypt(newEmail);
            Response.Cookies["ex"].Value = AesEnc.AESenprypt(System.DateTime.Now.AddMinutes(129600).ToString("yyyy-MM-dd HH:mm:ss"));
            Response.Cookies["NewLinks"].Value = newlinks;
            Response.Cookies["Password"].Value = AesEnc.AESenprypt(account.PWD);
            //會員註冊時間
            account.Registeron = DateTime.Now;
            account.CreateDate = DateTime.Now;
            account.Loginon = DateTime.Now;
            account.NewLinks = newlinks;
            account.LoginStatus = 1;
            account.RememberMe = 0;
            account.ActionCode = "C";
            account.Istosap = 0;
            // 判斷需要新增還是修改
            if (objAccount == null)
            {
                numNewAccountId = oAccountService.CreateAccount(account);
            }
            else
            {
                numNewAccountId = objAccount.ID;
                account.ID = numNewAccountId;
                oAccountService.UpdateAccount(account, true);
            }
            memberlist.AccID = numNewAccountId;
            memberlist.Mobile = account.Mobile;
            string[] resultString = Regex.Split(account.Name, account.Nickname, RegexOptions.IgnoreCase);
            memberlist.Firstname = account.Nickname;
            memberlist.Lastname = resultString[0];
            memberlist.Birthday = account.Birthday;
            memberlist.CreateDate = DateTime.Now;
            memberlist.Sex = account.Sex;


            accountJoinGroup.RegisterSuccess = true;
            accountJoinGroup.Status = 1;
            db.Member.Add(memberlist);
            db.SaveChanges();
            #region 發放折價券
            /* ------ 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
            //DateTime objDateNow = DateTime.Now;
            //if (DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/9 14:00:00")) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime("2014/6/23 00:00:00")) <= 0)
            //Convert.ToDateTime("2014/6/9 14:00:00")-->改成configure
            //int numEventId = 20;-->改成configure
            //int numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]);
            //var ActivityDate = db.Event.Where(p => p.id == numEventId).FirstOrDefault();

            //bool ActivityIngOrNot = EvenStatus(numEventId);
            //利用EvenId獲得開始與結束時間
            //if (ActivityIngOrNot == true)
            //{
            //    if (DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.datestart)) >= 0 && DateTime.Compare(objDateNow, Convert.ToDateTime(ActivityDate.dateend)) <= 0)
            //    {
            //        TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository objCouponService = null;
            //        numEventId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[environment + "_CouponID"]); ;//鎖定活動使用20
            //        objCouponService = new Redeem.Service.CouponService.CouponServiceRepository();

            //        objCouponService.addDynamicCouponByEventIdAndUserAccount(numEventId, account.ID.ToString());

            //        objCouponService = null;
            //    }
            //}

            /* ------ end of 寄發活動Coupon 活動期間為2014/6/9 14:00 至 2014/6/23 00:00 ------ */
            #endregion
            Mail_Message(account, "RegisterMessage"); // 註冊成功通知信
            //FormsAuthentication.SetAuthCookie(model.account_email, false);
            Account searchAccount = new Account();
            searchAccount = (from p in db.Account where p.Email == newEmail select p).FirstOrDefault();
            string mainDomain = string.Empty;

            if (environment == "DEV")
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
            }
            else
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
            }
            Response.Cookies["Accountid"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["Accountid"].Domain = mainDomain;
            Response.Cookies["LoginStatus"].Value = AesEnc.AESenprypt(Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss"));
            Response.Cookies["LoginStatus"].Domain = mainDomain;
            Response.Cookies["IE"].Value = "i" + AesEnc.AESenprypt(Convert.ToString(searchAccount.ID) + "_" + Convert.ToString(searchAccount.LoginStatus) + "_" + Convert.ToString(searchAccount.CreateDate) + "_" + DateTime.Now.ToString("yyyymmss")) + "e";
            Response.Cookies["IE"].Domain = mainDomain;

            ICarts repository = new CartsRepository();
            repository.SetTrackAll(searchAccount.ID, searchAccount.CreateDate.ToString()); //Set accid 
            repository.CheckTrackCreateDate(); //Del this accid's track item where didn't update in 30 days.

            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    string shippingCart = Request.Cookies["cart"].Value; //Get cookie's value
                    shippingCart = HttpUtility.UrlDecode(shippingCart); //URIDecode
                    List<CookieCart> cookieCarts = new List<CookieCart>();
                    cookieCarts = findShippingCart(shippingCart); //Trans it to model

                    bool checkadd = new bool();
                    checkadd = true;
                    foreach (var cookieCart in cookieCarts) //Add all cookie's item in to DB
                    {
                        if (repository.AddTrack(cookieCart.itemID, cookieCart.itemlistID, 0) == OutputMessage.addSuccess)
                        {
                            //checkadd = true;
                            //add cart success;
                        }
                        else
                        {
                            checkadd = false;
                            //add cart fail
                        }
                    }
                    if (checkadd)
                    {
                        Response.Cookies["cart"].Value = "";
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                    else
                    {
                        Response.Cookies["cart"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cart"].Domain = mainDomain;
                        Response.Cookies["cartNumberDetail"].Value = ""; //Here have to leave item that can't be add to DB need change in the future.
                        Response.Cookies["cartNumberDetail"].Domain = mainDomain;
                    }
                }
                else { }

            }
            catch (Exception e)
            {
            }

            return RedirectToAction("AutoLogin", "Account");
        }

    }
}
