using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("purchaseorder")]
    public partial class PurchaseOrder
    {
        public enum status
        {
            未拋單 = 99,
            正常 = 0,
            取消 = 1,
            未審核 = 2,
            已收貨 = 4,
            完成 = 7
        };
        public enum deliverystatus
        {
            未結案 = 0,
            完成進貨 = 1,
            作廢 = 2,
            採購異常 = 3,
            初始狀態 = 999
        };
        public enum tradestatus
        {
            待進貨 = 0,
            完成進貨 = 1,
            已成立 = 6,
            待出貨 = 7,
            已出貨 = 8

        }
        public enum accountstatus
        {
            未結案 = 0,
            完成進貨 = 1,
            作廢 = 2,
            已結案 = 3

        };
        public enum forworderstatus
        {
            FedEx = 1,
            SPEX = 2,

            Morrison = 3

        };
        public enum dimcurrency
        {
            cm = 0,
            inch = 1


        }
        public enum boxcurrency
        {
            kg = 0,
            lb = 1


        }
        public enum delivtype
        {

            間配 = 1,
            直配 = 2,
            三角 = 3,
            切貨 = 0
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public string Code { get; set; }
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
        public string InvoID { get; set; }
        public string InvoTitle { get; set; }
        public string InvoLOC { get; set; }
        public string InvoZip { get; set; }
        public string InvoADDR { get; set; }
        public string RecvName { get; set; }
        public string RecvENGName { get; set; }
        public string RecvTelDay { get; set; }
        public string RecvTelNight { get; set; }
        public string RecvMobile { get; set; }

        public Nullable<int> DELIVType { get; set; }
        public string DELIVData { get; set; }
        public string DELIVLOC { get; set; }
        public string DELIVZip { get; set; }
        public string DELIVADDR { get; set; }
        public string DelivENGADDR { get; set; }
        public string DELIVHitNote { get; set; }
        public string DELIVNO { get; set; }
        public string ForwardNO { get; set; }
        public int? InventoryStatus { get; set; }
        public DateTime? InventoryStatusDate { get; set; }
        public string InventoryStatusUser { get; set; }
        public Nullable<int> AccountStatus { get; set; }
        public string AccountNO { get; set; }
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
        public string COServerName { get; set; }
        public string ServerName { get; set; }
        public string ACTCode { get; set; }
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
        public Nullable<int> DelvStatus { get; set; }
        public Nullable<System.DateTime> DelvStatusdate { get; set; }
        public Nullable<int> Forwarder { get; set; }
        public int ASNNumber { get; set; }
    }
}
