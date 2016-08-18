using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TWNewEgg.API.Service
{
    public class ManageAccountService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        //檢驗資料格式是否符合  Boolean
        public Boolean isright(string text, String rule) 
        {
            Regex Regex1 = new Regex(rule, RegexOptions.IgnoreCase);
            return Regex1.IsMatch(text);
        }

        public int SellerID (string SellerName) {
            int sellerid = db.Seller_BasicInfo.Where(x => x.SellerName == SellerName).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>().SellerID;
            return sellerid;
        }

        #region Seller_BasicInfo table
        /// <summary>
        /// Get table GetSeller_BasicInfo data by seller ID
        /// </summary>
        /// <param name="Seller">Table Seller_BasicInfo Seller Name</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> GetSeller_BasicInfo(string Seller)
        {
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> BasicInfo = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            BasicInfo.Body = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();

            if (isright(Seller, @"[0-9]+"))
            {
                //用ID查詢
                int sellerid = 0;
                Int32.TryParse(Seller, out sellerid);
                BasicInfo.Body = db.Seller_BasicInfo.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            }
            else {
                //用名稱查詢
                BasicInfo.Body = db.Seller_BasicInfo.Where(x => x.SellerName == Seller).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            }
            
            
            if (BasicInfo.Body == null)
            {
                BasicInfo.Msg = "Table Seller_BasicInfo Can't find this seller ID!";
                BasicInfo.Code = 0;
                BasicInfo.IsSuccess = false;
            }
            else {
                BasicInfo.Msg = "Success";
                BasicInfo.Code = 0;
                BasicInfo.IsSuccess = true;
            }
            return BasicInfo;
        }

        /// <summary>
        /// Save table Seller_BasicInfo
        /// just save table
        /// </summary>
        /// <param name="BasicInfo">Table Seller_BasicInfo Data</param>
        public Models.ActionResponse<string> SaveSeller_BasicInfo(DB.TWSELLERPORTALDB.Models.Seller_BasicInfo BasicInfo)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            //DB.TWSELLERPORTALDB.Models.Seller_BasicInfo Table = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            //Table = BasicInfo;
            //暫定
            BasicInfo.UpdateUserID = 1;
            BasicInfo.CreateDate = dt;

            //沒有 Seller id 編號，為新增
            if (BasicInfo.SellerID == 0)
            {
                //判定是否有名稱重複
                if (db.Seller_BasicInfo.Where(x => x.SellerName == BasicInfo.SellerName).FirstOrDefault() == null)
                {
                    db.Seller_BasicInfo.Add(BasicInfo);
                    db.SaveChanges();
                    massage.Body = "新增成功!";
                    massage.Msg = "Create new SaveSeller_BasicInfo table success !";
                }
                else
                {
                    massage.Body = "商家名稱重複，請確認後再填入!";
                    massage.Msg = "SaveSeller_BasicInfo table of the same name already exists, please check your seller's name!";
                }
            }
            //已有 Seller id 編號，為修改
            else {
                db.Entry(BasicInfo).State = EntityState.Modified;
                db.SaveChanges();
                massage.Body = "修改成功，已儲存!";
                massage.Msg = "Save changed SaveSeller_BasicInfo table data success!";
            }

            return massage;
        }
        #endregion Seller_BasicInfo table

        #region Seller_Financial table
        /// <summary>
        /// Get table Seller_Financial Data by seller ID
        /// </summary>
        /// <param name="Seller">Table Seller_Financial Seller Name</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> GetSeller_Financial(string Seller)
        {
            int sellerid = 0;
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> Financial = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial>();
            Financial.Body = new DB.TWSELLERPORTALDB.Models.Seller_Financial();
            if (isright(Seller, @"[0-9]+"))
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else
            {
                //用名稱查詢
                sellerid = SellerID(Seller);
            }
            Financial.Body = db.Seller_Financial.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_Financial>();
            if (Financial.Body == null)
            {
                Financial.Msg = "Table Seller_Financial Can't find this seller ID!";
                Financial.Code = 0;
                Financial.IsSuccess = false;
            }
            else
            {
                Financial.Msg = "Success";
                Financial.Code = 0;
                Financial.IsSuccess = true;
            }
            return Financial;
        }
        /// <summary>
        /// Save table Seller_Financial
        /// 1. If table Seller_Financial have data , save table
        /// 2. If table Seller_Financial have not data , create table
        /// </summary>
        /// <param name="Financial">Table Seller_Financial Seller Data</param>
        public Models.ActionResponse<string> SaveSeller_Financial(DB.TWSELLERPORTALDB.Models.Seller_Financial Financial)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            if (Financial.SellerID == 0) {
                db.Seller_Financial.Add(Financial);
                db.SaveChanges();
                massage.Body = "新增成功!";
                massage.Msg = "Create new Seller_Financial table success !";
            } else {
                db.Entry(Financial).State = EntityState.Modified;
                db.SaveChanges();
                massage.Body = "修改成功，已儲存!";
                massage.Msg = "Save changed Seller_Financial table data success!";
            }

            return massage;
        }
        #endregion Seller_Financial table

        #region Seller_ContactAddress table
        /// <summary>
        /// Get table Seller_ContactAddress
        /// 1. If table Seller_ContactAddress have data , read table
        /// (Data list maybe one or more...)
        /// 2. If table Seller_Notification have not data , Empty table
        /// </summary>
        /// <param name="Seller">Table Seller_Notification Seller Name</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> GetSeller_ContactAddress(string Seller)
        {
            int sellerid = 0;
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> ContactAddress = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
            ContactAddress.Body = new DB.TWSELLERPORTALDB.Models.Seller_ContactInfo();
            if (isright(Seller, @"[0-9]+"))
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else
            {
                //用名稱查詢
                sellerid = SellerID(Seller);
            }
            ContactAddress.Body = db.Seller_ContactInfo.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
            if (ContactAddress.Body == null)
            {
                ContactAddress.Msg = "Table Seller_Financial Can't find this seller ID!";
                ContactAddress.Code = 0;
                ContactAddress.IsSuccess = false;
            }
            else
            {
                ContactAddress.Msg = "Success";
                ContactAddress.Code = 0;
                ContactAddress.IsSuccess = true;
            }
            return ContactAddress;
        }

        /// <summary>
        /// Save table Seller_ContactAddress
        /// 1. If table Seller_ContactAddress have data , save table
        /// 2. If table Seller_ContactAddress have not data , create table
        /// </summary>
        /// <param name="Seller">Table Seller_ContactAddress Seller Name</param>
        public Models.ActionResponse<List<string>> SaveSeller_ContactAddress(List<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> ContactAddress)
        {
            Models.ActionResponse<List<string>> massage = new Models.ActionResponse<List<string>>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            massage.Body = new List<string>();
            for (int i = 0; i < ContactAddress.Count() ; i++)
            {
                if (ContactAddress[i].SellerID == 0)
                {
                    db.Seller_ContactInfo.Add(ContactAddress[i]);
                    db.SaveChanges();
                    massage.Body[i] = "第" + i + "筆地址資料，新增成功!";
                    massage.Msg = "Create new Seller_ContactAddress table success !";
                }
                else
                {
                    db.Entry(ContactAddress[i]).State = EntityState.Modified;
                    db.SaveChanges();
                    massage.Body[i] = "第" + i + "筆地址資料，修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_ContactAddress table data success!";
                }

            }
            

            return massage;
        }

        /// <summary>
        /// Delete table Seller_ContactAddress 
        /// </summary>
        /// <param name="Seller">Table Seller_ContactAddress Seller Name</param>
        public Models.ActionResponse<string> DeleteSeller_ContactAddress(DB.TWSELLERPORTALDB.Models.Seller_ContactInfo ContactAddress)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            db.Seller_ContactInfo.Remove(ContactAddress);
            db.SaveChanges();

            massage.Body = "資料刪除成功!";
            massage.Msg = "Delete Seller_ContactAddress table data success!";

            return massage;
        }
        #endregion Seller_ContactAddress table

        #region Seller_Notification table
        /// <summary>
        /// Get table Seller_Notification
        /// 1. If table Seller_Notification have data , read table
        /// (there have 5 list data , Like:ON、VON、BN、FN、RMA)
        /// 2. If table Seller_Notification have not data , Empty table
        /// </summary>
        /// <param name="Seller">Table Seller_Notification Seller Name</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Notification> GetSeller_Notification(string Seller)
        {
            int sellerid = 0;
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Notification> Notification = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Notification>();
            Notification.Body = new DB.TWSELLERPORTALDB.Models.Seller_Notification();
            if (isright(Seller, @"[0-9]+"))
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else
            {
                //用名稱查詢
                sellerid = SellerID(Seller);
            }
            //Notification.Body = db.Seller_Notification.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_Notification>();
            if (Notification.Body == null)
            {
                Notification.Msg = "Table Seller_Financial Can't find this seller ID!";
                Notification.Code = 0;
                Notification.IsSuccess = false;
            }
            else
            {
                Notification.Msg = "Success";
                Notification.Code = 0;
                Notification.IsSuccess = true;
            }
            return Notification;
        }

        /// <summary>
        /// Save table Seller_Notification
        /// 1. If table Seller_Notification have data , save table
        /// 2. If table Seller_Notification have not data , create table
        /// </summary>
        /// <param name="Seller">Table Seller_Notification Seller Name</param>
        public Models.ActionResponse<List<string>> SaveSeller_Notification(List<DB.TWSELLERPORTALDB.Models.Seller_Notification> Notification)
        {
            Models.ActionResponse<List<string>> massage = new Models.ActionResponse<List<string>>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            massage.Body = new List<string>();

            for (int i = 0; i < Notification.Count(); i++)
            {
                if (Notification[i].SellerID == 0)
                {
                    db.Seller_Notification.Add(Notification[i]);
                    db.SaveChanges();
                    massage.Body[i] = "第" + i + "筆資料，新增成功!";
                    massage.Msg = "Create new Seller_Notification table success !";
                }
                else
                {
                    db.Entry(Notification[i]).State = EntityState.Modified;
                    db.SaveChanges();
                    massage.Body[i] = "第" + i + "筆資料，修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_Notification table data success!";
                }
            }

            return massage;
        }
        #endregion Seller_Notification table


    }
}
