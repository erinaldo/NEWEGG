using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// 正式 Item 更新項目(詳細頁面)
    /// </summary>
    public class Item_DetialEdit
    {
        /// <summary>
        /// 商品主分類
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 賣場刪除日期
        /// </summary>
        public DateTime DateDel { get; set; }

        /// <summary>
        /// 賣場結束日期
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 賣場開始日期
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 到貨天數
        /// </summary>
        public string DelvDate { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        public string DescTW { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? GrossMargin { get; set; }

        /// <summary>
        /// 商品成色 (Y:新品, N:二手)
        /// </summary>
        public string IsNew { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        public string ItemDesc { get; set; }

        /// <summary>
        /// 商品包裝 (0:零售, 1:OEM)
        /// </summary>
        public string ItemPackage { get; set; }

        /// <summary>
        /// 製造商 ID
        /// </summary>
        public int ManufactureID { get; set; }

        /// <summary>
        /// 市場建議售價
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 商品型號
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 注意事項
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 圖片結束
        /// </summary>
        public int PicEnd { get; set; }

        /// <summary>
        /// 圖片開始
        /// </summary>
        public int PicStart { get; set; }

        /// <summary>
        /// 限量數量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 限購數量
        /// </summary>
        public int QtyLimit { get; set; }

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
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 更新者 ID
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// Discard4 
        /// Y:全新
        /// N:二手
        /// </summary>
        public string Discard4 { get; set; }
    }

    /// <summary>
    /// 正式 Product 更新項目(詳細頁面)
    /// </summary>
    public class Product_DetialEdit
    {
        /// <summary>
        /// 條碼
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string DescriptionTW { get; set; }

        /// <summary>
        /// 材積(公分)_高
        /// </summary>
        public decimal Height { get; set; }

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
        public decimal Length { get; set; }

        /// <summary>
        /// 製造商 ID
        /// </summary>
        public int ManufactureID { get; set; }

        /// <summary>
        /// 製造商商品編號
        /// </summary>
        public string MenufacturePartNum { get; set; }

        /// <summary>
        /// 型號
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 注意事項
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 產品圖片最後一張
        /// </summary>
        public int PicEnd { get; set; }

        /// <summary>
        /// 產品圖片第一張
        /// </summary>
        public int PicStart { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// UPC 編號
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 更新者 ID
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 商品保固期(月)
        /// </summary>
        public int Warranty { get; set; }

        /// <summary>
        /// 重量(公斤)
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 材積(公分)_寬
        /// </summary>
        public decimal Width { get; set; }
    }

    /// <summary>
    /// 正式 ItemStock 更新項目(詳細頁面)
    /// </summary>
    public class ItemStock_DetialEdit
    {
        /// <summary>
        /// 可售數量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 安全庫存
        /// </summary>
        public int SafeQty { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 更新者 ID
        /// </summary>
        public string UpdateUser { get; set; }
    }

}
