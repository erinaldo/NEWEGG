using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    /*---------- add by Ian and Thisway ----------*/
    /// <summary>
    /// 名稱  ：TWSQLDB.dbo.Item的所有欄位
    /// DB位置：TWSQLDB.dbo.Item
    /// 說明  ：目前61個欄位都在裡面(更新日期：2014/01/14)
    /// </summary>
    public class ItemInfoResult 
    {
        #region 創建商品資訊，給商家填寫的欄位
        /// <summary>
        /// 名稱  ：SubCategoryID(類別)
        /// DB位置：TTWSQLDB.dbo.Item.CategoryID
        /// 說明  ：使用者可以不用填寫，不填寫時依照開頭填入，填寫時依照填寫內容填入
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 名稱  ：Product Description(商品描述)
        /// DB位置：TWSQLDB.dbo.Item.DescTW
        /// 說明  ：Product.DescriptionTW(nvarchar(500)) = Item.DescTW(nvarchar(4000))，字超過500，SQL會自動刪除
        /// </summary>
        public string DescTW { get; set; }

        /// <summary>
        /// 名稱  ：Product Short Desc(商品簡要描述)
        /// DB位置：TWSQLDB.dbo.Item.Sdesc
        /// 說明  ：
        /// </summary>
        public string Sdesc { get; set; }

        /// <summary>
        /// 名稱  ：Product Feature Title(商品特色標題)
        /// DB位置：TWSQLDB.dbo.Item.Spechead
        /// 說明  ：
        /// </summary>
        public string Spechead { get; set; }

        /// <summary>
        /// 名稱  ：Product Model(商品型號)
        /// DB位置：TWSQLDB.dbo.Item.Model
        /// 說明  ：(Product.Model = Item.Model)
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 名稱  ：Product Package(商品包裝)
        /// DB位置：TWSQLDB.dbo.Item.ItemPackage
        /// 說明  ：
        /// </summary>
        public string ItemPackage { get; set; }

        /// <summary>
        /// 名稱  ：Selling Price(網路售價)
        /// DB位置：TWSQLDB.dbo.Item.PriceCash
        /// 說明  ：
        /// </summary>
        public decimal PriceCash { get; set; }

        /// <summary>
        /// 名稱  ：Market Price(市場建議售價)
        /// DB位置：TWSQLDB.dbo.Item.MarketPrice
        /// 說明  ：
        /// </summary>
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 名稱  ：Is Limit(是否限量)
        /// DB位置：無
        /// 說明  ：判斷庫存是否限量，無欄位
        ///        若為限量IsLimit = "Yes" ，放入Item.Qty(限量庫存)
        ///        若非限量IsLimit = "No" ，放入Itemstock.Qty(一般庫存)
        /// </summary>
        public string IsLimit { get; set; }

        /// <summary>
        /// 名稱  ：Inventory(庫存)
        /// DB位置：TWSQLDB.dbo.Item.Qty
        /// 說明  ：Item.Qty(限量庫存)；Itemstock.Qty(一般庫存)
        /// </summary>
        public int Inventory { get; set; }

        /// <summary>
        /// 名稱  ：Is Ship by Newegg(運送類型)
        /// DB位置：TWSQLDB.dbo.Itemstock.ShipType
        /// 說明  ：
        ///        Item.ShipType(N) ==> Product.DelvType(7)
        ///        Item.ShipType(S) ==> Product.DelvType(2)
        ///        Item.DelvType = Product.DelvType
        /// </summary>
        public string ShipType { get; set; }

        /// <summary>
        /// 名稱  ：Delv Day(到貨天數)
        /// DB位置：TWSQLDB.dbo.Itemstock.DelvDate
        /// 說明  ：
        /// </summary>
        public string DelvDate { get; set; }

        /// <summary>
        /// 名稱  ：Item Start Date(賣場開始日期)
        /// DB位置：TWSQLDB.dbo.Itemstock.DateStart
        /// 說明  ：
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 名稱  ：Item End Date(賣場結束日期)
        /// DB位置：TWSQLDB.dbo.Itemstock.DateEnd
        /// 說明  ：
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 名稱  ：Item Images(商品圖)
        /// DB位置：xxxx
        /// 說明  ：數條圖片的URL連結
        /// </summary>
        public List<string> ItemImages { get; set; }


        #endregion

        #region 商品屬性 add by ice
        public List<SaveProductProperty> PropertyInfos { get; set; }
        #endregion

        #region SellerPortal自動擷取，放入API
        /// <summary>
        /// 名稱  ：CreateUser(建立者)
        /// DB位置：TWSQLDB.dbo.Item.CreateUser
        /// 說明  ：輸入創建時登入者的UserID
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 名稱  ：UpdateUser(更新者)
        /// DB位置：TWSQLDB.dbo.Item.UpdateUser
        /// 說明  ：輸入更新時登入者的UserID
        /// </summary>
        public string UpdateUser { get; set; }
        #endregion

        #region 與使用者輸入的某個欄位相同值(創建商品資料)
        /// <summary>
        /// 名稱  ：Name(商品名稱)
        /// DB位置：TTWSQLDB.dbo.Item.Name
        /// 說明  ：Item.Name = Product.Name
        ///        Item.Name = Product.NameTW;
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名稱  ：ItemDesc(英文描述)
        /// DB位置：TTWSQLDB.dbo.Item.ItemDesc
        /// 說明  ：Item.ItemDesc = Product.Description
        /// </summary>
        public string ItemDesc { get; set; }


        /// <summary>
        /// 名稱  ：DelvType(配送方式)
        /// DB位置：TTWSQLDB.dbo.Item.DelvType
        /// 說明  ：Item.DelvType = Product. DelvType
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// 名稱  ：ProductID(產品編號)
        /// DB位置：TTWSQLDB.dbo.Item.ProductID
        /// 說明  ：Item.ProductID = Product.ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 名稱  ：PriceCard(刷卡價)
        /// DB位置：TTWSQLDB.dbo.Item.PriceCard
        /// 說明  ：Item.PriceCard = Item.PriceCash
        /// </summary>
        public decimal PriceCard { get; set; }

        /// <summary>
        /// 名稱  ：ManufacturerID(製造商ID)
        /// DB位置：TTWSQLDB.dbo.Item.ManufacturerID
        /// 說明  ：Item.ManufacturerID = Product.ManufacturerID
        /// </summary>
        public int ManufactureID { get; set; }

        /// <summary>
        /// 名稱  ：SellerID(商家ID)
        /// DB位置：TTWSQLDB.dbo.Item.SellerID
        /// 說明  ：Item.SellerID = Product.SellerID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 名稱  ：Note(備註)
        /// DB位置：TTWSQLDB.dbo.Item.Note
        /// 說明  ：Item.Note = Product.Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 名稱  ：PicStart(產品圖片第一張)
        /// DB位置：TTWSQLDB.dbo.Item.PicStart
        /// 說明  ：Item.PicStart = Product.PicStart
        /// </summary>
        public int PicStart { get; set; }

        /// <summary>
        /// 名稱  ：PicEnd(產品圖片最後一張)
        /// DB位置：TTWSQLDB.dbo.Item.PicEnd
        /// 說明  ：Item.PicEnd = Product.PicEnd
        /// </summary>
        public int PicEnd { get; set; }

        /// <summary>
        /// 名稱  ：DateDel(商品臨時ID)
        /// DB位置：TTWSQLDB.dbo.Item.DateDel
        /// 說明  ：Item.DateDel > Item.DateEnd 
        /// </summary>
        public DateTime DateDel { get; set; }
        #endregion

        #region API寫入，放入Product DB(創建商品時給固定的值)
        /// <summary>
        /// 名稱  ：SpecDetail(英文描述)
        /// DB位置：TTWSQLDB.dbo.Item.SpecDetail
        /// 說明  ：string SpecDetail = "";
        /// </summary>
        public string SpecDetail { get; set; }

        /// <summary>
        /// 名稱  ：SaleType(販售類型)
        /// DB位置：TTWSQLDB.dbo.Item.SaleType
        /// 說明  ：int SaleType = 1;
        /// </summary>
        public int SaleType { get; set; }

        /// <summary>
        /// 名稱  ：PayType(付款方式)
        /// DB位置：TTWSQLDB.dbo.Item.PayType
        /// 說明  ：int PayType = 0;
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 名稱  ：Layout(安排)
        /// DB位置：TTWSQLDB.dbo.Item.Layout
        /// 說明  ：int Layout = 0;
        /// </summary>
        public int Layout { get; set; }

        /// <summary>
        /// 名稱  ：Itemnumber(來源賣場編號)
        /// DB位置：TTWSQLDB.dbo.Item.Itemnumber
        /// 說明  ：string Itemnumber = "";
        /// </summary>
        public string Itemnumber { get; set; }

        /// <summary>
        /// 名稱  ：Pricesgst(建議售價)
        /// DB位置：TTWSQLDB.dbo.Item.Pricesgst
        /// 說明  ：decimal Pricesgst = 0;
        /// </summary>
        public decimal Pricesgst { get; set; }

        /// <summary>
        /// 名稱  ：ServicePrice(服務費)
        /// DB位置：TTWSQLDB.dbo.Item.ServicePrice
        /// 說明  ：decimal ServicePrice = 0;
        /// </summary>
        public decimal ServicePrice { get; set; }

        /// 名稱  ：PricehpType1(分期期數)
        /// DB位置：TTWSQLDB.dbo.Item.PricehpType1
        /// 說明  ：int PricehpType1 = 0;
        /// </summary>
        public int PricehpType1 { get; set; }

        /// 名稱  ：PricehpInst1(分期利息)
        /// DB位置：TTWSQLDB.dbo.Item.PricehpInst1
        /// 說明  ：decimal PricehpInst1 = 0;
        /// </summary>
        public decimal PricehpInst1 { get; set; }

        /// 名稱  ：PricehpType2(分期期數二)
        /// DB位置：TTWSQLDB.dbo.Item.PricehpType2
        /// 說明  ：int PricehpType2 = 0;
        /// </summary>
        public int PricehpType2 { get; set; }

        /// 名稱  ：PricehpInst2(分期利息二)
        /// DB位置：TTWSQLDB.dbo.Item.PricehpInst2
        /// 說明  ：decimal PricehpInst2 = 0;
        /// </summary>
        public decimal PricehpInst2 { get; set; }

        /// 名稱  ：Inst0Rate(零利率分期期數)
        /// DB位置：TTWSQLDB.dbo.Item.Inst0Rate
        /// 說明  ：int Inst0Rate = 0;
        /// </summary>
        public int Inst0Rate { get; set; }

        /// 名稱  ：RedmfdbckRate(回饋比例)
        /// DB位置：TTWSQLDB.dbo.Item.RedmfdbckRate
        /// 說明  ：decimal RedmfdbckRate = 0;
        /// </summary>
        public decimal RedmfdbckRate { get; set; }

        /// 名稱  ：Coupon(折價券編號)
        /// DB位置：TTWSQLDB.dbo.Item.Coupon
        /// 說明  ：string Coupon = "0";
        /// </summary>
        public string Coupon { get; set; }

        /// 名稱  ：PriceCoupon(折價券金額)
        /// DB位置：TTWSQLDB.dbo.Item.PriceCoupon
        /// 說明  ：decimal PriceCoupon = 0;
        /// </summary>
        public decimal PriceCoupon { get; set; }

        /// 名稱  ：PriceLocalship(本地運費)
        /// DB位置：TTWSQLDB.dbo.Item.PriceLocalship
        /// 說明  ：decimal PriceLocalship = 0;
        /// </summary>
        public decimal PriceLocalship { get; set; }

        /// 名稱  ：PriceGlobalship(國際運費)
        /// DB位置：TTWSQLDB.dbo.Item.PriceGlobalship
        /// 說明  ：decimal PriceGlobalship = 0;
        /// </summary>
        public decimal PriceGlobalship { get; set; }

        /// 名稱  ：SafeQty(安全警告數量)
        /// DB位置：TTWSQLDB.dbo.Item.SafeQty
        /// 說明  ：int SafeQty = 0;
        /// </summary>
        public int SafeQty { get; set; }

        /// 名稱  ：QtyLimit(限購數量)
        /// DB位置：TTWSQLDB.dbo.Item.QtyLimit
        /// 說明  ：int QtyLimit = 0;
        /// </summary>
        public int QtyLimit { get; set; }

        /// 名稱  ：LimitRule(限購規則)
        /// DB位置：TTWSQLDB.dbo.Item.LimitRule
        /// 說明  ：string LimitRule = "";
        /// </summary>
        public string LimitRule { get; set; }

        /// 名稱  ：QtyReg(已登記數量)
        /// DB位置：TTWSQLDB.dbo.Item.QtyReg
        /// 說明  ：int QtyReg = 0;
        /// </summary>
        public int QtyReg { get; set; }

        /// 名稱  ：PhotoName(圖檔名稱)
        /// DB位置：TTWSQLDB.dbo.Item.PhotoName
        /// 說明  ：string PhotoName = "";
        /// </summary>
        public string PhotoName { get; set; }

        /// 名稱  ：HtmlName(網頁檔名)
        /// DB位置：TTWSQLDB.dbo.Item.HtmlName
        /// 說明  ：string HtmlName = "";
        /// </summary>
        public string HtmlName { get; set; }

        /// 名稱  ：ShowOrder(顯示順序)
        /// DB位置：TTWSQLDB.dbo.Item.ShowOrder
        /// 說明  ：int ShowOrder = 0;
        /// </summary>
        public int ShowOrder { get; set; }

        /// 名稱  ：Class(類型)
        /// DB位置：TTWSQLDB.dbo.Item.Class
        /// 說明  ：int Class = 1;
        /// </summary>
        public int Class { get; set; }

        /// 名稱  ：Status(上下架狀態)
        /// DB位置：TTWSQLDB.dbo.Item.Status
        /// 說明  ：int Status = 1;
        /// </summary>
        public int Status { get; set; }

        /// 名稱  ：StatusNote(狀態備註)
        /// DB位置：TTWSQLDB.dbo.Item.StatusNote
        /// 說明  ：string StatusNote = "";
        /// </summary>
        public string StatusNote { get; set; }

        /// 名稱  ：StatusDate(狀態最後更改時間)
        /// DB位置：TTWSQLDB.dbo.Item.StatusDate
        /// 說明  ：輸入上傳創建商品資料的時間
        /// </summary>
        public DateTime StatusDate { get; set; }

        /// <summary>
        /// 名稱  ：CreateDate(建檔日期)
        /// DB位置：TWSQLDB.dbo.Item.CreateDate
        /// 說明  ：輸入創建商品資訊的當下日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 名稱  ：Updated(更新)
        /// DB位置：TWSQLDB.dbo.Item.Updated
        /// 說明  ：int Updated = 0;
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// 名稱  ：UpdateDate(更新日期)
        /// DB位置：TWSQLDB.dbo.Item.UpdateDate
        /// 說明  ：輸入更新創建商品資訊的當下日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 名稱  ：Taxfee(代收代付稅款)
        /// DB位置：TTWSQLDB.dbo.Item.Taxfee
        /// 說明  ：Product.Tax = Item.Taxfee = 0
        /// </summary>
        public Decimal Taxfee { get; set; }
        #endregion

        #region 系統自動產生的欄位
        /// <summary>
        /// 名稱  ：ID(上架商品編號)
        /// DB位置：TTWSQLDB.dbo.Item.ID
        /// 說明  ：系統自動產生
        /// </summary>
        //public int ID { get; set; }
        #endregion

        #region 先不管的欄位
        /// <summary>
        /// 名稱  ：ItemtempID(商品臨時ID)
        /// DB位置：TTWSQLDB.dbo.Item.ItemtempID
        /// 說明  ：不用填
        /// </summary>
        public int ItemtempID { get; set; }

        /// <summary>
        /// 名稱  ：ShipFee(運費)
        /// DB位置：TTWSQLDB.dbo.Item.ShipFee
        /// 說明  ：不用填
        /// </summary>
        public DateTime ShipFee { get; set; }
        #endregion

        #region 商品單一建立新增的欄位
        /// <summary>
        /// 限量數量：textbox => item.Qty
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 商品成色：前台 Item.IsNew ，紀錄新品或福利品
        /// </summary>
        public string IsNew { get; set; }

        /// <summary>
        /// 名稱  ：安全庫存量(庫存警示)
        /// DB位置：TTWSQLDB.dbo.ItemStock.SafeQty
        /// 說明  ：
        /// </summary>
        public int ItemStockSafeQty { get; set; }

        /// <summary>
        /// ItemStock.QtyReg 已售商品數量
        /// </summary>
        public int ItemStockQtyReg { get; set; }

        /// <summary>
        /// ItemStock.Fdbcklmt 已售商品數量
        /// </summary>
        public int ItemStockFdbcklmt { get; set; }
        #endregion 

        
    }
    /*---------- end by Ian and Thisway ----------*/


    public class BathItemCreateInfo
    {
        /// <summary>
        /// 副檔名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 判斷是哪一個需要批次建立
        /// </summary>
        public string CreateFile { get; set; }
        /// <summary>
        /// 判斷是登入者為Seller 或 Vendor
        /// </summary>
        public string AccountTypeCode { get; set; }
        /// <summary>
        /// Create SellerID
        /// </summary>
        public int SellerID { get; set; }
        /// <summary>
        /// Create UserID
        /// </summary>
        public string UserID { get; set; }

    }

    public class BathItemCreateInfoResult
    {
        public BathItemCreateInfoQueryResult QueryResult { get; set; }

        public string ResultMessage { get; set; }
    }

    public enum BathItemCreateInfoQueryResult
    {
        success,
        Failed
    }

    /// <summary>
    /// 讀取 Excel 內 DetailInfo 表格
    /// </summary>
    public class BathCreateItemInfoListDetailInfo
    {
        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerPartNumber { get; set; }
        /// <summary>
        /// 製造商商品編號
        /// </summary>
        public string ManufacturePartNumber { get; set; }
        /// <summary>
        /// 通用產品代碼
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 讀取 Excel 表內 Datafeed 表格
    /// </summary>
    public class BathCreateItemInfoListDatafeed
    {
        public string SubCategoryID { get; set; }

        public string SellerPartNumber { get; set; }

        public string ManufacturerURL { get; set; }

        public string ManufacturerPartNumber { get; set; }

        public string UPC { get; set; }

        public string WebsiteShortTitle { get; set; }

        public string ProductDescription { get; set; }

        public string ProductNote { get; set; }

        public string ShortDesc { get; set; }

        public string FeatureTitle { get; set; }

        public string Model { get; set; }

        public string BarCode { get; set; }

        public string Length { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public string Weight { get; set; }

        public string Condition { get; set; }

        public string Package { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public string ProductCost { get; set; }
        /// <summary>
        /// 賣價
        /// </summary>
        public string SelleringPrice { get; set; }

        public string MarketPrice { get; set; }

        public string Warranty { get; set; }

        public string LimitQuantity { get; set; }

        public string QuotaQuantity { get; set; }

        public string IsLimit { get; set; }

        public string Inventory { get; set; }

        public string IsShipByNewegg { get; set; }

        public string DelvDay { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Action { get; set; }

        public string Images { get; set; }

        public string IsDangerous { get; set; }

        public string IsMarketPlace { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class BatchExcelCreate
    {
        public string ItemID { get; set; }
        public string MainCategoryID_Layer2 { get; set; }
        public string SubCategoryID_1_Layer2 { get; set; }
        public string SubCategoryID_2_Layer2 { get; set; }
        public string SellerProductID { get; set; }
        public string ManufactureID { get; set; }
        public string MenufacturePartNum { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Sdesc { get; set; }
        public string Spechead { get; set; }
        public string Model { get; set; }
        public string BarCode { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string IsNew { get; set; }
        public string ItemPackage { get; set; }
        public string Cost { get; set; }
        public string priceCash { get; set; }
        public string MarketPrice { get; set; }
        public string Warranty { get; set; }
        public string ItemQty { get; set; }
        public string QtyLimit { get; set; }
        public string SaveQty { get; set; }
        public string InventoryQty { get; set; }
        public string ShipType { get; set; }
        public string DelvDate { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string PicPatch_Edit { get; set; }
        public string IsShipDanger { get; set; }
    }
}
