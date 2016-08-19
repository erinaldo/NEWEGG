using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class StorageDetailSPResult
    {
        //Ben 2013-12-25
        [DisplayName("商家商品編號")]
        public string SellerProductID { get; set; }
       
        [DisplayName("新蛋產品編號")]
        public int NewEggProductID { get; set; }
         
        [DisplayName("商品說明")]
        public string ProductDescriptionTW { get; set; }

        [DisplayName("商品成色")]
        public string ProductStatus { get; set; }

        [DisplayName("日期自")]
        public string StoreStartDate { get; set; }

        [DisplayName("日期至")]
        public string StoreEndDate { get; set; }

        [DisplayName("長度(英吋)")]
        public decimal LengthInches { get; set; }

        [DisplayName("寬度(英吋)")]
        public decimal WidthInches { get; set; }

        [DisplayName("高(英吋)")]
        public decimal HeightInches { get; set; }

        [DisplayName("尺寸(立方英尺)")]
        public decimal Cubicfoot { get; set; }
       
        [DisplayName("倉儲費率($)")]
        public decimal StorageFeeRate { get; set; }
        
        [DisplayName("Avg instock")]
        public int  AvgInstock { get; set; }
        
       [DisplayName("倉儲費用($)")]
       public decimal StorageFee { get; set; }
        
       [DisplayName("結帳編號")]
       public string SettlementID { get; set; }
       

    }



}
