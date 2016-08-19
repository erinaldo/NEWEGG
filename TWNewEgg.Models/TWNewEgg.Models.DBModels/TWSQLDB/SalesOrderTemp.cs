using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("salesorderTemp")]
    public class SalesOrderTemp
    {
        public enum DelivTypeList
        {
            BuyOut = 0, // 切貨;
            Transshipment = 1, // 間配;
            DirectShipment = 2, // 直配;
            TriangleTrade = 3, // 三角;
            JieMai = 4, // 借賣網;
            //05 = 5, // 自貿區
            OverSeaBuyOut = 6, // 海外切貨
            B2CDirectShipment = 7, // B2C直配
            MKPLwarehouse = 8, // MKPL寄倉
            B2Cwarehouse = 9, // B2C寄倉
        }

        public enum status
        {
            付款成功 = 0,
            取消 = 1,

            付款失敗取消訂單 = 2,
            取消已補數量可查詢 = 3,

            付款成功拋單失敗 = 4,
            退貨 = 5,
            完成 = 7,
            貨到付款_暫不使用 = 8,
            中信分期處理中 = 21,
            聯邦分期處理中 = 22,
            聯信分期處理中 = 23,
            系統TWPAY處理中 = 25,
            台新分期處理中 = 26,
            NCCC處理中 = 27,
            聯信一次處理中 = 70,
            中信紅利處理中 = 72,
            聯邦紅利處理中 = 73,
            聯信紅利處理中 = 74,
            台新紅利處理中 = 76,
            信用卡資料修改 = 31,
            超商付款處理中 = 80,
            歐付寶儲值支付處理中 = 81,
            歐付寶WebATM處理中 = 82,
            歐付寶線下ATM處理中 = 83,
            歐付寶分期處理中 = 84,
            未付款 = 99 // 初始狀態

        }

        public SalesOrderTemp()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            StarvlDate = defaultDate;
            ConfirmDate = defaultDate;
            AuthDate = defaultDate;
            RcptDate = defaultDate;
            Expire = defaultDate;
            DateDEL = defaultDate;
            Date = defaultDate;
            CreateDate = DateTime.Now;
            UpdateDate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Code { get; set; }
        public Nullable<int> SalesOrderGroupID { get; set; }
        public string IDNO { get; set; }
        public string Name { get; set; }
        public int AccountID { get; set; }
        public string TelDay { get; set; }
        public string TelNight { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<System.DateTime> StarvlDate { get; set; }
        public string CardHolder { get; set; }
        public string CardTelDay { get; set; }
        public string CardTelNight { get; set; }
        public string CardMobile { get; set; }
        public string CardLOC { get; set; }
        public string CardZip { get; set; }
        public string CardADDR { get; set; }
        public string CardNo { get; set; }
        public string CardNochk { get; set; }
        public string CardType { get; set; }
        public string CardBank { get; set; }
        public string CardExpire { get; set; }
        public Nullable<System.DateTime> CardBirthday { get; set; }
        public string InvoiceReceiver { get; set; }
        public string InvoiceID { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceLoc { get; set; }
        public string InvoiceZip { get; set; }
        public string InvoiceAddr { get; set; }
        public string RecvName { get; set; }
        public string RecvEngName { get; set; }
        public string RecvTelDay { get; set; }
        public string RecvTelNight { get; set; }
        public string RecvMobile { get; set; }
        public Nullable<int> DelivType { get; set; }
        public string DelivData { get; set; }
        public string DelivLOC { get; set; }
        public string DelivZip { get; set; }
        public string DelivADDR { get; set; }
        public string DelivEngADDR { get; set; }
        public string DelivHitNote { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public string ConfirmNote { get; set; }
        public Nullable<System.DateTime> AuthDate { get; set; }
        public string AuthCode { get; set; }
        public string AuthNote { get; set; }
        public Nullable<int> HpType { get; set; }
        public Nullable<System.DateTime> RcptDate { get; set; }
        public string RcptNote { get; set; }
        public Nullable<System.DateTime> Expire { get; set; }
        public Nullable<System.DateTime> DateDEL { get; set; }
        public string CoServerName { get; set; }
        public string ServerName { get; set; }
        public string ActCode { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public string RemoteIP { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Note { get; set; }
        public string Note2 { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string invoiceCarrierType { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Type
        public string invoiceCarrierId1 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id1
        public string invoiceCarrierId2 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id2
        public string invoiceDonateCode { get; set; } //2015.04.20 add by Bill 紀錄電子發票的捐贈碼\
    }
}
