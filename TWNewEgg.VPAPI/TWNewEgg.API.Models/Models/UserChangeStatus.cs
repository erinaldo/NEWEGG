using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserChangeStatus
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 狀態  (not null)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Update UserID  (not null)
        /// </summary>
        public int UpdateUserID { get; set; }
    }
}
