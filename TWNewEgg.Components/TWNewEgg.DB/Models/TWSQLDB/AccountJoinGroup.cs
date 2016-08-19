using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("AccountJoinGroup")]
    public class AccountJoinGroup
    {
        public enum RegisterStatus
        {
            // 推薦人已送出邀請
            已送出邀請 = 0,
            // 被推薦者已註冊成為會員
            已註冊成為會員 = 1,
            推薦完成詳情請見活動規則 = 2,
        }

        public AccountJoinGroup()
        {
            DateTime _dateTime = new DateTime();
            _dateTime = DateTime.Now;
            CreateDate = _dateTime;
            Status = 0;
            ReceivedMail = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Introduction_time { get; set; }
        public string New_Account { get; set; }
        public string Old_Account { get; set; }
        public string SO_Number { get; set; }
        public Nullable<bool> RegisterSuccess { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> OldAccountGetCouponDate { get; set; }
        public Nullable<System.DateTime> NewAccountGetCouponDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<bool> ReceivedMail { get; set; }
    }
}
