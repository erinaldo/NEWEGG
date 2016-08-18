using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Data;

using System.Net;
using System.Web;

using log4net;
using log4net.Config;
using System.Threading;
using TWNewEgg.API.Models;
using System.Web.Script.Serialization;
using TWNewEgg.DB.TWBACKENDDB.Models.ExtModels;
using System.IO;

//2014.5.27 寫入台蛋前台 add by ice

namespace TWNewEgg.API.Service
{
    public class TWService
    {
        private DB.TWSqlDBContext db = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext Spdb = new DB.TWSellerPortalDBContext();
        private static ILog log = LogManager.GetLogger(typeof(TWService));  //2014.7.8 log4net add by ice
        string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];

        public DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo UpdateSPManufacture(string ManufactureURL)
        {
            DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            //dt.GetDateTimeFormats('r')[0].ToString();
            //Convert.ToDateTime(dt);

            try
            {
                Info = Spdb.Seller_ManufactureInfo.Where(x => x.ManufactureURL == ManufactureURL).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo>();
                Info.SN = db.Manufacture.Where(x => x.WebAddress == ManufactureURL).Select(x => x.ID).Single();
                Info.UpdateDate = dt;
                Info.UpdateUserID = 0;

                return Info;
            }
            catch (Exception e)
            {
                log.Error(ManufactureURL + e.Message);

                return Info;
            }
        }

        public Models.ActionResponse<string> UpdateTWManufacture(string ManufactureURL)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();

            try
            {
                DB.TWSQLDB.Models.Manufacture Info = new DB.TWSQLDB.Models.Manufacture();
                DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo ID = new DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo();

                Info = TWManufacture(ManufactureURL);                                       //對應TWSQLDB.ManufactureInfo and TWSELLERPORTALDB.Seller_ManufactureInfoInfo的資料

                if (db.Manufacture.Where(x => x.WebAddress == ManufactureURL).Any())
                {
                    db.Entry(Info).State = EntityState.Modified;
                    massage.Body = "修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_BasicInfo table data success!";
                }
                else
                {
                    db.Manufacture.Add(Info);
                    massage.Body = "新增成功，已儲存!";
                    massage.Msg = "Save changed Manufacture table data success!";
                }
                db.SaveChanges();

                //新增才需要更新seller portal的sellerID
                //if (massage.Body == "新增成功，已儲存!")
                //{
                ID = UpdateSPManufacture(ManufactureURL);                    //取得TWSQLDB.Manufacture.ID寫回TWSELLERPORTALDB.Seller_BasicInfo
                Spdb.Entry(ID).State = EntityState.Modified;
                Spdb.SaveChanges();
                //}
                massage.IsSuccess = true;
                massage.Code = 0;

            }
            catch (Exception e)
            {
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Manufacture table data false!";

                log.Error(ManufactureURL + e.Message);
            }

