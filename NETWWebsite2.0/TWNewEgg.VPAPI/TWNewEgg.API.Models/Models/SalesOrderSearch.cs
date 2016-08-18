using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    #region 主單

    /// <summary>
    /// 訂單主單
    /// </summary>
    public class MainOrderResult
    {
        /// <summary>
        /// 訂單主單清單
        /// </summary>
        public List<MainOrder> Grid { get; set; }

        /// <summary>
        /// 分頁篩選前的查詢結果資料筆數
        /// </summary>
        public int DataCount { get; set; }

        public MainOrderResult()
        {
            Grid = new List<MainOrder>();
            DataCount = 0;
        }
    }

    /// <summary>
    /// 訂單主單
    /// </summary>
    public class MainOrder
    {
        #region TWBACKENDDB

        #region Cart

        /// <summary>
        /// 訂單編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.ID</value>
        public string CartID { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.CreateDate</value>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 出貨狀態
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.DelvStatus</value>
        /// <value>enum TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus</value>
        public int? DelvStatus { get; set; }

        /// <summary>
        /// 出貨狀態名稱
        /// </summary>
        /// <value>enum TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus</value>
        public string DelvStatusName { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.PayType</value>
        public int? PayType { get; set; }

        /// <summary>
        /// 購物車編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.SalesOrderGroupID</value>
        public int? SalesOrderGroupID { get; set; }

        /// <summary>
        /// 出貨方
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.ShipType</value>
        public int? ShipType { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.Status</value>
        /// <value>enum TWNewEgg.DB.TWBACKENDDB.Models.Cart.status</value>
        public int? Status { get; set; }

        /// <summary>
        /// 訂購人姓名
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.Username</value>
        public string UserName { get; set; }

        /// <summary>
        /// 訂單更新日期
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.UpdateDate</value>
        public DateTime? UpdateDate { get; set; }

        #endregion Cart

        #region Process

        /// <summary>
        /// 數量
        /// </summary>
        /// <remarks>總合同一個 CartID 的 Qty</remarks>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.Qty</value>
        public int Qty { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.Title</value>
        public string ProcessTitle { get; set; }

        #endregion Process

        #endregion TWBACKENDDB

        #region TWSQLDB

        #region PayType

        /// <summary>
        /// 付款方式名稱
        /// </summary>
        /// <value>enum TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType</value>
        public string PayTypeName { get; set; }

        #endregion PayType

        #region Product

        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.Product.ID</value>
        public int ProductID { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.Product.SellerProductID</value>
        public string SellerProductID { get; set; }

        #endregion Product

        #region PurchaseOrder

        /// <summary>
        /// 採購單編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.PurchaseOrder.Code</value>
        public string POCode { get; set; }

        #endregion PurchaseOrder

        #endregion TWSQLDB

        /// <summary>
        /// 賣場路徑
        /// </summary>
        public string ItemUrl { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        /// <remarks>商家名稱(商家編號)</remarks>
        public string Seller { get; set; }

        /// <summary>
        /// 出貨方名稱
        /// </summary>
        /// <value>TWNewEgg.API.Service.SalesOrderSearchService.GetShipTypeName</value>
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 訂單狀態名稱
        /// </summary>
        /// <value>enum TWNewEgg.DB.TWSQLDB.Models.SalesOrder.status</value>
        /// <value>enum TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus</value>
        public string StatusName { get; set; }
    }

    /// <summary>
    /// 訂單主單查詢條件
    /// </summary>
    public class MainOrderSearchCondition
    {
        /// <summary>
        /// 查詢關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 關鍵字查詢目標
        /// </summary>
        /// <value>TWNewEgg.API.Models.OrderKeyWordSearchType</value>
        public int KeyWordSearchType { get; set; }

        /// <summary>
        /// 指定查詢訂單狀態
        /// </summary>
        /// <value>TWNewEgg.API.Models.MainOrderStatus</value>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 指定查詢訂單日期
        /// </summary>
        /// <value>TWNewEgg.API.Models.OrderCreateDateSearchType</value>
        public int CreateDateSearchType { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(起)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(迄)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 商家 ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PageInfo PageInfo { get; set; }

        public MainOrderSearchCondition()
        {
            KeyWord = string.Empty;
            KeyWordSearchType = 0;
            OrderStatus = 0;
            CreateDateSearchType = 0;
            StartDate = null;
            EndDate = null;
            SellerID = -1;
            PageInfo = new PageInfo();
            PageInfo.PageIndex = 0;
            PageInfo.PageSize = 10;
            PageInfo.TotalPage = 0;
        }
    }

    public class MainOrder_Process
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.ID</value>
        public string CartID { get; set; }

        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.ProductID</value>
        public int ProductID { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.Qty</value>
        public int? Qty { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.Title</value>
        public string Title { get; set; }

        /// <summary>
        /// 賣場ID
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Process.StoreID</value>
        public int StoreID { get; set; }

        /// <summary>
        /// 賣場路徑
        /// </summary>
        public string ItemUrl { get; set; }
    }

    public class MainOrder_PurchaseOrder
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWBACKENDDB.Models.Cart.ID</value>
        public string CartID { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.PurchaseOrder.Code</value>
        public string Code { get; set; }
    }

    public class MainOrder_Product
    {
        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.Product.ID</value>
        public int ProductID { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        /// <value>TWNewEgg.DB.TWSQLDB.Models.Product.SellerProductID</value>
        public string SellerProductID { get; set; }
    }

    /// <summary>
    /// Vendor Portal 所有訂單狀態列舉
    /// </summary>
    public enum MainOrderStatus
    { 
        全部 = 0,
        已成立 = 1,
        待出貨 = 2,
        已出貨 = 3,
        配達 = 4,
        取消 = 5,
        退貨 = 6
    }

    /// <summary>
    /// 關鍵字查詢目標
    /// </summary>
    public enum OrderKeyWordSearchType
    {
        訂單編號 = 0,
        訂購人姓名 = 1,
        新蛋商品編號 = 2,
        商家商品編號 = 3,
        商品名稱 = 4
    }

    public enum OrderCreateDateSearchType
    {
        全部 = 0,
        今天 = 1,
        最近3天 = 2,
        最近7天 = 3,
        最近30天 = 4,
        指定日期 = 5,
        定制日期範圍 = 6
    }

    #endregion 主單
}
