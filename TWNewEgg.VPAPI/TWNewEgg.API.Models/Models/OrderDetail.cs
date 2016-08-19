using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            this.ProcessIDList = new List<string>();
        }

        /// <summary>
        /// 會員身分別(Seller(S) or Vender(V))
        /// </summary>
        public string AccountTypeCode { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string CartID { get; set; }

        /// <summary>
        /// PurchaseOrder Code
        /// </summary>
        public string POCode { get; set; }

        /// <summary>
        /// ProcessID清單
        /// </summary>
        public List<string> ProcessIDList { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 訂單產生日期
        /// </summary>
        public string OrderCreateDate { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 訂單配送狀態
        /// </summary>
        public Nullable<int> DelvStatus { get; set; }

        /// <summary>
        /// 訂購人名稱
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 訂購人手機
        /// </summary>
        public string CustomerMobile { get; set; }

        /// <summary>
        ///  供貨通路/遞送服務/遞送方(代碼)
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// 遞送服務類別
        /// </summary>
        public string FulfillChannel { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人市話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellphone { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        public string NeweggPartNum { get; set; }

        /// <summary>
        /// 廠商產品編號
        /// </summary>
        public string MenufacturePartNum { get; set; }

        /// <summary>
        /// 廠商編號
        /// </summary>
        public int ManufactureID { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品說明
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        /// 訂購數量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 已遞送數量
        /// </summary>
        public int ShippedCount { get; set; }

        /// <summary>
        /// 單價(S)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 單位成本(V)
        /// </summary>
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 總成本(V)
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// 希望到貨時間
        /// </summary>
        public string DelvDate { get; set; }

        /// <summary>
        /// 貨運公司
        /// </summary>
        public string DelvCompanyName { get; set; }

        /// <summary>
        /// 貨運編號
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 子項目總計
        /// </summary>
        public decimal SubTotalPrice { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 服務費
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 訂購總額
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 營業地址
        /// </summary>
        public string SellerShippingAddress { get; set; }

        /// <summary>
        /// ECWeb賣場頁網址
        /// </summary>
        public string ItemUrl { get; set; }
    }
}
