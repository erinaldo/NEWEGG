using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
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

        public refund2c()
        {
        }
        public refund2c(int status, string Refund2cCode, Models.ExtModels.ReturnDetail rd)
        {
            this.Code = Refund2cCode;
            this.PayDate = rd.Cart_CreateDate;
            //this.BankID = rd.Retgood_BankName;
            this.BankID = rd.Retgood_BankBranch;
            this.Amount = (int)rd.Retgood_Price;
            this.Status = status;
            this.SubBankName = rd.Retgood_BankBranch;
            this.RetgoodID = rd.Retgood_ID;
            this.InvoiceNO = rd.Cart_InvoiceNO;
            this.CreateDate = DateTime.UtcNow.AddHours(8);
            this.BankName = rd.Retgood_BankName;
            this.Date = DateTime.UtcNow.AddHours(8);
            this.AccountName = rd.Cart_Username;
            this.AccountNO = rd.Retgood_AccountNO;
            this.ProcessID = rd.Process_ID;
            this.RetgoodID = rd.Retgood_ID;
            this.CartID = rd.Cart_ID;

            if (rd.Cart_InvoiceNO == null || rd.Cart_InvoiceNO == "")
            {
                this.InvoiceResult = (int)InvoiceResult_Status.未開立發票;
            }
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

        public enum reason :short
        {
            規格不合 = 1,
            與想像不符 = 2,
            其他原因 = 3,
            [Description("商品規格不符")]商品規格不符 = 4,
            [Description("不想等/等太久")]不想等等太久= 5,
            [Description("價格比別家貴")]價格比別家貴 = 6,
            [Description("重複購買")]重複購買 = 7,
            [Description("我要改買其他商品")]我要改買其他商品 = 8,
            [Description("接收時間無法配合")]接收時間無法配合 = 9,
            [Description("衝動購買")]衝動購買 = 10,
            [Description("我要改用其他付款方式")]我要改用其他付款方式 = 11,
            [Description("其他")]其他= 12,
            [Description("廠商通知缺貨")]廠商通知缺貨= 13,
        }
    }
}