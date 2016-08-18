using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.Models.Logistics.HiLife;

namespace TWNewEgg.DB
{
    public class TWSqlDBContext : DbContext
    {
        public TWSqlDBContext(): base("name=TWSqlDBConnection")
        {
        }
        public System.Data.Entity.DbSet<CategoryFromWS> CategoryFromWS { get; set; }
        public System.Data.Entity.DbSet<Category> Category { get; set; }
        public System.Data.Entity.DbSet<CategoryTranslate> CategoryTranslate { get; set; }
        public System.Data.Entity.DbSet<CategoryUpdatedHistory> CategoryUpdatedHistory { get; set; }
        public System.Data.Entity.DbSet<StoreFromWS> StoreFromWS { get; set; }
        public System.Data.Entity.DbSet<Store> Store { get; set; }
        public System.Data.Entity.DbSet<SubcategoryFromWS> SubcategoryFromWS { get; set; }
        public System.Data.Entity.DbSet<Subcategory> Subcategory { get; set; }
        public System.Data.Entity.DbSet<Account> Account { get; set; }
        public System.Data.Entity.DbSet<Activity> Activity { get; set; }
        public System.Data.Entity.DbSet<SalesOrder> SalesOrder { get; set; }
        public System.Data.Entity.DbSet<SalesOrderItem> SalesOrderItem { get; set; }
        public System.Data.Entity.DbSet<Problem> Problem { get; set; }
        public System.Data.Entity.DbSet<Answer> Answer { get; set; }
        public System.Data.Entity.DbSet<Product> Product { get; set; }
        public System.Data.Entity.DbSet<ProductTemp> ProductTemp { get; set; }
        public System.Data.Entity.DbSet<ProductPropertytemp> ProductPropertytemp { get; set; }
        public System.Data.Entity.DbSet<Seller> Seller { get; set; }
        public System.Data.Entity.DbSet<ItemStock> ItemStock { get; set; }
        public System.Data.Entity.DbSet<ItemStocktemp> ItemStocktemp { get; set; }
        public System.Data.Entity.DbSet<ItemStockHstry> ItemStockHstry { get; set; }
        public System.Data.Entity.DbSet<ItemSketch> ItemSketch { get; set; }
        public System.Data.Entity.DbSet<ItemSketchGroup> ItemSketchGroup { get; set; }
        public System.Data.Entity.DbSet<ItemSketchProperty> ItemSketchProperty { get; set; }
        public System.Data.Entity.DbSet<ProductPropertySketch> ProductPropertySketch { get; set; }
        public System.Data.Entity.DbSet<ItemCategorySketch> ItemCategorySketch { get; set; }
        public System.Data.Entity.DbSet<Item> Item { get; set; }
        public System.Data.Entity.DbSet<ItemGroup> ItemGroup { get; set; } 
        public System.Data.Entity.DbSet<ItemGroupDetailProperty> ItemGroupDetailProperty { get; set; }
        public System.Data.Entity.DbSet<ItemGroupProperty> ItemGroupProperty { get; set; }
        public System.Data.Entity.DbSet<Manufacture> Manufacture { get; set; }
        public System.Data.Entity.DbSet<ProductFromWS> ProductFromWS { get; set; }
        public System.Data.Entity.DbSet<TwTradeTax> TwTradeTax { get; set; }
        public System.Data.Entity.DbSet<ItemCategory> ItemCategory { get; set; }
        public System.Data.Entity.DbSet<ItemCategorytemp> ItemCategorytemp { get; set; }
        public System.Data.Entity.DbSet<ItemTemp> ItemTemp { get; set; }
        public System.Data.Entity.DbSet<ItemList> ItemList { get; set; }
        public System.Data.Entity.DbSet<ItemListTemp> ItemListTemp { get; set; }
        public System.Data.Entity.DbSet<ItemListGroup> ItemListGroup { get; set; }
        public System.Data.Entity.DbSet<ItemListGroupTemp> ItemListGroupTemp { get; set; }
        public System.Data.Entity.DbSet<NodeFromWS> NodeFromWS { get; set; }
        public System.Data.Entity.DbSet<TradeCategory> TradeCategory { get; set; }
        public System.Data.Entity.DbSet<Country> Country { get; set; }
        public System.Data.Entity.DbSet<Logistic> Logistic { get; set; }
        public System.Data.Entity.DbSet<Track> Track { get; set; }
        public System.Data.Entity.DbSet<TrackItem> TrackItem { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems> CartItems { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.ExtModels.ShoppingCartItems> ShoppingCartItems { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.ExtModels.GetItemTaxDetail> GetItemTaxDetail { get; set; }
        public System.Data.Entity.DbSet<Bank> Bank { get; set; }
        public System.Data.Entity.DbSet<Addressbook> Addressbook { get; set; }
        public System.Data.Entity.DbSet<PayType> PayType { get; set; }
        public System.Data.Entity.DbSet<Ads> Ads { get; set; }
        public System.Data.Entity.DbSet<AdSet> AdSet { get; set; }
        public System.Data.Entity.DbSet<ReviewFromWS> ReviewFromWS { get; set; }
        public System.Data.Entity.DbSet<HotSale> HotSale { get; set; }
        public System.Data.Entity.DbSet<NewProduct> NewProduct { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrderGroup> PurchaseOrderGroup { get; set; }
        public System.Data.Entity.DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; }
        public System.Data.Entity.DbSet<SalesOrderGroup> SalesOrderGroup { get; set; }
        public System.Data.Entity.DbSet<Auth> Auth { get; set; }
        public System.Data.Entity.DbSet<Currency> Currency { get; set; }
        public System.Data.Entity.DbSet<StarProduct> starProduct { get; set; }
        public System.Data.Entity.DbSet<FeatureItem> FeatureItem { get; set; }
        public System.Data.Entity.DbSet<ItemSearch> ItemSearch { get; set; }
        public System.Data.Entity.DbSet<SalesOrderCancel> SalesOrderCancel { get; set; }
        public System.Data.Entity.DbSet<CompanyBook> CompanyBook { get; set; }
        public System.Data.Entity.DbSet<MyNewEgg> MyNewEgg { get; set; }
        public System.Data.Entity.DbSet<ProductFromJieMai> ProductFromJieMai { get; set; }
        public System.Data.Entity.DbSet<Event> Event { get; set; }
        public System.Data.Entity.DbSet<EventFile> EventFile { get; set; }
        public System.Data.Entity.DbSet<EventTempImport> EventTempImport { get; set; }
        public System.Data.Entity.DbSet<Coupon> Coupon { get; set; }
        public System.Data.Entity.DbSet<Couponfrm> Couponfrm { get; set; }
        public System.Data.Entity.DbSet<Couponcncl> Couponcncl { get; set; }
        public System.Data.Entity.DbSet<Fmblk> Fmblk { get; set; }
        public System.Data.Entity.DbSet<Fmblkitm> Fmblkitm { get; set; }
        public System.Data.Entity.DbSet<Fmcncl> Fmcncl { get; set; }
        public System.Data.Entity.DbSet<Fmevnt> Fmevnt { get; set; }
        public System.Data.Entity.DbSet<Fmnew> Fmnew { get; set; }
        public System.Data.Entity.DbSet<Fmtkout> Fmtkout { get; set; }
        public System.Data.Entity.DbSet<Fmvoid> Fmvoid { get; set; }
        public System.Data.Entity.DbSet<Redm> Redm { get; set; }
        public System.Data.Entity.DbSet<Redmacnt> Redmacnt { get; set; }
        public System.Data.Entity.DbSet<Redmbatch> Redmbatch { get; set; }
        public System.Data.Entity.DbSet<Redmio> Redmio { get; set; }

        public System.Data.Entity.DbSet<ItemTranslate> ItemTranslate { get; set; }
        public System.Data.Entity.DbSet<ContactAddress> ContactAddress { get; set; }
        public System.Data.Entity.DbSet<ContactType> ContactType { get; set; }
        public System.Data.Entity.DbSet<AdvEvent> AdvEvent { get; set; } //廣告事件 add by Bill 2014/03/04?
        public System.Data.Entity.DbSet<AdvEventType> AdvEventType { get; set; } //廣告事件型態 add by Bill 2014/03/07
        public System.Data.Entity.DbSet<DrawingList> DrawingList { get; set; } //抽獎紀錄 add by Bill 2014/03/12
        public System.Data.Entity.DbSet<ItemDisplayPrice> ItemDisplayPrice { get; set; } //總價表 add by Bill 2014/04/02
        public System.Data.Entity.DbSet<EDMBook> EDMBook { get; set; }
        public System.Data.Entity.DbSet<EDMBookList> EDMBookList { get; set; }
        public System.Data.Entity.DbSet<BankCodeMessage> BankCodeMessage { get; set; }
        public System.Data.Entity.DbSet<Member> Member { get; set; }
        public System.Data.Entity.DbSet<ItemPropertyGroup> ItemPropertyGroup { get; set; }
        public System.Data.Entity.DbSet<ItemPropertyName> ItemPropertyName { get; set; }
        public System.Data.Entity.DbSet<ItemPropertyValue> ItemPropertyValue { get; set; }
        public System.Data.Entity.DbSet<ProductProperty> ProductProperty { get; set; }
        public System.Data.Entity.DbSet<BlockProduct> BlockProduct { get; set; }
        public System.Data.Entity.DbSet<GroupBuy> GroupBuy { get; set; }
        public System.Data.Entity.DbSet<SmsSubmit> SmsSubmit { get; set; }
        public System.Data.Entity.DbSet<SmsSubmitReturn> SmsSubmitReturn { get; set; }
        public System.Data.Entity.DbSet<LandingPage> LandingPage { get; set; }
        public System.Data.Entity.DbSet<LandingPageList> LandingPageList { get; set; }
        public System.Data.Entity.DbSet<PromotionGiftBasic> PromotionGiftBasic { get; set; }
        public System.Data.Entity.DbSet<PromotionGiftInterval> PromotionGiftInterval { get; set; }
        public System.Data.Entity.DbSet<PromotionGiftWhiteList> PromotionGiftWhiteList { get; set; }
        public System.Data.Entity.DbSet<PromotionGiftBlackList> PromotionGiftBlackList { get; set; }
        public System.Data.Entity.DbSet<PromotionGiftRecords> PromotionGiftRecords { get; set; }
        public System.Data.Entity.DbSet<OTPRecord> OTPRecord { get; set; }
        public System.Data.Entity.DbSet<Accountactcheck> Accountactcheck { get; set; }
        public System.Data.Entity.DbSet<PlaceOrderRecord> PlaceOrderRecord { get; set; }
        public System.Data.Entity.DbSet<TrackingCode> TrackingCode { get; set; }
        public System.Data.Entity.DbSet<ItemDeliverWhite> ItemDeliverWhite { get; set; }
        public System.Data.Entity.DbSet<ItemDeliverBlack> ItemDeliverBlack { get; set; }
        public System.Data.Entity.DbSet<HiLifeOrderInfo> HiLifeOrderInfo { get; set; }
        public System.Data.Entity.DbSet<StockAutoNotifyRecord> StockAutoNotifyRecord { get; set; }
        public System.Data.Entity.DbSet<MetaDataGroup> MetaDataGroup { get; set; }
        public System.Data.Entity.DbSet<ItemWarrantyBlackList> ItemWarrantyBlackList { get; set; }
        public System.Data.Entity.DbSet<ImageUrlReferenceForProduct> ImageUrlReferenceForProduct { get; set; }
        public System.Data.Entity.DbSet<ImageUrlReference> ImageUrlReference { get; set; }
        //public System.Data.Entity.DbSet<ItemTempDeliver> ItemTempDeliver { get; set; }
        
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F00Body> HiLifeF00Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F01Head> HiLifeF01Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F01Body> HiLifeF01Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F01Tail> HiLifeF01Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F02Head> HiLifeF02Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F02Body> HiLifeF02Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F02Tail> HiLifeF02Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F03Head> HiLifeF03Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F03Body> HiLifeF03Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F03Tail> HiLifeF03Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F04Head> HiLifeF04Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F04Body> HiLifeF04Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F04Tail> HiLifeF04Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F05Head> HiLifeF05Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F05Body> HiLifeF05Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F05Tail> HiLifeF05Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F06Head> HiLifeF06Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F06Body> HiLifeF06Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F06Tail> HiLifeF06Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F08Head> HiLifeF08Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F08Body> HiLifeF08Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F08Tail> HiLifeF08Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F09Head> HiLifeF09Head { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F09Body> HiLifeF09Body { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.F09Tail> HiLifeF09Tail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.Models.Logistics.HiLife.TxtBody> HiLifeTxtBody { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.GenerateNumberMap> GenerateNumberMap { get; set; }
        public System.Data.Entity.DbSet<OmusicAccount> OmusicAccount { get; set; } //2015.3.2 Omusic add by ice
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.ItemWarranty> ItemWarranty { get; set; }
        //public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.XQProcess> XQProcess { get; set; }
        //public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.XQProcessNumber> XQProcessNumber { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.AccountJoinGroup> AccountJoinGroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.PromoActive> PromoActive { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.PromoAwardLog> PromoAwardLog { get; set; }

        //首頁及分類頁櫥窗
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.SubCategory_NormalStore> SubCategory_NormalStore { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.ItemAndSubCategoryMapping_NormalStore> ItemAndSubCategoryMapping_NormalStore { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.Advertising> Advertising { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.Subcategorygroup> Subcategorygroup { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.Subcategorylogo> Subcategorylogo { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.WindowBlocks> WindowBlocks { get; set; }
        //熱門關鍵字設定
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.HotWords> HotWords { get; set; }
        
        //Azure 上傳紀錄
        public System.Data.Entity.DbSet<TWNewEgg.DB.TWSQLDB.Models.Image_log> Image_log { get; set; }
        
        public string GetAutoSN(string type)
        {
            string result = "";
            int max = 0;
            string code = "";
            type = type.ToUpper();
        
            switch (type)
            {
                case "SO":
                case "USBO":
                case "USMO":
                    //salesorder_code
                    max = 0;
                    code = (from row in this.SalesOrder select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }
                    code = (from row in this.SalesOrder.Local select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        int temp = int.Parse(code);
                        if (temp > max)
                            max = temp;
                    }
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
                case "SS":
                case "USBS":
                case "USMS":
                    //salesorderitem_code
                    max = 0;
                    code = (from row in this.SalesOrderItem select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }
                    code = (from row in this.SalesOrderItem.Local select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        int temp = int.Parse(code);
                        if (temp > max)
                            max = temp;
                    }
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
                case "PR":
                    //prblm_code
                    max = 0;
                    code = (from row in this.Problem select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }
                    code = (from row in this.Problem.Local select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        int temp = int.Parse(code);
                        if (temp > max)
                            max = temp;
                    }
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
                case "AN":
                    //prblm_code
                    max = 0;
                    code = (from row in this.Answer select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        max = int.Parse(code);
                    }
                    code = (from row in this.Answer.Local select row.Code).Max();
                    if (code != null)
                    {
                        code = code.Substring(8);
                        int temp = int.Parse(code);
                        if (temp > max)
                            max = temp;
                    }
                    max++;
                    result = type + DateTime.Now.ToString("yyMMdd") + max.ToString("0000000");
                    break;
                default:
                    throw new Exception("Type Error ! Only Accept 'SO'、'USBO'、'USMO'、'SS'、'USBS'、'USMS'、'PR'、'AN'。");
            }
            return result;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                 .Property(m => m.TradeTax)
                 .HasPrecision(10, 5);
            modelBuilder.Entity<Product>()
                 .Property(m => m.Tax)
                 .HasPrecision(10, 5);
            modelBuilder.Entity<Product>()
                 .Property(m => m.Weight)
                 .HasPrecision(12, 4);              //2014.2.13 儲存重量至小數第四位 add by ice
            modelBuilder.Entity<PurchaseOrderItem>()
                 .Property(m => m.Price)
                 .HasPrecision(13, 5);
            modelBuilder.Entity<PurchaseOrderItem>()
                 .Property(m => m.DutyRate)
                 .HasPrecision(13, 5);
            modelBuilder.Entity<PurchaseOrderItem>()
                 .Property(m => m.ProductTax)
                 .HasPrecision(13, 5);
            modelBuilder.Entity<PurchaseOrderItem>()
               .Property(m => m.LocalPriceinst)
               .HasPrecision(10, 4);
            modelBuilder.Entity<Currency>()
                 .Property(m => m.AverageexchangeRate)
                 .HasPrecision(10, 4);
            modelBuilder.Entity<Currency>()
                .Property(m => m.BufferRate)
                .HasPrecision(10, 4);
            modelBuilder.Entity<ItemDisplayPrice>()
                 .Property(m => m.DisplayPrice)
                 .HasPrecision(12, 4);
            modelBuilder.Entity<ItemDisplayPrice>()
                 .Property(m => m.ItemCost)
                 .HasPrecision(12, 4);

        }
    }

}
