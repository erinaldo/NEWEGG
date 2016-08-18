using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.FinanceRepoAdapters.Interface
{
    public interface ISellerFinanceRepoAdapter
    {
        void SaveFinanDetail(Seller_FinanDetail finanDetail);

        Seller_FinanDetail GetFinanDetail(string orderID);

        bool IsExistFinanDetail(string orderID);

        string GetOrderVendorID(int storeID);

        Seller GetSellerVendor(int storeID);

        Seller GetSellerVendorForPurchaseOrder(int sellerID);

        IQueryable<ItemInStock_trans> GetItemInStockTrans(AccountsDocumentType.DocTypeEnum docType, string codeNumber);
    }
}
