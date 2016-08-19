using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.FinanceRepoAdapters.Interface
{
    public interface IFinanceRepoAdapter
    {
        List<CartGroupInfo> GetXQData(DateTime startDate, DateTime endDate);

        List<CartGroupInfo> GetXDData_Offline(DateTime startDate, DateTime endDate);
        List<CartGroupInfo> GetXDData_PayOnDelivery(DateTime startDate, DateTime endDate);

        List<CartGroupInfo> GetXIData(DateTime startDate, DateTime endDate);
        List<PurchaseOrderItemGroupInfo> GetXIData_OverSeaBuyOutUSD(DateTime startDate, DateTime endDate);

        List<CartGroupInfo> GetXIRMAData(DateTime startDate, DateTime endDate);

        FinanceDocumentCreateNote GetDocCreateNote(string cartID, int intAccDocTypeCode);
        long GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum docType, DateTime nowDate);
        CreditAuth GetCreditAuth(int salesOrderGroupID);

        Auth GetAuth(int salesOrderGroupID);
        BankAccountsInfo GetBank(string bankCode, bool isAccounts = false);

        InvoiceList GetCartInvoice(string cartID);
        IQueryable<Process> GetCartProcess(string cartID);
        string GetNewEggInvoiceNo(string cartID);
        PurchaseOrder GetOrderPO(string cartID);
        Retgood GetRetgoodByCartID(string cartID);

        PurchaseOrderitemTWBACK GetOrderPOItem(string cartID);
        IQueryable<Cart> GetCartOrders(int salesOrderGroupID);

        string GetSellerProductID(string cartID);
        Coupon GetCoupon(int ID);
        PromotionGiftRecords GetPromotionGiftRecord(string processID);
    }
}
