using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SendInvitationEmail
    {
        /// <summary>
        /// 使用者ID  (not null)
        /// </summary>
        //public int UserID { get; set; }
        
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 新使用者亂數  (not null)
        /// </summary>
        //public string RanNum { get; set; }

    }
}
