using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class ManufacturerAdditional:TWNewEgg.API.Models.Manufacturer
    {
        public string PhoneNumberDetail { get; set; }
        public bool isAdmin { get; set; }
    }

    public enum ManufacturerValidateSummaryResult
    {
        // 驗證通過
        Success,

        // 驗證失敗
        Error
    }

    public class ItemListManufacturer
    {
        public string ManufactureName { get; set; }
        public int SN { get; set; }
    }
}
