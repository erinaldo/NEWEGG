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
        private Sap_BapiAccDocument_DocHeader CreateSAPDocHeaderForXIRMA(AccountsDocumentType accDocTypeInfo, CartGroupInfo cartGrpInfo, FinanceDataListFinanceData twNewEggData, 
            BankAccountsInfo bankInfo, int shipType)
        {
            Sap_BapiAccDocument_DocHeader docHeader = new Sap_BapiAccDocument_DocHeader();

            FinanceDocumentCreateNote finanDocCreNoteInfo = null;
            string strDocType = DocTypeEnum.XI.ToString(); //docType.ToString();

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

                IQueryable<Process> procList = this._finanRepoAdapter.GetCartProcess(cartGrpInfo.SalesOrderList.First().ID);
                if (procList.Count() == 0)
                    throw new Exception("Cart無Process資料。");

                //Seller sellerInfo = this._sellerFinanRepoAdapter.GetSellerVendor(procList.First().StoreID.GetValueOrDefault());
                //if (sellerInfo == null)
                //    throw new Exception("Cart.StoreID查無Seller資料。");

                //R_00(交易模式代號)_統一發票號碼(10碼)
                docHeader.HEADER_TXT = string.Format("R_{0}_{1}",
                    shipType.ToString().PadLeft(2, '0'), cartGrpInfo.SalesOrderList.First().InvoiceNO);
                
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
                docHeader.REF_DOC_NO = "R_" + cartGrpInfo.SalesOrderGroupID.ToString(); //cartFirstInfo.SalesorderGroupID.GetValueOrDefault().ToString();

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

        private Sap_BapiAccDocument_DocDetail CreateSAPDocDetailForXIRMA(Sap_BapiAccDocument_DocHeader docHeader, ChartOfAccountsProfile coaProfile, CartGroupInfo cartGrpInfo,
            Cart cartInfo, BankAccountsInfo bankInfo, int itemNoAcc, SapPriceInfo priceInfo)
        {
            Sap_BapiAccDocument_DocDetail docDetail = new Sap_BapiAccDocument_DocDetail();
            //decimal dAmount = 0;

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
                        docDetail.VENDOR_ID = this._sellerFinanRepoAdapter.GetOrderVendorID(procList.FirstOrDefault().StoreID.GetValueOrDefault());
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
                docDetail.CURRENCY = "TWD";
                
                //XIRMA
                switch (coaProfile.AccDocTypeCode)
                {
                    case 4:
                        SetAccountsItemForXIRMAType4(coaProfile, docDetail, cartGrpInfo, cartInfo, procList, priceInfo);
                        break;
                    default:
                        throw new Exception(string.Format("ChartOfAccountsProfile.AccDocTypeCode: {0}，程式尚未設定。", coaProfile.AccDocTypeCode));
                    //break;
                }

                return docDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetAccountsItemForXIRMAType4(ChartOfAccountsProfile coaProfile, Sap_BapiAccDocument_DocDetail docDetail, CartGroupInfo cartGrpInfo,
            Cart cartInfo, IQueryable<Process> procList, SapPriceInfo priceInfo)
        {
            string allocNmbr = "", itemText = "";
            decimal itemPrice = 0;

            Retgood retgoodInfo = this._finanRepoAdapter.GetRetgoodByCartID(cartInfo.ID);
            string strRetgoodNo = retgoodInfo.Code;

            string itemNo = this._finanRepoAdapter.GetSellerProductID(cartInfo.ID);
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
                            allocNmbr = cartGrpInfo.SalesOrderGroupID.ToString();
                            break;
                        case 8:
                        case 9:
                        case 13:
                            allocNmbr = strRetgoodNo;
                            break;
                        case 10:
                        case 11:
                        case 12:
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
                            //itemPrice = priceInfo.PreAVGCost;
                            if (IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            //itemPrice = priceInfo.PreRetgoodPrice;
                            if (!IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 10:
                            itemPrice = priceInfo.PreAVGCostShipandTax;
                            break;
                        case 11:
                            itemPrice = priceInfo.PreAVGCostTaxandDuty;
                            break;
                        case 12:
                            itemPrice = priceInfo.PreAVGCostCustoms_Charge;
                            break;
                        case 13:
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
                        case 9:
                        case 13:
                            allocNmbr = strRetgoodNo;
                            break;
                        case 10:
                        case 11:
                        case 12:
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
                            if (IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 9:
                            if (!IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 10:
                            itemPrice = priceInfo.PreAVGCostShipandTax;
                            break;
                        case 11:
                            itemPrice = priceInfo.PreAVGCostTaxandDuty;
                            break;
                        case 12:
                            itemPrice = priceInfo.PreAVGCostCustoms_Charge;
                            break;
                        case 13:
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
                            itemPrice = priceInfo.BusinessShippingExpense;
                            break;
                        case 2:
                            itemPrice = priceInfo.BusinessServiceExpense;;
                            break;
                        case 3:
                            //itemPrice = Math.Round((priceInfo.InvoiceAmount - priceInfo.InvoiceAmount / (decimal)1.05) - (priceInfo.TaxExpense - priceInfo.TaxExpense / (decimal)1.05), 0, MidpointRounding.AwayFromZero);

                            //this.BusinessShippingExpense = Math.Round((this.ShippingExpense - this.ApportionedAmount) / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                            //this.BusinessServiceExpense = Math.Round(this.ServiceExpense / (decimal)1.05, 0, MidpointRounding.AwayFromZero);

                            //itemPrice = (含稅運費＋含稅服務費) - (未稅運費＋未稅服務費)
                            itemPrice = ((priceInfo.ShippingExpense - priceInfo.ApportionedAmount) + priceInfo.ServiceExpense) - (priceInfo.BusinessShippingExpense + priceInfo.BusinessServiceExpense);
                            break;
                        case 4:
                            itemPrice = priceInfo.ImportTaxExpense;
                            break;
                        case 5:
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
                        case 11:
                        case 12:
                            allocNmbr = strRetgoodNo;
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
                            itemPrice = 0;
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
                            if (IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 11:
                            if (!IsNewProduct(retgoodInfo))
                                itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 12:
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
                        case 9:
                            allocNmbr = strRetgoodNo;
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
                        case 10:
                            itemPrice = priceInfo.PreAVGCost;
                            break;
                        case 11:
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
                        case 9:
                            allocNmbr = strRetgoodNo;
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

        private bool IsNewProduct(Retgood retgoodInfo)
        {
            if ((retgoodInfo.ProductStatus ?? 0) == 0)
                return true;
            else
                return false;
        }
    }
}
