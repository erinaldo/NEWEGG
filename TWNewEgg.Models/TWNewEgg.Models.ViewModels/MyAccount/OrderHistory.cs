using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.MyAccount
{
   public  class OrderHistory
    {

        public enum statusList
        {
            付款失敗已取消 = -4,
            無法配達 = -3,
            被動取消 = -2,
            已取消 = -1,
            確認中 = 0,
            訂單成立 = 1,
            空運中 = 2,
            待出貨 = 3,
            已出貨 = 4,
            已配達 = 5,
            退貨處理中 = 6,
            付款成功 = 30,
            貨到付款 = 31,
            訂單異常 = 32,
            付款失敗 = 33,
            訂購成功 = 34,
            台新分期處理中 = 26,
            歐付寶儲值支付處理中 = 81,
            歐付寶WebATM處理中 = 82,
            歐付寶線下ATM處理中 = 83,


            未付款 = 99 // 初始狀態

        }


        public enum retStatusList
        {
            退貨處理中 = 0,
            退貨中 = 1,
            完成退貨 = 2,

            退款中 = 3,
            完成退款 = 4,

            退貨異常 = 5,
            退款異常 = 6,

            退貨取消 = 7,
            退款取消 = 8,
        }

        public int totalpage { get; set; }
       public List<SalceOrder> SalceOrderList { get; set; }
    }

   public class SalceOrder  
   {
       public string ItemUrl { get;set; }
       public int Delivtype { get; set; }
       public int SalesOrderGroupID { get; set; }
       public string CountryName { get; set; }
       public string Status { get; set; }
       public string Code { get; set; }
       public string CreateDate { get; set; }
       public string RecvName { get; set; }
       public string DelivLOC { get; set; }
       public string DelivADDR { get; set; }
       public string RecvMobile { get; set; }
       public string RecvTelDay { get; set; }
       public string RecvTelNight { get; set; }
       public string InvoiceTitle { get; set; }
       public string InvoiceID { get; set; }
       public int DelivType { get; set; } 
       public string DelvStatus { get; set; }
       public string InvoiceNumber { get; set; }
       public decimal PiceSum { get; set; }
       public bool IsFix { get; set; }
       public bool IsReturnd { get; set; }
       public bool IsRefund { get; set; }
       public string PaytypeNmae { get; set; }
       public int SumQTY { get; set; }
       public bool Paytypeboolen { get; set; }
       public List<SalesOrderItem> SalesOrderItemDetil{ get; set; }
       public string InvoiceNo { get; set; }
       public DateTime Procout { get; set; }
       public string CardNo { get; set; }
       public string CardBank { get; set; }
       public DateTime Expire { get; set; }
       public string InvoiceInDate { get; set; }
   }


   public class SalesOrderItem 
   {
       public string CountryName { get; set; }
       public string Name { get; set; }

       public decimal DisplayPrice { get; set; }
       public int Qty { get; set; }
       public decimal InstallmentFee { get; set; }
       public decimal ApportionedAmount { get; set; }//折扣金額+滿額贈分攤金額
       public string PayType { get; set; }
       public Nullable<System.DateTime> ProcOut { get; set; }
       public string DelivNO { get; set; }
       public decimal DisplayPriceTemp { get; set; }
       public string Deliver { get; set; }
       public string DeliverName { get; set; }
       public string DeliverWebSite { get; set; }
   }
}
