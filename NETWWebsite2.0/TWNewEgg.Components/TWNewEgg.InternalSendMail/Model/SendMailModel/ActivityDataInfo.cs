using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class ActivityDataInfo
    {
        public ActivityDataInfo()
        {
            this.Email = string.Empty;
            this.ActivityName = string.Empty;
            this.ActivityNo = string.Empty;
            this.IsEffective = false;
            this.ErrorMessage = string.Empty;
        }
        /// <summary>
        /// 使用者Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 活動名稱
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 是否在活動有效期限內
        /// </summary>
        public bool IsEffective { get; set; }
        /// <summary>
        /// 活動序號
        /// </summary>
        public string ActivityNo { get; set; }
        /// <summary>
        /// 活動起始日期
        /// </summary>
        public Nullable<DateTime> StartDate { get; set; }
        /// <summary>
        /// 活動結束日期
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }
        /// <summary>
        /// 活動兌換截止日期
        /// </summary>
        public Nullable<DateTime> Deadline { get; set; }
        /// <summary>
        /// 錯誤回傳訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
