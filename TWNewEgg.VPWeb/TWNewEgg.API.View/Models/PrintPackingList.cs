using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class PrintPackingList
    {
        // 訂單編號
        public string SOCode { get; set; }

        // 客戶姓名
        public string UserName { get; set; }

        // 連絡電話
        public string Mobile { get; set; }

        // 遞送地址
        public string Address { get; set; }

        // 訂單商品資訊
        public List<PackageDetials> packageDetials { get; set; }

        public PrintPackingList()
        {
            this.packageDetials = new List<PackageDetials>();
        }

        // 備註
        public string Note { get; set; }
    }

    public class PackageDetials
    {
        // 商家銷售編號
        public string SellerProductID { get; set; }

        // 商品名稱
        public string Title { get; set; }

        // 數量
        public string Qty { get; set; }
    }
}
