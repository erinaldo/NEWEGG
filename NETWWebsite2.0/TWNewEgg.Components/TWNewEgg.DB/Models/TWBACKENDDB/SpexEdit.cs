using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("spexedit")]
    public class SpexEdit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SO { get; set; }
        public string SellerProductID { get; set; }
        public string PurchaseOrderSalesOrdercode { get; set; }
        public string SalesDEP { get; set; }
        public string AccountNumber { get; set; }
        public string SimpleName { get; set; }
        public string Sales { get; set; }
        public string CompanyNameSent { get; set; }
        public string ContactPersonSent { get; set; }
        public string Address1Sent { get; set; }
        public string Address2Sent { get; set; }
        public string State { get; set; }
        public string SentZipCode { get; set; }
        public string SentCityName { get; set; }
        public string SentPhoneNO { get; set; }
        public string ReceiveCompanyName { get; set; }
        public string ReceiveContactPerson { get; set; }
        public string ReceiveAddress1 { get; set; }
        public string ReceiveAddress2 { get; set; }
        public string ConsigneeState { get; set; }
        public string ReceiveZipCode { get; set; }
        public string CountryName { get; set; }
        public string ReceiveCityName { get; set; }
        public string ReceivePhoneNO { get; set; }
        public Nullable<int> Pieces { get; set; }
        public string PackageType { get; set; }
        public Nullable<decimal> DimWeight { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<int> InvoiceValue { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> FuelSurcharge { get; set; }
        public Nullable<int> Flag { get; set; }
        public string FileName { get; set; }
        public Nullable<int> DELIVType { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int NO { get; set; }
        public Nullable<decimal> Long { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> High { get; set; }
        public string ForwardNo { get; set; }
    }
}