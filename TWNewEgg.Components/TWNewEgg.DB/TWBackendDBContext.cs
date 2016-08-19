using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TWNewEgg.DB.TWBACKENDDB.Models;

namespace TWNewEgg.DB
{
    public class TWBackendDBContext : DbContext
    {
        public TWBackendDBContext()
            : base("name=TWBackendDBConnection")
        {
        }

        public System.Data.Entity.DbSet<Cart> Cart { get; set; }
        public System.Data.Entity.DbSet<Credit> Credit { get; set; }
        public System.Data.Entity.DbSet<Process> Process { get; set; }
        public System.Data.Entity.DbSet<FuncMenu> FuncMenu { get; set; }
        public System.Data.Entity.DbSet<User> User { get; set; }
        public System.Data.Entity.DbSet<Role> Role { get; set; }
        public System.Data.Entity.DbSet<GroupRoles> Grouproles { get; set; }
        public System.Data.Entity.DbSet<UserFunc> UserFunc { get; set; }
        public System.Data.Entity.DbSet<RoleFunc> RoleFunc { get; set; }
        public System.Data.Entity.DbSet<CreditAuth> CreditAuth { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrderTWBACK> PurchaseOrderTWBACK { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrdergroupTWBACK> PurchaseOrdergroupTWBACK { get; set; }
        public System.Data.Entity.DbSet<CurrencyTWBACK> CurrencyTWBACK { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrdergroupextTWBACK> PurchaseOrdergroupextTWBACK { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrderitemTWBACK> PurchaseOrderitemTWBACK { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrderitemexTWBACK> PurchaseOrderitemexTWBACK { get; set; }
        public System.Data.Entity.DbSet<Delivery> Delivery { get; set; }
        public System.Data.Entity.DbSet<DeliveryItem> DeliveryItem { get; set; }
        public System.Data.Entity.DbSet<Checkin> Checkin { get; set; }
        public System.Data.Entity.DbSet<Checkinitem> Checkinitem { get; set; }
        public System.Data.Entity.DbSet<Checkout> Checkout { get; set; }
        public System.Data.Entity.DbSet<Checkoutitem> Checkoutitem { get; set; }
        public System.Data.Entity.DbSet<CheckinV2> CheckinV2 { get; set; }
        public System.Data.Entity.DbSet<CheckinitemV2> CheckinitemV2 { get; set; }
        public System.Data.Entity.DbSet<CheckoutV2> CheckoutV2 { get; set; }
        public System.Data.Entity.DbSet<CheckoutitemV2> CheckoutitemV2 { get; set; }
        public System.Data.Entity.DbSet<POItemDetail> ProcessInfoView { get; set; }
        public System.Data.Entity.DbSet<ItemInStock> ItemInStock { get; set; }
        public System.Data.Entity.DbSet<ItemInStock_trans> ItemInStock_trans { get; set; }
        public System.Data.Entity.DbSet<ItemInStockV2> ItemInStockV2 { get; set; }
        public System.Data.Entity.DbSet<ItemInStock_transV2> ItemInStock_transV2 { get; set; }
        public System.Data.Entity.DbSet<Retgood> Retgood { get; set; }
        public System.Data.Entity.DbSet<InvoiceMaster> InvoiceMaster { get; set; }
        public System.Data.Entity.DbSet<InvoiceList> InvoiceList { get; set; }
        public System.Data.Entity.DbSet<SpexEdit> SpexEdit { get; set; }
        public System.Data.Entity.DbSet<SpexEditInvoice> SpexEditInvoice { get; set; }
        /*public System.Data.Entity.DbSet<Inventory> Inventory { get; set; }
        public System.Data.Entity.DbSet<Inventoryitem> Inventoryitem { get; set; }*/
        public System.Data.Entity.DbSet<refund2c> refund2c { get; set; }
        public System.Data.Entity.DbSet<Deliver> Deliver { get; set; }
        public System.Data.Entity.DbSet<CSApply> CSApply { get; set; }
        public System.Data.Entity.DbSet<CSApplyItem> CSApplyItem { get; set; }
        public System.Data.Entity.DbSet<ReversePurchaseOrder> ReversePurchaseOrder { get; set; }
        public System.Data.Entity.DbSet<ReversePurchaseOrderItem> ReversePurchaseOrderItem { get; set; }
        public System.Data.Entity.DbSet<Warehouse> Warehouse { get; set; }
        public System.Data.Entity.DbSet<ShippingOut> ShippingOut { get; set; }
        public System.Data.Entity.DbSet<ShippingOutItem> ShippingOutItem { get; set; }
        public System.Data.Entity.DbSet<OeyaIChannelsOrderInfo> OeyaIChannelsOrderInfo { get; set; }
        public System.Data.Entity.DbSet<CategoryAssociatedWithPM> CategoryAssociatedWithPM { get; set; }
        public System.Data.Entity.DbSet<API_Action> API_Action { get; set; }
        public System.Data.Entity.DbSet<API_User> API_User { get; set; }
        public System.Data.Entity.DbSet<API_Purview> API_Purview { get; set; }

        public System.Data.Entity.DbSet<CargoStatus> CargoStatus { get; set; }
        public System.Data.Entity.DbSet<CargoStatusTrack> CargoStatusTrack { get; set; }
        public System.Data.Entity.DbSet<JieMaiOrderInfo> JieMaiOrderInfo { get; set; }

        public System.Data.Entity.DbSet<SMSMemberGroup> SMSMemberGroup { get; set; }
        public System.Data.Entity.DbSet<SMSMember> SMSMember { get; set; }
        public System.Data.Entity.DbSet<SMSMessageSample> SMSMessageSample { get; set; }
        public System.Data.Entity.DbSet<SMSMessageRecord> SMSMessageRecord { get; set; }
        public System.Data.Entity.DbSet<DailyEstimate> DailyEstimate { get; set; }
        public System.Data.Entity.DbSet<DailyEstimateforCategory> DailyEstimateforCategory { get; set; }

        public System.Data.Entity.DbSet<InvoiceC0401> InvoiceC0401 { get; set; }
        public System.Data.Entity.DbSet<InvoiceC0401ProductItem> InvoiceC0401ProductItem { get; set; }
        public System.Data.Entity.DbSet<InvoiceTrack> InvoiceTrack { get; set; }
        public System.Data.Entity.DbSet<InvoiceC0501> InvoiceC0501 { get; set; }
        public System.Data.Entity.DbSet<InvoiceC0701> InvoiceC0701 { get; set; }
        public System.Data.Entity.DbSet<InvoiceD0401> InvoiceD0401 { get; set; }
        public System.Data.Entity.DbSet<InvoiceD0401ProductItem> InvoiceD0401ProductItem { get; set; }
        public System.Data.Entity.DbSet<InvoiceD0501> InvoiceD0501 { get; set; }
        public System.Data.Entity.DbSet<InvoicePrizeFooter> InvoicePrizeFooter { get; set; }
        public System.Data.Entity.DbSet<InvoicePrizeItem> InvoicePrizeItem { get; set; }

        public System.Data.Entity.DbSet<Seller_FinanDetail> Seller_FinanDetail { get; set; }
        public System.Data.Entity.DbSet<Seller_FinanMaster> Seller_FinanMaster { get; set; }

        public System.Data.Entity.DbSet<FinDocTransLog> FinDocTransLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrderitemTWBACK>()
                 .Property(m => m.SourcePrice)
                 .HasPrecision(13, 5);
            modelBuilder.Entity<PurchaseOrderitemTWBACK>()
             .Property(m => m.LocalPriceinst)
             .HasPrecision(12, 4);
            modelBuilder.Entity<PurchaseOrderitemTWBACK>()
                 .Property(m => m.LocalPrice)
                 .HasPrecision(10, 5);
            modelBuilder.Entity<PurchaseOrderitemTWBACK>()
                 .Property(m => m.DutyRate)
                 .HasPrecision(13, 5);
            modelBuilder.Entity<PurchaseOrderitemTWBACK>()
                 .Property(m => m.ProductTax)
                 .HasPrecision(13, 5);
        }
    }

}
