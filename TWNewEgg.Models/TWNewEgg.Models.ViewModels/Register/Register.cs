using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.Models.ViewModels.Register
{
    public class RegisterVM
    {
        public RegisterVM()
        {
            this.GuestLogin = 0;
            this.AgreePaper = 0;
            this.MessagePaper = 0;
        }

        public enum SetSex
        {
            Male = 1,
            Female = 0
        }

        // 密碼
        public string PWD { get; set; }
        // 密碼
        public string PWDtxt { get; set; }
        // 確認密碼
        public string confirmPWD { get; set; }
        // Email,帳號
        public string Email { get; set; }
        // 行動電話
        public string Mobile { get; set; }
        // 驗證碼
        public bool securitycode { get; set; }
        // 稱謂
        public Nullable<int> Sex { get; set; }
        // 姓氏
        public string Lastname { get; set; }
        // 名稱
        public string Firstname { get; set; }
        // 生日
        public Nullable<DateTime> Birthday { get; set; }
        // 我已閱讀完畢，並同意會員條款(需勾選才能加入會員)
        public Nullable<int> AgreePaper { get; set; }
        // 我要訂閱優惠訊息和電子報
        public Nullable<int> MessagePaper { get; set; }

        public int GuestLogin { get; set; }
        // login type, ex:facebook
        public string AUtype { get; set; }
        // 舊密碼
        public string OldPWD { get; set; }
    }
}
