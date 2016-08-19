using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.Models.ViewModels.Register
{
    public class RegistrationError
    {
        public RegistrationError() {
            this.account = new AccountError();
            this.password = false;
            this.passwordError = new PasswordError();
            this.confirmpassword = false;
            this.cellphone = false;
            this.securitycode = false;
            this.sex = false;
            this.familyname = false;
            this.name = false;
            this.agreePaper = false;
            this.birthday = false;
            this.error = false;
            this.errormessage = "";
        }
        /// <summary>
        /// user account
        /// </summary>
        public AccountError account { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public bool password { get; set; }
        /// <summary>
        /// 密碼錯誤內容
        /// </summary>
        public PasswordError passwordError { get; set; }
        /// <summary>
        /// 確認密碼
        /// </summary>
        public bool confirmpassword { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public bool cellphone { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public bool securitycode { get; set; }
        /// <summary>
        /// 暱稱
        /// </summary>
        public bool sex { get; set; }
        /// <summary>
        /// 姓氏
        /// </summary>
        public bool familyname { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public bool name { get; set; }
        /// <summary>
        /// 同意電子報
        /// </summary>
        public bool agreePaper { get; set; }
        /// <summary>
        /// 生日日期
        /// </summary>
        public bool birthday { get; set; }  
        /// <summary>
        /// 是否錯誤
        /// </summary>
        public bool error { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string errormessage { get; set; }
    }

    /// <summary>
    /// 密碼錯誤內容
    /// </summary>
    public class PasswordError
    {
        public PasswordError() {
            this.sameAccountPassword = false;
            this.passwordLength = false;
            this.includingIntEnglish = false;
            this.error = false;
        }
        //帳密相同
        public bool sameAccountPassword { get; set; }
        //密碼長度
        public bool passwordLength { get; set; }
        //含數字與英文
        public bool includingIntEnglish { get; set; }
        //error
        public bool error { get; set; }
        /// 錯誤訊息
        public string errormessage { get; set; }
    }

    /// <summary>
    /// 密碼錯誤內容
    /// </summary>
    public class AccountError
    {
        public AccountError()
        {
            this.accountFormat = false;
            this.accountExisted = false;
            this.error = false;
        }
        //帳號格式
        public bool accountFormat { get; set; }
        //帳號存在
        public bool accountExisted { get; set; }
        //error
        public bool error { get; set; }
        /// 錯誤訊息
        public string errormessage { get; set; }
    }
}
