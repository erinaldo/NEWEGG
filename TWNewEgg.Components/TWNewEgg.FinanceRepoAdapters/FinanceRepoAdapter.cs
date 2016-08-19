using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TWNewEgg.DAL.Interface;
using TWNewEgg.FinanceRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.Data.Objects.SqlClient;

namespace TWNewEgg.FinanceRepoAdapters
{
    public class FinanceRepoAdapter : IFinanceRepoAdapter
    {
        log4net.ILog _logger;

        IBackendRepository<InvoiceList> _invoiceListRepo;
        IBackendRepository<Cart> _cartRepo;
        IBackendRepository<PurchaseOrder> _purchaseOrderRepo;
        IBackendRepository<PurchaseOrderitemTWBACK> _purchaseOrderitemTWBACKRepo;
        IBackendRepository<Process> _processRepo;
        IBackendRepository<Retgood> _retgoodRepo;
        IBackendRepository<refund2c> _refund2cRepo;
        IBackendRepository<DocNumber_V2> _docNumberV2Repo;
        IRepository<PayType> _payTypeRepo;
        IBackendRepository<FinanceDocumentCreateNote> _finanDocCreNoteRepo;
        IBackendRepository<CreditAuth> _creditAuthRepo;
        IRepository<Auth> _authRepo;
        IRepository<Bank> _bankRepo;
        IBackendRepository<BankAccounts> _bankAccountsRepo;
        IBackendRepository<GLAccounts> _glAccounts;

        IRepository<Product> _productRepo;
        IRepository<Coupon> _couponRepo;
        IRepository<PromotionGiftRecords> _promotionGiftRecordsRepo;

        IBackendRepository<AccountsDocumentType> _accDocType;

        public FinanceRepoAdapter(IBackendRepository<InvoiceList> invoiceListRepo, IBackendRepository<Cart> cartRepo, IBackendRepository<PurchaseOrder> purchaseOrderRepo, IBackendRepository<PurchaseOrderitemTWBACK> purchaseOrderitemTWBACKRepo,
            IBackendRepository<Process> processRepo, IBackendRepository<Retgood> retgoodRepo, IBackendRepository<refund2c> refund2cRepo,
            IBackendRepository<DocNumber_V2> docNumberV2Repo, IRepository<PayType> payTypeRepo, IBackendRepository<FinanceDocumentCreateNote> finanDocCreNoteRepo,
            IBackendRepository<CreditAuth> creditAuthRepo, IRepository<Auth> authRepo, IRepository<Bank> bankRepo,
            IBackendRepository<BankAccounts> bankAccountsRepo, IBackendRepository<GLAccounts> glAccounts, IRepository<Product> productRepo,
            IRepository<Coupon> couponRepo, IRepository<PromotionGiftRecords> promotionGiftRecordsRepo, IBackendRepository<AccountsDocumentType> accDocType)
        {
            this._logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

            this._invoiceListRepo = invoiceListRepo;
            this._cartRepo = cartRepo;
            this._purchaseOrderRepo = purchaseOrderRepo;
            this._purchaseOrderitemTWBACKRepo = purchaseOrderitemTWBACKRepo;

            this._processRepo = processRepo;
            this._retgoodRepo = retgoodRepo;
            this._refund2cRepo = refund2cRepo;

            this._docNumberV2Repo = docNumberV2Repo;
            this._payTypeRepo = payTypeRepo;

            this._finanDocCreNoteRepo = finanDocCreNoteRepo;

            this._creditAuthRepo = creditAuthRepo;
            this._authRepo = authRepo;
            this._bankRepo = bankRepo;

            this._bankAccountsRepo = bankAccountsRepo;
            this._glAccounts = glAccounts;
            this._productRepo = productRepo;

            this._promotionGiftRecordsRepo = promotionGiftRecordsRepo;
            this._couponRepo = couponRepo;

            this._accDocType = accDocType;

        }

        //public List<int> PayType0rateNumList
        //{
        //    get
        //    {
        //        return new List<int> { 1, 3, 6, 10, 12, 18, 24, 110, 112, 118, 124, 201, 501 };
        //        //return System.Configuration.ConfigurationManager.AppSettings["PayType0rateNumList"].Split(',').ToList().Select(x => int.Parse(x)).ToList();
        //    }
        //}

