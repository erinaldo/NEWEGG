using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// 商品清單欄位
    /// </summary>
    public class ItemInfoList
    {
        public ItemInfoList()
        {
            this.ItemCategory = new Item_ItemCategory();
        }

        public int ProductID { get; set; }

        public int ItemID { get; set; }

        public int ItemSellerID { get; set; }

        public string SellerName { set; get; }

        public int Status { get; set; }

        /// <summary>
        /// 上下架
        /// </summary>
        public int ItemStatus { get; set; }

        public string ItemName { get; set; }

        public string ProductUPC { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public decimal ItemPriceCash { get; set; }

        /// <summary>
        /// 市場建議售價
        /// </summary>
        public decimal? ItemMarketPrice { get; set; }

        /// <summary>
        /// ShipType --遞送方式S：Seller; N：Newegg
        /// </summary>
        public string ItemShipType { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        private decimal? itemShipFee;
        public decimal? ItemShipFee
        {
            get
            {
                return itemShipFee;
            }
            set
            {
                if (value == null)
                {
                    itemShipFee = 0.0000m;
                }
                else
                {
                    itemShipFee = value;
                }
            }
        }

        /// <summary>
        /// 商家商品編號(SellerPartNum)
        /// </summary>
        public string ProductSellerProductID { get; set; }

        /// <summary>
        /// 製造商商品編號
        /// </summary>
        public string ProductManufacturerPartNum { get; set; }

        /// <summary>
        /// 製造商
        /// </summary>
        public int ItemManufacturerID { get; set; }

        public string ManufacturerName { set; get; }

        /// <summary>
        /// 主類別屬性 第0層
        /// </summary>
        public int Industry { get; set; }

        /// <summary>
        /// 次類別屬性 第1層
        /// </summary>
        public int SubcategoryID { get; set; }

        /// <summary>
        /// 分類屬性 第2層
        /// </summary>
        public int ItemCategoryID { get; set; }

        /// <summary>
        /// 主類別屬性名稱 第0層
        /// </summary>
        public string IndustryName { get; set; }

        /// <summary>
        /// 次類別屬性名稱 第1層
        /// </summary>
        public string SubcategoryName { get; set; }

        /// <summary>
        /// 分類屬性名稱 第2層
        /// </summary>
        public string ItemCategoryName { get; set; }

        /// <summary>
        /// 商品成色顯示 0:未標 1:全新 2:返修、拆封
        /// </summary>
        public string ProductStatus { get; set; }

        /// <summary>
        /// DB商品成色欄位
        /// </summary>
        public int ProductStatusInt { get; set; }

        /// <summary>
        /// 前台商品成色
        /// </summary>
        /// <remarks>
        /// NULL:未指定
        /// Y:新品
        /// N:褔利品
        /// </remarks>
        public string IsNew { get; set; }  // 2014.10.27 add by Smoke

        /// <summary>
        /// Seller Portal 商品成色
        /// </summary>
        /// <remarks>
        /// 0:未指定
        /// 1:新品
        /// 2:褔利品
        /// </remarks>
        public int Condition { get; set; }   // 2014.10.27 add by Smoke

        public DateTime ItemCreateDate { get; set; }

        public decimal? ProductDimension { get; set; }

        public string ImageSource { get; set; }

        public int ItemInventory { get; set; }

        public int ItemQty { get; set; }

        public int ItemReg { get; set; }

        public int ProductInventory { get; set; }

        public int ProductQty { get; set; }

        public int ProductReg { get; set; }

        public DateTime ItemDateStart { get; set; }

        public DateTime ItemDateEnd { get; set; }

        public string ItemUrl { get; set; }

        public string ProductNameUS { get; set; }

        /// <summary>
        /// 安全庫存量，from ItemStock.SafeQty
        /// </summary>
        public int ProductSafeQty { get; set; }

        public string AccountTypeCode { get; set; }

        public decimal? ProductCost { get; set; }

        public int UpdateUserID { get; set; }     //2014.7.2 add by ice

        /// <summary>
        /// 紀錄原始售價，若售價有變更發信通知
        /// </summary>
        public decimal OriginalItemPriceCash { get; set; } 


        /// <summary>
        /// 毛利率(from ItemDisplayPrice.ItemProfitPercent * 100)
        /// </summary>
        public decimal ItemGrossMargin { get; set; }
        
        // 規格品ID
        public int ItemTempID { get; set; }

        public Item_ItemCategory ItemCategory { get; set; }


    }

    /// <summary>
    /// 商品清單賣場跨分類類別
    /// </summary>
    public class Item_ItemCategory
    {       
        #region 第 1 個跨分類

        /// <summary>
        /// 第 1 個跨分類_第 1 層分類 ID
        /// </summary>
        public int? SubCategoryID_1_Layer1 { get; set; }
        public string SubCategoryID_1_Layer1_Name { get; set; }

        /// <summary>
        /// 第 1 個跨分類_第 2 層分類 ID
        /// </summary>
        public int? SubCategoryID_1_Layer2 { get; set; }
        public string SubCategoryID_1_Layer2_Name { get; set; }

        #endregion 第 1 個跨分類

        #region 第 2 個跨分類

    /// <summary>
        /// 第 1 個跨分類_第 1 層分類 ID
        /// </summary>
        public int? SubCategoryID_2_Layer1 { get; set; }
        public string SubCategoryID_2_Layer1_Name { get; set; }

        /// <summary>
        /// 第 1 個跨分類_第 2 層分類 ID
        /// </summary>
        public int? SubCategoryID_2_Layer2 { get; set; }
        public string SubCategoryID_2_Layer2_Name { get; set; }

        #endregion 第 2 個跨分類
    }

    /// <summary>
    /// 商品已由新蛋運送頁面欄位
    /// </summary>
    public class ShippedByNeweggList
    {
        /// <summary>
        /// 商家
        /// </summary>
        public int ItemSellerID { get; set; }

        public string ItemShipType { get; set; }

        public string SellerName { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string ProductSellerProductID { get; set; }

        /// <summary>
        /// 製造商商品編號
        /// </summary>
        public string ProductManufacturerPartNum { get; set; }

        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 生產廠商
        /// </summary>
        public int ItemManufacturerID { get; set; }

        public string ManufacturerName { get; set; }

        /// <summary>
        /// UPC代碼
        /// </summary>
        public string ProductUPC { get; set; }

        /// <summary>
        /// 存貨
        /// </summary>
        public int ItemQty { get; set; }

        /// <summary>
        /// 最終售價
        /// </summary>
        public decimal ItemPriceCash { get; set; }

        public decimal ItemMarketPrice { get; set; }

        /// <summary>
        /// 商品狀態
        /// </summary>
        public int ItemStatus { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal? ItemShipFee { get; set; }

        /// <summary>
        /// 商品體積
        /// </summary>
        public decimal? Volume { get; set; }

    }

    public class ModifyResult
    {
        public int ItemSellerID { get; set; }
        public int ItemID { get; set; }
        public int ProductSellerProductID { get; set; }

        public int ItemStatus { get; set; }
        public int ItemQty { get; set; }
        public string ItemShipType { get; set; }
        public decimal ItemPriceCash { get; set; }
        public decimal ItemMarketPrice { get; set; }
        public decimal ItemShipFee { get; set; }

    }

    public class ProductDetail
    {
        public int Condition { get; set; }

        public int ID { get; set; }
        public DateTime? InDate { get; set; }
        public int? InUserID { get; set; }

        public int ManufactureID { get; set; }
        public string ManufacturePartNum { get; set; }
        public string Name { get; set; }
        public string NameTW { get; set; }

        public int ProductID { get; set; }
        public int Qty { get; set; }
        public int QtyReg { get; set; }
        public int SafeQty { get; set; }

        public int SellerID { get; set; }

        public string SellerProductID { get; set; }

        public string ShipType { get; set; }
        public string Status { get; set; }
        public string UPC { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
    }

    /// <summary>
    /// 存取待審核上架商品 Model
    /// </summary>
    public class PendingItem
    {
        public int SellerID { get; set; }

        public int ItemID { get; set; }
    }

    #region 匯出Excel

    /// <summary>
    /// 匯出時用到的所有欄位 Model
    /// </summary>
    public class ExportToExcelModel
    {
        // 資料來源：ItemInfoList 的 ItemID
        [DisplayName("新蛋賣場編號")]
        public int ItemID { get; set; }

        // 資料來源：ItemInfoList 的 ItemName
        [DisplayName("商品名稱(品名)")]
        public string WebsiteShortTitle { get; set; }

        // 資料來源：TWSQLDB.item.DescTW
        [DisplayName("商品描述(內文)")]
        public string ProductDescription { get; set; }

        // 資料來源：TWSQLDB.item.Note
        [DisplayName("注意事項")]
        public string ProductNote { get; set; }

        // 資料來源：TWSQLDB.item.Sdesc
        [DisplayName("簡要描述(主賣點2)")]
        public string ProductShortDesc { get; set; }

        // 資料來源：TWSQLDB.item.Spechead
        [DisplayName("簡要條列式描述(主賣點1)")]
        public string ProductFeatureTitle { get; set; }

        // 資料來源：ItemInfoList 的 ProductSellerProductID
        [DisplayName("商家商品編號")]
        public string ProductSellerProductID { get; set; }

        // 資料來源：ItemInfoList 的 ProductCost
        [DisplayName("成本(seller非必填、vendor必填)")]
        public decimal? Cost { get; set; }

        // 資料來源：ItemInfoList 的 ItemPriceCash
        [DisplayName("賣價(user價)")]
        public decimal? SellingPrice { get; set; }

        // 資料來源：ItemInfoList 的 ItemMarketPrice
        [DisplayName("市場建議售價")]
        public decimal? MarketPrice { get; set; }

        // 資料來源：TWSQLDB.product.Warranty
        [DisplayName("保固")]
        public int? Warranty { get; set; }

        // 資料來源：ItemInfoList 的 ProductInventory
        [DisplayName("庫存量")]
        public int? Inventory { get; set; }

        // 資料來源：TWSQLDB.item.DelvDate
        [DisplayName("到貨天數")]
        public string DelvDay { get; set; }
    }

    /// <summary>
    /// 匯出時用到的所有欄位 Model  20150818
    /// </summary>
    public class ExportToExcel
    {
        // 資料來源: ItemInfoList 的 IndustryName
        [DisplayName("第一層分類")]
        public string IndustryName { get; set; }

        // 資料來源: ItemInfoList 的 SubcategoryName
        [DisplayName("第二層分類")]
        public string SubcategoryName { get; set; }

        // 資料來源: ItemInfoList 的 ItemCategoryName
        [DisplayName("第三層分類")]
        public string ItemCategoryName { get; set; }

        // 資料來源: ItemInfoList 的 SubcategoryName
        [DisplayName("第一跨分類次類別")]
        public string SubCategoryLayer1_Name { get; set; }

        // 資料來源: ItemInfoList 的 ItemCategoryName
        [DisplayName("第一跨分類子類別")]
        public string ItemCategoryLayer1_Name { get; set; }

        // 資料來源: ItemInfoList 的 SubcategoryName
        [DisplayName("第二跨分類次類別")]
        public string SubcategoryLayer2_Name { get; set; }

        // 資料來源: ItemInfoList 的 ItemCategoryName
        [DisplayName("第二跨分類子類別")]
        public string ItemCategoryLayer2_Name { get; set; }

        // 資料來源：ItemInfoList 的 ItemID
        [DisplayName("新蛋賣場編號")]
        public int ItemID { get; set; }

        // 資料來源：ItemInfoList 的 ProductSellerProductID
        [DisplayName("商家商品編號")]
        public string ProductSellerProductID { get; set; }

        // 資料來源：ItemInfoList 的 ItemName
        [DisplayName("商品名稱")]
        public string ItemName { get; set; }

        // 資料來源：ItemInfoList 的 ItemMarketPrice
        [DisplayName("建議售價(元)")]
        public decimal? ItemMarketPrice { get; set; }

        // 資料來源：ItemInfoList 的 ItemPriceCash
        [DisplayName("售價(元)")]
        public decimal? ItemPriceCash { get; set; }

        // 資料來源：ItemInfoList 的 ProductCost
        [DisplayName("成本價(元)")]
        public decimal? ProductCost { get; set; }

        // 資料來源：ItemInfoList 的 ProductInventory
        [DisplayName("可售數量")]
        public int? ProductInventory { get; set; }

        // 資料來源：ItemInfoList 的 ItemStatus
        [DisplayName("商品狀態")]
        public string ItemStatus { get; set; }

        // 資料來源 : ItemInfoList 的 ItemShipType
        [DisplayName("出貨方")]
        public string ItemShipType { get; set; }
    }

    /// <summary>
    /// 匯出 Excel 訂單列表 Model
    /// </summary>
    public class DownloadItemListModel
    {
        // 訂單篩選條件
        public ItemSearchCondition itemSearchCondition { get; set; }

        // 訂單列表
        public List<ItemInfoList> dataList { get; set; }

        // Excel 檔案名稱
        public string fileName { get; set; }

        // Excel 工作表名稱
        public string sheetName { get; set; }

        // Excel 抬頭行數
        public int titleLine { get; set; }
    }

    #endregion 匯出Excel
}