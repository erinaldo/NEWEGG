using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ReturnAddress
    {
        public ReturnAddress()
        {
            this.FindCity = false;
            this.County = "";
            this.City = "";
            this.Addr = "";
            this.ZipCode = "";
        }
        // 是否有查詢到資料
        public bool FindCity { get; set; }

        // 縣市
        public string County { get; set; }

        // 鄉鎮市區
        public string City { get; set; }

        // 道路或街名或村里名稱
        public string Addr { get; set; }

        // 郵遞區號
        public string ZipCode { get; set; }
    }
}