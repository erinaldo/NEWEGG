using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class MailTransformation
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 信件內容  (not null)
        /// </summary>
        public string MailContent { get; set; }

        //2014.1.27 改掉信件主旨寫死部分 by Smoke
        /// <summary>
        /// 信件主旨
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 密件副本
        /// </summary>
        public string RecipientBcc { get; set; }
    }
}
