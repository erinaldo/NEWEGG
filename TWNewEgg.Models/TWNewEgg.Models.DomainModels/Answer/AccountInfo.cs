using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Answer
{
    public class AccountInfo
    {
        public int ID { get; set; }

  
        public string Name { get; set; }

        //[MaxLength(8, ErrorMessage = "密碼不得超過8個字元")]
        //[DataType(DataType.Password)]
        public string PWD { get; set; }
        public string PWDtxt { get; set; }
        public string PWDenId { get; set; }
     
        public string Nickname { get; set; }
        public string NO { get; set; }
        public Nullable<int> Sex { get; set; }
        public Nullable<int> Type { get; set; }
        public string Birthday { get; set; }
      
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
       
        public string NewLinks { get; set; }

        public Nullable<int> LoginStatus { get; set; }
        //[Required(ErrorMessage = "{0}")]
        //[DisplayName("請輸入右邊圖片中的數字或英文")]
        //[DataType(DataType.Text)]
        public string ValidateCode { get; set; }
  
        public Nullable<System.DateTime> Registeron { get; set; }

        public Nullable<System.DateTime> Loginon { get; set; }

        public Nullable<int> RememberMe { get; set; }

        public Nullable<int> AgreePaper { get; set; }

        public Nullable<int> MessagePaper { get; set; }
        public string CreateUser { get; set; }
  
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
    }
}
