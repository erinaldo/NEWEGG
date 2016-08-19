using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class SellerRelationshipManagement
    {
        public SellerRelationshipManagement()
        {
            currencyList = new List<API.Models.GetCurrencyListResult>();

            //CreateDateOptions.Add("全部");
            //CreateDateOptions.Add("今天");
            //CreateDateOptions.Add("最近3天");
            //CreateDateOptions.Add("最近7天");
            //CreateDateOptions.Add("最近30天");
            //CreateDateOptions.Add("指定日期");
        }

        //public enumSellerStatus enumSellerStatus;

        public enum enumSellerStatus
        {
            Active = 0,
            Inactive = 1,
            Closed = 2
        }
        public List<string> CreateDateOptions = new List<string>();

        public enum enumDropDownCreateDate
        {
            全部 = 0,
            今天,
            最近3天,
            最近7天,
            最近30天,
            指定日期
        }

        public enum enumDropDownUpdateDate
        {
            全部 = 0,
            今天,
            最近3天,
            最近7天,
            最近30天,
            指定日期
        }

        public List<TWNewEgg.API.Models.GetCurrencyListResult> currencyList;

        /// <summary>
        /// 狀態
        /// </summary>
        public string SellerStatus { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        /// <remarks> public DateTime? CreateDate { get; set; } </remarks>
        public DateTime? CreateDateS { get; set; }
        public DateTime? CreateDateE { get; set; }

        /// <summary>
        /// 最後編輯日期
        /// </summary>
        /// <remarks> public DateTime LastEditDate { get; set; } </remarks>
        public Nullable<System.DateTime> UpdateDateS { get; set; }
        public Nullable<System.DateTime> UpdateDateE { get; set; }

        /// <summary>
        /// 地區
        /// </summary>
        /// <remarks> public string Region { get; set; } </remarks>
        public string SellerCountryCode { get; set; }


        /// <summary>
        /// 商家關係詳細資料
        /// </summary>
        public class SellerRelationshipDetails 
        {
            [DisplayName("編號")]
            public string SellerID { get; set; }

            [DisplayName("名稱")]
            public string SellerName { get; set; }

            [DisplayName("創建日期")]
            public DateTime? InDate { get; set; }

            [DisplayName("最後編輯日期")]
            public DateTime? UpdateDate { get; set; }

            [DisplayName("地區")]
            //public string Region { get; set; }
            public string CountryCode { get; set; }

            //國家/地區全名(非簡碼)
            public string SellerCountryCodeName { get; set; }

            [DisplayName("狀態")]
            public string SellerStatus { get; set; }

            [DisplayName("帳戶類別")]
            //public string AccountType { get; set; }
            public string AccountTypeCode { get; set; }

            [DisplayName("幣別")]
            public string Currency { get; set; }

            [DisplayName("統編/身分證字號")]
            public string CompanyCode { get; set; }

            [DisplayName("廠商身分別")]
            public int? Identy { get; set; }

            [DisplayName("付款方式")]
            public int? BillingCycle { get; set; }

            public string SellerEmail { get; set; }

            public string SellerUserStatus { get; set; }
            public string function_1 { get; set; }
            public string function_2 { get; set; }
        }
    }
}
