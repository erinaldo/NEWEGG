using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("refund2c")]
    public class refund2c
    {
        public enum InvoiceResult_Status
        {
            未開立發票 = 0,
            已配號 = 1,
            已列印 = 2,
            發票作廢 = 3,
            發票折讓 = 4,
            發票退回 = 5
        }
        public enum status : short
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

            退款尚未批示 = 98,
            進入退款程序 = 99
        }

        public refund2c()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string ReFund2cUser { get; set; }
        public DateTime? PayDate { get; set; }
        public int? PostAge { get; set; }
        public int? Amount { get; set; }
        public int? AmountNOTax { get; set; }
        public int? Amountreal { get; set; }
        public DateTime? FinalDate { get; set; }
        public int? Status { get; set; }
        public int? Cause { get; set; }
        public int? RefundType { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string SubBankName { get; set; }
        public string AccountNO { get; set; }
        public string AccountName { get; set; }
        public string CauseNote { get; set; }
        public DateTime? InvoiceConfirm { get; set; }
        public string ConfirmUser { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string ApplyUser { get; set; }
        public DateTime? ApplyDate { get; set; }
        public DateTime? PreFundDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string MailNote { get; set; }
        public string ProcessID { get; set; }
        public int? RetgoodID { get; set; }
        public string ApproveUser { get; set; }
        public int? InvoiceResult { get; set; }
        public string RecvUser { get; set; }
        public DateTime? DiscountDate { get; set; }
        public string DiscountUser { get; set; }
        public string Finisher { get; set; }
        public string InvoiceNO { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? InvoicePrice { get; set; }
        public DateTime? Refunding { get; set; }
        public int? FailCause { get; set; }
        public string Refund2cFile { get; set; }
        public int? ADJPrice { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string CartID { get; set; }
        public string Note { get; set; }
        public string UpdateNote { get; set; }
        public DateTime? OnRefundDate { get; set; }
        public DateTime? AbnRefundDate { get; set; }
        public DateTime? CancelRefundDate { get; set; }
        public string ABNRefundReason { get; set; }
        public string CreateUser { get; set; }//2014.03.10 penny
    }
}
