using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class APIItemModel
    {

        #region Basic Info

        /// <summary>
        /// 名稱  ：Name(商品名稱)
        /// DB位置：TTWSQLDB.dbo.Item.Name
        /// 說明  ：Item.Name = Product.Name
        ///        Item.Name = Product.NameTW;
        /// </summary>
        public string API_Item_Name { get; set; }

        /// <summary>
        /// 名稱  ：Description(英文描述)
        /// DB位置：TWSQLDB.dbo.product.Description
        /// 說明  ：與創建商品資料的Product.DescriptionTW一致(Product.DescriptionTW = Product.Description)
        /// </summary>
        public string API_Product_Description { get; set; }

        /// <summary>
        /// 名稱  ：product's note(商品注意事項)
        /// DB位置：TWSQLDB.dbo.Product.Note
        /// 說明  ：
        /// </summary>
        public string API_Product_Note { get; set; }

        /// <summary>
        /// 名稱  ：product Model(商品型號)
        /// DB位置：TWSQLDB.dbo.Product.Model
        /// 說明  ：Product.Model = Item.Model
        /// </summary>
        public string API_Product_Model { get; set; }

        /// <summary>
        /// 名稱  ：UPC(商品條碼)
        /// DB位置：TWSQLDB.dbo.Product.UPC
        /// 說明  ：
        /// </summary>
        public string API_Product_UPC { get; set; }

        /// <summary>
        /// 名稱  ：Product Length(長度)
        /// DB位置：TWSQLDB.dbo.Product.Length
        /// 說明  ：
        /// </summary>
        public decimal? API_Product_Length { get; set; }

        /// <summary>
        /// 名稱  ：Product Width(寬度)
        /// DB位置：TWSQLDB.dbo.Product.Width
        /// 說明  ：
        /// </summary>
        public decimal? API_Product_Width { get; set; }

        /// <summary>
        /// 名稱  ：Product Height(高度)
        /// DB位置：TWSQLDB.dbo.Product.Height
        /// 說明  ：
        /// </summary>
        public decimal? API_Product_Height { get; set; }

        /// <summary>
        /// 名稱  ：Product Weight(重量)
        /// DB位置：TWSQLDB.dbo.Product.Weight
        /// 說明  ：
        /// </summary>
        public decimal? API_Product_Weight { get; set; }

        /// <summary>
        /// 名稱  ：Warranty(保固期)
        /// DB位置：TWSQLDB.dbo.Product.Warranty
        /// 說明  ：
        /// </summary>
        public int? API_Product_Warranty { get; set; }

        #endregion

        #region Specific Info

        /// <summary>
        /// 名稱  ：Product Package(商品包裝)
        /// DB位置：TWSQLDB.dbo.Item.ItemPackage
        /// 說明  ：
        /// </summary>
        public string API_Item_ItemPackage { get; set; }

        /// <summary>
        /// 名稱  ：Product Condition(商品成色)
        /// DB位置：TWSQLDB.dbo.Product.Status
        /// 說明  ：
        /// </summary>
        public int API_Product_Status { get; set; }

        /// <summary>
        /// 名稱  ：Is Ship by Newegg(運送類型)
        /// DB位置：TWSQLDB.dbo.Itemstock.ShipType
        /// 說明  ：
        ///        Item.ShipType(N) ==> Product.DelvType(7)
        ///        Item.ShipType(S) ==> Product.DelvType(2)
        ///        Item.DelvType = Product.DelvType
        /// </summary>
        public string API_Item_ShipType { get; set; }

        /// <summary>
        /// 名稱  ：Product Feature Title(商品特色標題)
        /// DB位置：TWSQLDB.dbo.Item.Spechead
        /// 說明  ：
        /// </summary>
        public string API_Item_Spechead { get; set; }

        /// <summary>
        /// 名稱  ：Product Short Desc(商品簡要描述)
        /// DB位置：TWSQLDB.dbo.Item.Sdesc
        /// 說明  ：
        /// </summary>
        public string API_Item_Sdesc { get; set; }

        /// <summary>
        /// 名稱  ：Delv Day(到貨天數)
        /// DB位置：TWSQLDB.dbo.Itemstock.DelvDate
        /// 說明  ：
        /// </summary>
        public string API_Item_DelvDate { get; set; }

        /// <summary>
        /// 名稱  ：Item Start Date(賣場開始日期)
        /// DB位置：TWSQLDB.dbo.Itemstock.DateStart
        /// 說明  ：
        /// </summary>
        public DateTime API_Item_DateStart { get; set; }

        /// <summary>
        /// 名稱  ：Item End Date(賣場結束日期)
        /// DB位置：TWSQLDB.dbo.Itemstock.DateEnd
        /// 說明  ：
        /// </summary>
        public DateTime API_Item_DateEnd { get; set; }

        /// <summary>
        /// 名稱  ：Is Dangerous(危險物品)
        /// DB位置：TWSQLDB.dbo.Product.IsShipDanger
        /// 說明  ：
        /// </summary>
        public string API_Product_IsShipDanger { get; set; }

        /// <summary>
        /// 名稱  ：Is18(18歲限制)
        /// DB位置：TWSQLDB.dbo.product.Is18
        /// 說明  ：不用填
        /// </summary>
        public string API_Product_Is18 { get; set; }

        /// <summary>
        /// 名稱  ：IsChokingDanger(窒息危險物品)
        /// DB位置：TWSQLDB.dbo.product.IsChokingDanger
        /// 說明  ：不用填
        /// </summary>
        public string API_Product_IsChokingDanger { get; set; }

        #endregion

        #region Inventory Price Setting

        /// <summary>
        /// 名稱  ：Market Price(市場建議售價)
        /// DB位置：TWSQLDB.dbo.Item.MarketPrice
        /// 說明  ：
        /// </summary>
        public decimal? API_Item_MarketPrice { get; set; }

        /// <summary>
        /// 名稱  ：Selling Price(網路售價)
        /// DB位置：TWSQLDB.dbo.Item.PriceCash
        /// 說明  ：
        /// </summary>
        public decimal API_Item_PriceCash { get; set; }

        /// <summary>
        /// 名稱  ：Manufacturer's ID (製造商)
        /// DB位置：TWSQLDB.dbo.Product.ManufacturerID
        /// 說明  ：
        /// </summary>
        public int API_Product_ManufacturerID { get; set; }

        /// <summary>
        /// 名稱  ：Manufacturer Part # / ISBN(製造商商品編號)
        /// DB位置：TWSQLDB.dbo.Product.MenufacturePartNum
        /// 說明  ：
        /// </summary>
        public string API_Product_MenufacturePartNum { get; set; }

        /// <summary>
        /// 名稱  ：Seller Part #(商家商品編號)
        /// DB位置：TWSQLDB.dbo.Product.SellerProductID
        /// 說明  ：
        /// </summary>
        public string API_Product_SellerProductID { get; set; }

        /// <summary>
        /// 名稱  ：Product Bar Code(條碼)
        /// DB位置：TWSQLDB.dbo.Product.BarCode
        /// 說明  ：
        /// </summary>
        public string API_Product_BarCode { get; set; }

        /// <summary>
        /// 名稱  ：Inventory(庫存)
        /// DB位置：TWSQLDB.dbo.Item.Qty
        /// 說明  ：Item.Qty(限量庫存)；Itemstock.Qty(一般庫存)
        /// </summary>
        public int API_Item_Inventory { get; set; }

        /// 名稱  ：QtyLimit(限購數量)
        /// DB位置：TTWSQLDB.dbo.Item.QtyLimit
        /// 說明  ：int QtyLimit = 0;
        /// </summary>
        public int API_Item_QtyLimit { get; set; }

        /// <summary>
        /// 限量數量：textbox => item.Qty
        /// </summary>
        public int API_Item_Qty { get; set; }

        /// <summary>
        /// 名稱  ：Cost(供應商的成本)
        /// DB位置：TWSQLDB.dbo.product.Cost
        /// 說明  ：不用填
        /// </summary>
        public decimal? API_Product_Cost { get; set; }

        /// 名稱  ：PriceLocalship(本地運費)
        /// DB位置：TTWSQLDB.dbo.Item.PriceLocalship
        /// 說明  ：decimal PriceLocalship = 0;
        /// </summary>
        public decimal API_Item_PriceLocalship { get; set; }


        #endregion

        #region Item重複欄位

        /// <summary>
        /// 名稱  ：Product Description(商品描述)
        /// DB位置：TWSQLDB.dbo.Item.DescTW
        /// 說明  ：Product.DescriptionTW(nvarchar(500)) = Item.DescTW(nvarchar(4000))，字超過500，SQL會自動刪除
        /// </summary>
        public string API_Item_DescTW { get; set; }

        /// <summary>
        /// 名稱  ：ItemDesc(英文描述)
        /// DB位置：TTWSQLDB.dbo.Item.ItemDesc
        /// 說明  ：Item.ItemDesc = Product.Description
        /// </summary>
        public string API_Item_ItemDesc { get; set; }

        /// <summary>
        /// 名稱  ：Product Model(商品型號)
        /// DB位置：TWSQLDB.dbo.Item.Model
        /// 說明  ：(Product.Model = Item.Model)
        /// </summary>
        public string API_Item_Model { get; set; }

        /// <summary>
        /// 名稱  ：DelvType(配送方式)
        /// DB位置：TTWSQLDB.dbo.Item.DelvType
        /// 說明  ：Item.DelvType = Product. DelvType
        /// </summary>
        public int API_Item_DelvType { get; set; }

        /// <summary>
        /// 名稱  ：ProductID(產品編號)
        /// DB位置：TTWSQLDB.dbo.Item.ProductID
        /// 說明  ：Item.ProductID = Product.ID
        /// </summary>
        public int API_Item_ProductID { get; set; }

        /// <summary>
        /// 名稱  ：PriceCard(刷卡價)
        /// DB位置：TTWSQLDB.dbo.Item.PriceCard
        /// 說明  ：Item.PriceCard = Item.PriceCash
        /// </summary>
        public decimal API_Item_PriceCard { get; set; }

        /// <summary>
        /// 名稱  ：ManufacturerID(製造商ID)
        /// DB位置：TTWSQLDB.dbo.Item.ManufacturerID
        /// 說明  ：Item.ManufacturerID = Product.ManufacturerID
        /// </summary>
        public int API_Item_ManufactureID { get; set; }

        /// <summary>
        /// 名稱  ：SellerID(商家ID)
        /// DB位置：TTWSQLDB.dbo.Item.SellerID
        /// 說明  ：Item.SellerID = Product.SellerID
        /// </summary>
        public int API_Item_SellerID { get; set; }

        /// <summary>
        /// 名稱  ：Note(備註)
        /// DB位置：TTWSQLDB.dbo.Item.Note
        /// 說明  ：Item.Note = Product.Note
        /// </summary>
        public string API_Item_Note { get; set; }

        /// <summary>
        /// 名稱  ：PicStart(產品圖片第一張)
        /// DB位置：TTWSQLDB.dbo.Item.PicStart
        /// 說明  ：Item.PicStart = Product.PicStart
        /// </summary>
        public int? API_Item_PicStart { get; set; }

        /// <summary>
        /// 名稱  ：PicEnd(產品圖片最後一張)
        /// DB位置：TTWSQLDB.dbo.Item.PicEnd
        /// 說明  ：Item.PicEnd = Product.PicEnd
        /// </summary>
        public int? API_Item_PicEnd { get; set; }

        /// <summary>
        /// 名稱  ：DateDel(商品臨時ID)
        /// DB位置：TTWSQLDB.dbo.Item.DateDel
        /// 說明  ：Item.DateDel > Item.DateEnd 
        /// </summary>
        public DateTime API_Item_DateDel { get; set; }

        /// <summary>
        /// 名稱  ：UpdateUser(更新者)
        /// DB位置：TWSQLDB.dbo.Item.UpdateUser
        /// 說明  ：輸入更新時登入者的UserID
        /// </summary>
        public string API_Item_UpdateUser { get; set; }
        #endregion

        #region Product重複欄位

        /// <summary>
        /// 名稱  ：Website Short Title(商品名稱)
        /// DB位置：TWSQLDB.dbo.Product.Name
        /// 說明  ：
        /// </summary>
        public string API_Product_productName { get; set; }

        /// <summary>
        /// 名稱  ：NameTW(名稱TW)
        /// DB位置：TWSQLDB.dbo.product.NameTW
        /// 說明  ：與創建商品資料的Product.Name一致(Product.Name = Product.NameTW = Item.Name)
        ///         Product.productName = Product.NameTW
        /// </summary>
        public string API_Product_NameTW { get; set; }

        /// <summary>
        /// 名稱  ：SellerID(賣家ID)
        /// DB位置：TWSQLDB.dbo.product.SellerID
        /// 說明  ：
        /// </summary>
        public int API_Product_SellerID { get; set; }

        /// <summary>
        /// 名稱  ：DescriptionTW(商品描述)
        /// DB位置：TWSQLDB.dbo.Product.DescriptionTW
        /// 說明  ：Product.DescriptionTW(nvarchar(500)) = Item.DescTW(nvarchar(4000))，字超過500，SQL會自動刪除
        /// </summary>
        public string API_Product_DescriptionTW { get; set; }

        /// <summary>
        /// 名稱  ：CreateUser(建立人)
        /// DB位置：TWSQLDB.dbo.product.CreateUser
        /// 說明  ：輸入創建時登入者的UserID
        /// </summary>
        public string API_Product_CreateUser { get; set; }

        /// <summary>
        /// 名稱  ：UpdateUser(更新人)
        /// DB位置：TWSQLDB.dbo.product.UpdateUser
        /// 說明  ：輸入更新時登入者的UserID
        /// </summary>
        public string API_Product_UpdateUser { get; set; }

        /// <summary>
        /// 名稱  ：DelvType(Delv類型)
        /// DB位置：TWSQLDB.dbo.product.DelvType
        /// 說明  ：
        ///        Item.ShipType(N) ==> Product.DelvType(7)
        ///        Item.ShipType(S) ==> Product.DelvType(2)
        ///        Item.DelvType = Product.DelvType
        /// </summary>
        public int? API_Product_DelvType { get; set; }
        #endregion

        #region 頁面未填。API寫入固定值。

        /// <summary>
        /// 名稱  ：Updated(更新)
        /// DB位置：TWSQLDB.dbo.product.Updated
        /// 說明  ：int Updated = 0;
        /// </summary>
        public int API_Product_Updated { get; set; }

        /// <summary>
        /// 名稱  ：UpdateDate(更新日期)
        /// DB位置：TWSQLDB.dbo.product.UpdateDate
        /// 說明  ：輸入更新創建商品資訊的當下日期
        /// </summary>
        public DateTime? API_Product_UpdateDate { get; set; }


        //**********************************************************************

        ///// <summary>
        ///// 名稱  ：Is Limit(是否限量)
        ///// DB位置：無
        ///// 說明  ：判斷庫存是否限量，無欄位
        /////        若為限量IsLimit = "Yes" ，放入Item.Qty(限量庫存)
        /////        若非限量IsLimit = "No" ，放入Itemstock.Qty(一般庫存)
        ///// </summary>
        //public string API_Item_IsLimit { get; set; }

        ///// <summary>
        ///// 名稱  ：PicStart(產品圖片第一張)
        ///// DB位置：TWSQLDB.dbo.product.PicStart
        ///// 說明  ：
        ///// </summary>
        //public int? API_Product_PicStart { get; set; }

        ///// <summary>
        ///// 名稱  ：PicEnd(產品圖片最後一張)
        ///// DB位置：TWSQLDB.dbo.product.PicEnd
        ///// 說明  ：
        ///// </summary>
        //public int? API_Product_PicEnd { get; set; }

        ///// <summary>
        ///// 名稱  ：SourceTable(來源表)
        ///// DB位置：TWSQLDB.dbo.product.SourceTable
        ///// 說明  ：string SourceTable = "SellerPortal";
        ///// </summary>
        //public string API_Product_SourceTable { get; set; }

        ///// <summary>
        ///// 名稱  ：InvoiceType(發票)
        ///// DB位置：TWSQLDB.dbo.product.InvoiceType
        ///// 說明  ：int InvoiceType = 0;
        ///// </summary>
        //public int? API_Product_InvoiceType { get; set; }

        ///// <summary>
        ///// 名稱  ：SaleType(販售型態)
        ///// DB位置：TWSQLDB.dbo.product.SaleType
        ///// 說明  ：int SaleType = 0;
        ///// </summary>
        //public int? API_Product_SaleType { get; set; }

        ///// <summary>
        ///// 名稱  ：CreateDate(建立日期)
        ///// DB位置：TWSQLDB.dbo.product.CreateDate
        ///// 說明  ：輸入創建商品資訊的當下日期
        ///// </summary>
        //public DateTime API_Product_CreateDate { get; set; }

        ///// <summary>
        ///// 名稱  ：Tax(稅)
        ///// DB位置：TWSQLDB.dbo.product.Tax
        ///// 說明  ：decimal Tax = 0;
        ///// </summary>
        //public decimal? API_Product_Tax { get; set; }

        ///// <summary>
        ///// 名稱  ：Is Market Place(是否為商品價格)
        ///// DB位置：TWSQLDB.dbo.product.IsMarket
        ///// 說明  ：string IsMarket = "Y";
        ///// </summary>
        //public string API_Product_IsMarket { get; set; }

        ///// <summary>
        ///// 名稱  ：Spec(html規格)
        ///// DB位置：TWSQLDB.dbo.Product.Spec
        ///// 說明  ：
        ///// </summary>
        //public string API_Product_Spec { get; set; }

        #endregion

        #region 必填未分類

        /// <summary>
        /// 名稱  ：Status(上下架)
        /// DB位置：TTWSQLDB.dbo.Item.Status
        /// 說明  ：下價商品不得修改
        /// </summary>
        public int API_Item_Status { get; set; }

        /// <summary>
        /// 名稱  ：ID(上架商品編號)
        /// DB位置：TTWSQLDB.dbo.Item.ID
        /// 說明  ：紀錄查詢編號
        /// </summary>
        public int API_Item_ID { get; set; }

        /// <summary>
        /// 名稱  ：Industry ID (主類別ID)
        /// DB位置：
        /// 說明  ：抓此與SellerID來找佣金費率
        /// </summary>
        public int API_Record_IndustryID { get; set; }

        /// <summary>
        /// 名稱  ：Manufacturer's Name (製造商名稱)
        /// DB位置：TWSQLDB.dbo.Product.ManufacturerID
        /// 說明  ：用製造商名稱搜尋製造商ID
        /// </summary>
        public string API_Record_ManufacturerName { get; set; }

        /// <summary>
        /// 名稱  ：SubCategoryID(類別)
        /// DB位置：TTWSQLDB.dbo.Item.CategoryID
        /// 說明  ：使用者可以不用填寫，不填寫時依照開頭填入，填寫時依照填寫內容填入
        /// </summary>
        public int API_Item_CategoryID { get; set; }

        /// <summary> 
        /// 名稱  ：Item Images(商品圖) 
        /// DB位置：xxxx 
        /// 說明  ：數條圖片的URL連結 
        /// </summary> 
        public List<string> API_Item_ItemImages { get; set; }

        /// <summary> 
        /// 名稱  ：Product Property(商品屬性) 
        /// DB位置：TWSQLDB 
        /// 說明  ：數筆商品屬性的資料 
        /// </summary> 
        public List<SaveProductProperty> API_Product_Property { get; set; }

        //拿來記錄選擇的類別地圖
        public string API_Record_CategoryNameMap { get; set; }

        /// <summary>
        /// 名稱   : Item 商品成色
        /// DB位置：TWSQLDB.dbo.Item.IsNew 
        /// 說明  ：紀錄商品是新品或福利品 ("Y" or "N") 
        /// </summary>
        public string API_Item_IsNew { get; set; }
        
        #endregion

        /// <summary>
        /// 名稱  ：原始售價
        /// DB位置：TWSQLDB.dbo.Item.PriceCash
        /// 說明  ：售價修改需發信通知PM
        /// </summary>
        public decimal OriginalItemPriceCash { get; set; }

        /// <summary>
        /// 名稱  ：毛利率
        /// DB位置：TWSQLDB.dbo.Itemdisplayprice.ItemProfitPercent
        /// 說明  ：顯示毛利率
        /// </summary>
        public decimal? API_Itemdisplayprice_GrossMargin { get; set; }

        /// <summary>
        /// 名稱  ：安全庫存量
        /// DB位置：TWSQLDB.dbo.ItemStock.SafeQty
        /// 說明  ：安全庫存量，供庫存警示使用
        /// </summary>
        public int API_Item_ItemStockSafeQty { get; set; }

        /// <summary>
        /// 帳戶類型 Seller or Vendor
        /// </summary>
        public string AccountTypeCode { get; set; }

    }
}
