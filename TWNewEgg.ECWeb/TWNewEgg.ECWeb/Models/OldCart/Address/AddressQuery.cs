using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class AddressQuery
    {
        public AddressQuery()
        {
            this.Recv = false;
            this.County = "";
            this.Address = "";
            this.ZipCode = "";
        }
        // 是否有查詢到資料
        public bool Recv { get; set; }

        // 縣市
        public string County { get; set; }

        // 鄉鎮市區與道路或街名或村里名稱
        public string Address { get; set; }

        // 郵遞區號
        public string ZipCode { get; set; }
    }
}