using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserLogin
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 密碼  (not null)
        /// </summary>
        public string Password { get; set; }

        public string VendorSeller { get; set; }
    }
}
