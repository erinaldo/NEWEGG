using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartTempDM
    {
        public CartTempDM()
        {
            this.Status = 0;
            this.Updated = 0;
            this.CartCouponTempDMs = new List<CartCouponTempDM>();
            this.CartItemTempDMs = new List<CartItemTempDM>();
        }
        
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> PayTypeGroupID { get; set; }
        public Nullable<int> CartTypeID { get; set; }
        public Nullable<int> SalesOrderGroupID { get; set; }
        public int Status { get; set; }
        public string IPAddress { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public List<CartCouponTempDM> CartCouponTempDMs { get; set; }
        public List<CartItemTempDM> CartItemTempDMs { get; set; }

        /// <summary>
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </summary>
        public string AgreedDiscard4 { get; set; }


        public enum UpdateResultErrorCode
        {
            無異常 = 0,
            處理中 = 1,
            連線逾時 = 2,
            資料錯誤 = 3,
            系統錯誤 = 4,
            系統流程錯誤 = 5
        }

        public enum StatusEnum
        {
            Initial = 0,
            Step2 = 2,
            Step3 = 3,
            SOCreated = 100
        }
    }
}
