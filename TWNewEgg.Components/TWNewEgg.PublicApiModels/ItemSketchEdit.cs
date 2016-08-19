using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PublicApiModels
{
    public class ItemSketchEdit
    {
        public ItemSketchEdit()
        {
            this.dataCheckStatus = new DataCheckStatus();
            this.Sdesc = new Sdesc();
        }
        /// <summary>
        /// 賣場編號
        /// </summary>
        public Nullable<int> ItemID { get; set; }
        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }
        /// <summary>
        /// 主分類_第 0 層分類 ID
        /// </summary>
        public int? MainCategoryID_Layer0 { get; set; }
        /// <summary>
        /// 第 1 個跨分類_第 1 層分類 ID
        /// </summary>
        public Nullable<int> SubCategoryID_1_Layer1 { get; set; }
        /// <summary>
        /// 第 1 個跨分類_第 2 層分類 ID
        /// </summary>
        public Nullable<int> SubCategoryID_1_Layer2 { get; set; }
        /// <summary>
        /// 第 1 個跨分類_第 2 層分類 ID
        /// </summary>
        public Nullable<int> SubCategoryID_2_Layer1 { get; set; }
        /// <summary>
        /// 第 1 個跨分類_第 2 層分類 ID
        /// </summary>
        public Nullable<int> SubCategoryID_2_Layer2 { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商品特色標題
        /// </summary>
        public Sdesc Sdesc { get; set; }
        /// <summary>
        /// 商品簡要描述
        /// </summary>
        public string Spechead { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }
        /// <summary>
        /// 現金價(畫面：售價)
        /// </summary>
        public decimal? PriceCash { get; set; }
        /// <summary>
        /// 市場建議售價(畫面：建議價)
        /// </summary>
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 商品保固期(月)
        /// </summary>
        public int? Warranty { get; set; }
        /// <summary>
        /// 可售數量
        /// </summary>
        public int? CanSaleQty { get; set; }
        /// <summary>
        /// 安全庫存量
        /// </summary>
        public int? InventorySafeQty { get; set; }
        /// <summary>
        /// 資料檢查狀態
        /// </summary>
        public DataCheckStatus dataCheckStatus { get; set; }
    }
    public class Sdesc
    {
        public string Sdesc1 { get; set; }
        public string Sdesc2 { get; set; }
        public string Sdesc3 { get; set; }
    }
    public class DataCheckStatus
    {
        public int index { get; set; }
        public bool isCorrect { get; set; }
        public string reason { get; set; } 
        //public string codeStatusMsg { get; set; }
        //public string codeStatusCode { get; set; }
    }

    public class CheckModel2Use
    {
        public int itemID { get; set; }
        public int productID { get; set; }
        public int categoryID { get; set; }
        public Nullable<int> InventorySafeQty { get; set; }
        public bool isItemGroupDetailProperty { get; set; }
    }
    public class ItemWithsubLayer2Check
    {
        public int itemid { get; set; }
        public int? MainCategoryID_0_Layer0 { get; set; }
        public int? MainCategoryID_0_Layer1 { get; set; }
        public int? MainCategoryID_0_Layer2 { get; set; }
        public int? SubCategoryID_1_Layer0 { get; set; }
        public int? SubCategoryID_1_Layer1 { get; set; }
        public int? SubCategoryID_1_Layer2 { get; set; }
        public int? SubCategoryID_2_Layer0 { get; set; }
        public int? SubCategoryID_2_Layer1 { get; set; }
        public int? SubCategoryID_2_Layer2 { get; set; }
    }

    public class ststusChange
    {
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }
        /// <summary>
        /// 現金價(畫面：售價)
        /// </summary>
        public decimal? PriceCash { get; set; }
        
    }

    public class BatchResponse
    {
        public string code { get; set; }
        public string codeMessage { get; set; }
        public string message { get; set; }
        public List<DataCheckStatus> dataCheckStatus { get; set; }
    }
}
