using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("salesorder")]
    public class SalesOrder
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
            聯信一次處理中 = 70,
            中信紅利處理中 = 72,
            聯邦紅利處理中 = 73,
            聯信紅利處理中 = 74,
            台新紅利處理中 = 76,
            信用卡資料修改 = 31,
            超商付款處理中 = 80,
            未付款 = 99 // 初始狀態
        }

        public SalesOrder()
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

        /// <summary>
        /// 訂購單編號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        /// <summary>
        /// NULL
        /// </summary>
        public Nullable<int> SalesOrderGroupID { get; set; }
        /// <summary>
        /// 訂購人身分證字號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 訂購人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public int AccountID { get; set; }
        /// <summary>
        /// 訂購人電話(白)
        /// </summary>
        public string TelDay { get; set; }
        /// <summary>
        ///  訂購人電話(夜)
        /// </summary>
        public string TelNight { get; set; }
        /// <summary>
        /// 訂購人電話(行)
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 訂購人電子郵件信箱
        /// </summary>
        public string Email { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Nullable<int> PayType { get; set; }
        /// <summary>
        /// 預計到貨日
        /// </summary>
        public Nullable<System.DateTime> StarvlDate { get; set; }
        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardHolder { get; set; }
        /// <summary>
        /// 持卡人電話(白)
        /// </summary>
        public string CardTelDay { get; set; }
        /// <summary>
        /// 訂購人電話(夜)
        /// </summary>
        public string CardTelNight { get; set; }
        /// <summary>
        /// 訂購人電話(行)
        /// </summary>
        public string CardMobile { get; set; }
        /// <summary>
        /// 信用卡帳單縣市
        /// </summary>
        public string CardLOC { get; set; }
        /// <summary>
        /// 信用卡帳單郵遞區號
        /// </summary>
        public string CardZip { get; set; }
        /// <summary>
        /// 信用卡帳單地址
        /// </summary>
        public string CardADDR { get; set; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 卡號檢查碼
        /// </summary>
        public string CardNochk { get; set; }
        /// <summary>
        /// 卡別
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 發卡銀行
        /// </summary>
        public string CardBank { get; set; }
        /// <summary>
        /// 信用卡到期日
        /// </summary>
        public string CardExpire { get; set; }
        /// <summary>
        /// 持卡人出生日期
        /// </summary>
        public Nullable<System.DateTime> CardBirthday { get; set; }
        /// <summary>
        /// 發票收件人
        /// </summary>
        public string InvoiceReceiver { get; set; }
        /// <summary>
        /// 發票統編
        /// </summary>
        public string InvoiceID { get; set; }
        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 發票縣市
        /// </summary>
        public string InvoiceLoc { get; set; }
        /// <summary>
        /// 發票郵遞區號
        /// </summary>
        public string InvoiceZip { get; set; }
        /// <summary>
        /// 發票地址
        /// </summary>
        public string InvoiceAddr { get; set; }
        /// <summary>
        /// 收貨人
        /// </summary>
        public string RecvName { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public string RecvEngName { get; set; }
        /// <summary>
        /// 收貨人電話(白)
        /// </summary>
        public string RecvTelDay { get; set; }
        /// <summary>
        /// 收貨人電話(夜)
        /// </summary>
        public string RecvTelNight { get; set; }
        /// <summary>
        /// 收貨人電話(行)
        /// </summary>
        public string RecvMobile { get; set; }
        /// <summary>
        /// 交貨方式
        /// </summary>
        public Nullable<int> DelivType { get; set; }
        /// <summary>
        /// 交貨資訊
        /// </summary>
        public string DelivData { get; set; }
        /// <summary>
        /// 送貨縣市
        /// </summary>
        public string DelivLOC { get; set; }
        /// <summary>
        /// 送貨郵遞區號
        /// </summary>
        public string DelivZip { get; set; }
        /// <summary>
        /// 送貨地址
        /// </summary>
        public string DelivADDR { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public string DelivEngADDR { get; set; }
        /// <summary>
        /// 送貨注意事項
        /// </summary>
        public string DelivHitNote { get; set; }
        /// <summary>
        /// 徵信日期
        /// </summary>
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        /// <summary>
        /// 徵信備註
        /// </summary>
        public string ConfirmNote { get; set; }
        /// <summary>
        /// 授權日期
        /// </summary>
        public Nullable<System.DateTime> AuthDate { get; set; }
        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 授權備註
        /// </summary>
        public string AuthNote { get; set; }
        /// <summary>
        /// 期數
        /// </summary>
        public Nullable<int> HpType { get; set; }
        /// <summary>
        /// 入賬日期
        /// </summary>
        public Nullable<System.DateTime> RcptDate { get; set; }
        /// <summary>
        /// 入賬備註
        /// </summary>
        public string RcptNote { get; set; }
        /// <summary>
        /// 付款到期日期
        /// </summary>
        public Nullable<System.DateTime> Expire { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public Nullable<System.DateTime> DateDEL { get; set; }
        /// <summary>
        /// 網站來源代碼
        /// </summary>
        public string CoServerName { get; set; }
        /// <summary>
        /// 網站來源
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 活動代號
        /// </summary>
        public string ActCode { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public Nullable<int> Status { get; set; }
        /// <summary>
        /// 狀態備註
        /// </summary>
        public string StatusNote { get; set; }
        /// <summary>
        /// 使用者IP
        /// </summary>
        public string RemoteIP { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public Nullable<System.DateTime> Date { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 備註2
        /// </summary>
        public string Note2 { get; set; }
        /// <summary>
        /// Null
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 實際建檔日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Updated 
        /// </summary>
        public Nullable<int> Updated { get; set; }
        /// <summary>
        /// 最後修改人
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 更改日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string invoiceCarrierType { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Type
        public string invoiceCarrierId1 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id1
        public string invoiceCarrierId2 { get; set; } //2015.04.20 add by Bill 紀錄電子發票的Carrier Id2
        public string invoiceDonateCode { get; set; } //2015.04.20 add by Bill 紀錄電子發票的捐贈碼

        public SalesOrder(TWNewEgg.DB.TWBACKENDDB.Models.Cart cart)
        {
            this.Code = cart.ID;
            this.SalesOrderGroupID = cart.SalesorderGroupID;
            this.IDNO = cart.IDNo;
            this.Name = cart.Username;
            this.AccountID = Convert.ToInt32(cart.UserID);
            this.TelDay = cart.TelDay;
            this.TelNight = cart.TelNight;
            this.Mobile = cart.Mobile;
            this.Email = cart.Email;
            this.PayType = cart.PayType;
            //預計交貨日	=	cart.salesorder_starvldate;
            this.CardHolder = cart.CardHolder;
            this.CardTelDay = cart.CardPhone;
            this.CardTelNight = cart.CardPhone2;
            this.CardMobile = cart.CardMobile;
            this.CardLOC = cart.CardLocation;
            this.CardZip = cart.CardZipcode;
            this.CardADDR = cart.CardADDR;
            this.CardNo = cart.CardNO;
            this.CardNochk = cart.CardCheck;
            this.CardType = cart.CardType;
            this.CardBank = cart.CardBank;
            this.CardExpire = cart.CardExpire;
            this.CardBirthday = cart.Birthday;
            this.InvoiceReceiver = cart.InvoReceiver;
            this.InvoiceID = cart.ActCode;                ///發票統編存在cart_actcode
            this.InvoiceTitle = cart.IinvoiceTitle;
            this.InvoiceLoc = cart.InvoLocation;
            this.InvoiceZip = cart.InvoZipcode;
            this.InvoiceAddr = cart.InvoADDR;
            this.RecvName = cart.Receiver;
            this.RecvTelDay = cart.Phone;
            this.RecvTelNight = cart.Phone2;
            this.RecvMobile = cart.RecvMobile;
            this.DelivType = (byte?)cart.ShipType;
            this.DelivData = cart.DelivData;
            this.DelivLOC = cart.Location;
            this.DelivZip = cart.Zipcode;
            this.DelivADDR = cart.ADDR;
            this.DelivHitNote = cart.DelivHitNote;
            //徵信日期	=	this.salesorder_confirmdate=
            //徵信備註	=	this.salesorder_confirmnote=
            this.AuthDate = cart.AuthDate;
            this.AuthCode = cart.AuthCode;
            this.AuthNote = cart.AuthNote;
            this.HpType = cart.HpType;
            //入帳日	=	this.salesorder_rcptdate=
            //入帳備註	=	this.salesorder_rcptnote=
            //付款到期日	=	this.salesorder_expire=
            //付款刪除日	=	this.salesorder_datedel=
            this.CoServerName = cart.CoserverName;
            this.ServerName = cart.ServerName;
            //this.salesorder_actcode = cart.cart_actcode;
            this.Status = (byte?)cart.Status;
            //狀態備註	=	this.salesorder_statusnote=
            this.RemoteIP = cart.UsrIP;
            this.Date = cart.Date;
            this.Note = cart.Note;
            this.RecvEngName = cart.RecvENGName;
            this.DelivEngADDR = cart.DelivENGADDR;
            //	=	this.salesorder_note2=
            //建立者	=	this.salesorder_createuser=
            this.CreateDate = (cart.CreateDate != null) ? cart.CreateDate.Value : DateTime.Parse("1990/01/01");
            this.Updated = cart.Updated;
            this.UpdateUser = cart.UpdateUser;
            this.UpdateDate = cart.UpdateDate;
            this.Status = (byte?)cart.Status;

            this.ActCode = cart.InvoiceNO;          ///發票號碼暫存於此欄位

            this.invoiceCarrierType = cart.invoiceCarrierType;
            this.invoiceCarrierId1 = cart.invoiceCarrierId1;
            this.invoiceCarrierId2 = cart.invoiceCarrierId2;
            this.invoiceDonateCode = cart.invoiceDonateCode;
        }

    }
}
