using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Process")]
    public partial class Process
    {
        /// <summary>
        /// 購物子單序號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        [ForeignKey("cart")]
        public string CartID { get; set; }
        public virtual Cart cart { get; set; }

        public Nullable<int> ProductID { get; set; }
        public Nullable<int> ProductlistID { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string ACTID { get; set; }
        public Nullable<int> ActtkOut { get; set; }

        public Nullable<int> ItemlistID { get; set; }
        public string OrderItemCode { get; set; }
        public Nullable<int> DealType { get; set; }
        public Nullable<int> DealID { get; set; }
        public string Title { get; set; }
        public string Attribs { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Priceinst { get; set; }
        /// <summary>
        /// Coupon金額
        /// </summary>
        public Nullable<decimal> Pricecoupon { get; set; }
        public string Coupons { get; set; }
        public Nullable<int> RedmtkOut { get; set; }
        public Nullable<int> RedmBLN { get; set; }
        public Nullable<int> Redmfdbck { get; set; }
        public Nullable<int> WftkOut { get; set; }
        public Nullable<int> WfBLN { get; set; }
        /// <summary>
        /// 稅別
        /// </summary>
        public Nullable<int> TaxType { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<int> Qty { get; set; }
        /// <summary>
        /// 公司統編抬頭
        /// </summary>
        public string InvoTitle { get; set; }
        public string InvoTitleUser { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        /// <summary>
        /// 店配-出貨次數
        /// </summary>
        public Nullable<int> STDELVCount { get; set; }
        /// <summary>
        /// 店配-出貨檔轉出日
        /// </summary>
        public Nullable<System.DateTime> STDSTExportDate { get; set; }
        /// <summary>
        /// 店配-貨到門市預計日
        /// </summary>
        public Nullable<System.DateTime> StarvlDate { get; set; }
        /// <summary>
        /// 店配-到店簡訊日期
        /// </summary>
        public Nullable<System.DateTime> StarvDateReal { get; set; }
        /// <summary>
        /// 店配-出貨確認日
        /// </summary>
        public Nullable<System.DateTime> STCFMDate { get; set; }
        /// <summary>
        /// 店配-凱耀結算日(核帳)
        /// </summary>
        public Nullable<System.DateTime> STKYCLRDate { get; set; }
        /// <summary>
        /// 店配-供應商結算日(對帳單)
        /// </summary>
        public Nullable<System.DateTime> STSPCLRDate { get; set; }
        public Nullable<System.DateTime> ProcOrder { get; set; }
        public Nullable<System.DateTime> ProcIn { get; set; }
        public Nullable<System.DateTime> WillShipping { get; set; }
        public Nullable<System.DateTime> ProcOut { get; set; }
        public Nullable<System.DateTime> Received { get; set; }
        /// <summary>
        /// User選擇到貨時間點
        /// </summary>
        public string OrderNote { get; set; }
        public string InNote { get; set; }
        public string OutNote { get; set; }
        public string ReceivedNote { get; set; }
        public Nullable<int> SPID { get; set; }
        public string SPNO { get; set; }
        public Nullable<int> ShipperID { get; set; }
        public Nullable<int> ADJPrice { get; set; }
        public Nullable<int> Priveinst { get; set; }
        public Nullable<System.DateTime> ApplySheet { get; set; }
        public string ApplySheetNote { get; set; }
        public Nullable<int> PurhItemID { get; set; }
        public Nullable<int> CheckInItemID { get; set; }
        public Nullable<int> RetgoodID { get; set; }
        public Nullable<int> Refund2cID { get; set; }
        public Nullable<int> DeliveryCheck { get; set; }
        public string PROCUser { get; set; }
        public Nullable<int> Parent { get; set; }
        public Nullable<int> ScmenaBled { get; set; }
        public Nullable<int> QtyProccncl { get; set; }
        public Nullable<int> QtyPurh { get; set; }
        public Nullable<int> QtySPOut { get; set; }
        public Nullable<int> QtyCheckIn { get; set; }
        public Nullable<int> QtyStckresv { get; set; }
        public Nullable<int> Qtypkt { get; set; }
        public Nullable<int> QtyDelivery { get; set; }
        public Nullable<int> QtyRETGood { get; set; }
        public Nullable<int> QtyCHGood { get; set; }
        public Nullable<int> AmutRefund2c { get; set; }
        public Nullable<System.DateTime> WorkHource { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string DelivNO { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string SN { get; set; }
        public Nullable<decimal> DisplayPrice { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }
        /// <summary>
        /// 國際運費收入
        /// </summary>
        public Nullable<decimal> ShippingExpense { get; set; }
        /// <summary>
        /// 服務費收入
        /// </summary>
        public Nullable<decimal> ServiceExpense { get; set; }
        public Nullable<int> UpdateSapStatus { get; set; }
        public string XDDocNumber { get; set; }
        public string XDDocNumber_RMA { get; set; }
        public string XQDocNumber { get; set; }
        public string XQDocNumber_RMA { get; set; }
        public string XIDocNumber { get; set; }
        public string XIDocNumber_RMA { get; set; }
        public string XVInDocNumber { get; set; }
        public string XVDocNumber { get; set; }
        public string XVDocNumber_RMA { get; set; }
        public string SettlementID { get; set; }  //2013.11.1 add columns by Ice   //2013.11.4 int => string modify by Ice
        /// <summary>
        /// 貨運公司代碼
        /// </summary>
        public Nullable<int> Deliver { get; set; }
        /// <summary>
        /// 進口稅賦
        /// </summary>
        public Nullable<decimal> Tax { get; set; }
        public Nullable<int> WarehouseID { get; set; } //2013.12.20 add column by Bill
        public Nullable<decimal> ItemPriceSum { get; set; }
        /// <summary>
        /// 分期利息
        /// </summary>
        public decimal InstallmentFee { get; set; }
        public string IsNew { get; set; }
        /// <summary>
        /// 滿額贈分攤金額
        /// </summary>
        public decimal ApportionedAmount { get; set; }
        public Nullable<decimal> SupplyShippingCharge { get; set; }
        public Nullable<int> itemCategory { get; set; } //2015.04.20 add by Bill 紀錄購買的Item Category
    }
}
