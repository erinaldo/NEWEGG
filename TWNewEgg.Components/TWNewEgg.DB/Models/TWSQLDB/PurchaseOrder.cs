using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("purchaseorder")]    
    public class PurchaseOrder
    {
        public enum status
        {
            初始狀態=99,
            已成立=0,
            取消=1
        }
        [Key]
        public string Code { get; set; }
        /// <summary>
        /// =cartID
        /// </summary>
        public string SalesorderCode { get; set; }
        public Nullable<int> PurchaseorderGroupID { get; set; }
        public string IDNO { get; set; }
        public string Name { get; set; }
        public int AccountID { get; set; }
        public string TelDay { get; set; }
        public string TelNight { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<System.DateTime> StarvlDate { get; set; }
        public string CardHolder { get; set; }
        public string CardTelDay { get; set; }
        public string CardTelNight { get; set; }
        public string CardMobile { get; set; }
        public string CardLOC { get; set; }
        public string CardZip { get; set; }
        public string CardADDR { get; set; }
        public string CardNO { get; set; }
        public string CardNOCHK { get; set; }
        public string CardType { get; set; }
        public string CardBank { get; set; }
        public string CardExpire { get; set; }
        public Nullable<System.DateTime> CardBirthday { get; set; }
        public string InvoReceiver { get; set; }
        public string Invoid { get; set; }
        public string InvoTitle { get; set; }
        public string InvoLOC { get; set; }
        public string InvoZip { get; set; }
        public string InvoADDR { get; set; }
        public string RecvName { get; set; }
        public string RecvENGName { get; set; }
        public string RecvTelDay { get; set; }
        public string RecvTelNight { get; set; }
        public string RecvMobile { get; set; }
        public Nullable<int> DelivType { get; set; }
        public string DelivData { get; set; }
        public string DelivLOC { get; set; }
        public string DelivZip { get; set; }
        public string DelivADDR { get; set; }
        public string DelivENGADDR { get; set; }
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
        public string CoserverName { get; set; }
        public string ServerName { get; set; }
        public string ActCode { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public string RemoteIP { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Note { get; set; }
        public string Note2 { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

    }
}