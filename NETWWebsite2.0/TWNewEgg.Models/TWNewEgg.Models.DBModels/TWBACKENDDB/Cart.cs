using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{

    [Table("Cart")]
    public class Cart
    {
        public enum paytypestatus
        {
            信用卡一次付清 = 1,
            三期零利率 = 3,
            六期零利率 = 6,
            九期零利率 = 9,
            十期零利率 = 10,
            十二期零利率 = 12,
            十八期零利率 = 18,
            二十四期零利率 = 24,
            三期分期 = 103,
            六期分期 = 106,
            九期分期 = 109,
            十期分期 = 110,
            十二期分期 = 112,
            十八期分期 = 118,
            二十四期分期 = 124,
            三十期分期 = 130,
            信用卡紅利折抵 = 201,
            網路ATM = 30,
            貨到付款 = 31,
            超商付款 = 32,
            電匯 = 33,
            實體ATM = 34,
            歐付寶儲值支付 = 501
        }

        public enum bank
        {
            中國信託商業銀行 = 822,
            seven = 83,
            全家 = 84,
            花旗銀行 = 021,
            第一商業銀行 = 007,
            國泰世華聯合商業銀行 = 013,
            新竹貨運 = 85,
            歐付寶 = 10006,
            Newegg = 10004
        }

        public enum status
        {
            正常 = 0,
            取消 = 1,
            被動取消 = 2,
            付款成功拋單失敗 = 4,
            退貨 = 5,
            完成 = 7,
            初始狀態 = 99
        }

        public enum cartstatus
        {
            待出貨 = 0,
            配達 = 2,
            已出貨 = 1,
            已成立 = 6,
            空運中 = 5,
            待進貨 = 7,
            已進貨 = 8,
            初始狀態 = 999
        }

        public Cart()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            //cart_date = defaultDate;
            //cart_createdate = DateTime.Now;
            //cart_updatedate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string IDNo { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Zipcode { get; set; }
        public string ADDR { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Receiver { get; set; }
        public string IinvoiceTitle { get; set; }
        public string InvoiceNO { get; set; }
        public string Note { get; set; }
        public string CardType { get; set; }
        public string CardNO { get; set; }
        public string CardExpire { get; set; }
        public string CardHolder { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string CardADDR { get; set; }
        public string CardBank { get; set; }
        public Nullable<int> StoreID { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        public Nullable<int> PayType { get; set; }

        public string UsrIP { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CardCheck { get; set; }
        public Nullable<int> Device { get; set; }
        public string CardPhone { get; set; }
        public string CardPhone2 { get; set; }
        public string CardMobile { get; set; }
        public Nullable<int> PaySelect { get; set; }
        public string Username { get; set; }
        public Nullable<int> ShipType { get; set; }
        public Nullable<int> ShipPrice { get; set; }
        public Nullable<int> Status { get; set; }
        public string Location { get; set; }
        public string CardLocation { get; set; }
        public string CardZipcode { get; set; }
        public string ServerName { get; set; }
        public string CoserverName { get; set; }
        public Nullable<int> HpType { get; set; }
        public string InvoLocation { get; set; }
        public string InvoZipcode { get; set; }
        public string InvoADDR { get; set; }
        public string InvoReceiver { get; set; }
        public string FrmCode { get; set; }
        public Nullable<int> UsrLOC { get; set; }
        public string UserZipcode { get; set; }
        public string UsrADDR { get; set; }
        public string CMPName { get; set; }
        public Nullable<int> CMPLOC { get; set; }
        public string CMOZipcode { get; set; }
        public string CMPADDR { get; set; }
        public string CntName { get; set; }
        public string CnRelation { get; set; }
        public string CntTelCMP { get; set; }
        public string CntTelHome { get; set; }
        public string CntMobile { get; set; }
        public string StID { get; set; }
        public string StName { get; set; }
        public string StPhone { get; set; }
        public string StADDR { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string TelDay { get; set; }
        public string TelNight { get; set; }
        public string RecvMobile { get; set; }
        public string DelivData { get; set; }
        public string DelivHitNote { get; set; }
        public Nullable<System.DateTime> AuthDate { get; set; }
        public string AuthCode { get; set; }
        public string AuthNote { get; set; }
        public string ActCode { get; set; }
        public string StatusNote { get; set; }
        public Nullable<int> SalesorderGroupID { get; set; }
        //public int SalesorderGroupID { get; set; }
        public string RecvENGName { get; set; }
        public string DelivENGADDR { get; set; }
        public string DelivNO { get; set; }
        public int? DelvStatus { get; set; }
        public Nullable<System.DateTime> DelvStatusDate { get; set; }
        public int? RefSONumber { get; set; }
        public int? Forwarder { get; set; }
        public int? InventoryStatus { get; set; }
        public DateTime? InventoryStatusDate { get; set; }
        public string IinventorystatusUser { get; set; }
        public string CheckCustomsDoc { get; set; }//BY Penny
        public string UserNote { get; set; }//BY Penny
        public virtual ICollection<Process> processes { get; set; }
        public Nullable<int> WarehouseID { get; set; }//BY Penny
        public string UpdateNote { get; set; }//BY Penny

        // 快遞單號
        public string Deliv4NO { get; set; }//BY Penny

        // 到店時間 (萊爾富)
        public Nullable<System.DateTime> O2OShopDeliveryDate { get; set; }//BY Penny

        // 已發送退貨檔知標記
        public string O2ORMAfalg { get; set; }//BY Penny

        public Nullable<System.DateTime> O2ODeliveryDate { get; set; }//BY Penny
        public int? O2OForwarder { get; set; }
    }
}
