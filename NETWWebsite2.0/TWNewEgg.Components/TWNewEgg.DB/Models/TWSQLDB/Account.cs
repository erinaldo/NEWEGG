using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("account")]
    public class Account
    {
        public Account()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            ConfirmDate = defaultDate;
            LockedDate = defaultDate;
            StatusDate = defaultDate;
            Registeron = defaultDate;
            Loginon = defaultDate;
            CreateDate = defaultDate;
            UpdateDate = defaultDate;

            this.AgreePaper = 0;
            this.RememberMe = 0;
            this.MessagePaper = 0;
            this.GuestLogin = 0;
            this.ReceiveEDM = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("姓名")]
        [MaxLength(30, ErrorMessage = "姓名請勿輸入超過5個字元")]
        public string Name { get; set; }
        [Required]
        [DisplayName("設定密碼")]
        //[MaxLength(8, ErrorMessage = "密碼不得超過8個字元")]
        //[DataType(DataType.Password)]
        public string PWD { get; set; }
        public string PWDtxt { get; set; }
        public string PWDenId { get; set; }
        [DisplayName("網路暱稱")]
        [MaxLength(30, ErrorMessage = "網路暱稱請勿輸入超過10個字元")]
        public string Nickname { get; set; }
        public string NO { get; set; }
        public Nullable<int> Sex { get; set; }
        public Nullable<int> Type { get; set; }
        public string Birthday { get; set; }
        [Required]
        [DisplayName("設定會員帳號(e-mail)")]
        [Description("我們直接以 Email 當成會員的登入帳號")]
        [MaxLength(250, ErrorMessage = "Email地址長度無法超過250個字元")]
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Loc { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        public string TelDay { get; set; }
        public string TelNight { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public string ConfirmCode { get; set; }
        public Nullable<int> Subscribe { get; set; }
        public string ACTName { get; set; }
        public Nullable<int> Degree { get; set; }
        public Nullable<int> Income { get; set; }
        public Nullable<int> Job { get; set; }
        public Nullable<int> Marrige { get; set; }
        public string ServerName { get; set; }
        public Nullable<int> Chkfailcnt { get; set; }
        public Nullable<System.DateTime> LockedDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNote { get; set; }
        public string Note { get; set; }
        [DisplayName("更改密碼連結")]
        public string NewLinks { get; set; }
        [DisplayName("登入狀態")]
        public Nullable<int> LoginStatus { get; set; }
        [DisplayName("請輸入右邊圖片中的數字或英文")]
        [DataType(DataType.Text)]
        public string ValidateCode { get; set; }
        [DisplayName("會員註冊時間")]
        public Nullable<System.DateTime> Registeron { get; set; }
        [DisplayName("會員登入時間")]
        public Nullable<System.DateTime> Loginon { get; set; }
        [DisplayName("記住我的帳號和密碼，下次自動登入")]
        public Nullable<int> RememberMe { get; set; }
        [DisplayName("我已閱讀完畢，並同意會員條款(需勾選才能加入會員)")]
        public Nullable<int> AgreePaper { get; set; }
        [DisplayName("我要訂閱優惠訊息和電子報")]
        public Nullable<int> MessagePaper { get; set; }
        public string CreateUser { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string FacebookUID { get; set; }
        public Nullable<int> Istosap { get; set; }
        public Nullable<int> MemberAgreement { get; set; }
        public string ActionCode { get; set; }
        public int GuestLogin { get; set; }
        public int ReceiveEDM { get; set; }
        public string InvoiceCarrierReturn { get; set; }
    }
}