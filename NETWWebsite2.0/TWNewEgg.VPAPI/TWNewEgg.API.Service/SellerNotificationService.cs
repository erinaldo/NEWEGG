using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TWNewEgg.API.Service
{
    public class SellerNotificationService
    {
        ManageAccountService MA = new ManageAccountService();

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        #region Seller_Notification table
        /// <summary>
        /// [Logic]
        /// Get table Seller_Notification
        /// 1. If table Seller_Notification have data , read table
        /// (there have 5 list data , Like:ON、VON、BN、FN、RMA)
        /// 2. If table Seller_Notification have not data , Empty table
        /// </summary>
        /// <param name="Seller">Search table Seller_Notification Seller Name</param>
        /// <param name="type">Table Seller_Notification Seller Name</param>
        public Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>> GetSeller_Notification(string Seller,int type)
        {
            int sellerid = 0;
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>> Notification = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>>();
            Notification.Body = new List<DB.TWSELLERPORTALDB.Models.Seller_Notification>();
            if (type == 0)
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
                Notification.Body = db.Seller_Notification.Where(x => x.SellerID == sellerid).ToList<DB.TWSELLERPORTALDB.Models.Seller_Notification>();
            }
            else if (type == 1)
            {
                //用名稱查詢
                sellerid = MA.SellerID(Seller);
                //Notification.Body = db.Seller_Notification.Where(x => x.S == sellerid).ToList<DB.TWSELLERPORTALDB.Models.Seller_Notification>();
            }
            
            if (Notification.Body.Count <= 5)
            {
                //需要先給須新增的資料，Silverlight上會需要，否則抓不到值
                Int32.TryParse(Seller, out sellerid);

                int[] ListNotification = new int[5]{ 0 , 0 , 0 , 0 , 0 };

                foreach(DB.TWSELLERPORTALDB.Models.Seller_Notification NotificationData in Notification.Body){
                    switch (NotificationData.NotificationTypeCode){
                        case "ON":
                            ListNotification[0] = 1;
                            break;
                        case "VON":
                            ListNotification[1] = 1;
                            break;
                        case "BN":
                            ListNotification[2] = 1;
                            break;
                        case "FN":
                            ListNotification[3] = 1;
                            break;
                        case "RMA":
                            ListNotification[4] = 1;
                            break;
                    }
                }

                if (ListNotification[0] == 0)
                {
                    //Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "ON", Enabled = "A" });
                    Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "ON", Enabled = "N" });
                }

                if (ListNotification[1] == 0)
                {
                    //Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "VON", Enabled = "A" });
                    Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "VON", Enabled = "N" });
                }

                if (ListNotification[2] == 0)
                {
                    Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "BN", Enabled = "Y" });
                }

                if (ListNotification[3] == 0)
                {
                    Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "FN", Enabled = "Y" });
                }

                if (ListNotification[4] == 0)
                {
                    //Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "RMA", Enabled = "A" });
                    Notification.Body.Add(new DB.TWSELLERPORTALDB.Models.Seller_Notification() { SellerID = sellerid, NotificationTypeCode = "RMA", Enabled = "N" });
                }
            

                Notification.Msg = "Table Seller_Notification Can't find this seller ID!";
                Notification.Code = 1;
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
        /// [Logic]
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

            try
            {
                for (int i = 0; i < Notification.Count(); i++)
                {
                    if (Notification[i].EmailAddress1 == "" || Notification[i].EmailAddress1 == null)
                    {
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body.Add("失敗，請確認第" + (i + 1) + "筆資料，EmailAddress1有無資料!");
                        massage.Msg = "false! Please check no." + i + ", EmailAddress1 have data!";
                    }
                    else if (Notification[i].SN == 0)
                    {
                        Notification[i].InDate = dt;
                        Notification[i].InUserID = Notification[i].UpdateUserID;
                        Notification[i].UpdateUserID = null;
                        Notification[i].UpdateDate = dt;
                        db.Seller_Notification.Add(Notification[i]);
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body.Add("第" + (i + 1) + "筆資料，新增成功!");
                        massage.Msg = "Create new Seller_Notification table success !";
                    }
                    else
                    {
                        Notification[i].UpdateDate = dt;
                        db.Entry(Notification[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body.Add("第" + (i + 1) + "筆資料，修改成功，已儲存!");
                        massage.Msg = "Save changed Seller_Notification table data success!";
                    }
                }
            }
            catch (Exception e)
            {
                massage.IsSuccess = false;
                massage.Code = 1;
                massage.Body.Add("修改失敗，請檢查資料!");
                massage.Msg = "Save changed Seller_Notification table data list false!";
            }

            return massage;
        }
        #endregion Seller_Notification table
    }
}
