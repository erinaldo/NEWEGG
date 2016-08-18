using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Seller
{

    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
    /// 資料查詢
    /// </summary>
    public class SellerCorrectionPriceSearchDM 
    {
        //供應商對帳單新增調整項目
        public SellerCorrectionPriceSearchDM()
        {
            this.SellerIDs = new List<int>();
            this.SettlementIDs = new List<string>();
            this.FinanStatus = string.Empty;
            this.ShowAll = string.Empty;
            this.OrderByID = false;
            this.OrderByShoworder = false;
            this.DescByCreateDate = false;
            
        }

        /// <summary>
        /// 商家編號;供應商代號
        /// </summary>
        public List<int> SellerIDs { get; set; }

        /// <summary>
        /// 對帳單編號
        /// </summary>
        public List<string> SettlementIDs { get; set; }

        /// <summary>
        /// 對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票        
        /// </summary>
        public string FinanStatus { get; set; }

        /// <summary>
        /// 是否顯示, 顯示:show|1, 不顯示:hide|0
        /// </summary>
        public string ShowAll { get; set; }

        /// <summary>
        /// 以流水號排序---------------------add by bruce 20160720
        /// </summary>
        public bool OrderByID { get; set; }

        /// <summary>
        /// 以建檔日期排序降序 -------------------add by bruce 20160720
        /// </summary>
        public bool DescByCreateDate { get; set; }


        /// <summary>
        /// 以建檔日期排序升序-------------------add by bruce 20160720
        /// </summary>
        public bool OrderByShoworder { get; set; }

       

    }

}
