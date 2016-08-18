using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserCheckStatus
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 使用者亂數  (not null)
        /// </summary>
        public string RanCode { get; set; }
    }
}
