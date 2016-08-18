using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Seller
{
    public class SellerBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string EngName { get; set; }
        public string Description { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string CurrencyType { get; set; }
        public string TableName { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string AccountType { get; set; }
        public string CompanyCode { get; set; }
        public string ACCT_GROUP { get; set; }
        public string PUR_ORG { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Region { get; set; }
        public string Sortl { get; set; }
        public string Address { get; set; }
        public string TELF1 { get; set; }
        public string TELF2 { get; set; }
        public string TELFX { get; set; }
        public string LANGUAGE { get; set; }
        public string VAT_NO { get; set; }
        public string ZTERM { get; set; }
        public string RECON_ACCT { get; set; }
        public string ACTION_CODE { get; set; }
        public int? Istosap { get; set; }
        public string AboutInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string ComAdd { get; set; }
        public string Status { get; set; }
        //  Country
        public string CountryName { get; set; }
        public string CountryNameCHT { get; set; }
        public string UsageCurrency { get; set; }

    }
}
