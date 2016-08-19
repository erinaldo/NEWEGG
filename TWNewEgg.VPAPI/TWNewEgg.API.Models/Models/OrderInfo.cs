using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// Order Info(訂單資訊)
    /// <para>Website Page: Manage Order >> Order List</para>
    /// </summary>
    public class OrderInfo
    {
        #region Enum
        /// <summary>
        /// 訂單狀態
        /// </summary>
        public enum EnumCartStatus
        {
            正常 = 0,
            取消 = 1,
            被動取消 = 2,
            退貨 = 5,
            完成 = 7,
            初始狀態 = 99
        }

        /// <summary>
        /// 輸入字串, 轉成列舉(EnumCartStatus)後輸出
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static EnumCartStatus CartStatus_GetEnum(string s)
        {
            return (EnumCartStatus)Enum.Parse(typeof(EnumCartStatus), s, false);
        }

        /// <summary>
        /// 出貨狀態
        /// </summary>
        public enum EnumDelvStatus
        {
            已出貨 = 1,
            配達 = 2,
            待出貨 = 0, //2014/07/11 Ted Modified 依要求將"出貨中"改為顯示"待出貨">>以跟前台顯示一致
            已成立 = 6,
            待進貨 = 7,
            已進貨 = 8,
            初始狀態 = 999,
        }

        /// <summary>
        /// 輸入字串, 轉成列舉(EnumDelvStatus)後輸出
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static EnumDelvStatus DelvStatus_GetEnum(string s)
        {
            return (EnumDelvStatus)Enum.Parse(typeof(EnumDelvStatus), s, false);
        }

        //EnumAccountTypeCode 可能應該建立在別的MODEL裡 Seller_BasicInfo
        /// <summary>
        /// Seller/Vendor 身分別Enum
        /// </summary>
        public enum EnumAccountTypeCode
        {
            /// <summary>
            /// S:商家
            /// </summary>
            Seller,

            /// <summary>
            /// V:供應商
            /// </summary>
            Vendor
        }

        /// <summary>
        /// 供貨通路/遞送方/ !!注意：目前只能使用文字，其隱含數字目前無法使用!!
        /// </summary>
        //public enum EnumFulfillChannel
        //{
        //    /// <summary>
        //    /// 注意!! 目前8或9皆為SBN，故暫時無法成功以列舉對應
        //    /// </summary>
        //    SBN,
        //    SBS,
        //    SBV,

        //    //SBN = 9,
        //    //SBN = 8,
        //    //SBS = 2,
        //    //SBV = 7,
        //}
        #endregion


        #region OrderList
        /// <summary>
        /// 出貨狀態/遞送狀態
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.DelvStatus</para>
        /// </summary>
        //public int? DelvStatus { get; set; }

        /// <summary>
        /// Seller/Vendor
        /// </summary>
        public EnumAccountTypeCode AccountTypeCode { get; set; }

        /// <summary>
        /// 出貨狀態/遞送狀態 (列舉: 原始資料)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.DelvStatus</para>
        /// </summary>
        public EnumDelvStatus DelvStatus { get; set; }

        /// <summary>
        /// 出貨狀態/遞送狀態 (列舉: 原始資料)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.DelvStatus</para>
        /// </summary>
        public EnumCartStatus CartStatus { get; set; }

        /// <summary>
        /// 出貨狀態/遞送狀態 (透過列舉值轉成字串: 包含 '取消' 以及 '退貨' 等不在列舉狀態中的特殊狀況)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.DelvStatus</para>
        /// </summary>
        public string DelvStatusStr { get; set; }

        /// <summary>
        /// 建立日期/實際建檔日期cart.CreateDate 0710改為string Jack.W.Wu
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.CreateDate</para>
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 更新日期  0710改為string Jack.W.Wu
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.UpdateDate</para>
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 商家(商家名稱)
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 供貨通路/遞送方/遞送服務(Ship Service)
        /// <para>
        ///     TWSELLERPORTALDB.dbo.Seller_BasicInfo.AccountTypeCode >>討論後改為 TWSQLDB.dbo.Item.ShipType 2014/4 by Ted
        ///     >>2014/9/16 by Ted:修正為Item.DelvType(或cart.ShipType)
        /// </para>
        /// </summary>
        public string FulfillChannel { get; set; } //AccountTypeCode { get; set; }

        /// <summary>
        /// 購物車群組編號 add by 2015.09.01 Jack
        /// </summary>
        public int? SaleOrderGroupID { get; set; }

        /// <summary>
        /// 供貨通路/遞送方/遞送服務(Ship Service) >> 的原始資料庫數值
        /// 2014/9/16 by Ted:此欄位為欄位FulfillChannel的原資料數值(資料庫中保存的數值(目前為int))
        /// </summary>
        public string Item_DelvType{ get; set; }

        /// <summary>
        /// 訂單中第一筆產品描述/訂單內容
        /// TWBACKENDDB.dbo.cart.process.Title
        /// </summary>
        public string FirstProductTitle { get; set; }

        ///// <summary>
        ///// 訂單中第一筆製造商商品編號/Manufacturer Part Num/ISBN
        ///// <para>DB From:TWSQLDB.dbo.Product.MenufacturePartNum</para>
        ///// </summary>
        public string FirstMenufacturePartNum { get; set; }

        /// <summary>
        /// 顧客(顧客名稱)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Username</para>
        /// <remarks>
        ///     目前資料和 "UserName" 有相同資料內容
        /// </remarks>
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public string Status { get; set; }

        public List<string> AllDeliverIDxName { get; set; }

        public string UpdateNote { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 供應商營業地址
        /// </summary>
        public string SellerShippingAddress { get; set; }

        #endregion

        #region (其他訂單資訊)
        /// <summary>
        /// 營業地址
        /// </summary>
        public string BusinessAddress { get; set; }
        #endregion

        #region 訂單概要資訊
        /// <summary>
        /// 訂單編號
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.ID</para>
        /// </summary>
        public string SOCode { get; set; }

        /// <summary>
        /// 採購單號
        /// </summary>
        public string POCode { get; set; }

        /// <summary>
        /// 訂單單號+採購單號
        /// </summary>
        public string SOCode_POCode { get; set; }

        /// <summary>
        /// ?訂單日期(=?建立日期)  //Ron : 跟ICE確認過了, 訂單日期 = 建立日期 所以這部分Mark掉OderDate欄位
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Date</para>
        /// </summary>
        //public DateTime OrderDate { get; set; }

        ///// <summary>
        ///// 客戶手機 >>20140617改欄位名稱為"訂購人手機"資料來源為cart.Mobile(PS.依據CODE此欄位本就顯示cart.Mobile)
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.Mobile</para>
        ///// </summary>
        public string CustomerMobile { get; set; }

        /// <summary>
        /// 發票號碼
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.InvoiceNO</para>
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 遞送服務類別
        /// <para></para>
        /// </summary>
        public string DelvServiceType{ get; set; }

        /// <summary>
        /// 收件人(姓名)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Receiver</para>
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 退換貨單據號碼
        /// </summary>
        public string RMACode { get; set; }

        /// <summary>
        /// 收件人地址/客戶地址
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.ADDR</para>
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收件人公司
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.CMPName</para>
        /// </summary>
        public string CMPName { get; set; }


        ///// <summary>
        ///// 總數量
        ///// <para>DB FROM:TWSQLDB.dbo.process.qty</para>
        ///// </summary>
        public int TotalQty { get; set; }
        
        /// <summary>
        /// 商品小計
        /// </summary>
        public decimal SubtotalPrice { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 服務費
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 收件人手機 Add by Jack.W.Wu 0613
        /// </summary>
        public string ReceiverCellphone { get; set; }

        /// <summary>
        /// 收件人市話 Add by Jack.W.Wu 0613
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 備註 Add by Jack.W.Wu 0613
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Zipcode</para>
        /// <remarks>20140617依據要求增加此地址資訊 add by Ted</remarks>
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// ?(地區EX:台北市/新北市/新竹縣)
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Location</para>
        /// <remarks>20140617依據要求增加此地址資訊 add by Ted</remarks>
        /// </summary>
        public string Location { get; set; }
        #endregion

        #region 訂單商品資訊
        public List<OrderDetailsInfo> OrderDetails { get; set; }
        #endregion

        #region 匯出Excel
        /// <summary>
        /// 匯出時用到的所有欄位 Model
        /// </summary>
        public class ExportToExcelModel
        {
            [DisplayName("sellerID")]
            public string SellerID { get; set; }

            #region 配送資訊

            [DisplayName("出貨提單編號T#3")]
            public string TrackingNumber { get; set; }

            [DisplayName("貨運公司")]
            public string DelvName { get; set; }

            [DisplayName("出貨時間")]
            public string ShipDate { get; set; }

            [DisplayName("配達時間")]
            public string ArriveDate { get; set; }

            #endregion 配送資訊

            #region 訂購單資訊

            [DisplayName("單據日期")]
            public string CreateDate { get; set; }

            [DisplayName("客戶訂單編號")]
            public string SOCode { get; set; }

            [DisplayName("供應商訂單編號")]
            public string POCode { get; set; }

            [DisplayName("供應商產品編號")]
            public string SellerProductID { get; set; }
            
            [DisplayName("台蛋產品編號")]
            public string NeweggPartNum { get; set; }

            [DisplayName("付款方式")]
            public string PayType { get; set; }

            [DisplayName("商品名稱")]
            public string ItemName { get; set; }
    
            [DisplayName("數量")]
            public int? Qty { get; set; }
    
            [DisplayName("單價")]
            public decimal? Price { get; set; }

            [DisplayName("總價")]
            public decimal? TotalPrice { get; set; }

            [DisplayName("單位成本")]
            public decimal? UnitCost { get; set; }

            [DisplayName("總成本")]
            public decimal? TotalCost { get; set; }

            #endregion 訂購單資訊

            #region 訂購人資訊
            
            [DisplayName("收件人姓名")]
            public string Receiver { get; set; }

            [DisplayName("郵遞區號")]
            public string ZipCode { get; set; }
    
            [DisplayName("縣市")]
            public string Location { get; set; }
    
            [DisplayName("地址")]
            public string Address { get; set; }
    
            [DisplayName("聯絡電話")]
            public string ReceiverCellphone { get; set; }
    
            [DisplayName("配送時間")]
            public string DelvDate { get; set; }
    
            [DisplayName("備註")]
            public string Note { get; set; }

            #endregion 訂購人資訊
        }

        /// <summary>
        /// 匯出 Seller 的 Excel 欄位格式
        /// </summary>
        public class ExportSellerExcel
        {
            [DisplayName("sellerID")]
            public string SellerID { get; set; }

            [DisplayName("訂單狀態")]
            public string OrderStatus { get; set; }

            [DisplayName("出貨方")]
            public string ShipType { get; set; }

            [DisplayName("購物車編號")]
            public int? SalesOrderGroupID { get; set; }

            #region 配送資訊

            [DisplayName("出貨提單編號T#3")]
            public string TrackingNumber { get; set; }

            [DisplayName("貨運公司")]
            public string DelvName { get; set; }

            [DisplayName("出貨時間")]
            public string ShipDate { get; set; }

            [DisplayName("配達時間")]
            public string ArriveDate { get; set; }

            #endregion 配送資訊

            #region 訂購單資訊

            [DisplayName("單據日期")]
            public string CreateDate { get; set; }

            [DisplayName("客戶訂單編號")]
            public string SOCode { get; set; }

            [DisplayName("供應商產品編號")]
            public string SellerProductID { get; set; }

            [DisplayName("台蛋產品編號")]
            public string NeweggPartNum { get; set; }

            [DisplayName("付款方式")]
            public string PayType { get; set; }

            [DisplayName("商品名稱")]
            public string ItemName { get; set; }

            [DisplayName("數量")]
            public int? Qty { get; set; }

            [DisplayName("單價")]
            public decimal? Price { get; set; }

            [DisplayName("總價")]
            public decimal? TotalPrice { get; set; }

            #endregion 訂購單資訊

            #region 訂購人資訊

            [DisplayName("收件人姓名")]
            public string Receiver { get; set; }

            [DisplayName("郵遞區號")]
            public string ZipCode { get; set; }

            [DisplayName("縣市")]
            public string Location { get; set; }

            [DisplayName("地址")]
            public string Address { get; set; }

            [DisplayName("聯絡電話")]
            public string ReceiverCellphone { get; set; }

            [DisplayName("配送時間")]
            public string DelvDate { get; set; }

            [DisplayName("備註")]
            public string Note { get; set; }

            #endregion 訂購人資訊
        }


        /// <summary>
        /// 匯出 Vendor 的 Excel 欄位格式
        /// </summary>
        public class ExportVendorExcel
        {
            [DisplayName("VenderID")]
            public string SellerID { get; set; }

            [DisplayName("訂單狀態")]
            public string OrderStatus { get; set; }

            [DisplayName("出貨方")]
            public string ShipType { get; set; }

            [DisplayName("購物車編號")]
            public int? SalesOrderGroupID { get; set; }

            #region 配送資訊

            [DisplayName("出貨提單編號T#3")]
            public string TrackingNumber { get; set; }

            [DisplayName("貨運公司")]
            public string DelvName { get; set; }

            [DisplayName("出貨時間")]
            public string ShipDate { get; set; }

            [DisplayName("配達時間")]
            public string ArriveDate { get; set; }

            #endregion 配送資訊

            #region 訂購單資訊

            [DisplayName("單據日期")]
            public string CreateDate { get; set; }

            [DisplayName("客戶訂單編號")]
            public string SOCode { get; set; }

            [DisplayName("供應商訂單編號")]
            public string POCode { get; set; }

            [DisplayName("供應商產品編號")]
            public string SellerProductID { get; set; }

            [DisplayName("台蛋產品編號")]
            public string NeweggPartNum { get; set; }

            [DisplayName("商品名稱")]
            public string ItemName { get; set; }

            [DisplayName("數量")]
            public int? Qty { get; set; }

            [DisplayName("單位成本")]
            public decimal? UnitCost { get; set; }

            [DisplayName("總成本")]
            public decimal? TotalCost { get; set; }

            #endregion 訂購單資訊

            #region 訂購人資訊

            [DisplayName("收件人姓名")]
            public string Receiver { get; set; }

            [DisplayName("郵遞區號")]
            public string ZipCode { get; set; }

            [DisplayName("縣市")]
            public string Location { get; set; }

            [DisplayName("地址")]
            public string Address { get; set; }

            [DisplayName("聯絡電話")]
            public string ReceiverCellphone { get; set; }

            [DisplayName("配送時間")]
            public string DelvDate { get; set; }

            [DisplayName("備註")]
            public string Note { get; set; }

            #endregion 訂購人資訊
        }

        /// <summary>
        /// 匯出 Excel 訂單列表 Model
        /// </summary>
        public class DownloadSalesOrderListModel
        {
            // 商家 ID
            public string SellerID { get; set; }

            // 訂單篩選條件
            public QueryCartCondition queryCartCondition { get; set; }

            // 訂單列表
            public List<OrderInfo> dataList { get; set; }

            // Excel 檔案名稱
            public string fileName { get; set; }

            // Excel 工作表名稱
            public string sheetName { get; set; }

            // Excel 抬頭行數
            public int titleLine { get; set; }

            // 登入者身分別
            public EnumAccountTypeCode AccountType { get; set; }
        }
        #endregion 匯出Excel

        #region (保留)
        //public OrderInfo()
        //{

        //} 

        /// <summary>
        /// ?品項名稱(?ItemName)>>
        /// <para>DB FROM:TWSQLDB.dbo.item.Name >> 討論後改用 TWSQLDB.dbo.Product.Name 2014/4/29 by Ted</para>
        /// </summary>
        //public string ItemName { get; set; }

        ////in details
        ///// <summary>
        ///// 新蛋商品編號
        ///// <para>DB FROM:TWSQLDB.dbo.process.ProductID</para>
        ///// </summary>
        //public string ProductID { get; set; }

        ///// <summary>
        ///// 商家商品編號
        ///// <para>DB FROM:TWSQLDB.dbo.product.SellerProductID</para>
        ///// </summary>
        ////public string SellerProductID { get; set; }

        ///// <summary>
        ///// ?價格
        ///// </summary>
        //public decimal Price { get; set; } //

        ///// <summary>
        ///// 購買人姓名/客戶名稱/顧客姓名
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.Username</para>
        ///// </summary>
        //public string UserName { get; set; }

        ///// <summary>
        ///// 付款人行動電話
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.CardMobile</para>
        ///// </summary>
        //public string CardMobile { get; set; }

        ///// <summary>
        ///// ?收件人電子信箱
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.Email</para>
        ///// </summary>
        //public string Email { get; set; }

        ///// <summary>
        ///// ?購物車備註 ?到貨時間?Status//目前使用： cart.Note  //? = orderNote 到貨時間 ?
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.Note</para>
        ///// </summary>
        //public string Note { get; set; }

        ///// <summary>
        ///// 送貨方式
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.ShipType</para>
        ///// </summary>
        //public string ShippingType { get; set; }

        //2014/4/30 被告知改為暫時不使用 by Ted
        /// <summary>
        /// 訂單狀態
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Status</para>
        /// </summary>
        //public string Status { get; set; }

        /// <summary>
        /// 自動取消訂單
        /// </summary>
        //public DateTime AutoVoidSODate { get; set; }

        //保留暫不使用
        /// <summary>
        /// 銷售通路
        /// </summary>
        //public SalesChannel{ get; set; }

        //in details
        ///// <summary>
        ///// Product Condition(商品成色)
        ///// <para>TWSQLDB.dbo.Product.Status</para>
        ///// </summary>
        //public int ProductStatus { get; set; }


        //保留
        //public string SellerID { get; set; }

        //預保留
        //public string ProductName { get; set; }

        //public List<ItemInfo> items { }

        #endregion
    }
}
