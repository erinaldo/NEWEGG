using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    /*---------- add by Ian and Thisway ----------*/
    /// <summary>
    /// 名稱  ：TWSQLDB.dbo.Product的所有欄位
    /// DB位置：TWSQLDB.dbo.Product
    /// 說明  ：目前40個欄位都在裡面(更新日期：2014/01/14)
    /// </summary>
    public class ProductsInfo
    {
        #region 創建商品資訊，給商家填寫的欄位
        /// <summary>
        /// 名稱  ：Seller Part #(商家商品編號)
        /// DB位置：TWSQLDB.dbo.Product.SellerProductID
        /// 說明  ：
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// 名稱  ：Manufacturer's ID (製造商)
        /// DB位置：TWSQLDB.dbo.Product.ManufacturerID
        /// 說明  ：
        /// </summary>
        public int ManufacturerID { get; set; }

        /// <summary>
        /// 名稱  ：Manufacturer Part # / ISBN(製造商商品編號)
        /// DB位置：TWSQLDB.dbo.Product.MenufacturePartNum
        /// 說明  ：
        /// </summary>
        public string MenufacturePartNum { get; set; }

        /// <summary>
        /// 名稱  ：UPC(商品條碼)
        /// DB位置：TWSQLDB.dbo.Product.UPC
        /// 說明  ：
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 名稱  ：Website Short Title(商品名稱)
        /// DB位置：TWSQLDB.dbo.Product.Name
        /// 說明  ：
        /// </summary>
        public string productName { get; set; }

        /// <summary>
        /// 名稱  ：DescriptionTW(商品描述)
        /// DB位置：TWSQLDB.dbo.Product.DescriptionTW
        /// 說明  ：Product.DescriptionTW(nvarchar(500)) = Item.DescTW(nvarchar(4000))，字超過500，SQL會自動刪除
        /// </summary>
        public string DescriptionTW { get; set; }

        /// <summary>
        /// 名稱  ：product's note(商品注意事項)
        /// DB位置：TWSQLDB.dbo.Product.Note
        /// 說明  ：
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 名稱  ：product Model(商品型號)
        /// DB位置：TWSQLDB.dbo.Product.Model
        /// 說明  ：Product.Model = Item.Model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 名稱  ：Product Bar Code(條碼)
        /// DB位置：TWSQLDB.dbo.Product.BarCode
        /// 說明  ：
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 名稱  ：Product Length(長度)
        /// DB位置：TWSQLDB.dbo.Product.Length
        /// 說明  ：
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// 名稱  ：Product Width(寬度)
        /// DB位置：TWSQLDB.dbo.Product.Width
        /// 說明  ：
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 名稱  ：Product Height(高度)
        /// DB位置：TWSQLDB.dbo.Product.Height
        /// 說明  ：
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 名稱  ：Product Weight(重量)
        /// DB位置：TWSQLDB.dbo.Product.Weight
        /// 說明  ：
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 名稱  ：Product Condition(商品成色)
        /// DB位置：TWSQLDB.dbo.Product.Status
        /// 說明  ：
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 名稱  ：Warranty(保固期)
        /// DB位置：TWSQLDB.dbo.Product.Warranty
        /// 說明  ：
        /// </summary>
        public int? Warranty { get; set; }

        /// <summary>
        /// 名稱  ：Is Dangerous(危險物品)
        /// DB位置：TWSQLDB.dbo.Product.IsShipDanger
        /// 說明  ：
        /// </summary>
        public string IsShipDanger { get; set; }

        /// <summary>
        /// 名稱  ：Spec(html規格)
        /// DB位置：TWSQLDB.dbo.Product.Spec
        /// 說明  ：
        /// </summary>
        public string Spec { get; set; }
        #endregion

        #region SellerPortal自動擷取，放入API
        /// <summary>
        /// 名稱  ：SellerID(賣家ID)
        /// DB位置：TWSQLDB.dbo.product.SellerID
        /// 說明  ：
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 名稱  ：CreateUser(建立人)
        /// DB位置：TWSQLDB.dbo.product.CreateUser
        /// 說明  ：輸入創建時登入者的UserID
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 名稱  ：UpdateUser(更新人)
        /// DB位置：TWSQLDB.dbo.product.UpdateUser
        /// 說明  ：輸入更新時登入者的UserID
        /// </summary>
        public string UpdateUser { get; set; }
        #endregion

        #region 與使用者輸入的某個欄位相同值(創建商品資料)
        /// <summary>
        /// 名稱  ：NameTW(名稱TW)
        /// DB位置：TWSQLDB.dbo.product.NameTW
        /// 說明  ：與創建商品資料的Product.Name一致(Product.Name = Product.NameTW = Item.Name)
        ///         Product.productName = Product.NameTW
        /// </summary>
        public string NameTW { get; set; }

        /// <summary>
        /// 名稱  ：Description(英文描述)
        /// DB位置：TWSQLDB.dbo.product.Description
        /// 說明  ：與創建商品資料的Product.DescriptionTW一致(Product.DescriptionTW = Product.Description)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 名稱  ：DelvType(Delv類型)
        /// DB位置：TWSQLDB.dbo.product.DelvType
        /// 說明  ：
        ///        Item.ShipType(N) ==> Product.DelvType(7)
        ///        Item.ShipType(S) ==> Product.DelvType(2)
        ///        Item.DelvType = Product.DelvType
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// 名稱  ：PicStart(產品圖片第一張)
        /// DB位置：TWSQLDB.dbo.product.PicStart
        /// 說明  ：
        /// </summary>
        public int PicStart { get; set; }

        /// <summary>
        /// 名稱  ：PicEnd(產品圖片最後一張)
        /// DB位置：TWSQLDB.dbo.product.PicEnd
        /// 說明  ：
        /// </summary>
        public int PicEnd { get; set; }
        #endregion

        #region API寫入，放入Product DB(創建商品時給固定的值)
        /// <summary>
        /// 名稱  ：SourceTable(來源表)
        /// DB位置：TWSQLDB.dbo.product.SourceTable
        /// 說明  ：string SourceTable = "SellerPortal";
        /// </summary>
        public string SourceTable { get; set; }

        /// <summary>
        /// 名稱  ：InvoiceType(發票)
        /// DB位置：TWSQLDB.dbo.product.InvoiceType
        /// 說明  ：int InvoiceType = 0;
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 名稱  ：SaleType(販售型態)
        /// DB位置：TWSQLDB.dbo.product.SaleType
        /// 說明  ：int SaleType = 0;
        /// </summary>
        public int SaleType { get; set; }

        /// <summary>
        /// 名稱  ：Updated(更新)
        /// DB位置：TWSQLDB.dbo.product.Updated
        /// 說明  ：int Updated = 0;
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// 名稱  ：CreateDate(建立日期)
        /// DB位置：TWSQLDB.dbo.product.CreateDate
        /// 說明  ：輸入創建商品資訊的當下日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 名稱  ：UpdateDate(更新日期)
        /// DB位置：TWSQLDB.dbo.product.UpdateDate
        /// 說明  ：輸入更新創建商品資訊的當下日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 名稱  ：Tax(稅)
        /// DB位置：TWSQLDB.dbo.product.Tax
        /// 說明  ：decimal Tax = 0;
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// 名稱  ：Is Market Place(是否為商品價格)
        /// DB位置：TWSQLDB.dbo.product.IsMarket
        /// 說明  ：string IsMarket = "Y";
        /// </summary>
        public string IsMarket { get; set; }
        #endregion

        #region 系統自動產生的欄位
        /// <summary>
        /// 名稱  ：ID(新蛋商品編號)
        /// DB位置：TWSQLDB.dbo.product.ID
        /// 說明  ：系統自動產生
        /// </summary>
        //public int ID { get; set; }

        /// <summary>
        /// 名稱  ：FK(外來鍵)
        /// DB位置：TWSQLDB.dbo.product.FK
        /// 說明  ：系統自動產生
        /// </summary>
        //public int FK { get; set; }
        #endregion

        #region 先不管的欄位
        /// <summary>
        /// 名稱  ：Cost(供應商的成本)
        /// DB位置：TWSQLDB.dbo.product.Cost
        /// 說明  ：不用填
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 名稱  ：TradeTax(進口關稅)
        /// DB位置：TWSQLDB.dbo.product.TradeTax
        /// 說明  ：不用填
        /// </summary>
        public decimal TradeTax { get; set; }

        /// <summary>
        /// 名稱  ：Is18(18歲限制)
        /// DB位置：TWSQLDB.dbo.product.Is18
        /// 說明  ：不用填
        /// </summary>
        public string Is18 { get; set; }

        /// <summary>
        /// 名稱  ：IsChokingDanger(窒息危險物品)
        /// DB位置：TWSQLDB.dbo.product.IsChokingDanger
        /// 說明  ：不用填
        /// </summary>
        public string IsChokingDanger { get; set; }

        /// <summary>
        /// 名稱  ：SPECLabel(規格標籤)
        /// DB位置：TWSQLDB.dbo.product.SPECLabel
        /// 說明  ：不用填
        /// </summary>
        public string SPECLabel { get; set; }
        #endregion
    }
    /*---------- end by Ian and Thisway ----------*/
}
