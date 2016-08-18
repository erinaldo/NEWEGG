using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
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
            WebATM轉帳 = 30,
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
            尚未付款 = 8,
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

        /// <summary>
        /// 購物單編號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string IDNo { get; set; }
        /// <summary>
        /// 金額
        /// </summary>
        public Nullable<decimal> Price { get; set; }
        /// <summary>
        /// 品項數量
        /// </summary>
        public Nullable<int> Qty { get; set; }
        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string Zipcode { get; set; }
        /// <summary>
        /// 受件人住址
        /// </summary>
        public string ADDR { get; set; }
        /// <summary>
        /// 訂購人電話(日)
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 訂購人電話(夜)
        /// </summary>
        public string Phone2 { get; set; }
        /// <summary>
        /// 訂購人行動電話
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 收件人電子信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string IinvoiceTitle { get; set; }
        /// <summary>
        /// 發票統編
        /// </summary>
        public string InvoiceNO { get; set; }
        /// <summary>
        /// 購物車備註(手寫備註)
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 付款人信用卡種類
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 付款人信用卡號
        /// </summary>
        public string CardNO { get; set; }
        /// <summary>
        /// 付款人信用卡到期日
        /// </summary>
        public string CardExpire { get; set; }
        /// <summary>
        /// 信用卡持卡人
        /// </summary>
        public string CardHolder { get; set; }
        /// <summary>
        /// 持卡人出生日期
        /// </summary>
        public Nullable<System.DateTime> Birthday { get; set; }
        /// <summary>
        /// 信用卡帳單地址
        /// </summary>
        public string CardADDR { get; set; }
        /// <summary>
        /// 信用卡發卡銀行
        /// </summary>
        public string CardBank { get; set; }
        /// <summary>
        /// 商場代號
        /// </summary>
        public Nullable<int> StoreID { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Nullable<int> PayType { get; set; }
        /// <summary>
        /// 購買人IP
        /// </summary>
        public string UsrIP { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        /// <summary>
        /// 付款人信用卡檢查碼
        /// </summary>
        public string CardCheck { get; set; }
        public Nullable<int> Device { get; set; }
        /// <summary>
        /// 付款人電話(日)
        /// </summary>
        public string CardPhone { get; set; }
        /// <summary>
        /// 付款人電話(夜)
        /// </summary>
        public string CardPhone2 { get; set; }
        /// <summary>
        /// /// <summary>
        /// 付款人行動電話
        /// </summary>
        public string CardMobile { get; set; }
        public Nullable<int> PaySelect { get; set; }
        /// <summary>
        /// 購買人姓名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 送貨方式
        /// </summary>
        public Nullable<int> ShipType { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public Nullable<int> ShipPrice { get; set; }
        /// <summary>
        /// 店單狀態 0正常 1取消 2被動取消 5退貨 7完成 99初始值
        /// </summary>
        public Nullable<int> Status { get; set; }
        /// <summary>
        /// 收件人所在縣市
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 信用卡帳單寄送地址所在縣市
        /// </summary>
        public string CardLocation { get; set; }
        /// <summary>
        /// 信用卡帳單寄送地址郵遞區號
        /// </summary>
        public string CardZipcode { get; set; }
        /// <summary>
        /// 下訂網站
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 訂單來自
        /// </summary>
        public string CoserverName { get; set; }
        /// <summary>
        /// 分期付款期數
        /// </summary>
        public Nullable<int> HpType { get; set; }
        /// <summary>
        ///  發票縣市
        /// </summary>
        public string InvoLocation { get; set; }
        /// <summary>
        /// 發票郵遞區號
        /// </summary>
        public string InvoZipcode { get; set; }
        /// <summary>
        /// 發票地址
        /// </summary>
        public string InvoADDR { get; set; }
        /// <summary>
        /// 發票收件人
        /// </summary>
        public string InvoReceiver { get; set; }
        /// <summary>
        /// 來源表單編號
        /// </summary>
        public string FrmCode { get; set; }
        //購買人縣市
        public Nullable<int> UsrLOC { get; set; }
        /// <summary>
        /// 購買人郵遞區號
        /// </summary>
        public string UserZipcode { get; set; }
        /// <summary>
        /// 購買人地址
        /// </summary>
        public string UsrADDR { get; set; }
        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CMPName { get; set; }
        /// <summary>
        /// 公司縣市
        /// </summary>
        public Nullable<int> CMPLOC { get; set; }
        /// <summary>
        /// 公司郵遞缺號
        /// </summary>
        public string CMOZipcode { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string CMPADDR { get; set; }
        /// <summary>
        /// 聯絡人姓名
        /// </summary>
        public string CntName { get; set; }
        /// <summary>
        /// 聯絡人關係
        /// </summary>
        public string CnRelation { get; set; }
        /// <summary>
        /// 聯絡人電話(公)
        /// </summary>
        public string CntTelCMP { get; set; }
        /// <summary>
        /// 聯絡人電話(家)
        /// </summary>
        public string CntTelHome { get; set; }
        /// <summary>
        /// 聯絡人電話(行)
        /// </summary>
        public string CntMobile { get; set; }
        /// <summary>
        /// 店配-門市編號
        /// </summary>
        public string StID { get; set; }
        /// <summary>
        /// 店配-門市名稱 
        /// </summary>
        public string StName { get; set; }
        /// <summary>
        /// 店配-門市電話
        /// </summary>
        public string StPhone { get; set; }
        /// <summary>
        /// 店配-門市地址
        /// </summary>
        public string StADDR { get; set; }
        /// <summary>
        /// 狀態修改日期
        /// </summary>
        public Nullable<System.DateTime> StatusDate { get; set; }
        /// <summary>
        /// 實際建檔日期
        /// </summary>
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        /// <summary>
        /// 最後修改人
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 最後修改日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        /// <summary>
        /// 收件人電話
        /// </summary>
        public string TelDay { get; set; }
        /// <summary>
        /// 收件人(夜)
        /// </summary>
        public string TelNight { get; set; }
        /// <summary>
        /// 收件人行動電話
        /// </summary>
        public string RecvMobile { get; set; }

        public string DelivData { get; set; }
        public string DelivHitNote { get; set; }
        public Nullable<System.DateTime> AuthDate { get; set; }
        public string AuthCode { get; set; }
        public string AuthNote { get; set; }
        /// <summary>
        /// 公司統編
        /// </summary>
        public string ActCode { get; set; }
        public string StatusNote { get; set; }
        public Nullable<int> SalesorderGroupID { get; set; }
        //public int SalesorderGroupID { get; set; }
        public string RecvENGName { get; set; }
        public string DelivENGADDR { get; set; }
        public string DelivNO { get; set; }
        /// <summary>
        /// 出貨狀態 
        /// 0 出貨中，消費者已付款等待出貨中
        /// 1 已出貨，已開發票
        /// 2 配達，貨運公司已送達消費者手中
        /// 5 空運中
        /// 6 已成立，從SO轉單而來是6
        /// 7 待進貨
        /// 8 已進貨
        /// 999 初始狀態，現在已沒有
        /// </summary>
        public int? DelvStatus { get; set; }
        /// <summary>
        /// 填寫tracking number的時間
        /// </summary>
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

        /// <summary>
        /// 快遞單號
        /// </summary>
        public string Deliv4NO { get; set; }//BY Penny
        /// <summary>
        ///  到店時間 (萊爾富)
        /// </summary>
        public Nullable<System.DateTime> O2OShopDeliveryDate { get; set; }//BY Penny
        /// <summary>
        /// 已發送退貨檔知標記
        /// </summary>
        public string O2ORMAfalg { get; set; }//BY Penny

        public Nullable<System.DateTime> O2ODeliveryDate { get; set; }//BY Penny
        public int? O2OForwarder { get; set; }

        public string invoiceCarrierType { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Type
        public string invoiceCarrierId1 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id1
        public string invoiceCarrierId2 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id2
        public string invoiceDonateCode { get; set; } //2015.04.20 add by Bill 紀錄電子發票的捐贈碼
    }
}
