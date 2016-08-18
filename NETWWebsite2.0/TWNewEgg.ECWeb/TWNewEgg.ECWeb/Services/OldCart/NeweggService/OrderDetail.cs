using Newegg.Mobile.MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Redeem.Service;
using TWNewEgg.InternalSendMail.Service;
using System.Data;
using System.Data.SqlClient;
using System.Web.Util;
using TWNewEgg.GetConfigData.Service;
using TWNewEgg.DB;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class OrderDetail
    {
        private const string ITEMURLLINK = "/item?itemid={0}&categoryid={1}&StoreID={2}";
        private TWBackendDBContext dbafter = new TWBackendDBContext();
        private TWSqlDBContext dbbefore = new TWSqlDBContext();
        const int recentOrderSpan = 4;
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> OrderHistory(int? length, int start = 0, int accID = 0)
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> RecentOrderItemDetail = recentOrders(length, start, accID);

            return RecentOrderItemDetail;

        }
      
        /// <summary>
        /// recentOrders
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> recentOrders(int? length, int start = 0, int accID = 0)
        {

            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();
            if (accID > 0)
            {
                int count = length ?? dbbefore.SalesOrder.Where(s => s.AccountID == accID).Select(s => s.SalesOrderGroupID).Distinct().Count();

                //start = start* recentOrderSpan ;
          
                //if (start >= count)
                //{
                //    start = (count - 1) / recentOrderSpan * recentOrderSpan;
                   
                //}
                //if (start < 0)
                //{
                //    start = 0;
                //}
            
             

                List<SalesOrder> salesOrdersGroup = dbbefore.SalesOrder.Where(s => s.AccountID == accID).GroupBy(s => s.SalesOrderGroupID)
                    .OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).Skip(start).Take(recentOrderSpan).SelectMany(s => s).ToList();

                var model = SetRoI2(accID, salesOrdersGroup);

              


                ActionResponse.Body = model.Body;
                ActionResponse.IsSuccess = true;
                ActionResponse.Msg = "myAccountContent";
                //ViewBag.Content = "Partial_RecentOrder";
                return ActionResponse;
            }
            else
            {


                ActionResponse.IsSuccess = false;
                ActionResponse.Msg = "no login";
                //ViewBag.Content = "Partial_RecentOrder";
                return ActionResponse;
            }
        }
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> SalesOrderItem(int accID, string SOCode) 
        {
           
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> OrderHistory = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();
         List<SalesOrder> salesOrdersGroup=dbbefore.SalesOrder.Where(x => x.Code == SOCode).ToList();
         OrderHistory = SetRoI2(accID, salesOrdersGroup);
        

        return OrderHistory;
        }
        private List<RecentOrderItem> setROI(int accID, List<SalesOrder> salesOrdersGroup)
        {
            DateTime dateTimeNow = DateTime.Now;
            List<SalesOrder> fromCartGroup = new List<SalesOrder>();
            List<List<IGrouping<int?, SalesOrder>>> allGroup = new List<List<IGrouping<int?, SalesOrder>>>();
            List<Item> items = new List<Item>();
            List<string> eachSOCode = new List<string>();
            List<SalesOrderItem> salesorderitems;
            List<Process> processes;
            List<RecentOrderItem> ROItems = new List<RecentOrderItem>();
            string accIDstring = accID.ToString();

            var groupIDs = salesOrdersGroup.Select(s => s.SalesOrderGroupID).Distinct().ToList();
            var carts = dbafter.Cart.Where(c => groupIDs.Contains(c.SalesorderGroupID)).OrderBy(c => c.ID).ToList();
            //拿後台一整車的資料
            var fromSOGroup = salesOrdersGroup.Except((from c in carts
                                                       join s in salesOrdersGroup on c.ID equals s.Code into a
                                                       from b in a
                                                       select b)).ToList();
            //拿除了後台一整車的資料
            foreach (var cart in carts)
            {
                fromCartGroup.Add(new SalesOrder(cart));
            }
            //將後台資料餵回SO

            allGroup.Add(fromSOGroup.GroupBy(s => s.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
            allGroup.Add(fromCartGroup.GroupBy(c => c.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
            //拿所有SOCode資料
            eachSOCode = allGroup.SelectMany(g => g).SelectMany(s => s).Where(s => s.Note != "國際運費" && s.Note != "服務費").Select(s => s.Code).ToList();
            //拿所有SOCode資料裡面的SOItem
            salesorderitems = dbbefore.SalesOrderItem.Where(s => eachSOCode.Contains(s.SalesorderCode)).ToList();
            //拿所有SOCode資料裡面的Process
            processes = dbafter.Process.Where(p => eachSOCode.Contains(p.CartID)).ToList();
            //SOItem裡面的所有商品
            var productIDs = salesorderitems.Select(s => (s.ProductID > 0) ? s.ProductID : s.ProductlistID).ToList();
            //商品資料
            var products = dbbefore.Product.Where(p => productIDs.Contains(p.ID)).ToList();
            var sellerIDs = products.Select(p => p.SellerID).ToList();
            var sellers = dbbefore.Seller.Where(s => sellerIDs.Contains(s.ID)).ToList();
            var countryIds = sellers.Select(s => s.CountryID).ToList();
            var countrys = dbbefore.Country.Where(s => countryIds.Contains(s.ID)).ToList();
            //PO所有資料
            var POs = dbafter.PurchaseOrderTWBACK.Where(p => eachSOCode.Contains(p.SalesorderCode)).ToList();
            //PO所有資料裡面的Code
            var PONumbers = POs.Select(p => p.Code).ToList();
            //POItem所有資料
            var POIs = dbafter.PurchaseOrderitemTWBACK.Where(p => PONumbers.Contains(p.PurchaseorderCode)).ToList();
            //payTypes所有資料
            var payTypes = dbbefore.PayType.ToList();
            //是否有Process 資料以及SOITEM資料
            for (int k = 0; k < allGroup.Count(); k++)
            {
                foreach (var SOGroup in allGroup[k])
                {

                    bool isCart = k > 0;
                    var socodes = SOGroup.Select(s => s.Code).ToList();
                    List<RecentOrderItem> ros = new List<RecentOrderItem>();
                    if (isCart)
                    {
                        List<Process> tempProc = processes.Where(p => socodes.Contains(p.CartID)).ToList();
                        foreach (var proc in tempProc)
                        {
                            ros.Add(new RecentOrderItem(proc));
                        }
                    }
                    else
                    {
                        List<SalesOrderItem> tempSOI = salesorderitems.Where(p => socodes.Contains(p.SalesorderCode)).ToList();
                        foreach (var soi in tempSOI)
                        {
                            ros.Add(new RecentOrderItem(soi));
                        }
                    }

                    //要顯示的資料(product_sellerid,seller_country,country_name,salesorder_delivtype,salesorder_groupid,fromCart(是否從Cart來),TrackNo(從PO或是SO來),O2OShopDeliveryDate(o2o到貨日期))
                    for (int i = 0; i < ros.Count(); i++)
                    {

                        var r = ros[i];

                        Product product = products.FirstOrDefault(p => p.ID == r.ProductID);

                        DB.TWBACKENDDB.Models.Cart cart = carts.FirstOrDefault(c => c.ID == r.SalesorderCode);
                        r.product_sellerid = product.SellerID;

                        r.seller_country = sellers.FirstOrDefault(s => s.ID == product.SellerID).CountryID ?? 0;

                        r.country_name = countrys.FirstOrDefault(c => c.ID == r.seller_country).countryName;

                        r.salesorder_delivtype = SOGroup.FirstOrDefault(g => g.Code == r.SalesorderCode).DelivType;

                        r.salesorder_groupid = SOGroup.First(g => g.Code == r.SalesorderCode).SalesOrderGroupID ?? 0;

                        r.fromCart = isCart;


                        var purchaseOrder = POs.FirstOrDefault(p => p.SalesorderCode == r.SalesorderCode) ?? new PurchaseOrderTWBACK();
                        var purchaseOrderItem = POIs.Where(p => p.PurchaseorderCode == purchaseOrder.Code).ToList() ?? new List<PurchaseOrderitemTWBACK>();

                        if (r.salesorder_delivtype == 3)
                        {
                            r.TrackNo = purchaseOrder.ForwardNO;
                        }
                        else if (cart != null)
                        {
                            r.O2OShopDeliveryDate = cart.O2OShopDeliveryDate.ToString() ?? "";
                            r.TrackNo = string.IsNullOrEmpty(cart.Deliv4NO) ? cart.DelivNO : cart.Deliv4NO;
                            r.process_delivno = string.IsNullOrEmpty(cart.Deliv4NO) ? cart.DelivNO : cart.Deliv4NO;
                        }


                        SalesOrder so = SOGroup.FirstOrDefault(g => g.Code == r.SalesorderCode);
                        // 若有訂單存在
                        if (isCart)
                        {
                            // 訂單正常(Status = 0 || Status = 99)進入下列判斷,其他則判斷定單狀態
                            if (so.Status == 0 || so.Status == 99)
                            {
                                if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                                {
                                    // 待可自動回填客戶收貨日期補上條件,已送達
                                    r.status = 5;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                                {
                                    // 已出貨
                                    r.status = 4;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.待出貨)
                                {
                                    // 商品押上發票號碼,出貨中
                                    r.status = 3;
                                }
                                else if (cart != null && (r.salesorder_delivtype == 0 || r.salesorder_delivtype == 2 || r.salesorder_delivtype == 7) && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立)
                                {
                                    // 切貨商品初始狀態,已成立
                                    r.status = 1;
                                }
                                else if (cart != null && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.空運中)
                                {
                                    // 空運中
                                    r.status = 2;
                                }
                                else if (cart != null && r.salesorder_delivtype == 6 && (cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.初始狀態 || cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立))
                                {
                                    // 海外切貨商品初始狀態,空運中
                                    r.status = 2;
                                }
                                else if (purchaseOrder.DelvStatus == 1 || purchaseOrder.DelvStatus == 0)
                                {
                                    // 空運中
                                    r.status = 2;
                                }
                                else if (purchaseOrder.DelvStatus == 999)
                                {
                                    // 拋單成功 Tracking#2生成,上飛機, WH60尚未收貨前
                                    if (!string.IsNullOrEmpty(purchaseOrder.ForwardNO))
                                    {
                                        // 空運中
                                        r.status = 2;
                                    }
                                    else if (!purchaseOrderItem.Any(p => string.IsNullOrEmpty(p.SellerorderCode)))
                                    {
                                        // 訂單成立
                                        r.status = 1;
                                    }
                                    else if (purchaseOrderItem.Any(p => string.IsNullOrEmpty(p.SellerorderCode)))
                                    {
                                        // 確認中
                                        r.status = 0;
                                    }
                                    else if (!string.IsNullOrEmpty(purchaseOrder.DELIVNO) || !products.FirstOrDefault(p => p.ID == r.ProductID).SellerProductID.StartsWith("9SI"))
                                    {
                                        // 回押tracking#1或為自營商品,訂單成立
                                        r.status = 1;
                                    }
                                }
                                else
                                {
                                    // status = 0,確認中->統統改成訂單成立
                                    r.status = 1;
                                }

                            }
                            else if (so.Status == 5)
                            {
                                // 已退貨
                                r.status = 6;
                                if (cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                                {
                                    // 無法配達，需要已出貨並且已退貨的狀態 (只針對貨到付款)
                                    r.status = -3;
                                }
                            }
                            else if (so.Status == 7 && cart.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                            {
                                // 已配達
                                r.status = 5;
                            }
                            else if (so.Status == 7)
                            {
                                // 已完成但未壓配達, 所以不能申請退貨, 狀態為已出貨
                                r.status = 4;
                            }
                            else if (so.Status == 1 || so.Status == 2)
                            {
                                // 已取消
                                r.status = -1;
                            }

                            // Find Invoice Number from backend DB
                            var invoiceInfo = dbafter.InvoiceList.Where(x => x.SONumber == r.SalesorderCode).FirstOrDefault();
                            if (invoiceInfo != null)
                            {
                                r.InvoiceNumber = invoiceInfo.InvoiceNumber;
                                r.InvoiceInDate = invoiceInfo.InDate;
                            }
                            else
                            {
                                r.InvoiceNumber = "";
                                r.InvoiceInDate = null;
                            }

                        }
                        else
                        {
                            // check salesorder status after 6 mins.
                            if (DateTime.Compare(so.CreateDate.AddMinutes(6), dateTimeNow) < 0)
                            {
                                if (so.Status == 99)
                                {
                                    // paid fail, and cancel.
                                    r.status = -4;
                                }
                                else if (so.Status == 0)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = 34;
                                }
                                else if (so.Status == 1)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -1;
                                }
                                else if (so.Status == 2)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -4;
                                }
                                else if (so.Status == 3)
                                {
                                    // paid success, but backend db didn't create cart and process.
                                    r.status = -1;
                                }
                                else
                                {
                                    // other reason, then so error.
                                    r.status = 34;
                                }
                            }
                            else
                            {
                                if (so.Status == 99)
                                {
                                    // confirm this salesorder in db, paid failed, show cancel this order.
                                    r.status = 33;
                                }
                                else if (so.Status == 0)
                                {
                                    // r.status = 30; paid success.
                                    if (payTypes.Where(x => x.PayType0rateNum == so.PayType).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == 31).FirstOrDefault())
                                    {
                                        // paid success, but backend db didn't create cart and process.
                                        r.status = 31;
                                    }
                                    else
                                    {
                                        // paid success, but backend db didn't create cart and process.
                                        r.status = 30;
                                    }
                                }
                            }
                        }
                        ROItems.Add(r);
                    }
                }
            }



            // 已退貨的resultCart加上retStatus
            var canceled = ROItems.Where(r => r.status == 6).ToList();
            var retCartProcIds = canceled.Select(p => new { salesorderitem_salesordercode = p.SalesorderCode, salesorderitem_code = p.Code }).ToList();

            Dictionary<Retgood, string> retgoodDic = new Dictionary<Retgood, string>();
            var returnedProcID = retCartProcIds.Select(r => r.salesorderitem_code).ToList();
            var retgoods = dbafter.Retgood.Where(r => returnedProcID.Contains(r.ProcessID)).ToList();
            foreach (var ret in retgoods)
            {
                retgoodDic.Add(ret, retCartProcIds.FirstOrDefault(r => r.salesorderitem_code == ret.ProcessID).salesorderitem_salesordercode);
            }
            for (int i = 0; i < canceled.Count(); i++)
            {
                var retstatus = retgoodDic.Where(r => r.Value == canceled[i].SalesorderCode).LastOrDefault().Key.Status;
                canceled[i].retStatus = retstatus;
            }

            // 已進入退款的resultCart, retStatus看refund
            var refunding = ROItems.Where(r => r.retStatus == 99).ToList();
            var refCartProcIds = canceled.Select(p => new { salesorderitem_salesordercode = p.SalesorderCode, salesorderitem_code = p.Code }).ToList();

            Dictionary<refund2c, string> refundDic = new Dictionary<refund2c, string>();
            var refundingProcID = refCartProcIds.Select(r => r.salesorderitem_code).ToList();
            var refunds = dbafter.refund2c.Where(r => refundingProcID.Contains(r.ProcessID)).ToList();
            foreach (var rf in refunds)
            {
                refundDic.Add(rf, refCartProcIds.FirstOrDefault(r => r.salesorderitem_code == rf.ProcessID).salesorderitem_salesordercode);
            }
            for (int i = 0; i < refunding.Count(); i++)
            {
                var retstatus = refundDic.FirstOrDefault(r => r.Value == refunding[i].SalesorderCode).Key.Status;
                refunding[i].retStatus = retstatus;
            }

            var prob = dbbefore.Problem.Where(p => eachSOCode.Contains(p.BlngCode)).Select(p => new { prblm_blngcode = p.BlngCode, prblm_code = p.Code }).ToList();
            foreach (var code in prob)
            {
                var temp = ROItems.Where(r => r.SalesorderCode == code.prblm_blngcode).ToList();
                foreach (var item in temp)
                {
                    item.prblm_prblmcode = code.prblm_code;
                }
            }

            return ROItems;
        }

        public int SalesOrderData(int? length, int accID = 0) 
        {

            int count = length ?? dbbefore.SalesOrder.Where(s => s.AccountID == accID).Select(s => s.SalesOrderGroupID).Distinct().Count();
            return count;
        }
        public TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> SetRoI2(int accID, List<SalesOrder> salesOrdersGroup) 
        {
            TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory> ActionResponse = new TWNewEgg.Models.ViewModels.Redeem.ActionResponse<TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>();
                DateTime dateTimeNow = DateTime.Now;
            List<SalesOrder> fromCartGroup = new List<SalesOrder>();
            List<List<IGrouping<int?, SalesOrder>>> allGroup = new List<List<IGrouping<int?, SalesOrder>>>();
            List<Item> items = new List<Item>();
            List<string> eachSOCode = new List<string>();
            List<SalesOrderItem> salesorderitems;
            List<Process> processes;
             TWBackendDBContext after = new TWBackendDBContext();
              TWSqlDBContext before = new TWSqlDBContext();
              List<Deliver> DeliverList = after.Deliver.ToList();
            TWNewEgg.Models.ViewModels.MyAccount.OrderHistory OrderHistoryDetail = new TWNewEgg.Models.ViewModels.MyAccount.OrderHistory();

            try
            {
                var groupIDs = salesOrdersGroup.Select(s => s.SalesOrderGroupID).Distinct().ToList();
                var carts = after.Cart.Where(c => groupIDs.Contains(c.SalesorderGroupID)).OrderBy(c => c.ID).ToList();
                //拿後台一整車的資料
                var fromSOGroup = salesOrdersGroup.Except((from c in carts
                                                           join s in salesOrdersGroup on c.ID equals s.Code into a
                                                           from b in a
                                                           select b)).ToList();
                //拿除了後台一整車的資料
                foreach (var cart in carts)
                {
                    fromCartGroup.Add(new SalesOrder(cart));
                }
                //將後台資料餵回SO

                allGroup.Add(fromSOGroup.GroupBy(s => s.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
                allGroup.Add(fromCartGroup.GroupBy(c => c.SalesOrderGroupID).OrderByDescending(s => s.FirstOrDefault().SalesOrderGroupID).ToList());
                //拿所有SOCode資料
                eachSOCode = allGroup.SelectMany(g => g).SelectMany(s => s).Where(s => s.Note != "國際運費" && s.Note != "服務費").Select(s => s.Code).ToList();
                //拿所有SOCode資料裡面的SOItem
                salesorderitems = before.SalesOrderItem.Where(s => eachSOCode.Contains(s.SalesorderCode)).ToList();
                //拿所有SOCode資料裡面的Process
                processes = after.Process.Where(p => eachSOCode.Contains(p.CartID)).ToList();
         
                //PO所有資料
                var POs = after.PurchaseOrderTWBACK.Where(p => eachSOCode.Contains(p.SalesorderCode)).ToList();
                //PO所有資料裡面的Code
                var PONumbers = POs.Select(p => p.Code).ToList();
                //POItem所有資料
                var POIs =after.PurchaseOrderitemTWBACK.Where(p => PONumbers.Contains(p.PurchaseorderCode)).ToList();
                //payTypes所有資料
                var payTypes = before.PayType.ToList();
                //是否有Process 資料以及SOITEM資料
                OrderHistoryDetail.SalceOrderList = new List<TWNewEgg.Models.ViewModels.MyAccount.SalceOrder>();
                int i = 0;
                for (int k = 0; k < allGroup.Count(); k++)
                {
               
                    foreach (var SOGroup in allGroup[k])
                    {
              
                      foreach (var SOGroupItem in SOGroup)
                      {
                          TWNewEgg.Models.ViewModels.MyAccount.SalceOrder salceorderdetile = new TWNewEgg.Models.ViewModels.MyAccount.SalceOrder();
                          AutoMapper.Mapper.Map(SOGroupItem, salceorderdetile);
                         
                          OrderHistoryDetail.SalceOrderList.Add(salceorderdetile);
                          OrderHistoryDetail.SalceOrderList[i] = new TWNewEgg.Models.ViewModels.MyAccount.SalceOrder();
                          OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil = new List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>();
                        var socodes = SOGroup.Select(s => s.Code).ToList();
        
                        bool isCart = k > 0;
                        if (isCart)
                        {
                            //for SEO URL
                            int categoryID = new int(), storeID = new int();
                            //SOItem裡面的所有商品
                            var productIDs = processes.Select(s => (s.ProductID > 0) ? s.ProductID : s.ProductlistID).ToList();
                            //商品資料
                            var products = before.Product.Where(p => productIDs.Contains(p.ID)).ToList();
                            var sellerIDs = products.Select(p => p.SellerID).ToList();
                            var sellers = before.Seller.Where(s => sellerIDs.Contains(s.ID)).ToList();
                            var countryIds = sellers.Select(s => s.CountryID).ToList();
                            var countrys = before.Country.Where(s => countryIds.Contains(s.ID)).ToList();
                            AutoMapper.Mapper.Map(SOGroupItem, OrderHistoryDetail.SalceOrderList[i]);
                            List<Process> tempProc = processes.Where(p => p.CartID == SOGroupItem.Code).ToList();
                            List <string>SOGID= SOGroup.Select(x => x.Code).ToList();
                            List<Process> ProcessListDetail=after.Process.Where(x =>SOGID.Contains(x.CartID)).ToList();
                             List<Cart> CartList=after.Cart.Where(x=>SOGID.Contains(x.ID)).ToList();

                            decimal? CartPriceSum = 0;
                            CartPriceSum = processes.Where(x => x.CartID == SOGroupItem.Code).Sum(x => x.DisplayPrice + (decimal)Math.Round(double.Parse(x.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)x.Pricecoupon - (decimal)x.ApportionedAmount);
                            foreach (var tempProcdetail in tempProc)
                            {
                                decimal DisplayoringPrice = tempProcdetail.DisplayPrice ?? 0;
                                List<string> InvoiceNO = new List<string>();

                                int? PODelvStatus = null;
                                string ForwardNO = null;
                                string PODELIVNO = null;
                                bool SellerorderCode = false;
                                DateTime CreateDate = carts.Where(x => x.ID == tempProcdetail.CartID).Select(x => (DateTime)x.CreateDate).FirstOrDefault();
                                int CartStatus = carts.Where(x => x.ID == tempProcdetail.CartID).Select(x => (int)x.Status).FirstOrDefault();
                                int Delvstatus = carts.Where(x => x.ID == tempProcdetail.CartID).Select(x => (int)x.DelvStatus).FirstOrDefault();
                                int Delvtype = carts.Where(x => x.ID == tempProcdetail.CartID).Select(x => (int)x.ShipType).FirstOrDefault();
                                int SoStatus = dbbefore.SalesOrder.Where(x => x.Code == tempProcdetail.CartID).Select(x => (int)x.Status).FirstOrDefault();
                                int Paytype = dbbefore.SalesOrder.Where(x => x.Code == tempProcdetail.CartID).Select(x => (int)x.PayType).FirstOrDefault();
                               string InoiceNO= carts.Where(x => x.ID == tempProcdetail.CartID).Select(x => x.InvoiceNO).FirstOrDefault();
                               string InvoiceInDate = after.InvoiceMaster.Where(x => x.InvoiceNo == InoiceNO).Select(x => x.InDate).FirstOrDefault().ToString("yyyy/MM/dd hh:mm:ss");

                                //int PaytypeID = dbbefore.SalesOrder.Where(x => x.Code == tempProcdetail.CartID).Select(x => (int)x.PayTypeID).FirstOrDefault();
                                int? PaytypeID = dbbefore.SalesOrder.Where(x => x.Code == tempProcdetail.CartID).Select(x => x.PayTypeID).FirstOrDefault();

                                if (PaytypeID == null)
                                {
                                    PaytypeID = dbbefore.PayType.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault().ID;
                                }
                                bool getPaytypeboolen = ( (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()) );
                                bool Paytypeboolen = ((payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.網路ATM).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.實體ATM).FirstOrDefault()));
                               string PaytypeName= payTypes.Where(x => x.ID == PaytypeID).Select(x => x.Name).FirstOrDefault();
                             
                                TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem SalesOrderItem = new TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem();
                                if (products.FirstOrDefault(p => p.ID == tempProcdetail.ProductID).SellerID != 2)
                                {
                                    SalesOrderItem.CountryName = "台灣";
                                }
                                else
                                {

                                    SalesOrderItem.CountryName = "美國";
                                }
                                if (POs.Where(x => x.SalesorderCode == tempProcdetail.CartID).Count() != 0)
                                {
                                    PODelvStatus = POs.Where(x => x.SalesorderCode == tempProcdetail.CartID).Select(x => (int)x.DelvStatus).FirstOrDefault();
                                    ForwardNO = POs.Where(x => x.SalesorderCode == tempProcdetail.CartID).Select(x => x.ForwardNO).FirstOrDefault();
                                    PODELIVNO = POs.Where(x => x.SalesorderCode == tempProcdetail.CartID).Select(x => x.DELIVNO).FirstOrDefault();
                                    SellerorderCode = dbafter.PurchaseOrderitemTWBACK.Any(p => string.IsNullOrEmpty(p.SellerorderCode));

                                }
                                int ProductIDS = (int)tempProcdetail.ProductID;
                                bool IS9SIA = false;
                                if (products.FirstOrDefault(p => p.ID == tempProcdetail.ProductID)!=null&&products.FirstOrDefault(p => p.ID == tempProcdetail.ProductID).SellerProductID != null)
                                {
                                    IS9SIA = products.FirstOrDefault(p => p.ID == tempProcdetail.ProductID).SellerProductID.StartsWith("9SI");
                                }
                              
                                AutoMapper.Mapper.Map(tempProcdetail, SalesOrderItem);

                                SalesOrderItem.DisplayPriceTemp = DisplayoringPrice;
                                SalesOrderItem.PayType = Paytype.ToString();
                                OrderHistoryDetail.SalceOrderList[i].InvoiceNo = InoiceNO;
                                OrderHistoryDetail.SalceOrderList[i].Paytypeboolen = Paytypeboolen;
                                OrderHistoryDetail.SalceOrderList[i].PaytypeNmae = PaytypeName;
                                if (tempProcdetail.ProcOut != null)
                                    OrderHistoryDetail.SalceOrderList[i].Procout = (DateTime)tempProcdetail.ProcOut;
                                OrderHistoryDetail.SalceOrderList[i].Status = Returnstatus(isCart, CartStatus, SoStatus, PODelvStatus, Delvstatus, Delvtype, ForwardNO, PODELIVNO, IS9SIA, SellerorderCode, CreateDate, getPaytypeboolen, tempProcdetail.CartID);
                                OrderHistoryDetail.SalceOrderList[i].CreateDate = SOGroupItem.CreateDate.ToString("yyyy/MM/dd hh:mm:ss");
                                OrderHistoryDetail.SalceOrderList[i].InvoiceInDate = InvoiceInDate;
                               TWNewEgg.DB.TWBACKENDDB.Models.Deliver DeliverItem= DeliverList.Where(x => x.code == tempProcdetail.Deliver).FirstOrDefault();
                  
                                if (SalesOrderItem != null)
                                {
                                    OrderHistoryDetail.SalceOrderList[i].PiceSum = (decimal)CartPriceSum;
                                   // OrderHistoryDetail.SalceOrderList[i].PiceSum += SalesOrderItem.DisplayPrice + (decimal)Math.Round(double.Parse(tempProcdetail.ServiceExpense.ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempProcdetail.ShippingExpense.ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempProcdetail.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)tempProcdetail.ShippingExpense - (decimal)tempProcdetail.ApportionedAmount;
                                    OrderHistoryDetail.SalceOrderList[i].SumQTY += SalesOrderItem.Qty;
                                }
                                else 
                                {

                                    OrderHistoryDetail.SalceOrderList[i].PiceSum +=0;
                                    OrderHistoryDetail.SalceOrderList[i].SumQTY +=0;
                                }
                                if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達 && CartStatus != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨)
                                {
                                    if (after.InvoiceList.Where(x => x.SONumber == tempProcdetail.CartID).Count() != 0)
                                    {
                                        InvoiceNO = after.InvoiceList.Where(x => x.SONumber == tempProcdetail.CartID).Select(x => x.InvoiceNumber).ToList();
                                       var dt= after.InvoiceList.Where(x => InvoiceNO.Contains(x.InvoiceNumber)).FirstOrDefault();
                                        if (after.InvoiceList.Where(x => InvoiceNO.Contains(x.InvoiceNumber)).Count() != 0)
                                        {
                                           
                                            if(dt.InDate > DateTime.Now.AddDays(-8))
                                            {
                                            OrderHistoryDetail.SalceOrderList[i].IsReturnd = true;
                                            }
                                            if (dt.InDate.AddDays(8) < DateTime.Now) 
                                            {
                                                OrderHistoryDetail.SalceOrderList[i].IsFix = true;
                                            
                                            }
                                      
                                        }
                                        

                                    }

                                }
                                else if (OrderHistoryDetail.SalceOrderList[i].Status == "確認中" || OrderHistoryDetail.SalceOrderList[i].Status == "訂單成立" || OrderHistoryDetail.SalceOrderList[i].Status == "付款完成")
                                       {


                                          OrderHistoryDetail.SalceOrderList[i].IsRefund = CheckISRefundStatus(CartList);
                                           //OrderHistoryDetail.SalceOrderList[i].IsRefund = true;
                                       }
                                       else 
                                       {
                                           OrderHistoryDetail.SalceOrderList[i].IsFix = false;
                                           OrderHistoryDetail.SalceOrderList[i].IsReturnd = false;
                                           OrderHistoryDetail.SalceOrderList[i].IsRefund = false;
                                         
                                       }
                                  //  OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil
                                OrderHistoryDetail.SalceOrderList[i].Delivtype = Delvtype;
                                this.GetItemCategoryIDStoreID(tempProcdetail.StoreID ?? 0, out categoryID, out storeID);
                                //OrderHistoryDetail.SalceOrderList[i].ItemUrl = "/item?itemid=" + tempProcdetail.StoreID;
                                OrderHistoryDetail.SalceOrderList[i].ItemUrl = string.Format(ITEMURLLINK, tempProcdetail.StoreID.Value.ToString(), categoryID.ToString(), storeID.ToString());
                                OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil = new List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>();
                                SalesOrderItem.DisplayPrice = DisplayoringPrice + (decimal)Math.Round(double.Parse((tempProcdetail.ServiceExpense??0m).ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse((tempProcdetail.ShippingExpense??0m).ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempProcdetail.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)(tempProcdetail.ShippingExpense??0m) - (decimal)tempProcdetail.ApportionedAmount;
                                SalesOrderItem.ApportionedAmount = tempProc.Sum(x => x.ApportionedAmount+(decimal)x.Pricecoupon);
                                SalesOrderItem.InstallmentFee = tempProc.Sum(x => x.InstallmentFee);
                                if (DeliverItem != null)
                                {
                                    SalesOrderItem.DeliverName = DeliverItem.Name;
                                    SalesOrderItem.DeliverWebSite = DeliverItem.WebSite;
                                }

                                OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil.Add(SalesOrderItem);
                             
                            }
                        }
                        else
                        {
                            //for SEO URL
                            int categoryID = new int(), storeID = new int();
                            //SOItem裡面的所有商品
                            var productIDs = salesorderitems.Select(s => (s.ProductID > 0) ? s.ProductID : s.ProductlistID).ToList();
                            //商品資料
                            var products = before.Product.Where(p => productIDs.Contains(p.ID)).ToList();
                            var sellerIDs = products.Select(p => p.SellerID).ToList();
                            var sellers = before.Seller.Where(s => sellerIDs.Contains(s.ID)).ToList();
                            var countryIds = sellers.Select(s => s.CountryID).ToList();
                            var countrys = before.Country.Where(s => countryIds.Contains(s.ID)).ToList();
                            AutoMapper.Mapper.Map(SOGroupItem, OrderHistoryDetail.SalceOrderList[i]);
                            decimal? SalesorderItemPriceSum = 0;
                            List<SalesOrderItem> tempSOI = salesorderitems.Where(p =>p.SalesorderCode == SOGroupItem.Code).ToList();
                            List<string> SOGID = SOGroup.Select(x => x.Code).ToList();

                            SalesorderItemPriceSum = tempSOI.Where(x => x.SalesorderCode == SOGroupItem.Code).Sum(x => x.DisplayPrice + (decimal)Math.Round(double.Parse(x.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)x.Pricecoupon - (decimal)x.ApportionedAmount);
                            foreach (var tempSOIdetail in tempSOI)
                            {
                                decimal DisplayoringPrice = tempSOIdetail.DisplayPrice ?? 0;
                                int? PODelvStatus = null;
                                string ForwardNO = null;
                                string PODELIVNO = null;
                                bool SellerorderCode = false;
                                int SoStatus = dbbefore.SalesOrder.Where(x => x.Code == SOGroupItem.Code).Select(x => (int)x.Status).FirstOrDefault();
                                int Delvtype = dbbefore.SalesOrder.Where(x => x.Code == SOGroupItem.Code).Select(x => (int)x.DelivType).FirstOrDefault();
                                bool IS9SIA = false;
                                if (products.FirstOrDefault(p => p.ID == tempSOIdetail.ProductID)!=null&&products.FirstOrDefault(p => p.ID == tempSOIdetail.ProductID).SellerProductID != null)
                                {
                                   IS9SIA = products.FirstOrDefault(p => p.ID == tempSOIdetail.ProductID).SellerProductID.StartsWith("9SI");
                                }
                                DateTime CreateDate = dbbefore.SalesOrder.Where(x => x.Code == tempSOIdetail.SalesorderCode).Select(x => (DateTime)x.CreateDate).FirstOrDefault();
                                int Paytype = dbbefore.SalesOrder.Where(x => x.Code == tempSOIdetail.SalesorderCode).Select(x => (int)x.PayType).FirstOrDefault();
                                int? PaytypeID = dbbefore.SalesOrder.Where(x => x.Code == tempSOIdetail.SalesorderCode).Select(x => x.PayTypeID).FirstOrDefault();

                                if (PaytypeID == null) {
                                    PaytypeID = dbbefore.PayType.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault().ID;
                                }
                                bool getPaytypeboolen = ((payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()));
                                bool Paytypeboolen = ((payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.網路ATM).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).FirstOrDefault()) || (payTypes.Where(x => x.PayType0rateNum == Paytype).FirstOrDefault() == payTypes.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.實體ATM).FirstOrDefault()));
                                string PaytypeNmae = payTypes.Where(x => x.ID == PaytypeID).Select(x => x.Name).FirstOrDefault();
                             
                                if (POs.Where(x => x.SalesorderCode == tempSOIdetail.SalesorderCode).Count() != 0)
                                {
                                    PODelvStatus = POs.Where(x => x.SalesorderCode == tempSOIdetail.SalesorderCode).Select(x => (int)x.DelvStatus).FirstOrDefault();
                                    ForwardNO = POs.Where(x => x.SalesorderCode == tempSOIdetail.SalesorderCode).Select(x => x.ForwardNO).FirstOrDefault();
                                    PODELIVNO = POs.Where(x => x.SalesorderCode == tempSOIdetail.SalesorderCode).Select(x => x.DELIVNO).FirstOrDefault();
                                    SellerorderCode = dbafter.PurchaseOrderitemTWBACK.Any(p => string.IsNullOrEmpty(p.SellerorderCode));

                                }
                                TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem SalesOrderItem = new TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem();
                                if (products.FirstOrDefault(p => p.ID == tempSOIdetail.ProductID).SellerID != 2)
                                {
                                    SalesOrderItem.CountryName = "台灣";
                                }
                                else
                                {

                                    SalesOrderItem.CountryName = "美國";
                                }
                                AutoMapper.Mapper.Map(tempSOIdetail, SalesOrderItem);
                                SalesOrderItem.DisplayPriceTemp = DisplayoringPrice;
                                SalesOrderItem.PayType = Paytype.ToString();
                                OrderHistoryDetail.SalceOrderList[i].Code = tempSOIdetail.SalesorderCode;
                                this.GetItemCategoryIDStoreID(tempSOIdetail.ItemID, out categoryID, out storeID);
                                //OrderHistoryDetail.SalceOrderList[i].ItemUrl = "/item?itemid=" + tempSOIdetail.ItemID;
                                OrderHistoryDetail.SalceOrderList[i].ItemUrl = string.Format(ITEMURLLINK, tempSOIdetail.ItemID.ToString(), categoryID.ToString(), storeID.ToString());
                                OrderHistoryDetail.SalceOrderList[i].Paytypeboolen = Paytypeboolen;
                                OrderHistoryDetail.SalceOrderList[i].Status = Returnstatus(isCart, null, SoStatus, PODelvStatus, null, Delvtype, ForwardNO, PODELIVNO, IS9SIA, SellerorderCode, CreateDate, getPaytypeboolen, tempSOIdetail.SalesorderCode);
                                OrderHistoryDetail.SalceOrderList[i].CreateDate = SOGroupItem.CreateDate.ToString("yyyy/MM/dd");
                                OrderHistoryDetail.SalceOrderList[i].PaytypeNmae = PaytypeNmae;
                                OrderHistoryDetail.SalceOrderList[i].Delivtype = Delvtype;
                                if (SalesOrderItem != null)
                                {
                                    OrderHistoryDetail.SalceOrderList[i].PiceSum = (decimal)SalesorderItemPriceSum;
                                    //OrderHistoryDetail.SalceOrderList[i].PiceSum += SalesOrderItem.DisplayPrice + (decimal)Math.Round(double.Parse(tempSOIdetail.ServiceExpense.ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempSOIdetail.ShippingExpense.ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempSOIdetail.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)tempSOIdetail.ShippingExpense - (decimal)tempSOIdetail.ApportionedAmount;
                                    OrderHistoryDetail.SalceOrderList[i].SumQTY += SalesOrderItem.Qty;
                                }
                                else 
                                {


                                    OrderHistoryDetail.SalceOrderList[i].PiceSum += 0;
                                    OrderHistoryDetail.SalceOrderList[i].SumQTY += 0;
                                
                                }
                                
                                OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil = new List<TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>();
                                SalesOrderItem.DisplayPrice = DisplayoringPrice + (decimal)Math.Round(double.Parse((tempSOIdetail.ServiceExpense ?? 0m).ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse((tempSOIdetail.ShippingExpense ?? 0m).ToString()), 0, MidpointRounding.AwayFromZero) + (decimal)Math.Round(double.Parse(tempSOIdetail.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - (decimal)(tempSOIdetail.ShippingExpense ?? 0m) - (decimal)tempSOIdetail.ApportionedAmount;
                                SalesOrderItem.ApportionedAmount = tempSOI.Sum(x => x.ApportionedAmount + (decimal)x.Pricecoupon);
                                SalesOrderItem.InstallmentFee = tempSOI.Sum(x => x.InstallmentFee);
                                OrderHistoryDetail.SalceOrderList[i].SalesOrderItemDetil.Add(SalesOrderItem);
                       
                            }
                            
                        }
                        i++;
                      }
                    }
                }
                ActionResponse.Body = OrderHistoryDetail;
                ActionResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ActionResponse.IsSuccess = false;
                ActionResponse.Msg = ex.StackTrace;
            }
            return ActionResponse;
        }
        public string Returnstatus(bool isCart, int? Cartstatus, int SoStatus, int? PODelvStatus, int? Delvstatus, int Delvtype, string ForwardNO, string PODELIVNO, bool IS9SIA, bool SellerorderCode, DateTime SOCreateDate, bool Paytypeboolen,string SoCode) 
        {
            string retStatusList = "";

            // 若有訂單存在
            if (isCart)
            {
                // 訂單正常(Status = 0 || Status = 99)進入下列判斷,其他則判斷定單狀態
                if (SoStatus == 0 || SoStatus == 99)
                {
                    if ( Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                    {
                        // 待可自動回填客戶收貨日期補上條件,已送達
                        retStatusList=(TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已配達).ToString();
                    }
                    else if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                    {
                        // 已出貨
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已出貨).ToString();
                    }
                    else if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.待出貨)
                    {
                        // 商品押上發票號碼,出貨中
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.待出貨).ToString();
                    }


                    else if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立)
                    {
                    //{
                    //    if (Delvtype == 0 || Delvtype == 2 || Delvtype == 7)
                    //    {
                    //        // 切貨商品初始狀態,已成立

                    //        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.訂單成立).ToString();
                    //    }
                    //    else 
                    //    {

                            retStatusList ="付款完成";
                        
                        //}
                    }
                    else if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.空運中)
                    {
                        // 空運中
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.空運中).ToString();
                    }
                    else if (Delvtype == 6 && (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.初始狀態 || Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立))
                    {
                        // 空運中
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.空運中).ToString();
                    }
                    else if (PODelvStatus == 1 || PODelvStatus == 0)
                    {
                        // 空運中
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.空運中).ToString();
                    }
                    else if (PODelvStatus == 999)
                    {
                        // 拋單成功 Tracking#2生成,上飛機, WH60尚未收貨前
                        if (!string.IsNullOrEmpty(ForwardNO))
                        {
                            // 空運中
                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.空運中).ToString();
                        }
                        else if (SellerorderCode)
                        {
                            // 訂單成立
                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.訂單成立).ToString();
                        }
                        else if (!SellerorderCode)
                        {
                            // 確認中
                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.確認中).ToString();
                        }
                        else if (!string.IsNullOrEmpty(PODELIVNO) || !IS9SIA)
                        {
                            // 回押tracking#1或為自營商品,訂單成立
                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.訂單成立).ToString();
                        }
                    }
                    else
                    {
                        // status = 0,確認中->統統改成訂單成立
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.訂單成立).ToString();
                    }

                }
                else if (SoStatus == 5)
                {
                    // 已退貨
                    retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.retStatusList.退款異常).ToString();
                    if (Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨)
                    {
                        // 無法配達，需要已出貨並且已退貨的狀態 (只針對貨到付款)
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.無法配達).ToString();
                    }
                    else 
                    {
                        int Statusretgood=dbafter.Retgood.Where(x => x.CartID == SoCode).Select(x=>(int)x.Status).FirstOrDefault();
                       retStatusList= Enum.GetName(typeof(DB.TWBACKENDDB.Models.Retgood.status), Statusretgood).ToString();
                       if (Statusretgood == (int)DB.TWBACKENDDB.Models.Retgood.status.進入退款程序) 
                       {

                           int Statusrefund = dbafter.refund2c.Where(x => x.CartID == SoCode).Select(x => (int)x.Status).FirstOrDefault();
                           retStatusList = Enum.GetName(typeof(DB.TWBACKENDDB.Models.Retgood.status), Statusrefund).ToString();
                       }
                      
                    }
                }
                else if (SoStatus == 7 && Delvstatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.配達)
                {
                    // 已配達
                    retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已配達).ToString();
                }
                else if (SoStatus == 7)
                {
                    // 已完成但未壓配達, 所以不能申請退貨, 狀態為已出貨
                    retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已出貨).ToString();
                }
                else if (SoStatus == 1 || SoStatus == 2)
                {
                    // 已取消
                    retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已取消).ToString();
                }

                //// Find Invoice Number from backend DB
                //var invoiceInfo = dbafter.InvoiceList.Where(x => x.SONumber == r.SalesorderCode).FirstOrDefault();
                //if (invoiceInfo != null)
                //{
                //    r.InvoiceNumber = invoiceInfo.InvoiceNumber;
                //    r.InvoiceInDate = invoiceInfo.InDate;
                //}
                //else
                //{
                //    r.InvoiceNumber = "";
                //    r.InvoiceInDate = null;
                //}

            }
            else
            {
                DateTime dateTimeNow = DateTime.Now;
                // check salesorder status after 6 mins.
                if (DateTime.Compare(SOCreateDate.AddMinutes(6), dateTimeNow) < 0)
                {
                    if (SoStatus == 99)
                    {
                        // paid fail, and cancel.
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.付款失敗).ToString();
                    }
                    else if (SoStatus == 0)
                    {
                        // paid success, but backend db didn't create cart and process.
                        retStatusList = "付款完成";
                    }
                    else if (SoStatus == 1)
                    {
                        // paid success, but backend db didn't create cart and process.
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已取消).ToString();
                    }
                    else if (SoStatus == 2)
                    {
                        // paid success, but backend db didn't create cart and process.
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.付款失敗已取消).ToString();
                    }
                    else if (SoStatus == 3)
                    {
                        // paid success, but backend db didn't create cart and process.
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已取消).ToString();
                      //  r.status = -1;
                    }
                    else if (SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.台新分期處理中) 
                    {

                        retStatusList = "金流處理中";
                    
                    }
                    else
                    {
                        if (SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶WebATM處理中 || SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶線下ATM處理中 || SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶儲值支付處理中)
                        {
                            retStatusList = "未付款";
                        }
                        else
                        {

                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.已取消).ToString();
                        }
                    }
                }
                else
                {
                    if (SoStatus == 99)
                    {
                        // confirm this salesorder in db, paid failed, show cancel this order.
                       // r.status = 33;
                        retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.付款失敗).ToString();
                    }
                    else if (SoStatus == 0)
                    {
                        // r.status = 30; paid success.
                        if (Paytypeboolen)
                        {
                            // paid success, but backend db didn't create cart and process.
                            retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.貨到付款).ToString();
                            //r.status = 31;
                        }
                        else
                        {
                            // paid success, but backend db didn't create cart and process.
                    
                                   retStatusList = (TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.付款成功).ToString();
                            //r.status = 30;
                        }
                    }
                    else if (SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.台新分期處理中)
                    {

                        retStatusList = "金流處理中";

                    }
                    else if (SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶WebATM處理中 || SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶線下ATM處理中 || SoStatus == (int)TWNewEgg.Models.ViewModels.MyAccount.OrderHistory.statusList.歐付寶儲值支付處理中)
                    {
                        retStatusList = "未付款";
                    }
                }
            }
            return retStatusList;
        }
        public bool CheckISRefundStatus(List<Cart> Carts) 
        {
            bool ISrefund = true;
            
                foreach (var CartsDetail in Carts) 
                {
                    if (CartsDetail.DelvStatus != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立) 
                    {

                        ISrefund = false;
                    }
                
                }
                return ISrefund;
        
        }
        private void GetItemCategoryIDStoreID(int itemID, out int categoryID, out int storeID)
        {
            categoryID = 0;
            storeID = 0;
            #region Sorry...時間緊迫=  = 只能這樣寫...Orz
            DB.TWSqlDBContext twsql = new DB.TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.Item item = twsql.Item.Where(x => x.ID == itemID).FirstOrDefault();
            if (item == null)
            {
                return;
            }
            TWNewEgg.DB.TWSQLDB.Models.Category category = twsql.Category.Where(x => x.ID == item.CategoryID).FirstOrDefault();
            TWNewEgg.DB.TWSQLDB.Models.Category second = null;
            TWNewEgg.DB.TWSQLDB.Models.Category store = null;
            if (category != null)
            {
                categoryID = category.ID;
                second = twsql.Category.Where(x => x.ID == category.ParentID).FirstOrDefault();
                if (second != null)
                {
                    storeID = second.ID;
                    store = twsql.Category.Where(x => x.ID == second.ParentID).FirstOrDefault();
                }
            }
            if (store != null)
            {
                storeID = store.ID;
            }
            #endregion Sorry...時間緊迫=  = 只能這樣寫...Orz 
        }
      
    }

}

