using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("purchaseorderitem")]
    public partial class PurchaseOrderitemTWBACK
    {
        public enum status
        {
            初始狀態 = 99,
            已成立 = 0,
            取消 = 1
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        public string PurchaseorderCode { get; set; }
        public string SellerorderCode { get; set; }
        public int ItemID { get; set; }
        public int ItemlistID { get; set; }
        public int ProductID { get; set; }
        public int ProductlistID { get; set; }
        public Nullable<int> SellerID { get; set; }
        public string Name { get; set; }
        public Nullable<int> SourceCurrencyID { get; set; }
        public decimal SourcePrice { get; set; }
        public Nullable<int> LocalCurrencyID { get; set; }
        public decimal LocalPrice { get; set; }
        public Nullable<decimal> LocalPriceinst { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> LocalPriceCoupon { get; set; }
        public string Coupons { get; set; }
        public Nullable<int> RedmtkOut { get; set; }
        public Nullable<int> RedmBLN { get; set; }
        public Nullable<int> Redmfdbck { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Attribs { get; set; }
        public string Note { get; set; }
        public Nullable<int> WftkOut { get; set; }
        public Nullable<int> WfBLN { get; set; }
        public Nullable<int> ADJPrice { get; set; }
        public string ACTID { get; set; }
        public Nullable<int> ActtkOut { get; set; }
        public Nullable<int> ProdcutCostID { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string InvoiceNO { get; set; }
        public string BoxNO { get; set; }
        public Nullable<decimal> BoxWeight { get; set; }
        public Nullable<int> BoxCurrency { get; set; }
        public Nullable<decimal> DimLenght { get; set; }
        public Nullable<decimal> DimWidth { get; set; }
        public Nullable<decimal> DimHieght { get; set; }
        public Nullable<int> DimCurrency { get; set; }
        public Nullable<decimal> ImportCost { get; set; }
        public string CCCCode { get; set; }
        public Nullable<decimal> DutyRate { get; set; }
        public Nullable<decimal> ProductTax { get; set; }
        public Nullable<decimal> FullTaxValue { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public Nullable<decimal> TaxandDuty { get; set; }
        public Nullable<decimal> Customs_Charge { get; set; }
        public Nullable<decimal> TradeServiceCharges { get; set; }
        public Nullable<decimal> SupplyShippingCharge { get; set; }
        /// <summary>
        /// 供應商料號
        /// </summary>
        public string SellerProductID { get; set; }
    }
}
