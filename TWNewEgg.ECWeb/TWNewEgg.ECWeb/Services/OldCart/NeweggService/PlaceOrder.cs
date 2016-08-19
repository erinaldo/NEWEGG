using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TWNewEgg.Website.ECWeb.Models;
using System.Data.SqlClient;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
//using JieMai.JieMaiAPI;
//using JieMai.com.jiemai.query;
using TWNewEgg.DB;
using TWNewEgg.ECWeb.Services.OldCart.NeweggService;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class PlaceOrder : IPlaceOrder
    {
        //JieMai.Service.JieMaiClass JieMaiServiceAPI;
        //ProductDetailResult productdetailinfo;
        log4net.ILog logger;
        CustomerInfo customerBillingInfoNeihu;
        CustomerInfo customerShippingInfoTamsui;
        CustomerInfo customerShippingInfoWH08;

        private int _salesOrderGroupID = -1;
        private int purchaseordergroup_id = -1;

        int 國際運費;
        int 服務費;

        string environment;
        string NeweggUSAServiceUri;
        string JieMaiServiceUri;
        string JieMaiQueryServiceUri;
        string JieMaiServiceKey;
        string IPPAddress;
        public PlaceOrder()
        {
            var TestXMLEXport = new TWNewEgg.GetConfigData.Service.CompanyInformation();
            TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA VENDORListVENDORDATA = new TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA();

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            //productdetailinfo = null;

            //設定
            this.environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            this.NeweggUSAServiceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_NeweggAPICreateSOService"];
            this.JieMaiServiceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPICreateSOService"];
            this.JieMaiQueryServiceUri = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPIQueryService"];
            this.JieMaiServiceKey = System.Configuration.ConfigurationManager.AppSettings[environment + "_JieMaiAPICreateSOServiceKey"];
            this.國際運費 = int.Parse(System.Configuration.ConfigurationManager.AppSettings[this.environment + "_NationalTransportFeeProductID"]);
            this.服務費 = int.Parse(System.Configuration.ConfigurationManager.AppSettings[this.environment + "_ServiceFeeProductID"]);
            this.IPPAddress = System.Configuration.ConfigurationManager.AppSettings[environment + "_IPP"];

            //Bill to 內湖
            VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("BILL", "");
            customerBillingInfoNeihu = new CustomerInfo();
            customerBillingInfoNeihu.ContactWith = VENDORListVENDORDATA.POContactWith;
            customerBillingInfoNeihu.Address1 = VENDORListVENDORDATA.ADDRESSENG1;
            customerBillingInfoNeihu.Address2 = VENDORListVENDORDATA.ADDRESSENG2;
            customerBillingInfoNeihu.State = VENDORListVENDORDATA.STATUS;
            customerBillingInfoNeihu.City = VENDORListVENDORDATA.CITY;
            customerBillingInfoNeihu.Country = VENDORListVENDORDATA.COUNTRY;
            customerBillingInfoNeihu.ZipCode = VENDORListVENDORDATA.POSTCODE;
            customerBillingInfoNeihu.CompanyName = VENDORListVENDORDATA.NAMEENG;
            customerBillingInfoNeihu.HomePhone = VENDORListVENDORDATA.TELFPO1;
            customerBillingInfoNeihu.Fax = VENDORListVENDORDATA.TELFAX;
            //Shipping to 淡水
            VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("SHIPPINGTOTW", "");
            customerShippingInfoTamsui = new CustomerInfo();
            customerShippingInfoTamsui.ContactWith = VENDORListVENDORDATA.POContactWith;
            customerShippingInfoTamsui.Address1 = VENDORListVENDORDATA.ADDRESSENG1;
            customerShippingInfoTamsui.Address2 = VENDORListVENDORDATA.ADDRESSENG2;
            customerShippingInfoTamsui.State = VENDORListVENDORDATA.STATUS;
            customerShippingInfoTamsui.City = VENDORListVENDORDATA.CITY;
            customerShippingInfoTamsui.Country = VENDORListVENDORDATA.COUNTRY;
            customerShippingInfoTamsui.ZipCode = VENDORListVENDORDATA.POSTCODE;
            customerShippingInfoTamsui.CompanyName = VENDORListVENDORDATA.NAMEENG;
            customerShippingInfoTamsui.HomePhone = VENDORListVENDORDATA.TELFPO1;
            customerShippingInfoTamsui.Fax = VENDORListVENDORDATA.TELFAX;
            //Shipping to WH08
            VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("SHIPPINGTOWH08", "");
            customerShippingInfoWH08 = new CustomerInfo();
            customerShippingInfoWH08.ContactWith = VENDORListVENDORDATA.POContactWith;
            customerShippingInfoWH08.Address1 = VENDORListVENDORDATA.ADDRESSENG1;
            customerShippingInfoWH08.Address2 = VENDORListVENDORDATA.ADDRESSENG2;
            customerShippingInfoWH08.State = VENDORListVENDORDATA.STATUS;
            customerShippingInfoWH08.City = VENDORListVENDORDATA.CITY;
            customerShippingInfoWH08.Country = VENDORListVENDORDATA.COUNTRY;
            customerShippingInfoWH08.ZipCode = VENDORListVENDORDATA.POSTCODE;
            customerShippingInfoWH08.CompanyName = VENDORListVENDORDATA.NAMEENG;
            customerShippingInfoWH08.HomePhone = VENDORListVENDORDATA.TELFPO1;
            customerShippingInfoWH08.Fax = "";
        }

        private int GeneratePurchaseOrderGroup(int salesOrderGroupID)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();

            SalesOrderGroup soGroup = db_before.SalesOrderGroup.Where(x => x.ID == salesOrderGroupID).FirstOrDefault();
            if (soGroup == null)
            {
                throw new Exception("SalesOrderGroup不存在");
            }
            else
            {
                //檢查是否已經複製過此筆SalseOrderGroup
                var soCodeList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == soGroup.ID).Select(x => x.Code).ToList();
                var poList = db_before.PurchaseOrder.Where(x => soCodeList.Contains(x.SalesorderCode)).ToList();
                int pogID = 0;
                var po = poList.Where(x => x.PurchaseorderGroupID != null && x.PurchaseorderGroupID > 0).FirstOrDefault();
                if (po != null)
                {
                    if (po.PurchaseorderGroupID.HasValue)
                    {
                        pogID = po.PurchaseorderGroupID.Value;
                    }
                }
                if (pogID == 0)
                {
                    //複製SalseOrderGroup 到 PurchaseOrderGroup
                    PurchaseOrderGroup poGroup = new PurchaseOrderGroup();
                    poGroup.Vaccunt = soGroup.Vaccunt;
                    poGroup.PriceSum = soGroup.PriceSum;
                    poGroup.OrderNum = soGroup.OrderNum;
                    poGroup.Note = soGroup.Note;
                    poGroup.CreateUser = "system";
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    poGroup.CreateDate = dt;
                    poGroup.Updated = 0;
                    db_before.PurchaseOrderGroup.Add(poGroup);
                    db_before.SaveChanges();
                    pogID = poGroup.ID;
                }
                return pogID;
            }
        }

        private string GeneratePurchaseOrder(string SONumber)
        {
            logger.Info("GeneratePurchaseOrder(" + SONumber + ") Start");
            TWSqlDBContext db_before = new TWSqlDBContext();

            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                SalesOrder order = db_before.SalesOrder.Where(x => x.Code == SONumber).FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("SalesOrder不存在");
                }
                else
                {
                    var TestXMLEXport = new TWNewEgg.GetConfigData.Service.CompanyInformation();
                    TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA VENDORListVENDORDATA = new TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA();
                    VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("SHIPPINGTOTW", "");
                    //複製SalseOrder 到 PurchaseOrder
                    PurchaseOrder po = new PurchaseOrder();
                    // LBO、FBO : L為Local指本地，F為Foreign指國外，LBO、LMO : B:B2B、M:MKPL?，O為Order
                    // USBO、CNBO : 第一個US、CN為國家，USMO、CNMO : B:B2B、M:MKPL?，O為Order
                    // 目前暫時不分美蛋與借賣，直接使用SellerID做分別
                    po.Code = db_before.Database.SqlQuery<string>("select  dbo.fn_EC_GetPurchaseOrderAutoSN(@0) ", new SqlParameter("@0", "USBO")).FirstOrDefault();
                    po.SalesorderCode = order.Code;
                    po.PurchaseorderGroupID = this.purchaseordergroup_id;
                    if (order.DelivType == 3)
                    {
                        //海外直購(管制商品、三角)
                        //直接使用 SO 資料
                        po.IDNO = order.IDNO;
                        po.Name = order.Name;
                        po.AccountID = order.AccountID;
                        po.TelDay = order.TelDay;
                        po.TelNight = order.TelNight;
                        po.Mobile = order.Mobile;
                        po.Email = order.Email;
                        po.PayType = order.PayType;
                        po.StarvlDate = order.StarvlDate;
                        po.CardHolder = order.CardHolder;
                        po.CardTelDay = order.CardTelDay;
                        po.CardTelNight = order.CardTelNight;
                        po.CardMobile = order.CardMobile;
                        po.CardLOC = order.CardLOC;
                        po.CardZip = order.CardZip;
                        po.CardADDR = order.CardADDR;
                        po.CardNO = order.CardNo;
                        po.CardNOCHK = order.CardNochk;
                        po.CardType = order.CardType;
                        po.CardBank = order.CardBank;
                        po.CardExpire = order.CardExpire;
                        po.CardBirthday = order.CardBirthday;
                        po.InvoReceiver = order.InvoiceReceiver;
                        po.Invoid = order.InvoiceID;
                        po.InvoTitle = order.InvoiceTitle;
                        po.InvoLOC = order.InvoiceLoc;
                        po.InvoZip = order.InvoiceZip;
                        po.InvoADDR = order.InvoiceAddr;
                        po.RecvName = order.RecvName;
                        po.RecvENGName = order.RecvEngName;
                        po.RecvTelDay = order.RecvTelDay;
                        po.RecvTelNight = order.RecvTelNight;
                        po.RecvMobile = order.RecvMobile;
                        po.DelivType = order.DelivType;
                        po.DelivData = order.DelivData;
                        po.DelivLOC = order.DelivLOC;
                        po.DelivZip = order.DelivZip;
                        po.DelivADDR = order.DelivADDR;
                        po.DelivENGADDR = order.DelivEngADDR;
                        po.DelivHitNote = order.DelivHitNote;
                        po.ConfirmDate = order.ConfirmDate;
                        po.ConfirmNote = order.ConfirmNote;
                        po.AuthDate = order.AuthDate;
                        po.AuthCode = order.AuthCode;
                        po.AuthNote = order.AuthNote;
                        po.HpType = order.HpType;
                        po.RcptDate = order.CreateDate;
                        po.RcptNote = order.RcptNote;
                        po.Expire = order.Expire;
                        po.DateDEL = order.DateDEL;
                        po.CoserverName = order.CoServerName;
                        po.ServerName = order.ServerName;
                        po.ActCode = order.ActCode;
                        po.Status = (int)PurchaseOrder.status.初始狀態;
                        po.StatusNote = order.StatusNote;
                        po.RemoteIP = order.RemoteIP;
                        po.Date = order.Date;
                        po.Note = order.Note;
                        po.Note2 = order.Note2;
                        po.CreateUser = order.CreateUser;
                    }
                    else
                    {
                        //非海外直購(管制商品、三角)
                        po.IDNO = "";
                        po.Name = VENDORListVENDORDATA.NAME;
                        po.AccountID = 0;
                        po.TelDay = customerShippingInfoTamsui.HomePhone;
                        po.TelNight = customerShippingInfoTamsui.HomePhone;
                        po.Mobile = "";
                        po.Email = "";
                        po.PayType = null;
                        po.StarvlDate = null;
                        po.CardHolder = "";
                        po.CardTelDay = "";
                        po.CardTelNight = "";
                        po.CardMobile = "";
                        po.CardLOC = "";
                        po.CardZip = "";
                        po.CardADDR = "";
                        po.CardNO = "";
                        po.CardNOCHK = "";
                        po.CardType = "";
                        po.CardBank = "";
                        po.CardExpire = "";
                        po.CardBirthday = null;
                        po.InvoReceiver = customerBillingInfoNeihu.ContactWith;
                        po.Invoid = "";
                        po.InvoTitle = "";
                        po.InvoLOC = customerBillingInfoNeihu.City;
                        po.InvoZip = customerBillingInfoNeihu.ZipCode;
                        po.InvoADDR = customerBillingInfoNeihu.Address1 + " " + customerBillingInfoNeihu.Address2;
                        po.RecvName = customerShippingInfoTamsui.ContactWith;
                        po.RecvTelDay = customerShippingInfoTamsui.HomePhone;
                        po.RecvTelNight = customerShippingInfoTamsui.HomePhone;
                        po.RecvMobile = customerShippingInfoTamsui.HomePhone;
                        po.DelivType = order.DelivType;
                        po.DelivData = order.DelivData;
                        po.DelivLOC = customerShippingInfoTamsui.City;
                        po.DelivZip = customerShippingInfoTamsui.ZipCode;
                        po.DelivADDR = customerShippingInfoTamsui.Address1 + " " + customerShippingInfoTamsui.Address2;
                        po.DelivHitNote = order.DelivHitNote;
                        po.ConfirmDate = null;
                        po.ConfirmNote = order.ConfirmNote;
                        po.AuthDate = null;
                        po.AuthCode = "";
                        po.AuthNote = order.AuthNote;
                        po.HpType = null;
                        po.RcptDate = null;
                        po.RcptNote = order.RcptNote;
                        po.Expire = null;
                        po.DateDEL = null;
                        po.CoserverName = order.CoServerName;
                        po.ServerName = order.ServerName;
                        po.ActCode = "";
                        po.Status = (int)PurchaseOrder.status.初始狀態;
                        po.StatusNote = order.StatusNote;
                        po.RemoteIP = order.RemoteIP;
                        po.Date = null;
                        po.Note = order.Note;
                        po.Note2 = order.Note2;
                        po.CreateUser = "system";
                    }
                    po.CreateDate = DateTime.Now;
                    po.Updated = 0;
                    db_before.PurchaseOrder.Add(po);
                    db_before.SaveChanges();

                    //複製SalseOrderItem 到 PurchaseOrderItem
                    List<SalesOrderItem> soiList = db_before.SalesOrderItem.Where(x => x.SalesorderCode == order.Code).ToList();
                    //聚合SalesOrderItem，統計相同商品的訂購數量，聚合成一筆。
                    List<SalesOrderItem> groupList = new List<SalesOrderItem>();
                    List<int> productIDs = soiList.Select(x => x.ProductID).Distinct().ToList();
                    //存放商品統計後的數量 [key] : SalesOrderItem_Code [Value] : 訂購數量
                    Dictionary<string, int> sum = new Dictionary<string, int>();
                    for (int i = 0; i < productIDs.Count(); i++)
                    {
                        //計算相同商品的訂購數量
                        int total = 0;
                        List<SalesOrderItem> temp = soiList.Where(x => x.ProductID == productIDs[i]).ToList();
                        for (int k = 0; k < temp.Count(); k++)
                        {
                            total += temp[k].Qty;
                        }

                        //同商品取第一筆SalesOrderItem加入至groupList做為聚合後的結果
                        groupList.Add(temp[0]);
                        //儲存統計後的加總數量
                        sum[temp[0].Code] = total;
                    }
                    //根據聚合後的結果集groupList寫入資料庫
                    foreach (SalesOrderItem soi in groupList)
                    {
                        PurchaseOrderItem tempPOI = new PurchaseOrderItem();
                        Product product = db_before.Product.Where(x => x.ID == soi.ProductID).FirstOrDefault();
                        tempPOI.Code = db_before.Database.SqlQuery<string>("select  dbo.fn_EC_GetPurchaseOrderitemAutoSN(@0) ", new SqlParameter("@0", "USBS")).FirstOrDefault();
                        tempPOI.PurchaseorderCode = po.Code;
                        tempPOI.SalesOrderItemCode = soi.Code;

                        tempPOI.ItemID = soi.ItemID;
                        tempPOI.ItemlistID = soi.ItemlistID;
                        tempPOI.ProductID = soi.ProductID;
                        tempPOI.ProductlistID = soi.ProductlistID;
                        tempPOI.SellerID = product.SellerID;
                        tempPOI.Name = soi.Name;
                        Seller seller = db_before.Seller.Where(x => x.ID == product.SellerID).FirstOrDefault();
                        string CurrencyYear = DateTime.Now.Year.ToString();
                        string CurrencyMonth = DateTime.Now.Month.ToString();
                        Currency currency = null;
                        currency = db_before.Currency.Where(x => x.CountryID == seller.CountryID && x.Year == CurrencyYear && x.Month == CurrencyMonth).FirstOrDefault();
                        if (currency == null)
                        {
                            logger.Error("Currency[匯率] Table中， CountryID[" + seller.CountryID + "] " + CurrencyYear + "/" + CurrencyMonth + "不存在，請補上此匯率");
                        }

                        tempPOI.LocalPriceinst = currency.AverageexchangeRate;
                        tempPOI.Price = (decimal)product.Cost;
                        //貨物稅
                        tempPOI.ProductTax = null;
                        //關稅
                        tempPOI.DutyRate = product.TradeTax;
                        //ccc
                        if (product.SourceTable.ToLower() == "productfromws")
                        {
                            ProductFromWS pfws = db_before.ProductFromWS.Where(x => x.ItemNumber == product.SellerProductID).FirstOrDefault();
                            if (pfws != null)
                            {
                                tempPOI.CCCCode = pfws.CCC;
                            }
                        }
                        if (product.SourceTable.ToLower() == "productfromjiemai")
                        {
                            ProductFromJieMai pfJM = db_before.ProductFromJieMai.Where(x => x.ID == product.FK).FirstOrDefault();
                            if (pfJM != null)
                            {
                                tempPOI.CCCCode = pfJM.CCC;
                            }
                        }
                        tempPOI.Priceinst = soi.Priceinst;
                        tempPOI.Qty = sum[soi.Code];//統計加總數量
                        tempPOI.PriceCoupon = soi.Pricecoupon;
                        tempPOI.RedmtkOut = soi.RedmtkOut;
                        tempPOI.RedmBLN = soi.RedmBLN;
                        tempPOI.RedmFDBCK = soi.Redmfdbck;
                        tempPOI.Status = (int)PurchaseOrderItem.status.初始狀態;
                        tempPOI.StatusNote = soi.StatusNote;
                        tempPOI.Date = soi.Date;
                        tempPOI.Attribs = soi.Attribs;
                        tempPOI.Note = soi.Note;
                        tempPOI.WftkOut = soi.WftkOut;
                        tempPOI.WfBLN = soi.WfBLN;
                        tempPOI.AdjPrice = soi.AdjPrice;
                        tempPOI.ActID = soi.ActID;
                        tempPOI.ActtkOut = soi.ActtkOut;
                        tempPOI.ProdcutCostID = soi.ProdcutCostID;
                        tempPOI.CreateUser = "system";
                        tempPOI.CreateDate = DateTime.Now;
                        tempPOI.Updated = 0;
                        tempPOI.SupplyShippingCharge = soi.SupplyShippingCharge;
                        db_before.PurchaseOrderItem.Add(tempPOI);
                        db_before.SaveChanges();
                        //產生PurchaseOrderItem後更新售價
                        int pid = 0;
                        if (tempPOI.ProductID > tempPOI.ProductlistID)
                        {
                            pid = tempPOI.ProductID;
                        }
                        else
                        {
                            pid = tempPOI.ProductlistID;
                        }
                        //logger.Info("CheckItemPrice(" + pid.ToString() + ") Start");
                        if (product.SourceTable.ToLower() == "productfromws")
                        {
                            new Service.ItemService().CheckItemPriceNeweggUSA(pid);
                        }
                        //if (product.SourceTable.ToLower() == "productfromjiemai") // 暫時拿掉，若借賣網付款資訊取得與Product.Cost有關或修改就得加回來
                        //{
                        //    try
                        //    {
                        //        string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                        //        string turnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemPriceNeweggUSA"].ToLower();
                        //        if (turnON == "on")
                        //        {
                        //            ProductDetailResult productdetail = new ProductDetailResult();
                        //            JieMaiServiceAPI = new JieMai.Service.JieMaiClass(this.JieMaiServiceUri, this.JieMaiQueryServiceUri, this.JieMaiServiceKey);
                        //            List<string> SKUs = new List<string>();
                        //            //嘗試取得商品資訊
                        //            SKUs.Add(product.SellerProductID);
                        //            productdetail = JieMaiServiceAPI.GetProductDetail(SKUs);
                        //            new Service.ItemService().CheckItemPriceJieMai(product, productdetail);
                        //        }
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        logger.Error("ItemID : " + tempPOI.ItemID + " Error : " + e.Message + "_" + e.StackTrace);
                        //        throw new Exception("ItemID : " + tempPOI.ItemID + " Error : " + e.Message + "_" + e.StackTrace);
                        //    }
                        //}
                        //logger.Info("CheckItemPrice(" + pid.ToString() + ") End");
                    }

                    //資料庫交易正確完成
                    //logger.Info("Transaction Complete Start");
                    ts.Complete();
                    //logger.Info("Transaction Complete End");
                    logger.Info("GeneratePurchaseOrder(" + SONumber + ") End");
                    return po.Code;
                }
            }
        }

        public string SendPlaceOrder(int salesOrderGroupID, string salesOrderCode)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            // 避免重複產生PO與POItem 在此先做PO是否已產生過的檢察
            PurchaseOrder searchPurchaseOrder = db_before.PurchaseOrder.Where(x => x.SalesorderCode == salesOrderCode).FirstOrDefault();
            if (searchPurchaseOrder != null)
            {
                logger.Error("該PurchaseOrder存在不再重複拋單 : PurchaseOrderCode [" + searchPurchaseOrder.Code + "]");
                return "";
            }
            SalesOrder so = db_before.SalesOrder.Where(x => x.Code == salesOrderCode).FirstOrDefault();
            //ShopOrderResult ShopOrderResults = new ShopOrderResult();
            if (so == null)
            {
                logger.Error("SalesOrder不存在: " + salesOrderCode);
                throw new Exception("SalesOrder不存在: " + salesOrderCode);
            }
            int delvType = (int)so.DelivType;

            //切貨自營 不產生PO也不進行拋單流程 直接回傳空字串
            if (delvType == (int)Item.tradestatus.切貨 || delvType == (int)Item.tradestatus.直配 || delvType == (int)Item.tradestatus.海外切貨)
            {
                logger.Error("切貨自營、直配、海外切貨 不產生PO也不進行拋單流程 直接回傳空字串: " + salesOrderCode);
                return "";
            }

            //過濾掉 國際運費、服務費 不產生PO也不進行拋單流程 直接回傳空字串
            int tempProductID = db_before.SalesOrderItem.Where(x => x.SalesorderCode == salesOrderCode).Select(x => x.ProductID).FirstOrDefault();
            if (tempProductID == 國際運費 || tempProductID == 服務費)
            {
                return "";
            }

            this._salesOrderGroupID = salesOrderGroupID;
            //產生 PurchaseOrderGroup
            this.purchaseordergroup_id = GeneratePurchaseOrderGroup(this._salesOrderGroupID);
            //產生PurchaseOrder
            string poCode = GeneratePurchaseOrder(salesOrderCode);

            PurchaseOrder po = db_before.PurchaseOrder.Where(x => x.Code == poCode).FirstOrDefault();
            if (po == null)
            {
                logger.Error("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
                throw new Exception("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
            }
             // 非間配與三角者不進行拋單流程 直接回傳空字串
            if (delvType != (int)Item.tradestatus.間配 && delvType != (int)Item.tradestatus.三角)
            {
                logger.Error("非間配與三角者不進行拋單流程 直接回傳空字串: " + salesOrderCode);
                return "";
            }

            if(po != null)
            {
                List<PurchaseOrderItem> poiList = db_before.PurchaseOrderItem.Where(x => x.PurchaseorderCode == po.Code).ToList();
                if (poiList.Count <= 0)
                {
                    logger.Error("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
                    throw new Exception("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
                }
                foreach (PurchaseOrderItem poi in poiList)
                {
                    int pid = 0;
                    if (poi.ProductID != 0)
                    {
                        pid = poi.ProductID;
                    }
                    else if (poi.ProductlistID != 0)
                    {
                        pid = poi.ProductlistID;
                    }
                    if (pid <= 0)
                    {
                        logger.Error("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
                        throw new Exception("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
                    }
                    string sellerOrderCode = "";
                    //string OrderID = "";
                    Product p = db_before.Product.Where(x => x.ID == pid).FirstOrDefault();
                    if (p == null)
                    {
                        logger.Error("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生異常，Product ID:" + pid.ToString() + " 不存在");
                    }
                    //如果是運費 則 product_sellerproductid 為 Null or 0
                    if (p.SellerProductID == null || p.SellerProductID == "" || p.SellerProductID.Length <= 0)
                    {
                        //跳過 運費項目 不需要拋轉訂單
                        continue;
                    }
                    else
                    {
                        try
                        {
                            float price = 0;
                            float.TryParse(poi.Price.ToString(), out price);
                            decimal shippingCharge = 0;
                            decimal.TryParse(poi.SupplyShippingCharge.ToString(), out shippingCharge);
                            switch (p.SourceTable.ToLower())
                            {
                                case "productfromws":
                                    switch (delvType)
                                    {
                                        //間配
                                        case 1:
                                            if (p.SellerProductID.IndexOf("9SI") >= 0)
                                            {
                                                //MKPL商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 501
                                                logger.Info("MKPL商品拋送訂單_間配_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", false);
                                            }
                                            else
                                            {
                                                //B2C商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 001
                                                //不需要CustomerOwnShippingAccount 361901878
                                                //SpecialComment [[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC07]]
                                                logger.Info("B2C商品拋送訂單_間配_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                string specialComment = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + warehouse + "]]";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "001", false, "", "", specialComment);
                                            }
                                            break;
                                        //海外直購(管制商品、三角)
                                        case 3:
                                            //Bill to Customer
                                            CustomerInfo customerBillingInfo = new CustomerInfo();
                                            customerBillingInfo.ContactWith = so.RecvEngName;
                                            string addr = so.DelivEngADDR;
                                            if (addr.Length > 40)
                                            {
                                                customerBillingInfo.Address1 = addr.Substring(0, 40).Replace("repdot", ",");
                                                customerBillingInfo.Address2 = addr.Substring(40).Replace("repdot", ",");
                                            }
                                            else
                                            {
                                                customerBillingInfo.Address1 = addr;
                                            }
                                            customerBillingInfo.State = "TW";
                                            customerBillingInfo.City = "Taiwan";
                                            customerBillingInfo.Country = "TWN";
                                            customerBillingInfo.ZipCode = so.DelivZip;
                                            customerBillingInfo.CompanyName = "";
                                            customerBillingInfo.HomePhone = so.Mobile;
                                            customerBillingInfo.Fax = "";
                                            //Ship to Customer
                                            CustomerInfo customerShippingInfo = new CustomerInfo();
                                            customerShippingInfo.ContactWith = so.RecvEngName;
                                            addr = so.DelivEngADDR;
                                            if (addr.Length > 40)
                                            {
                                                customerShippingInfo.Address1 = addr.Substring(0, 40);
                                                customerShippingInfo.Address2 = addr.Substring(40);
                                            }
                                            else
                                            {
                                                customerShippingInfo.Address1 = addr;
                                            }
                                            customerShippingInfo.State = "TW";
                                            customerShippingInfo.City = "Taiwan";
                                            customerShippingInfo.Country = "TWN";
                                            customerShippingInfo.ZipCode = so.DelivZip;
                                            customerShippingInfo.CompanyName = "";
                                            customerShippingInfo.HomePhone = so.Mobile;
                                            customerShippingInfo.Fax = "";
                                            if (p.SellerProductID.IndexOf("9SI") >= 0)
                                            {
                                                //MKPL商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 501
                                                logger.Info("MKPL商品拋送訂單_海外直購_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", true, "TWN Commission:2%");
                                            }
                                            else
                                            {
                                                //B2C商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 001
                                                //不需要CustomerOwnShippingAccount 361901878
                                                //SpecialComment [[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC07]]
                                                logger.Info("B2C商品拋送訂單_海外直購_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                string specialComment = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + warehouse + "]]";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "001", true, "TWN Commission:2%", "", specialComment);
                                            }
                                            break;
                                    }
                                    break;
                                //case "productfromjiemai":
                                //    switch (delvType)
                                //    {
                                //        //借賣網特別模式
                                //        case 4:
                                //            logger.Info("Step 1");
                                //            ShopOrderResults = SendToJieMai(p.SellerProductID, price, poi.Qty, poCode);
                                //            try
                                //            {
                                //                string SuccessOrFail = ShopOrderResults.status.ToLower();
                                //                if (SuccessOrFail == "fail")
                                //                {
                                //                    logger.Info("ShopOrderResults.失敗原因:" + ShopOrderResults.message);
                                //                }
                                //            }
                                //            catch { }
                                //            poi.Price = Convert.ToDecimal(ShopOrderResults.totalFee) / poi.Qty;
                                //            logger.Info("Step 2");
                                //            SaveJieMaiOrderInfo(ShopOrderResults);
                                //            //OrderID = ShopOrderResults.orderId;
                                //            try
                                //            {
                                //                logger.Info("ShopOrderResults.orderId:" + ShopOrderResults.orderId);
                                //            }
                                //            catch { }
                                //            logger.Info("Step 3");
                                //            //sellerOrderCode = ShopOrderResults.orderList[0];
                                //            sellerOrderCode = ShopOrderResults.jmOrderId;
                                //            //poi.Price = (decimal)(ShopOrderResults.totalFee / poi.qty);
                                //            logger.Info("Step 4");
                                //            CallBackEndSyncJeiMaiSO(); // 呼叫後台Steven 寫的 批次查詢JieMai的訂單狀態API
                                //            logger.Info("Step 5");
                                //            break;
                                //    }
                                //    break;
                                case "producttemp":
                                    switch (delvType)
                                    {
                                        //Local Vender B2C直配
                                        case 7:
                                            sellerOrderCode = "0";
                                            break;
                                    }
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            //write log
                            logger.Error("purchaseorderitem_code:" + poi.Code + " Error Message : " + e.Message + " Error StackTrace : " + e.StackTrace);
                            //將例外繼續往上層拋
                            throw e;
                        }
                    }
                    //訂單編號
                    poi.SellerOrderCode = sellerOrderCode;
                    //變更PO單狀態
                    if (sellerOrderCode != "")
                    {
                        poi.Status = (int)PurchaseOrderItem.status.已成立;
                        po.Status = (int)PurchaseOrder.status.已成立;
                    }
                    //搜尋匯率
                    try
                    {
                        Seller seller = db_before.Seller.Where(x => x.ID == p.SellerID).FirstOrDefault();
                        if (seller != null)
                        {
                            string year = DateTime.Now.Year.ToString();
                            string month = DateTime.Now.Month.ToString();
                            Currency currency = db_before.Currency.Where(x => x.Type == seller.CurrencyType && x.Year == year && x.Month == month).FirstOrDefault();
                            if (currency != null)
                            {
                                //打上 匯率 流水號
                                poi.CurrencyID = currency.ID;
                                poi.SourcecurrencyID = currency.ID;
                            }
                            else
                            {
                                //找不到 當期 匯率
                                logger.Fatal("CurrencyType:" + (seller.CurrencyType ?? "NULL") + " Year:" + year + " Month:" + month + " 找不到當期匯率");
                            }
                        }
                        else
                        {
                            //找不到 seller
                            logger.Fatal("ProductID:" + p.ID.ToString() + " SellerID:" + p.SellerID.ToString() + " 找不到Seller");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message + ((e.InnerException != null) ? e.InnerException.Message : ""));
                    }
                    //儲存
                    db_before.SaveChanges();
                }
            }
            return poCode;
        }

        //public void SaveJieMaiOrderInfo(ShopOrderResult ShopOrderResults)
        //{
        //    //log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //    //logger.Info("productdetail.orderId : " + ShopOrderResults.orderId);
        //    //logger.Info("productdetail.jmOrderId : " + ShopOrderResults.jmOrderId);
        //    //logger.Info("productdetail.message : " + ShopOrderResults.message);
        //    //logger.Info("productdetail.status : " + ShopOrderResults.status);
        //    //logger.Info("productdetail.totalFee : " + ShopOrderResults.totalFee);
        //    TWNewEgg.DB.TWBackendDBContext db_back = new TWNewEgg.DB.TWBackendDBContext();
        //    TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
        //    PurchaseOrder po = db_before.PurchaseOrder.Where(x => x.Code == ShopOrderResults.orderId).FirstOrDefault();
        //    //logger.Info("PurchaseOrder.Code : " + po.Code);
        //    //logger.Info("PurchaseOrder.SalesorderCode : " + po.SalesorderCode);
        //    SalesOrder SalesOrderInfo = db_before.SalesOrder.Where(x => x.Code == po.SalesorderCode).FirstOrDefault();
        //    logger.Info("將資訊儲存至[JieMaiOrderInfo] : " + SalesOrderInfo.Code);
        //    try
        //    {
        //        if (po != null && po.DelivType == (int)Item.tradestatus.國外直購)
        //        {
        //            JieMaiOrderInfo SearchJMOrderInfo = db_back.JieMaiOrderInfo.Where(x => x.SalesOrderCode == po.SalesorderCode).FirstOrDefault();
        //            //if (SearchJMOrderInfo == null) { logger.Info("SearchJMOrderInfo is null"); }
        //            //else { logger.Info("SearchJMOrderInfo is  not null : " + SearchJMOrderInfo.ID); }
        //            if (SearchJMOrderInfo == null)
        //            {
        //                JieMaiOrderInfo JMOrderInfo = new JieMaiOrderInfo();
        //                //logger.Info("PurchaseOrder.SalesorderCode : " + po.SalesorderCode);
        //                JMOrderInfo.SalesOrderCode = po.SalesorderCode;
        //                JMOrderInfo.UploadFlag = 0;
        //                //logger.Info("ShopOrderResults.orderId : " + ShopOrderResults.orderId);
        //                JMOrderInfo.OrderID = ShopOrderResults.orderId;
        //                //logger.Info("ShopOrderResults.jmOrderId : " + ShopOrderResults.jmOrderId);
        //                JMOrderInfo.JMOrderID = ShopOrderResults.jmOrderId;
        //                //logger.Info("ShopOrderResults.totalFee : " + (decimal)ShopOrderResults.totalFee);
        //                JMOrderInfo.TotalFee = (decimal)ShopOrderResults.totalFee;
        //                //logger.Info("PurchaseOrder.CreateDate : " + po.CreateDate);
        //                JMOrderInfo.CreateDate = po.CreateDate;
        //                JMOrderInfo.Updated = 0;
        //                //logger.Info("SalesOrder.Status : " + SalesOrderInfo.Status);
        //                JMOrderInfo.CartStatus = SalesOrderInfo.Status;
        //                JMOrderInfo.CreateDate = DateTime.Now;
        //                //logger.Info("SalesOrder.CreateDate : " + SalesOrderInfo.CreateDate);
        //                JMOrderInfo.CartCreateDate = SalesOrderInfo.CreateDate;
        //                db_back.JieMaiOrderInfo.Add(JMOrderInfo);
        //                db_back.SaveChanges();
        //                logger.Info("JieMaiOrderInfo資料建立成功 : " + po.SalesorderCode);
        //            }
        //            else
        //            {
        //                logger.Info("JieMaiOrderInfo資料已存在 : " + po.SalesorderCode);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Info("JieMaiOrderInfo資料建立失敗 : " + po.SalesorderCode + " Message : " + e.Message + " StrackTrace : " + e.StackTrace);
        //    }
        //}

        //public void SaveJieMaiOrderInfoV2(ShopOrderResult ShopOrderResults)
        //{
        //    TWNewEgg.DB.TWBackendDBContext db_back = new TWNewEgg.DB.TWBackendDBContext();
        //    TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
        //    string PurchaseOrderGroupID = ShopOrderResults.orderId.Replace("PGID", "");
        //    //PurchaseOrder po = db_before.PurchaseOrder.Where(x => x.PurchaseorderGroupID == PurchaseOrderGroupID).FirstOrDefault();
        //    //cart cartinfo = db_back.cart.Where(x => x.ID == po.SalesorderCode).FirstOrDefault();
        //    log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //    try
        //    {
        //        //if (po != null && po.DelivType == (int)Item.tradestatus.國外直購)
        //        //{
        //        JieMaiOrderInfo SearchJMOrderInfo = db_back.JieMaiOrderInfo.Where(x => x.SalesOrderCode == PurchaseOrderGroupID).FirstOrDefault();
        //        if (SearchJMOrderInfo == null)
        //        {
        //            JieMaiOrderInfo JMOrderInfo = new JieMaiOrderInfo();
        //            JMOrderInfo.SalesOrderCode = ShopOrderResults.orderId.Replace("PGID", ""); // 之後要新增Purchaseordergroupid 取代現在的salesordercode
        //            JMOrderInfo.UploadFlag = 0;
        //            JMOrderInfo.OrderID = ShopOrderResults.orderId;
        //            JMOrderInfo.JMOrderID = ShopOrderResults.jmOrderId;
        //            JMOrderInfo.TotalFee = (decimal)ShopOrderResults.totalFee;
        //            JMOrderInfo.CreateDate = DateTime.Now;
        //            //JMOrderInfo.CartCreateDate = cartinfo.CreateDate;
        //            //JMOrderInfo.CreateDate = ""; // 待更新為以車拋單時，再思考放法
        //            //JMOrderInfo.CartStatus = "";
        //            JMOrderInfo.Updated = 0;
        //            db_back.JieMaiOrderInfo.Add(JMOrderInfo);
        //            db_back.SaveChanges();
        //            logger.Info("JieMaiOrderInfo資料建立成功 : " + ShopOrderResults.orderId);
        //        }
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Info("JieMaiOrderInfo資料建立失敗 : " + ShopOrderResults.orderId + " Message : " + e.Message + " StrackTrace : " + e.StackTrace);
        //    }
        //}

        public string SendToNeweggUSA()
        {
            return "do nothing";
            //====================================================
            //產品編號
            string sellerProductID = "9SIBG9D0367008";
            //採購價格
            float price = 988.00f;
            //運費
            decimal shippingCharge = 5.99m;
            //數量
            int qty = 3;
            //PO單號
            string poCode = "USPO14090200001";
            //倉庫代號
            string whNumber = "07";
            //is newegg flash ??
            bool isNeweggFlash = false;
            //====================================================
            //若為中蛋委託拋單，則收件人加註 (CN) 字樣
            //customerShippingInfoWH08.ContactWith = "Victor Chen (CN)";
            //====================================================
            //間配 9SI
            //return SendToNeweggUSA(sellerProductID, price, shippingCharge, qty, whNumber, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", false, "", "", "", isNeweggFlash);
            //間配 B2C
            //string specialComment1 = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + whNumber + "]]";
            //return SendToNeweggUSA(sellerProductID, price, shippingCharge, qty, whNumber, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "001", false, "", "", specialComment1, isNeweggFlash);
            //三角 9SI
            //return SendToNeweggUSA(sellerProductID, price, shippingCharge, qty, whNumber, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", true, "TWN Commission:2%", "", "", isNeweggFlash);
            //三角 B2C
            //string specialComment2 = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + whNumber + "]]";
            //return SendToNeweggUSA(sellerProductID, price, shippingCharge, qty, whNumber, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "001", true, "TWN Commission:2%", "", specialComment2, isNeweggFlash);
        }

        private string SendToNeweggUSA(string itemNumber, float price, decimal shippingCharge, int quantity, string WarehouseNumber, string refPONumber, CustomerInfo customerBillingInfo, CustomerInfo customerShippingInfo, string shipViaCode, bool isControlled, string commissionNote = "", string customerOwnShippingAccount = "", string specialComment = "", bool isNeweggFlash = false)
        {
            logger.Info("Send Start...ItemNumber:" + itemNumber);

            //依據美蛋的資料規則，運費若為0.01 則變更為0
            if (shippingCharge <= 0.01m)
            {
                shippingCharge = 0m;
            }

            Publish sendOrderInfo = new Publish();

            sendOrderInfo.schemaLocation = "http://soa.newegg.com/SOA/USA/InfrastructureService/V10/EcommercePubSubServiceNEG.OrderManagement.CreateSOForEDI.V10.xsd";
            sendOrderInfo.Subject = "CreateSOForEDI";
            sendOrderInfo.FromService = "http://soa.newegg.com/SOA/USA/InfrastructureService/V10/Ecommerce/PubSubService";
            sendOrderInfo.ToService = "http://soa.newegg.com/SOA/USA/OrderManagement/V10/SSL31/CreateSOForEDI";

            Message sendMessage = new Message();

            HeaderInfo sendHeaderInfo = new HeaderInfo();
            sendHeaderInfo.Namespace = "http://soa.newegg.com/OrderManagement";
            sendHeaderInfo.Version = "V10";
            sendHeaderInfo.Action = "CreateSOForEDI";
            sendHeaderInfo.Sender = "EDIPortal";
            sendHeaderInfo.CompanyID = 1003;
            sendHeaderInfo.Tag = "CreateSOForEDI";
            sendHeaderInfo.Language = "en-us";
            sendHeaderInfo.CountryCode = "USA";
            sendHeaderInfo.Description = commissionNote;
            sendHeaderInfo.OriginalSender = "";
            sendHeaderInfo.CallbackAddress = "";
            sendHeaderInfo.From = "";
            sendHeaderInfo.To = "";
            sendHeaderInfo.FromSystem = "";
            sendHeaderInfo.ToSystem = "";
            sendHeaderInfo.GlobalBusinessType = "Listing";
            sendHeaderInfo.TransactionCode = "UD-002-0-001";

            sendMessage.Header = sendHeaderInfo;

            BodyInfo sendBodyInfo = new BodyInfo();

            OrderInfoBase orderInfoBase;

            if (itemNumber.IndexOf("9SI") >= 0)
            {
                //WH08
                orderInfoBase = new OrderInfo9SI();
                OrderInfo9SI tempOrderInfo9SI = (OrderInfo9SI)orderInfoBase;
                tempOrderInfo9SI.SOType = 99999;
            }
            else
            {
                //淡水
                orderInfoBase = new OrderInfo();
                OrderInfo tempOrderInfo = (OrderInfo)orderInfoBase;
                tempOrderInfo.IsInternational = 1;
            }

            //is newegg flash ??
            orderInfoBase.IsNF = isNeweggFlash;

            orderInfoBase.EDIID = Guid.NewGuid().ToString();
            if (isControlled)
            {
                //管制商品 下單帳號 7535442
                orderInfoBase.CustomerNumber = 7535442;
            }
            else
            {
                //非管制商品 下單帳號 24672992
                orderInfoBase.CustomerNumber = 24672992;
            }
            orderInfoBase.SODate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.SOAmount = 0;
            orderInfoBase.ShipViaCode = shipViaCode;
            orderInfoBase.CustomerOwnShippingAccount = customerOwnShippingAccount;

            // 取得本機名稱
            string strHostName = System.Net.Dns.GetHostName();

            // 取得本機的IpHostEntry類別實體
            System.Net.IPHostEntry iphostentry = System.Net.Dns.GetHostEntry(strHostName);

            string ip = "";
            // 取得所有 IP 位址
            foreach (System.Net.IPAddress ipaddress in iphostentry.AddressList)
            {
                // 只取得IP V4的Address
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = ipaddress.ToString();
                    break;
                }
            }

            orderInfoBase.IPAddress = ip;
            orderInfoBase.CompanyCode = 1003;
            orderInfoBase.TaxRate = 0;
            orderInfoBase.CustomerPONumber = refPONumber;
            orderInfoBase.ReferenceSONumber = "";
            orderInfoBase.PayTermsCode = "011";
            orderInfoBase.SalesPostUser = "TWPM";
            orderInfoBase.SalesPostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.AcctPostUser = "TWPM";
            orderInfoBase.AcctPostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            orderInfoBase.SalesPerson = "TWPM";
            orderInfoBase.CreditCardCharged = 1;
            orderInfoBase.CreditCardVerifyMark = "G";
            orderInfoBase.SOMemo = "TWPM";
            orderInfoBase.SpecialComment = specialComment;

            //SellerInfo
            SellerInfo sendSellerInfo = new SellerInfo();

            if (itemNumber.IndexOf("9SI") >= 0)
            {
                //取得產品詳細資訊
                if (this.environment == "PRD")
                {
                    //正式PRD環境
                    Service.NeweggRequest nr = new NeweggRequest();
                    Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo productDetail = nr.GetProductDetail(itemNumber);
                    if (productDetail != null)
                    {
                        if (productDetail.SellerId != null)
                        {
                            sendSellerInfo.SellerID = productDetail.SellerId;
                        }
                        else
                        {
                            throw new Exception(" ItemNumber:" + itemNumber + " Seller資訊查詢失敗");
                        }
                        if (productDetail.SellerName != null)
                        {
                            sendSellerInfo.SellerName = productDetail.SellerName;
                        }
                        else
                        {
                            throw new Exception(" ItemNumber:" + itemNumber + " Seller資訊查詢失敗");
                        }
                    }
                    else
                    {
                        throw new Exception(" ItemNumber:" + itemNumber + " Seller資訊查詢失敗");
                    }
                }
                else
                {
                    //"AWE5";
                    //"Test_Sandbox_Taiwan_004";
                    sendSellerInfo.SellerID = "AWE5";
                    sendSellerInfo.SellerName = "Test_Sandbox_Taiwan_004";
                }
            }
            else
            {
                //非 9SI 商品 SellerID、SellerName 給予空字串
                sendSellerInfo.SellerID = "";
                sendSellerInfo.SellerName = "";
            }

            orderInfoBase.SellerInfo = sendSellerInfo;

            //MKTOrderInfo
            sendBodyInfo.MKTOrderInfo = orderInfoBase;

            //帳單地址
            sendBodyInfo.MKTOrderInfo.CustomerBillingInfo = customerBillingInfo;

            //商品配送地址
            sendBodyInfo.MKTOrderInfo.CustomerShippingInfo = customerShippingInfo;

            sendMessage.Body = sendBodyInfo;

            //訂購商品
            ItemInfo sendItem;

            if (itemNumber.IndexOf("9SI") >= 0)
            {
                //WH08
                sendItem = new ItemInfo();
            }
            else
            {
                //淡水
                sendItem = new ItemInfoExt();
                ItemInfoExt tempItemInfoExt = (ItemInfoExt)sendItem;
                //單價
                tempItemInfoExt.UnitPrice = price;
                //下單 倉庫07
                tempItemInfoExt.WarehouseNumber = WarehouseNumber;
            }

            sendItem.ItemNumber = itemNumber;
            sendItem.DefaultShippingCharge = shippingCharge;
            sendItem.Quantity = quantity;
            //計算總價，商品價格已內涵運費
            sendBodyInfo.MKTOrderInfo.SOAmount = (float)((decimal)price * (decimal)quantity);
            if (price <= 0 || sendBodyInfo.MKTOrderInfo.SOAmount <= 0)
            {
                //throw new Exception("商品價錢異常");
            }

            //放入商品資訊陣列
            ItemInfo[] sendItems = new ItemInfo[1];
            sendItems[0] = sendItem;

            sendBodyInfo.MKTOrderInfo.ItemList = sendItems;

            NodeList nodeList = new NodeList();
            nodeList.Message = sendMessage;

            sendOrderInfo.Node = nodeList;


            string serializeStr = Serialize(sendOrderInfo);

            //紀錄序列化產生的XML
            logger.Info("Write XML Log ItemNumber:" + itemNumber);
            string getNumber = sendBodyInfo.MKTOrderInfo.CustomerPONumber.Substring(6, sendBodyInfo.MKTOrderInfo.CustomerPONumber.Length - 7);
            int writeNumber = 0;
            int.TryParse(getNumber, out writeNumber);
            int writeSn = 0;
            writeSn = writeNumber / 10000;
            string cfolder = "USBO/" + writeSn.ToString();
            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder + "/placeOrderDetail_" + sendBodyInfo.MKTOrderInfo.CustomerPONumber + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml"));
            sw.Write(serializeStr);
            sw.Close();

            //送出到美蛋 WebService
            logger.Info("Send XML ItemNumber:" + itemNumber);
            string result = "";

            WebRequest req = HttpWebRequest.Create(this.NeweggUSAServiceUri);
            req.Timeout = 60 * 1000;
            req.Method = "POST";
            req.ContentType = "text/xml; charset=utf-8";
            req.Headers.Add("SOAPAction", "http://tempuri.org/IMessageProcessor/Process");
            string key = "";
            try
            {
                key = System.Configuration.ConfigurationManager.AppSettings[this.environment + "_NeweggAPICreateSOServiceKey"];
            }
            catch { }
            req.Headers.Add("Authorization", key);

            Stream w = req.GetRequestStream();
            StreamWriter swOut = new StreamWriter(w);
            swOut.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><Process xmlns=\"http://tempuri.org/\"><message>" + HttpContext.Current.Server.HtmlEncode(serializeStr) + "</message></Process></soap:Body></soap:Envelope>");
            swOut.Flush();
            Stream r = req.GetResponse().GetResponseStream();
            StreamReader srIn = new StreamReader(r);
            result = srIn.ReadToEnd();
            swOut.Close();
            srIn.Close();
            w.Close();
            r.Close();


            //紀錄回傳的XML
            logger.Info("Write Response XML Log ItemNumber:" + itemNumber);
            System.IO.StreamWriter sw_output = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("/log/placeOrderToUsByWs/" + cfolder + "/placeOrderDetail_" + sendBodyInfo.MKTOrderInfo.CustomerPONumber + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_output.xml"));
            sw_output.Write(result);
            sw_output.Close();

            if (result.IndexOf("&lt;SuccessFlag&gt;true&lt;/SuccessFlag&gt;") >= 0)
            {
                result = result.Substring(result.IndexOf("&lt;SONumber&gt;") + 16);
                result = result.Substring(0, result.IndexOf("&lt;"));
                logger.Info("Success ItemNumber:" + itemNumber + " SellerOrderCode:" + result);
                return result;
            }
            else
            {
                logger.Info("拋送訂單發生錯誤 ItemNumber:" + itemNumber + " [回傳資訊] " + result);
                throw new Exception("拋送訂單發生錯誤");
            }
        }

        //private ShopOrderResult SendToJieMai(string SellerProductID, float price, int Qty, string poCode)
        //{
        //    JieMai.JieMaiAPI.OrderItem OItem = new JieMai.JieMaiAPI.OrderItem();
        //    //JieMaiOrderItem.OrderItem OItem = new JieMaiOrderItem.OrderItem();
        //    try
        //    {
        //        var TestXMLEXport = new TWNewEgg.GetConfigData.Service.CompanyInformation();
        //        TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA VENDORListVENDORDATA = new TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA();
        //        VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("SHIPPINGTOTW", "");

        //        OItem.consigeneeName = VENDORListVENDORDATA.NAMEENG; // 收件人姓名
        //        //OItem.consigeneeName = "台灣新蛋國際電子商務平台股份有限公司"; // 收件人姓名
        //        OItem.consigeneeAddress = VENDORListVENDORDATA.ADDRESSENG1 + VENDORListVENDORDATA.ADDRESSENG2; // 收貨人地址
        //        //OItem.consigeneeAddress = "251新北市淡水區中正東路二段27-5號28樓"; // 收貨人地址
        //        OItem.consigeneeCountryCode = "CN"; // 收貨人國家代碼
        //        OItem.consigneeCity = VENDORListVENDORDATA.CITY; // 收貨人城市
        //        OItem.consigneeState = VENDORListVENDORDATA.STATUS; // 收貨人州
        //        OItem.ebayTrasactionId = ""; // eBay刊登號
        //        OItem.consigeneeEmail = VENDORListVENDORDATA.EMAIL; // 收貨人郵箱
        //        OItem.consigeneePhone = VENDORListVENDORDATA.TELFPO1; // 收貨人電話
        //        OItem.consigneePostCode = VENDORListVENDORDATA.POSTCODE; // 收貨人郵編
        //        OItem.warehouse = "SZZW"; // 倉庫
        //        OItem.shippingMethod = "JMZIL"; // 派送方式 // 自提
        //        OItem.orderSource = ""; // 訂單來源(默認不傳，如需請諮詢)
        //        OItem.isBuyInsurance = "0";
        //        OItem.isReturnProduct = "0";
        //        JieMai.JieMaiAPI.ItemSKUAndNum ItemSKUAndNums = new JieMai.JieMaiAPI.ItemSKUAndNum();
        //        OItem.arrItemSKUAndNum = new JieMai.JieMaiAPI.ItemSKUAndNum[1];
        //        OItem.arrItemSKUAndNum[0] = new JieMai.JieMaiAPI.ItemSKUAndNum(); // 產品SKU
        //        OItem.arrItemSKUAndNum[0].dealPrice = Convert.ToDouble(price); // 產品成交價 -- 價格需要修改，因為要用使用者購買時商品的價格
        //        OItem.arrItemSKUAndNum[0].num = Convert.ToInt32(Qty); // 數量
        //        OItem.arrItemSKUAndNum[0].dpCode = "JMZIL"; // 自提 // 該產品需要走的派送方式.1、如果為空，則取賣家設置的派送方式；2、如果未設置派送風是，則取系統默認。3、同倉庫多個物品，則生成一個訂單，且取第一個派送方式。
        //        OItem.arrItemSKUAndNum[0].productSku = SellerProductID;
        //        #region // 傳送資料XML收集
        //        //System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(JieMai.JieMaiAPI.OrderItem));
        //        //System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //        //xmlSerializer.Serialize(ms, OItem);
        //        //string xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());

        //        //WebRequest req = HttpWebRequest.Create(this.serviceUri);
        //        //req.Method = "POST";
        //        //req.ContentType = "text/xml; charset=utf-8";
        //        //req.Headers.Add("SOAPAction", "http://tempuri.org/IMessageProcessor/Process");
        //        //string key = "";
        //        //try
        //        //{
        //        //    key = System.Configuration.ConfigurationManager.AppSettings[this.environment + "_NeweggAPICreateSOServiceKey"];
        //        //}
        //        //catch { }
        //        //req.Headers.Add("Authorization", key);
        //        //
        //        //Stream w = req.GetRequestStream();
        //        //StreamWriter swOut = new StreamWriter(w);
        //        //swOut.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><addShopOrder xmlns=\"com.jmservice\"><usertoken>73b61435-9688-4a5e-aa67-558f560fe256</usertoken><orderId>USBO13120400003</orderId><shopOrderItem><arrItemSKUAndNum xmlns=\"http://vo.webservice.jmservice.com\"><ItemSKUAndNum><dpCode>ABC</dpCode><productSku>950004-JD-500-168-01-SZZW</productSku></ItemSKUAndNum></arrItemSKUAndNum><consigeneeAddress xmlns=\"http://vo.webservice.jmservice.com\">28F., No.27-5, Sec. 2, Zhongzheng E. Rd., Tamsui Dist., New Taipei City 251, Taiwan</consigeneeAddress><consigeneeCountryCode xmlns=\"http://vo.webservice.jmservice.com\">CN</consigeneeCountryCode><consigeneeEmail xmlns=\"http://vo.webservice.jmservice.com\">service@newegg.com.tw</consigeneeEmail><consigeneeName xmlns=\"http://vo.webservice.jmservice.com\">Newegg Taiwan</consigeneeName><consigeneePhone xmlns=\"http://vo.webservice.jmservice.com\">88-6280068010</consigeneePhone><consigneeCity xmlns=\"http://vo.webservice.jmservice.com\">Taiwan</consigneeCity><consigneePostCode xmlns=\"http://vo.webservice.jmservice.com\">25170</consigneePostCode><consigneeState xmlns=\"http://vo.webservice.jmservice.com\">TW</consigneeState><ebayTrasactionId xmlns=\"http://vo.webservice.jmservice.com\" /><isBuyInsurance xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><isReturnProduct xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><orderSource xmlns=\"http://vo.webservice.jmservice.com\" /><shippingMethod xmlns=\"http://vo.webservice.jmservice.com\">SGGH</shippingMethod><warehouse xmlns=\"http://vo.webservice.jmservice.com\">SZZW</warehouse></shopOrderItem><version>1.0</version></addShopOrder></soap:Body></soap:Envelope>");
        //        //swOut.Flush();
        //        //Stream r = req.GetResponse().GetResponseStream();
        //        //StreamReader srIn = new StreamReader(r);
        //        //string result = srIn.ReadToEnd();
        //        //swOut.Close();
        //        //srIn.Close();
        //        //w.Close();
        //        //r.Close();
        //        #endregion // 傳送資料XML收集
        //        JieMaiServiceAPI = new JieMai.Service.JieMaiClass(this.JieMaiServiceUri, this.JieMaiQueryServiceUri, this.JieMaiServiceKey);
        //        JieMai.JieMaiAPI.ShopOrderResult SOResult = new JieMai.JieMaiAPI.ShopOrderResult();
        //        SOResult = JieMaiServiceAPI.AddAndPayShopOrder(poCode, OItem); // 拋送訂單給借賣網
        //        string SuccessOrFail = SOResult.status.ToLower();
        //        if (SuccessOrFail == "fail")
        //        {
        //            throw new Exception(SOResult.message);
        //        }
        //        return SOResult;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Info("拋送訂單發生錯誤 ItemNumber:" + SellerProductID + " ErrorMessage :" + e.Message + "ErrorStackTrace : " + e.StackTrace);
        //        throw new Exception("拋送訂單發生錯誤 ItemNumber:" + SellerProductID + "_" + e.Message);
        //    }
        //}

        //private ShopOrderResult SendToJieMaiV2(List<PurchaseOrderItem> POItemList, int PurchaseOrderGroupID)
        //{
        //    DatabaseContext db_before = new DatabaseContext();
        //    JieMai.JieMaiAPI.OrderItem OItem = new JieMai.JieMaiAPI.OrderItem();
        //    List<int> ProductIDList = new List<int>();
        //    int pid = 0;
        //    foreach (PurchaseOrderItem poi in POItemList)
        //    {
        //        if (poi.ProductID != 0)
        //        {
        //            pid = poi.ProductID;
        //        }
        //        else if (poi.ProductlistID != 0)
        //        {
        //            pid = poi.ProductlistID;
        //        }
        //        if (pid <= 0)
        //        {
        //            logger.Error("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
        //            throw new Exception("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
        //        }
        //        ProductIDList.Add(pid);
        //    }
        //    List<Product> productList = db_before.Product.Where(x => ProductIDList.Contains(x.ID)).Distinct().ToList();
        //    try
        //    {
        //        var TestXMLEXport = new TWNewEgg.GetConfigData.Service.CompanyInformation();
        //        TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA VENDORListVENDORDATA = new TWNewEgg.GetConfigData.Models.CompanyInformation.VENDORListVENDORDATA();
        //        VENDORListVENDORDATA = TestXMLEXport.GetCompanyInformation("SHIPPINGTOTW", "");

        //        OItem.consigeneeName = VENDORListVENDORDATA.NAMEENG; // 收件人姓名
        //        //OItem.consigeneeName = "台灣新蛋國際電子商務平台股份有限公司"; // 收件人姓名
        //        OItem.consigeneeAddress = VENDORListVENDORDATA.ADDRESSENG1 + VENDORListVENDORDATA.ADDRESSENG2; // 收貨人地址
        //        //OItem.consigeneeAddress = "251新北市淡水區中正東路二段27-5號28樓"; // 收貨人地址
        //        OItem.consigeneeCountryCode = "CN"; // 收貨人國家代碼
        //        OItem.consigneeCity = VENDORListVENDORDATA.CITY; // 收貨人城市
        //        OItem.consigneeState = VENDORListVENDORDATA.STATUS; // 收貨人州
        //        OItem.ebayTrasactionId = ""; // eBay刊登號
        //        OItem.consigeneeEmail = VENDORListVENDORDATA.EMAIL; // 收貨人郵箱
        //        OItem.consigeneePhone = VENDORListVENDORDATA.TELFPO1; // 收貨人電話
        //        OItem.consigneePostCode = VENDORListVENDORDATA.POSTCODE; // 收貨人郵編
        //        OItem.warehouse = "SZZW"; // 倉庫
        //        OItem.shippingMethod = "JMZIL"; // 派送方式 // 自提
        //        OItem.orderSource = ""; // 訂單來源(默認不傳，如需請諮詢)
        //        OItem.isBuyInsurance = "0";
        //        OItem.isReturnProduct = "0";
        //        JieMai.JieMaiAPI.ItemSKUAndNum ItemSKUAndNums = new JieMai.JieMaiAPI.ItemSKUAndNum();
        //        OItem.arrItemSKUAndNum = new JieMai.JieMaiAPI.ItemSKUAndNum[POItemList.Count];
        //        int poiCount = 0;
        //        foreach (PurchaseOrderItem poi in POItemList)
        //        {
        //            if (poi.ProductID != 0)
        //            {
        //                pid = poi.ProductID;
        //            }
        //            else if (poi.ProductlistID != 0)
        //            {
        //                pid = poi.ProductlistID;
        //            }
        //            Product searchProduct = productList.Where(x => x.ID == pid).FirstOrDefault();
        //            if (searchProduct.SourceTable.ToLower() != "productfromjiemai") throw new Exception("非借賣網商品");
        //            OItem.arrItemSKUAndNum[poiCount] = new JieMai.JieMaiAPI.ItemSKUAndNum(); // 產品SKU
        //            OItem.arrItemSKUAndNum[poiCount].dealPrice = Convert.ToDouble(poi.Price); // 產品成交價 -- 價格需要修改，因為要用使用者購買時商品的價格
        //            OItem.arrItemSKUAndNum[poiCount].num = Convert.ToInt32(poi.Qty); // 數量
        //            OItem.arrItemSKUAndNum[poiCount].dpCode = "JMZIL"; // 自提 // 該產品需要走的派送方式.1、如果為空，則取賣家設置的派送方式；2、如果未設置派送風是，則取系統默認。3、同倉庫多個物品，則生成一個訂單，且取第一個派送方式。
        //            OItem.arrItemSKUAndNum[poiCount].productSku = searchProduct.SellerProductID;
        //            poiCount++;
        //        }
        //        #region // 傳送資料XML收集
        //        //System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(JieMai.JieMaiAPI.OrderItem));
        //        //System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //        //xmlSerializer.Serialize(ms, OItem);
        //        //string xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());

        //        //WebRequest req = HttpWebRequest.Create(this.serviceUri);
        //        //req.Method = "POST";
        //        //req.ContentType = "text/xml; charset=utf-8";
        //        //req.Headers.Add("SOAPAction", "http://tempuri.org/IMessageProcessor/Process");
        //        //string key = "";
        //        //try
        //        //{
        //        //    key = System.Configuration.ConfigurationManager.AppSettings[this.environment + "_NeweggAPICreateSOServiceKey"];
        //        //}
        //        //catch { }
        //        //req.Headers.Add("Authorization", key);
        //        //
        //        //Stream w = req.GetRequestStream();
        //        //StreamWriter swOut = new StreamWriter(w);
        //        //swOut.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><addShopOrder xmlns=\"com.jmservice\"><usertoken>73b61435-9688-4a5e-aa67-558f560fe256</usertoken><orderId>USBO13120400003</orderId><shopOrderItem><arrItemSKUAndNum xmlns=\"http://vo.webservice.jmservice.com\"><ItemSKUAndNum><dpCode>ABC</dpCode><productSku>950004-JD-500-168-01-SZZW</productSku></ItemSKUAndNum></arrItemSKUAndNum><consigeneeAddress xmlns=\"http://vo.webservice.jmservice.com\">28F., No.27-5, Sec. 2, Zhongzheng E. Rd., Tamsui Dist., New Taipei City 251, Taiwan</consigeneeAddress><consigeneeCountryCode xmlns=\"http://vo.webservice.jmservice.com\">CN</consigeneeCountryCode><consigeneeEmail xmlns=\"http://vo.webservice.jmservice.com\">service@newegg.com.tw</consigeneeEmail><consigeneeName xmlns=\"http://vo.webservice.jmservice.com\">Newegg Taiwan</consigeneeName><consigeneePhone xmlns=\"http://vo.webservice.jmservice.com\">88-6280068010</consigeneePhone><consigneeCity xmlns=\"http://vo.webservice.jmservice.com\">Taiwan</consigneeCity><consigneePostCode xmlns=\"http://vo.webservice.jmservice.com\">25170</consigneePostCode><consigneeState xmlns=\"http://vo.webservice.jmservice.com\">TW</consigneeState><ebayTrasactionId xmlns=\"http://vo.webservice.jmservice.com\" /><isBuyInsurance xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><isReturnProduct xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><orderSource xmlns=\"http://vo.webservice.jmservice.com\" /><shippingMethod xmlns=\"http://vo.webservice.jmservice.com\">SGGH</shippingMethod><warehouse xmlns=\"http://vo.webservice.jmservice.com\">SZZW</warehouse></shopOrderItem><version>1.0</version></addShopOrder></soap:Body></soap:Envelope>");
        //        //swOut.Flush();
        //        //Stream r = req.GetResponse().GetResponseStream();
        //        //StreamReader srIn = new StreamReader(r);
        //        //string result = srIn.ReadToEnd();
        //        //swOut.Close();
        //        //srIn.Close();
        //        //w.Close();
        //        //r.Close();
        //        #endregion // 傳送資料XML收集
        //        JieMaiServiceAPI = new JieMai.Service.JieMaiClass(this.JieMaiServiceUri, this.JieMaiQueryServiceUri, this.JieMaiServiceKey);
        //        JieMai.JieMaiAPI.ShopOrderResult SOResult = new JieMai.JieMaiAPI.ShopOrderResult();
        //        SOResult = JieMaiServiceAPI.AddAndPayShopOrder("PGID" + PurchaseOrderGroupID, OItem); // 拋送訂單給借賣網
        //        if (SOResult == null) throw new Exception("JM拋單失敗");
        //        string SuccessOrFail = SOResult.status.ToLower();
        //        if (SuccessOrFail == "fail")
        //        {
        //            throw new Exception(SOResult.message);
        //        }
        //        return SOResult;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Info("借賣網拋送訂單發生錯誤 PurchaseOrderGroupID :" + PurchaseOrderGroupID + " ErrorMessage :" + e.Message + "ErrorStackTrace : " + e.StackTrace);
        //        throw new Exception("借賣網拋送訂單發生錯誤 PurchaseOrderGroupID :" + PurchaseOrderGroupID + "_" + e.Message);
        //    }
        //}

        //private ShopOrderResult SendToJieMai(string SellerProductID, float price, int qty, string poCode)
        //{
        //    JieMai.JieMaiAPI.OrderItem OItem = new JieMai.JieMaiAPI.OrderItem();
        //    //JieMaiOrderItem.OrderItem OItem = new JieMaiOrderItem.OrderItem();
        //    try
        //    {
        //        OItem.consigeneeName = "Newegg Taiwan"; // 收件人姓名
        //        //OItem.consigeneeName = "台灣新蛋國際電子商務平台股份有限公司"; // 收件人姓名
        //        OItem.consigeneeAddress = "28F., No.27-5, Sec. 2, Zhongzheng E. Rd., Tamsui Dist., New Taipei City 251, Taiwan"; // 收貨人地址
        //        //OItem.consigeneeAddress = "251新北市淡水區中正東路二段27-5號28樓"; // 收貨人地址
        //        OItem.consigeneeCountryCode = "CN"; // 收貨人國家代碼
        //        OItem.consigneeCity = "Taiwan"; // 收貨人城市
        //        OItem.consigneeState = "TW"; // 收貨人州
        //        OItem.ebayTrasactionId = ""; // eBay刊登號
        //        OItem.consigeneeEmail = "service@newegg.com.tw"; // 收貨人郵箱
        //        OItem.consigeneePhone = "88-6280068010"; // 收貨人電話
        //        OItem.consigneePostCode = "25170"; // 收貨人郵編
        //        OItem.warehouse = "SZZW"; // 倉庫
        //        OItem.shippingMethod = "JMZIL"; // 派送方式 // 自提
        //        OItem.orderSource = ""; // 訂單來源(默認不傳，如需請諮詢)
        //        //OItem.isBuyInsurance = "0";
        //        //OItem.isReturnProduct = "0";
        //        JieMai.JieMaiAPI.ItemSKUAndNum ItemSKUAndNums = new JieMai.JieMaiAPI.ItemSKUAndNum();
        //        OItem.arrItemSKUAndNum = new JieMai.JieMaiAPI.ItemSKUAndNum[1];
        //        OItem.arrItemSKUAndNum[0] = new JieMai.JieMaiAPI.ItemSKUAndNum(); // 產品SKU
        //        OItem.arrItemSKUAndNum[0].dealPrice = Convert.ToDouble(price); // 產品成交價 -- 價格需要修改，因為要用使用者購買時商品的價格
        //        OItem.arrItemSKUAndNum[0].num = Convert.ToInt32(qty); // 數量
        //        OItem.arrItemSKUAndNum[0].dpCode = "JMZIL"; // 自提 // 該產品需要走的派送方式.1、如果為空，則取賣家設置的派送方式；2、如果未設置派送風是，則取系統默認。3、同倉庫多個物品，則生成一個訂單，且取第一個派送方式。
        //        OItem.arrItemSKUAndNum[0].productSku = SellerProductID;
        //        #region // 傳送資料XML收集
        //        //System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(JieMai.JieMaiAPI.OrderItem));
        //        //System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //        //xmlSerializer.Serialize(ms, OItem);
        //        //string xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());

        //        //WebRequest req = HttpWebRequest.Create(this.serviceUri);
        //        //req.Method = "POST";
        //        //req.ContentType = "text/xml; charset=utf-8";
        //        //req.Headers.Add("SOAPAction", "http://tempuri.org/IMessageProcessor/Process");
        //        //string key = "";
        //        //try
        //        //{
        //        //    key = System.Configuration.ConfigurationManager.AppSettings[this.environment + "_NeweggAPICreateSOServiceKey"];
        //        //}
        //        //catch { }
        //        //req.Headers.Add("Authorization", key);
        //        //
        //        //Stream w = req.GetRequestStream();
        //        //StreamWriter swOut = new StreamWriter(w);
        //        //swOut.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><addShopOrder xmlns=\"com.jmservice\"><usertoken>73b61435-9688-4a5e-aa67-558f560fe256</usertoken><orderId>USBO13120400003</orderId><shopOrderItem><arrItemSKUAndNum xmlns=\"http://vo.webservice.jmservice.com\"><ItemSKUAndNum><dpCode>ABC</dpCode><productSku>950004-JD-500-168-01-SZZW</productSku></ItemSKUAndNum></arrItemSKUAndNum><consigeneeAddress xmlns=\"http://vo.webservice.jmservice.com\">28F., No.27-5, Sec. 2, Zhongzheng E. Rd., Tamsui Dist., New Taipei City 251, Taiwan</consigeneeAddress><consigeneeCountryCode xmlns=\"http://vo.webservice.jmservice.com\">CN</consigeneeCountryCode><consigeneeEmail xmlns=\"http://vo.webservice.jmservice.com\">service@newegg.com.tw</consigeneeEmail><consigeneeName xmlns=\"http://vo.webservice.jmservice.com\">Newegg Taiwan</consigeneeName><consigeneePhone xmlns=\"http://vo.webservice.jmservice.com\">88-6280068010</consigeneePhone><consigneeCity xmlns=\"http://vo.webservice.jmservice.com\">Taiwan</consigneeCity><consigneePostCode xmlns=\"http://vo.webservice.jmservice.com\">25170</consigneePostCode><consigneeState xmlns=\"http://vo.webservice.jmservice.com\">TW</consigneeState><ebayTrasactionId xmlns=\"http://vo.webservice.jmservice.com\" /><isBuyInsurance xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><isReturnProduct xsi:nil=\"true\" xmlns=\"http://vo.webservice.jmservice.com\" /><orderSource xmlns=\"http://vo.webservice.jmservice.com\" /><shippingMethod xmlns=\"http://vo.webservice.jmservice.com\">SGGH</shippingMethod><warehouse xmlns=\"http://vo.webservice.jmservice.com\">SZZW</warehouse></shopOrderItem><version>1.0</version></addShopOrder></soap:Body></soap:Envelope>");
        //        //swOut.Flush();
        //        //Stream r = req.GetResponse().GetResponseStream();
        //        //StreamReader srIn = new StreamReader(r);
        //        //string result = srIn.ReadToEnd();
        //        //swOut.Close();
        //        //srIn.Close();
        //        //w.Close();
        //        //r.Close();
        //        #endregion // 傳送資料XML收集
        //        JieMaiServiceAPI = new JieMai.Service.JieMaiClass(this.JieMaiServiceUri, this.JieMaiServiceKey);
        //        JieMai.JieMaiAPI.ShopOrderResult SOResult = new JieMai.JieMaiAPI.ShopOrderResult();
        //        SOResult = JieMaiServiceAPI.AddShopOrder(poCode, OItem); // 拋送訂單給借賣網
        //        string SuccessOrFail = SOResult.status.ToLower();
        //        if (SuccessOrFail == "fail") {
        //            throw new Exception(SOResult.message);
        //        }
        //        return SOResult;
        //    }
        //    catch(Exception e)
        //    {
        //        logger.Info("拋送訂單發生錯誤 ItemNumber:" + SellerProductID + "_" + e.Message);
        //        throw new Exception("拋送訂單發生錯誤 ItemNumber:" + SellerProductID + "_" + e.Message);
        //    }
        //}

        private string Serialize(object o)
        {
            XmlSerializer ser = new XmlSerializer(o.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, o);
            writer.Close();
            string result = sb.ToString();
            result = result.Replace(":d1p1", "");
            result = result.Replace("d1p1:", "xsi:");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("<MKTOrderInfo[\\w :=\\\"]+>");
            string temp = regex.Match(result).Value;
            if (temp.Length > 0)
            {
                result = result.Replace(temp, "<MKTOrderInfo>");
            }
            regex = new System.Text.RegularExpressions.Regex("<ItemInfo[\\w :=\\\"]+>");
            temp = regex.Match(result).Value;
            if (temp.Length > 0)
            {
                result = result.Replace(temp, "<ItemInfo>");
            }
            return result;
        }

        public void CallBackEndSyncJeiMaiSO()
        {
            try
            {
                string url = "http://" + IPPAddress + "/SalesOrder/SyncJeiMaiSO";
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = 1000;
                System.IO.Stream s = req.GetResponse().GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(s);
                string str = sr.ReadToEnd();
                sr.Close();
                s.Close();
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// 重新拋送PO
        /// </summary>
        /// <param name="salesOrderGroupID"></param>
        /// <param name="salesOrderCode"></param>
        /// <returns></returns>
        public string ReSendPlaceOrder(string salesOrderCode)
        {
            logger.Error("執行重新拋送PO salesOrderCode : " + salesOrderCode);
            TWSqlDBContext db = new TWSqlDBContext();
            SalesOrder so = db.SalesOrder.Where(x => x.Code == salesOrderCode).FirstOrDefault();
            //ShopOrderResult ShopOrderResults = new ShopOrderResult();
            if (so == null)
            {
                logger.Error("SalesOrder不存在: " + salesOrderCode);
                throw new Exception("SalesOrder不存在: " + salesOrderCode);
            }
            int delvType = (int)so.DelivType;
            logger.Error("delvType : " + delvType);
            //切貨自營 不產生PO也不進行拋單流程 直接回傳空字串
            if (delvType == (int)Item.tradestatus.切貨 || delvType == (int)Item.tradestatus.直配 || delvType == (int)Item.tradestatus.海外切貨)
            {
                logger.Error("切貨自營、直配、海外切貨 不產生PO也不進行拋單流程 直接回傳空字串: " + salesOrderCode);
                return "";
            }

            //過濾掉 國際運費、服務費 不產生PO也不進行拋單流程 直接回傳空字串
            int tempProductID = db.SalesOrderItem.Where(x => x.SalesorderCode == salesOrderCode).Select(x => x.ProductID).FirstOrDefault();
            if (tempProductID == 國際運費 || tempProductID == 服務費)
            {
                return "";
            }

            this._salesOrderGroupID = (int)so.SalesOrderGroupID;
            logger.Error("SalesOrderGroupID : " + (int)so.SalesOrderGroupID);
            //產生 PurchaseOrderGroup
            //this.purchaseordergroup_id = GeneratePurchaseOrderGroup(this._salesOrderGroupID);
            //產生PurchaseOrder
            //string poCode = GeneratePurchaseOrder(salesOrderCode);

            PurchaseOrder po = db.PurchaseOrder.Where(x => x.SalesorderCode == salesOrderCode).FirstOrDefault();
            string poCode = po.Code;
            logger.Error("poCode : " + po.Code);
            if (po == null)
            {
                logger.Error("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
                throw new Exception("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
            }
            // 非間配與三角者不進行拋單流程 直接回傳空字串
            if (delvType != (int)Item.tradestatus.間配 && delvType != (int)Item.tradestatus.三角)
            {
                logger.Error("非間配與三角者不進行拋單流程 直接回傳空字串: " + salesOrderCode);
                return "";
            }

            if (po != null)
            {
                logger.Error("Start");
                List<PurchaseOrderItem> poiList = db.PurchaseOrderItem.Where(x => x.PurchaseorderCode == po.Code).ToList();
                if (poiList.Count <= 0)
                {
                    logger.Error("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
                    throw new Exception("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
                }
                foreach (PurchaseOrderItem poi in poiList)
                {
                    logger.Error("poItem : " + poi.Code);
                    int pid = 0;
                    if (poi.ProductID != 0)
                    {
                        pid = poi.ProductID;
                    }
                    else if (poi.ProductlistID != 0)
                    {
                        pid = poi.ProductlistID;
                    }
                    logger.Error("ProductID : " + pid);
                    if (pid <= 0)
                    {
                        logger.Error("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
                        throw new Exception("PurchaseOrderItem [purchaseorderitem_code]:" + poi.Code + " 發生 [productid] and [productlistid] 異常");
                    }
                    string sellerOrderCode = "";
                    //string OrderID = "";
                    Product p = db.Product.Where(x => x.ID == pid).FirstOrDefault();
                    //如果是運費 則 product_sellerproductid 為 Null or 0
                    if (p.SellerProductID == null || p.SellerProductID == "" || p.SellerProductID.Length <= 0)
                    {
                        //跳過 運費項目 不需要拋轉訂單
                        continue;
                    }
                    else
                    {
                        try
                        {
                            float price = 0;
                            float.TryParse(p.Cost.ToString(), out price);
                            decimal shippingCharge = 0;
                            decimal.TryParse(p.SupplyShippingCharge.ToString(), out shippingCharge);
                            logger.Error("price : " + price);
                            logger.Error("SourceTable : " + p.SourceTable.ToLower());
                            switch (p.SourceTable.ToLower())
                            {
                                case "productfromws":
                                    switch (delvType)
                                    {
                                        //間配
                                        case 1:
                                            if (p.SellerProductID.IndexOf("9SI") >= 0)
                                            {
                                                //MKPL商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 501
                                                logger.Info("MKPL商品拋送訂單_間配_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", false);
                                            }
                                            else
                                            {
                                                //B2C商品
                                                //Bill to Neihu，Ship to Tamsui，ShipViaCode 001
                                                //不需要CustomerOwnShippingAccount 361901878
                                                //SpecialComment [[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC07]]
                                                logger.Info("B2C商品拋送訂單_間配_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                string specialComment = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + warehouse + "]]";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoTamsui, "001", false, "", "", specialComment);
                                            }
                                            break;
                                        //海外直購(管制商品、三角)
                                        case 3:
                                            //Bill to Customer
                                            CustomerInfo customerBillingInfo = new CustomerInfo();
                                            customerBillingInfo.ContactWith = so.RecvEngName;
                                            string addr = so.DelivEngADDR;
                                            if (addr.Length > 40)
                                            {
                                                customerBillingInfo.Address1 = addr.Substring(0, 40).Replace("repdot", ",");
                                                customerBillingInfo.Address2 = addr.Substring(40).Replace("repdot", ",");
                                            }
                                            else
                                            {
                                                customerBillingInfo.Address1 = addr;
                                            }
                                            customerBillingInfo.State = "TW";
                                            customerBillingInfo.City = "Taiwan";
                                            customerBillingInfo.Country = "TWN";
                                            customerBillingInfo.ZipCode = so.DelivZip;
                                            customerBillingInfo.CompanyName = "";
                                            customerBillingInfo.HomePhone = so.Mobile;
                                            customerBillingInfo.Fax = "";
                                            //Ship to Customer
                                            CustomerInfo customerShippingInfo = new CustomerInfo();
                                            customerShippingInfo.ContactWith = so.RecvEngName;
                                            addr = so.DelivEngADDR;
                                            if (addr.Length > 40)
                                            {
                                                customerShippingInfo.Address1 = addr.Substring(0, 40);
                                                customerShippingInfo.Address2 = addr.Substring(40);
                                            }
                                            else
                                            {
                                                customerShippingInfo.Address1 = addr;
                                            }
                                            customerShippingInfo.State = "TW";
                                            customerShippingInfo.City = "Taiwan";
                                            customerShippingInfo.Country = "TWN";
                                            customerShippingInfo.ZipCode = so.DelivZip;
                                            customerShippingInfo.CompanyName = "";
                                            customerShippingInfo.HomePhone = so.Mobile;
                                            customerShippingInfo.Fax = "";
                                            if (p.SellerProductID.IndexOf("9SI") >= 0)
                                            {
                                                //MKPL商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 501
                                                logger.Info("MKPL商品拋送訂單_海外直購_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "501", true, "TWN Commission:2%");
                                            }
                                            else
                                            {
                                                //B2C商品
                                                //Bill to Neihu，Ship to WH08，ShipViaCode 001
                                                //不需要CustomerOwnShippingAccount 361901878
                                                //SpecialComment [[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC07]]
                                                logger.Info("B2C商品拋送訂單_海外直購_PurchaseOrderItemCode:" + poi.Code.ToString());
                                                string warehouse = "07";
                                                string specialComment = "[[NAME:SanChen;Phone:6262711420;type:B2C;WCC:WCC" + warehouse + "]]";
                                                sellerOrderCode = SendToNeweggUSA(p.SellerProductID, price, shippingCharge, poi.Qty, warehouse, poCode, customerBillingInfoNeihu, customerShippingInfoWH08, "001", true, "TWN Commission:2%", "", specialComment);
                                            }
                                            break;
                                    }
                                    break;
                                //case "productfromjiemai":
                                //    switch (delvType)
                                //    {
                                //        //借賣網特別模式
                                //        case 4:
                                //            logger.Info("Step 1");
                                //            ShopOrderResults = SendToJieMai(p.SellerProductID, price, poi.Qty, poCode);
                                //            try
                                //            {
                                //                string SuccessOrFail = ShopOrderResults.status.ToLower();
                                //                if (SuccessOrFail == "fail")
                                //                {
                                //                    logger.Info("ShopOrderResults.失敗原因:" + ShopOrderResults.message);
                                //                }
                                //            }
                                //            catch { }
                                //            poi.Price = Convert.ToDecimal(ShopOrderResults.totalFee) / poi.Qty;
                                //            logger.Info("Step 2");
                                //            SaveJieMaiOrderInfo(ShopOrderResults);
                                //            //OrderID = ShopOrderResults.orderId;
                                //            try
                                //            {
                                //                logger.Info("ShopOrderResults.orderId:" + ShopOrderResults.orderId);
                                //            }
                                //            catch { }
                                //            logger.Info("Step 3");
                                //            //sellerOrderCode = ShopOrderResults.orderList[0];
                                //            sellerOrderCode = ShopOrderResults.jmOrderId;
                                //            //poi.Price = (decimal)(ShopOrderResults.totalFee / poi.qty);
                                //            logger.Info("Step 4");
                                //            CallBackEndSyncJeiMaiSO(); // 呼叫後台Steven 寫的 批次查詢JieMai的訂單狀態API
                                //            logger.Info("Step 5");
                                //            break;
                                //    }
                                //    break;
                                case "producttemp":
                                    switch (delvType)
                                    {
                                        //Local Vender B2C直配
                                        case 7:
                                            sellerOrderCode = "0";
                                            break;
                                    }
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            //write log
                            logger.Error("purchaseorderitem_code:" + poi.Code + " Error Message : " + e.Message + " Error StackTrace : " + e.StackTrace);
                            //將例外繼續往上層拋
                            throw e;
                        }
                    }
                    //訂單編號
                    logger.Error("sellerOrderCode : " + sellerOrderCode);
                    poi.SellerOrderCode = sellerOrderCode;
                    //變更PO單狀態
                    if (sellerOrderCode != "")
                    {
                        poi.Status = (int)PurchaseOrderItem.status.已成立;
                        po.Status = (int)PurchaseOrder.status.已成立;
                        logger.Error("po.Status : " + (int)PurchaseOrder.status.已成立);
                    }
                    //搜尋匯率
                    try
                    {
                        Seller seller = db.Seller.Where(x => x.ID == p.SellerID).FirstOrDefault();
                        logger.Error("sellerID : " + seller.ID);
                        if (seller != null)
                        {
                            string year = DateTime.Now.Year.ToString();
                            string month = DateTime.Now.Month.ToString();
                            Currency currency = db.Currency.Where(x => x.Type == seller.CurrencyType && x.Year == year && x.Month == month).FirstOrDefault();
                            logger.Error("poi.SourcecurrencyID : " + currency.ID);
                            if (currency != null)
                            {
                                //打上 匯率 流水號
                                poi.CurrencyID = currency.ID;
                                poi.SourcecurrencyID = currency.ID;
                            }
                            else
                            {
                                //找不到 當期 匯率
                                logger.Fatal("CurrencyType:" + (seller.CurrencyType ?? "NULL") + " Year:" + year + " Month:" + month + " 找不到當期匯率");
                            }
                        }
                        else
                        {
                            //找不到 seller
                            logger.Fatal("ProductID:" + p.ID.ToString() + " SellerID:" + p.SellerID.ToString() + " 找不到Seller");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message + ((e.InnerException != null) ? e.InnerException.Message : ""));
                    }
                    //儲存
                    db.SaveChanges();
                }
            }
            return poCode;
        }

        /// <summary>
        /// 目前不使用 Mobile API，改為使用 [WCF] placeOrderToUsByWs
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        /*private string SendByMobileAPI(string itemNumber, int qty)
        {
            //目前不使用此函式 所以拋出例外結束呼叫
            throw new Exception("暫時不允許使用");

            string soNumber = "";
            Service.NeweggRequest nr = new Service.NeweggRequest();
            //登入
            Newegg.Mobile.MvcApplication.Models.UIRegisterResultInfo regInfo = nr.Login("wh08@gmail.com", "wh08wh08");
            int cid = 0;
            if (regInfo != null && regInfo.Code == 0 && regInfo.Body != null && regInfo.Body.LoginStatus == Newegg.Mobile.MvcApplication.Models.UILoginStatus.Success && regInfo.Body.Customer != null && regInfo.Body.Customer.CustomerNumber > 0)
            {

                cid = regInfo.Body.Customer.CustomerNumber;
                //write log
                logger.Info("Newegg API Login Success");
            }
            else
            {
                string error = "";
                try
                {
                    error = regInfo.Description;
                }
                catch { }
                //write log
                logger.Info("拋送訂單失敗:[Newegg API Login Failure]_" + error);
            }

            try
            {
                //Checkout
                Newegg.Mobile.MvcApplication.Models.UICheckoutData cd = nr.Checkout(cid, itemNumber, qty);
                if (cd == null || cd.SessionID == null || cd.SessionID == "")
                {
                    throw new Exception("拋送訂單失敗:[Newegg API Checkout Failure]");
                }

                //write log
                logger.Info("Checkout ok ___ SessionID:" + cd.SessionID);

                //PlaceOrder
                Newegg.Mobile.MvcApplication.Models.Message<Newegg.Mobile.MvcApplication.Models.UICheckoutResult> result = nr.PlaceOrder(cd);
                if (result == null || result.HasSuccess == false || result.Body == null || result.Body.SoOrderNumbers == null)
                {
                    throw new Exception("拋送訂單失敗:[Newegg API PlaceOrder Failure]_SessionID:" + cd.SessionID + " _ " + result.Description);
                }
                soNumber = result.Body.SoOrderNumbers;

                //write log
                logger.Info("PlaceOrder ok ___ SoNumber:" + result.Body.SoOrderNumbers);
            }
            catch (Exception e)
            {
                //write log
                logger.Error("_[Error]_" + e.Message);
            }
            return soNumber;
        }*/

        //public string SendPlaceOrderToJM(int salesOrderGroupID)
        //{
        //    TWSqlDBContext db_before = new TWSqlDBContext();
        //    PurchaseOrder po = null;
        //    List<SalesOrder> JMSOList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID && x.DelivType == (int)Item.tradestatus.國外直購).ToList();
        //    // 先執行PO是否已產生的檢察，避免重複產生同一個SalesOrderCode的PO
        //    List<string> JMSalesOrderCodeList = null;
        //    JMSalesOrderCodeList = JMSOList.Select(x => x.Code).Distinct().ToList();
        //    List<PurchaseOrder> poList = null;
        //    poList = db_before.PurchaseOrder.Where(x => JMSalesOrderCodeList.Contains(x.SalesorderCode)).ToList();
        //    if (poList != null)
        //    {
        //        logger.Error("該PurchaseOrder存在不再重複拋單 : PurchaseOrderCode [" + poList[0].Code + "]");
        //        return "";
        //    }
        //    //ShopOrderResult ShopOrderResults = new ShopOrderResult(); // JieMai用
        //    List<PurchaseOrderItem> poiListALL = new List<PurchaseOrderItem>();
        //    decimal TotalPrice = 0; // SG總金額
        //    int TotalQty = 0; // SG商品總量
        //    if (JMSOList.Count <= 0)
        //    {
        //        logger.Error("JM_SalesOrderGroupID不存在: " + salesOrderGroupID);
        //        throw new Exception("JM_SalesOrderGroupID不存在: " + salesOrderGroupID);
        //    }
        //    string poCode = "", sellerOrderCode = "", PurchaseorderGroupID = "", tempPOCode = "";
        //    foreach (SalesOrder jmso in JMSOList)
        //    {
        //        this._salesOrderGroupID = jmso.SalesOrderGroupID ?? 0;
        //        //產生 PurchaseOrderGroup
        //        this.purchaseordergroup_id = GeneratePurchaseOrderGroup(this._salesOrderGroupID);
        //        //產生PurchaseOrder
        //        poCode = GeneratePurchaseOrder(jmso.Code);
        //        po = db_before.PurchaseOrder.Where(x => x.Code == poCode).FirstOrDefault();
        //        if (po == null)
        //        {
        //            logger.Error("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
        //            throw new Exception("PurchaseOrder不存在: [purchaseorder_code] " + poCode);
        //        }
        //        else
        //        {
        //            List<PurchaseOrderItem> poiList = db_before.PurchaseOrderItem.Where(x => x.PurchaseorderCode == po.Code).ToList();
        //            foreach (PurchaseOrderItem row in poiList)
        //            {
        //                TotalPrice += row.Price * row.Qty;
        //                TotalQty += row.Qty;
        //                poiListALL.Add(row);
        //            }
        //            if (poiList.Count <= 0)
        //            {
        //                logger.Error("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
        //                throw new Exception("PurchaseOrderItem數量異常 :" + poiList.Count.ToString());
        //            }
        //        }
        //    }
        //    try
        //    {
        //        logger.Info("Step 1");
        //        ShopOrderResults = SendToJieMaiV2(poiListALL, (int)po.PurchaseorderGroupID); // 拋單給借賣
        //        //SaveJieMaiOrderInfo(ShopOrderResults); // 再確認看看改成SG形式後應該怎麼修改2014-03-14
        //        logger.Info("Step 2");
        //        sellerOrderCode = ShopOrderResults.jmOrderId;
        //        PurchaseorderGroupID = ShopOrderResults.orderId;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message); // JM拋單失敗原因
        //    }
        //    logger.Info("Step 3");
        //    CallBackEndSyncJeiMaiSO(); // 呼叫後台Steven 寫的 批次查詢JieMai的訂單狀態API
        //    logger.Info("Step 4");
        //    decimal ExtraPrice = (TotalPrice - Convert.ToDecimal(ShopOrderResults.totalFee)) / TotalQty;
        //    foreach (PurchaseOrderItem poi in poiListALL)
        //    {
        //        poi.Price += ExtraPrice; // 將多出來的借賣網"其他費用"平均後回填到PO的金額中(此為Product.Cost商品原價)
        //        poi.SellerOrderCode = sellerOrderCode;
        //        if (sellerOrderCode != "")
        //        {
        //            poi.Status = (int)PurchaseOrderItem.status.已成立;
        //            if (tempPOCode != poi.PurchaseorderCode)
        //            {
        //                tempPOCode = poi.PurchaseorderCode;
        //                try
        //                {
        //                    PurchaseOrder searchPO = db_before.PurchaseOrder.Where(x => x.Code == poi.PurchaseorderCode).FirstOrDefault();
        //                    searchPO.Status = (int)PurchaseOrder.status.已成立;
        //                }
        //                catch
        //                {
        //                    // PO Status更新失敗
        //                    logger.Fatal("PurchaseOrder:" + poi.PurchaseorderCode + "Status更新失敗");
        //                }
        //            }
        //        }
        //        //搜尋匯率
        //        try
        //        {
        //            Seller seller = db_before.Seller.Where(x => x.ID == poi.SellerID).FirstOrDefault();
        //            if (seller != null)
        //            {
        //                string year = DateTime.Now.Year.ToString();
        //                string month = DateTime.Now.Month.ToString();
        //                Currency currency = db_before.Currency.Where(x => x.Type == seller.CurrencyType && x.Year == year && x.Month == month).FirstOrDefault();
        //                if (currency != null)
        //                {
        //                    //打上 匯率 流水號
        //                    poi.CurrencyID = currency.ID;
        //                    poi.SourcecurrencyID = currency.ID;
        //                }
        //                else
        //                {
        //                    //找不到 當期 匯率
        //                    logger.Fatal("CurrencyType:" + (seller.CurrencyType ?? "NULL") + " Year:" + year + " Month:" + month + " 找不到當期匯率");
        //                }
        //            }
        //            else
        //            {
        //                //找不到 seller
        //                logger.Fatal("ProductID:" + poi.ProductID.ToString() + " SellerID:" + poi.SellerID.ToString() + " 找不到Seller");
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            logger.Error(e.Message + ((e.InnerException != null) ? e.InnerException.Message : ""));
        //        }
        //        //儲存
        //        db_before.SaveChanges();
        //    }
        //    logger.Info("Step 5");
        //    return PurchaseorderGroupID;
        //}
    }
}