using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace TWNewEgg.Models.DBModels
{
    public class TWSqlDBContext : DbContext
    {
        public TWSqlDBContext(string connectionString)
            : base(connectionString)
        {

        }

        // 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdSetDB> DeviceAdSet { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdContentDB> DeviceAdContent { get; set; }
        //public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdItemDB> DeviceAdItem { get; set; }
        //public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdC2DB> DeviceAdC2 { get; set; }

        // 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160502
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Discard4DB> Discard4 { get; set; }

        //1問候語+2問候卡 -----------------------add by bruce 20160329
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB> GreetingWords { get; set; }


        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Product> Product { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp> ProductTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Account> Account { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.AddressBook> AddressBook { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Member> Member { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Item> Item { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemSketch> ItemSketch { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp> ItemTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Category> Category { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> ItemCategoryTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Country> Country { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CompanyBook> CompanyBook { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Currency> Currency { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> ItemDisplayPrice { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> ItemCategory { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> ItemPropertyGroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> ItemPropertyName { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> ItemPropertyValue { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> ItemStock { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp> ItemStocktemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ProductProperty> ProductProperty { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Seller> Seller { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBasic> PromotionGiftBasic { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBlackList> PromotionGiftBlackList { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftWhiteList> PromotionGiftWhiteList { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftInterval> PromotionGiftInterval { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftRecords> PromotionGiftRecords { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.GroupBuy> GroupBuy{ get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Adsoptionalselect> Adsoptionalselect { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Advertising> Advertising { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemAndSubCategoryMapping_NormalStore> ItemAndSubCategoryMapping_NormalStore { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup> SalesOrderGroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> SalesOrder { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> SalesOrderItem { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PayType> PayType { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Problem> Problem { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Answer> Answer { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SubCategory_NormalStore> SubCategory_NormalStore { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SubCategory_OptionStore> SubCategory_OptionStore { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Subcategorygroup> Subcategorygroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Subcategorylogo> Subcategorylogo { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Track> Track { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemSearch> ItemSearch { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.StarProduct> StarProduct { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ImageUrlReference> ImageUrlReference { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ImageUrlReferenceForProduct> ImageUrlReferenceForProduct { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Activity> Activity { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.NewsInfo> News { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.WindowBlocks> WindowBlocks { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.MediaInfo> Media { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemForChoice> ItemForChoice { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HiTrust> HiTrust { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustTrans> HiTrustTrans { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustQuery> HiTrustQuery { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustTransLog> HiTrustTransLog { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustQueryLog> HiTrustQueryLog { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> VotingActivityRec { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> VotingActivityGroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> VotingActivityItems { get; set; }   
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems> ViewTracksCartItems { get; set; }   
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Auth> Auth { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Event> Event { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.EventFile> EventFile { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.EventTempImport> EventTempImport { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.EDMBook> EDMBook { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.EDMBookList> EDMBookList { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> ItemGroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> ItemGroupDetailProperty { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> ItemGroupProperty { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CartTemp> CartTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CartItemTemp> CartItemTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CartCouponTemp> CartCouponTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.LotteryAward> LotteryAward { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.LotteryGame> LotteryGame { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.LotteryLog> LotteryLog { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Manufacture> Manufacture { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.NCCCTrans> NCCCTrans { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.HotWords> HotWords { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> View_ItemSellingQty { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.ComponentInfo> ComponentInfo { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.DynamicObject> DynamicObject { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.ImageObject> ImageObject { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.PageInfo> PageInfo { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.TextObject> TextObject { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt.VideoInfo> VideoInfo { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromoActive> PromoActive { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PromoAwardLog> PromoAwardLog { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Coupon> Coupon { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3> AdLayer3 { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3Item> AdLayer3Item { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.BankBonus> BankBonus { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.BankBonusTemp> BankBonusTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Bank> Bank { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart> AdditionalItemForCart { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForItem> AdditionalItemForItem { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem> CategoryTopItem { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.OrderQueue> OrderQueue { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.OrderQueueLog> OrderQueueLog { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PurchaseOrderTWSQLDB> PurchaseOrderTWSQLDB { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroupTemp> SalesOrderGroupTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderTemp> SalesOrderTemp { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItemTemp> SalesOrderItemTemp { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PaymentGateway> PaymentGateway { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.Installment> Installment { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.BankInstallment> BankInstallment { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment> ItemTopInstallment { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.ItemInstallmentRule> ItemInstallmentRule { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.BeneficiaryParty> BeneficiaryParty { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PaymentTerm> PaymenyTerm { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.DeliveryType_PaymentTerm> DeliveryType_PaymentTerm { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> PurgeQueue { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest> PurgeRequest { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWSQLDB.CouponActivity> CouponActivity { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.Product>()
                 .Property(m => m.TradeTax)
                 .HasPrecision(10, 5);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.Product>()
                 .Property(m => m.Tax)
                 .HasPrecision(10, 5);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.Product>()
                 .Property(m => m.Weight)
                 .HasPrecision(12, 4);              //2014.2.13 儲存重量至小數第四位 add by ice
            //modelBuilder.Entity<PurchaseOrderItem>()
            // .Property(m => m.Price)
            // .HasPrecision(13, 5);
            //modelBuilder.Entity<PurchaseOrderItem>()
            //     .Property(m => m.DutyRate)
            //     .HasPrecision(13, 5);
            //modelBuilder.Entity<PurchaseOrderItem>()
            //     .Property(m => m.ProductTax)
            //     .HasPrecision(13, 5);
            //modelBuilder.Entity<PurchaseOrderItem>()
            //   .Property(m => m.LocalPriceinst)
            //   .HasPrecision(10, 4);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.Currency>()
                 .Property(m => m.AverageexchangeRate)
                 .HasPrecision(10, 4);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.Currency>()
                .Property(m => m.BufferRate)
                .HasPrecision(10, 4);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>()
                 .Property(m => m.DisplayPrice)
                 .HasPrecision(12, 4);
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>()
                 .Property(m => m.ItemCost)
                 .HasPrecision(12, 4);

        }

    }
}
