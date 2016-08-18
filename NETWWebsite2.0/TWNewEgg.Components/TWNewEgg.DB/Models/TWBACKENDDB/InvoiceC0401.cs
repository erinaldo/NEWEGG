using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceC0401")]
    public class InvoiceC0401
    {
        public enum InvoiceStatus
        {
            C0401 = 1,
            C0501 = 2,
            C0701 = 3,
            D0401 = 4,
            D0501 = 5,
            CreateC0401 = 10,
            C0401ToC0501 = 12,
            C0401ToC0701 = 13,
            C0401ToD0401 = 14,
            C0501ToC0701 = 23,
            C0701ToC0401 = 31,
            D0401ToD0501 = 45,
            D0501ToD0401 = 54,
            D0401ToC0401 = 41
        }

        public string TurnkeyErrMsg()
        {
            string msg = "";
            switch (Status)
            {
                case (int)InvoiceStatus.CreateC0401:
                    msg = "發票開立錯誤";
                    break;
                case (int)InvoiceStatus.C0401ToC0501:
                    msg = "發票作廢錯誤";
                    break;
                case (int)InvoiceStatus.C0401ToC0701:
                    msg = "發票註銷錯誤";
                    break;
                case (int)InvoiceStatus.C0401ToD0401:
                    msg = "發票折讓錯誤";
                    break;
                case (int)InvoiceStatus.C0501ToC0701:
                    msg = "註銷作廢錯誤";
                    break;
                case (int)InvoiceStatus.C0701ToC0401:
                    msg = "重新開立錯誤";
                    break;
                case (int)InvoiceStatus.D0401ToD0501:
                    msg = "作廢折讓錯誤";
                    break;
                case (int)InvoiceStatus.D0501ToD0401:
                    msg = "重新折讓錯誤";
                    break;
                default:
                    throw new Exception("no such a status: " +Status);
            }
            return msg;
        }

        public int PreStatus()
        {
            int status = 0;
            switch (Status)
            {
                case (int)InvoiceStatus.CreateC0401:
                    status = (int)InvoiceStatus.CreateC0401;
                    break;
                case (int)InvoiceStatus.C0401ToC0501:
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.C0401ToC0701:
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.C0401ToD0401:
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.C0501ToC0701:
                    status = (int)InvoiceStatus.C0501;
                    break;
                case (int)InvoiceStatus.C0701ToC0401:
                    status = (int)InvoiceStatus.C0701;
                    break;
                case (int)InvoiceStatus.D0401ToD0501:
                    status = (int)InvoiceStatus.D0401;
                    break;
                case (int)InvoiceStatus.D0501ToD0401:
                    status = (int)InvoiceStatus.D0501;
                    break;
                case (int)InvoiceStatus.D0401ToC0401:
                    status = (int)InvoiceStatus.D0401;
                    break;
                default:
                    throw new Exception("no such a status: " + Status);
            }
            return status;
        }

        public int AfterStatus()
        {
            int status = 0;
            switch (Status)
            {
                case (int)InvoiceStatus.CreateC0401:
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.C0401ToC0501:
                    status = (int)InvoiceStatus.C0501;
                    break;
                case (int)InvoiceStatus.C0401ToC0701:
                    status = (int)InvoiceStatus.C0701;
                    break;
                case (int)InvoiceStatus.C0401ToD0401:
                    //發票顯示狀態，折讓處理中>折讓
                    //status = (int)InvoiceStatus.D0401;
                    //發票顯示狀態，折讓處理中>開立
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.C0501ToC0701:
                    status = (int)InvoiceStatus.C0701;
                    break;
                case (int)InvoiceStatus.C0701ToC0401:
                    status = (int)InvoiceStatus.C0401;
                    break;
                case (int)InvoiceStatus.D0401ToD0501:
                    status = (int)InvoiceStatus.D0501;
                    break;
                case (int)InvoiceStatus.D0501ToD0401:
                    status = (int)InvoiceStatus.D0401;
                    break;
                case (int)InvoiceStatus.D0401ToC0401:
                    status = (int)InvoiceStatus.C0401;
                    break;
                default:
                    throw new Exception("no such a status: " + Status);
            }
            return status;
        }

        public InvoiceC0401()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceTime { get; set; }
        public string SellerIdentifier { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public string SellerPersonInCharge { get; set; }
        public string SellerTelephoneNumber { get; set; }
        public string SellerFacsimileNumber { get; set; }
        public string SellerEmailAddress { get; set; }
        public string SellerCustomerNumber { get; set; }
        public string SellerRoleRemark { get; set; }
        public string BuyerIdentifier { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerPersonInCharge { get; set; }
        public string BuyerTelephoneNumber { get; set; }
        public string BuyerFacsimileNumber { get; set; }
        public string BuyerEmailAddress { get; set; }
        public string BuyerCustomerNumber { get; set; }
        public string BuyerRoleRemark { get; set; }
        public string CheckNumber { get; set; }
        public string BuyerRemark { get; set; }
        public string MainRemark { get; set; }
        public string CustomsClearanceMark { get; set; }
        public string Category { get; set; }
        public string RelateNumber { get; set; }
        public string InvoiceType { get; set; }
        public string GroupMark { get; set; }
        public string DonateMark { get; set; }
        public string CarrierType { get; set; }
        public string CarrierId1 { get; set; }
        public string CarrierId2 { get; set; }
        public string PrintMark { get; set; }
        public string NPOBAN { get; set; }
        public string RandomNumber { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal FreeTaxSalesAmount { get; set; }
        public decimal ZeroTaxSalesAmount { get; set; }
        public string TaxType { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal OriginalCurrencyAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Currency { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
