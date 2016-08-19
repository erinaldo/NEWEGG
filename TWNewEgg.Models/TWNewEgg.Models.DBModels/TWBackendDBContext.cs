using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels
{
    public class TWBackendDBContext : DbContext
    {
        public TWBackendDBContext(string connectionString)
            : base(connectionString)
        {
            
        }

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB> SellerCorrectionPrice { get; set; }

        // 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160502
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB> Discard4Item { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Cart> Cart { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Process> Process { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.PurchaseOrder> PurchaseOrder { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.PurchaseOrderitemTWBACK> PurchaseOrderitemTWBACK { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.refund2c> refund2c { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Retgood> Retgood { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Datamaintain_log> Datamaintain_log { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.AccountsDocumentType> AccountsDocumentType { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.BankAccounts> BankAccounts { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.ChartOfAccountsProfile> ChartOfAccountsProfile { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.CreditAuth> CreditAuth { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.DeliverType> DeliverType { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.DocNumber_V2> DocNumber_V2 { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.FinanceDocumentCreateNote> FinanceDocumentCreateNote { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.GLAccounts> GLAccounts { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.InvoiceList> InvoiceList { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Sap_BapiAccDocument_DocHeader> Sap_BapiAccDocument_DocHeader { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Sap_BapiAccDocument_DocDetail> Sap_BapiAccDocument_DocDetail { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Seller_FinanMaster> Seller_FinanMaster { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.Seller_FinanDetail> Seller_FinanDetail { get; set; }

        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.ItemInStock_trans> ItemInStock_trans { get; set; }
        public System.Data.Entity.DbSet<TWNewEgg.Models.DBModels.TWBACKENDDB.FinDocTransLog> FinDocTransLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWBACKENDDB.BankAccounts>().HasKey(t => new { t.BankID, t.AccNumber });
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWBACKENDDB.ChartOfAccountsProfile>().HasKey(t => new { t.AccDocTypeCode, t.DeliverTypeCode, t.Seq });
            modelBuilder.Entity<TWNewEgg.Models.DBModels.TWBACKENDDB.FinanceDocumentCreateNote>().HasKey(t => new { t.SalesOrderCode, t.AccDocTypeCode });
        }
    }
}
