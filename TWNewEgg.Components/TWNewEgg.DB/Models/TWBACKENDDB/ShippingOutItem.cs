using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("shippingoutitem")]
    public partial class ShippingOutItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public string ShippingOutCode { get; set; }
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
        public Nullable<decimal> Pricecoupon { get; set; }
        public string Coupons { get; set; }
        public Nullable<int> RedmtkOut { get; set; }
        public Nullable<int> RedmBLN { get; set; }
        public Nullable<int> Redmfdbck { get; set; }
        public Nullable<int> WftkOut { get; set; }
        public Nullable<int> WfBLN { get; set; }
        public Nullable<int> TaxType { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<int> Qty { get; set; }
        public string InvoTitle { get; set; }
        public string InvoTitleUser { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> STDELVCount { get; set; }
        public Nullable<System.DateTime> STDSTExportDate { get; set; }
        public Nullable<System.DateTime> StarvlDate { get; set; }
        public Nullable<System.DateTime> StarvDateReal { get; set; }
        public Nullable<System.DateTime> STCFMDate { get; set; }
        public Nullable<System.DateTime> STKYCLRDate { get; set; }
        public Nullable<System.DateTime> STSPCLRDate { get; set; }
        public Nullable<System.DateTime> ProcOrder { get; set; }
        public Nullable<System.DateTime> ProcIn { get; set; }
        public Nullable<System.DateTime> WillShipping { get; set; }
        public Nullable<System.DateTime> ProcOut { get; set; }
        public Nullable<System.DateTime> Received { get; set; }
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
        public Nullable<decimal> ShippingExpense { get; set; }
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
        public Nullable<int> Deliver { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<int> WarehouseID { get; set; } //2013.12.20 add column by Bill
        public Nullable<decimal> ItemPriceSum { get; set; }
        // 分期利息
        //public decimal InstallmentFee { get; set; }
    }
}
