using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    #region 主單

    public class MainRetgood {

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string CartID { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        /// <remarks>Process/CreateDate</remarks>
        public DateTime? CartCreateDate { get; set; }

        /// <summary>
        /// 退貨成立日期
        /// </summary>
        public DateTime? RetgoodCreateDate { get; set; }

        /// <summary>
        /// 退貨完成日期
        /// </summary>
        public DateTime? FinReturnDate { get; set; }

        /// <summary>
        /// 取貨客戶姓名
        /// </summary>
        public string FrmName { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        /// <remarks>Cart/PayType</remarks>
        public int? PayType { get; set; }

        /// <summary>
        /// PayType 名稱
        /// </summary>
        public string PayTypeName { get; set; }

        /// <summary>
        /// 商品編號
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        /// <remarks>Process/Title</remarks>
        public string ProcessTitle { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        /// <remarks>商家名稱(商家編號)</remarks>
        public string Seller { get; set; }

        /// <summary>
        /// 商家編號
        /// </summary>
        public int? SellerID { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 商家銷售編號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// 交易模式
        /// </summary>
        public int? ShipType { get; set; }

        /// <summary>
        /// 交易模式名稱
        /// </summary>
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 退貨狀態
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 退貨狀態名稱
        /// </summary>
        public string StatusName { get; set; }
    }

    /// <summary>
    /// 退貨主單查詢條件
    /// </summary>
    public class MainRetgoodSearchCondition
    {
        /// <summary>
        /// 查詢關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 關鍵字查詢目標
        /// </summary>
        public int KeyWordSearchType { get; set; }

        /// <summary>
        /// 指定查詢退貨狀態
        /// </summary>
        public int? RetgoodStatus { get; set; }

        /// <summary>
        /// 指定查詢訂單日期
        /// </summary>
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
        public int? SellerID { get; set; }

        /// <summary>
        /// 是否具有管理權限
        /// </summary>
        public bool IsAdmin { get; set; }

        public MainRetgoodSearchCondition()
        {
            KeyWord = string.Empty;
            KeyWordSearchType = 0;
            RetgoodStatus = null;
            CreateDateSearchType = 0;
            StartDate = null;
            EndDate = null;
            SellerID = -1;
            IsAdmin = false;
        }
    }

    public enum RetgoodKeyWordSearchType
    {
        訂單編號 = 0,
        商品名稱 = 1,
        收件人姓名 = 2,
        商家銷售編號 = 3
    }

    public enum RetgoodCreateDateSearchType
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

    public class RetgoodAPIModel
    {
        public List<RetgoodGrid> retgoodgrid { get; set; }
        public RetgoodUpper retgoodUpper { get; set; }
    }

    public class RetgoodGrid
    {
        //退貨編號
        public string card_id { get; set; }
        //商家銷售編號
        public string product_sellerproductid { get; set; }
        //新蛋商品編號
        public int product_productid { get; set; }
        //廠商產品編號
        public string product_MenufacturePartNum { get; set; }
        //UPC
        public string product_UPC { get; set; }
        //商品說明
        public string process_Title { get; set; }
        //訂購數量
        public int process_Qty { get; set; }
        //退/換貨數量
        public int retgood_Qty { get; set; }
        //單價
        public decimal process_unitPrice { get; set; }
        //退/換貨原因
        public string retgood_CauseNote { get; set; }
        // 退款總金額
        public decimal retgood_Price { get; set; }
        //貨運編號
        public string retgood_ShpCode { get; set; }
        // 優惠總價, (pricecoupon + ApportionedAmount)
        public decimal process_coupon_total { get; set; }
    }

    public class RetgoodUpper
    {
        //訂單編號
        public string Cart_ID { get; set; }
        //訂單日期
        public DateTime Cart_CreateDate { get; set; }
        //訂購人姓名
        public string Cart_Username { get; set; }
        //訂購人手機
        public string Cart_Mobile { get; set; }
        //取件地址(郵遞區號)
        public string Retgood_FrmZipcode { get; set; }
        //取件地址(縣市)
        public string Retgood_FrmLocation { get; set; }
        //取件地址(地址)
        public string Retgood_FrmADDR { get; set; }
        //收件聯絡人姓名
        public string Retgood_FrmName { get; set; }
        //收件人手機
        public string Retgood_FrmMobile { get; set; }
        //收件人市話
        public string Retgood_FrmPhone { get; set; }
        //退/換貨狀態(int)
        public int Retgood_Status { get; set; }
        //退/換貨狀態(string)
        public string Retgood_Status_str { get; set; }

    }
    public class RetgoodNote
    {
        public Int32 Note_Status { get; set; }
        public string Note_Des { get; set; }
        public string Note_UpdateRec { get; set; }
    }
}
