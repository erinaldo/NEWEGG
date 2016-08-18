using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Account
{
    public class AccountVM
    {
        public int ID { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        public string PWD { get; set; }
        //public string PWDtxt { get; set; }
        //public string PWDenId { get; set; }
        public string Nickname { get; set; }
        //public string NO { get; set; }
        public Nullable<int> Sex { get; set; }
        //public Nullable<int> Type { get; set; }
        public string Birthday { get; set; }
        [DisplayName("設定會員帳號(e-mail)")]
        [Description("我們直接以 Email 當成會員的登入帳號")]
        public string Email { get; set; }
        //public string Email2 { get; set; }
        public string Loc { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        public string TelDay { get; set; }
        //public string TelNight { get; set; }
        public string Mobile { get; set; }
        //public string Fax { get; set; }
        //public Nullable<System.DateTime> ConfirmDate { get; set; }
        //public string ConfirmCode { get; set; }
        //public Nullable<int> Subscribe { get; set; }
        //public string ACTName { get; set; }
        //public Nullable<int> Degree { get; set; }
        //public Nullable<int> Income { get; set; }
        //public Nullable<int> Job { get; set; }
        //public Nullable<int> Marrige { get; set; }
        //public string ServerName { get; set; }
        //public Nullable<int> Chkfailcnt { get; set; }
        //public Nullable<System.DateTime> LockedDate { get; set; }
        //public Nullable<int> Status { get; set; }
        //public Nullable<System.DateTime> StatusDate { get; set; }
        //public string StatusNote { get; set; }
        //public string Note { get; set; }
        //[DisplayName("更改密碼連結")]
        //public string NewLinks { get; set; }
        //[DisplayName("登入狀態")]
        //public Nullable<int> LoginStatus { get; set; }
        //[DisplayName("會員註冊時間")]
        //public Nullable<System.DateTime> Registeron { get; set; }
        //[DisplayName("會員登入時間")]
        public Nullable<System.DateTime> Loginon { get; set; }
        //[DisplayName("記住我的帳號和密碼，下次自動登入")]
        //public Nullable<int> RememberMe { get; set; }
        //[DisplayName("我已閱讀完畢，並同意會員條款(需勾選才能加入會員)")]
        //public Nullable<int> AgreePaper { get; set; }
        //[DisplayName("我要訂閱優惠訊息和電子報")]
        //public Nullable<int> MessagePaper { get; set; }
        //public string CreateUser { get; set; }
        //public string FacebookUID { get; set; }
        //public Nullable<int> Istosap { get; set; }
        public Nullable<int> MemberAgreement { get; set; }
        //public string ActionCode { get; set; }
        public int GuestLogin { get; set; }
        public int ReceiveEDM { get; set; }
    }
}
