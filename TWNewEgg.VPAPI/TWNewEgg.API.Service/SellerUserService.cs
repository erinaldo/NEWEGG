using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    public class SellerUserService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        /// <summary>
        /// [Logic]
        /// Get table Seller_User Data by seller id/name
        /// </summary>
        /// <param name="User">Search table Seller_User Seller by User(keyword)</param>
        /// <param name="type">Search table Seller_User Seller , can setting search type is id or name</param>
        
        public Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>> GetSeller_User(string User, int type)
        {
            Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>> UserInfo = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>>();
            UserInfo.Body = new List<DB.TWSELLERPORTALDB.Models.Seller_User>();
            
            if (type == 0)
            {
                //用ID查詢
                int userid = 0;
                Int32.TryParse(User, out userid);
                UserInfo.Body = db.Seller_User.Where(x => x.UserID == userid).ToList<DB.TWSELLERPORTALDB.Models.Seller_User>();
                userid = 0;
            }
            else if (type == 1)
            {
                //用名稱查詢
                UserInfo.Body = db.Seller_User.Where(x => x.UserEmail == User).ToList<DB.TWSELLERPORTALDB.Models.Seller_User>();
            }

            //User.Body = db.Seller_User.Where(x => x.UserID == UserID).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_User>();
            if (UserInfo.Body == null)
            {
                UserInfo.Msg = "Table Seller_User Can't find this seller ID!";
                UserInfo.Code = 0;
                UserInfo.IsSuccess = false;
            }
            else
            {
                UserInfo.Msg = "Success";
                UserInfo.Code = 1;
                UserInfo.IsSuccess = true;
            }
            return UserInfo;
        }

        /// <summary>
        /// [Logic]
        /// Get User IS Vender or Sellser
        /// </summary>
        /// <param name="User">Search table Seller_User Seller by User(keyword)</param>
        /// <param name="type">Search table Seller_User Seller , can setting search type is id or name</param>

        public Models.ActionResponse<string> GetVenderOrSeller(string User, int type)
        {
            Models.ActionResponse<string> UserInfo = new Models.ActionResponse<string>();
            if (type == 0)
            {
                //用ID查詢
                int userid = 0;
                int groupid;
                if (Int32.TryParse(User, out userid) == true)
                {
                    try
                    {
                        var DBresult = db.Seller_User.Where(x => x.UserID == userid).Select(x => x.GroupID);

                        if (DBresult.Count() != 0)
                        {
                            groupid = DBresult.FirstOrDefault();

                            if (groupid == 5 || groupid == 3)
                            {
                                UserInfo.Body = "S";
                                UserInfo.IsSuccess = true;
                            }
                            else
                            {
                                UserInfo.Body = "V";
                                UserInfo.IsSuccess = true;
                            }
                        }
                        else
                        {
                            UserInfo.Code = (int)ResponseCode.Error;
                            UserInfo.Msg = "查無此使用者";
                            UserInfo.IsSuccess = false;
                            logger.Error("GetVenderOrSeller : 查無此使用者");
                        }
                    }
                    catch (Exception e)
                    {
                        UserInfo.Code = (int)ResponseCode.Error;
                        UserInfo.IsSuccess = false;
                        UserInfo.Msg = "查詢使用者身分時發生例外 ";
                        logger.Error("查詢使用者身分時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                    }

                }
                else
                {
                    UserInfo.Code = (int)ResponseCode.Error;
                    UserInfo.Msg = "userID 轉換錯誤";
                    UserInfo.IsSuccess = false;
                    logger.Error("GetVenderOrSeller : userID 轉換錯誤");
                }

            }
            else if (type == 1)
            {
                int groupid = 0;
                try
                {
                    var DBresult = db.Seller_User.Where(x => x.UserEmail == User).Select(x => x.GroupID);

                    if (DBresult.Count() != 0)
                    {
                        groupid = DBresult.FirstOrDefault();

                        if (groupid == 5 || groupid == 3)
                        {
                            UserInfo.Body = "S";
                            UserInfo.IsSuccess = true;
                        }
                        else
                        {
                            UserInfo.Body = "V";
                            UserInfo.IsSuccess = true;
                        }
                    }
                    else
                    {
                        UserInfo.Code = (int)ResponseCode.Error;
                        UserInfo.Msg = "查無此使用者";
                        UserInfo.IsSuccess = false;
                        logger.Error("GetVenderOrSeller : 查無此使用者");
                    }
                }
                catch (Exception e)
                {
                    UserInfo.Code = (int)ResponseCode.Error;
                    UserInfo.IsSuccess = false;
                    UserInfo.Msg = "查詢使用者身分時發生例外 ";
                    logger.Error("查詢使用者身分時發生例外 : " + e.Message + ", Stacktrace: " + e.StackTrace);
                }

            }
            return UserInfo;
        }

        /// <summary>
        /// [Logic]
        /// Save table Seller_User
        /// 1. If table Seller_User have data , save table
        /// 2. If table Seller_User have not data , create table
        /// </summary>
        /// <param name="User">Table Seller_User User</param>
        
        public Models.ActionResponse<string> SaveSeller_User(DB.TWSELLERPORTALDB.Models.Seller_User User)
        {
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            try
            {
                if (User.UserID == 0)
                {
                    User.InDate = dt;
                    User.InUserID = User.UpdateUserID;
                    User.UpdateUserID = null;
                    db.Seller_User.Add(User);
                    db.SaveChanges();
                    int UserID = db.Seller_User.Where(x => x.UserEmail == User.UserEmail).FirstOrDefault().UserID;
                    massage.Body = UserID.ToString();
                    massage.Code = 0;
                    massage.Msg = "Create new Seller_User table success !";
                    massage.IsSuccess = true;
                }
                else
                {
                    User.UpdateDate = dt;
                    db.Entry(User).State = EntityState.Modified;
                    db.SaveChanges();
                    massage.Code = 0;
                    massage.Body = "修改成功，已儲存!";
                    massage.Msg = "Save changed Seller_User table data success!";
                    massage.IsSuccess = true;
                }
            }catch(Exception e){
                massage.Code = 1;
                massage.IsSuccess = false;
                massage.Body = "修改失敗，請檢查資料!";
                massage.Msg = "Save changed Seller_User table data false!";
            }

            return massage;
        }

        //Ron
        /// <summary>
        /// Check the Email is unique or not
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public Models.ActionResponse<bool> CheckSellerUserEmailUnique(string Email)
        {
            Models.ActionResponse<bool> IsEmailUnique = new Models.ActionResponse<bool>();

            IsEmailUnique.Body = (db.Seller_User.Count(r => r.UserEmail.Trim() == Email.Trim()) == 0);

            return IsEmailUnique;
        }
        
    }
}
