using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserCheckStatusResult
    {
        /// <summary>
        /// 使用者ID  (not null)
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 使用者狀態  (not null)
        /// </summary>
        public string Status { get; set; }
    }
}