        //public List<int> NCCCPayTypeIDList
        //{
        //    get
        //    {
        //        return new List<int> { 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229 };
        //        //return System.Configuration.ConfigurationManager.AppSettings["NCCCPayTypeCodeList"].Split(',').ToList().Select(x => int.Parse(x)).ToList();
        //    }
        //}

        public List<CartGroupInfo> GetXQData(DateTime startDate, DateTime endDate)
        {
            try
            {
                //付款日期(現為訂單日期)
                IQueryable<Cart> cartList = this._cartRepo.GetAll()
                    .Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate);

                //付款方式-信用卡
                cartList = VerifyPayment(AccountsDocumentType.DocTypeEnum.XQ, cartList);
                
                //取得XQ已處理的項目
                IQueryable<int> docTypeList = this._accDocType.GetAll().Where(x => x.DocType == "XQ").Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.ReprocessingFlag == "0")
                    .GroupBy(x => x.SalesOrderGroupID).Select(g => new
                    {
                        SalesOrderGroupID = g.Key
                    });


                //not exists 取得未產生會計文件的購物車List
                IQueryable<CartGroupInfo> cartGrpList = from cart in cartList
                                                            .GroupBy(x => x.SalesorderGroupID ?? 0)
                                                            .Select(g => new CartGroupInfo
                                                        {
                                                            SalesOrderGroupID = g.Key,
                                                            DocDate = g.Min(x => x.CreateDate.Value),
                                                            SalesOrderList = g,
                                                            DataType = CartGroupInfo.DataTypeEnum.XQ
                                                        })
                                                        where !noteList.Any(x => x.SalesOrderGroupID == cart.SalesOrderGroupID)
                                                        select cart;

                return cartGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CartGroupInfo> GetXDData_Offline(DateTime startDate, DateTime endDate)
        {
            try
            {
                //ATM為付款完成日(現為AUTH的產生日期)，資料重覆時取最小建立日期
                var authList = this._authRepo.GetAll()
                    .Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate && x.SuccessFlag == "1")
                    .GroupBy(x => x.SalesOrderGroupID)
                    .Select(g => new
                    {
                        SalesOrderGroupID = g.Key,
                        CreateDate = g.Min(x => x.CreateDate)
                    }).ToList();

                //List<int> payTypeList = new List<int>() { 31, 32 };

                //付款方式-ATM
                IQueryable<Cart> cartList = this._cartRepo.GetAll().Where(x => !PayOnDeliveryPayTypeList.Contains(x.PayType ?? 0));
                cartList = VerifyPayment(AccountsDocumentType.DocTypeEnum.XD, cartList);

                //以SalesorderGroupID作分組
                IEnumerable<CartGroupInfo> cartGrpList = (from auth in authList
                                                          join cart in cartList
                                                          on auth.SalesOrderGroupID equals cart.SalesorderGroupID ?? 0
                                                          group new { auth, cart } by new { cart.SalesorderGroupID } into grp
                                                          select new CartGroupInfo
                                                          {
                                                              SalesOrderGroupID = grp.Key.SalesorderGroupID.Value,
                                                              DocDate = grp.FirstOrDefault().auth.CreateDate,
                                                              SalesOrderList = grp.Select(x => x.cart),
                                                              DataType = CartGroupInfo.DataTypeEnum.XD_Offline
                                                          });
                
                //取得XD已處理的項目
                IQueryable<int> docTypeList = this._accDocType.GetAll().Where(x => x.DocType == "XD" && x.Code != AccountsDocumentType.PayOnDeliveryCode).Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.ReprocessingFlag == "0")
                    .GroupBy(x => x.SalesOrderGroupID).Select(g => new
                    {
                        SalesOrderGroupID = g.Key
                    });

                //not exists 取得未產生會計文件的購物車List
                cartGrpList = from cart in cartGrpList
                              where !noteList.Any(x => x.SalesOrderGroupID == cart.SalesOrderGroupID)
                              select cart;

