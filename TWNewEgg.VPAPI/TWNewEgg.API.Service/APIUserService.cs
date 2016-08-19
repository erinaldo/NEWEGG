using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Account Service
    /// </summary>
    public class APIUserService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        /// <summary>
        /// Account Login
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.LoginResult> Login(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<Models.LoginResult> result = new Models.ActionResponse<Models.LoginResult>();
            
            string pwd = AesEncryptor.AesEncrypt(loginInfo.Password);
            
            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            DB.TWBACKENDDB.Models.API_User user = backendDB.API_User.Where(x => x.UserName == loginInfo.UserName && x.Pwd==pwd).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Login Fail";
                result.Body = new Models.LoginResult();
                result.Body.Token = "";
                result.Body.UserName = loginInfo.UserName;
            }
            else
            {
                //Generate AccessToken
                /*user.AccessToken = AesEncryptor.AesEncrypt(user.UserName + DateTime.UtcNow.ToString());
                user.UpdateDate = DateTime.UtcNow.AddHours(8);
                user.UpdateUserID = 0;
                backendDB.SaveChanges();*/

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Success";
                result.Body = new Models.LoginResult();
                result.Body.UserName = loginInfo.UserName;
                result.Body.Token = user.AccessToken;
            }
            return result;
        }

        public Models.ActionResponse<Models.LoginResult> CreateUser(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<Models.LoginResult> result = new Models.ActionResponse<Models.LoginResult>();

            //Check UserEmail is unique
            

            return result;
        }

        public Models.ActionResponse<DB.TWBACKENDDB.Models.API_User> UserDetail(Models.LoginInfo loginInfo)
        {
            Models.ActionResponse<DB.TWBACKENDDB.Models.API_User> result = new Models.ActionResponse<DB.TWBACKENDDB.Models.API_User>();
            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            string pwd = AesEncryptor.AesEncrypt(loginInfo.Password);
            var user = backendDB.API_User.Where(x => x.UserName == loginInfo.UserName && x.Pwd==pwd).FirstOrDefault();
            result.Code = 0;
            result.IsSuccess = true;
            result.Msg = "";
            result.Body = user;
            return result;
        }
    }
}
