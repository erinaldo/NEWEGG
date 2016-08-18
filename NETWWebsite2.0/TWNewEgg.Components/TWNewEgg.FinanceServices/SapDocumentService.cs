using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.FinanceServices.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;
using TWNewEgg.FinanceRepoAdapters.Interface;
using System.Collections;
using TWNewEgg.Framework.AutoMapper;
using Autofac;
using TWNewEgg.Framework.Autofac;

namespace TWNewEgg.FinanceServices
{
    public class SapDocumentService : ISapDocumentService
    {
        log4net.ILog _logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);
        ISapBapiAccDocumentRepoAdapter _sapRepoAdapter;
        ISellerFinanceRepoAdapter _sellerFinanRepoAdapter;

        public SapDocumentService(ISapBapiAccDocumentRepoAdapter sapRepoAdapter, ISellerFinanceRepoAdapter sellerFinanRepoAdapter)
        {
            this._sapRepoAdapter = sapRepoAdapter;
            this._sellerFinanRepoAdapter = sellerFinanRepoAdapter;
        }

        enum DateRangeEnum
        {
            Start,
            End
        }
        private DateTime DateTimeParse(DateTime oDate, DateRangeEnum rangeType)
        {
            string strTime = "00:00:00";
            if (rangeType == DateRangeEnum.End)
                strTime = "23:59:59";

            return DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1}", oDate.ToShortDateString(), strTime));
        }

        /// <summary>
        /// 取得SAP上傳的資料
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ResponseMessage<List<SapBapiAccDocumentDM>> GetData(DocConditionDM condition)
        {
            DateTime SDate, EDate;
            ResponseMessage<List<SapBapiAccDocumentDM>> result = new ResponseMessage<List<SapBapiAccDocumentDM>>();

            try
            {
                SDate = DateTimeParse(condition.StartDate, DateRangeEnum.Start);
                EDate = DateTimeParse(condition.EndDate, DateRangeEnum.End);

                //記錄待執行的會計文件類型
                ArrayList docTypeList = new ArrayList();
                switch (condition.DocType)
                {
                    case FinanDocTypeEnum.XQ:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XQ);
                        break;
                    case FinanDocTypeEnum.XD:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XD);
                        break;
                    case FinanDocTypeEnum.XI:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XI);
                        break;
                    case FinanDocTypeEnum.XIRMA:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XIRMA);
                        break;
                    default:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XQ);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XD);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XI);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XIRMA);
                        break;
                }

                //取得會計SAP資料
                List<SapBapiAccDocumentInfo> sapList = new List<SapBapiAccDocumentInfo>();
                IEnumerable<SapBapiAccDocumentInfo> tmpList = null;

                foreach (AccountsDocumentType.DocTypeEnum docType in docTypeList)
                {
                    tmpList = this._sapRepoAdapter.GetData(SDate, EDate, docType, condition.SalesOrderCodeList);
                    if (tmpList != null && tmpList.Count() > 0)
                        sapList.AddRange(tmpList);
                }

                result.IsSuccess = true;
                result.Data = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<SapBapiAccDocumentDM>>(sapList);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }

        public ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerData(DateTime nowDate)
        {
            DateTime SDate, EDate;
            ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> result = new ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>>();

            try
            {
                SDate = DateTimeParse(nowDate, DateRangeEnum.Start);
                EDate = DateTimeParse(nowDate, DateRangeEnum.End);

                ICompanyFinanceDataService twNewEggFinanService = new CompanyFinanceDataService();

                FinanceDataListFinanceData twNewEggData = twNewEggFinanService.Get(nowDate);

                List<CustomerInfo> customerList = this._sapRepoAdapter.GetCustomerData(SDate, EDate, twNewEggData);

                result.IsSuccess = true;
                result.Data = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<ZNETW_CUSTOMERCUSTOMERDATA>>(customerList);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }

        public ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerDataByCartID(DateTime nowDate, List<string> cartIDList)
        {
            ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> result = new ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>>();
            ICompanyFinanceDataService twNewEggFinanService = new CompanyFinanceDataService();

            try
            {
                FinanceDataListFinanceData twNewEggData = twNewEggFinanService.Get(nowDate);

                List<CustomerInfo> customerList = this._sapRepoAdapter.GetCustomerDataByCartID(cartIDList, twNewEggData);

                result.IsSuccess = true;
                result.Data = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<ZNETW_CUSTOMERCUSTOMERDATA>>(customerList);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }

        public ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerDataByDocNumber(DateTime nowDate, List<string> docNumberList)
        {
            ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> result = new ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>>();
            ICompanyFinanceDataService twNewEggFinanService = new CompanyFinanceDataService();

            try
            {
                FinanceDataListFinanceData twNewEggData = twNewEggFinanService.Get(nowDate);

                List<CustomerInfo> customerList = this._sapRepoAdapter.GetCustomerDataByDocNumber(docNumberList, twNewEggData);

                result.IsSuccess = true;
                result.Data = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<ZNETW_CUSTOMERCUSTOMERDATA>>(customerList);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }

        public ResponseMessage<List<SAPLogDM>> GetSAPLog(DateTime startDate, DateTime endDate, FinanDocTypeEnum docType, SAPLogDM.LogTypeEnum logType, List<string> cartIDList)
        {
            ResponseMessage<List<SAPLogDM>> result = new ResponseMessage<List<SAPLogDM>>();
            List<SAPLogInfo> znetwCustomerList = new List<SAPLogInfo>();
            List<FinanDocTypeEnum> docTypeList = new List<FinanDocTypeEnum>();

            try
            {
                switch (docType)
                {
                    case FinanDocTypeEnum.ALL:
                        docTypeList.Add(FinanDocTypeEnum.XQ);
                        docTypeList.Add(FinanDocTypeEnum.XD);
                        docTypeList.Add(FinanDocTypeEnum.XI);
                        docTypeList.Add(FinanDocTypeEnum.XIRMA);
                        break;
                    default:
                        docTypeList.Add(docType);
                        break;
                }

                List<SAPLogInfo> logList = this._sapRepoAdapter.GetSAPLog(
                    startDate, endDate, 
                    docTypeList.Select(x => x.ToString()).ToList(), 
                    (SAPLogInfo.LogTypeEnum)logType,
                    cartIDList
                    );

                result.IsSuccess = true;
                result.Data = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<SAPLogDM>>(logList);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }

        public ResponseMessage<List<string>> RedoFinanceDocument(List<string> salesOrderList)
        {
            ResponseMessage<List<string>> result = new ResponseMessage<List<string>>();
            string[] aryTmp;
            List<string> redoList = new List<string>();

            try
            {
                foreach (string strItem in salesOrderList)
                {
                    aryTmp = strItem.Split(Convert.ToChar(1));
                    if (this._sapRepoAdapter.RedoFinanceDocument(aryTmp[0], int.Parse(aryTmp[1])))
                    {
                        redoList.Add(strItem);
                    }
                }

                result.IsSuccess = true;
                result.Message = string.Format("共{0}筆，重置完成{1}筆。", salesOrderList.Count, redoList.Count);
                result.Data = redoList;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error.Detail = ex.ToString();
            }
            return result;
        }       
    }
}
