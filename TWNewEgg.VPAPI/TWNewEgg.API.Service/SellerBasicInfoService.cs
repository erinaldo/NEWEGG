using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using log4net;


namespace TWNewEgg.API.Service
{
    public class SellerBasicInfoService
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //string images = System.Configuration.ConfigurationManager.AppSettings["Images"];  //2014.5.30 mark by ice
        string images = System.Configuration.ConfigurationManager.AppSettings["LogoImage"]; //2014.5.30 seller logo獨立路徑，因為item list的商品圖片在正式區會改抓正式圖片機之圖片 add by ice

        ManageAccountService MA = new ManageAccountService(); 

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        #region Seller_BasicInfo table
        /// <summary>
        /// [Logic]
        /// Get table GetSeller_BasicInfo data by seller ID
        ///  0 - SellerID
        ///  1 - SellerName
        /// </summary>
        /// <param name="Seller">Search table Seller_BasicInfo keyword</param>
        /// <param name="type">Witch type search Seller_BasicInfo keyword</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> GetSeller_BasicInfo(string Seller, int type)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> BasicInfo = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            BasicInfo.Body = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();

            if ( type == 0 )
            {
                //用ID查詢
                int sellerid = 0;
                Int32.TryParse(Seller, out sellerid);
                BasicInfo.Body = db.Seller_BasicInfo.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            }
            else if( type == 1 )
            {
                //用名稱查詢
                BasicInfo.Body = db.Seller_BasicInfo.Where(x => x.SellerName == Seller).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            }


            if (BasicInfo.Body == null)
            {
                BasicInfo.Msg = "Table Seller_BasicInfo Can't find this seller ID!";
                BasicInfo.Code = 1;
                BasicInfo.IsSuccess = false;
            }
            else
            {
                BasicInfo.Msg = "Success";
                BasicInfo.Code = 0;
                BasicInfo.IsSuccess = true;
            }

