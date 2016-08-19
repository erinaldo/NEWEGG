using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class SapDocumentInfo
    {
        public SapDocumentInfo()
        {
            DocDetail = new List<Sap_BapiAccDocument_DocDetail>();
            SellerFinanDetail = new List<Seller_FinanDetail>();
        }

        public AccountsDocumentType.DocTypeEnum DocType { get; set; }
        public int AccDocTypeCode { get; set; }

        /// <summary>
        /// 購物車Cart List
        /// </summary>
        public List<Cart> SalesOrderList { get; set; }

        /// <summary>
        /// 採購單List
        /// </summary>
        public List<PurchaseOrderitemTWBACK> PurchaseOrderItemList { get; set; }

        public Sap_BapiAccDocument_DocHeader DocHeader { get; set; }
        public List<Sap_BapiAccDocument_DocDetail> DocDetail { get; set; }
        public List<Seller_FinanDetail> SellerFinanDetail { get; set; }
    }
}
