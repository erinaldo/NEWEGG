using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.DomainModels.SellerFinance
{
    public class SellerFinanceMaster_Domain
    {
        public SellerFinanceMaster_Domain()
        {
            this.InDate = DateTime.UtcNow.AddHours(8);
            this.InUserID = "System";
        }
        public string SettlementID { get; set; }
        public int SellerID { get; set; }
        public string IsOpen { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public System.DateTime SettleDate { get; set; }
        public string SettleMonth { get; set; }
        public Nullable<System.DateTime> InvoDate { get; set; }
        public string InvoNumber { get; set; }
        public Nullable<System.DateTime> RemitDate { get; set; }
        public string BaseCurrency { get; set; }
        public string SettleCurrency { get; set; }
        public decimal POPrice { get; set; }
        public decimal POTax { get; set; }
        public decimal RMAPrice { get; set; }
        public decimal RMATax { get; set; }
        public decimal WarehousePrice { get; set; }
        public decimal WarehouseTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal PaymentAmount { get; set; }
        public string FinanStatus { get; set; }

        /// <summary>
        /// 帳戶名稱
        /// </summary>
        public string BeneficiaryName { get; set; }

        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public string InUserID { get; set; }
    }
}
