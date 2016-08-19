using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace TWNewEgg.ECWeb.Controllers
{
    //[AllowNonSecures]
    [AllowAnonymous]
    public class VendorController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Marketplace()
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;

            listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;
            if (listTreeItem != null && listTreeItem.Count > 0)
                ViewBag.Categories = listTreeItem[0].Nodes;

            return View();
        }

        [HttpPost]
        public JsonResult SendMarketMail(TWNewEgg.Models.ViewModels.Vendor.Marketplace_View info)
        {
            var result = new { IsSuccess = false, Msg = "" };
            string mailBody = "", imgUrl = "", templatePath = "";
            try
            {                
                //讀取 MailTemplate ("/Views/MailTemplate/Mail_ConnectPartner.cshtml")
                templatePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings.Get("ECWeb_PartnerNoticeMail_TemplateUrl"));
                using (StreamReader sr = new StreamReader(templatePath))
                {
                    mailBody = sr.ReadToEnd();
                }

                //公司名稱
                mailBody = mailBody.Replace("{$CompanyName}", info.CompanyName);
                //統一編號
                mailBody = mailBody.Replace("{$CompanyUnifiedNumber}", info.CompanyUnifiedNumber);
                //公司電話
                mailBody = mailBody.Replace("{$CompanyPhone}", info.CompanyPhone);
                //公司網址
                mailBody = mailBody.Replace("{$CompanyOfficialSiteUrl}", info.CompanyOfficialSiteUrl);
                //聯絡人
                mailBody = mailBody.Replace("{$CompanyContact}", info.CompanyContact);
                //聯絡電話
                mailBody = mailBody.Replace("{$CompanyContactPhone}", info.CompanyContactPhone);
                //聯絡EMAIL
                mailBody = mailBody.Replace("{$CompanyContactEmail}", info.CompanyContactEmail);
                //希望上架類別
                mailBody = mailBody.Replace("{$Category}", info.Category);
                //其他說明
                if (!String.IsNullOrEmpty(info.Remark))
                {
                    mailBody = mailBody.Replace("{$Remark}", info.Remark.Replace("\n", "<br />"));
                }
                else
                {
                    mailBody = mailBody.Replace("{$Remark}", "");
                }

                //圖檔位罝
                imgUrl = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"].ToString();
                imgUrl += (imgUrl.EndsWith("/") ? "" : "/");

                mailBody = mailBody.Replace("{$imgUrl}", imgUrl);

                //發信通知
                MailMessage oMail = new MailMessage();
                SmtpClient oSmtp = null;
                try
                {                    
                    oMail.Subject = WebSiteData.SiteName + "招商 - 【" + info.Category + "】 " + info.CompanyName;
                    oMail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings.Get("ECWeb_MKPServiceAccount"));
                    oMail.To.Add(System.Configuration.ConfigurationManager.AppSettings.Get("ECWeb_PartnerNoticeMail"));
                    
                    oMail.IsBodyHtml = true;
                    oMail.Body = mailBody;

                    //AppSettings.Config
                    string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"].ToUpper();
                    string SmtpServer = System.Configuration.ConfigurationManager.AppSettings[environment + "_SmtpServer"];

                    oSmtp = new SmtpClient(SmtpServer);
                    oSmtp.Port = 25;
                    oSmtp.Send(oMail);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (oMail != null)
                        oMail.Dispose();
                    if (oSmtp != null)
                        oSmtp.Dispose();
                }

                result = new { IsSuccess = true, Msg = "信件寄送成功！" };
            }
            catch (Exception ex)
            {
                result = new
                {
                    IsSuccess = false,
                    Msg = (ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
