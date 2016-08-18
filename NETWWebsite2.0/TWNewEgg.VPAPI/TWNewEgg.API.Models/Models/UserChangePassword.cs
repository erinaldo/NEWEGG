using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserChangePassword
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 新使用者亂數  (啟用時 not null)
        /// </summary>
        public string RanCode { get; set; }

        /// <summary>
        /// 舊密碼  (更改密碼時 not null)
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密碼  (not null)
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Update UserID  (not null)
        /// </summary>
        public int UpdateUserID { get; set; }
    }
}
