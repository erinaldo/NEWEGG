using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Models;
using System.Data;
using System.Web;
using log4net;
using log4net.Config;
using System.Transactions;
using AutoMapper;

namespace TWNewEgg.API.Service
{
    public class SellerToVendorService
    {
        //private DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
        //private DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
        //private DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();
        
        // SellerChangetoVendor ID
        public int sellerChangeVendorID = 0;

        private int GetNewSellerID(int oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string oldSellerEmail = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == oldSellerID).Select(r => r.SellerEmail).FirstOrDefault();

            int newSellerID = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerEmail == oldSellerEmail).Where(y => y.AccountTypeCode == "V").Select(r => r.SellerID).FirstOrDefault();

            return newSellerID;
        }

        #region Old Seller Change To NewSeller

        public List<string> UpdateSellerBasicInfo(string oldSellerID)
        {

            List<string> changeSellerReulst = new List<string>();

            int intoldsellerID = 0;
            int.TryParse(oldSellerID, out intoldsellerID);
            TWNewEgg.API.Service.SellerBasicInfoService sellerbasicservice = new SellerBasicInfoService();

            //a. Seller_BasicInfo
            TWNewEgg.API.Service.UserService userService = new UserService();

            // 尋找舊的Seller_BasicInfo 資料
            //this.ChangeSellerStatus(sellerID);
            var sellerbasicInfo = sellerbasicservice.GetSeller_BasicInfo(oldSellerID, 0).Body;

            /*– 2. update Seller_BasicInfo.SellerID from seller 利用舊的sellerID 取得資料
             *   GetSeller_BasicInfo(string Seller, int type) 組 Model
             *   CreateSeller(Model)*/
            changeSellerReulst = this.CreateNewBasicInfo(sellerbasicInfo);

            // 有新建取得新的SellerID才進行資料轉移
            if (this.GetNewSellerID(intoldsellerID) != 0)
            {
                // b. Seller_Financial
                changeSellerReulst.Add(this.ChangeFinancial(oldSellerID));

                // c. Seller_ContactInfo
                changeSellerReulst.Add(this.ChangeContactInfo(oldSellerID));

                // d. Seller_Notification
                changeSellerReulst.Add(this.ChangeNotification(oldSellerID));

                // e. Seller_ReturnInfo
                changeSellerReulst.Add(this.ChangeReturnInfo(oldSellerID));
            }

            return changeSellerReulst;
        }

        // 修改本來 Seller 的權限
        private void ChangeSellerStatus(int sellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            var seller_basicInfo = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == sellerID && x.AccountTypeCode == "S").FirstOrDefault();

            if (seller_basicInfo != null)
            {
                seller_basicInfo.SellerStatus = "I";

                try
                {
                    sellerPortalDB.Entry(seller_basicInfo).State = EntityState.Modified;
                    sellerPortalDB.SaveChanges();                   
                }
                catch (Exception)
                {
                }
            }
        }

        private string ChangeReturnInfo(string oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;

            Service.SellerReturnAddressService SRA = new Service.SellerReturnAddressService();

            var oldReturnAddress = SRA.GetSeller_ReturnAddress(oldSellerID, 0).Body;

            int newSellerID = this.GetNewSellerID(Convert.ToInt32(oldSellerID));

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewSellerID == newSellerID).FirstOrDefault();

            if (oldReturnAddress != null)
            {

                Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo, DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo>()
                    .ForMember(x => x.SellerID, y => y.Ignore())
                    .ForMember(x => x.UpdateUserID, y => y.Ignore());

                DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo newReturnInfo = new DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo();

                newReturnInfo = Mapper.Map<DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo>(oldReturnAddress);

                newReturnInfo.SellerID = newSellerID;
                newReturnInfo.UpdateDate = DateTime.Now;
                newReturnInfo.InDate = DateTime.Now;
                newReturnInfo.InUserID = 61753;
                newReturnInfo.UpdateUserID = 61753;

                sellerPortalDB.Seller_ReturnInfo.Add(newReturnInfo);


                try
                {
                    sellerPortalDB.SaveChanges();
                    result = "Seller_ReturnInfo Change Success";
                    log.ReturnInfo = "Y";
                }
                catch (Exception ex)
                {
                    log.ReturnInfo = "E";
                    log.Exception += "Seller_ReturnInfo Change Faild";
                    result = "Seller_ReturnInfo Change Faild,Exception: " + ex.Message;
                }

            }
            else
            {
                result = "Seller 未填寫 ReturnInfo";
                log.ReturnInfo = "N";
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();

            return result;
        }

        private string ChangeNotification(string oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;

            Service.SellerNotificationService SNS = new Service.SellerNotificationService();

            int sellerid = Convert.ToInt32(oldSellerID);
            var oldNotification = sellerPortalDB.Seller_Notification.Where(x => x.SellerID == sellerid).ToList<DB.TWSELLERPORTALDB.Models.Seller_Notification>();

            int newSellerID = this.GetNewSellerID(Convert.ToInt32(oldSellerID));

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewSellerID == newSellerID).FirstOrDefault();

            if (oldNotification.Any())
            {
                foreach (var oldNotification_index in oldNotification)
                {
                    Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_Notification, DB.TWSELLERPORTALDB.Models.Seller_Notification>()
                        .ForMember(x => x.SellerID, y => y.Ignore())
                        .ForMember(x => x.UpdateUserID, y => y.Ignore());

                    DB.TWSELLERPORTALDB.Models.Seller_Notification newNotification = new DB.TWSELLERPORTALDB.Models.Seller_Notification();

                    newNotification = Mapper.Map<DB.TWSELLERPORTALDB.Models.Seller_Notification>(oldNotification_index);

                    newNotification.SellerID = newSellerID;
                    newNotification.UpdateDate = DateTime.Now;
                    newNotification.InDate = DateTime.Now;
                    newNotification.InUserID = 61753;
                    newNotification.UpdateUserID = 61753;

                    sellerPortalDB.Seller_Notification.Add(newNotification);
                }

                try
                {
                    sellerPortalDB.SaveChanges();
                    result = "Seller_Notification Change Success";
                    log.Notification = "Y";
                }
                catch (Exception ex)
                {
                    result = "Seller_Notification Change Faild, Exception: " + ex.Message;
                    log.Notification = "E";
                    log.Exception += ", Seller_Notification Change Faild";
                    try
                    {
                        result += ex.InnerException;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else
            {
                result = "Seller 未填寫 Notification";
                log.Notification = "N";
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();

            return result;
        }

        private string ChangeContactInfo(string oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;
            int sellerid = Convert.ToInt32(oldSellerID);
            int newSellerID = this.GetNewSellerID(Convert.ToInt32(oldSellerID));

            List<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> DBData = sellerPortalDB.Seller_ContactInfo.Where(x => x.SellerID == sellerid).ToList<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
            List<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> newSeller_ContactInfo = new List<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewSellerID == newSellerID).FirstOrDefault();

            if (DBData.Any())
            {
                AutoMapper.Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo, DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>()
                .ForMember(x => x.SellerID, y => y.Ignore());

                foreach (DB.TWSELLERPORTALDB.Models.Seller_ContactInfo ChangeModel in DBData)
                {
                    DB.TWSELLERPORTALDB.Models.Seller_ContactInfo newDataModel = new DB.TWSELLERPORTALDB.Models.Seller_ContactInfo();
                    AutoMapper.Mapper.Map(ChangeModel, newDataModel);

                    newDataModel.SellerID = newSellerID;
                    newDataModel.InDate = DateTime.Now;
                    newDataModel.UpdateDate = DateTime.Now;
                    newDataModel.InUserID = 61753;
                    newDataModel.UpdateUserID = 61753;

                    sellerPortalDB.Seller_ContactInfo.Add(newDataModel);
                }

                try
                {
                    sellerPortalDB.SaveChanges();
                    result = "Seller_ContactInfo Change Success";
                    log.ContactInfo = "Y";
                }
                catch (Exception ex)
                {
                    result = "Seller_ContactInfo Change Faild,Exception: " + ex.Message;
                    log.ContactInfo = "E";
                    log.Exception += ", Seller_ContactInfo Change Faild";
                    try
                    {
                        result += ex.InnerException;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else
            {
                result = "Seller 未填寫 ContactInfo";
                log.ContactInfo = "N";                
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();

            return result;
        }

        private string ChangeFinancial(string oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;
            SellerFinancialService sfs = new SellerFinancialService();

            DB.TWSELLERPORTALDB.Models.Seller_Financial oldseller_Financial = sfs.GetSeller_Financial(oldSellerID, 0).Body;
            DB.TWSELLERPORTALDB.Models.Seller_Financial newseller_Financial = new DB.TWSELLERPORTALDB.Models.Seller_Financial();
            int newSellerID = this.GetNewSellerID(Convert.ToInt32(oldSellerID));

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewSellerID == newSellerID).FirstOrDefault();
            
            if (oldseller_Financial != null)
            {
                Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_Financial, DB.TWSELLERPORTALDB.Models.Seller_Financial>()
                .ForMember(x => x.SellerID, y => y.Ignore())
                .ForMember(x => x.UpdateUserID, y => y.Ignore());

                newseller_Financial = Mapper.Map<DB.TWSELLERPORTALDB.Models.Seller_Financial>(oldseller_Financial);
                newseller_Financial.SellerID = newSellerID;
                newseller_Financial.InUserID = 61753;
                newseller_Financial.UpdateUserID = 61753;
                newseller_Financial.InDate = DateTime.Now;
                newseller_Financial.UpdateDate = DateTime.Now;

                sellerPortalDB.Seller_Financial.Add(newseller_Financial);

                try
                {
                    sellerPortalDB.SaveChanges();
                    result = "Seller_Financial Change Success";
                    log.Financial = "Y";
                }
                catch (Exception ex)
                {
                    result = "Seller_Financial Change Faild,Exception: " + ex.Message;
                    log.Financial = "E";
                    log.Exception += ", Seller_Financial Change Faild";
                    try
                    {
                        result += ex.InnerException;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else
            {
                log.Financial = "N";
                result = "Seller 未填寫 Financial";
            }

           
            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();


            return result;

        }

        // 
        private List<string> CreateNewBasicInfo(DB.TWSELLERPORTALDB.Models.Seller_BasicInfo oldsellerbasicInfo)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            List<string> result = new List<string>();
            DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor sellerchangelog = new DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor();
            sellerchangelog.OldSellerID = oldsellerbasicInfo.SellerID;

            result.Add("Seller: " + oldsellerbasicInfo.SellerEmail + ", ID: " + oldsellerbasicInfo.SellerID);
            
            Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo, DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>()
                .ForMember(x => x.AccountTypeCode, y => y.Ignore())
                .ForMember(x => x.SellerID, y => y.Ignore())
                .ForMember(x => x.InUserID, y => y.Ignore())
                .ForMember(x => x.InDate, y => y.Ignore())
                .ForMember(x => x.UpdateDate, y => y.Ignore());

            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo newSellerBasicInfoModel = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();

            newSellerBasicInfoModel = Mapper.Map<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>(oldsellerbasicInfo);

            int maxSellerID;
            if ((sellerPortalDB.Seller_BasicInfo.Max(x => x.SellerID)) != null)
                maxSellerID = sellerPortalDB.Seller_BasicInfo.Max(x => x.SellerID);
            else
                maxSellerID = 0;

            newSellerBasicInfoModel.SellerID = maxSellerID + 1;
            newSellerBasicInfoModel.AccountTypeCode = "V";
            newSellerBasicInfoModel.SellerStatus = "I";
            newSellerBasicInfoModel.InUserID = 61753;
            newSellerBasicInfoModel.UpdateUserID = 61753;
            newSellerBasicInfoModel.InDate = DateTime.Now;
            newSellerBasicInfoModel.UpdateDate = DateTime.Now;

            try
            {
                sellerPortalDB.Seller_BasicInfo.Add(newSellerBasicInfoModel);
                sellerPortalDB.SaveChanges();
                result.Add("Step1. Seller_BasicInfo Create Success");
                sellerchangelog.BasicInfo = "Y";
            }
            catch (Exception ex)
            {
                string msg = string.Empty;
                result.Add("Step1. " + ex.Message);
                sellerchangelog.BasicInfo = "E";
                msg = ex.Message;
                try
                {
                    msg += ex.InnerException;
                    result.Add(ex.InnerException.ToString());
                }
                catch (Exception)
                {

                }

                sellerchangelog.Exception = "New SellerBasic Create Failed";
            }

            sellerchangelog.InDate = DateTime.Now;
            sellerPortalDB.Seller_ChangeToVendor.Add(sellerchangelog);
            sellerPortalDB.SaveChanges();

            sellerChangeVendorID = sellerchangelog.ID;
            //寫到前台seller的service begin

            TWService twser = new TWService();
            var Twupdate = twser.UpdateTWSeller(oldsellerbasicInfo.SellerEmail, "V");
            result.Add("前台修改: " + Twupdate.Body);

            // 若成功新增並增加至前台，返回Seller_BasicInfo 取得新的 SellerID
            if (Twupdate.IsSuccess)
            {
                int newSellerID = this.GetNewSellerID(oldsellerbasicInfo.SellerID);

                var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.ID == sellerChangeVendorID).FirstOrDefault();

                log.NewSellerID = newSellerID;
                sellerPortalDB.Entry(log).State = EntityState.Modified;
                sellerPortalDB.SaveChanges();

                result.Add("新SellerID: " + newSellerID.ToString());

                // 利用 SellerEmail 至 Seller_User 將主要 Seller_User 舊的SellerID 改為 -1 and PurviewType改為 S groupid 1 改為 7
                result.Add(this.ChangeSellerUserAuth(oldsellerbasicInfo.SellerEmail, oldsellerbasicInfo.SellerID));

                // 至 Group_Purview 將 GroupID = 1 的複製一份至 Seller_Purview ，SellerID 改為新的SellerID
                var MasterSellerAuth = sellerPortalDB.Group_Purview.Where(x => x.GroupID == 1).ToList();
                result.Add(this.CombineNewAuthtoSellerPurview(MasterSellerAuth, newSellerID, oldsellerbasicInfo.SellerID));

                // 將 Seller_User 內舊的 SellerID 都改為新的 SellerID
                result.Add(this.ChangeoldSellerUser(oldsellerbasicInfo.SellerID, newSellerID));
            }
            else
            {
                var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.OldSellerID == oldsellerbasicInfo.SellerID).FirstOrDefault();
                log.Exception += ", 前台 Seller更新失敗";
                log.NewSellerID = 0;
                sellerPortalDB.Entry(log).State = EntityState.Modified;
                sellerPortalDB.SaveChanges();
              
                result.Add("前台 Seller 資料更新失敗: " + oldsellerbasicInfo.SellerEmail);
            }

            return result;
        }
        
        /// <summary>
        /// 修改 Seller_User Auth
        /// </summary>
        /// <param name="oldSellerEmail"></param>
        /// <returns></returns>
        private string ChangeSellerUserAuth(string oldSellerEmail,int oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;

            var MasterSellerUser = sellerPortalDB.Seller_User.Where(x => x.UserEmail == oldSellerEmail).FirstOrDefault();

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.ID == sellerChangeVendorID).FirstOrDefault();

            if (MasterSellerUser != null)
            {
                MasterSellerUser.GroupID = 7;
                MasterSellerUser.SellerID = -1;
                MasterSellerUser.PurviewType = "S";
                MasterSellerUser.UpdateDate = DateTime.Now;
                MasterSellerUser.UpdateUserID = 61753;
               
                try
                {
                    sellerPortalDB.Entry(MasterSellerUser).State = EntityState.Modified;
                    sellerPortalDB.SaveChanges();
                    result = "Step2. Master Seller_User 修改成功";
                    log.SellerUser = "Y";
                }
                catch (Exception ex)
                {
                    result = "Step2. Master Seller_User 修改失敗";
                    result += ex.Message;
                    log.SellerUser = "E";
                    log.Exception += ", Step2. Master Seller_User 修改失敗";
                    try
                    {
                        result += ex.InnerException;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            { 
                
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();


            return result;
        }

        /// <summary>
        /// 修改 Seller_User 內 oldSellerID 改為 newSellerID
        /// </summary>
        /// <param name="oldSellerID"></param>
        /// <param name="newSellerID"></param>
        /// <returns></returns>
        private string ChangeoldSellerUser(int oldSellerID, int newSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;

            var oldsellerUser = sellerPortalDB.Seller_User.Where(x => x.SellerID == oldSellerID).ToList();

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.ID == sellerChangeVendorID).FirstOrDefault();

            if (oldsellerUser.Any())
            {
                foreach (var seller_Userindex in oldsellerUser)
                {
                    seller_Userindex.SellerID = newSellerID;
                    seller_Userindex.UpdateUserID = 61753;
                    seller_Userindex.UpdateDate = DateTime.Now;

                    sellerPortalDB.Entry(seller_Userindex).State = EntityState.Modified;
                }

                try
                {
                    sellerPortalDB.SaveChanges();
                    result = "Step4. 修改SellerID成功, OldSellerID " + oldSellerID + " ,NewSellerID " + newSellerID;
                    log.SellerUser = "Y";
                }
                catch (Exception ex)
                {
                    result = "Step4. 修改Seller_User SellerID 失敗";
                    result += ex.Message;
                    log.SellerUser = "E";
                    log.Exception += ", Step4. 修改Seller_User SellerID 失敗";
                    try
                    {
                        result += ex.InnerException;
                    }
                    catch (Exception)
                    {


                    }
                }
            }
            else
            {
                result = "Step4. Seller_User 底下無 User";
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();

            return result;
        }

        /// <summary>
        /// 新建新的 Seller Auth
        /// </summary>
        /// <param name="MasterSellerAuth"></param>
        private string CombineNewAuthtoSellerPurview(List<DB.TWSELLERPORTALDB.Models.Group_Purview> MasterSellerAuth, int newSellerID, int oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string result = string.Empty;
            List<DB.TWSELLERPORTALDB.Models.Seller_Purview> listNewSeller_Purview = new List<DB.TWSELLERPORTALDB.Models.Seller_Purview>();

            var log = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.ID == sellerChangeVendorID).FirstOrDefault();

            if (!sellerPortalDB.Seller_Purview.Where(x => x.SellerID == newSellerID).Any())
            {
                // 建立新 Seller 的 Auth Menu
                foreach (var auth_Index in MasterSellerAuth)
                {
                    DB.TWSELLERPORTALDB.Models.Seller_Purview masterSeller_Purview = new DB.TWSELLERPORTALDB.Models.Seller_Purview();

                    masterSeller_Purview.Enable = auth_Index.Enable;
                    masterSeller_Purview.FunctionID = auth_Index.FunctionID;
                    masterSeller_Purview.InDate = DateTime.Now;
                    masterSeller_Purview.InUserID = 61753;
                    masterSeller_Purview.SellerID = newSellerID;
                    masterSeller_Purview.SN = auth_Index.SN;
                    masterSeller_Purview.UpdateDate = DateTime.Now;
                    masterSeller_Purview.UpdateUserID = 61753;

                    sellerPortalDB.Seller_Purview.Add(masterSeller_Purview);
                }
            }
            else
            {
                result += "SellerID: " + newSellerID + "已經建立 Purview";
                log.SellerPurview = "E";
                log.Exception += "SellerID: " + newSellerID + "已經建立 Purview"; 
            }

            if (!sellerPortalDB.Seller_Purview.Where(x => x.SellerID == oldSellerID).Any())
            {
                // 建立舊 Seller 的 Auth Menu
                foreach (var auth_Index in MasterSellerAuth)
                {
                    DB.TWSELLERPORTALDB.Models.Seller_Purview masterSeller_Purview = new DB.TWSELLERPORTALDB.Models.Seller_Purview();

                    masterSeller_Purview.Enable = auth_Index.Enable;
                    masterSeller_Purview.FunctionID = auth_Index.FunctionID;
                    masterSeller_Purview.InDate = DateTime.Now;
                    masterSeller_Purview.InUserID = 61753;
                    masterSeller_Purview.SellerID = oldSellerID;
                    masterSeller_Purview.SN = auth_Index.SN;
                    masterSeller_Purview.UpdateDate = DateTime.Now;
                    masterSeller_Purview.UpdateUserID = 61753;

                    sellerPortalDB.Seller_Purview.Add(masterSeller_Purview);
                }
            }
            else
            {
                result += "SellerID: " + oldSellerID + "已經建立 Purview";
                log.SellerPurview = "E";
                log.Exception += "SellerID: " + oldSellerID + "已經建立 Purview"; 
            }

            try
            {
                sellerPortalDB.SaveChanges();
                result += "Step3. New Seller_Purview Create ok: " + newSellerID.ToString();
                log.SellerPurview = "Y";
            }
            catch (Exception ex)
            {
                log.SellerPurview = "E";
                log.Exception += "SellerPurview 發生例外錯誤"; 

                result = ex.Message;
                try
                {
                    result += ex.InnerException;
                }
                catch (Exception)
                {

                }
            }

            sellerPortalDB.Entry(log).State = EntityState.Modified;
            sellerPortalDB.SaveChanges();

            return result;
        }


        #endregion
        

    }
}
