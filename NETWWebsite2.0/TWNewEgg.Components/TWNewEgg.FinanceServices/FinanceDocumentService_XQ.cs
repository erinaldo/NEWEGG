using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.FinanceServices.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;
using TWNewEgg.FinanceRepoAdapters.Interface;

namespace TWNewEgg.FinanceServices
{
    public partial class FinanceDocumentService : IFinanceDocumentService
    {
        private Sap_BapiAccDocument_DocHeader CreateSAPDocHeaderForXQ(AccountsDocumentType accDocTypeInfo, CartGroupInfo cartGrpInfo, FinanceDataListFinanceData twNewEggData, BankAccountsInfo bankInfo)
        {
            Sap_BapiAccDocument_DocHeader docHeader = new Sap_BapiAccDocument_DocHeader();

            FinanceDocumentCreateNote finanDocCreNoteInfo = null;
            //string strDocType = docType.ToString(); //"XQ";
            string strDocType = accDocTypeInfo.DocType.Trim();
            
            try
            {
                //確認會計文件是否已存在
                finanDocCreNoteInfo = this._finanRepoAdapter.GetDocCreateNote(cartGrpInfo.SalesOrderList.First().ID, accDocTypeInfo.Code);

                //基本資料
                docHeader.TransactionType = strDocType;

                if (finanDocCreNoteInfo == null)
                    docHeader.TransactionID = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.TrID, cartGrpInfo.DocDate).ToString();
                else
                    docHeader.TransactionID = finanDocCreNoteInfo.TransactionID;

                docHeader.BUS_ACT = twNewEggData.DOCUMENTHEADER.BUS_ACT;
                docHeader.USERNAME = twNewEggData.DOCUMENTHEADER.USERNAME;

                //"花旗_授權碼、歐付寶_授權碼"
                CreditAuth caInfo = this._finanRepoAdapter.GetCreditAuth(cartGrpInfo.SalesOrderGroupID);
                if (caInfo == null)
                    throw new Exception("CreditAuth查無授權碼。");
                else
                    docHeader.HEADER_TXT = string.Format("{0}_{1}", bankInfo.Accounts.AccDescription, caInfo.AuthCode);

                if (!string.IsNullOrWhiteSpace(docHeader.HEADER_TXT) && docHeader.HEADER_TXT.Length > 24)
                    docHeader.HEADER_TXT = docHeader.HEADER_TXT.Substring(0, 24);

                docHeader.COMP_CODE = twNewEggData.DOCUMENTHEADER.COMP_CODE;

                docHeader.DOC_DATE = cartGrpInfo.DocDate;
                docHeader.PSTNG_DATE = docHeader.DOC_DATE;
                docHeader.TRANS_DATE = docHeader.DOC_DATE;

                docHeader.FISC_YEAR = string.Format("{0:yyyy}", cartGrpInfo.DocDate);
                docHeader.FIS_PERIOD = string.Format("{0:MM}", cartGrpInfo.DocDate);

                docHeader.DOC_TYPE = strDocType;

                //存檔才給文件編號(文件編號不可跳號)
                //if (finanDocCreNoteInfo == null)
                //    docHeader.DOC_NUMBER = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.XQ, cartGrpInfo.DocDate).ToString();
                //else
                //    docHeader.DOC_NUMBER = finanDocCreNoteInfo.DocNumber;

                //docHeader.AC_DOC_NO = docHeader.DOC_NUMBER;
                docHeader.REF_DOC_NO = cartGrpInfo.SalesOrderGroupID.ToString(); //cartFirstInfo.SalesorderGroupID.GetValueOrDefault().ToString();

                docHeader.C_STATUS = "";
                docHeader.P_STATUS = "";

                //客戶資料Customer
                //docHeader.Receivalbe_ItemNOACC = coaProfileList.Where(x => x.AccPattern == "C").FirstOrDefault().ItemNo;
                docHeader.Receivable_Customer = "WW" + cartGrpInfo.SalesOrderList.First().UserID.PadLeft(8, '0');
                //docHeader.Receivable_BusArea = "";
                //docHeader.Receivable_Pmnttrms = "";
                docHeader.Receivable_AllocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                //docHeader.Receivable_ItemText = cartInfo.ID;
                //docHeader.Receivable_SPGLIND = "";

                ////廠商資料Vendor
                //docHeader.Payable_ItemNOACC = coaProfile.Where(x => x.AccType == "V").FirstOrDefault().ItemNo;
                //docHeader.Payable_VendorNO = "";
                //docHeader.Payable_BusArea = "";
                //docHeader.Payable_Pmnttrms = "";
                //docHeader.Payable_AllocNmbr = "";
                //docHeader.Payable_ItemText = "";
                //docHeader.Payable_SPGLIND = "";

                return docHeader;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        private Sap_BapiAccDocument_DocDetail CreateSAPDocDetailForXQ(Sap_BapiAccDocument_DocHeader docHeader, ChartOfAccountsProfile coaProfile, CartGroupInfo cartGrpInfo, 
            Cart cartInfo, BankAccountsInfo bankInfo, int itemNoAcc)
        {
            Sap_BapiAccDocument_DocDetail docDetail = new Sap_BapiAccDocument_DocDetail();
            decimal itemPrice = 0;

            try
            {
                docDetail.TransactionType = docHeader.TransactionType;
                docDetail.TransactionID = docHeader.TransactionID;
                docDetail.ITEMNO_ACC = itemNoAcc; //coaProfile.ItemNo;
                docDetail.ACCT_TYPE = "S";

                docDetail.GL_ACCOUNT = GetFinanDocGLAccount(coaProfile, bankInfo);

                docDetail.CUSTOMER_ID = "";
                docDetail.VENDOR_ID = "";
                switch (coaProfile.AccPattern.Trim())
                {
                    case "C":
                        docDetail.CUSTOMER_ID = string.Format("WW{0}", cartInfo.UserID.PadLeft(8, '0'));
                        break;
                    case "S":
                        break;
                    default:
                        throw new Exception(string.Format("AccPattern({0})程式尚未設定。", coaProfile.AccPattern));
                }

                docDetail.ALLOC_NMBR = cartGrpInfo.SalesOrderGroupID.ToString();
                docDetail.ITEM_TEXT = cartInfo.ID;
                docDetail.BUS_AREA = "";
                docDetail.ORDERID = "";
                docDetail.COSTCENTER = "";
                docDetail.PROFIT_CTR = coaProfile.ProfitCtr;
                docDetail.PMNTTREMS = "";

                docDetail.VALUE_DATE = cartGrpInfo.DocDate;
                docDetail.BLINE_DATE = docDetail.VALUE_DATE;

                docDetail.PYMT_METH = "";
                docDetail.PMNT_BLOCK = "";
                docDetail.SP_GL_IND = "";

                docDetail.CURR_TYPE = "";
                docDetail.CURRENCY = "TWD";

                //AMT_DOCCUR
                List<Process> procList = null;
                SapPriceInfo priceInfo = null;

                foreach (Cart tmpCartInfo in cartGrpInfo.SalesOrderList)
                {
                    procList = this._finanRepoAdapter.GetCartProcess(tmpCartInfo.ID).ToList();
                    priceInfo = new SapPriceInfo(AccountsDocumentType.DocTypeEnum.XQ, cartInfo.ShipType ?? 0, null, procList);

                    itemPrice += priceInfo.SalesAmount;
                }

                //收入(購物車結帳金額合計)
                docDetail.AMT_DOCCUR = (coaProfile.SignFlag == "1" ? 1 : -1) * itemPrice;

                return docDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