            return BasicInfo;
        }

        /// <summary>
        /// Get All Sellers information , include Seller_BasicInfo and Seller_FinancialInfo
        /// </summary>
        /// <returns></returns>
        public Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> GetAllSeller_BasicInfoWithFinancial()
        {
            Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> info = new Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>>();
            try
            {
                info.Body = (from slr in db.Seller_BasicInfo
                             join sf in db.Seller_Financial on slr.SellerID equals sf.SellerID into tempFinan
                             from sf in tempFinan.DefaultIfEmpty()
                             select new TWNewEgg.API.Models.Seller_BasicInfoWithFinancial()
                             {
                                 SellerID = slr.SellerID,
                                 SellerEmail = slr.SellerEmail,
                                 SellerCountryCode = slr.SellerCountryCode,
                                 LanguageCode = slr.LanguageCode,
                                 FTP = slr.FTP,
                                 StoreMenu = slr.StoreMenu,
                                 StoreWebSite = slr.StoreWebSite,
                                 SellerName = slr.SellerName,
                                 AccountTypeCode = slr.AccountTypeCode,
                                 SellerStatus = slr.SellerStatus,
                                 SellerLogoURL = slr.SellerLogoURL,
                                 AboutInfo = slr.AboutInfo,
                                 FirstName = slr.FirstName,
                                 LastName = slr.LastName,
                                 PhoneRegion = slr.PhoneRegion,
                                 Phone = slr.Phone,
                                 PhoneExt = slr.PhoneExt,
                                 EmailAddress = slr.EmailAddress,
                                 SellerAddress = slr.SellerAddress,
                                 City = slr.City,
                                 SellerState = slr.SellerState,
                                 Zipcode = slr.Zipcode,
                                 CompanyCode = slr.CompanyCode,
                                 CountryCode = slr.CountryCode,
                                 ActiveatedDate = slr.ActiveatedDate,
                                 ActiveatedUserID = slr.ActiveatedUserID,
                                 SellerShortName = slr.SellerShortName,
                                 ComSellerAdd = slr.ComSellerAdd,
                                 ComCity = slr.ComCity,
                                 ComSellerState = slr.ComSellerState,
                                 ComZipcode = slr.ComZipcode,
                                 ComCountryCode = slr.ComCountryCode,
                                 Currency = slr.Currency,

                                 SWIFTCode = sf.SWIFTCode,
                                 BankName = sf.BankName,
                                 BankCode = sf.BankCode,
                                 BankBranchName = sf.BankBranchName,
                                 BankBranchCode = sf.BankBranchCode,
                                 BankAccountNumber = sf.BankAccountNumber,
                                 BankAddress = sf.BankAddress,
                                 BankCity = sf.BankCity,
                                 BankState = sf.BankState,
                                 BankCountryCode = sf.BankCountryCode,
                                 BankZipCode = sf.BankZipCode,
                                 BeneficiaryName = sf.BeneficiaryName,
                                 BeneficiaryAddress = sf.BeneficiaryAddress,
                                 BeneficiaryCity = sf.BeneficiaryCity,
                                 BeneficiaryState = sf.BeneficiaryState,
                                 BeneficiaryCountryCode = sf.BeneficiaryCountryCode,
                                 BeneficiaryZipcode = sf.BeneficiaryZipcode
                             }
                             ).ToList<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>();
                info.Code = 0;
                info.Msg = "OK";
                info.IsSuccess = true;
            }
            catch (Exception ex)
            {
                info.Body = null;
                info.Code = 1;
                info.Msg = ex.Message;
                info.IsSuccess = false;
            }
            return info;
        }

        public Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial> GetSeller_BasicInfoWithFinancialByID(string sellerID)
        {
            Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial> info = new Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>();

            int id = -1;
            int.TryParse(sellerID, out id);
            info.Body = GetAllSeller_BasicInfoWithFinancial().Body.Where(r => r.SellerID == id).FirstOrDefault<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>();
            return info;
        }

        public Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> GetSeller_BasicInfoWithFinancialByEmail(string sellerEmail)
        {
            Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> info = new Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>>();

            info.Body = GetAllSeller_BasicInfoWithFinancial().Body.Where(r => r.SellerEmail.ToLower() == sellerEmail.ToLower()).ToList<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>();
            return info;
        }


        /// <summary>
        /// [Logic]
        /// Save table Seller_BasicInfo
        /// </summary>
        /// <param name="BasicInfo">Table Seller_BasicInfo Data</param>
        public Models.ActionResponse<string> SaveSeller_BasicProInfo(API.Models.Seller_BasicProInfo BasicProInfo)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            BasicProInfo.UpdateDate = dt;
            TWNewEgg.DB.TWSellerPortalDBContext dbContent = new DB.TWSellerPortalDBContext();
            try
            {
                DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
                Info = Seller_BasicProInfo(BasicProInfo);
                db.Entry(Info).State = EntityState.Modified;
                db.SaveChanges();
                TWService twser = new TWService();
                massage = twser.UpdateTWSeller(Info.SellerEmail, Info.AccountTypeCode);
                int sellerid = Convert.ToInt32(dbContent.Seller_BasicInfo.Where(p => p.SellerEmail == BasicProInfo.EmailAddress).Select(p => p.SellerID).FirstOrDefault());
                

                massage.IsSuccess = true;
                massage.Code = 0;
                massage.Body = sellerid.ToString();
                massage.Msg = "Save changed Seller_BasicInfo table data success!";

                //2014.6.26 修改Seller_BasicInfo寫到前台seller的service add by ice begin
                
                //2014.6.26 修改Seller_BasicInfo寫到前台seller的service add by ice end
                
            }
            catch (Exception e)
            {
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "";
                //massage.Msg = "Save changed Seller_BasicInfo table data false!";
                massage.Msg = e.Message.ToString();
            }

            return massage;
        }

        /// <summary>
        /// [Logic]
        /// Save table Seller_BasicInfo
        /// </summary>
        /// <param name="BasicInfo">Table Seller_BasicInfo Data</param>
        public Models.ActionResponse<string> SaveSeller_BasicafterInfo(API.Models.Seller_BasicafterInfo BasicafterInfo)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            BasicafterInfo.UpdateDate = dt;
            try
            {
                DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
                string ImageStatus = CheckImage(BasicafterInfo.SellerLogoURL);

                switch(ImageStatus){
                    case "Success":
                        Info = Seller_BasicafterInfo(BasicafterInfo);
                        db.Entry(Info).State = EntityState.Modified;
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body = "修改成功，已儲存!";
                        massage.Msg = "Save changed Seller_BasicInfo table data success!";
                        break;
                    case "No upload picture!":
                        Info = Seller_BasicafterInfo(BasicafterInfo);
                        db.Entry(Info).State = EntityState.Modified;
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body = "修改成功，已儲存!";
                        massage.Msg = "Save changed Seller_BasicInfo table data success!";
                        break;
                    case "Image file isn't Exist!":
                        massage.IsSuccess = false;
                        massage.Code = 1;
                        massage.Body = "修改失敗，請檢查圖片檔案是否存在!";
                        massage.Msg = "Save changed Seller_BasicInfo table data false, please check Image file isn't Exist!";
                        break;
                    case "Exception Error":
                        massage.IsSuccess = false;
                        massage.Code = 1;
                        massage.Body = "修改失敗，請檢查錯誤訊息!";
                        massage.Msg = "Save changed Seller_BasicInfo table data false, please check txt of ErrorMassage!";
                        break;
                }
            }
            catch (Exception exp)
            {
                WriteErrorMassage(AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo", "SaveSeller_BasicafterInfo", exp);
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Seller_BasicInfo table data false!";
            }

            return massage;
        }

        /// <summary>
        /// [Logic]
        /// Mapping Seller_BasicProInfo to Seller_BasicInfo
        /// </summary>
        /// <param name="BasicProInfo">whitch Mapping Seller_BasicInfo</param>
        private DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Seller_BasicProInfo(API.Models.Seller_BasicProInfo BasicProInfo)
        {
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            try
            {
                Info = db.Seller_BasicInfo.Where(x => x.SellerID == BasicProInfo.SellerID).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();

                Info.Phone = BasicProInfo.Phone;
                Info.PhoneExt = BasicProInfo.PhoneExt;
                Info.PhoneRegion = BasicProInfo.PhoneRegion;
                Info.SellerName = BasicProInfo.SellerName;
                //Info.CSEmailAddress = BasicProInfo.CSEmailAddress;
                //Info.CSPhone = BasicProInfo.CSPhone;
                //Info.CSPhoneExt = BasicProInfo.CSPhoneExt;
                //Info.CSPhoneRegion = BasicProInfo.CSPhoneRegion;
                Info.EmailAddress = BasicProInfo.EmailAddress;
                Info.SellerAddress = BasicProInfo.SellerAddress;
                Info.City = BasicProInfo.City;

                Info.CompanyCode = BasicProInfo.CompanyCode;
                Info.CountryCode = BasicProInfo.CountryCode;

                Info.FirstName = BasicProInfo.FirstName;
                Info.LastName = BasicProInfo.LastName;
                Info.Zipcode = BasicProInfo.Zipcode;
                Info.SellerState = BasicProInfo.SellerState;

                //Info.InUserID = BasicProInfo.InUserID;
                //Info.InDate = BasicProInfo.InDate;
                Info.CreateDate = BasicProInfo.CreateDate;
                Info.ActiveatedDate = BasicProInfo.ActiveatedDate;
                Info.ActiveatedUserID = BasicProInfo.ActiveatedUserID;
                Info.UpdateDate = DateTime.Now;
                Info.UpdateUserID = BasicProInfo.UpdateUserID;

                Info.SellerShortName = BasicProInfo.SellerShortName;
                Info.ComCity = BasicProInfo.ComCity;
                Info.ComSellerAdd = BasicProInfo.ComSellerAdd;
                Info.ComSellerState = BasicProInfo.ComSellerState;
                Info.ComZipcode = BasicProInfo.ComZipcode;
                Info.ComCountryCode = BasicProInfo.ComCountryCode;

                return Info;
            }
            catch {
                return Info;
            }
        }

        /// <summary>
        /// [Logic]
        /// Mapping Seller_BasicafterInfo to Seller_BasicInfo
        /// </summary>
        /// <param name="BasicafterInfo">whitch Mapping Seller_BasicInfo</param>
        private DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Seller_BasicafterInfo(API.Models.Seller_BasicafterInfo BasicafterInfo)
        {
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Info = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();

            try
            {
                Info = db.Seller_BasicInfo.Where(x => x.SellerID == BasicafterInfo.SellerID).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
                Info.AboutInfo = BasicafterInfo.AboutInfo;
                Info.SellerLogoURL = BasicafterInfo.SellerLogoURL;
                //Info.InUserID = BasicafterInfo.InUserID;
                //Info.InDate = BasicafterInfo.InDate;
                //Info.CreateDate = BasicafterInfo.CreateDate;
                Info.ActiveatedDate = BasicafterInfo.ActiveatedDate;
                Info.ActiveatedUserID = BasicafterInfo.ActiveatedUserID;
                Info.UpdateDate = DateTime.Now;
                Info.UpdateUserID = BasicafterInfo.UpdateUserID;

                return Info;
            }
            catch
            {
                return Info;
            }
        }

        /// <summary>
        /// RegExr Data
        /// </summary>
        /// <param name="word">which is Regularized word</param>
        /// <param name="RegExr">Regularized word</param>
        private string RegExrData(string word, string RegExr) {
            Match match = Regex.Match(word, RegExr, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return "false";
        }

        /// <summary>
        /// Check image, make the one seller just have one image in seller logo file
        /// </summary>
        /// <param name="ImgURL">The official image URL(Just stay this image, others delete)</param>
        private string CheckImage(string ImgURL) {
            try
            {
                if (ImgURL == null) {
                    return "No upload picture!";
                }

                string filename = RegExrData(ImgURL, @"SellerPortal_([0-9]+)_([0-9]+)\.jpg$");
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo\\" + filename;
                string sellerID = RegExrData(filename, @"_([0-9]+)_");
                sellerID = sellerID.Substring(1,sellerID.Length-2);

                if (System.IO.File.Exists(filepath))
                {
                    //<string> All_JPG_file = Read_File_Type(@"d:/SellerPortalAPI\Pic\SellerLogo", "jpg");                              //2014.5.30 mark by ice
                    List<string> All_JPG_file = Read_File_Type(AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo", "jpg");       //2014.5.30 不寫死路徑，改抓實體檔案路徑 add by ice
                   
                    string search_sellerID_JPG = "SellerPortal_" + sellerID + "_";
                    foreach (string JPGFile in All_JPG_file)
                    {
                        if (JPGFile.IndexOf(filename) <= 0 && JPGFile.IndexOf(search_sellerID_JPG) > 0)
                        {
                            System.IO.File.Delete(JPGFile);
                        }
                    }
                    return "Success";
                }
                return "Image file isn't Exist!";
            }
            catch (Exception exp)
            {

                WriteErrorMassage(AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo\\", "CheckImage", exp);
                return "Exception Error";
            }
        }

        //Ron
        /// <summary>
        /// Check the SellerName is unique or not
        /// </summary>
        /// <param name="SellerID"></param>
        /// <param name="SellerName"></param>
        /// <returns></returns>
        public Models.ActionResponse<bool> CheckSellerNameUnique(string SellerID, string SellerName)
        {
            Models.ActionResponse<bool> IsUnique = new Models.ActionResponse<bool>();

            int id = -1;
            int.TryParse(SellerID, out id);
            IsUnique.Body = (db.Seller_BasicInfo.Count(r => r.SellerID != id && r.SellerName == SellerName) == 0);

            return IsUnique;
        }

        #endregion Seller_BasicInfo table

        /// <summary>
        /// Save seller logo image
        /// </summary>
        /// <param name="LogoImage">Saving seller's logo image need data</param>
        public Models.ActionResponse<string> SaveSellerLogoImage(API.Models.SellerLogoInfo LogoImage)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            string path = AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo";

            DateTime Date = DateTime.Now;
            string file_name = "SellerPortal_" + LogoImage.SellerID + "_" + Date.ToString("yyyyMMddhhmmss") + "." + LogoImage.FileType;

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            
            if( CreateLogoImage ( path , file_name, LogoImage.LogoImage ) ){
                massage.IsSuccess = true;
                massage.Body = images + "/pic/sellerlogo/" + file_name;
                massage.Msg = "Save changed Seller_BasicInfo table data success!";
            }else{
                massage.IsSuccess = false;
                massage.Body = "儲存失敗，請確認錯誤資訊!";
                massage.Msg = "We can't save your logo image , please check it again!";
            }

            return massage;
        }

        /// <summary>
        /// Create logo image in file 
        /// </summary>
        /// <param name="file">The image save file</param>
        /// <param name="file_name">The image file name</param>
        /// <param name="Base64String">Image trans to Base64</param>
        private bool CreateLogoImage(string file , string file_name, string Base64String) { 
            try{
                using (System.IO.FileStream reader = System.IO.File.Create(file + "/" + file_name))
                {
                    byte[] newbyte = Convert.FromBase64String(Base64String);
                    reader.Write(newbyte, 0, newbyte.Length);
                }
                return true;
            }catch (Exception exp){

                WriteErrorMassage(file, "CreateLogoImage", exp);

                return false;
            }
        
        }

        /// <summary>
        /// Write error massage
        /// </summary>
        /// <param name="file">Save error massage file</param>
        /// <param name="Action">Write whitch Action happen error</param>
        /// <param name="e">Error Detail</param>
        private void WriteErrorMassage(string file,string Action, Exception e) {
            string ErrorMassage = e.Message;
            System.IO.StreamWriter sw = null;
            sw = new System.IO.StreamWriter(file + "/ErrorMassage.txt", true);
            sw.Write(Action + "  :    ");
            sw.WriteLine(ErrorMassage);
            sw.Close();
        }

        /// <summary>
        /// Read a specific data type in a specific file
        /// </summary>
        /// <param name="ReadFile">Read file</param>
        /// <param name="FileType">Whitch a specific data type need to read</param>
        private List<string> Read_File_Type(string ReadFile , string FileType)
        {
            List<string> Filename = new List<string>();

            foreach (string sFile in System.IO.Directory.EnumerateFiles(ReadFile, "*." + FileType))
            {
                Filename.Add(sFile);
            }
            return Filename;
        }

        /// <summary>
        /// 由商家編號讀取商家名稱(多筆)
        /// </summary>
        /// <param name="idCell">商家編號清單</param>
        /// <returns>商家編號及商家名稱清單</returns>
        public Models.ActionResponse<List<Models.Seller_ID_Name>> GetSellerNameBySellerID(List<int> idCell)
        {
            Models.ActionResponse<List<Models.Seller_ID_Name>> result = new Models.ActionResponse<List<Models.Seller_ID_Name>>();
            result.Body = new List<Models.Seller_ID_Name>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            DB.TWSellerPortalDBContext dbSellerPortal = new DB.TWSellerPortalDBContext();

            if (idCell != null && idCell.Count > 0)
            {
                try
                {
                    result.Body = dbSellerPortal.Seller_BasicInfo.Where(x => idCell.Contains(x.SellerID)).Select(x => new Models.Seller_ID_Name()
                    {
                        ID = x.SellerID,
                        Name = x.SellerName
                    }).ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("由商家編號讀取商家名稱(多筆)失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info(string.Format("由商家編號讀取商家名稱(多筆)失敗; ErrorMessage = {0}.", "未傳入參數或傳入的參數篳數為 0"));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 由商家編號讀取商家名稱(單筆)
        /// </summary>
        /// <param name="sellerID">商家編號</param>
        /// <returns>商家名稱</returns>
        public Models.ActionResponse<string> GetSellerNameBySellerID(int sellerID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result.Finish(true, (int)Models.ResponseCode.Success, string.Empty, string.Empty);

            if (sellerID < 0)
            {
                log.Info(string.Format("由商家編號讀取商家名稱(單筆)失敗; ErrorMessage = {0}; SellerID = {1}.", "商家編號小於 0", sellerID));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            DB.TWSellerPortalDBContext dbSellerPortal = new DB.TWSellerPortalDBContext();

            try
            {
                result.Body = dbSellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => x.SellerName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Info(string.Format("由商家編號讀取商家名稱(單筆)失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)Models.ResponseCode.Success;
            }
            else
            {
                return (int)Models.ResponseCode.Error;
            }
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
