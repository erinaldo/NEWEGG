using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TWNewEgg.DataToExcel;
using log4net;
using log4net.Config;



namespace TWNewEgg.API.Service
{
    #region Enum

    ///// <summary>
    ///// 遞送服務類別
    ///// </summary>
    //public enum EnumDelvServiceType
    //{
    //    SBS = "S",
    //    SBN = "N"
    //}

    ///// <summary>
    ///// 供貨通路
    ///// </summary>
    //public enum EnumFulfillChannel
    //{
    //    商家 = "S",
    //    供應商 = "V"
    //}
    #endregion

    public partial class SalesOrderService
    {
        #region Properties
        private DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
        private DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext sellerPortal = new DB.TWSellerPortalDBContext();
        private static ILog log = LogManager.GetLogger(typeof(ProcessOrderNumService));



        // Add by Jack.W.Wu 0626
        private enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        // 特殊費用
        //   國際運費: productID = 13189
        //   服務費  : productID = 13190
        private int ShippingFeeID = 13189;
        private int ServiceFeeID = 13190;
        private List<int> GetSpecialFee()
        {
            return new List<int>() { ShippingFeeID, ServiceFeeID };
        }
        #endregion

        /// <summary>
        /// Search for "sales order"。訂單搜尋
        /// </summary>
        /// <param name="condition">搜尋條件</param>
        /// <returns></returns>
        public Models.ActionResponse<List<Models.OrderInfo>> QueryOrderInfos(Models.QueryCartCondition condition)
        {
            Models.ActionResponse<List<Models.OrderInfo>> result = new Models.ActionResponse<List<Models.OrderInfo>>();
            try
            {
                #region 判斷身分別

                // 商家 ID
                int sellerID = -1;

                // 商家營業地址帶入出貨地址
                string sellerShippingAddress = string.Empty;

                // 若商家 ID 可以正常轉為 int 型態
                if (Int32.TryParse(condition.SellerID, out sellerID))
                {
                    // 查詢此商家 ID 的身分是 Seller 或 Vender
                    var sellerInfo = sellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).FirstOrDefault();

                    string comZip = string.IsNullOrEmpty(sellerInfo.ComZipcode) ? string.Empty : sellerInfo.ComZipcode.Trim() + " ";

                    sellerShippingAddress = comZip + sellerInfo.ComCity + sellerInfo.ComSellerAdd;

                    // 寫入搜尋條件
                    switch (sellerInfo.AccountTypeCode)
                    {
                        default:
                        case "S":
                            {
                                condition.AccountType = Models.OrderInfo.EnumAccountTypeCode.Seller;
                                break;
                            }
                        case "V":
                            {
                                condition.AccountType = Models.OrderInfo.EnumAccountTypeCode.Vendor;
                                break;
                            }
                    }
                }
                else
                {
                    // 若商家 ID 轉 int 失敗，則將身分指定為 Seller
                    condition.AccountType = Models.OrderInfo.EnumAccountTypeCode.Seller;
                }

                #endregion 判斷身分別



                List<int> specialFee = GetSpecialFee();
                string SellerName = "";
                var Query_Cart = from crt in backend.Cart select crt;
                var Query_Process = from prcs in backend.Process select prcs;
                var Query_Product = from prd in frontend.Product select prd;
                var Query_PurchaseOrder = from pchsOdr in backend.PurchaseOrderTWBACK select pchsOdr;
                var Query_PurchaseOrderItem = from pchsOdrItm in backend.PurchaseOrderitemTWBACK select pchsOdrItm;
                //var Query_SellerInfo = from slrInfo in sellerPortal.Seller_BasicInfo where slrInfo.SellerID == condition.SellerID select slrInfo;

                #region Condition Filter

                //假資料
                //condition.SellerID = "1";

                // sellerID
                if (!string.IsNullOrEmpty(condition.SellerID))
                {
                    int id = -1;
                    if (int.TryParse(condition.SellerID, out id))
                    {
                        Query_Product = Query_Product.Where(r => r.SellerID == id);
                        SellerName = sellerPortal.Seller_BasicInfo.Where(x => x.SellerID == id).Select(r => r.SellerName).FirstOrDefault();
                    }
                }

                //訂單編號
                if (!string.IsNullOrEmpty(condition.SOCode))
                {
                    Query_Cart = Query_Cart.Where(r => r.ID.Contains(condition.SOCode));
                }

                if (condition.ProcessIDs != null && condition.ProcessIDs.Count > 0)
                {
                    Query_Process = Query_Process.Where(r => condition.ProcessIDs.Contains(r.ID));
                }

                //發票號碼
                if (!string.IsNullOrEmpty(condition.InvoiceNumber))
                {
                    Query_Cart = Query_Cart.Where(r => r.InvoiceNO.Trim() == condition.InvoiceNumber.Trim());
                }

                //客戶姓名
                if (!string.IsNullOrEmpty(condition.CustomerName))
                {
                    Query_Cart = Query_Cart.Where(r => r.Username.Contains(condition.CustomerName));
                }

                //商家商品編號
                if (!string.IsNullOrEmpty(condition.SellerProductID))
                {
                    Query_Product = Query_Product.Where(r => r.SellerProductID.Contains(condition.SellerProductID));
                }

                //新蛋商品編號
                if (!string.IsNullOrEmpty(condition.ProductID))
                {
                    int id = -1;
                    if (int.TryParse(condition.ProductID, out id))
                    {
                        Query_Process = Query_Process.Where(r => r.ProductID == id);
                    }
                }

                //客戶電話
                if (!string.IsNullOrEmpty(condition.CustomerPhone))
                {
                    Query_Cart = Query_Cart.Where(r => r.Phone.Trim() == condition.CustomerPhone.Trim());
                }

                //標題描述
                if (!string.IsNullOrEmpty(condition.Title))
                {
                    Query_Process = Query_Process.Where(r => r.Title.Contains(condition.Title));
                }

                //生產廠商
                if (!string.IsNullOrEmpty(condition.Manufacture))
                {
                    var manufactureIDs = sellerPortal.Seller_ManufactureInfo
                                               .Where(r => r.ManufactureName.Contains(condition.Manufacture))
                                               .Select(r => r.SN).AsQueryable();

                    if (manufactureIDs != null && manufactureIDs.Count() > 0)
                    {
                        Query_Product = Query_Product.Where(r => manufactureIDs.Contains(r.ManufactureID));
                    }
                }

                // 訂單狀態
                switch (condition.OrderSearchMode)
                {
                    // 已成立，只可取消訂單，所以須排除取消之訂單
                    case (int)TWNewEgg.API.Models.MainOrderStatus.已成立:
                        Query_Cart = Query_Cart.Where(x => x.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已成立 && x.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消);
                        break;
                    // 待出貨，只可取消訂單，所以須排除取消之訂單
                    case (int)TWNewEgg.API.Models.MainOrderStatus.待出貨:
                        Query_Cart = Query_Cart.Where(x => x.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.待出貨 && x.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消);
                        break;
                    // 已出貨，只可退貨，所以須排除退貨之訂單
                    case (int)TWNewEgg.API.Models.MainOrderStatus.已出貨:
                        Query_Cart = Query_Cart.Where(x => x.DelvStatus == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.cartstatus.已出貨 && x.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨);
                        break;
                    case (int)TWNewEgg.API.Models.MainOrderStatus.配達:
                        Query_Cart = Query_Cart.Where(x => x.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.完成);
                        break;
                    case (int)TWNewEgg.API.Models.MainOrderStatus.取消:
                        Query_Cart = Query_Cart.Where(x => x.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.取消);
                        break;
                    case (int)TWNewEgg.API.Models.MainOrderStatus.退貨:
                        Query_Cart = Query_Cart.Where(x => x.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.退貨);
                        break;
                    default:
                        break;
                }

                #endregion

                // 某Seller 名下的所有商品資訊
                List<DB.TWSQLDB.Models.Product> SellersProduct = Query_Product.ToList();
                //var SellersProduct = Query_Product.AsQueryable();
                // 某Seller 名下的所有商品ID
                List<int> idOfsellersProduct = SellersProduct.Select(r => r.ID).ToList();
                //var idOfsellersProduct = SellersProduct.Select(r => r.ID).AsQueryable();

                // 某Seller 名下的所有Process join Cart, 含以下邏輯
                // 1. 篩選遞送狀態 (只取狀態為 0, 6, 1者)
                // 2. 篩選特殊費用 (國際運費, 服務費)
                var SellersProcessJoinCart = (from crt in Query_Cart
                                              join proc in Query_Process on crt.ID equals proc.CartID
                                              where idOfsellersProduct.Contains(proc.ProductID.Value)
                                                    && (crt.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.已出貨
                                                        || crt.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.待出貨
                                                        || crt.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.配達
                                                        || crt.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.已成立)
                                                    && !specialFee.Contains(proc.ProductID.Value)
                                              select new { crt, proc });

                // 某Seller 名下的所有Process, 包括相同訂單的子商品。之後需要做Group by所以只能先存成List Jack.W.Wu 0626
                List<DB.TWBACKENDDB.Models.Process> sellersProcessOriginal = (from pp in SellersProcessJoinCart select pp.proc).ToList();

                // 同訂單的子單Group by, 只需要一筆之後再計算總量 add by Jack.W.Wu 0625
                IQueryable<DB.TWBACKENDDB.Models.Process> sellersProcess = sellersProcessOriginal.GroupBy(x => x.CartID).Select(g => g.First()).AsQueryable();

                // 某Seller 名下的所有Cart // 只顯示單一主單，排除多子單導致主單重複出現問題 add by Jack.W.Wu 0625
                IQueryable<DB.TWBACKENDDB.Models.Cart> sellersCartQueryable = (from pp in SellersProcessJoinCart select pp.crt).Distinct().AsQueryable();

                // 日期搜尋
                if (condition.DayBefore.HasValue)
                {
                    var Query_CartList = sellersCartQueryable.ToList();

                    // 輸入天數，搜尋距離現在日期前幾天建立的商品
                    Query_CartList = Query_CartList.Where(x => DateTime.Compare(x.CreateDate.Value, DateTime.Now.AddDays(-condition.DayBefore.Value)) > 0).ToList();
                    sellersCartQueryable = Query_CartList.AsQueryable();
                }
                else if (!string.IsNullOrEmpty(condition.BeginDate) && string.IsNullOrEmpty(condition.EndDate))
                {
                    var Query_CartList = sellersCartQueryable.ToList();
                    // 只輸入開始日期，搜尋特定日期
                    Query_CartList = Query_CartList.Where(x => x.CreateDate.Value.DayOfYear == Convert.ToDateTime(condition.BeginDate).DayOfYear).ToList();
                    sellersCartQueryable = Query_CartList.AsQueryable();
                }
                else if (!string.IsNullOrEmpty(condition.BeginDate) && !string.IsNullOrEmpty(condition.EndDate))
                {
                    var Query_CartList = sellersCartQueryable.ToList();
                    // 輸入開始及結束日期區間，搜尋區間中所建立的商品
                    Query_CartList = Query_CartList.Where(x => DateTime.Compare(x.CreateDate.Value, Convert.ToDateTime(condition.BeginDate)) > 0 && DateTime.Compare(x.CreateDate.Value, Convert.ToDateTime(condition.EndDate).AddDays(1)) < 0).ToList();
                    sellersCartQueryable = Query_CartList.AsQueryable();
                }

                int totalCount = sellersCartQueryable.Count();



                // API分頁功能 Add by Jack.W.Wu 0626
                if (condition.PageIndex != null && condition.PageSize != null)
                {
                    sellersCartQueryable = sellersCartQueryable.OrderByDescending(x => x.UpdateDate).Skip(condition.PageIndex.Value * condition.PageSize.Value).Take(condition.PageSize.Value).AsQueryable();
                }
                else
                {
                    sellersCartQueryable = sellersCartQueryable.OrderByDescending(x => x.UpdateDate).AsQueryable();
                }

                List<DB.TWBACKENDDB.Models.Cart> sellersCart = sellersCartQueryable.ToList();

                // 某Seller 名下的所有特殊運費 (國際運費, 服務費)
                var sellersSpecialFee = (from crt in Query_Cart
                                         join proc in Query_Process on crt.ID equals proc.CartID
                                         where specialFee.Contains(proc.ProductID.Value)
                                         select new { crt, proc }).ToList();

                // 使用 Ship Carrier API取代 Jack.W.Wu 0626
                //List<string> allDeliverIDxName = new List<string>();
                //Add by Jack.W 0618
                Dictionary<int, string> deliverPair = new Dictionary<int, string>();

                backend.Deliver.OrderBy(r => r.code).ToList().ForEach(r =>
                {
                    // 使用 Ship Carrier API取代 Jack.W.Wu 0626
                    //allDeliverIDxName.Add(string.Format("{0}.{1}", r.code, r.Name));

                    //Add by Jack.W 0618
                    deliverPair.Add(r.code, r.Name);
                });

                //透過某Seller 名下的所有Cart, 去找到其相關聯的product, 並且組出訂單資訊以及訂單明細
                List<Models.OrderInfo> Orders = new List<Models.OrderInfo>();

                //補齊資料 Start
                //補齊資料 End
                foreach (var singleCart in sellersCart)
                {
                    //List<DB.TWBACKENDDB.Models.Process> processOfSingleOrder = sellersProcess.Where(r => r.CartID == singleCart.ID).ToList();
                    IQueryable<DB.TWBACKENDDB.Models.Process> processOfSingleOrder = sellersProcess.Where(x => x.CartID == singleCart.ID).AsQueryable();

                    if (processOfSingleOrder != null && processOfSingleOrder.Count() > 0)
                    {
                        List<Models.OrderDetailsInfo> OrderDtails = new List<Models.OrderDetailsInfo>();

                        #region Vendor 使用項目

                        // PO 單號
                        string purchaseOrderNumber = string.Empty;

                        // 成本 Cost
                        decimal unitCost = 0;

                        // 身分為 Vendor 才查 PO單號、成本Cost
                        if (condition.AccountType == Models.OrderInfo.EnumAccountTypeCode.Vendor)
                        {
                            #region PO 單號

                            // 查詢 PO 單號
                            if (Query_PurchaseOrder != null)
                            {
                                purchaseOrderNumber = Query_PurchaseOrder.Where(x => x.SalesorderCode == singleCart.ID).Select(x => x.Code).FirstOrDefault();
                            }

                            #endregion PO 單號

                            #region 成本 Cost

                            // 如果有查到 PO 單號，才查詢成本 Cost
                            if (!string.IsNullOrEmpty(purchaseOrderNumber))
                            {
                                // 查詢成本 Cost
                                unitCost = Query_PurchaseOrderItem.Where(x => x.PurchaseorderCode == purchaseOrderNumber).Select(y => y.SourcePrice).FirstOrDefault();
                            }

                            #endregion 成本Cost
                        }

                        #endregion Vendor 使用項目

                        foreach (var process in processOfSingleOrder)
                        {
                            // 查詢tracking number丟入model Add by Jack.W.Wu 0613
                            string trackingNum = sellerPortal.Seller_DelvTrack.Where(x => x.ProcessID == process.ID).Select(x => x.TrackingNum).FirstOrDefault();
                            //int delvID = sellerPortal.Seller_DelvTrack.Where(x => x.ProcessID == process.ID).Select(x => x.DeliverID).FirstOrDefault();
                            int delvID = backend.Process.Where(x => x.ID == process.ID && x.Deliver.HasValue).Select(x => x.Deliver.Value).FirstOrDefault();
                            string deliverName = deliverPair.Where(x => x.Key == delvID).Select(x => x.Value).FirstOrDefault();

                            //DB.TWSQLDB.Models.Product product = SellersProduct.Find(r => r.ID == process.ProductID);
                            //if (product == null)
                            //    product = new DB.TWSQLDB.Models.Product();
                            DB.TWSQLDB.Models.Product product = SellersProduct.Where(r => r.ID == process.ProductID).FirstOrDefault();

                            // Detail已遞送數量修改顯示部分：2014/08/06by Ted 依需求修改shippedCount
                            // if cart.status = 1(訂單取消) then 已遞送數量 = 0
                            // else
                            // {
                            //     if cart.DelvStatus = 6 || 0 (訂單狀態是"已成立"or"待出貨") then 
                            //         已遞送數量 = 0 
                            //     else 
                            //         已遞送數量 = 訂購數量
                            // }
                            int shippedCount_Modified = -1;
                            if (singleCart.Status == (int)Models.OrderInfo.EnumCartStatus.取消)
                            {
                                shippedCount_Modified = 0;
                            }
                            else if (singleCart.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.已成立 || singleCart.DelvStatus == (int)Models.OrderInfo.EnumDelvStatus.待出貨)
                            {
                                shippedCount_Modified = 0;
                            }
                            else
                            {
                                shippedCount_Modified = sellersProcessOriginal.Where(x => x.CartID == singleCart.ID).Sum(s => s.Qty) ?? 0;
                            }

                            int _Qty = sellersProcessOriginal.Where(x => x.CartID == singleCart.ID).Sum(s => s.Qty) ?? 0;

                            #region 寫入 OrderDetailsInfo/OrderDetailsInfo.Add(
                            OrderDtails.Add(new Models.OrderDetailsInfo()
                            {
                                ItemName = (process == null ? "" : process.Title.ToString()),
                                MenufacturePartNum = product == null ? "" : string.IsNullOrEmpty(product.MenufacturePartNum) ? "N/A" : product.MenufacturePartNum,
                                NeweggPartNum = (process == null ? "" : process.ProductID.ToString()),
                                ProductStatus = (product == null ? -1 : product.Status),
                                // Qty = process.Qty ?? 0,
                                // Qty = process.ProductID.Value,
                                // 計算子單商品總量 Changed by Jack.W.Wu 0626
                                Qty = _Qty,

                                SellerProductID = product == null ? "" : product.SellerProductID,
                                Status = ((Models.OrderInfo.EnumDelvStatus)singleCart.DelvStatus).ToString(),
                                Title = process.Title,
                                UPC = product == null ? "" : (string.IsNullOrEmpty(product.UPC) ? "N/A" : product.UPC),

                                //補資訊
                                CMPName = singleCart.CMPName,
                                CustomerMobile = singleCart.Mobile, //20140617 Ted 補註明：此在(訂單管理)訂單頁面上為"訂購人手機"cart.Mobile
                                Price = process.Price.Value,

                                // 2014.05.23 Change by Jack.C ICE: 怕Seller會錯亂，將 ShippedCount = Qty，無做其他判斷
                                // ShippedCount = process.Qty.Value /*(Models.OrderInfo.EnumDelvStatus)singleCart.DelvStatus == Models.OrderInfo.EnumDelvStatus.出貨中 ? (process.Qty ?? 0) : 0*/,
                                // 計算子單商品總量 Changed by Jack.W.Wu 0626
                                // 2014.08.11 Modiified by Ted:依照需求修改ShippedCount
                                ShippedCount = shippedCount_Modified,

                                ProcessID = process.ID,
                                ProductID = product == null ? -1 : product.ID,
                                MenufactureID = product == null ? -1 : product.ManufactureID,
                                // 加入Tracking Number Add by Jack.W.Wu 0613
                                TrackingNumber = trackingNum,
                                DelvDate = process.OrderNote.Replace("</arrive>", ""),// 加入希望到貨時間 Add by Jack.W.Wu 0613
                                DelvName = deliverName,//貨運商名稱 Add by Jack.W.Wu 0618

                                // 成本
                                UnitCost = unitCost,
                                TotalCost = unitCost * ((decimal)_Qty),
                            });

                            #endregion 寫入 OrderDetailsInfo
                        }



                        //對 DelvStatus欄位做邏輯上的加工
                        string delvStatusStr = singleCart.DelvStatus.HasValue ? ((Models.OrderInfo.EnumDelvStatus)singleCart.DelvStatus.Value).ToString() : string.Empty;
                        if ((Models.OrderInfo.EnumCartStatus)singleCart.Status == Models.OrderInfo.EnumCartStatus.取消
                            || (Models.OrderInfo.EnumCartStatus)singleCart.Status == Models.OrderInfo.EnumCartStatus.退貨)
                        {
                            delvStatusStr = ((Models.OrderInfo.EnumCartStatus)singleCart.Status).ToString();
                        }

                        // 增加 付款方式PayType
                        string _PayTypeName = string.Empty;
                        string Query_paytype = frontend.PayType.Where(x => x.ID == singleCart.PayTypeID).Select(x => x.Name).FirstOrDefault();
                        if (Query_paytype != null)
                        {
                            _PayTypeName = Query_paytype.ToString();
                        }
                        //else

                        //增加 營業地址 資訊
                        var _Seller_BasicInfo_AddressInfo = sellerPortal.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => new { x.ComZipcode, x.ComCity, x.ComSellerAdd }).FirstOrDefault();
                        string _businessAddress = string.Empty;
                        if (_Seller_BasicInfo_AddressInfo != null)
                        { _businessAddress = (_Seller_BasicInfo_AddressInfo.ComZipcode ?? "").Trim() + (_Seller_BasicInfo_AddressInfo.ComCity ?? "").Trim() + (_Seller_BasicInfo_AddressInfo.ComSellerAdd ?? "").Trim(); }
                        else
                        { _businessAddress = ""; }



                        #region 小計項目
                        // 子項總計
                        decimal subtotalPrice = 0;

                        // 運費
                        decimal shippingFee = sellersSpecialFee
                                              .Where(r => r.proc.ProductID == this.ShippingFeeID && r.crt.SalesorderGroupID == singleCart.SalesorderGroupID)
                                              .Sum(r => r.proc.Price.Value);

                        // 服務費
                        decimal serviceFee = sellersSpecialFee
                                             .Where(r => r.proc.ProductID == this.ServiceFeeID && r.crt.SalesorderGroupID == singleCart.SalesorderGroupID)
                                             .Sum(r => r.proc.Price.Value);

                        // 訂購總額
                        decimal totalPrice = 0;

                        switch (condition.AccountType)
                        {
                            default:
                            case Models.OrderInfo.EnumAccountTypeCode.Seller:
                                {
                                    subtotalPrice = sellersProcessOriginal.Where(x => x.CartID == singleCart.ID).Sum(s => s.Price) ?? 0;
                                    totalPrice = subtotalPrice + shippingFee + shippingFee;
                                    break;
                                }
                            case Models.OrderInfo.EnumAccountTypeCode.Vendor:
                                {
                                    subtotalPrice = unitCost * OrderDtails.Sum(r => r.Qty);
                                    totalPrice = subtotalPrice + shippingFee + shippingFee;
                                    break;
                                }
                        }
                        #endregion 小計項目


                        #region 寫入OrderInfo/OrderInfo.Add(
                        Orders.Add(new Models.OrderInfo()
                                        {
                                            Address = singleCart.ADDR,
                                            CMPName = singleCart.CMPName,
                                            CreateDate = singleCart.CreateDate.ToString() ?? DateTime.MinValue.ToString(),
                                            CustomerName = singleCart.Username,

                                            DelvStatus = (Models.OrderInfo.EnumDelvStatus)singleCart.DelvStatus,

                                            DelvStatusStr = delvStatusStr,

                                            // 購物車編號 add by Jack 2015.9.11
                                            SaleOrderGroupID = singleCart.SalesorderGroupID,

                                            //供貨通路/遞送服務/遞送方(代碼)
                                            Item_DelvType = (singleCart.ShipType ?? -1).ToString(),

                                            //供貨通路/遞送服務/遞送方
                                            FulfillChannel = this.ShipType_IntToStr(singleCart.ShipType ?? -1),
                                            DelvServiceType = "", //暫時顯示SBS

                                            InvoiceNumber = singleCart.InvoiceNO,
                                            Receiver = singleCart.Receiver,
                                            RMACode = "", //RMACODE
                                            SellerName = SellerName,

                                            SOCode = singleCart.ID,

                                            FirstProductTitle = string.Format("{0}...", OrderDtails.FirstOrDefault().Title), //ItemName
                                            FirstMenufacturePartNum = string.Format("{0}...", OrderDtails.FirstOrDefault().MenufacturePartNum),
                                            UpdateDate = singleCart.UpdateDate.ToString() ?? DateTime.MinValue.ToString(),
                                            //CustomerMobile = singleCart.Mobile => singleCart.CardMobile 訂購人手機 Modi by Jack.W.Wu 0613
                                            CustomerMobile = singleCart.Mobile, //singleCart.CardMobile, >>0617 Ted改回singleCart.Mobile依據MAIL中要求此應為正確
                                            TotalQty = OrderDtails.Sum(r => r.Qty),

                                            // 修改子項總計，原始的只會計算一筆 OrderDtails.Sum(r => r.Price)
                                            SubtotalPrice = subtotalPrice, //sellersProcessOriginal.Where(x => x.CartID == singleCart.ID).Sum(s => s.Price) ?? 0, 
                                            ShippingFee = shippingFee,
                                            ServiceFee = serviceFee,
                                            TotalPrice = totalPrice, //(sellersProcessOriginal.Where(x => x.CartID == singleCart.ID).Sum(s => s.Price) ?? 0) + shippingFee + serviceFee, // 修改子項總計算

                                            // 增加配送資訊的狀態值 (2014.07.31)
                                            UpdateNote = singleCart.UpdateNote,

                                            // 遞送包裹撈取貨運公司成為獨立API，減少資料傳輸量 Jack.W.Wu 0626
                                            //AllDeliverIDxName = allDeliverIDxName,

                                            //Order Details
                                            OrderDetails = OrderDtails,

                                            //收件人手機 Add by by Jack.W.Wu 0613
                                            ReceiverCellphone = singleCart.RecvMobile, //ReceiverCellphone = singleCart.Mobile,  >>依據要求改為singleCart.RecvMobile, Modified by Ted 0617

                                            //收件人市話 Add by by Jack.W.Wu 0613
                                            ReceiverPhone = singleCart.TelDay, //ReceiverPhone = singleCart.Phone, >>依據要求改為cart.TelDay 20140617 Modi By Ted

                                            //備註 Add by by Jack.W.Wu 0613
                                            Note = singleCart.Note,

                                            //0617依據要求增加此地址資訊(Zipcode/Location) add by ted
                                            ZipCode = singleCart.Zipcode,
                                            Location = singleCart.Location,

                                            // 付款方式
                                            PayType = _PayTypeName,

                                            // 採購單號
                                            POCode = purchaseOrderNumber,
                                            // 依據需求:Vendor看到SO單號 + PO單號；Seller 看到SO單號(隱藏PO單號)
                                            // 說明：Seller/Vendor判定:AccountTypeCode = Models.OrderInfo.EnumAccountTypeCode //Add by Ted

                                            // 訂單編號 + 採購單號
                                            SOCode_POCode = singleCart.ID + " / " + purchaseOrderNumber,

                                            // 營業地址
                                            BusinessAddress = _businessAddress == null ? "" : _businessAddress,
                                            AccountTypeCode = condition.AccountType,

                                            // 營業地址
                                            SellerShippingAddress = sellerShippingAddress,
                                        }
                        #endregion
);
                    }
                }

