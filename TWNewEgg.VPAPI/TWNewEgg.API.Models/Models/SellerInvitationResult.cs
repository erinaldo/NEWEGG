using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SellerInvitationResult
    {
        /// <summary>
        /// SellerID  (not null)
        /// </summary>
        public int SellerID { set; get; }

        /// <summary>
        /// 商家電子郵件  (not null)
        /// </summary>      
        public string SellerEmail { set; get; }

        /// <summary>
        /// 商家權限  (not null)
        /// </summary>
        public string Authority { set; get; }

        /// <summary>
        /// 商家區域
        /// </summary>
        public int SellerCountryCode { set; get; }

        /// <summary>
        /// 商家語言
        /// </summary>
        public int LanguageCode { set; get; }

        /// <summary>
        /// Invite Seller for (目前只有for seller portal)
        /// </summary>
        public int InviteSellerFor { set; get; }

        /// <summary>
        /// Account Initial Status
        /// </summary>
        public string SellerStatus { set; get; }

        /// <summary>
        /// AccountTypeCode (not null)
        /// </summary>
        public int AccountTypeCode { set; get; }

        /// <summary>
        /// 商家收費種類
        /// </summary>
        public string ChargeType { set; get; }

        /// <summary>
        /// 佣金
        /// </summary>
        public List<CommissionRateInfo> CommissionRate { set; get; }

        public class CommissionRateInfo
        {
            /// <summary>
            /// 類別代號
            /// </summary>
            public int CategoryID { set; get; }

            /// <summary>
            /// 佣金
            /// </summary>
            public decimal Commission { set; get; }

        }
    }
}
