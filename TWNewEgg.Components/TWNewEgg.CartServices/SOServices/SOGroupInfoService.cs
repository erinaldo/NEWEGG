using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.SOServices
{
    public class SOGroupInfoService : ISOGroupInfoService
    {
        private IDBSOInfoRepoAdapter _dbSoInfoRepoAdapter;
        private ISORepoAdapter _soRepoAdapter;
        private IPurchaseOrderRepoAdapter _purchaseOrderRepoAdapter;
        private IPayTypeRepoAdapter _payTypeRepoAdapter;
        private IBankRepoAdapter _bankRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;

        public SOGroupInfoService(
            ISORepoAdapter soRepoAdapter, 
            IDBSOInfoRepoAdapter dbSoInfoRepoAdapter, 
            IPurchaseOrderRepoAdapter purchaseOrderRepoAdapter,
            IPayTypeRepoAdapter payTypeRepoAdapter, 
            IBankRepoAdapter bankRepoAdapter,
            IProductRepoAdapter productRepoAdapter)
        {
            this._soRepoAdapter = soRepoAdapter;
            this._dbSoInfoRepoAdapter = dbSoInfoRepoAdapter;
            this._purchaseOrderRepoAdapter = purchaseOrderRepoAdapter;
            this._payTypeRepoAdapter = payTypeRepoAdapter;
            this._bankRepoAdapter = bankRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
        }

        public SOGroupInfo GetSOGroupInfo(int soGroupId)
        {
            SOGroupInfo groupInfo = new SOGroupInfo();

            SalesOrderGroup soGroup = this._soRepoAdapter.GetSOGroup(soGroupId);
            SOGroupBase soGroupBase = ModelConverter.ConvertTo<SOGroupBase>(soGroup);
            List<SalesOrder> SOs = this._soRepoAdapter.GetSOs(soGroupId).ToList();
            List<SOBase> soBases = ModelConverter.ConvertTo<List<SOBase>>(SOs);

            groupInfo.Main = soGroupBase;
            foreach (SOBase soBase in soBases)
            {
                List<SalesOrderItem> soItems = this._soRepoAdapter.GetSOItems(soBase.Code).ToList();
                List<SOItemBase> soItemBases = ModelConverter.ConvertTo<List<SOItemBase>>(soItems);
                groupInfo.SalesOrders.Add(new SOInfo()
                {
                    Main = soBase,
                    SOItems = soItemBases
                });
            }

            DbSOInfo dbSoInfo = _dbSoInfoRepoAdapter.GetDBSOInfo(SOs[0].Code);
            switch(dbSoInfo.Status){
                case DbSOInfo.SOInfoStatus.初始:
                    groupInfo.Status = SOGroupInfo.SOGroupStatus.Initial;
                    break;
                case DbSOInfo.SOInfoStatus.未付款:
                    groupInfo.Status = SOGroupInfo.SOGroupStatus.NotPayed;
                    break;
                case DbSOInfo.SOInfoStatus.已付款:
                    groupInfo.Status = SOGroupInfo.SOGroupStatus.Payed;
                    break;
                case DbSOInfo.SOInfoStatus.失敗:
                    groupInfo.Status = SOGroupInfo.SOGroupStatus.Failed;
                    break;
                case DbSOInfo.SOInfoStatus.訂單成立:
                case DbSOInfo.SOInfoStatus.採購單確認中:
                case DbSOInfo.SOInfoStatus.待出貨:
                case DbSOInfo.SOInfoStatus.已退貨:
                case DbSOInfo.SOInfoStatus.已退款:
                case DbSOInfo.SOInfoStatus.空運中:
                case DbSOInfo.SOInfoStatus.海外轉運中:
                case DbSOInfo.SOInfoStatus.配送中:
                case DbSOInfo.SOInfoStatus.清關中:
                case DbSOInfo.SOInfoStatus.已送達:
                case DbSOInfo.SOInfoStatus.退貨中:
                case DbSOInfo.SOInfoStatus.退款中:
                    groupInfo.Status = SOGroupInfo.SOGroupStatus.Completed;
                    break;
            }

            return groupInfo;
        }

        public SOGroupPaymentFailureMailData GetSOGroupPaymentFailureMailData(int soGroupId)
        {
            SOGroupPaymentFailureMailData SOGroupPaymentFailureMailData = new SOGroupPaymentFailureMailData();

            List<SalesOrder> SalesOrderList = this._soRepoAdapter.GetSOs(soGroupId).ToList();
            List<string> SalesOrderIDList = SalesOrderList.Select(x => x.Code).ToList();
            List<SalesOrderItem> SalesOrderItemList = this._soRepoAdapter.GetSOItemsByCodeList(SalesOrderIDList).ToList();
            List<int> ProductIDList = SalesOrderItemList.Select(x => x.ProductID).ToList();
            List<Product> ProductList = this._productRepoAdapter.GetListAllByProductID(ProductIDList).ToList();
            List<PurchaseOrderTWSQLDB> PurchaseOrderTWSQLDBList = this._purchaseOrderRepoAdapter.GetPurchaseOrder(SalesOrderIDList).ToList();

            if (SalesOrderList != null && SalesOrderList.Count > 0) {

                TWNewEgg.Models.DBModels.TWSQLDB.Bank Bank = this._bankRepoAdapter.GetAll().ToList().Where(x => x.Code == SalesOrderList.FirstOrDefault().CardBank).FirstOrDefault();
                TWNewEgg.Models.DBModels.TWSQLDB.PayType PayType = this._payTypeRepoAdapter.GetPayTypeByPayType0rateNumandBankID(SalesOrderList.FirstOrDefault().PayType, Bank.ID);

                SOGroupPaymentFailureMailData.SalesOrdersList = SalesOrderList;
                SOGroupPaymentFailureMailData.SalesOrderItemList = SalesOrderItemList;
                SOGroupPaymentFailureMailData.PurchaseOrderList = PurchaseOrderTWSQLDBList;
                SOGroupPaymentFailureMailData.Bank = Bank;
                SOGroupPaymentFailureMailData.PayType = PayType;
                SOGroupPaymentFailureMailData.Name = SalesOrderList.FirstOrDefault().Name;
                SOGroupPaymentFailureMailData.Email = SalesOrderList.FirstOrDefault().Email;
                SOGroupPaymentFailureMailData.PayTypeID = SalesOrderList.FirstOrDefault().PayType ?? 0;
                SOGroupPaymentFailureMailData.PayTypeString = Bank.Name + "(" + PayType.Name + ")";
                SOGroupPaymentFailureMailData.Status = ((SalesOrder.status)SalesOrderList.FirstOrDefault().Status.Value).ToString();
                SOGroupPaymentFailureMailData.ProductList = ProductList;
                SOGroupPaymentFailureMailData.SOGroupID = soGroupId;

                foreach (var item in SalesOrderList) {
                    MailDataDetail MailDataDetailtemp = new MailDataDetail();
                    MailDataDetailtemp.SalesOrderCode = item.Code;
                    List<SalesOrderItem> SalesOrderItemtemp = SalesOrderItemList.Where(x=>x.SalesorderCode == item.Code).ToList();
                    List<PurchaseOrderTWSQLDB> PurchaseOrderTWSQLDBtemp = PurchaseOrderTWSQLDBList.Where(x => x.SalesorderCode == item.Code).ToList();
                    if(SalesOrderItemtemp!=null && SalesOrderItemtemp.Count > 0){
                        MailDataDetailtemp.ItemID = SalesOrderItemtemp.FirstOrDefault().ItemID.ToString();
                        MailDataDetailtemp.ItemName = SalesOrderItemtemp.FirstOrDefault().Name.ToString();
                        MailDataDetailtemp.ProductID = SalesOrderItemtemp.FirstOrDefault().ProductID.ToString();
                        MailDataDetailtemp.DelvType = ((Item.tradestatus)SalesOrderList.FirstOrDefault().DelivType).ToString();
                        Product Producttemp = ProductList.Where(x=>x.ID==SalesOrderItemtemp.FirstOrDefault().ProductID).FirstOrDefault();
                        if(Producttemp!=null){
                            MailDataDetailtemp.SellerProductID = Producttemp.SellerProductID;
                        }
                    }
                    if (PurchaseOrderTWSQLDBtemp != null && PurchaseOrderTWSQLDBtemp.Count > 0) {
                        MailDataDetailtemp.PurchaseOrderCode = PurchaseOrderTWSQLDBtemp.FirstOrDefault().Code;
                    }
                    SOGroupPaymentFailureMailData.MailDataDetailList.Add(MailDataDetailtemp);
                }               
            }

            return SOGroupPaymentFailureMailData;
        }

        public void UpdateATMPayment(int soGroupId, string bankCode, string vAccount, DateTime expireDate)
        {
            List<SalesOrder> salesorders = this._soRepoAdapter.GetSOs(soGroupId).ToList();
            foreach (SalesOrder so in salesorders)
            {
                so.CardNo = vAccount;
                so.CardBank = bankCode;
                so.Expire = expireDate.AddDays(1);
                this._soRepoAdapter.UpdateSO(so);
            }
        }
        /// <summary>
        /// 判斷是否付款成功
        /// </summary>
        /// <param name="SalesOrderStatus"></param>
        /// <returns>bool true(付款成功) false(付款失敗)</returns>
        public bool Ispayed(int SalesOrderStatus)
        {
            switch (SalesOrderStatus)
            {
                //Status為0（付款成功）
                case (int)SalesOrder.status.付款成功:
                    return true;
                //Status為4（付款成功，拋單失敗） 
                case (int)SalesOrder.status.付款成功拋單失敗:
                    return true;
                case (int)SalesOrder.status.完成:
                    return true;
                //其餘Status為付款失敗
                default:
                    return false;    

            }
        }
    }
}
