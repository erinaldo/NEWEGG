using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Seller_FinanMaster")]
    public class Seller_FinanMaster
    {
        #region -- FinanStatusType 結帳狀態 --
        /// <summary>
        /// 結帳狀態，使用英文字存檔
        /// </summary>
        public enum FinanStatusType
        {
            S = 1,//Start對帳單初始狀態
            V = 2,//Vendor押發票時變動
            C = 3//Close關帳
        }

        public Dictionary<string, string> FinanstatusType
        {
            get
            {
                Dictionary<string, string> col = new Dictionary<string, string>();
                //col.Add("S", "Start");
                //col.Add("V", "Vendor");
                //col.Add("C", "Close");
                col.Add("S", "已結算");
                col.Add("V", "已開發票");
                col.Add("C", "已匯款");
                
                return col;
            }
        }
        #endregion

        #region-- 開放狀態 --
        public Dictionary<string, string> IsopenType
        {
            get
            {
                Dictionary<string, string> col = new Dictionary<string, string>();
                col.Add("Y", "已開放");
                col.Add("N", "未開放");

                return col;
            }
        }
        #endregion

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string FinanStatus { get; set; }
        [Key]
        public string SettlementID { get; set; }
        public int SellerID { get; set; }
        public string IsOpen { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public System.DateTime SettleDate { get; set; }
        public string SettleMonth { get; set; }
        public Nullable<System.DateTime> InvoDate { get; set; }
        public string InvoNumber { get; set; }
        public Nullable<System.DateTime> RemitDate { get; set; }
        public string BaseCurrency { get; set; }
        public string SettleCurrency { get; set; }
        public decimal POPrice { get; set; }
        public decimal POTax { get; set; }
        public decimal RMAPrice { get; set; }
        public decimal RMATax { get; set; }
        public decimal WarehousePrice { get; set; }
        public decimal WarehouseTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal PaymentAmount { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public string InUserID { get; set; }

    }
}
