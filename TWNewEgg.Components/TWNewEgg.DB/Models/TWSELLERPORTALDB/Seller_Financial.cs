using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_Financial")]
    public class Seller_Financial
    {
        [Key]
        //[ForeignKey("SellerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }
        public string SWIFTCode { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankBranchName { get; set; }
        public string BankBranchCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankState { get; set; }
        public string BankCountryCode { get; set; }
        public string BankZipCode { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string BeneficiaryCity { get; set; }
        public string BeneficiaryState { get; set; }
        public string BeneficiaryCountryCode { get; set; }
        public string BeneficiaryZipcode { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