                // 按照訂單成立順序排列，新成立訂單在前 add by Jack.W.Wu 0626
                //result.Finish(true, 0, "Sucess", Orders.OrderByDescending(x => x.SOCode).Take(500).ToList());
                result.Finish(true, (int)ResponseCode.Success, totalCount.ToString(), Orders);
            }
            catch (Exception ex)
            {
                // result.Finish(true, 0, ex.Message, null);
                // Changed by Jack.W.Wu
                result.Finish(false, (int)ResponseCode.Error, ex.Message, null);
            }

            return result;

        }


        /// <summary>
        /// ActionResponse
        /// 編輯 訂單/(目前為編輯CART資料表)
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public Models.ActionResponse<bool> EditOrderInfo(Models.OrderInfo order)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            try
            {
                #region 必要欄位檢核

                if (string.IsNullOrEmpty(order.SOCode))
                {
                    if (order.OrderDetails != null && order.OrderDetails.Count > 0)
                    {
                        //正常來說, 用一個orderInfo 下的 OrderDetails 中的ProcessID當條件所找到的CartID應該要一樣的, 所以只取第一個
                        order.SOCode = (from crt in backend.Cart
                                        join proc in backend.Process on crt.ID equals proc.CartID
                                        where order.OrderDetails.Select(r => r.ProcessID).Contains(proc.ID)
                                        select crt.ID).FirstOrDefault();
                    }

                    // if(string.IsNullOrEmpty(order.SOCode))
                    //     return error
                }

                #endregion

                #region Edit Cart
                DB.TWBACKENDDB.Models.Cart currentCart = backend.Cart.Where(r => r.ID == order.SOCode).FirstOrDefault();

                currentCart.DelvStatus = (int)order.DelvStatus;
                // 日期已改為字串傳接，序列化不會調整日期時間 DateTime.Now => DateTime.Now。 Jack.W.Wu 140716
                currentCart.DelvStatusDate = DateTime.Now;
                currentCart.UpdateUser = "FackData";
                currentCart.UpdateDate = Convert.ToDateTime(order.UpdateDate);

                #endregion

                #region Edit Product

                #endregion

                #region Edit Process

                #endregion

                backend.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Finish(false, 0, ex.Message, false);
            }

            return result;
            /*
            try
            {
                //DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
                DB.TWSellerPortalDBContext DB_TWSELLERPORTALDB = new DB.TWSellerPortalDBContext();

                #region 方法1:查詢原CART(目前無使用)
                //var qOriginalCart = backend.Cart.Where(x => x.ID == cart.ID).FirstOrDefault();
                //qOriginalCart.DelvStatus = cart.DelvStatus;
                //backend.Entry(cart).Member("ID").CurrentValue
                //backend.Entry(cart). 
                #endregion

                #region 方法2:使用SP 修改CART(目前使用此方法)
                ////cart.
                ////var CartResult = TWSellerPortalDB.Database.SqlQuery<Models.SummarySPResult>("exec SP_RPT_Summary '" + SummarySPSrarch.inputSellerID + "','" + SummarySPSrarch.inputStartDate + "','" + SummarySPSrarch.inputEndDate + "'").ToList();
                ////string sqlcmd = string.Format("exec SP_RPT_Summary '{1}', '{1}', '{2}'", SummarySPSrarch.inputSellerID, SummarySPSrarch.inputStartDate, SummarySPSrarch.inputEndDate);

                //測試:先UPDATE DelvStatus
                DB.TWBACKENDDB.Models.Cart currentCart = backend.Cart.Where(r => r.ID == cart.ID).FirstOrDefault();
                if (currentCart != null)
                {
                    currentCart.DelvStatus = cart.DelvStatus;
                    currentCart.DelvStatusDate = cart.DelvStatusDate;
                    currentCart.UpdateUser = cart.UpdateUser;
                    currentCart.UpdateDate = cart.UpdateDate;
                }
                backend.SaveChanges();

                //string sqlcmd = string.Format("EXEC SP_UPDATE_CART '{0}',{1},'{2:yyyy/MM/dd}','{3}','{4:yyyy/MM/dd}';"
                //                                ,cart.ID //SOcode
                //                                ,cart.DelvStatus //DelvStatus  //備註:cart.DelvStatus 型別為INT
                //                                ,cart.DelvStatusDate //DateTime.Now.ToShortDateString() //DelvStatusDate
                //                                ,cart.UpdateUser  //UpdateUser
                //                                ,cart.UpdateDate //DateTime.Now.ToShortDateString() //UpdateDate
                //                             );
                //DB_TWSELLERPORTALDB.Database.ExecuteSqlCommand(sqlcmd);
                
                result.Msg = "Done";
                result.IsSuccess = true;
                result.Code = 0;
                #endregion


                #region 使用方法1則須要此段CODE(目前不使用)
                //backend.Entry(cart).State = System.Data.EntityState.Modified;
                //backend.Entry(qOriginalCart).State = System.Data.EntityState.Modified;
                //backend.SaveChanges();
                //result.Finish(true, 0, "Done", cart); 
                #endregion
            }
            catch (Exception e)
            {
                result.Finish(false, 0, e.Message, null);
            }

            return result;
            */
        }


        /// <summary>
        /// 更新遞送狀態
        /// </summary>
        /// <returns></returns>
        public Models.ActionResponse<bool> UpdateDelvStatus(string soCode, int delvStatus, string updateUser)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            try
            {
                DB.TWBACKENDDB.Models.Cart currentCart = backend.Cart.Where(r => r.ID == soCode).FirstOrDefault();

                bool canUpdateStatus = this.checkCartStatus(currentCart.Status, currentCart.UpdateDate);

                if (canUpdateStatus == true)
                {
                    // 日期已改為字串傳接，序列化不會調整日期時間 DateTime.Now => DateTime.Now。 Jack.W.Wu 140716
                    currentCart.DelvStatus = delvStatus;
                    currentCart.DelvStatusDate = DateTime.Now;
                    currentCart.UpdateUser = updateUser;
                    currentCart.UpdateDate = DateTime.Now;

                    //更新UpdateNote
                    string delvStatusString = string.Empty;
                    if (string.IsNullOrEmpty(currentCart.UpdateNote))
                    {
                        currentCart.UpdateNote = string.Empty;
                        currentCart.UpdateNote = DateTime.Now + " " + Enum.GetName(typeof(Models.OrderInfo.EnumDelvStatus), delvStatus).ToString() + ".SellerPortal(" + updateUser + ")</br>";
                    }
                    else
                    {
                        currentCart.UpdateNote += DateTime.Now + " " + Enum.GetName(typeof(Models.OrderInfo.EnumDelvStatus), delvStatus).ToString() + ".SellerPortal(" + updateUser + ")</br>";
                    }

                    backend.SaveChanges();
                    result.Finish(true, (int)ResponseCode.Success, "Success", true);
                }
                else
                {
                    result.Finish(false, (int)ResponseCode.Error, "此訂單狀態已經被更新，請重新查詢訂單資料!", false);
                }

                // 2014/10/17 增加LOG紀錄 Add by Ted
                //log.Info("執行UpdateDelvStatus服務，成功與否:");
            }
            catch (Exception ex)
            {
                result.Finish(false, (int)ResponseCode.Error, ex.Message, false);

                // 2014/10/17 增加LOG紀錄 Add by Ted
                //log.Info("執行UpdateDelvStatus服務，成功與否:");
            }

            return result;
        }

        /// <summary>
        /// 檢查訂單狀態是否為正常的
        /// </summary>
        /// <param name="cartStatus">Cart 訂單狀態</param>
        /// <param name="cartUpdateTime">Cart 更新時間</param>
        /// <returns>若可更新則回傳 True </returns>
        public bool checkCartStatus(int? cartStatus, DateTime? cartUpdateTime)
        {
            bool updateCartStatus = false;

            if (cartStatus.HasValue && cartUpdateTime.HasValue)
            {
                if (cartStatus.Value == 0 && cartUpdateTime.Value < DateTime.Now)
                {
                    updateCartStatus = true;
                }
            }

            return updateCartStatus;
        }

        /// <summary>
        /// 目前沒提供connector直接呼叫
        /// </summary>
        /// <param name="processIDs"></param>
        /// <returns></returns>
        public List<string> GetAllSOCodeByProcessIDs(List<string> processIDs)
        {
            if (processIDs != null)
            {
                List<string> currentCartIDs = backend.Process
                                    .Where(r => processIDs.Contains(r.ID))
                                    .Select(r => r.CartID).Distinct().Distinct()
                                    .ToList();
                return currentCartIDs;
            }
            return new List<string>();
        }

        /// <summary>
        /// 遞送包裹
        /// </summary>
        /// <param name="delvTrack"></param>
        /// <returns></returns>
        public Models.ActionResponse<bool> SendPackage(List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> delvTrack)
        {
            //Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> result = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>();
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            try
            {
                List<string> allSOCodes = this.GetAllSOCodeByProcessIDs(delvTrack.Select(r => r.ProcessID).ToList());

                foreach (var code in allSOCodes)
                {
                    DB.TWBACKENDDB.Models.Cart currentCart = backend.Cart.Where(r => r.ID == code).FirstOrDefault();

                    bool canUpdateStatus = this.checkCartStatus(currentCart.Status, currentCart.UpdateDate);

                    if (canUpdateStatus == false)
                    {
                        result.Finish(false, (int)ResponseCode.Error, "此訂單狀態已經被更新，請重新查詢訂單資料!", false);

                        return result;
                    }
                }

                if (delvTrack != null)
                {
                    foreach (var single_DelvTrack in delvTrack)
                    {
                        sellerPortal.Seller_DelvTrack.Add(single_DelvTrack);
                    }

                    sellerPortal.SaveChanges();
                }

                //DB.TWSELLERPORTALDB.Models.Seller_DelvTrack Result_Seller_DelvTrack = new DB.TWSELLERPORTALDB.Models.Seller_DelvTrack();
                result.Finish(true, 0, "Done", true);
            }
            catch (Exception ex)
            {
                result.Finish(false, 1, ex.Message, false);
            }
            return result;
        }

        /// <summary>
        /// MAIL通知廠商 新訂單
        /// </summary>
        /// <param name="inputOrderInfo"></param>
        /// <returns>IsSendMailSuccess</returns>
        public Models.ActionResponse<Models.MailResult> MailSellerNewOrder(API.Models.OrderInfo inputOrderInfo)
        //public Models.ActionResponse<bool> MailSellerNewOrder(API.Models.OrderInfo inputOrderInfo)
        {
            Models.ActionResponse<Models.MailResult> MailSellerNewOrder_MailResult = new Models.ActionResponse<Models.MailResult>();
            //API.Models.MailResult MailSellerNewOrder_MailResult_2 = new Models.MailResult();

            try
            {
                API.Models.Connector connector = new Models.Connector();

                Models.Mail MailContent = new Models.Mail();
                MailContent.UserName = inputOrderInfo.SellerName; //?? MailContent.UserName = ?寄件者
                MailContent.UserEmail = "Ice.C.Lai@newegg.com"; //測試MAIL
                MailContent.MailType = Models.Mail.MailTypeEnum.InformSellerNewSalesOrder;
                //MailContent.RecipientBcc = 
                MailContent.MailMessage = string.Format("{0},{1}", inputOrderInfo.SOCode, inputOrderInfo.CreateDate/*.ToString("yyyy/MM/dd hh:mm")*/);

                MailSellerNewOrder_MailResult = connector.SendMail(null, null, MailContent);

                MailSellerNewOrder_MailResult.Finish(MailSellerNewOrder_MailResult.IsSuccess, MailSellerNewOrder_MailResult.Code, MailSellerNewOrder_MailResult.Msg, MailSellerNewOrder_MailResult.Body);
            }
            catch
            {
                MailSellerNewOrder_MailResult.Finish(MailSellerNewOrder_MailResult.IsSuccess, MailSellerNewOrder_MailResult.Code, MailSellerNewOrder_MailResult.Msg, MailSellerNewOrder_MailResult.Body);
            }

            return MailSellerNewOrder_MailResult;
        }

        /// <summary>
        /// 撈取貨運公司列表
        /// </summary>
        /// <returns></returns>
        public Models.ActionResponse<List<string>> QueryShipCarrier()
        {
            Models.ActionResponse<List<string>> result = new Models.ActionResponse<List<string>>();
            List<string> allDeliverIDxName = new List<string>();

            try
            {
                backend.Deliver.OrderBy(r => r.code).ToList().ForEach(r =>
                {
                    allDeliverIDxName.Add(string.Format("{0}.{1}", r.code, r.Name));
                }
                );

                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "Success";
                result.Body = allDeliverIDxName;
            }
            catch
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "Error";
            }

            return result;
        }

        /// <summary>
        /// 更新訂單狀態(cart.Status)
        /// </summary>
        /// <param name="soCode"></param>
        /// <param name="CartStatus"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Models.ActionResponse<bool> UpdateCartStatus(string soCode, int cartStatus, string updateUser)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            try
            {
                DB.TWBACKENDDB.Models.Cart currentCart = backend.Cart.Where(r => r.ID == soCode).FirstOrDefault();

                currentCart.Status = cartStatus;
                // 日期已改為字串傳接，序列化不會調整日期時間 DateTime.Now => DateTime.Now。 Jack.W.Wu 140716
                currentCart.StatusDate = DateTime.Now;
                currentCart.UpdateUser = updateUser;
                currentCart.UpdateDate = DateTime.Now;

                //更新cart.UpdateNote
                //string delvStatusString = string.Empty;
                if (string.IsNullOrEmpty(currentCart.UpdateNote))
                {
                    currentCart.UpdateNote = string.Empty;
                    currentCart.UpdateNote = DateTime.Now + " " + Enum.GetName(typeof(Models.OrderInfo.EnumCartStatus), cartStatus).ToString() + ".SellerPortal(" + updateUser + ")</br>";
                }
                else
                {
                    currentCart.UpdateNote += DateTime.Now + " " + Enum.GetName(typeof(Models.OrderInfo.EnumCartStatus), cartStatus).ToString() + ".SellerPortal(" + updateUser + ")</br>";
                }

                backend.SaveChanges();
                result.Finish(true, (int)ResponseCode.Success, ResponseCode.Success.ToString(), true);
            }
            catch (Exception ex)
            {
                result.Finish(false, (int)ResponseCode.Error, ex.Message, false);
            }

            return result;
        }

        #region 匯出 Excel

        /// <summary>
        /// 匯出 Excel 訂單列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public API.Models.ActionResponse<string> DownloadSalesOrderList(API.Models.OrderInfo.DownloadSalesOrderListModel dataInfo)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            // 匯出指定的顯示格式的 Excel 檔案
            switch (dataInfo.AccountType)
            {
                // 匯出 Seller 格式
                default:
                case Models.OrderInfo.EnumAccountTypeCode.Seller:
                    {
                        result.Msg = DataToExcel.Export.ListToExcel(ExportSellerExcel(dataInfo), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
                        break;
                    }
                // 匯出 Vendor 格式
                case Models.OrderInfo.EnumAccountTypeCode.Vendor:
                    {
                        result.Msg = DataToExcel.Export.ListToExcel(ExportVendorExcel(dataInfo), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
                        break;
                    }
            }

            // 判斷轉換是否成功
            // 成功：提供下載位置
            // 失敗：顯示錯誤資訊
            if (result.Msg.IndexOf(@"Success") == 0)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;

                // 讀取成功訊息後面的日期檔名
                string saveDate = result.Msg.Substring(8).Trim();

                // 合併下載路徑
                result.Body = string.Format("{0}{1}_{2}.xls", System.Configuration.ConfigurationSettings.AppSettings["ReturnExcel"], dataInfo.fileName, saveDate);

                result.Msg = "Success";
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 轉成的 Seller 的顯示格式
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>Seller 格式的列表</returns>
        private List<API.Models.OrderInfo.ExportSellerExcel> ExportSellerExcel(API.Models.OrderInfo.DownloadSalesOrderListModel dataInfo)
        {
            List<API.Models.OrderInfo.ExportSellerExcel> exportSellerExcel = new List<Models.OrderInfo.ExportSellerExcel>();

            // 判斷是否有傳入資料
            if (dataInfo.dataList != null)
            {
                // 將傳入的資料轉換為 Seller 的顯示格式
                foreach (API.Models.OrderInfo orderInfo in dataInfo.dataList)
                {
                    #region 將配送資訊的出貨時間及配達時間拆出來

                    // 出貨時間
                    string shipDate = string.Empty;

                    // 配達時間
                    string arrivedDate = string.Empty;

                    // 將配送資訊的出貨時間及配達時間拆出來
                    DetachShipDateAndArrivedDate(orderInfo, out shipDate, out arrivedDate);

                    #endregion 將配送資訊的出貨時間及配達時間拆出來

                    // 讀取配送時間，並刪除配送時間後面的 </arrive> 字樣
                    string delvDate = DeleteEndArriveTag(orderInfo);

                    exportSellerExcel.Add(new API.Models.OrderInfo.ExportSellerExcel
                    {
                        // 商家 ID
                        SellerID = dataInfo.SellerID,

                        //訂單狀態
                        OrderStatus = orderInfo.DelvStatusStr,

                        // 出貨方
                        ShipType = orderInfo.FulfillChannel,

                        // 購物車編號
                        SalesOrderGroupID = orderInfo.SaleOrderGroupID,

                        // 出貨提單編號
                        TrackingNumber = orderInfo.OrderDetails.FirstOrDefault().TrackingNumber,

                        // 貨運公司
                        DelvName = orderInfo.OrderDetails.FirstOrDefault().DelvName,

                        // 出貨時間
                        ShipDate = shipDate,

                        // 配達時間
                        ArriveDate = arrivedDate,

                        // 單據日期
                        CreateDate = orderInfo.CreateDate,

                        // 客戶訂單編號
                        SOCode = orderInfo.SOCode,

                        // 供應商產品編號
                        SellerProductID = orderInfo.OrderDetails.FirstOrDefault().SellerProductID,

                        // 台蛋產品編號
                        NeweggPartNum = orderInfo.OrderDetails.FirstOrDefault().NeweggPartNum,

                        // 付款方式
                        PayType = orderInfo.PayType,

                        // 商品名稱
                        ItemName = orderInfo.OrderDetails.FirstOrDefault().ItemName,

                        // 數量
                        Qty = orderInfo.OrderDetails.FirstOrDefault().Qty,

                        // 單價
                        Price = orderInfo.OrderDetails.FirstOrDefault().Price,

                        // 總價
                        TotalPrice = orderInfo.SubtotalPrice,

                        // 訂購人姓名
                        Receiver = orderInfo.Receiver,

                        // 郵遞區號
                        ZipCode = orderInfo.ZipCode,

                        // 縣市
                        Location = orderInfo.Location,

                        // 地址
                        Address = orderInfo.Address,

                        // 聯絡電話
                        ReceiverCellphone = orderInfo.ReceiverCellphone,

                        // 配送時間 (客戶指定的到貨時間)
                        DelvDate = delvDate,

                        // 備註
                        Note = orderInfo.Note
                    });
                }
            }

            return exportSellerExcel;
        }

        /// <summary>
        /// 轉成的 Vendor 的顯示格式
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>Vendor 格式的列表</returns>
        private List<API.Models.OrderInfo.ExportVendorExcel> ExportVendorExcel(API.Models.OrderInfo.DownloadSalesOrderListModel dataInfo)
        {
            List<API.Models.OrderInfo.ExportVendorExcel> exportVendorExcel = new List<Models.OrderInfo.ExportVendorExcel>();

            // 判斷是否有傳入資料
            if (dataInfo.dataList != null)
            {
                // 將傳入的資料轉換為 Vendor 的顯示格式
                foreach (API.Models.OrderInfo orderInfo in dataInfo.dataList)
                {
                    #region 將配送資訊的出貨時間及配達時間拆出來

                    // 出貨時間
                    string shipDate = string.Empty;

                    // 配達時間
                    string arrivedDate = string.Empty;

                    // 將配送資訊的出貨時間及配達時間拆出來
                    DetachShipDateAndArrivedDate(orderInfo, out shipDate, out arrivedDate);

                    #endregion 將配送資訊的出貨時間及配達時間拆出來

                    // 讀取配送時間，並刪除配送時間後面的 </arrive> 字樣
                    string delvDate = DeleteEndArriveTag(orderInfo);

                    exportVendorExcel.Add(new API.Models.OrderInfo.ExportVendorExcel
                    {
                        // 商家 ID
                        SellerID = dataInfo.SellerID,

                        //訂單狀態
                        OrderStatus = orderInfo.DelvStatusStr,

                        // 出貨方
                        ShipType = orderInfo.FulfillChannel,

                        // 購物車編號
                        SalesOrderGroupID = orderInfo.SaleOrderGroupID,

                        // 出貨提單編號
                        TrackingNumber = orderInfo.OrderDetails.FirstOrDefault().TrackingNumber,

                        // 貨運公司
                        DelvName = orderInfo.OrderDetails.FirstOrDefault().DelvName,

                        // 出貨時間
                        ShipDate = shipDate,

                        // 配達時間
                        ArriveDate = arrivedDate,

                        // 單據日期
                        CreateDate = orderInfo.CreateDate,

                        // 客戶訂單編號
                        SOCode = orderInfo.SOCode,

                        // 供應商訂單編號
                        POCode = orderInfo.POCode,

                        // 供應商產品編號
                        SellerProductID = orderInfo.OrderDetails.FirstOrDefault().SellerProductID,

                        // 台蛋產品編號
                        NeweggPartNum = orderInfo.OrderDetails.FirstOrDefault().NeweggPartNum,

                        // 商品名稱
                        ItemName = orderInfo.OrderDetails.FirstOrDefault().ItemName,

                        // 數量
                        Qty = orderInfo.OrderDetails.FirstOrDefault().Qty,

                        // 單位成本
                        UnitCost = orderInfo.OrderDetails.FirstOrDefault().UnitCost,

                        // 總成本
                        TotalCost = orderInfo.OrderDetails.FirstOrDefault().TotalCost,

                        // 訂購人姓名
                        Receiver = orderInfo.Receiver,

                        // 郵遞區號
                        ZipCode = orderInfo.ZipCode,

                        // 縣市
                        Location = orderInfo.Location,

                        // 地址
                        Address = orderInfo.Address,

                        // 聯絡電話
                        ReceiverCellphone = orderInfo.ReceiverCellphone,

                        // 配送時間 (客戶指定的到貨時間)
                        DelvDate = delvDate,

                        // 備註
                        Note = orderInfo.Note
                    });
                }
            }

            return exportVendorExcel;
        }

        /// <summary>
        /// 將配送資訊的出貨時間及配達時間拆出來
        /// </summary>
        /// <param name="orderInfo">訂單資訊</param>
        /// <param name="shipDate">出貨時間</param>
        /// <param name="arrivedDate">配達時間</param>
        private void DetachShipDateAndArrivedDate(API.Models.OrderInfo orderInfo, out string shipDate, out string arrivedDate)
        {
            // 配送資訊的狀態值 (ex：2/7/2014 4:06:51 PM訂單成立,System</br>2/7/2014 4:10:10 PM系統轉單,System</br>2/7/2014 5:16:11 PM已出貨HCT新竹貨運</br>2/10/2014 10:27:55 AM配達,HCT新竹貨運</br>)
            string updateNote = string.IsNullOrEmpty(orderInfo.UpdateNote) ? string.Empty : orderInfo.UpdateNote;

            // 配送資訊的狀態值轉為 List 的暫存空間
            List<string> updateNoteList = new List<string>();

            // 出貨時間
            shipDate = string.Empty;

            // 配達時間
            arrivedDate = string.Empty;

            // 判斷是否有要拆的狀態值
            if (updateNote.IndexOf(@">") != -1)
            {
                // 將配送資訊的狀態值，轉為 List 的方式暫存
                do
                {
                    // 讀取每個狀態值的斷點
                    int access_length = updateNote.IndexOf(@">") + 1;

                    // 將狀態值寫入 List 中
                    updateNoteList.Add(updateNote.Substring(0, access_length));

                    // 刪除已寫入的狀態值
                    updateNote = updateNote.Remove(0, access_length);
                }
                while (updateNote.IndexOf(@">") != -1);


                // 從狀態值列表中，分別讀取出 出貨時間 及 配達時間
                foreach (string item in updateNoteList)
                {
                    if (item.IndexOf(@"已出貨") != -1)
                    {
                        shipDate = item.Substring(0, item.IndexOf(@"已出貨"));
                    }

                    if (item.IndexOf(@"配達") != -1)
                    {
                        arrivedDate = item.Substring(0, item.IndexOf(@"配達"));
                    }
                }
            }
        }

        /// <summary>
        /// 刪除配送時間後面的 html 文字
        /// </summary>
        /// <param name="orderInfo">訂單資訊</param>
        /// <returns>配送時間</returns>
        private string DeleteEndArriveTag(API.Models.OrderInfo orderInfo)
        {
            // 讀取配送時間
            string delvDate = orderInfo.OrderDetails.FirstOrDefault().DelvDate;

            // 若配送時間有值，才進行刪除 </arrive> 字樣
            if (!string.IsNullOrEmpty(delvDate))
            {
                // 讀取切割長度
                int slip_leght = delvDate.IndexOf(@"</arrive>");
                if (slip_leght != -1)
                {
                    delvDate = delvDate.Substring(0, slip_leght);
                }
                // 將 </arrive> 之前的內容，重新寫入

            }

            return delvDate;
        }

        #endregion 匯出 Excel

        /// <summary>
        /// 檢查是否有新訂單兩天以上卻尚未出貨，並寄出信件給Seller、新蛋管理人員
        /// </summary>
        /// <remarks>
        /// 若MailSeller = true將會寄信給seller email。開發測試時期 MailSeller 請設定為false。預設值目前設定為false避免將信件直接寄出。
        /// </remarks>
        /// <returns></returns>
        public Models.ActionResponse<List<Models.UnShipList>> Mail_RemindSellerToSendPackage(bool MailSeller = false)
        {
            #region 找出所有需要寄送的mail address List
            Models.ActionResponse<List<Models.UnShipList>> mailResult_MailRemindSellerToSendPackage = new Models.ActionResponse<List<Models.UnShipList>>();
            Dictionary<int, int> tmp_result = new Dictionary<int, int>();
            Dictionary<string, int> cart_productID = new Dictionary<string, int>();

            List<int> productIDList = new List<int>();
            List<string> tmp_cartID = new List<string>();
            List<Models.UnShipList> shipList = new List<Models.UnShipList>();

            DateTime today = new DateTime();

            today = DateTime.Now;

            // 取得今日時間
            //DateTime.TryParse(DateTime.Now.ToString("yyyy-MM-dd"), out today);
            // 比對 "已成立" 及 "待出貨"
            var tmp_preshipInfo = backend.Cart.Where(x => (x.DelvStatus == 0 || x.DelvStatus == 6) && x.DelivNO == null && x.Status != 1).Select(r => new { r.ID, r.CreateDate }).ToList();

            // 找出建立日期 >= 2 的訂單
            //tmp_preshipInfo = tmp_preshipInfo.Where(x => TimeSpan(today - x.CreateDate.Value) >= 0).ToList();
            foreach (var cartInfo in tmp_preshipInfo)
            {
                TimeSpan t1 = new TimeSpan(cartInfo.CreateDate.Value.Ticks);
                TimeSpan t2 = new TimeSpan(today.Ticks);

                double ts = (t2 - t1).TotalDays;

                //if( DateTime.Compare(today,cartInfo.CreateDate.Value)>0)
                //    tmp_cartID.Add(cartInfo.ID);

                if (ts >= 2)
                    tmp_cartID.Add(cartInfo.ID);
            }

            // 利用 CartID 至 Process 內找出 ProductID 
            foreach (var cartID in tmp_cartID)
            {
                //int? productID = backend.Process.Where(x => x.CartID == cartID).Select(r => r.ProductID).FirstOrDefault();
                var test = backend.Process.Where(x => x.CartID == cartID).Select(r => new { r.CartID, r.ProductID }).FirstOrDefault();

                if (cart_productID.Keys.Contains(test.CartID) == false && test.ProductID != 13189 && test.ProductID != 13190)
                {
                    cart_productID.Add(test.CartID, test.ProductID.Value);
                }

                //if (productID.HasValue)
                //    productIDList.Add(productID.Value);
            }
            // 排除 Process 內重複的 ProductID 
            //productIDList = productIDList.Distinct().ToList();


            foreach (var id in cart_productID)
            {
                int sellerID = frontend.Product.Where(x => x.ID == id.Value).Select(r => r.SellerID).FirstOrDefault();
                // 利用 productID 來 count seller 有多少訂單未押入 Tracking No.
                if (tmp_result.Keys.Contains(sellerID) == true)
                {
                    tmp_result[sellerID]++;
                }
                else
                {
                    tmp_result.Add(sellerID, 1);
                }
            }

            foreach (var item in tmp_result)
            {
                Models.UnShipList shipInfo = new Models.UnShipList();

                var tmp_sellerInfo = sellerPortal.Seller_BasicInfo.Where(x => x.SellerID == item.Key).Select(r => new { r.SellerEmail, r.SellerName, r.SellerID }).FirstOrDefault();

                if (tmp_sellerInfo != null)
                {
                    shipInfo.SellerID = tmp_sellerInfo.SellerID;
                    shipInfo.SellerEmail = tmp_sellerInfo.SellerEmail;
                    shipInfo.SellerName = tmp_sellerInfo.SellerName;
                    shipInfo.Unshipcount = tmp_result[item.Key];

                    shipList.Add(shipInfo);
                }
            }
            #endregion



            //寄送信件用相關MODEL與 服務

            API.Models.Connector connector = new Models.Connector();
            Models.Mail mailContent = new Models.Mail();
            Models.MailResult mailResult = new Models.MailResult();



            #region 找出管理員email address
            string adminEmailStr = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            string[] adminEmailS = adminEmailStr.Split(',');
            #endregion



            #region 寄送信件給seller
            if (MailSeller == true)
            {
                foreach (var SingleShipList in shipList)
                {
                    // 先排除不寄送的email address
                    string emailsNotToSend = connector.GetAPIWebConfigSetting("VoidProcessEmailAddress");
                    var voidemail = emailsNotToSend.Split(',').ToList();
                    if (voidemail.Contains(SingleShipList.SellerEmail))
                    {
                        log.Info("此信箱收件人屬於不處理清單內，Email: " + SingleShipList.SellerEmail + "未寄送。");

                        mailResult_MailRemindSellerToSendPackage.IsSuccess = true;
                        mailResult_MailRemindSellerToSendPackage.Msg = "此信箱收件人屬於不處理清單內，Email: " + SingleShipList.SellerEmail;
                        mailResult_MailRemindSellerToSendPackage.Code = (int)ResponseCode.Success;
                        mailResult_MailRemindSellerToSendPackage.Body = null;
                    }
                    else
                    {
                        #region 填寫信件內容
                        mailContent.IsAdmin = false;
                        mailContent.UserEmail = SingleShipList.SellerEmail;
                        mailContent.MailType = Models.Mail.MailTypeEnum.RemindSellerToSendPackage;
                        //mailContent.RecipientBcc //目前未使用
                        mailContent.UserName = SingleShipList.SellerName;

                        List<API.Models.UnShipList> unshipList_temp = new List<Models.UnShipList>();
                        unshipList_temp.Add(new Models.UnShipList()
                            {
                                SellerEmail = SingleShipList.SellerEmail
                                ,
                                SellerID = SingleShipList.SellerID
                                ,
                                SellerName = SingleShipList.SellerName
                                ,
                                Unshipcount = SingleShipList.Unshipcount
                            });
                        //mailContent.MailMessage =  + "," + ;

                        mailContent.UnshipList = unshipList_temp;
                        #endregion

                        // 寄出 Mail
                        Thread.Sleep(1000);
                        var mailResult_mailSeller = connector.SendMail(null, null, mailContent);
                        mailResult_MailRemindSellerToSendPackage.Finish(mailResult_mailSeller.IsSuccess, mailResult_mailSeller.Code, mailResult_mailSeller.Msg, null);
                    }


                    #region 記錄訊息的抬頭文字
                    string logTitle = string.Empty;
                    logTitle = "出貨提醒通知信";
                    #endregion


                    // 判斷信件是否寄送成功，若失敗則再另外寄信通知管理人
                    if (mailResult_MailRemindSellerToSendPackage.IsSuccess)
                    {
                        //this.sendMailResult.Add(logTitle + _mailAddrIndex.ToString(), string.Format("{0} 寄送成功。", singleMailAddr));
                        mailResult_MailRemindSellerToSendPackage.Msg += SingleShipList.SellerID + logTitle + SingleShipList.SellerEmail;
                    }
                    else
                    {
                        // 寄送失敗
                        mailResult_MailRemindSellerToSendPackage.Msg += SingleShipList.SellerID + logTitle + SingleShipList.SellerEmail + "寄送失敗;";

                        // 寄信通知管理員
                        mailContent.MailMessage = mailResult_MailRemindSellerToSendPackage.Msg;
                        foreach (var mailAddress in adminEmailS)
                        {
                            mailContent.UserEmail = mailAddress;
                            mailContent.MailType = Models.Mail.MailTypeEnum.ErrorInfo;
                            Thread.Sleep(1000);
                            var _tmpMailResult = connector.SendMail(null, null, mailContent);

                            //LOG
                            log.Info("提醒出貨通知信" + mailAddress + _tmpMailResult.IsSuccess);
                        }
                    }
                }
            }
            #endregion

            #region 寄一份提醒通知出貨的清單給管理員
            foreach (var mailAddress in adminEmailS)
            {
                mailContent.IsAdmin = true;
                mailContent.UnshipList = shipList;

                mailContent.MailMessage = string.Format("{0},{1}", "", mailContent.UnshipList.Sum(x => x.Unshipcount));

                mailContent.UserEmail = mailAddress;
                mailContent.UserName = (string.IsNullOrEmpty(mailAddress) ? "管理員@newegg.com" : mailAddress).Split('@')[0];
                mailContent.MailType = Models.Mail.MailTypeEnum.RemindSellerToSendPackage;

                Thread.Sleep(1000);
                var mailResult_RemindSellerSendPackage_ListForAdmin = connector.SendMail(null, null, mailContent);

                //LOG
                log.Info("提醒出貨通知信--給管理員" + mailAddress + mailResult_RemindSellerSendPackage_ListForAdmin.IsSuccess);
            }
            #endregion


            mailResult_MailRemindSellerToSendPackage.Body = shipList;
            return mailResult_MailRemindSellerToSendPackage;
        }

        /// <summary>
        /// 配達(日後需要時才會CHECK IN使用)
        /// </summary>
        /// <param name="soCode"></param>
        /// <param name="delvStatus"></param>
        /// <param name="cartStatus"></param>
        /// <param name="sellerID"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Models.ActionResponse<List<Models.OrderInfo>> Arrival_RollbackVersion(string soCode, int delvStatus, int cartStatus, string sellerID, string updateUser)
        {
            Models.ActionResponse<List<Models.OrderInfo>> result = new Models.ActionResponse<List<Models.OrderInfo>>();
            Models.ActionResponse<bool> updateDelvStatusResult = new Models.ActionResponse<bool>();
            Models.ActionResponse<bool> updateCartStatusResult = new Models.ActionResponse<bool>();


            // 遞送狀態 訂單狀態 須一起成功修改
            using (TransactionScope scope = new TransactionScope())
            {
                //修改遞送狀態
                updateDelvStatusResult = this.UpdateDelvStatus(soCode, delvStatus, updateUser);
                log.Info("配達功能 DelvStatus更新是否成功:" + updateDelvStatusResult.IsSuccess);

                //修改訂單狀態
                updateCartStatusResult = this.UpdateCartStatus(soCode, cartStatus, updateUser);
                log.Info("配達功能 CartStatus更新是否成功:" + updateCartStatusResult.IsSuccess);

                // 遞送狀態 訂單狀態 須一起成功修改 否則一起rollback
                if (result.IsSuccess = updateDelvStatusResult.IsSuccess && updateCartStatusResult.IsSuccess)
                {
                    scope.Complete();
                }
            }

            // (不論是否修改/執行成功)取得目前完整訂單資訊/OrderInfo)
            Models.QueryCartCondition condition = new Models.QueryCartCondition()
            {
                SOCode = soCode,
                SellerID = sellerID
            };


            #region ActionResult/填寫回傳資訊
            result.Finish((updateDelvStatusResult.IsSuccess && updateCartStatusResult.IsSuccess),
                            (updateDelvStatusResult.IsSuccess && updateCartStatusResult.IsSuccess) ? (int)ResponseCode.Success : (int)ResponseCode.Error,
                            updateDelvStatusResult.Msg + "/" + updateCartStatusResult.Msg,
                            this.QueryOrderInfos(condition).Body);

            // LOG
            log.Info("配達功能執行是否成功:" + result.IsSuccess + "/" + result.Msg);
            #endregion

            return result;
        }

        #region 其他函數
        /// <summary>
        /// 供貨通路/遞送方/
        /// </summary>
        /// <param name="str"></param>
        /// <remarks>
        ///     因規格中，多個數值對應到同一名稱，使用列舉可能會有問題，故寫此函數。
        /// </remarks>
        /// <returns></returns>
        private string ShipType_IntToStr(int shipType_int)
        {
            switch (shipType_int)
            {
                //case 0: { return "SBN"; }
                //case 1: { return "SBN"; }
                //case 2: { return "SBS"; }
                //case 3: { return "SBS"; }
                //case 4: { return "SBV"; }
                //case 5: { return "SBN"; }
                //case 6: { return "SBSPEX"; }
                //case 7: { return "SBV"; }
                //case 8: { return "SBN"; }
                //case 9: { return "SBN"; }
                case 0: { return "Newegg"; }
                case 1: { return "Newegg"; }
                case 2: { return "Seller"; }
                case 3: { return "Seller"; }
                case 4: { return "供應商"; }
                case 5: { return "Newegg"; }
                case 6: { return "SBSPEX"; }
                case 7: { return "供應商"; }
                case 8: { return "Newegg"; }
                case 9: { return "Newegg"; }
                default: { return string.Format("(錯誤:輸入的shipType{0}不在規定內)", shipType_int); }
            }
        }
        #endregion
    }
}
