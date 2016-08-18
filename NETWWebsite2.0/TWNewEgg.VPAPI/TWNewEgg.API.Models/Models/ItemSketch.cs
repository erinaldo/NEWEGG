using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    #region 草稿

    public class ItemSketch
    {
        /// <summary>
        /// 草稿 ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 草稿狀態 (0:未送審, 99:刪除)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 國家ID
        /// </summary>
        public int CountryID { get; set; }

        /// <summary>
        /// 規格商品群組ID
        /// </summary>
        public int? GroupID { get; set; }

        /// <summary>
        /// 該賣場目前匯率
        /// </summary>
        public decimal CurrencyAverageExchange { get; set; }

        /// <summary>
        /// 賣場資訊
        /// </summary>
        public ItemSketch_Item Item { get; set; }

        /// <summary>
        /// 產品資訊
        /// </summary>
        public ItemSketch_Product Product { get; set; }

        /// <summary>
        /// 產品庫存資訊
        /// </summary>
        public ItemSketch_ItemStock ItemStock { get; set; }

        /// <summary>
        /// 總價化後資料及區間變價資料
        /// </summary>
        public ItemSketchListItemDisplayPrice ItemDisplayPrice { get; set; }

        /// <summary>
        /// 賣場跨分類類別
        /// </summary>
        public ItemSketch_ItemCategory ItemCategory { get; set; }

        /// <summary>
        /// 建立及更新資訊
        /// </summary>
        public CreateAndUpdateIfno CreateAndUpdate { get; set; }

        /// <summary>
        /// 商品屬性
        /// </summary>
        public List<SaveProductProperty> SaveProductPropertyList { get; set; }

        public ItemSketch()
        {
            this.Item = new ItemSketch_Item();
            this.Product = new ItemSketch_Product();
            this.ItemStock = new ItemSketch_ItemStock();
            this.ItemDisplayPrice = new ItemSketchListItemDisplayPrice();
            this.ItemCategory = new ItemSketch_ItemCategory();
            this.CreateAndUpdate = new CreateAndUpdateIfno();
            this.SaveProductPropertyList = new List<SaveProductProperty>();
            this.Product.PicPatch_Edit = new List<string>();

            // 預設值
            this.Item.DateStart = DateTime.Now;
            this.Item.DateEnd = Item.DateStart.AddYears(2099 - Item.DateStart.Year);
            this.Item.IsNew = "Y";
            this.Item.DelvDate = "1-7";
            this.Item.ItemPackage = "0";
            this.Item.ShipType = "S";
            this.Item.ItemQty = 0;
            this.Item.ItemQtyReg = 0;
            this.Item.QtyLimit = 0;
            this.Item.CanSaleLimitQty = 0;
            this.ItemStock.InventoryQty = 0;
            this.ItemStock.InventoryQtyReg = 0;
            this.ItemStock.InventorySafeQty = 0;
            this.ItemStock.CanSaleQty = 0;
            this.Product.Is18 = "N";
            this.Product.IsChokingDanger = "N";
            this.Product.IsShipDanger = "N";
            this.Product.Model = "";
            this.Product.Length = 1m;
            this.Product.Width = 1m;
            this.Product.Height = 1m;
            this.Product.Weight = 1m;
            this.Product.MenufacturePartNum = string.Empty;
            this.Status = 0;
            this.CountryID = 1;
            this.CurrencyAverageExchange = 1m;
        }
    }

    #region 資料轉換

    public enum IsNewDisplayName
    {
        全新 = 0,
        二手 = 1
    }

    public enum IsNewDBValue
    {
        Y = 0,
        N = 1
    }

    public enum ShipTypeDisplayName
    {
        供應商 = 0,
        新蛋 = 1
    }

    public enum ShipTypeDBValue
    {
        // 供應商
        S = 0,
        // 新蛋
        N = 1
    }

    #endregion

    #region Item (賣場)

    /// <summary>
    /// 草稿清單賣場資訊
    /// </summary>
    public class ItemSketch_Item
    {
        /// <summary>
        /// Item.ID 
        /// </summary>
        public int? ItemID { get; set; }

        /// <summary>
        /// ItemTemp.ID (ItemTempID)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ItemTemp.Status (審核狀態)
        /// 0:審核通過
        /// 1:未審核
        /// 2:未通過
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 正式商品狀態
        //0：上架
        //1：下架、未上架
        //2：強制下架(無上架機會)
        //3：售價異常(系統判斷下架)
        //99：刪除
        /// </summary>
        public int ItemStatus { get; set; }

        /// <summary>
        /// 賣場開始日期
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 賣場結束日期
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 賣場建立日期
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// 到貨天數
        /// </summary>
        public string DelvDate { get; set; }

        /// <summary>
        /// 商品包裝 (0:零售, 1:OEM)
        /// </summary>
        public string ItemPackage { get; set; }

        /// <summary>
        /// 限量數量
        /// </summary>
        public int? ItemQty { get; set; }

        /// <summary>
        /// 限量已賣
        /// </summary>
        public int? ItemQtyReg { get; set; }
        
        /// <summary>
        /// 限量可售量
        /// </summary>
        public int? CanSaleLimitQty { get; set; }

        /// <summary>
        /// 賣場連結
        /// </summary>
        public string ItemURL { get; set; }

        /// <summary>
        /// 商品成色 (Y:新品, N:二手)
        /// </summary>
        public string IsNew { get; set; }

        /// <summary>
        /// 市場建議售價
        /// </summary>
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 注意事項
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 現金價
        /// </summary>
        public decimal? PriceCash { get; set; }

        /// <summary>
        /// 限購數量
        /// </summary>
        public int? QtyLimit { get; set; }

        /// <summary>
        /// 商家 ID
        /// </summary>
        public int? SellerID { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 商品特色標題
        /// </summary>
        public string Sdesc { get; set; }

        /// <summary>
        /// 運送類型 (S:供應商, N:新蛋)
        /// </summary>
        public string ShipType { get; set; }

        /// <summary>
        /// 商品簡要描述
        /// </summary>
        public string Spechead { get; set; }

        /// <summary>
        /// 加價購
        /// </summary>
        public int? ShowOrder { get; set; }
        /// <summary>
        /// Discard4 
        /// Y:全新
        /// N:二手
        /// </summary>
        public string Discard4 { get; set; }
    }

    #endregion Item (賣場)

    #region Product (產品)

    /// <summary>
    /// 草稿清單產品資訊
    /// </summary>
    public class ItemSketch_Product
    {
        /// <summary>
        /// ProductTemp.ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 正式 Product.ID
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// 條碼
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 材積(公分)_高
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// 是否為18歲商品
        /// </summary>
        public string Is18 { get; set; }

        /// <summary>
        /// 是否有窒息危險
        /// </summary>
        /// <remarks></remarks>
        public string IsChokingDanger { get; set; }

        /// <summary>
        /// 是否有遞送危險
        /// </summary>
        public string IsShipDanger { get; set; }

        /// <summary>
        /// 材積(公分)_長
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// 製造商 ID
        /// </summary>
        public int? ManufactureID { get; set; }

        /// <summary>
        /// 製造商名稱
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 製造商商品編號 (沒有輸入欄位)
        /// </summary>
        public string MenufacturePartNum { get; set; }

        /// <summary>
        /// 型號
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 編輯圖片位置 (商品資訊使用，路徑：圖片機/pic/itemsketch/草稿ID前4碼/草稿ID未4碼_1_300.jpg)
        /// </summary>
        public List<string> PicPatch_Edit { get; set; }

        /// <summary>
        /// 草稿圖片位置 (草稿明細使用，路徑：圖片機/pic/itemsketch/草稿ID前4碼/草稿ID未4碼_1_60.jpg)
        /// </summary>
        public string PicPath_Sketch { get; set; }

        /// <summary>
        /// 產品圖片最後一張
        /// </summary>
        public int? PicEnd { get; set; }

        /// <summary>
        /// 產品圖片第一張
        /// </summary>
        public int? PicStart { get; set; }

        /// <summary>
        /// 草稿ID，產品編號
        /// </summary>
        public int? ProducttempID { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// UPC 編號
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品保固期(月)
        /// </summary>
        public int? Warranty { get; set; }

        /// <summary>
        /// 重量(公斤)
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 材積(公分)_寬
        /// </summary>
        public decimal? Width { get; set; }
    }

    #endregion Product (產品)

    #region ItemStock (產品庫存)

    /// <summary>
    /// 草稿清單產品庫存資訊
    /// </summary>
    public class ItemSketch_ItemStock
    {
        /// <summary>
        /// 可售數量
        /// </summary>
        public int? CanSaleQty { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int? InventoryQty { get; set; }

        /// <summary>
        /// 庫存已賣量
        /// </summary>
        public int? InventoryQtyReg { get; set; }

        /// <summary>
        /// 安全庫存量
        /// </summary>
        public int? InventorySafeQty { get; set; }
    }

    #endregion ItemStock (產品庫存)

    #region ItemDisplayPrice (總價化後資料及區間變價資料)

    /// <summary>
    /// 草稿清單總價化後資料及區間變價資料
    /// </summary>
    public class ItemSketchListItemDisplayPrice
    {
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? GrossMargin { get; set; }
    }

    #endregion ItemDisplayPrice (總價化後資料及區間變價資料)

    #region ItemCategory (賣場跨分類類別)

    /// <summary>
    /// 草稿清單賣場跨分類類別
    /// </summary>
    public class ItemSketch_ItemCategory
    {
        #region 主分類

        /// <summary>
        /// 主分類_第 0 層分類 ID
        /// </summary>
        public int? MainCategoryID_Layer0 { get; set; }

        /// <summary>
        /// 主分類_第 1 層分類 ID
        /// </summary>
        public int? MainCategoryID_Layer1 { get; set; }

        /// <summary>
        /// 主分類_第 2 層分類 ID (商品分類 ID)
        /// </summary>
        public int? MainCategoryID_Layer2 { get; set; }

        /// <summary>
        /// 主分類_第 0 層分類名稱
        /// </summary>
        public string MainCategoryName_Layer0 { get; set; }

        /// <summary>
        /// 主分類_第 1 層分類名稱
        /// </summary>
        public string MainCategoryName_Layer1 { get; set; }

        /// <summary>
        /// 主分類_第 2 層分類名稱
        /// </summary>
        public string MainCategoryName_Layer2 { get; set; }

        #endregion 主分類

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

    #endregion ItemCategory (賣場跨分類類別)

    #endregion 草稿

    #region 草稿查詢條件

    /// <summary>
    /// 草稿查詢條件
    /// </summary>
    public class ItemSketchSearchCondition
    {
        /// <summary>
        /// 商家 ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 是否有管理權限 (有管理權限時，SellerID = 0 則表示不做 SellerID 的篩選)
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 關鍵字搜尋目標
        /// </summary>
        public ItemSketchKeyWordSearchTarget KeyWordScarchTarget { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        #region 進階搜索

        /// <summary>
        /// 商品狀態
        /// </summary>
        public int? ItemStatus { get; set; }

        /// <summary>
        /// 商品審核狀態
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 製造商 ID (0 = 所有)
        /// </summary>
        public int ManufactureID { get; set; }

        /// <summary>
        /// 可賣數量
        /// </summary>
        public ItemSketchCanSellQty canSellQty { get; set; }

        /// <summary>
        /// 第 0 層分類 (0 = 所有)
        /// </summary>
        public int categoryID_Layer0 { get; set; }

        /// <summary>
        /// 第 1 層分類 (0 = 上一層所有)
        /// </summary>
        public int categoryID_Layer1 { get; set; }

        /// <summary>
        /// 第 2 層分類 (0 = 上一層所有)
        /// </summary>
        public int categoryID_Layer2 { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public ItemSketchCreateDate createDate { get; set; }

        /// <summary>
        /// 迄始日期
        /// </summary>
        public DateTime? startDate { get; set; }

        /// <summary>
        /// 迄止日期
        /// </summary>
        public DateTime? endDate { get; set; }

        /// <summary>
        /// 加價購
        /// </summary>
        public int? ShowOrder { get; set; }
        /// <summary>
        /// 廢四機
        /// </summary>
        public string IsRecover { get; set; }


        #endregion 進階搜索

        public PageInfo pageInfo { get; set; }

        #region 分頁資訊

        ///// <summary>
        ///// 排序項目
        ///// </summary>
        //public string OrderBy { get; set; }

        ///// <summary>
        ///// 是否遞減
        ///// </summary>
        //public bool? OrderByDesc { get; set; }

        ///// <summary>
        ///// 分頁頁數
        ///// </summary>
        //public int? PageIndex { get; set; }

        ///// <summary>
        ///// 資料筆數
        ///// </summary>
        //public int? PageSize { get; set; }

        #endregion 分頁資訊

        // 預設值
        public ItemSketchSearchCondition()
        {
            IsAdmin = false;
            SellerID = -1;
            KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.SellerProductID;
            canSellQty = ItemSketchCanSellQty.All;
            createDate = ItemSketchCreateDate.All;
            ManufactureID = 0;
            categoryID_Layer0 = 0;
            categoryID_Layer1 = 0;
            categoryID_Layer2 = 0;
            pageInfo = new PageInfo();
            pageInfo.PageIndex = 0;
            pageInfo.PageSize = 10;
        }
    }

    /// <summary>
    /// 關鍵字搜尋目標
    /// </summary>
    public enum ItemSketchKeyWordSearchTarget
    {
        /// <summary>
        /// 商家商品編號
        /// </summary>
        SellerProductID = 0,

        /// <summary>
        /// 廠商產品編號
        /// </summary>
        MenufacturePartNum = 1,

        /// <summary>
        /// 草稿 ID
        /// </summary>
        ItemSketchID = 2,

        /// <summary>
        /// 商品名稱
        /// </summary>
        ProductName = 3,
        
        /// <summary>
        /// 新蛋賣場編號
        /// </summary>
        ItemID = 4,

        /// <summary>
        /// 新蛋 Item 待審區 ID
        /// </summary>
        ItemTempID = 5,

        /// <summary>
        /// Groupid
        /// </summary>
        GroupId = 6,

        All
    }

    /// <summary>
    /// 可賣數量搜尋條件
    /// </summary>
    public enum ItemSketchCanSellQty
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 0,

        /// <summary>
        /// 少於 10 筆
        /// </summary>
        LessThan10 = 10,

        /// <summary>
        /// 少於 50 筆
        /// </summary>
        LessThan50 = 50,

        /// <summary>
        /// 少於 100 筆
        /// </summary>
        LessThan100 = 100,

        /// <summary>
        /// 大於等於 100 筆
        /// </summary>
        EqualOrMoreThen100 = 1000
    }

    /// <summary>
    /// 創建日期搜尋條件
    /// </summary>
    public enum ItemSketchCreateDate
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 0,

        /// <summary>
        ///  今天
        /// </summary>
        Today = 1,

        /// <summary>
        /// 最近 3 天
        /// </summary>
        Last3Days = 3,

        /// <summary>
        /// 最近 7 天
        /// </summary>
        Last7Days = 7,

        /// <summary>
        /// 最近 30 天
        /// </summary>
        Last30Days = 30,

        /// <summary>
        /// 指定日期
        /// </summary>
        SpecifyDate = 2,

        /// <summary>
        /// 日期範圍
        /// </summary>
        DateRange = 4
    }

    #endregion 草稿查詢條件

    /// <summary>
    /// 建立及更新資訊
    /// </summary>
    public class CreateAndUpdateIfno
    {
        /// <summary>
        /// 建立者
        /// </summary>
        public int CreateUser { get; set; }

        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public int UpdateUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }

    /// <summary>
    /// 分類資訊
    /// </summary>
    public class CategoryInfo
    {
        /// <summary>
        /// 分類 ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string CategoryName { get; set; }
    }

    /// <summary>
    /// 儲存草稿類型
    /// </summary>
    public enum ItemSketchEditType
    { 
        Create,
        DetailEdit,
        ListEdit
    }
}