                return cartGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<int> PayOnDeliveryPayTypeList
        {
            get
            {
                return new List<int>() { 31, 32 };
            }
        }

        public List<CartGroupInfo> GetXDData_PayOnDelivery(DateTime startDate, DateTime endDate)
        {
            try
            {
                //List<int> payTypeList = new List<int>() { 31, 32 };

                //貨到付款為配達日且狀態為已配達
                IQueryable<Cart> cartList = this._cartRepo.GetAll()
                    .Where(x => PayOnDeliveryPayTypeList.Contains(x.PayType ?? 0) && x.DelvStatusDate.Value >= startDate && x.DelvStatusDate.Value <= endDate);
                    
                //物流為新竹貨運(801, 802)或黑貓(803, 804)，並以SalesorderGroupID作分組取得資料
                List<int> delivList = new List<int>() { 801, 802, 803, 804 };
                IQueryable<CartGroupInfo> cartGrpList = from cart in cartList
                                                        join proc in this._processRepo.GetAll().Where(x => delivList.Contains(x.Deliver.Value))
                                                        on cart.ID equals proc.CartID
                                                        group cart by cart.SalesorderGroupID into grp
                                                        select new CartGroupInfo
                                                        {
                                                            SalesOrderGroupID = grp.Key.Value,
                                                            DocDate = grp.Min(x => x.DelvStatusDate.Value),
                                                            //SalesOrder = grp.OrderBy(x => x.ID).FirstOrDefault(),
                                                            SalesOrderList = grp,
                                                            DataType = CartGroupInfo.DataTypeEnum.XD_PayOnDelivery
                                                        };

                //取得XD已處理的項目
                IQueryable<int> docTypeList = this._accDocType.GetAll().Where(x => x.DocType == "XD" && x.Code == AccountsDocumentType.PayOnDeliveryCode).Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.ReprocessingFlag == "0")
                    .GroupBy(x => x.SalesOrderGroupID).Select(g => new
                    {
                        SalesOrderGroupID = g.Key
                    });

                //not exists 取得未產生會計文件的購物車List
                cartGrpList = from cart in cartGrpList
                              where !noteList.Any(x=>x.SalesOrderGroupID == cart.SalesOrderGroupID)
                              select cart;
                
                return cartGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CartGroupInfo> GetXIData(DateTime startDate, DateTime endDate)
        {
            try
            {
                //發票日期(避免發票重覆開立)
                var invoiceList = this._invoiceListRepo.GetAll()
                    .Where(x => x.InDate >= startDate && x.InDate <= endDate)
                    .GroupBy(x => x.SONumber)
                    .Select(g => new
                    {
                        SONumber = g.Key,
                        InDate = g.Max(x => x.InDate),
                    });

                //找出已開立發票的訂單
                var cartList = from cart in this._cartRepo.GetAll()
                               join invoice in invoiceList
                               on cart.ID equals invoice.SONumber
                               select new
                               {
                                   SalesOrderGroupID = cart.SalesorderGroupID ?? 0,
                                   DocDate = invoice.InDate,
                                   SalesOrder = cart,
                                   DataType = CartGroupInfo.DataTypeEnum.XI
                               };
                                
                //取得XI已處理的項目
                IQueryable<int> docTypeList = this._accDocType.GetAll().Where(x => x.DocType == "XI" && x.Code != AccountsDocumentType.OverSeaBuyOutUSDCode).Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.ReprocessingFlag == "0")
                    .GroupBy(g => new { g.SalesOrderCode, g.SalesOrderGroupID })
                    .Select(g => new
                    {
                        g.Key.SalesOrderCode,
                        g.Key.SalesOrderGroupID
                    });

                //not exists 取得未產生會計文件的購物車List
                IEnumerable<CartGroupInfo> cartGrpList = from cart in cartList
                                                         where !noteList.Any(x => x.SalesOrderGroupID == cart.SalesOrderGroupID && x.SalesOrderCode == cart.SalesOrder.ID)
                                                         group cart by cart.SalesOrderGroupID into grp
                                                         select new CartGroupInfo
                                                         {
                                                             SalesOrderGroupID = grp.Key,
                                                             DocDate = grp.FirstOrDefault().DocDate,
                                                             SalesOrderList = grp.Select(x => x.SalesOrder),
                                                             DataType = grp.FirstOrDefault().DataType
                                                         };


                return cartGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 海外切貨XI-美金
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<PurchaseOrderItemGroupInfo> GetXIData_OverSeaBuyOutUSD(DateTime startDate, DateTime endDate)
        {
            try
            {
                //取得XI已處理的項目
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => x.AccDocTypeCode == 32 && x.ReprocessingFlag == "0")
                    .Select(g => new
                    {
                        g.SalesOrderCode
                    });

                var poList = from po in this._purchaseOrderRepo.GetAll()
                                 .Where(x => x.DELIVType == 6 && x.DelvStatus == 1 && x.DelvStatusdate >= startDate && x.DelvStatusdate <= endDate)
                             join poItem in this._purchaseOrderitemTWBACKRepo.GetAll()
                             on po.Code equals poItem.PurchaseorderCode
                             select new
                             {
                                 Code = po.Code,
                                 DocDate = po.DelvStatusdate ?? DateTime.MinValue,
                                 Detail = poItem
                             };

                IEnumerable<PurchaseOrderItemGroupInfo> poGrpList = from po in poList
                                                                    where !noteList.Any(x=>x.SalesOrderCode == po.Detail.SellerorderCode)
                                                                    group po by po.Detail.SellerorderCode into grp
                                                                    select new PurchaseOrderItemGroupInfo
                                                                    {
                                                                        SalesOrderGroupID = grp.Key,
                                                                        DocDate = grp.FirstOrDefault().DocDate,
                                                                        PurchaseOrderItemList = grp.Select(x => x.Detail).AsEnumerable()
                                                                    };


                return poGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CartGroupInfo> GetXIRMAData(DateTime startDate, DateTime endDate)
        {
            try
            {              
                //退貨狀態(先退貨再退款)，XIRMA只管退貨
                List<int> statusList = new List<int>() { 2, 99 };

                //退貨完成日
                IQueryable<Retgood> retgoodList = this._retgoodRepo.GetAll()
                    .Where(x => x.FinReturnDate.Value >= startDate && x.FinReturnDate.Value <= endDate && statusList.Contains(x.Status ?? -1));

                //找出已退貨的訂單
                var cartList = from cart in this._cartRepo.GetAll()
                               join retgood in retgoodList
                               on cart.ID equals retgood.CartID
                               select new
                               {
                                   SalesOrderGroupID = cart.SalesorderGroupID ?? 0,
                                   DocDate = retgood.FinReturnDate.Value,
                                   SalesOrder = cart,
                                   DataType = CartGroupInfo.DataTypeEnum.XIRMA
                               };

                //取得XIRMA已處理的項目
                IQueryable<int> docTypeList = this._accDocType.GetAll().Where(x => x.DocType == "XIRMA").Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.ReprocessingFlag == "0")
                    .GroupBy(g => new { g.SalesOrderCode, g.SalesOrderGroupID })
                    .Select(g => new
                    {
                        g.Key.SalesOrderCode,
                        g.Key.SalesOrderGroupID
                    });

                //not exists 取得未產生會計文件的購物車List
                IQueryable<CartGroupInfo> cartGrpList = from cart in cartList
                                                        where !noteList.Any(x => x.SalesOrderGroupID == cart.SalesOrderGroupID && x.SalesOrderCode == cart.SalesOrder.ID)
                                                        group cart by cart.SalesOrderGroupID into grp
                                                        select new CartGroupInfo
                                                        {
                                                            SalesOrderGroupID = grp.Key,
                                                            DocDate = grp.FirstOrDefault().DocDate,
                                                            SalesOrderList = grp.Select(x => x.SalesOrder),
                                                            DataType = grp.FirstOrDefault().DataType
                                                        };

                return cartGrpList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FinanceDocumentCreateNote GetDocCreateNote(string cartID, int intAccDocTypeCode)
        {
            return this._finanDocCreNoteRepo.GetAll().Where(x => x.SalesOrderCode == cartID && x.AccDocTypeCode == intAccDocTypeCode).FirstOrDefault();
        }

        public long GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum docType, DateTime nowDate)
        {
            long longCurrentNumber = 0;

            using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }))
            {
                try
                {
                    string strDocType = docType.ToString();

                    DocNumber_V2 info = this._docNumberV2Repo
                        .GetAll().Where(x => x.DocType == strDocType && x.StartUsingDate <= nowDate && x.EndUsingDate > nowDate).FirstOrDefault();

                    //DocNumber_V2未設定區間編號
                    if (info == null)
                        throw new Exception(string.Format("DocNumber_V2取無{0}的CurrentNumber！", strDocType));

                    //DocNumber_V2目前可用編號
                    longCurrentNumber = long.Parse(info.StartNumber) + info.CurrentNumber;

                    //檢查 CurrentNumber 是否大於 EndNumber
                    if (longCurrentNumber > long.Parse(info.EndNumber))
                        throw new Exception(string.Format("DocNumber_V2取無{0}可使用的CurrentNumber！", strDocType));

                    //更新目前可用編號
                    info.CurrentNumber += 1;

                    info.Updated = info.Updated.GetValueOrDefault() + 1;
                    info.UpdateDate = DateTime.UtcNow.AddHours(8);

                    this._docNumberV2Repo.Update(info);
                    trans.Complete();

                    return longCurrentNumber;
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    throw ex;
                }
            }
        }

