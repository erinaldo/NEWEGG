using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class OrderDetailsInfo
    {

        public string ProcessID { get; set; }
        public int ProductID { get; set; }
        public int MenufactureID { get; set; }
        

        /// <summary>
        /// 商家商品編號
        /// <para>DB FROM:TWSQLDB.dbo.product.SellerProductID</para>
        /// </summary>
        public string SellerProductID { get; set; }
        
        /// <summary>
        /// Newegg商品編號/Newegg Part Num
        /// <para>DB From:??  </para>
        /// </summary>
        public string NeweggPartNum { get; set; }

        /// <summary>
        /// 製造商商品編號/Manufacturer Part Num/ISBN
        /// <para>DB From:TWSQLDB.dbo.Product.MenufacturePartNum</para>
        /// </summary>
        public string MenufacturePartNum { get; set; }
        
        /// <summary>
        /// UPC CODE
        /// <para>DB From: </para>
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// Product Condition(商品成色)
        /// <para>TWSQLDB.dbo.Product.Status</para>
        /// </summary>
        public int ProductStatus { get; set; }


        /// <summary>
        /// 產品描述/訂單內容/...
        /// TWBACKENDDB.dbo.cart.process.Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 品項名稱/ItemName/...
        /// <para>DB FROM:TWSQLDB.dbo.item.Name</para>
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 數量
        /// <para>DB FROM:TWSQLDB.dbo.process.qty</para>
        /// <remarks>DBtype:int</remarks>
        /// </summary>
        public int Qty { get; set; }
        

        /// <summary>
        /// 訂單狀態
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.Status</para>
        /// </summary>
        public string Status { get; set; }

        //補資訊
        ///// <summary>
        ///// 客戶電話
        ///// <para>DB FROM:TWBACKENDDB.dbo.cart.Mobile</para>
        ///// </summary>
        public string CustomerMobile { get; set; }

        //補資訊
        /// <summary>
        /// 收件人公司
        /// <para>DB FROM:TWBACKENDDB.dbo.cart.CMPName</para>
        /// </summary>
        public string CMPName { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// 已遞送數量
        /// </summary>
        public int ShippedCount { get; set; }

        /// <summary>
        /// Tracking Number add by Jack.W.Wu 0613
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 希望到貨時間 add by Jack.W.Wu 0613
        /// </summary>
        public string DelvDate { get; set; }

        /// <summary>
        /// 貨運公司名稱 add by Jack.W.Wu 0613
        /// </summary>
        public string DelvName { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal TotalCost { get; set; }
    }
}
