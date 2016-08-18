using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class SOGroupPaymentFailureMailData
    {
        public SOGroupPaymentFailureMailData()
        {
            SalesOrdersList = new List<SalesOrder>();
            SalesOrderItemList = new List<SalesOrderItem>();
            PurchaseOrderList = new List<PurchaseOrderTWSQLDB>();
            MailDataDetailList = new List<MailDataDetail>();
            ProductList = new List<TWNewEgg.Models.DBModels.TWSQLDB.Product>();
            PayType = new TWNewEgg.Models.DBModels.TWSQLDB.PayType();
            Bank = new TWNewEgg.Models.DBModels.TWSQLDB.Bank();
        }

        public string Environment { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PayTypeString { get; set; }
        public int PayTypeID { get; set; }
        public int SOGroupID { get; set; }
        public string Status { get; set; }
        public string payFail { get; set; }
        public string rtnCode { get; set; }
        public string rtnMsg { get; set; }
        public List<SalesOrder> SalesOrdersList { get; set; }
        public List<SalesOrderItem> SalesOrderItemList { get; set; }
        public List<PurchaseOrderTWSQLDB> PurchaseOrderList { get; set; }
        public List<MailDataDetail> MailDataDetailList { get; set; }
        public List<TWNewEgg.Models.DBModels.TWSQLDB.Product> ProductList { get; set; }
        public TWNewEgg.Models.DBModels.TWSQLDB.PayType PayType { get; set; }
        public TWNewEgg.Models.DBModels.TWSQLDB.Bank Bank { get; set; }
    }

    public class MailDataDetail
    {
        public string SalesOrderCode { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string ItemID { get; set; }
        public string ProductID { get; set; }
        public string SellerProductID { get; set; }
        public string ItemName { get; set; }
        public string DelvType { get; set; }
    }
}
