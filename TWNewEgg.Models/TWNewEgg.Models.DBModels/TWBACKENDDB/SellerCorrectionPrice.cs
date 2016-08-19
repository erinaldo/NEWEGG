using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    /// <summary>
    /// 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
    /// </summary>
    [Table("Seller_CorrectionPrice")]
    public class SellerCorrectionPriceDB
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 商家編號;供應商代號
        /// </summary>
        [DisplayName("賣場品編號")]
        [Column("SellerID")]
        public int SellerID { get; set; }


        /// <summary>
        /// 主因
        /// </summary>
        [DisplayName("主因")]
        [Column("Subject")]
        public string Subject { get; set; }


        /// <summary>
        /// 說明
        /// </summary>
        [DisplayName("說明")]
        [Column("Description")]
        public string Description { get; set; }


        /// <summary>
        /// 對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票        
        /// </summary>
        [DisplayName("對帳單調整項狀態")]
        [Column("FinanStatus")]
        public string FinanStatus { get; set; }

        /// <summary>
        /// 調整金額(含稅)   
        /// </summary>
        [DisplayName("調整金額(含稅)")]
        [Column("TotalAmount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 調整金額(未稅)      
        /// </summary>
        [DisplayName("調整金額(未稅)")]
        [Column("PurePrice")]
        public decimal PurePrice { get; set; }

        /// <summary>
        /// 稅額
        /// </summary>
        [DisplayName("稅額")]
        [Column("Tax")]
        public decimal Tax { get; set; }

        /// <summary>
        /// 對帳單編號
        /// </summary>
        [DisplayName("對帳單編號")]
        [Column("SettlementID")]
        public string SettlementID { get; set; }

        /// <summary>
        /// 創建者
        /// </summary>
        [DisplayName("創建者")]
        [Column("CreateUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        [DisplayName("創建日期")]
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        [DisplayName("最後修改者")]
        [Column("UpdateUser")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        [DisplayName("最後修改日期")]
        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }


    }
}
