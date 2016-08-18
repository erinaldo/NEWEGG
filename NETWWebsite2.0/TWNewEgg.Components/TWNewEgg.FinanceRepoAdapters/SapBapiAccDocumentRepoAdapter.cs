using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.FinanceRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.Data.Objects.SqlClient;

//using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.FinanceRepoAdapters
{
    public class SapBapiAccDocumentRepoAdapter : ISapBapiAccDocumentRepoAdapter
    {
        log4net.ILog _logger;
        //log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        IBackendRepository<Sap_BapiAccDocument_DocHeader> _sapDocHeaderRepo;
        IBackendRepository<Sap_BapiAccDocument_DocDetail> _sapDocDetailRepo;
        IBackendRepository<FinanceDocumentCreateNote> _finanDocCreNoteRepo;
        IBackendRepository<Cart> _cartRepo;

        ISellerFinanceRepoAdapter _sellerFinanRepoAdapter;
        IBackendRepository<AccountsDocumentType> _accDocTypeRepo;
        IRepository<Account> _accountRepo;
        IRepository<SalesOrder> _saleOrderRepo;

        IBackendRepository<FinDocTransLog> _finDocTransLogRepo;
        IFinanceRepoAdapter _finanRepoAdapter;
        IBackendRepository<Seller_FinanMaster> _sellerFinanMasterRepo;
        IBackendRepository<Seller_FinanDetail> _sellerFinanDetailRepo;

        IBackendRepository<Retgood> _retgoodRepo;

        public SapBapiAccDocumentRepoAdapter(IBackendRepository<Sap_BapiAccDocument_DocHeader> sapDocHeaderRepo, IBackendRepository<Sap_BapiAccDocument_DocDetail> sapDocDetailRepo, IBackendRepository<FinanceDocumentCreateNote> finanDocCreNoteRepo,
            IBackendRepository<Cart> cartRepo, ISellerFinanceRepoAdapter sellerFinanRepoAdapter, IBackendRepository<AccountsDocumentType> accDocTypeRepo, IRepository<Account> accountRepo,
            IRepository<SalesOrder> saleOrderRepo, IBackendRepository<FinDocTransLog> finDocTransLogRepo, IFinanceRepoAdapter finanRepoAdapter,
            IBackendRepository<Seller_FinanMaster> sellerFinanMasterRepo, IBackendRepository<Seller_FinanDetail> sellerFinanDetailRepo, IBackendRepository<Retgood> retgoodRepo)
        {
            this._logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

            this._sapDocHeaderRepo = sapDocHeaderRepo;
            this._sapDocDetailRepo = sapDocDetailRepo;
            this._finanDocCreNoteRepo = finanDocCreNoteRepo;
            this._cartRepo = cartRepo;

            this._sellerFinanRepoAdapter = sellerFinanRepoAdapter;
            this._accDocTypeRepo = accDocTypeRepo;
            this._accountRepo = accountRepo;
            this._saleOrderRepo = saleOrderRepo;

            this._finDocTransLogRepo = finDocTransLogRepo;
            this._finanRepoAdapter = finanRepoAdapter;
            this._sellerFinanMasterRepo = sellerFinanMasterRepo;
            this._sellerFinanDetailRepo = sellerFinanDetailRepo;

            this._retgoodRepo = retgoodRepo;
        }
        
        public void SaveDocument(SapDocumentInfo sapInfo)
        {
            FinanceDocumentCreateNote finanNoteInfo = null;
            string strSalesOrderCode = "";

            try
            {
                if (sapInfo.PurchaseOrderItemList == null || sapInfo.PurchaseOrderItemList.Count == 0)
                    strSalesOrderCode = sapInfo.SalesOrderList.First().ID;
                else
                    strSalesOrderCode = sapInfo.PurchaseOrderItemList.First().SellerorderCode;

                finanNoteInfo = this._finanRepoAdapter.GetDocCreateNote(strSalesOrderCode, sapInfo.AccDocTypeCode);

                //取得文件編號(不可跳號)
                if (finanNoteInfo == null)
                {
                    DocNumber_V2.DOCTypeEnum DNv2DocType;// = DocNumber_V2.DOCTypeEnum.XQ;
                    switch (sapInfo.DocType)
                    {
                        case AccountsDocumentType.DocTypeEnum.XQ:
                            DNv2DocType = DocNumber_V2.DOCTypeEnum.XQ;
                            break;
                        case AccountsDocumentType.DocTypeEnum.XD:
                            DNv2DocType = DocNumber_V2.DOCTypeEnum.XD;
                            break;
                        case AccountsDocumentType.DocTypeEnum.XI:
                        case AccountsDocumentType.DocTypeEnum.XIRMA:
                            DNv2DocType = DocNumber_V2.DOCTypeEnum.XI;
                            break;
                        default:
                            throw new Exception(string.Format("DocNumber_V2.DOCTypeEnum無此會計文件({0})類型。", sapInfo.DocType.ToString()));
                    }

                    //sapInfo.DocHeader.DOC_NUMBER = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.XQ, cartGrpInfo.DocDate).ToString();
                    sapInfo.DocHeader.DOC_NUMBER = this._finanRepoAdapter.GetDocCurrentNumber(DNv2DocType, sapInfo.DocHeader.DOC_DATE.GetValueOrDefault()).ToString();
                }
                else
                {
                    sapInfo.DocHeader.DOC_NUMBER = finanNoteInfo.DocNumber;
                }
                sapInfo.DocHeader.AC_DOC_NO = sapInfo.DocHeader.DOC_NUMBER;

                //會計文件
                this._sapDocHeaderRepo.Delete(x => x.TransactionType == sapInfo.DocHeader.TransactionType && x.TransactionID == sapInfo.DocHeader.TransactionID);                
                this._sapDocHeaderRepo.Create(sapInfo.DocHeader);

                this._sapDocDetailRepo.Delete(x => x.TransactionType == sapInfo.DocHeader.TransactionType && x.TransactionID == sapInfo.DocHeader.TransactionID);
                this._sapDocDetailRepo.CreateMany(sapInfo.DocDetail);

                //FinanceDocumentCreateNote(一個SalesorderGroupID對應多筆SalesOrderCode)
                if (sapInfo.SalesOrderList != null)
                {
                    foreach (Cart cartInfo in sapInfo.SalesOrderList)
                    {
                        //finanNoteInfo = this._finanDocCreNoteRepo.GetAll()
                        //    .Where(x => x.SalesOrderGroupID == cartInfo.SalesorderGroupID && x.SalesOrderCode == cartInfo.ID && x.AccDocTypeCode == sapInfo.AccDocTypeCode).FirstOrDefault();

                        finanNoteInfo = this._finanRepoAdapter.GetDocCreateNote(cartInfo.ID, sapInfo.AccDocTypeCode);

                        if (finanNoteInfo == null)
                        {
                            finanNoteInfo = new FinanceDocumentCreateNote();

                            finanNoteInfo.SalesOrderCode = cartInfo.ID;
                            finanNoteInfo.AccDocTypeCode = sapInfo.AccDocTypeCode;
                            finanNoteInfo.SalesOrderGroupID = cartInfo.SalesorderGroupID ?? 0;

                            finanNoteInfo.TransactionID = sapInfo.DocHeader.TransactionID;
                            finanNoteInfo.DocNumber = sapInfo.DocHeader.DOC_NUMBER;
                            //台蛋訂單
                            finanNoteInfo.SalesOrderType = 1;

                            UpdateDocCreNoteData(finanNoteInfo, sapInfo);

                            this._finanDocCreNoteRepo.Create(finanNoteInfo);
                        }
                        else
                        {
                            UpdateDocCreNoteData(finanNoteInfo, sapInfo);

                            this._finanDocCreNoteRepo.Update(finanNoteInfo);
                        }
                    }
                }

                //海外切貨XI-美金(同一筆採購單)
                if (sapInfo.PurchaseOrderItemList != null && sapInfo.PurchaseOrderItemList.Count > 0)
                {
                    PurchaseOrderitemTWBACK poItemFirstInfo = sapInfo.PurchaseOrderItemList.FirstOrDefault();

                    //finanNoteInfo = this._finanDocCreNoteRepo.GetAll()
                    //       .Where(x => x.SalesOrderCode == poItemFirstInfo.SellerorderCode && x.AccDocTypeCode == sapInfo.AccDocTypeCode).FirstOrDefault();

                    finanNoteInfo = this._finanRepoAdapter.GetDocCreateNote(poItemFirstInfo.SellerorderCode, sapInfo.AccDocTypeCode);

                    if (finanNoteInfo == null)
                    {
                        finanNoteInfo = new FinanceDocumentCreateNote();

                        finanNoteInfo.SalesOrderCode = sapInfo.PurchaseOrderItemList.FirstOrDefault().SellerorderCode;
                        finanNoteInfo.AccDocTypeCode = sapInfo.AccDocTypeCode;
                        finanNoteInfo.SalesOrderGroupID = 0; //cartInfo.SalesorderGroupID ?? 0;

                        finanNoteInfo.TransactionID = sapInfo.DocHeader.TransactionID;
                        finanNoteInfo.DocNumber = sapInfo.DocHeader.DOC_NUMBER;
                        //美蛋訂單
                        finanNoteInfo.SalesOrderType = 2;

                        UpdateDocCreNoteData(finanNoteInfo, sapInfo);

                        this._finanDocCreNoteRepo.Create(finanNoteInfo);
                    }
                    else
                    {
                        UpdateDocCreNoteData(finanNoteInfo, sapInfo);

                        this._finanDocCreNoteRepo.Update(finanNoteInfo);
                    }
                }
               
                switch (sapInfo.DocType)
                {
                    case AccountsDocumentType.DocTypeEnum.XI:
                    case AccountsDocumentType.DocTypeEnum.XIRMA:
                        //寫入對帳單
                        foreach (Seller_FinanDetail finanDetail in sapInfo.SellerFinanDetail)
                        {
                            this._sellerFinanRepoAdapter.SaveFinanDetail(finanDetail);
                        }
                        break;
                }            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateDocCreNoteData(FinanceDocumentCreateNote finanNoteInfo, SapDocumentInfo sapInfo)
        {
            finanNoteInfo.ReprocessingFlag = "0";
            finanNoteInfo.CreateDate = DateTime.UtcNow.AddHours(8);
        }

        /// <summary>
        /// 取得會計文件SAP資料
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">迄止日期</param>
        /// <param name="docType">文件類 </param>
        /// <param name="soCodeList">訂單編號List</param>
        /// <returns></returns>
        public IEnumerable<SapBapiAccDocumentInfo> GetData(DateTime startDate, DateTime endDate, AccountsDocumentType.DocTypeEnum docType, List<string> soCodeList)
        {
            IEnumerable<SapBapiAccDocumentInfo> list = null;
            SapBapiAccDocumentInfo sapInfo = new SapBapiAccDocumentInfo();

            try
            {
                string strDocType = docType.ToString();

                //FinanceDocumentCreateNote
                IQueryable<int> docTypeList = this._accDocTypeRepo.GetAll().Where(x => x.DocType == strDocType).Select(x => x.Code);
                var noteList = this._finanDocCreNoteRepo.GetAll()
                           .Where(x => docTypeList.Contains(x.AccDocTypeCode) && x.CreateDate >= startDate && x.CreateDate <= endDate);

                if (soCodeList != null && soCodeList.Count > 0)
                    noteList = noteList.Where(x => soCodeList.Contains(x.SalesOrderCode));

                //取得SAP資料 Sap_BapiAccDocument_DocHeader、Sap_BapiAccDocument_DocDetail
                var detailList = this._sapDocDetailRepo.GetAll();
                list = this._sapDocHeaderRepo.GetAll()
                    .Where(header => noteList.Any(note => note.DocNumber == header.DOC_NUMBER))
                    .OrderBy(header => header.DOC_NUMBER)
                    .ToList()
                    .Select(header => new SapBapiAccDocumentInfo
                    {
                        DocType = docType,
                        DocHeader = header,
                        DocDetail = detailList
                         .Where(detail => detail.TransactionType == header.TransactionType && detail.TransactionID == header.TransactionID)
                         .OrderBy(detail => detail.ITEMNO_ACC)
                    });

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得客戶資料
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="twNewEggData"></param>
        /// <returns></returns>
        public List<CustomerInfo> GetCustomerData(DateTime startDate, DateTime endDate, FinanceDataListFinanceData twNewEggData)
        {
            List<CustomerInfo> customerList = new List<CustomerInfo>();
            try
            {
                List<string> typeList = new List<string>() { "XQ", "XD" };

                //取得XQ、XD的訂單
                IQueryable<int> docTypeList = this._accDocTypeRepo.GetAll().Where(x => typeList.Contains(x.DocType.Trim())).Select(x => x.Code);
                List<string> docList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate && docTypeList.Contains(x.AccDocTypeCode))
                    .Select(x => x.SalesOrderCode).ToList();

                if (docList.Count() > 0)
                {
                    //取得訂單客戶資料
                    //IQueryable<SalesOrder> soList = from so in this._saleOrderRepo.GetAll().Where(x=>docList.Contains(x.Code))
                    //                                join user in this._accountRepo.GetAll()
                    //                                on so.AccountID equals user.ID
                    //                                select so;

                    customerList = (from so in this._saleOrderRepo.GetAll().Where(x => docList.Contains(x.Code))
                                    join user in this._accountRepo.GetAll()
                                    on so.AccountID equals user.ID
                                    select new
                                    {
                                        SO = so,
                                        User = user
                                    }).AsEnumerable().Select(x => new CustomerInfo
                                    {
                                        KUNNR = "WW" + x.SO.AccountID.ToString().PadLeft(8, '0'),
                                        KTOKD = (twNewEggData.CUSTOMERDATA.KTOKD ?? "W200"),
                                        LAND1 = (twNewEggData.CUSTOMERDATA.LAND1 ?? "TW"),
                                        NAME1 = x.SO.Name, //顧客姓名(中文)
                                        NAME2 = "",
                                        EMAIL = x.SO.Email, //顧客Email
                                        ORT01 = x.SO.CardLOC, //顧客縣市
                                        PSTLZ = (x.SO.CardZip).PadRight(5, '0'), //顧客郵遞區號
                                        REGIO = "",
                                        SORTL = "",
                                        STRAS = x.SO.CardLOC + x.SO.CardADDR, //顧客住址
                                        TELF1 = ((x.SO.TelDay.Length > 15) ? x.SO.TelDay.Substring(0, 16) : x.SO.TelDay), //顧客電話
                                        TELF2 = x.SO.Mobile, //顧客手機
                                        TELFX = "",
                                        SPRAS = (twNewEggData.CUSTOMERDATA.SPRAS ?? "M"),
                                        STCD1 = (string.IsNullOrWhiteSpace(x.SO.InvoiceID) ? "" : x.SO.InvoiceID.Trim()), //客戶統編
                                        BUKRS = (twNewEggData.CUSTOMERDATA.BUKRS ?? "3103"),
                                        AKONT = (twNewEggData.CUSTOMERDATA.AKONT ?? "0002261010"),
                                        ZTERM = (twNewEggData.CUSTOMERDATA.ZTERM ?? "NT00"),
                                        ZACTIONCODE = (string.IsNullOrWhiteSpace(x.User.ActionCode) ? "C" : x.User.ActionCode.Trim())    //Istosap=0 => ActionCode=C, Istosap=1 => ActionCode=U
                                    })
                                    .GroupBy(g => new
                                    {
                                        g.KUNNR,
                                        //g.KTOKD,
                                        //g.LAND1,
                                        g.NAME1,
                                        g.NAME2,
                                        g.EMAIL,
                                        g.ORT01,
                                        g.PSTLZ,
                                        g.REGIO,
                                        g.SORTL,
                                        g.STRAS,
                                        g.TELF1,
                                        g.TELF2,
                                        g.TELFX,
                                        //g.SPRAS,
                                        g.STCD1,
                                        //g.BUKRS,
                                        //g.AKONT,
                                        //g.ZTERM,
                                        g.ZACTIONCODE
                                    })
                                .Select(g => g.First()).ToList();
                }

                return customerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 依訂單編號取得客戶資料
        /// </summary>
        /// <param name="cartIDList"></param>
        /// <param name="twNewEggData"></param>
        /// <returns></returns>
        public List<CustomerInfo> GetCustomerDataByCartID(List<string> cartIDList, FinanceDataListFinanceData twNewEggData)
        {
            List<CustomerInfo> customerList = new List<CustomerInfo>();
            try
            {
                //取得訂單客戶資料
                customerList = (from so in this._saleOrderRepo.GetAll().Where(x => cartIDList.Contains(x.Code))
                                join user in this._accountRepo.GetAll()
                                on so.AccountID equals user.ID
                                select new
                                {
                                    SO = so,
                                    User = user
                                }).AsEnumerable().Select(x => new CustomerInfo
                                {
                                    KUNNR = "WW" + x.SO.AccountID.ToString().PadLeft(8, '0'),
                                    KTOKD = (twNewEggData.CUSTOMERDATA.KTOKD ?? "W200"),
                                    LAND1 = (twNewEggData.CUSTOMERDATA.LAND1 ?? "TW"),
                                    NAME1 = x.SO.Name, //顧客姓名(中文)
                                    NAME2 = "",
                                    EMAIL = x.SO.Email, //顧客Email
                                    ORT01 = x.SO.CardLOC, //顧客縣市
                                    PSTLZ = (x.SO.CardZip).PadRight(5, '0'), //顧客郵遞區號
                                    REGIO = "",
                                    SORTL = "",
                                    STRAS = x.SO.CardLOC + x.SO.CardADDR, //顧客住址
                                    TELF1 = ((x.SO.TelDay.Length > 15) ? x.SO.TelDay.Substring(0, 16) : x.SO.TelDay), //顧客電話
                                    TELF2 = x.SO.Mobile, //顧客手機
                                    TELFX = "",
                                    SPRAS = (twNewEggData.CUSTOMERDATA.SPRAS ?? "M"),
                                    STCD1 = (string.IsNullOrWhiteSpace(x.SO.InvoiceID) ? "" : x.SO.InvoiceID.Trim()), //客戶統編
                                    BUKRS = (twNewEggData.CUSTOMERDATA.BUKRS ?? "3103"),
                                    AKONT = (twNewEggData.CUSTOMERDATA.AKONT ?? "0002261010"),
                                    ZTERM = (twNewEggData.CUSTOMERDATA.ZTERM ?? "NT00"),
                                    ZACTIONCODE = (string.IsNullOrWhiteSpace(x.User.ActionCode) ? "C" : x.User.ActionCode.Trim())    //Istosap=0 => ActionCode=C, Istosap=1 => ActionCode=U
                                })
                                .GroupBy(g => new
                                {
                                    g.KUNNR,
                                    //g.KTOKD,
                                    //g.LAND1,
                                    g.NAME1,
                                    g.NAME2,
                                    g.EMAIL,
                                    g.ORT01,
                                    g.PSTLZ,
                                    g.REGIO,
                                    g.SORTL,
                                    g.STRAS,
                                    g.TELF1,
                                    g.TELF2,
                                    g.TELFX,
                                    //g.SPRAS,
                                    g.STCD1,
                                    //g.BUKRS,
                                    //g.AKONT,
                                    //g.ZTERM,
                                    g.ZACTIONCODE
                                })
                                .Select(g => g.First()).ToList();

                return customerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 依會計文件編號取得客戶資料
        /// </summary>
        /// <param name="docNumberList"></param>
        /// <param name="twNewEggData"></param>
        /// <returns></returns>
        public List<CustomerInfo> GetCustomerDataByDocNumber(List<string> docNumberList, FinanceDataListFinanceData twNewEggData)
        {
            List<CustomerInfo> customerList = new List<CustomerInfo>();
            try
            {
                List<string> soCodeList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => docNumberList.Contains(x.DocNumber))
                    .Select(x => x.SalesOrderCode).ToList();

                //取得訂單客戶資料
                customerList = (from so in this._saleOrderRepo.GetAll().Where(x => soCodeList.Contains(x.Code))
                                join user in this._accountRepo.GetAll()
                                on so.AccountID equals user.ID
                                select new
                                {
                                    SO = so,
                                    User = user
                                }).AsEnumerable().Select(x => new CustomerInfo
                                {
                                    KUNNR = "WW" + x.SO.AccountID.ToString().PadLeft(8, '0'),
                                    KTOKD = (twNewEggData.CUSTOMERDATA.KTOKD ?? "W200"),
                                    LAND1 = (twNewEggData.CUSTOMERDATA.LAND1 ?? "TW"),
                                    NAME1 = x.SO.Name, //顧客姓名(中文)
                                    NAME2 = "",
                                    EMAIL = x.SO.Email, //顧客Email
                                    ORT01 = x.SO.CardLOC, //顧客縣市
                                    PSTLZ = (x.SO.CardZip).PadRight(5, '0'), //顧客郵遞區號
                                    REGIO = "",
                                    SORTL = "",
                                    STRAS = x.SO.CardLOC + x.SO.CardADDR, //顧客住址
                                    TELF1 = ((x.SO.TelDay.Length > 15) ? x.SO.TelDay.Substring(0, 16) : x.SO.TelDay), //顧客電話
                                    TELF2 = x.SO.Mobile, //顧客手機
                                    TELFX = "",
                                    SPRAS = (twNewEggData.CUSTOMERDATA.SPRAS ?? "M"),
                                    STCD1 = (string.IsNullOrWhiteSpace(x.SO.InvoiceID) ? "" : x.SO.InvoiceID.Trim()), //客戶統編
                                    BUKRS = (twNewEggData.CUSTOMERDATA.BUKRS ?? "3103"),
                                    AKONT = (twNewEggData.CUSTOMERDATA.AKONT ?? "0002261010"),
                                    ZTERM = (twNewEggData.CUSTOMERDATA.ZTERM ?? "NT00"),
                                    ZACTIONCODE = (string.IsNullOrWhiteSpace(x.User.ActionCode) ? "C" : x.User.ActionCode.Trim())    //Istosap=0 => ActionCode=C, Istosap=1 => ActionCode=U
                                })
                                .GroupBy(g => new
                                {
                                    g.KUNNR,
                                    //g.KTOKD,
                                    //g.LAND1,
                                    g.NAME1,
                                    g.NAME2,
                                    g.EMAIL,
                                    g.ORT01,
                                    g.PSTLZ,
                                    g.REGIO,
                                    g.SORTL,
                                    g.STRAS,
                                    g.TELF1,
                                    g.TELF2,
                                    g.TELFX,
                                    //g.SPRAS,
                                    g.STCD1,
                                    //g.BUKRS,
                                    //g.AKONT,
                                    //g.ZTERM,
                                    g.ZACTIONCODE
                                })
                                .Select(g => g.First()).ToList();

                return customerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SAPLogInfo> GetSAPLog(DateTime startDate, DateTime endDate, List<string> docTypeList, SAPLogInfo.LogTypeEnum logType, List<string> cartIDList)
        {
            List<string> resultTypeList = new List<string>();
            try
            {
                startDate = DateTime.Parse(startDate.ToShortDateString());
                endDate = DateTime.Parse(endDate.AddDays(1).ToShortDateString());

                var docNoteList = this._finanDocCreNoteRepo.GetAll()
                    .Where(x => x.CreateDate >= startDate && x.CreateDate < endDate);

                if (cartIDList.Count > 0)
                    docNoteList = docNoteList.Where(x => cartIDList.Contains(x.SalesOrderCode));

                IQueryable<SAPLogInfo> logList;

                if (logType == SAPLogInfo.LogTypeEnum.Sussess)
                {
                    //成功
                    logList = (from doc in docNoteList
                               join accDocType in this._accDocTypeRepo.GetAll().Where(x => docTypeList.Contains(x.DocType))
                               on doc.AccDocTypeCode equals accDocType.Code
                               join trans in this._finDocTransLogRepo.GetAll()
                                    .Where(x => x.CreateDate >= startDate && x.ResultType == "S")
                               on doc.TransactionID equals trans.TransactionNumber
                               join finDetail in this._sellerFinanDetailRepo.GetAll()
                               on doc.SalesOrderCode equals finDetail.OrderID into tmpFinDetailTable
                               from tmpFinDetail in tmpFinDetailTable.DefaultIfEmpty()
                               select new SAPLogInfo
                               {
                                   DocType = accDocType.DocType,
                                   DocTypeDesc = accDocType.Description,
                                   FinanceDocumentCreateNote = doc,
                                   FinDocTransLog = trans,
                                   SellerIsCheck = tmpFinDetail.IsCheck
                               });
                }
                else if (logType == SAPLogInfo.LogTypeEnum.Fail)
                {
                    //失敗
                    logList = (from doc in docNoteList
                               join accDocType in this._accDocTypeRepo.GetAll().Where(x => docTypeList.Contains(x.DocType))
                               on doc.AccDocTypeCode equals accDocType.Code
                               join trans in this._finDocTransLogRepo.GetAll()
                                    .Where(x => x.CreateDate >= startDate && x.ResultType == "S")
                               on doc.TransactionID equals trans.TransactionNumber into tmpTransTable
                               from tmpTrans in tmpTransTable.DefaultIfEmpty()
                               where tmpTrans == null
                               join finDetail in this._sellerFinanDetailRepo.GetAll()
                               on doc.SalesOrderCode equals finDetail.OrderID into tmpFinDetailTable
                               from tmpFinDetail in tmpFinDetailTable.DefaultIfEmpty()
                               select new SAPLogInfo
                               {
                                   DocType = accDocType.DocType,
                                   DocTypeDesc = accDocType.Description,
                                   FinanceDocumentCreateNote = doc,
                                   FinDocTransLog = tmpTrans,
                                   SellerIsCheck = tmpFinDetail.IsCheck
                               });
                }
                else
                {
                    //全部
                    logList = (from doc in docNoteList
                               join accDocType in this._accDocTypeRepo.GetAll().Where(x => docTypeList.Contains(x.DocType))
                               on doc.AccDocTypeCode equals accDocType.Code
                               join trans in this._finDocTransLogRepo.GetAll()
                                    .Where(x => x.CreateDate >= startDate)
                               on doc.TransactionID equals trans.TransactionNumber into tmpTransTable
                               from tmpTrans in tmpTransTable.DefaultIfEmpty()
                               join finDetail in this._sellerFinanDetailRepo.GetAll()
                               on doc.SalesOrderCode equals finDetail.OrderID into tmpFinDetailTable
                               from tmpFinDetail in tmpFinDetailTable.DefaultIfEmpty()
                               select new SAPLogInfo
                               {
                                   DocType = accDocType.DocType,
                                   DocTypeDesc = accDocType.Description,
                                   FinanceDocumentCreateNote = doc,
                                   FinDocTransLog = tmpTrans,
                                   SellerIsCheck = tmpFinDetail.IsCheck
                               });
                }

                return logList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 重置會計文件
        /// </summary>
        /// <param name="soCode"></param>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public bool RedoFinanceDocument(string soCode, int docTypeCode)
        {
            Seller_FinanDetail detailInfo = null;

            try
            {
                //檢查 FinanceDocumentCreateNote
                FinanceDocumentCreateNote finanDocInfo = this._finanDocCreNoteRepo.Get(x => x.SalesOrderCode == soCode && x.AccDocTypeCode == docTypeCode);
                if (finanDocInfo == null)
                    throw new Exception("FinanceDocumentCreateNote查無資料。");

                finanDocInfo.ReprocessingFlag = "1";
                
                //檢查台蛋訂單 Seller_FinanMaster、Seller_FinanDetail
                Cart cartInfo = this._cartRepo.Get(x => x.ID == soCode);
                if (cartInfo == null)
                    throw new Exception("查無訂單資料。");

                //7=B2C直配,9=B2C寄倉
                AccountsDocumentType docTypeInfo = this._accDocTypeRepo.Get(x => x.Code == docTypeCode);
                if (docTypeInfo == null)
                    throw new Exception(string.Format("訂單{0}會計文件類型({1})異常。", soCode, docTypeCode));

                if (docTypeInfo.DocType.ToUpper().Trim() == "XI" || docTypeInfo.DocType.ToUpper().Trim() == "XIRMA")
                {
                    if ((cartInfo.ShipType == 7 || cartInfo.ShipType == 9) && finanDocInfo.SalesOrderType == 1)
                    {
                        switch (docTypeInfo.DocType.ToUpper().Trim())
                        {
                            case "XI":
                                detailInfo = this._sellerFinanDetailRepo.GetAll()
                                    .Where(x => x.OrderID == soCode && (x.SettleType == (int)Seller_FinanDetail.SettleType_Identify.訂單 || x.SettleType == (int)Seller_FinanDetail.SettleType_Identify.寄倉)).FirstOrDefault();
                                break;
                            case "XIRMA":
                                //detailInfo = detailList.Where(x => x.SettleType == (int)Seller_FinanDetail.SettleType_Identify.退貨).FirstOrDefault();
                                var retgoodList = this._retgoodRepo.GetAll().Where(x => x.CartID == soCode);

                                detailInfo = (from detail in this._sellerFinanDetailRepo.GetAll().Where(x => x.SettleType == (int)Seller_FinanDetail.SettleType_Identify.退貨)
                                              where retgoodList.Any(x => x.Code == detail.OrderID)
                                              select detail).FirstOrDefault();
                                break;
                            //default:
                            //    throw new Exception(string.Format("對帳單無法處理此會計文件類型({0})。", docTypeInfo.DocType.ToUpper().Trim()));
                        }

                        if (detailInfo == null)
                            throw new Exception("Seller_FinanDetail查無資料。");

                        Seller_FinanMaster masterInfo = this._sellerFinanMasterRepo.Get(x => x.SettlementID == detailInfo.SettlementID);
                        if (masterInfo != null && (
                            masterInfo.FinanStatus.ToUpper().Trim() == Seller_FinanMaster.FinanStatusType.C.ToString() ||
                            masterInfo.InvoDate != null || !string.IsNullOrWhiteSpace(masterInfo.InvoNumber)
                            ))
                        {
                            throw new Exception("對帳單已押發票，無法重置。");
                        }

                        detailInfo.IsCheck = "N";

                        this._sellerFinanDetailRepo.Update(detailInfo);
                    }
                }

                this._finanDocCreNoteRepo.Update(finanDocInfo);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.Error(string.Format("SalesOrderCode: {0}, AccDocTypeCode: {1}", soCode, docTypeCode), ex);
                return false;
            }
        }
    }
}
