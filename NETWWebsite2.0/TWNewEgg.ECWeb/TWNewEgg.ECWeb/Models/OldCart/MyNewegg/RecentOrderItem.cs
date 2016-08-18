using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class RecentOrderItem : IOrderItems
    {
        public enum statusList : int
        {
            付款失敗已取消 = -4,
            無法配達 = -3,
            被動取消 = -2,
            已取消 = -1,
            確認中 = 0,
            訂單成立 = 1,
            空運中 = 2,
            待出貨 = 3,
            已出貨 = 4,
            已配達 = 5,
            退貨處理中 = 6,
            付款成功 = 30,
            貨到付款 = 31,
            訂單異常 = 32,
            付款失敗 = 33,
            訂購成功 = 34

        }

        public enum retStatusList : int
        {
            退貨處理中 = 0,
            退貨中 = 1,
            完成退貨 = 2,

            退款中 = 3,
            完成退款 = 4,

            退貨異常 = 5,
            退款異常 = 6,

            退貨取消 = 7,
            退款取消 = 8,
        }
        public bool fromCart { get; set; }
        public string prblm_prblmcode { get; set; }
        public decimal ItemPriceSum { get; set; }
        public decimal serviceFees { get; set; }
        public decimal? tax { get; set; }
        public DateTime? shippingDay { get; set; }
        public DateTime? proc_out { get; set; }
        public string TrackNo { get; set; }
        public int? salesorder_delivtype { get; set; }
        public string process_delivno { get; set; }
        public int? process_deliver { get; set; }
        //public string delivnoLink
        //{
        //    get
        //    {
        //        if (salesorder_delivtype.HasValue)
        //        {
        //            if (salesorder_delivtype == 3)
        //            {
        //                return "http://web.ucf.com.tw/wiznet/querycwb.php";
        //            }
        //            else
        //            {
        //                return "http://www.hct.com.tw/SearchGoods.aspx";
        //            }
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }

        //}
        public string delivnoLink { get; set; }
        public int salesorder_groupid { get; set; }
        public int status { get; set; }
        public int product_sellerid { get; set; }
        public int seller_country { get; set; }
        public string country_name { get; set; }
        public int? retStatus { get; set; }
        public Enum statusDesciption
        {
            get
            {
                if (status == 6)
                {
                    return (retStatusList)(retStatus ?? 0);
                }
                else
                {
                    return (statusList)status;
                }
            }
        }



        public string Code { get; set; }
        public string SalesorderCode { get; set; }
        public int ItemID { get; set; }
        public int ItemlistID { get; set; }
        public int ProductID { get; set; }
        public int ProductlistID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Priceinst { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> Pricecoupon { get; set; }
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
        public Nullable<int> AdjPrice { get; set; }
        public string ActID { get; set; }
        public Nullable<int> ActtkOut { get; set; }
        public Nullable<int> ProdcutCostID { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public decimal? ShippingExpense { get; set; }
        public decimal? ServiceExpense { get; set; }
        public decimal? DisplayPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<DateTime> InvoiceInDate { get; set; }
        public decimal InstallmentFee { get; set; }
        public decimal ApportionedAmount { get; set; }
        public string O2OShopDeliveryDate { get; set; }
        public RecentOrderItem(SalesOrderItem soi)
        {
            Type tt = typeof(SalesOrderItem);
            Type rr = typeof(RecentOrderItem);
            var propNames = tt.GetProperties().Select(p => p.Name).ToList();
            foreach (var property in rr.GetProperties())
            {
                if (propNames.Contains(property.Name))
                {
                    rr.GetProperty(property.Name).SetValue(this, tt.GetProperty(property.Name).GetValue(soi));
                }
            }

        }
        public RecentOrderItem(Process proc)
        {
            this.shippingDay = proc.ProcOut;
            this.Code = proc.ID;
            this.SalesorderCode = proc.CartID;
            this.ItemID = proc.StoreID ?? 0;
            this.ItemlistID = proc.ItemlistID ?? 0;
            this.ProductID = proc.ProductID ?? 0;
            this.ProductlistID = proc.ProductlistID ?? 0;
            this.Name = proc.Title;
            this.Price = proc.Price ?? 0;
            this.Priceinst = proc.Priceinst;
            this.Qty = proc.Qty ?? 0;
            this.Pricecoupon = proc.Pricecoupon;
            this.RedmtkOut = proc.RedmtkOut;
            this.RedmBLN = proc.RedmBLN;
            this.Redmfdbck = proc.Redmfdbck;
            this.Status = proc.Status ?? 0;
            this.StatusNote = proc.StatusNote;
            this.Date = proc.Date;
            this.process_deliver = proc.Deliver;
            this.Attribs = proc.Attribs;
            this.Note = proc.OrderNote;
            this.WftkOut = proc.WftkOut;
            this.WfBLN = proc.WfBLN;
            this.AdjPrice = proc.ADJPrice;
            this.ActID = proc.ACTID;
            this.ActtkOut = proc.ActtkOut;
            this.CreateUser = proc.PROCUser;
            this.CreateDate = proc.CreateDate ?? DateTime.MinValue;
            this.Updated = proc.Updated;
            this.UpdateDate = proc.UpdateDate;
            this.UpdateUser = proc.UpdateUser;
            this.process_delivno = proc.DelivNO;
            this.ShippingExpense = proc.ShippingExpense;
            this.ServiceExpense = proc.ServiceExpense;
            this.DisplayPrice = proc.DisplayPrice;
            this.DiscountPrice = (proc.DiscountPrice == null) ? 0 : proc.DiscountPrice;
            this.tax = proc.Tax;
            this.WarehouseID = proc.WarehouseID;
            this.InstallmentFee = proc.InstallmentFee;
            this.ApportionedAmount = proc.ApportionedAmount;
        }
        public RecentOrderItem() { }
    }
}