using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserLoginResult
    {
        /// <summary>
        /// 使用者ID  (not null)
        /// </summary>
        public string UserID { get; set; }
        public string SellerID { get; set; }
        /// <summary>
        /// Token  (not null)
        /// </summary>

        public string AccessToken { get; set; }
        public bool IsAdmin { get; set; }
        public string AccountTypeCode { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string GroupID { get; set; }

        public string Token { get; set; }

        
        public string PurviewType { get; set; }
    }

    public enum UserLoginingResponseCode
    {
        Success = 0,
        Error = 1,
        URLFailedValidation = 2,
        ResetPasswordTimeOut = 3,
        RequiredFieldEmpty = 4,
        SellerExist = 5,
        UserExist = 6,
        UserNotFound = 7,
        PurviewTypeError = 8,
        Accountalreadystop = 9,
        PasswordFaild = 10,
        AccountTypeError = 11
    }
}
