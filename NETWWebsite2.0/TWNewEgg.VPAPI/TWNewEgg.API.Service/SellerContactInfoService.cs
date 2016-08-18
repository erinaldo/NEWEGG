using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace TWNewEgg.API.Service
{
    public class SellerContactInfoService
    {
        ManageAccountService MA = new ManageAccountService();

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        #region Seller_ContactAddress table
        /// <summary>
        /// Get table Seller_ContactAddress
        /// 1. If table Seller_ContactAddress have data , read table
        /// (Data list maybe one or more...)
        /// 2. If table Seller_Notification have not data , Empty table
        /// </summary>
        /// <param name="Seller">Table Seller_Notification Seller Name</param>
        public Models.ActionResponse<List<API.Models.Seller_ContactInfoData>> GetSeller_ContactInfo(string Seller)
        {
            int sellerid = 0;
            Models.ActionResponse<List<API.Models.Seller_ContactInfoData>> ContactAddress = new Models.ActionResponse<List<API.Models.Seller_ContactInfoData>>();
            ContactAddress.Body = new List<API.Models.Seller_ContactInfoData>();
            if (MA.isright(Seller, @"[0-9]+"))
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else
            {
                //用名稱查詢
                sellerid = MA.SellerID(Seller);
            }

            List<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo> DBData = db.Seller_ContactInfo.Where(x => x.SellerID == sellerid).ToList<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();

            

            AutoMapper.Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo, API.Models.Seller_ContactInfoData>();

            foreach (DB.TWSELLERPORTALDB.Models.Seller_ContactInfo ChangeModel in DBData) {
                API.Models.Seller_ContactInfoData Model = new Models.Seller_ContactInfoData();
                AutoMapper.Mapper.Map(ChangeModel , Model);
                ContactAddress.Body.Add(Model);
            
            }

            if (ContactAddress.Body.Count == 0)
            {
                ContactAddress.Msg = "Table Seller_ContactInfo Can't find this seller ID!";
                ContactAddress.Code = 1;
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
        public Models.ActionResponse<string> SaveSeller_ContactInfo(API.Models.Seller_ContactInfoData ContactAddress)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            DB.TWSELLERPORTALDB.Models.Seller_ContactInfo SavaModelData = new DB.TWSELLERPORTALDB.Models.Seller_ContactInfo();

            AutoMapper.Mapper.CreateMap<API.Models.Seller_ContactInfoData, DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
            AutoMapper.Mapper.Map(ContactAddress, SavaModelData);

            try
            {
                if (ContactAddress.SN == 0)
                {
                    SavaModelData.InDate = dt;
                    SavaModelData.InUserID = SavaModelData.InUserID;
                    SavaModelData.UpdateUserID = SavaModelData.UpdateUserID;
                    db.Seller_ContactInfo.Add(SavaModelData);
                    db.SaveChanges();
                    massage.Body = "地址資料，新增成功!";
                    massage.Msg = "Create new Seller_ContactAddress table success !";
                    massage.Code = 0;
                    massage.IsSuccess = true;
                }
                else
                {
                    var intInUserID = db.Seller_ContactInfo.Where(x => x.SellerID == SavaModelData.SellerID && x.ContactTypeID == SavaModelData.ContactTypeID && x.EmailAddress == SavaModelData.EmailAddress).Select(x => x.InUserID).Single();
                    if (SavaModelData.PrimaryCode == "Y")
                    {
                        DB.TWSELLERPORTALDB.Models.Seller_ContactInfo CA = new DB.TWSELLERPORTALDB.Models.Seller_ContactInfo();
                        CA = db.Seller_ContactInfo.Where(x => x.PrimaryCode == "Y" && x.SellerID == SavaModelData.SellerID && x.SN != SavaModelData.SN && x.ContactTypeID == SavaModelData.ContactTypeID).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
                        if (CA != null)
                        {
                            CA.PrimaryCode = "N";
                            CA.UpdateDate = dt;
                            CA.UpdateUserID = SavaModelData.UpdateUserID;
                            db.Entry(CA).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        SavaModelData.UpdateDate = dt;
                        SavaModelData.InUserID = intInUserID;
                        db.Entry(SavaModelData).State = EntityState.Modified;

                        db.SaveChanges();
                        massage.Body = "主要設定地址，成功儲存!";
                        massage.Msg = "Save changed Seller_ContactAddress table data success!";
                        massage.Code = 0;
                        massage.IsSuccess = true;
                    }
                    else if (SavaModelData.PrimaryCode == "N")
                    {
                        //特殊狀況，Email 為 primary key 如需修改須將舊有資料刪除，重新建立(之後必須將EmailAddress改為"非"primary key)
                        DB.TWSELLERPORTALDB.Models.Seller_ContactInfo CA = new DB.TWSELLERPORTALDB.Models.Seller_ContactInfo();
                        CA = db.Seller_ContactInfo.Where(x => x.SN == ContactAddress.SN).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_ContactInfo>();
                        if (SavaModelData.EmailAddress == CA.EmailAddress && SavaModelData.ContactTypeID == CA.ContactTypeID )
                        {
                            CA.Address = SavaModelData.Address;
                            CA.City = SavaModelData.City;
                            CA.CountryCode = SavaModelData.CountryCode;
                            CA.FirstName = SavaModelData.FirstName;
                            CA.InDate = SavaModelData.InDate;
                            CA.InUserID = intInUserID;
                            CA.LastName = SavaModelData.LastName;
                            CA.Phone = SavaModelData.Phone;
                            CA.PhoneExt = SavaModelData.PhoneExt;
                            CA.PhoneRegion = SavaModelData.PhoneRegion;
                            CA.State = SavaModelData.State;
                            CA.UpdateDate = SavaModelData.UpdateDate;
                            CA.UpdateUserID = SavaModelData.UpdateUserID;
                            CA.ZipCode = SavaModelData.ZipCode;
  
                            db.Entry(CA).State = EntityState.Modified;
                            db.SaveChanges();
                        }else{
                            db.Seller_ContactInfo.Remove(CA);
                            SavaModelData.InDate = dt;
                            SavaModelData.InUserID = intInUserID;
                            //SavaModelData.UpdateUserID = SavaModelData.UpdateUserID;
                            db.Seller_ContactInfo.Add(SavaModelData);
                            db.SaveChanges();
                        }
                        massage.Body = "地址資料，修改成功，已儲存!";
                        massage.Code = 0;
                        massage.Msg = "Save changed Seller_ContactAddress table data success!";
                        massage.IsSuccess = true;
                    }
                    else {
                        massage.IsSuccess = false;
                        massage.Body = "修改失敗，請檢查PrimaryCode!";
                        massage.Code = 1;
                        massage.Msg = "Save changed Seller_ContactAddress table PrimaryCode error!";
                    }
                    
                }
            }
            catch (Exception exp)
            {
                WriteErrorMassage(AppDomain.CurrentDomain.BaseDirectory + "pic\\sellerlogo", "SaveSeller_ContactInfo", exp.InnerException.InnerException);

                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Seller_ContactAddress table data false!";
            }

            return massage;
        }

        /// <summary>
        /// Delete table Seller_ContactAddress 
        /// </summary>
        /// <param name="Seller">Table Seller_ContactAddress Seller Name</param>
        public Models.ActionResponse<API.Models.Seller_ContactInfoData> DeleteSeller_ContactInfo(API.Models.Seller_ContactInfoData ContactAddress)
        {
            Models.ActionResponse<API.Models.Seller_ContactInfoData> CA = new Models.ActionResponse<API.Models.Seller_ContactInfoData>();
            CA.Body = new API.Models.Seller_ContactInfoData();
            CA.Body = ContactAddress;
            try
            {
                var DCA = db.Seller_ContactInfo.Where(x => x.SN == ContactAddress.SN).First();
                db.Seller_ContactInfo.Remove(DCA);
                db.SaveChanges();

                CA.Msg = "資料刪除成功! Delete Seller_ContactAddress table data success!";
                CA.IsSuccess = true;
            }
            catch (Exception e)
            {
                CA.IsSuccess = false;
                CA.Msg = "刪除失敗，請檢查資料! Delete Seller_ContactAddress table data false!";
            }

            return CA;
        }

        /// <summary>
        /// table Seller_ContactType Info
        /// </summary>

        public Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>> GetSeller_ContactType()
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>> CA = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>>();
            CA.Body = new List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>();
            try
            {
                List<DB.TWSELLERPORTALDB.Models.Seller_ContactType> ContactType = db.Seller_ContactType.ToList();

                CA.Body = ContactType;
                CA.Msg = "success!";
                CA.IsSuccess = true;
            }
            catch (Exception e)
            {
                CA.IsSuccess = false;
                CA.Msg = "抓不到資料!";
            }

            return CA;
        }
        #endregion Seller_ContactAddress table


        /// <summary>
        /// Write error massage
        /// </summary>
        /// <param name="file">Save error massage file</param>
        /// <param name="Action">Write whitch Action happen error</param>
        /// <param name="e">Error Detail</param>
        private void WriteErrorMassage(string file, string Action, Exception e)
        {
            string ErrorMassage = e.Message;
            System.IO.StreamWriter sw = null;
            sw = new System.IO.StreamWriter(file + "/ErrorMassage.txt", true);
            sw.Write(Action + "  :    ");
            sw.WriteLine(ErrorMassage);
            sw.Close();
        }
    }
}
