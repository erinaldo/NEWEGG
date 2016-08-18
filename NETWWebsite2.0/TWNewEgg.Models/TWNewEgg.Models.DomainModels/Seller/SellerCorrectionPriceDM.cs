using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Seller
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
    /// </summary>
    public class SellerCorrectionPriceDM
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 商家編號;供應商代號
        /// </summary>
        public int SellerID { get; set; }


        /// <summary>
        /// 主因
        /// </summary>
        public string Subject { get; set; }


        /// <summary>
        /// 說明
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票        
        /// </summary>
        public string FinanStatus { get; set; }

        /// <summary>
        /// 調整金額(含稅)   
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 調整金額(未稅)      
        /// </summary>
        public decimal PurePrice { get; set; }

        /// <summary>
        /// 稅額
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// 對帳單編號
        /// </summary>
        public string SettlementID { get; set; }

        /// <summary>
        /// 創建者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }


    }
}