            return massage;
        }

        public DB.TWSQLDB.Models.Manufacture TWManufacture(string ManufactureURL)
        {
            DB.TWSQLDB.Models.Manufacture Info = new DB.TWSQLDB.Models.Manufacture();
            DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo SPInfo = new DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            try
            {
                Info = db.Manufacture.Where(x => x.WebAddress == ManufactureURL).SingleOrDefault();
                SPInfo = Spdb.Seller_ManufactureInfo.Where(x => x.ManufactureURL == ManufactureURL).Single();

                if (Info == null)
                {
                    Info = new DB.TWSQLDB.Models.Manufacture();
                    Info.CreateUser = "SellerPortal";
                    Info.CreateDate = dt;
                    Info.WebAddress = SPInfo.ManufactureURL;   //製造商網址
                }

                Info.Name = SPInfo.ManufactureName;    //製造商名稱

                //2014.7.9 判斷是否為null再給值 add by ice (SellerPortal 分機不一定有資料)
                //if ((SPInfo.PhoneRegion != null) ||
                //    (SPInfo.Phone != null) ||
                //    (SPInfo.PhoneExt != null))
                //{
                //    Info.Phone = SPInfo.PhoneRegion.Trim() + "-" + SPInfo.Phone.Trim() + "#" + SPInfo.PhoneExt.Trim(); //製造商電話
                //}

                // 2014.08.21 修改增加判斷是否為空值 by Jack
                if ((SPInfo.PhoneRegion != null) ||
                    (SPInfo.Phone != null) ||
                    (SPInfo.PhoneExt != null))
                {
                    Info.Phone = (string.IsNullOrWhiteSpace(SPInfo.PhoneRegion) ? " " : SPInfo.PhoneRegion.Trim() + "-")
                        + (string.IsNullOrWhiteSpace(SPInfo.Phone) ? " " : SPInfo.Phone.Trim())
                        + (string.IsNullOrWhiteSpace(SPInfo.PhoneExt) ? " " : "#" + SPInfo.PhoneExt.Trim()); //製造商電話
                }

                Info.Showorder = 90000;
                Info.Status = 0;
                Info.Address = null;
                Info.SourceContry = null;
                Info.Updated = 0;
                Info.Updatedate = dt;
                Info.UpdateUser = "SellerPortal";

                return Info;
            }
            catch (Exception e)
            {
                log.Error(ManufactureURL + e.Message);

                return Info;
            }
        }

        //呼叫台蛋IPP總價API
        public string PriceAPI(int ItemID)
        {
            string result = string.Empty;
            if (HttpContext.Current.Request.Url.Authority != "localhost:57035")
            {
                TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
                string IP = conn.GetAPIWebConfigSetting("IPPIP");
                string url = IP + "/api/ItemInfo/Get?q=thisistestfunction&delvT=&itemID=" + ItemID;
                try
                {
                    //2014.7.8 增加是否有呼叫wmsAPI add by ice
                    log.Info("IPP IP: " + url);
                    result = conn.Get<string>(url, "", "", 5000);
                    if (!string.IsNullOrEmpty(result))
                    {
                        log.Error("After IPP IP msg: " + result);
                        this.sendAdminMail(ItemID, "Update Price Faild ItemID:" + ItemID.ToString());
                    }
                    else
                    {
                        log.Info("After IPP IP msg: Update Price Success");
                    }
                }
                catch (Exception ex)
                {
                    //exception 不一定有 InnerException，導致 Error
                    //result = ex.InnerException.ToString();  //2014.7.8 add try catch by ice
                    result = ex.Message.ToString();

                    // 無法辨認錯誤地方
                    //2014.7.8 增加是否有呼叫wmsAPI add by ice
                    //log.Error("IPP IP: " + url + "; msg:" + result);
                    log.Error("Update IPP Price Exception: " + url + "; msg: " + result);
                }
            }
            return result;
        }

        /// <summary>
        /// 傳入 List ItemID 
        /// </summary>
        /// <param name="updateItemIDs"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<int>> PriceAPI(List<int> updateItemIDs)
        {
            // 宣告儲存 IPP 更新結果
            TWNewEgg.API.Models.ActionResponse<List<int>> result = new Models.ActionResponse<List<int>>();
            List<int> list_priceresult = new List<int>();

            // 取得 PriceAPI URL
            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            string IP = conn.GetAPIWebConfigSetting("IPPIP");
            string priceurl = IP + "/api/ItemInfo/Get?q=thisistestfunction&delvT=&itemID=";

            // 非開發環境會進行 Price 更新呼叫
            if (HttpContext.Current.Request.Url.Authority != "localhost:57035")
            {
                try
                {
                    foreach (var itemid in updateItemIDs)
                    {
                        string priceresult = string.Empty;
                        //2014.7.8 增加是否有呼叫wmsAPI add by ice
                        log.Info("IPP IP: " + priceurl + itemid);
                        priceresult = conn.Get<string>(priceurl + itemid, "", "", 5000);

                        if (!string.IsNullOrEmpty(priceresult))
                        {
                            log.Error("After IPP IP msg: " + priceresult + itemid);
                            list_priceresult.Add(itemid);
                        }
                        else
                        {
                            log.Info("After IPP IP msg: Update Price Success");
                        }
                    }

                    // 判斷放入失敗的 dic 是否有內容，若無內容代表 Update price 成功
                    if (list_priceresult.Count() == 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        //將有更新失敗的寄信給管理人
                        sendAdminMail(list_priceresult, "Update IPP Price Faild, ItemID: ");

                        result.IsSuccess = false;
                    }

                    result.Body = list_priceresult;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Error("Update IPP Price Exception: " + priceurl + "; msg: " + ex.Message);
                }

            }
            else
            {
                // 在開發環境，直接回傳 true
                result.IsSuccess = true;
                result.Body = list_priceresult;
            }

            return result;
        }

        /// <summary>
        /// 總價化(多筆)更新失敗，寄送給管理人
        /// </summary>
        /// <param name="updateFaildID">更新失敗的ItemID</param>
        public void sendAdminMail(List<int> updateFaildID, string ErrorInfo)
        {
            API.Models.Connector connector = new Models.Connector();
            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            Models.Mail mailContent = new Models.Mail();
            string idList = string.Empty;
            foreach (var id in updateFaildID)
            {
                idList += id.ToString() + ", ";
            }

            mailContent.MailMessage = ErrorInfo + idList;
            string[] adminMaul_Users = adminEmail.Split(',');

            foreach (var mail in adminMaul_Users)
            {
                mailContent.UserEmail = mail;
                mailContent.MailType = Models.Mail.MailTypeEnum.ErrorInfo;
                Thread.Sleep(1000);
                connector.SendMail(null, null, mailContent);
            }
        }

        /// <summary>
        /// 總價化(單筆)更新失敗，寄送給管理人
        /// </summary>
        /// <param name="updateFaildID">更新失敗的ItemID</param>
        public void sendAdminMail(int updateFaildID, string ErrorInfo)
        {
            API.Models.Connector connector = new Models.Connector();
            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            Models.Mail mailContent = new Models.Mail();

            mailContent.MailMessage = ErrorInfo;

            //mailContent.MailMessage = "Update Price Faild ItemID: " + updateFaildID;
            string[] errorInfo = adminEmail.Split(',');

            foreach (var mail in errorInfo)
            {
                mailContent.UserEmail = mail;
                mailContent.MailType = Models.Mail.MailTypeEnum.ErrorInfo;
                Thread.Sleep(1000);
                connector.SendMail(null, null, mailContent);
            }
        }

        //呼叫台蛋IPP tracking number API
        public TWNewEgg.API.Models.ActionResponse<bool> TrackingNumAPI(string cartID, List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> delvTrack, int delvStatus, string updateUser)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();

            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            TWNewEgg.API.Models.UpdateTWTrackingNum tracking = new TWNewEgg.API.Models.UpdateTWTrackingNum();

            tracking.SalesOrderCode = cartID;
            tracking.TrackNO = delvTrack.Select(r => r.TrackingNum).FirstOrDefault().ToString();
            tracking.ForwarderID = delvTrack.Select(r => r.DeliverID).FirstOrDefault();
            tracking.UpdateNote = DateTime.Now + " " + Enum.GetName(typeof(Models.OrderInfo.EnumDelvStatus), delvStatus).ToString() + ".SellerPortal(" + updateUser + ")";

            string IP = conn.GetAPIWebConfigSetting("IPPIP");
            string url = IP + "/wms/UpdateDeliveryInfo";
            try
            {
                //2014.7.8 增加是否有呼叫wmsAPI add by ice
                log.Info("IPP wms IP: " + url + "; CartID:" + cartID + "; Track#:" + tracking.TrackNO + "; ForwarderID:" + tracking.ForwarderID);
                result = conn.Post<TWNewEgg.API.Models.ActionResponse<bool>>(url, "", "", tracking, 10000);
                if (!string.IsNullOrEmpty(result.Msg))
                    log.Info("After IPP wms IP msg: " + result.Msg);
            }
            catch (Exception ex)
            {
                // 2014.08.22  InnerException 不一定有訊息，導致錯誤發生 by jack
                //result.Msg = ex.InnerException.ToString();  //2014.7.8 add try catch by ice
                result.Msg = ex.Message.ToString();
                //2014.7.8 增加是否有呼叫wmsAPI add by ice
                log.Error("IPP wms IP: " + url + "; msg:" + result.Msg);
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
            }

            return result;
        }

        //呼叫台蛋IPP 組product. SPEC xml欄位 API
        public TWNewEgg.API.Models.ActionResponse<string> PropertyXMLAPI(int ProductID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            if (HttpContext.Current.Request.Url.Authority != "localhost:57035")
            {
                TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();

                string IP = conn.GetAPIWebConfigSetting("IPPIP");
                string url = IP + "/Property/Updatespexlabel?productID=" + ProductID;

                try
                {
                    log.Info("Update URL: " + url + "; ProductID:" + ProductID);
                    result.Msg = conn.Get<string>(url, "", "", 10000);
                    if (!string.IsNullOrEmpty(result.Msg))
                    {
                        log.Error("After Update XML msg: " + result.Msg);
                        result.IsSuccess = false;
                        result.Code = (int)ResponseCode.Error;
                        sendAdminMail(ProductID, "Update Price Faild ProductID:" + ProductID.ToString());
                    }
                    else
                    {
                        log.Info("After Update XML msg: XML Update Success");
                        result.IsSuccess = true;
                        result.Code = (int)ResponseCode.Success;
                    }
                }
                catch (Exception ex)
                {
                    result.Msg = ex.Message.ToString();
                    log.Error("IPP IP: " + url + "; msg:" + result.Msg);
                    result.IsSuccess = false;
                    result.Code = (int)ResponseCode.Error;
                }
            }

            return result;
        }

        /// <summary>
        /// 進行多筆 XML 更新
        /// </summary>
        /// <param name="updateIPPItemID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<int>> PropertyXMLAPI(List<string> updateIPPItemID)
        {
            // 宣告儲存 IPP 更新結果
            TWNewEgg.API.Models.ActionResponse<List<int>> result = new Models.ActionResponse<List<int>>();
            List<int> list_UpdateXMLResult = new List<int>();

            // 取得 PriceAPI URL
            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            string IP = conn.GetAPIWebConfigSetting("IPPIP");
            string xmlUpdateurl = IP + "/Property/Updatespexlabel?productID=";

            // 非開發環境會進行 Price 更新呼叫
            if (HttpContext.Current.Request.Url.Authority != "localhost:57035")
            {
                try
                {
                    foreach (var updateproductID in updateIPPItemID)
                    {
                        string productXMLUpdateResult = string.Empty;

                        log.Info("IPP IP: " + xmlUpdateurl + updateproductID);
                        productXMLUpdateResult = conn.Get<string>(xmlUpdateurl + updateproductID, "", "", 10000);

                        if (!string.IsNullOrEmpty(productXMLUpdateResult))
                        {
                            log.Error("After Update XML msg: " + productXMLUpdateResult + updateproductID);

                            list_UpdateXMLResult.Add(Convert.ToInt32(updateproductID));
                        }
                        else
                        {
                            log.Info("After Update XML msg: XML Update Success");
                        }
                    }

                    // 判斷放入失敗的 dic 是否有內容，若無內容代表 Update price 成功
                    if (list_UpdateXMLResult.Count() == 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;

                        //將有更新失敗的寄信給管理人
                        sendAdminMail(list_UpdateXMLResult, "Update IPP Price Faild, ItemID: ");
                    }

                    result.Body = list_UpdateXMLResult;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Error("Update Product XML Exception: " + xmlUpdateurl + "; msg: " + ex.Message);
                }

            }
            else
            {
                // 在開發環境，直接回傳 true
                result.IsSuccess = true;
                result.Body = list_UpdateXMLResult;
            }

            return result;
        }

        //取得前台國家代碼
        private int getCountryID(string c_code)
        {
            return db.Country.Where(c => c.ShortName == c_code).Select(c => c.ID).Single();
        }


        public ActionResponse<string> UpdateTWSeller(string SellerEmail, string accountType)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();

            try
            {
                DB.TWSQLDB.Models.Seller Info = new DB.TWSQLDB.Models.Seller();
                DB.TWSELLERPORTALDB.Models.Seller_BasicInfo SellerID = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
                DB.TWSELLERPORTALDB.Models.Seller_User USellerID = new DB.TWSELLERPORTALDB.Models.Seller_User();

                Info = TWSeller(SellerEmail, accountType); //對應TWSQLDB.Seller and TWSELLERPORTALDB.Seller_BasicInfo的資料

                if (db.Seller.Where(x => x.Email == SellerEmail && x.AccountType == accountType).Any())
                {
                    db.Entry(Info).State = EntityState.Modified;
                    massage.Body = "修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_BasicInfo table data success!";
                }
                else
                {
                    db.Seller.Add(Info);
                    massage.Body = "新增成功，已儲存!";
                    massage.Msg = "Save changed Seller_BasicInfo table data success!";
                }

                db.SaveChanges();

                // 取得TWSQLDB.Seller.ID 寫回 TWSELLERPORTALDB.Seller_BasicInfo
                // 更新Seller_BasicInfo.SellerID
                SellerID = UpdateSPSellerID(SellerEmail, accountType);
                Spdb.Entry(SellerID).State = EntityState.Modified;

                // 新增時才需要更新 Seller_User 的 SellerID
                if (massage.Body == "新增成功，已儲存!")
                {
                    // 更新Seller_User.SellerID
                    USellerID = UpdateSPUserSellerID(SellerEmail, accountType);
                    Spdb.Entry(USellerID).State = EntityState.Modified;
                }

                Spdb.SaveChanges();

                massage.Body = SellerID.SellerID.ToString();
                massage.IsSuccess = true;
                massage.Code = 0;
            }
            catch (Exception e)
            {
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Seller_BasicInfo table data false!";
                log.Error(SellerEmail + e.Message + e.StackTrace);
            }

            return massage;
        }

        private Seller_User UpdateSPUserSellerID(string SellerEmail, string accountType)
        {
            DB.TWSELLERPORTALDB.Models.Seller_User Info = new DB.TWSELLERPORTALDB.Models.Seller_User();
            DateTime dt = DateTime.UtcNow.AddHours(8);

            try
            {
                Info = Spdb.Seller_User.Where(x => x.UserEmail == SellerEmail).SingleOrDefault();

                // 判斷若是 SellerID = -1，就不更新 SellerID(GroupID = 7)
                if (Info.SellerID != -1)
                {
                    Info.SellerID = db.Seller.Where(x => x.Email == SellerEmail && x.AccountType == accountType).Select(x => x.ID).Single();
                    Info.UpdateDate = dt;
                    Info.UpdateUserID = 0;
                }

                return Info;
            }
            catch (Exception e)
            {
                log.Error(SellerEmail + e.Message);
                return Info;
            }
        }

        private Seller_BasicInfo UpdateSPSellerID(string SellerEmail, string accountType)
        {
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            //dt.GetDateTimeFormats('r')[0].ToString();
            //Convert.ToDateTime(dt);

            try
            {
                Info = Spdb.Seller_BasicInfo.Where(x => x.SellerEmail == SellerEmail && x.AccountTypeCode == accountType).FirstOrDefault();
                Info.SellerID = db.Seller.Where(x => x.Email == SellerEmail && x.AccountType == accountType).Select(x => x.ID).FirstOrDefault();
                Info.UpdateDate = dt;
                Info.UpdateUserID = 0;

                return Info;
            }
            catch (Exception e)
            {
                log.Error(SellerEmail + e.Message);
                return Info;
            }
        }

        private DB.TWSQLDB.Models.Seller TWSeller(string SellerEmail, string accountType)
        {
            DB.TWSQLDB.Models.Seller Info = new DB.TWSQLDB.Models.Seller();
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo SPInfo = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            //dt.GetDateTimeFormats('r')[0].ToString();
            //Convert.ToDateTime(dt);
            TWNewEgg.DB.TWSqlDBContext dbcontent = new DB.TWSqlDBContext();

            try
            {

                Info = dbcontent.Seller.Where(x => x.Email == SellerEmail && x.AccountType == accountType).FirstOrDefault();                
                SPInfo = Spdb.Seller_BasicInfo.Where(x => x.SellerEmail == SellerEmail && x.AccountTypeCode == accountType).FirstOrDefault();

                if (Info == null)
                {
                    Info = new DB.TWSQLDB.Models.Seller();
                }
                //電話
                //2014.7.9 判斷是否為null再給值 add by ice 
                //if ((SPInfo.PhoneRegion != null) ||
                //    (SPInfo.Phone != null) ||
                //    (SPInfo.PhoneExt != null))
                //{
                //    Info.TELF1 = SPInfo.PhoneRegion.Trim() + "-" + SPInfo.Phone.Trim() + "#" + SPInfo.PhoneExt.Trim(); //製造商電話
                //}

                // 2014.08.21 修改增加判斷是否為空值 by Jack 
                if ((SPInfo.PhoneRegion != null) ||
                    (SPInfo.Phone != null) ||
                    (SPInfo.PhoneExt != null))
                {
                    string tmpTelf1 = (string.IsNullOrWhiteSpace(SPInfo.PhoneRegion) ? " " : SPInfo.PhoneRegion.Trim() + "-")
                        + (string.IsNullOrWhiteSpace(SPInfo.Phone) ? " " : SPInfo.Phone.Trim());

                    if (tmpTelf1.Length > 15)
                    {
                        Info.TELF1 = tmpTelf1.Substring(0, 15);
                    }
                    else
                    {
                        Info.TELF1 = tmpTelf1;
                    }
                }


                Info.Name = SPInfo.SellerName;//名稱
                Info.Description = SPInfo.SellerName;
                Info.CurrencyType = SPInfo.Currency;//幣別
                Info.Email = SPInfo.SellerEmail;//email
                Info.Address = SPInfo.SellerAddress;//地址
                Info.City = SPInfo.City;//城市
                Info.AccountType = SPInfo.AccountTypeCode; //身分別
                Info.PostCode = string.IsNullOrWhiteSpace(SPInfo.Zipcode) ? "00000" : SPInfo.Zipcode; //郵遞區號
                //Info.CompanyCode = SPInfo.CompanyCode;//統一編號 
                Info.CompanyCode = "3101"; // 2014.09.03 此欄位固定填入 3101 by ST 
                Info.CountryID = getCountryID(SPInfo.SellerCountryCode);//國家代碼
                Info.VAT_NO = SPInfo.CompanyCode; // Seller 公司統編 2014.09.23 by ST
                Info.FirstName = SPInfo.FirstName;//名
                Info.LastName = SPInfo.LastName;//姓

                //Info.Zipcode = BasicProInfo.Zipcode;
                //Info.Region = SPInfo.SellerState;//州 2014.09.03 ST 說此欄位不用填入，會壞掉

                Info.ACCT_GROUP = "W100";
                Info.PUR_ORG = "3101";
                Info.CreateUser = "SellerPortal";
                Info.CreateDate = dt;
                Info.UpdateDate = dt;
                Info.UpdateUser = "SellerPortal";

                Info.Sortl = SPInfo.SellerShortName;
                //Info.ComCity = BasicProInfo.ComCity;
                Info.ComAdd = SPInfo.ComSellerAdd;//營業地址
                Info.State = SPInfo.ComSellerState;//營業地址-州
                Info.Status = SPInfo.SellerStatus;//商家狀態
                Info.Istosap = 0;//結帳狀態，0：Seller update from SellerPortal；1：Seller update to sap

                //付款方式
                Info.BillingCycle = SPInfo.BillingCycle;
                //廠商身分別
                Info.Identy = SPInfo.Identy;

                return Info;
            }
            catch (Exception e)
            {
                log.Error(SellerEmail + e.Message + e.StackTrace);
                return Info;
            }
        }

        /// <summary>
        /// 呼叫後台，押入發票
        /// </summary>
        /// <param name="trackingResult"></param>
        /// <param name="code"></param>
        public ActionResponse<List<string>> UpdateInvoiceAPI(bool trackingResult, string code)
        {
            ActionResponse<List<string>> InvoiceUpdateResult = new ActionResponse<List<string>>();

            // 取得 發票押入 URL
            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            string IP = conn.GetAPIWebConfigSetting("IPPIP");
            string invoiceUpdateurl = IP + "/Cart/UpdateInvoice";
            List<string> codes = new List<string>();
            codes.Add(code);

            // 非開發環境會進行 Price 更新呼叫
            if (HttpContext.Current.Request.Url.Authority != "localhost:57035")
            {
                try
                {
                    if (trackingResult)
                    {
                        log.Info("Before InvoiceAPI, TrackingResult: " + trackingResult + ", Code: " + code);
                        InvoiceUpdateResult = conn.Post<ActionResponse<List<string>>>(invoiceUpdateurl, "", "", new { CartLisID = codes });
                        log.Info("After InvoiceAPI, Result: " + InvoiceUpdateResult.Msg);
                        if (InvoiceUpdateResult.IsSuccess == true)
                        {
                            log.Info("發票押入成功! ID: " + code);
                        }
                        else
                        {

                            TWNewEgg.API.Models.Mail mail = new Mail();

                            mail.MailType = Mail.MailTypeEnum.ErrorInfo;
                            mail.MailMessage = "後台發票押入功能: " + InvoiceUpdateResult.Msg;
                            mail.UserEmail = "Penny.P.Lee@newegg.com";
                            mail.RecipientBcc = adminEmail;
                            conn.SendMail("", "", mail);

                            log.Error("押入訊息: " + InvoiceUpdateResult.Msg);
                        }
                    }
                    else
                    {
                        InvoiceUpdateResult.IsSuccess = false;
                        InvoiceUpdateResult.Msg = "TrackingNum押入失敗 ID: " + code;
                        InvoiceUpdateResult.Code = (int)ResponseCode.Error;
                        InvoiceUpdateResult.Body = null;

                        log.Error("TrackingNum押入失敗 ID: " + code);
                    }
                }
                catch (WebException webex)
                {
                    string webEx = new StreamReader(webex.Response.GetResponseStream()).ReadToEnd().ToString();

                    log.Error(webEx);

                    TWNewEgg.API.Models.Mail mail = new Mail();

                    mail.MailType = Mail.MailTypeEnum.ErrorInfo;
                    mail.MailMessage = "Update Invoice API Exception: " + webex.Response + "; msg: " + webex.Message;
                    mail.UserEmail = "Penny.P.Lee@newegg.com";
                    mail.RecipientBcc = adminEmail;
                    conn.SendMail("", "", mail);

                    //log.Error("Update Invoice API Exception: " + ex.StackTrace + "; msg: " + ex.Message);

                }
            }

            return InvoiceUpdateResult;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> UpdateRetGoods(UpdateRetGoodsInfo updateRetGoodsInfo)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();

            TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
            string IP = conn.GetAPIWebConfigSetting("IPPIP");
            string url = IP + "/return/UpdateRetGoods";
            try
            {
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                // 序列化
                string szJson = jsonSerializer.Serialize(updateRetGoodsInfo);
                log.Info("IPP url: " + url + "; UpdateRetGoodsInfo : " + szJson);
                result = conn.Post<TWNewEgg.API.Models.ActionResponse<bool>>(url, "", "", updateRetGoodsInfo);
                if (!string.IsNullOrEmpty(result.Msg))
                {
                    log.Info("IPP url : " + url + " return Message : " + result.Msg);
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message.ToString();
                log.Error("IPP url : " + url + " return Message : " + result.Msg);
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
            }

            return result;
        }
    }
}
