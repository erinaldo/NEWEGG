using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View.Models
{
    public class AddressQuery
    {
        public AddressQuery()
        {
            this.Recv = false;
            this.FindCity = false;
            this.County = "";
            this.City = "";
            this.Addr = "";
            this.ZipCode = "";
            this.Address = "";
        }
        // 是否有查詢到資料
        public bool Recv { get; set; }
        // 是否有查詢到資料
        public bool FindCity { get; set; }

        // 縣市
        public string County { get; set; }

        // 鄉鎮市區
        public string City { get; set; }
        // 鄉鎮市區與道路或街名或村里名稱
        public string Address { get; set; }
        // 道路或街名或村里名稱
        public string Addr { get; set; }

        // 郵遞區號
        public string ZipCode { get; set; }
    }
}