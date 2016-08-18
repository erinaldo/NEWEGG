using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class UpdateRetGoodsInfo
    {
        public UpdateRetGoodsInfo()
        {
            // 預設為已開箱
            //this.Retgood_ProductStatus = 1;
        }
        // 訂單編號
        public string Cart_ID { get; set; }
        // 訂單付款模式
        //public Nullable<int> Cart_PayType { get; set; }
        // 商品類型
        //public Nullable<int> Cart_ShipType { get; set; }
        // 使用者AccountID
        //public string Cart_UserID { get; set; }
        // 退貨單號
        //public string Retgood_Code { get; set; }
        // Email
        //public string Retgood_FrmEmail { get; set; }
        // 
        //public string Retgood_SendStatus { get; set; }
        // 退貨單狀態
        public Nullable<int> Retgood_Status { get; set; }
        // 原因備註
        public string OtherUpDataNote { get; set; }
        // 換貨原訂單編號
        //public string Retgood_ChangeSalesOrderCode { get; set; }
        // 運費分攤(國際物流)
        //public Nullable<decimal> Retgood_FreightalLocation { get; set; }
        // 退運運費
        //public Nullable<decimal> Retgood_REShipping { get; set; }
        // 貨運單號
        public string Retgood_ShpCode { get; set; }
        // 稅賦(只有三角商品才需要輸入)
        //public Nullable<decimal> Retgood_TaxCost { get; set; }
        // 商品狀態(0:未開箱、1:已開箱、2:已損壞)
        //public Nullable<int> Retgood_ProductStatus { get; set; }
        // 是否為拒收或配達的情況
        //public Nullable<int> Retgood_Declined { get; set; }

        //public string Retgood_FinreturndateNote { get; set; }
    }
}
