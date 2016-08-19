using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartResults_View
    {
        public CartResults_View() {
            this.SalesOrderGroup_View = new SalesOrderGroup_View();
            this.CartPayType_View = new CartPayType_View();
            this.SalesOrder_ViewList = new List<SalesOrder_View>();
            this.TotalPrice = 0;
            this.PromotionPriceSum = 0;
            this.CouponePriceSum = 0;
            this.NeedPayMoneyPriceSum = 0;
            this.Status = "付款成功";
            this.redeemInfo = new RedeemInfo();
            //this.NCCCResultData = new DomainModels.PaymentGateway.NCCCResult();
        }
        // So Group資料
        public SalesOrderGroup_View SalesOrderGroup_View { get; set; }
        // So 付款資料
        public CartPayType_View CartPayType_View { get; set; }
        // SO 明細資料
        public List<SalesOrder_View> SalesOrder_ViewList { get; set; }
        // SO 總計
        public int TotalPrice { get; set; }
        // SO 滿額折優惠
        public int PromotionPriceSum { get; set; }
        // SO 購物金優惠
        public int CouponePriceSum { get; set; }
        // SO 分期手續費
        public int InstallmentFeeSum { get; set; }
        // SO 應付金額
        public int NeedPayMoneyPriceSum { get; set; }

        public string Status { get; set; }

        public string TimeofReceipt { get; set; }

        // 尚未結帳購物車
        public int OtherCartNumber { get; set; }
        // 是否顯示收貨人
        public int IsConsignee { get; set; }

        public RedeemInfo redeemInfo { get; set; }
        //public TWNewEgg.Models.DomainModels.PaymentGateway.NCCCResult NCCCResultData { get; set; }

    }
    public class RedeemInfo
    {
        public string BankRedeemName { get; set; }
        public int BonusUsed { get; set; }
        public int BonusBLN { get; set; }
        public int AmountSelf { get; set; }
    }
}