        public CreditAuth GetCreditAuth(int salesOrderGroupID)
        {
            try
            {
                IQueryable<CreditAuth> list = from auth in this._creditAuthRepo.GetAll()
                                              where auth.SuccessFlag == "1" && auth.SalesOrderGroupID == salesOrderGroupID
                                              select auth;

                if (list.Count() == 0)
                    throw new Exception();
                else
                    //回傳第一筆
                    return list.OrderBy(x => x.CreateDate).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Auth GetAuth(int salesOrderGroupID)
        {
            try
            {
                IQueryable<Auth> list = from auth in this._authRepo.GetAll()
                                        where auth.SuccessFlag == "1" && auth.SalesOrderGroupID == salesOrderGroupID
                                        select auth;

                if (list.Count() == 0)
                    throw new Exception();
                else
                    //回傳第一筆
                    return list.OrderBy(x => x.CreateDate).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
                
        private IQueryable<Cart> VerifyPayment(AccountsDocumentType.DocTypeEnum docType, IQueryable<Cart> cartList)
        {
            List<int> payTypeList;
            switch (docType)
            {
                case AccountsDocumentType.DocTypeEnum.XQ:
                    //信用卡
                    payTypeList = new List<int> { 1, 3, 6, 10, 12, 18, 24, 110, 112, 118, 124, 201, 501 };
                    cartList = cartList.Where(x => payTypeList.Contains(x.PayType ?? 0));
                    break;

                case AccountsDocumentType.DocTypeEnum.XD:
                    //ATM
                    payTypeList = new List<int>() { 30, 34 };
                    cartList = cartList.Where(x => payTypeList.Contains(x.PayType ?? 0));
                    break;
            }

            return cartList;
        }
        
        public BankAccountsInfo GetBank(string bankCode, bool isAccounts = false)
        {
            BankAccountsInfo info = new BankAccountsInfo();

            //取得建立日期最新的資料 (例：809=萬泰商業銀行、凱基銀行)
            Bank bankInfo = this._bankRepo.GetAll().Where(x => x.Code == bankCode).OrderByDescending(x => x.CreateDate).FirstOrDefault();

            if (isAccounts)
            {
                var baList = this._bankAccountsRepo.GetAll().Where(x => x.BankID == bankInfo.ID);
                IQueryable<GLAccounts> accList = from gla in this._glAccounts.GetAll().Where(x => x.UseFlag == "1")
                                                 where baList.Any(x => x.AccNumber == gla.AccNumber)
                                                 select gla;

                if (accList.Count() == 0)
                    throw new Exception(string.Format("查無銀行的會計科目 ({0})。", bankCode));

                info.Accounts = accList.First();
            }

            info.Bank = bankInfo;

            return info;
        }

        public InvoiceList GetCartInvoice(string cartID)
        {
            return this._invoiceListRepo.GetAll().Where(x => x.SONumber == cartID).OrderByDescending(x => x.SN).FirstOrDefault();
        }

        public IQueryable<Process> GetCartProcess(string cartID)
        {
            //Process.Qty固定為1，數量>1時會有多筆資料
            return this._processRepo.GetAll().Where(x => x.CartID == cartID);
        }

        public string GetNewEggInvoiceNo(string cartID)
        {
            PurchaseOrderitemTWBACK poItemInfo = (from po in this._purchaseOrderRepo.GetAll().Where(x => x.SalesorderCode == cartID)
                                                  join poItem in this._purchaseOrderitemTWBACKRepo.GetAll()
                                                  on po.Code equals poItem.PurchaseorderCode
                                                  select poItem).FirstOrDefault();

            return poItemInfo.InvoiceNO;
        }

        public PurchaseOrder GetOrderPO(string cartID)
        {
            //檢查PO單是否拋單成功
            PurchaseOrderitemTWBACK poItemInfo = GetOrderPOItem(cartID);

            //取得PO單
            IQueryable<PurchaseOrder> poList = this._purchaseOrderRepo.GetAll()
                .Where(x => x.SalesorderCode == cartID);

            if (poList.Count() == 0 || poList.Count() > 1)
                throw new Exception("PurchaseOrder資料異常，待確認。");

            //return this._purchaseOrderRepo.Get(x => x.SalesorderCode == cartID && statusList.Contains(x.Status ?? -1));
            return poList.FirstOrDefault();
        }

        public PurchaseOrderitemTWBACK GetOrderPOItem(string cartID)
        {
            //List<int> statusList = new List<int>() { 0, 7 };
            
            var soInfo = this._cartRepo.Get(x => x.ID == cartID);
            if (soInfo == null)
                throw new Exception("查無訂單資料。");

            ////檢查交易模式(寄倉PO單狀態為99)
            //if (soInfo.ShipType == 9)
            //    statusList = new List<int>() { 0, 7, 99 };
            
            //取得PO單明細
            IQueryable<PurchaseOrderitemTWBACK> poItemList = from po in this._purchaseOrderRepo.GetAll().Where(x => x.SalesorderCode == cartID)
                                                             join poi in this._purchaseOrderitemTWBACKRepo.GetAll()
                                                             on po.Code equals poi.PurchaseorderCode
                                                             select poi;

            if (poItemList.Count() == 0 || poItemList.Count() > 1)
                throw new Exception("PurchaseOrderitemTWBACK資料異常，待確認。");

            PurchaseOrderitemTWBACK poItemInfo = poItemList.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(poItemInfo.SellerorderCode))
                throw new Exception("PurchaseOrderitemTWBACK.SellerorderCode不可為空值，拋單失敗。");

            return poItemInfo;
        }

        public Retgood GetRetgoodByCartID(string cartID)
        {
            return this._retgoodRepo.Get(x => x.CartID == cartID);
        }

        public IQueryable<Cart> GetCartOrders(int salesOrderGroupID)
        {
            return this._cartRepo.GetAll().Where(x => x.SalesorderGroupID == salesOrderGroupID);
        }

        public string GetSellerProductID(string cartID)
        {
            int strPrdID = this._processRepo.Get(x => x.CartID == cartID).ProductID.GetValueOrDefault();
            IQueryable<Product> list = this._productRepo.GetAll().Where(x => x.ID == strPrdID);

            if (list.Count() == 0)
                return "";
            else
                return list.FirstOrDefault().SellerProductID;
        }

        public Coupon GetCoupon(int ID)
        {
            return this._couponRepo.Get(x => x.id == ID);
        }

        public PromotionGiftRecords GetPromotionGiftRecord(string processID)
        {
            return this._promotionGiftRecordsRepo.Get(x => x.SalesOrderItemCode == processID);
        }
    }
}
