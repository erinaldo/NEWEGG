using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;

namespace TWNewEgg.API.Models
{
    public class VM_Seller_BasicInfo : Seller_BasicInfo
    {
        //地區 中文
        public string SellerCountryCodeName { get; set; }

        // 是否可以再點擊再邀請
        public bool SellerUserStatus { get; set; }

        public VM_Seller_BasicInfo()
        {
            SellerUserStatus = false;
        }
    }

    /// <summary>
    /// 商家編號及商家名稱
    /// </summary>
    public class Seller_ID_Name
    {
        /// <summary>
        ///  商家編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string Name { get; set; }
    }
}
