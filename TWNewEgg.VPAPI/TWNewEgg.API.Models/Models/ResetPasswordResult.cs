using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class ResetPasswordResult
    {

        /// <summary>
        /// 使用者ID  (not null)
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 寄送時間  (not null)
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 信件主旨  (not null)
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 信件內容  (not null)
        /// </summary>
        public string MailContent { get; set; }

    }
}
