using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UserCreation
    {
        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// SellerID  (not null)
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// User群組  (not null)
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 建立者UserID  (not null)
        /// </summary>
        public int InUserID { get; set; }

        /// <summary>
        /// 權限類別  (not null)
        /// </summary>
        public string PurviewType { get; set; }
    }
}
