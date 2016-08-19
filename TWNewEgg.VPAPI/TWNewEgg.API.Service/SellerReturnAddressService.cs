using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TWNewEgg.API.Service
{
    public class SellerReturnAddressService
    {
        ManageAccountService MA = new ManageAccountService();

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        #region Seller_ReturnAddress table
        
        /// <summary>
        /// [Logic]
        /// Get table Seller_ReturnAddress Data by seller id/name
        /// </summary>
        /// <param name="Seller">Search table Seller_ReturnAddress Seller keyword</param>
        /// <param name="type">Search table Seller_ReturnAddress Seller type , whitch is id or name</param>

        public Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo> GetSeller_ReturnAddress(string Seller, int type)
        {
            int sellerid = 0;
            Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo> ReturnAddress = new Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo>();
            ReturnAddress.Body = new TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo();
            TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo ReturnInfoAdress = new TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo();
            if (type == 0)
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else
            {
                //用名稱查詢
                sellerid = MA.SellerID(Seller);
            }

            //ReturnAddress.Body = ReturnInfoAdress;
            ReturnAddress.Body = db.Seller_ReturnInfo.Where(x => x.SellerID == sellerid).FirstOrDefault<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo>();
            if (ReturnAddress.Body == null)
            {
                ReturnAddress.Msg = "Table SaveSeller_ReturnAddress Can't find this seller ID!";
                ReturnAddress.Code = 1;
                ReturnAddress.IsSuccess = false;
            }
            else
            {
                ReturnAddress.Msg = "Success";
                ReturnAddress.Code = 0;
                ReturnAddress.IsSuccess = true;
            }
            return ReturnAddress;
        }

        /// <summary>
        /// [Logic]
        /// Save table Seller_ReturnAddress
        /// 1. If table Seller_ReturnAddress have data , save table
        /// 2. If table Seller_ReturnAddress have not data , create table
        /// </summary>
        /// <param name="ReturnAddress">Table Seller_ReturnAddress Seller</param>

        public Models.ActionResponse<string> SaveSeller_ReturnAddress(TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo ReturnAddress)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            try
            {
                TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo a = db.Seller_ReturnInfo.Where(x => x.SellerID == ReturnAddress.SellerID).FirstOrDefault();


                if (db.Seller_ReturnInfo.Where( x => x.SellerID == ReturnAddress.SellerID).FirstOrDefault() == null)
                {
                    ReturnAddress.InDate = dt;
                    ReturnAddress.InUserID = ReturnAddress.UpdateUserID;
                    ReturnAddress.UpdateUserID = null;
                    db.Seller_ReturnInfo.Add(ReturnAddress);
                    db.SaveChanges();
                    massage.IsSuccess = true;
                    massage.Code = 0;
                    massage.Body = "新增成功!";
                    massage.Msg = "Create new Seller_ReturnAddress table success !";
                }
                else
                {
                    DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo SaveData = new DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo();
                    SaveData = db.Seller_ReturnInfo.Find(ReturnAddress.SellerID);

                    SaveData.RefundPeriod = ReturnAddress.RefundPeriod;
                    SaveData.ReplacementPeriod = ReturnAddress.ReplacementPeriod;
                    SaveData.RestockingFee = ReturnAddress.RestockingFee;
                    SaveData.ReturnPolicy = ReturnAddress.ReturnPolicy;
                    SaveData.UpdateUserID = ReturnAddress.UpdateUserID;
                    SaveData.UpdateDate = dt;
                    db.Entry(SaveData).State = EntityState.Modified;
                    db.SaveChanges();
                    massage.IsSuccess = true;
                    massage.Code = 0;
                    massage.Body = "修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_ReturnAddress table data success!";
                }
            }
            catch (Exception e)
            {
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Seller_ReturnAddress table data false!";
            }

            return massage;
        }
        #endregion Seller_ReturnAddress table
    }
}
