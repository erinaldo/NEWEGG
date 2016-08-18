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
        private Sap_BapiAccDocument_DocHeader CreateSAPDocHeaderForXI(AccountsDocumentType accDocTypeInfo, CartGroupInfo cartGrpInfo, FinanceDataListFinanceData twNewEggData, 
            BankAccountsInfo bankInfo, int shipType)
        {
            Sap_BapiAccDocument_DocHeader docHeader = new Sap_BapiAccDocument_DocHeader();

            FinanceDocumentCreateNote finanDocCreNoteInfo = null;
            //AccountsDocumentType.DocTypeEnum docType
            string strDocType = accDocTypeInfo.DocType.Trim();
            PurchaseOrderitemTWBACK poItem = null;

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

                if (string.IsNullOrWhiteSpace(cartGrpInfo.SalesOrderList.First().InvoiceNO))
                    throw new Exception("Cart無InvoiceNO資料。");

                string strInvoiceNO = "";
                switch (accDocTypeInfo.Code)
                {
                    case 31://三角-美金
                        //strInvoiceNO = this._finanRepoAdapter.GetNewEggInvoiceNo(cartGrpInfo.SalesOrderList.First().ID);
                        
                        poItem = this._finanRepoAdapter.GetOrderPOItem(cartGrpInfo.SalesOrderList.First().ID);
                        strInvoiceNO = poItem.InvoiceNO;                   
                        break;
                    default:
                        strInvoiceNO = cartGrpInfo.SalesOrderList.First().InvoiceNO;
                        break;
                }

                //00(交易模式代號)_統一發票號碼(10碼)_客戶統編
                //docHeader.HEADER_TXT = string.Format("{0}_{1}_WW{2}",
                //    shipType.ToString().PadLeft(2, '0'), strInvoiceNO, cartGrpInfo.SalesOrderList.First().UserID.PadLeft(8, '0'));

                //客戶統編
                string strInvoiceID = "";
                if (!string.IsNullOrWhiteSpace(cartGrpInfo.SalesOrderList.First().ActCode))
                    strInvoiceID = "_" + cartGrpInfo.SalesOrderList.First().ActCode.Trim();

                docHeader.HEADER_TXT = string.Format("{0}_{1}{2}",
                    shipType.ToString().PadLeft(2, '0'), strInvoiceNO, strInvoiceID);

                if (!string.IsNullOrWhiteSpace(docHeader.HEADER_TXT) && docHeader.HEADER_TXT.Length > 24)
                    docHeader.HEADER_TXT = docHeader.HEADER_TXT.Substring(0, 24);

                docHeader.COMP_CODE = twNewEggData.DOCUMENTHEADER.COMP_CODE;

                docHeader.DOC_DATE = cartGrpInfo.DocDate;
                docHeader.PSTNG_DATE = docHeader.DOC_DATE;
                docHeader.TRANS_DATE = docHeader.DOC_DATE;

                docHeader.FISC_YEAR = string.Format("{0:yyyy}", cartGrpInfo.DocDate);
                docHeader.FIS_PERIOD = string.Format("{0:MM}", cartGrpInfo.DocDate);

                docHeader.DOC_TYPE = strDocType;

                //if (finanDocCreNoteInfo == null)
                //    docHeader.DOC_NUMBER = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.XI, cartGrpInfo.DocDate).ToString();
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
                //docHeader.Receivable_ItemText = cartGrpInfo.CartFirstOrder.ID; //cartFirstInfo.ID;
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

        private Sap_BapiAccDocument_DocDetail CreateSAPDocDetailForXI(Sap_BapiAccDocument_DocHeader docHeader, ChartOfAccountsProfile coaProfile, CartGroupInfo cartGrpInfo,
            Cart cartInfo, BankAccountsInfo bankInfo, int itemNoAcc, SapPriceInfo priceInfo)
        {
            Sap_BapiAccDocument_DocDetail docDetail = new Sap_BapiAccDocument_DocDetail();

            try
            {
                IQueryable<Process> procList = this._finanRepoAdapter.GetCartProcess(cartInfo.ID);

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
                    case "V":
                        switch (coaProfile.AccDocTypeCode)
                        {
                            case 3:
                                docDetail.VENDOR_ID = this._sellerFinanRepoAdapter.GetOrderVendorID(procList.FirstOrDefault().StoreID.GetValueOrDefault());
                                break;
                            case 31://三角-美金
                                docDetail.VENDOR_ID = "331055";
                                break;
                            default:
                                throw new Exception(string.Format("ChartOfAccountsProfile.AccDocTypeCode: {0}，程式尚未設定。", coaProfile.AccDocTypeCode));
                        }                        
                        break;
                    case "S":
                    case "A":
                        break;
                    default:
                        throw new Exception(string.Format("AccPattern({0})程式尚未設定。", coaProfile.AccPattern));
                }
               
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
                switch (coaProfile.AccDocTypeCode)
                {
                    case 31://三角-美金
                        docDetail.CURRENCY = "USD";
                        break;
                    default:
                        docDetail.CURRENCY = "TWD";
                        break;
                }                

                //XI
                switch (coaProfile.AccDocTypeCode)
                {
                    case 3:
                        SetAccountsItemForXIType3(coaProfile, docDetail, cartGrpInfo, cartInfo, procList, priceInfo);
                        break;
                    case 31:
                        SetAccountsItemForXIType31(coaProfile, docDetail, cartGrpInfo, cartInfo, priceInfo);
                        break;
                    default:
                        throw new Exception(string.Format("ChartOfAccountsProfile.AccDocTypeCode: {0}，程式尚未設定。", coaProfile.AccDocTypeCode));
                }

                return docDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Sap_BapiAccDocument_DocHeader CreateSAPDocHeaderForXI_Newegg(PurchaseOrderItemGroupInfo poGrpInfo, FinanceDataListFinanceData twNewEggData, int accDocTypeCode)
        {
            Sap_BapiAccDocument_DocHeader docHeader = new Sap_BapiAccDocument_DocHeader();

            FinanceDocumentCreateNote finanDocCreNoteInfo = null;
            //AccountsDocumentType.DocTypeEnum docType
            string strDocType = "XI";
            //PurchaseOrderitemTWBACK poItem = null;

            try
            {
                //確認會計文件是否已存在
                finanDocCreNoteInfo = this._finanRepoAdapter.GetDocCreateNote(poGrpInfo.PurchaseOrderItemList.First().SellerorderCode, accDocTypeCode);

                //基本資料
                docHeader.TransactionType = strDocType;

                if (finanDocCreNoteInfo == null)
                    docHeader.TransactionID = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.TrID, poGrpInfo.DocDate).ToString();
                else
                    docHeader.TransactionID = finanDocCreNoteInfo.TransactionID;

                docHeader.BUS_ACT = twNewEggData.DOCUMENTHEADER.BUS_ACT;
                docHeader.USERNAME = twNewEggData.DOCUMENTHEADER.USERNAME;

                if (string.IsNullOrWhiteSpace(poGrpInfo.PurchaseOrderItemList.First().InvoiceNO))
                    throw new Exception("查無美蛋InvoiceNO資料。");

                //switch (accDocTypeInfo.Code)
                //{
                //    case 31://三角-美金
                //    case 32://海外切貨-美金
                //        //strInvoiceNO = this._finanRepoAdapter.GetNewEggInvoiceNo(cartGrpInfo.SalesOrderList.First().ID);

                //        poItem = this._finanRepoAdapter.GetOrderPOItem(cartGrpInfo.SalesOrderList.First().ID);
                //        strInvoiceNO = poItem.InvoiceNO;
                //        break;
                //    default:
                //        strInvoiceNO = cartGrpInfo.SalesOrderList.First().InvoiceNO;
                //        break;
                //}

                //00(交易模式代號)_美蛋發票號碼
                docHeader.HEADER_TXT = string.Format("{0}_{1}",
                    "06", poGrpInfo.PurchaseOrderItemList.First().InvoiceNO);

                if (!string.IsNullOrWhiteSpace(docHeader.HEADER_TXT) && docHeader.HEADER_TXT.Length > 24)
                    docHeader.HEADER_TXT = docHeader.HEADER_TXT.Substring(0, 24);

                docHeader.COMP_CODE = twNewEggData.DOCUMENTHEADER.COMP_CODE;

                docHeader.DOC_DATE = poGrpInfo.DocDate;
                docHeader.PSTNG_DATE = docHeader.DOC_DATE;
                docHeader.TRANS_DATE = docHeader.DOC_DATE;

                docHeader.FISC_YEAR = string.Format("{0:yyyy}", poGrpInfo.DocDate);
                docHeader.FIS_PERIOD = string.Format("{0:MM}", poGrpInfo.DocDate);

                docHeader.DOC_TYPE = strDocType;

                //if (finanDocCreNoteInfo == null)
                //    docHeader.DOC_NUMBER = this._finanRepoAdapter.GetDocCurrentNumber(DocNumber_V2.DOCTypeEnum.XI, poGrpInfo.DocDate).ToString();
                //else
                //    docHeader.DOC_NUMBER = finanDocCreNoteInfo.DocNumber;

                //docHeader.AC_DOC_NO = docHeader.DOC_NUMBER;
                docHeader.REF_DOC_NO = poGrpInfo.PurchaseOrderItemList.First().SellerorderCode;
                //switch (accDocTypeInfo.Code)
                //{
                //    case 32://海外切貨-美金
                        
                //        break;
                //    default:
                //        docHeader.REF_DOC_NO = cartGrpInfo.SalesOrderGroupID.ToString(); //cartFirstInfo.SalesorderGroupID.GetValueOrDefault().ToString();
                //        break;
                //}


                docHeader.C_STATUS = "";
                docHeader.P_STATUS = "";

                //客戶資料Customer
                //docHeader.Receivalbe_ItemNOACC = coaProfileList.Where(x => x.AccPattern == "C").FirstOrDefault().ItemNo;
                docHeader.Receivable_Customer = ""; //"WW";// +cartGrpInfo.SalesOrderList.First().UserID.PadLeft(8, '0');
                //docHeader.Receivable_BusArea = "";
                //docHeader.Receivable_Pmnttrms = "";
                docHeader.Receivable_AllocNmbr = poGrpInfo.SalesOrderGroupID.ToString();
                //docHeader.Receivable_ItemText = cartGrpInfo.CartFirstOrder.ID; //cartFirstInfo.ID;
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

        private Sap_BapiAccDocument_DocDetail CreateSAPDocDetailForXI_Newegg(Sap_BapiAccDocument_DocHeader docHeader, ChartOfAccountsProfile coaProfile, PurchaseOrderItemGroupInfo poGrpInfo,
            int itemNoAcc)
        {
            Sap_BapiAccDocument_DocDetail docDetail = new Sap_BapiAccDocument_DocDetail();

            try
            {
                //IQueryable<Process> procList = this._finanRepoAdapter.GetCartProcess(cartInfo.ID);

                docDetail.TransactionType = docHeader.TransactionType;
                docDetail.TransactionID = docHeader.TransactionID;
                docDetail.ITEMNO_ACC = itemNoAcc; //coaProfile.ItemNo;
                docDetail.ACCT_TYPE = "S";

                docDetail.GL_ACCOUNT = GetFinanDocGLAccount(coaProfile, null);

                docDetail.CUSTOMER_ID = "";
                docDetail.VENDOR_ID = "";
                switch (coaProfile.AccPattern.Trim())
                {
                    //case "C":
                    //    docDetail.CUSTOMER_ID = string.Format("WW{0}", cartInfo.UserID.PadLeft(8, '0'));
                    //    break;
                    case "V":
                        docDetail.VENDOR_ID = "331055";
                        break;
                    case "S":
                    case "A":
                        break;
                    default:
                        throw new Exception(string.Format("AccPattern({0})程式尚未設定。", coaProfile.AccPattern));
                }

                docDetail.BUS_AREA = "";
                docDetail.ORDERID = "";
                docDetail.COSTCENTER = "";
                docDetail.PROFIT_CTR = coaProfile.ProfitCtr;
                docDetail.PMNTTREMS = "";

                docDetail.VALUE_DATE = poGrpInfo.DocDate;
                docDetail.BLINE_DATE = docDetail.VALUE_DATE;

                docDetail.PYMT_METH = "";
                docDetail.PMNT_BLOCK = "";
                docDetail.SP_GL_IND = "";

                docDetail.CURR_TYPE = "";
                docDetail.CURRENCY = "USD";

                //XI
                SetAccountsItemForXIType32(coaProfile, docDetail, poGrpInfo);

                return docDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetAccountsItemForXIType3(ChartOfAccountsProfile coaProfile, Sap_BapiAccDocument_DocDetail docDetail, CartGroupInfo cartGrpInfo, 
            Cart cartInfo, IQueryable<Process> procList, SapPriceInfo priceInfo)
        {
            string allocNmbr = "", itemText = "";
            decimal itemPrice = 0;

            string strCouponNo = GetCouponNo(cartInfo, procList.FirstOrDefault());
            string strProductID = procList.FirstOrDefault().ProductID.GetValueOrDefault().ToString();

            switch (coaProfile.DeliverTypeCode)
            {
                case 0:
                    #region -- 切貨 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;
                        case 5:
                        case 6:
                            allocNmbr = strCouponNo;
                            break;
                        case 7:
                            allocNmbr = cartInfo.SalesorderGroupID.ToString();
                            break;
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            allocNmbr = strProductID;
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }                    

                    //ITEM_TEXT
                    itemText = cartInfo.ID;

                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            //itemPrice = priceInfo.amount0;
                            itemPrice = priceInfo.SalesRevenue;
                            break;
                        case 2:
                            //itemPrice = priceInfo.amount2tax;
                            itemPrice = priceInfo.SalesTax;
                            break;                       
                        case 5:
                        case 6:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 7:
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                        case 8:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            itemPrice = priceInfo.PreAVGCostShipandTax;
                            break;
                        case 10:
                            itemPrice = priceInfo.PreAVGCostTaxandDuty;
                            break;
                        case 11:
                            itemPrice = priceInfo.PreAVGCostCustoms_Charge;
                            break;
                        case 12:
                            itemPrice = priceInfo.PreAVGCost + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            break;
                    }
                    #endregion
                    break;
                case 1:
                    #region -- 間配 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;                        
                        case 5:
                        case 6:
                            allocNmbr = strCouponNo;
                            break;
                        case 7:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        case 8:
                            //allocNmbr = this._finanRepoAdapter.GetNewEggInvoiceNo(cartInfo.ID);
                            //if (string.IsNullOrWhiteSpace(allocNmbr))
                            //    throw new Exception("Cart無美蛋InvoiceNO資料。");
                            allocNmbr = procList.FirstOrDefault().ProductID.GetValueOrDefault().ToString();
                            break;
                        case 9:
                        case 10:
                        case 11:
                            allocNmbr = strProductID;
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }

                    //ITEM_TEXT
                    itemText = cartInfo.ID;

                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            //itemPrice = priceInfo.amount0;
                            itemPrice = priceInfo.SalesRevenue;
                            break;
                        case 2:
                            //itemPrice = priceInfo.amount2tax;
                            itemPrice = priceInfo.SalesTax;
                            break;
                        case 5:
                        case 6:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 7:
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                        case 8:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            itemPrice = priceInfo.PreAVGCostShipandTax;
                            break;
                        case 10:
                            itemPrice = priceInfo.PreAVGCostTaxandDuty;
                            break;
                        case 11:
                            itemPrice = priceInfo.PreAVGCostCustoms_Charge;
                            break;
                        case 12:
                            itemPrice = priceInfo.PreAVGCost + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            break;
                    }
                    #endregion
                    break;
                case 3:
                    #region -- 三角 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;
                        case 8:
                        case 9:
                            allocNmbr = strCouponNo;
                            break;
                        case 10:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }

                    //ITEM_TEXT
                    itemText = cartInfo.ID;

                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            //itemPrice = ShippingExpensetemp;
                            itemPrice = priceInfo.BusinessShippingExpense;
                            break;
                        case 2:
                            //itemPrice = ServiceExpensetemp;
                            itemPrice = priceInfo.BusinessServiceExpense;
                            break;
                        case 3:
                            //itemPrice = Math.Round((priceInfo.InvoiceAmount - priceInfo.InvoiceAmount / (decimal)1.05) - (priceInfo.TaxExpense - priceInfo.TaxExpense / (decimal)1.05), 0, MidpointRounding.AwayFromZero);

                            //this.BusinessShippingExpense = Math.Round((this.ShippingExpense - this.ApportionedAmount) / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                            //this.BusinessServiceExpense = Math.Round(this.ServiceExpense / (decimal)1.05, 0, MidpointRounding.AwayFromZero);

                            //itemPrice = (含稅運費＋含稅服務費) - (未稅運費＋未稅服務費)
                            itemPrice = ((priceInfo.ShippingExpense - priceInfo.ApportionedAmount) + priceInfo.ServiceExpense) - (priceInfo.BusinessShippingExpense + priceInfo.BusinessServiceExpense);
                            break;
                        case 4:
                            //itemPrice = Process_TaxExpensetemp;
                            itemPrice = priceInfo.ImportTaxExpense;
                            break;
                        case 5:
                            //itemPrice = amount2tax_tax;
                            itemPrice = priceInfo.ImportTax;
                            break;
                        case 8:
                        case 9:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 10:
                            //itemPrice = priceInfo.ShippingExpense + priceInfo.ServiceExpense + priceInfo.Process_TaxExpense + priceInfo.InstallmentFee;
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                    }
                    #endregion
                    break;
                case 6:
                    #region -- 海外切貨-消費者報關 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;
                        case 7:
                        case 8:
                            allocNmbr = strCouponNo;
                            break;
                        case 9:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        case 10:
                            //allocNmbr = this._finanRepoAdapter.GetNewEggInvoiceNo(cartInfo.ID);
                            //if (string.IsNullOrWhiteSpace(allocNmbr))
                            //    throw new Exception("Cart無美蛋InvoiceNO資料。");
                            allocNmbr = procList.FirstOrDefault().ProductID.GetValueOrDefault().ToString();
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }

                    //ITEM_TEXT
                    itemText = cartInfo.ID;
                   
                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            itemPrice = priceInfo.SalesRevenue;
                            break;
                        case 2:
                            itemPrice = priceInfo.SalesTax;
                            break;
                        case 3:
                            //itemPrice = 
                            break;
                        case 4:
                            itemPrice = 0;
                            break;
                        case 7:
                        case 8:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 9:
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                        case 10:
                        case 11:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                    }
                    #endregion
                    break;
                case 7:
                    #region -- B2C直配 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;
                        case 5:
                        case 6:
                            allocNmbr = strCouponNo;
                            break;
                        case 7:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        case 8:
                            PurchaseOrder poInfo = this._finanRepoAdapter.GetOrderPO(cartInfo.ID);
                            //if (poItem == null)
                            //    throw new Exception("PurchaseOrder查無資料。");

                            allocNmbr = poInfo.Code;
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }

                    //ITEM_TEXT
                    itemText = cartInfo.ID;

                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            itemPrice = priceInfo.SalesRevenue;
                            break;
                        case 2:
                            itemPrice = priceInfo.SalesTax;
                            break;
                        case 5:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 6:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 7:
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                        case 8:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                    }
                    #endregion
                    break;
                case 9:
                    #region -- B2C寄倉 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                        case 2:
                            allocNmbr = cartInfo.InvoiceNO;
                            break;
                        case 5:
                        case 6:
                            allocNmbr = strCouponNo;
                            break;
                        case 7:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        case 8:
                            PurchaseOrder poInfo = this._finanRepoAdapter.GetOrderPO(cartInfo.ID);
                            //if (poInfo == null || string.IsNullOrWhiteSpace(poInfo.Code))
                            //    throw new Exception("PO單查無資料。");

                            allocNmbr = poInfo.Code;
                            break;
                        default: //LBO
                            allocNmbr = cartInfo.ID;
                            break;
                    }

                    //ITEM_TEXT
                    itemText = cartInfo.ID;

                    //AMT_DOCCUR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            itemPrice = priceInfo.SalesRevenue;
                            break;
                        case 2:
                            itemPrice = priceInfo.SalesTax;
                            break;
                        case 5:
                        case 6:
                            itemPrice = priceInfo.ApportionedAmount;
                            break;
                        case 7:
                            itemPrice = priceInfo.InvoiceAmount;
                            break;
                        case 8:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                    }
                    #endregion
                    break;
            }

            docDetail.ALLOC_NMBR = allocNmbr;
            docDetail.ITEM_TEXT = itemText;
            docDetail.AMT_DOCCUR = (coaProfile.SignFlag == "1" ? 1 : -1) * itemPrice;
        }

        private void SetAccountsItemForXIType31(ChartOfAccountsProfile coaProfile, Sap_BapiAccDocument_DocDetail docDetail, CartGroupInfo cartGrpInfo, 
            Cart cartInfo, SapPriceInfo priceInfo)
        {
            string allocNmbr = "", itemText = "";
            decimal itemPrice = 0;

            PurchaseOrderitemTWBACK poItemInfo = this._finanRepoAdapter.GetOrderPOItem(cartInfo.ID);
            if (poItemInfo == null)
                throw new Exception("PurchaseOrderitemTWBACK查無資料。");

            switch (coaProfile.DeliverTypeCode)
            {
                case 3:
                    #region -- 三角-美金 --
                    //ALLOC_NMBR
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            //allocNmbr = poItemInfo.PurchaseorderCode;
                            //allocNmbr = this._finanRepoAdapter.GetNewEggInvoiceNo(cartGrpInfo.SalesOrderList.First().ID);
                            allocNmbr = poItemInfo.InvoiceNO;
                            break;
                        case 2:
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                    }

                    //ITEM_TEXT
                    switch (coaProfile.ItemNo)
                    {
                        case 1:
                            /*-- 美蛋SO/台蛋PO/台蛋LBO --*/
                            itemText = string.Format("{0}/{1}/{2}", poItemInfo.SellerorderCode, poItemInfo.PurchaseorderCode, cartInfo.ID);
                            break;
                        case 2: //LBO
                            itemText = cartInfo.ID;
                            break;
                    }

                    //AMT_DOCCUR
                    itemPrice = poItemInfo.SourcePrice * poItemInfo.Qty;
                    #endregion
                    break;
            }

            docDetail.ALLOC_NMBR = allocNmbr;
            docDetail.ITEM_TEXT = itemText;
            docDetail.AMT_DOCCUR = (coaProfile.SignFlag == "1" ? 1 : -1) * itemPrice;
        }

        private void SetAccountsItemForXIType32(ChartOfAccountsProfile coaProfile, Sap_BapiAccDocument_DocDetail docDetail, PurchaseOrderItemGroupInfo poGrpInfo)
        {
            string allocNmbr = "", itemText = "";
            decimal itemPrice = 0, localPrice = 0;

            //PurchaseOrderitemTWBACK poItemInfo = this._finanRepoAdapter.GetOrderPOItem(cartInfo.ID);
            //if (poItemInfo == null)
            //    throw new Exception("PurchaseOrderitemTWBACK查無資料。");

            PurchaseOrderitemTWBACK poItemFirstInfo = poGrpInfo.PurchaseOrderItemList.First();

            #region -- 海外切貨-美金 --
            //ALLOC_NMBR
            allocNmbr = poItemFirstInfo.InvoiceNO;
            
            //AMT_DOCCUR
            //itemPrice = poItemInfo.SourcePrice * poItemInfo.Qty;
            foreach (PurchaseOrderitemTWBACK tmpInfo in poGrpInfo.PurchaseOrderItemList)
            {
                //美金
                itemPrice += (tmpInfo.SourcePrice * tmpInfo.Qty);
                //台幣
                localPrice += (tmpInfo.LocalPrice * tmpInfo.Qty);
            }

            localPrice = Math.Round(localPrice, 0, MidpointRounding.AwayFromZero);
           
            //ITEM_TEXT
            /*-- 美蛋SO/台蛋PO/台幣 --*/
            itemText = string.Format("{0}/{1}/{2}",
                poItemFirstInfo.SellerorderCode, poItemFirstInfo.PurchaseorderCode, (coaProfile.SignFlag == "1" ? 1 : -1) * localPrice);
            #endregion

            docDetail.ALLOC_NMBR = allocNmbr;
            docDetail.ITEM_TEXT = itemText;
            docDetail.AMT_DOCCUR = (coaProfile.SignFlag == "1" ? 1 : -1) * itemPrice;
        }      

        private string GetCouponNo(Cart cartInfo, Process procInfo)
        {            
            string strCouponNo = "";         

            //if (procList.Sum(x => (x.Pricecoupon ?? 0)) != 0 || procList.Sum(x => (x.ApportionedAmount)) != 0)
            if (procInfo.Pricecoupon.GetValueOrDefault() > 0 || procInfo.ApportionedAmount > 0)
            {
                int Coupon_NOint = 0;
                if (!string.IsNullOrWhiteSpace(procInfo.Coupons))
                {
                    List<int> Coupon_NOintList = new List<int>();
                    procInfo.Coupons.Split(',').ToList().ForEach(x => Coupon_NOintList.Add(Convert.ToInt32(x)));
                    Coupon_NOint = Coupon_NOintList.FirstOrDefault();
                    //Coupon_eventid = db_before.Coupon.Where(x => x.id == Coupon_NOint).Select(x => x.eventid).FirstOrDefault();

                    Coupon couponInfo = this._finanRepoAdapter.GetCoupon(Coupon_NOint);
                    //strCouponNo = couponInfo.eventid + "_" + (procInfo.Coupons ?? "");
                    strCouponNo = couponInfo.eventid.ToString();
                }
                else if (procInfo.ApportionedAmount != 0m)
                {
                    //string ProcessStringtemp = eachProcessgroupbyCartListitemitem.Process_ID;
                    //PromotionGiftRecords PromotionGiftRecordsitem = db_before.PromotionGiftRecords.Where(x => x.SalesOrderItemCode == procInfo.ID).FirstOrDefault();

                    PromotionGiftRecords giftInfo = this._finanRepoAdapter.GetPromotionGiftRecord(procInfo.ID);
                    if (giftInfo != null)
                    {
                        //Coupon_eventid = db_before.Coupon.Where(x => x.id == Coupon_NOint).Select(x => x.eventid).FirstOrDefault();
                        //strCouponNo = "P" + giftInfo.PromotionGiftBasicID + "_" + (giftInfo.SalesOrderItemCode ?? "");
                        strCouponNo = "P" + giftInfo.PromotionGiftBasicID;
                    }
                }
            }

            return strCouponNo;
        }

    }
}
