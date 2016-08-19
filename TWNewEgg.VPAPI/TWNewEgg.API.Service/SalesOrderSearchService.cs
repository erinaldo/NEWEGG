using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    public class SalesOrderSearchService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private string webSite = System.Configuration.ConfigurationManager.AppSettings["WebSite"];
        #region 特殊費用

        // 國際運費: productID = 13189
        private readonly int ShippingFeeID = 13189;

        // 服務費: productID = 13190
        private readonly int ServiceFeeID = 13190;

        /// <summary>
        /// 取得特殊費用 ID
        /// </summary>
        /// <returns>特殊費用 ID 清單</returns>
        private List<int> GetSpecialFee()
        {
            return new List<int>() { ShippingFeeID, ServiceFeeID };
        }

        #endregion 特殊費用

        #region 主單

        /// <summary>
        /// 取得訂單主單清單
        /// </summary>
        /// <param name="searchCondition">搜尋絛件</param>
        /// <returns>訂單主單清單</returns>
        public ActionResponse<MainOrderResult> GetMainOrder(MainOrderSearchCondition searchCondition)
        {
            #region 變數宣告

            ActionResponse<MainOrderResult> result = new ActionResponse<MainOrderResult>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new MainOrderResult());
            
            // 商品編號清單
            ActionResponse<List<int>> productIdCell = new ActionResponse<List<int>>();

            // 訂單子單清單
            ActionResponse<List<MainOrder_Process>> processCell = new ActionResponse<List<MainOrder_Process>>();

            // 訂單主單編號清單
            List<string> cartIdCell = new List<string>();

            ActionResponse<List<MainOrder>> mainOrderCell = new ActionResponse<List<MainOrder>>();

            // 連接取得商家名稱的 Service
            SellerBasicInfoService sellerBasicInfoService = new SellerBasicInfoService();

            // 商家名稱
            ActionResponse<string> sellerName = new ActionResponse<string>();

            // 要顯示的商家名稱 = 商家名稱 + (商家編號)
            string seller = string.Empty;

            // 採購單編號清單
            ActionResponse<List<MainOrder_PurchaseOrder>> poCodeCell = new ActionResponse<List<MainOrder_PurchaseOrder>>();

            // 商家商品編號清單
            ActionResponse<List<MainOrder_Product>> sellerProductIDCell = new ActionResponse<List<MainOrder_Product>>();

            #endregion 變數宣告

            #region 讀取訂單資料

            #region 取得某商家的商品編號清單

            productIdCell = this.GetProductIdCell(searchCondition.SellerID, searchCondition.KeyWordSearchType, searchCondition.KeyWord);

            if (productIdCell == null || productIdCell.IsSuccess == false || productIdCell.Body == null)
            {
                if (productIdCell == null)
                {
                    logger.Info(string.Format("取得某商家的商品編號清單失敗; ErrorMessage = {0}; SellerID = {1}.", "ActionResponse is null", searchCondition.SellerID));
                }

                result.Finish(false, (int)ResponseCode.Error, null, null);
                return result;
            }
            else
            {
                // 查無商品編號清單，直接回傳無資料
                if (productIdCell.Body.Count == 0)
                {
                    result.Msg = "查無資料";
                    return result;
                }
            }

            #endregion 取得某商家的商品編號清單

            #region 取得訂單子單清單

            // 取得訂單子單清單
            processCell = GetProcessCell(productIdCell.Body, searchCondition.KeyWordSearchType, searchCondition.KeyWord);

            // 取得訂單子單清單後，清除商品編號清單，釋放 Memory
            productIdCell = null;

            if (processCell == null || processCell.IsSuccess == false || processCell.Body == null)
            {
                if (processCell == null)
                {
                    logger.Info(string.Format("取得訂單子單清單; ErrorMessage = {0}.", "ActionResponse is null"));
                }

                result.Finish(false, (int)ResponseCode.Error, null, null);
                return result;
            }
            else
            {
                // 查無訂單子單清單，直接回傳無資料
                if (processCell.Body.Count == 0)
                {
                    result.Msg = "查無資料";
                    return result;
                }
            }

            #endregion 取得訂單子單清單

            #region 取得訂單主單

            // 取得訂單主單編號清單
            cartIdCell = processCell.Body.Select(x => x.CartID).ToList();

            // 取得訂單主單
            mainOrderCell = GetCartCell(cartIdCell);

            if (mainOrderCell == null || mainOrderCell.IsSuccess == false || mainOrderCell.Body == null)
            {
                if (mainOrderCell == null)
                {
                    logger.Info(string.Format("取得訂單主單失敗; ErrorMessage = {0}.", "ActionResponse is null"));
                }

                result.Finish(false, (int)ResponseCode.Error, null, null);
                return result;
            }
            else
            {
                // 查無訂單主單清單，直接回傳無資料
                if (mainOrderCell.Body.Count == 0)
                {
                    result.Msg = "查無資料";
                    result.Code = (int)ResponseCode.Success;
                    return result;
                }
                else
                {
                    result.Body.Grid = mainOrderCell.Body;

                    // 取得訂單主單清單後，清除暫存資料，釋放 Memory
                    mainOrderCell = null;
                }
            }

            #endregion 取得訂單主單

            #endregion 讀取訂單資料

            #region 依搜尋條件篩選

            #region 依關鍵字篩選

            if (!string.IsNullOrEmpty(searchCondition.KeyWord))
            {
                switch (searchCondition.KeyWordSearchType)
                {
                    default:
                    case (int)OrderKeyWordSearchType.訂單編號:
                        {
                            // 在取得訂單子單清單時篩選(因為和商品名稱所搜尋的 DB Table 都是 Process，所以就移過去做在一起)
                            //result.Body = result.Body.Where(x => x.CartID == searchCondition.KeyWord).ToList();
                            break;
                        }
                    case (int)OrderKeyWordSearchType.訂購人姓名:
                        {
                            result.Body.Grid = result.Body.Grid.Where(x => x.UserName == searchCondition.KeyWord).ToList();
                            break;
                        }
                    case (int)OrderKeyWordSearchType.新蛋商品編號:
                        {
                            // 在取得某商家的商品編號清單時篩選(此時此欄位還尚未填入資料，與其再去撈 DB，還不如在取得商品編號清單時就先篩選掉)
                            //result.Body = FilterByProductID(result.Body, searchCondition.KeyWord);
                            break;
                        }
                    case (int)OrderKeyWordSearchType.商家商品編號:
                        {
                            // 在取得某商家的商品編號清單時篩選(此時此欄位還尚未填入資料，與其再去撈 DB，還不如在取得商品編號清單時就先篩選掉)
                            //result.Body = result.Body.Where(x => x.SellerProductID == searchCondition.KeyWord).ToList();
                            break;
                        }
                    case (int)OrderKeyWordSearchType.商品名稱:
                        {
                            // 在取得訂單子單清單時篩選(此時此欄位還尚未填入資料，與其再去撈 DB，還不如在取得商品編號清單時就先篩選掉)
                            //result.Body = result.Body.Where(x => x.ProcessTitle.IndexOf(searchCondition.KeyWord) != -1).ToList();
                            break;
                        }
                }

                // 篩選完若已無資料，則回直接回傳無資料
                if (result.Body.Grid.Count == 0)
                {
                    result.Msg = "查無資料";
                    result.Code = (int)ResponseCode.Success;
                    return result;
                }
            }

            #endregion 依關鍵字篩選

            #region 依訂單狀態篩選

            switch (searchCondition.OrderStatus)
            {
                default:
                case (int)MainOrderStatus.全部:
                    {
                        break;
                    }
                case (int)MainOrderStatus.已成立:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.DelvStatus.Value == (int)Cart.cartstatus.已成立 && x.Status != (int)Cart.status.取消).ToList();
                        break;
                    }
                case (int)MainOrderStatus.待出貨:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.DelvStatus.Value == (int)Cart.cartstatus.待出貨 && x.Status != (int)Cart.status.取消).ToList();
                        break;
                    }
                case (int)MainOrderStatus.已出貨:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.DelvStatus.Value == (int)Cart.cartstatus.已出貨 && x.Status != (int)Cart.status.退貨).ToList();
                        break;
                    }
                case (int)MainOrderStatus.配達:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.Status == (int)Cart.status.完成).ToList();
                        break;
                    }
                case (int)MainOrderStatus.取消:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.Status.Value == (int)Cart.status.取消).ToList();
                        break;
                    }
                case (int)MainOrderStatus.退貨:
                    {
                        result.Body.Grid = result.Body.Grid.Where(x => x.Status.Value == (int)Cart.status.退貨).ToList();
                        break;
                    }
            }

            // 篩選完若已無資料，則回直接回傳無資料
            if (result.Body.Grid.Count == 0)
            {
                result.Msg = "查無資料";
                result.Code = (int)ResponseCode.Success;
                return result;
            }

            #endregion 依出貨狀態篩選

            #region 依訂單日期篩選

            DateTime? startDate = null;
            DateTime? endDate = null;

            switch (searchCondition.CreateDateSearchType)
            {
                default:
                case (int)OrderCreateDateSearchType.全部:
                    {
                        break;
                    }
                case (int)OrderCreateDateSearchType.今天:
                    {
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1);
                        break;
                    }
                case (int)OrderCreateDateSearchType.最近3天:
                    {
                        startDate = DateTime.Today.AddDays(-2);
                        endDate = DateTime.Today.AddDays(1);
                        break;
                    }
                case (int)OrderCreateDateSearchType.最近7天:
                    {
                        startDate = DateTime.Today.AddDays(-6);
                        endDate = DateTime.Today.AddDays(1);
                        break;
                    }
                case (int)OrderCreateDateSearchType.最近30天:
                    {
                        startDate = DateTime.Today.AddDays(-29);
                        endDate = DateTime.Today.AddDays(1);
                        break;
                    }
                case (int)OrderCreateDateSearchType.指定日期:
                    {
                        startDate = searchCondition.StartDate.Value.ToLocalTime().Date;
                        endDate = searchCondition.StartDate.Value.ToLocalTime().Date.AddDays(1);
                        break;
                    }
                case (int)OrderCreateDateSearchType.定制日期範圍:
                    {
                        startDate = searchCondition.StartDate.Value.ToLocalTime().Date;
                        endDate = searchCondition.EndDate.Value.ToLocalTime().Date.AddDays(1);
                        break;
                    }
            }

            if (searchCondition.CreateDateSearchType >= 1 && searchCondition.CreateDateSearchType <= 6)
            {
                result.Body.Grid = result.Body.Grid.Where(x => x.CreateDate >= startDate && x.CreateDate < endDate).ToList();
            }

            // 篩選完若已無資料，則回直接回傳無資料
            if (result.Body.Grid.Count == 0)
            {
                result.Msg = "查無資料";
                result.Code = (int)ResponseCode.Success;
                return result;
            }

            #endregion 依訂單日期篩選

            #region 依分頁資訊篩選

            // 輸出還沒分頁前的資料筆數
            result.Body.DataCount = result.Body.Grid.Count;

            // 將查詢結果，依訂單日期，做降冪排序
            result.Body.Grid = result.Body.Grid.OrderByDescending(x => x.CreateDate).ToList();

            #region 排除錯誤的頁數、筆數設定並設為預設值

            if (searchCondition.PageInfo.PageIndex < 0)
            {
                searchCondition.PageInfo.PageIndex = 0;
            }

            if (searchCondition.PageInfo.PageSize < 5)
            {
                searchCondition.PageInfo.PageSize = 10;
            }

            #endregion 排除錯誤的頁數、筆數設定並設為預設值

            result.Body.Grid = result.Body.Grid.Skip(searchCondition.PageInfo.PageIndex * searchCondition.PageInfo.PageSize).Take(searchCondition.PageInfo.PageSize).ToList();

            #endregion 依分頁資訊篩選

            #endregion 依搜尋條件篩選

            #region 組合資料

            #region 讀取並組合商家名稱

            sellerName = sellerBasicInfoService.GetSellerNameBySellerID(searchCondition.SellerID);

            if (sellerName != null && sellerName.IsSuccess == true && string.IsNullOrEmpty(sellerName.Body) == false)
            {
                seller = sellerName.Body;
            }

            seller += "(" + searchCondition.SellerID + ")";

            #endregion 讀取並組合商家名稱

            #region 取得採購單編號清單

            cartIdCell = new List<string>();
            cartIdCell = result.Body.Grid.Select(x => x.CartID).ToList();

            poCodeCell = this.GetPOCodeCell(cartIdCell);

            if (poCodeCell == null || poCodeCell.IsSuccess == false || poCodeCell.Body == null)
            {
                if (poCodeCell == null)
                {
                    logger.Info(string.Format("取得採購單編號清單失敗; ErrorMessage = {0}.", "ActionResponse is null"));
                }

                result.Finish(false, (int)ResponseCode.Error, null, null);
                return result;
            }
            
            #endregion 取得採購單編號清單

            #region 取得商家商品編號清單

            productIdCell = new ActionResponse<List<int>>();
            productIdCell.Body = new List<int>();

            // 此時新蛋商品編號，還尚未填值，因此先由 cartIdCell 去 processCell 中，找要查商家商品編號的 ProductID 
            productIdCell.Body = processCell.Body.Where(x => cartIdCell.Contains(x.CartID)).Select(x => x.ProductID).ToList();

            sellerProductIDCell = this.GetSellerProductIDCell(productIdCell.Body);
       
            #endregion 取得商家商品編號清單

            foreach (MainOrder mainOrder in result.Body.Grid)
            {
                #region 數量

                mainOrder.Qty = processCell.Body.Where(x => x.CartID == mainOrder.CartID).Select(x => x.Qty.Value).First();

                #endregion 數量

                #region 商品名稱

                mainOrder.ProcessTitle = processCell.Body.Where(x => x.CartID == mainOrder.CartID).Select(x => x.Title).First();

                #endregion 商品名稱

                #region 賣場Url

                mainOrder.ItemUrl = processCell.Body.Where(x => x.CartID == mainOrder.CartID).Select(x => x.ItemUrl).First();

                #endregion 賣場Url

                #region 採購單編號

                if (poCodeCell.Body.Any(x => x.CartID == mainOrder.CartID))
                {
                    mainOrder.POCode = poCodeCell.Body.Where(x => x.CartID == mainOrder.CartID).Select(x => x.Code).First();
                }

                #endregion 採購單編號

                #region 新蛋商品編號

                mainOrder.ProductID = processCell.Body.Where(x => x.CartID == mainOrder.CartID).Select(x => x.ProductID).First();

                #endregion 新蛋商品編號

                #region 商家商品編號

                if ((sellerProductIDCell != null && sellerProductIDCell.IsSuccess == true && sellerProductIDCell.Body != null) && sellerProductIDCell.Body.Any(x => x.ProductID == mainOrder.ProductID))
                {
                    // 商家商品編號，是由新蛋商品編號去查的，所以新蛋商品編號一定要先填寫完成
                    mainOrder.SellerProductID = sellerProductIDCell.Body.Where(x => x.ProductID == mainOrder.ProductID).Select(x => x.SellerProductID).First();
                }

                #endregion 商家商品編號

                #region 訂單狀態名稱

                if (mainOrder.Status.HasValue)
                {
                    switch (mainOrder.Status.Value)
                    {
                        case (int)Cart.status.完成:
                        case (int)Cart.status.正常:
                            {
                                if (mainOrder.DelvStatus.HasValue)
                                {
                                    mainOrder.StatusName = ((Cart.cartstatus)mainOrder.DelvStatus.Value).ToString();
                                }
                                break;
                            }
                        case (int)Cart.status.取消:
                        case (int)Cart.status.退貨:
                            {
                                mainOrder.StatusName = ((Cart.status)mainOrder.Status.Value).ToString();
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

                #endregion 訂單狀態名稱

                #region 付款方式名稱

                if (mainOrder.PayType.HasValue)
                {
                    mainOrder.PayTypeName = ((PayType.nPayType)mainOrder.PayType.Value).ToString();
                }

                #endregion 付款方式名稱

                #region 商家

                mainOrder.Seller = seller;

                #endregion 商家

                #region 出貨方名稱

                if (mainOrder.ShipType.HasValue)
                {
                    mainOrder.ShipTypeName = GetShipTypeName(mainOrder.ShipType.Value);
                }

                #endregion 出貨方名稱
            }

            #endregion 組合資料

            return result;
        }

        /// <summary>
        /// 取得某商家的商品編號清單
        /// </summary>
        /// <param name="sellerId">商家編號</param>
        /// <param name="orderKeyWordSearchType">關鍵字查詢目標</param>
        /// <param name="keyWord">關鍵字</param>
        /// <returns>商品編號清單</returns>
        private ActionResponse<List<int>> GetProductIdCell(int sellerId, int orderKeyWordSearchType, string keyWord)
        {
            ActionResponse<List<int>> result = new ActionResponse<List<int>>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new List<int>());

            if (sellerId < 0)
            {
                logger.Info(string.Format("取得某商家的商品編號清單失敗; ErrorMessage = {0}; SellerID = {1}.", "商家編號小於 0", sellerId));
                result.Finish(false, (int)ResponseCode.Error, null, null);
                return result;
            }

            // 連接前台 DB
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            // 取得特殊費用 ID
            List<int> specialFee = GetSpecialFee();

            try
            {
                // 判斷是否依新蛋商品編號或商家商品編號的關鍵字篩選
                if ((orderKeyWordSearchType == (int)OrderKeyWordSearchType.新蛋商品編號 || orderKeyWordSearchType == (int)OrderKeyWordSearchType.商家商品編號) && string.IsNullOrEmpty(keyWord) == false)
                {
                    switch (orderKeyWordSearchType)
                    {
                        default:
                        case (int)OrderKeyWordSearchType.新蛋商品編號:
                            {
                                int productID = 0;
                                if (int.TryParse(keyWord, out productID) && productID > 0)
                                {
                                    result.Body = dbFront.Product.Where(x => !specialFee.Contains(x.ID) && x.SellerID == sellerId && x.ID == productID).Select(r => r.ID).ToList();
                                }
                                
                                break;
                            }
                        case (int)OrderKeyWordSearchType.商家商品編號:
                            {
                                result.Body = dbFront.Product.Where(x => !specialFee.Contains(x.ID) && x.SellerID == sellerId && x.SellerProductID == keyWord).Select(r => r.ID).ToList();
                                break;
                            }
                    }
                }
                else
                {
                    result.Body = dbFront.Product.Where(x => !specialFee.Contains(x.ID) && x.SellerID == sellerId).Select(r => r.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得某商家的商品編號清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 取得訂單子單清單
        /// </summary>
        /// <param name="productIdCell">商品編號清單</param>
        /// <param name="orderKeyWordSearchType">關鍵字查詢目標</param>
        /// <param name="keyWord">關鍵字</param>
        /// <returns>訂單子單清單</returns>
        private ActionResponse<List<MainOrder_Process>> GetProcessCell(List<int> productIdCell, int orderKeyWordSearchType, string keyWord)
        {
            ActionResponse<List<MainOrder_Process>> result = new ActionResponse<List<MainOrder_Process>>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new List<MainOrder_Process>());

            if (productIdCell == null)
            {
                logger.Info(string.Format("取得訂單子單清單失敗; ErrorMessage = {0}.", "未傳入商品編號清單"));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            if (productIdCell.Count == 0)
            {
                return result;
            }

            // 連接後台 DB
            DB.TWBackendDBContext dbBack = new DB.TWBackendDBContext();

            try
            {
                for (int i = 0; i < Math.Ceiling((double)productIdCell.Count / 2000); i++)
                {
                    List<int> subProductIdCell = productIdCell.Skip(i * 2000).Take(2000).ToList();

                    IQueryable<MainOrder_Process> mainOrder_Process = dbBack.Process.Where(x => subProductIdCell.Contains(x.ProductID.Value))
                        .Select(x => new MainOrder_Process()
                        {
                            CartID = x.CartID,
                            StoreID = (x.StoreID ?? 0),
                            ProductID = x.ProductID.Value,
                            Title = x.Title,
                            Qty = x.Qty
                        })
                        .AsQueryable();

                    if ((orderKeyWordSearchType == (int)OrderKeyWordSearchType.訂單編號 || orderKeyWordSearchType == (int)OrderKeyWordSearchType.商品名稱) && string.IsNullOrEmpty(keyWord) == false)
                    {
                        switch (orderKeyWordSearchType)
                        {
                            default:
                            case (int)OrderKeyWordSearchType.訂單編號:
                                {
                                    mainOrder_Process = mainOrder_Process.Where(x => x.CartID == keyWord).AsQueryable();
                                    break;
                                }
                            case (int)OrderKeyWordSearchType.商品名稱:
                                {
                                    mainOrder_Process = mainOrder_Process.Where(x => x.Title.IndexOf(keyWord) != -1).AsQueryable();
                                    break;
                                }
                        }
                    }

                    result.Body.AddRange(mainOrder_Process.ToList());
                }

                if (result.Body.Count > 0)
                {
                    // 依訂單編號 GroupBy，取第 1 筆資料，並合計 Qty
                    result.Body = result.Body
                        .GroupBy(x => x.CartID)
                        .Select(x => new MainOrder_Process()
                        {
                            CartID = x.Select(y => y.CartID).First(),
                            StoreID = x.Select(y => y.StoreID).First(),
                            ItemUrl = GetItemUrl(x.Select(y => y.StoreID).First()),
                            ProductID = x.Select(y => y.ProductID).First(),
                            Qty = x.Sum(y => y.Qty),
                            Title = x.Select(y => y.Title).First()
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得訂單子單清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 取得訂單主單清單
        /// </summary>
        /// <param name="processCell">訂單主單編號清單</param>
        /// <returns>訂單主單清單</returns>
        private ActionResponse<List<MainOrder>> GetCartCell(List<string> cartIdCell)
        {
            ActionResponse<List<MainOrder>> result = new ActionResponse<List<MainOrder>>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new List<MainOrder>());

            if (cartIdCell == null)
            {
                logger.Info(string.Format("取得訂單主單清單失敗; ErrorMessage = {0}.", "未傳入訂單主單編號清單"));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            if (cartIdCell.Count == 0)
            {
                return result;
            }

            // 連接後台 DB
            DB.TWBackendDBContext dbBack = new DB.TWBackendDBContext();

            try
            {
                //取得所有在 Vendor Portal 訂單清單中，會使用到的訂單狀態
                List<int> vendorPortalCartStatusCell = this.GetVendorPortalCartStatusCell();

                // 取得所有在 Vendor Portal 訂單清單中，會使用到的出貨狀態
                List<int> vendorPortalDelvStatusCell = this.GetVendorPortalDelvStatusCell();

                result.Body = dbBack.Cart
                    .Where
                    (x =>
                        cartIdCell.Contains(x.ID)
                        && vendorPortalCartStatusCell.Contains(x.Status.Value)
                        && vendorPortalDelvStatusCell.Contains(x.DelvStatus.Value)
                    )
                    .Select(x => new MainOrder()
                    {
                        CartID = x.ID,
                        CreateDate = x.CreateDate,
                        DelvStatus = x.DelvStatus,
                        PayType = x.PayType,
                        SalesOrderGroupID = x.SalesorderGroupID,
                        ShipType = x.ShipType,
                        Status = x.Status,
                        UserName = x.Username,
                        UpdateDate = x.UpdateDate
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得訂單主單清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        #region 在取得某商家的商品編號清單時篩選(此時此欄位還尚未填入資料，與其此時再去撈 DB，還不如在取得商品編號清單時就先篩選掉)

        ///// <summary>
        ///// 依新蛋商品編號的關鍵字篩選
        ///// </summary>
        ///// <param name="mainOrderCell">訂單清單</param>
        ///// <param name="keyWord">關鍵字</param>
        ///// <returns>訂單清單</returns>
        //private List<MainOrder> FilterByProductID(List<MainOrder> mainOrderCell, string keyWord)
        //{
        //    List<MainOrder> result = new List<MainOrder>();

        //    if (mainOrderCell.Count == 0)
        //    {
        //        return result;
        //    }

        //    foreach (MainOrder mainOrder in mainOrderCell)
        //    {
        //        if (mainOrder.ProductID.ToString() == keyWord)
        //        {
        //            result.Add(mainOrder);
        //        }
        //    }

        //    return result;
        //}

        #endregion 在取得某商家的商品編號清單時篩選(此時此欄位還尚未填入資料，與其此時再去撈 DB，還不如在取得商品編號清單時就先篩選掉)

        /// <summary>
        /// 取得採購單編號清單
        /// </summary>
        /// <param name="cartIdCell">訂單主單編號清單</param>
        /// <returns>採購單編號清單</returns>
        private ActionResponse<List<MainOrder_PurchaseOrder>> GetPOCodeCell(List<string> cartIdCell)
        {
            ActionResponse<List<MainOrder_PurchaseOrder>> result = new ActionResponse<List<MainOrder_PurchaseOrder>>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new List<MainOrder_PurchaseOrder>());

            if (cartIdCell == null)
            {
                logger.Info(string.Format("取得採購單編號清單失敗; ErrorMessage = {0}.", "未傳入訂單主單編號清單"));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            if (cartIdCell.Count == 0)
            {
                return result;
            }

            // 連接前台 DB
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.PurchaseOrder.Where(x => cartIdCell.Contains(x.SalesorderCode)).Select(x => new MainOrder_PurchaseOrder()
                { 
                    CartID = x.SalesorderCode,
                    Code = x.Code
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得採購單編號清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 取得商家商品編號清單
        /// </summary>
        /// <param name="cartIdCell">新蛋商品編號清單</param>
        /// <returns>商家商品編號清單</returns>
        private ActionResponse<List<MainOrder_Product>> GetSellerProductIDCell(List<int> productIdCell)
        {
            ActionResponse<List<MainOrder_Product>> result = new ActionResponse<List<MainOrder_Product>>();
            result.Finish(true, (int)ResponseCode.Success, string.Empty, new List<MainOrder_Product>());

            if (productIdCell == null)
            {
                logger.Info(string.Format("取得商家商品編號清單失敗; ErrorMessage = {0}.", "新蛋商品編號清單"));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            if (productIdCell.Count == 0)
            {
                return result;
            }

            // 連接前台 DB
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

            try
            {
                result.Body = dbFront.Product.Where(x => productIdCell.Contains(x.ID)).Select(x => new MainOrder_Product()
                {
                    ProductID = x.ID,
                    SellerProductID = x.SellerProductID
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得商家商品編號清單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                result.Finish(false, (int)Models.ResponseCode.Error, null, null);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 取得所有在 Vendor Portal 訂單清單中，會使用到的訂單狀態
        /// </summary>
        /// <returns>訂單狀態清單</returns>
        private List<int> GetVendorPortalCartStatusCell()
        {
            return new List<int>()
            {
                (int)Cart.status.正常,
                (int)Cart.status.取消,
                (int)Cart.status.退貨,
                (int)Cart.status.完成,
            };
        }

        /// <summary>
        /// 舊的 Cart 列舉，放在這純粹比對
        /// </summary>
        public enum EnumCartStatus
        {
            正常 = 0,
            取消 = 1,
            被動取消 = 2,
            退貨 = 5,
            完成 = 7,
            初始狀態 = 99
        }

        /// <summary>
        /// 取得所有在 Vendor Portal 訂單清單中，會使用到的出貨狀態
        /// </summary>
        /// <returns>出貨狀態清單</returns>
        private List<int> GetVendorPortalDelvStatusCell()
        {
            return new List<int>()
            {
                (int)Cart.cartstatus.已成立,
                (int)Cart.cartstatus.待出貨,
                (int)Cart.cartstatus.已出貨,
                (int)Cart.cartstatus.配達
            };
        }

        /// <summary>
        /// 取得出貨方名稱
        /// </summary>
        /// <param name="shipType">出貨方</param>
        /// <returns>出貨方名稱</returns>
        private string GetShipTypeName(int shipType)
        {
            switch (shipType)
            {
                case (int)Item.tradestatus.切貨:
                    {
                        return "Newegg";
                    }
                case (int)Item.tradestatus.間配:
                    {
                        return "Newegg"; 
                    }
                case (int)Item.tradestatus.直配:
                    {
                        return "Seller";
                    }
                case (int)Item.tradestatus.三角:
                    {
                        return "Seller";
                    }
                case (int)Item.tradestatus.國外直購: 
                    {
                        return "供應商"; 
                    }
                case (int)Item.tradestatus.自貿區:
                    {
                        return "Newegg"; 
                    }
                case (int)Item.tradestatus.海外切貨:
                    {
                        return "SBSPEX"; 
                    }
                case (int)Item.tradestatus.B2C直配:
                    {
                        return "供應商"; 
                    }
                case (int)Item.tradestatus.MKPL寄倉:
                    {
                        return "Newegg";
                    }
                case (int)Item.tradestatus.B2c寄倉:
                    {
                        return "Newegg";
                    }
                default:
                    {
                        logger.Info(string.Format("輸入的 ShipType = {0} 不在規定內", shipType));
                        return null;
                    }
            }
        }

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
            }
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        #endregion 主單

        #region 子單
        
        /// <summary>
        /// 訂單子單資料蒐集
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回蒐集結果</returns>
        public ActionResponse<OrderDetail> OrderDetail(string cartID)
        {
            ActionResponse<OrderDetail> result = new ActionResponse<OrderDetail>();
            TWSqlDBContext db_Front = new TWSqlDBContext();
            TWBackendDBContext db_Backend = new TWBackendDBContext();
            TWSellerPortalDBContext db_SellerPortal = new TWSellerPortalDBContext();
            
            result.IsSuccess = false;
            result.Code = (int)ResponseCode.Error;
            #region 子單資料蒐集
            try
            {
                if (string.IsNullOrEmpty(cartID))
                {
                    logger.Error("訂單編號缺漏");
                    result.Msg = "訂單編號缺漏";
                    return result;
                }

                Cart cart = db_Backend.Cart.Where(x => x.ID == cartID).FirstOrDefault();
                if (cart == null)
                {
                    logger.Error("訂單編號 : " + cartID + " 查無此訂單編號資訊");
                    result.Msg = "訂單編號 : " + cartID + " 查無此訂單編號資訊";
                    return result;
                }

                List<Process> processList = db_Backend.Process.Where(x => x.CartID == cartID).ToList();
                if (processList == null)
                {
                    logger.Error("訂單編號 : " + cartID + " 子單缺失");
                    result.Msg = "訂單編號 : " + cartID + " 子單缺失";
                    return result;
                }

                Process firstProcess = processList.FirstOrDefault();
                string poCode = db_Backend.PurchaseOrderTWBACK.Where(x => x.SalesorderCode == cartID).Select(x => x.Code).FirstOrDefault();
                //if (string.IsNullOrEmpty(poCode))
                //{
                //    result.Msg = "訂單編號 : " + cartID + " PurchaseOrderCode缺失";
                //    return result;
                //}

                int productID = firstProcess.ProductID ?? 0;
                Product product = db_Front.Product.Where(x => x.ID == productID).FirstOrDefault();
                if (product == null)
                {
                    logger.Error("產品編號 : " + productID + " 查無此產品編號資訊");
                    result.Msg = "產品編號 : " + productID + " 查無此產品編號資訊";
                    return result;
                }

                Seller seller = db_Front.Seller.Where(x => x.ID == product.SellerID).FirstOrDefault();
                var SellerInfo = db_SellerPortal.Seller_BasicInfo.Where(x => x.SellerID == product.SellerID).FirstOrDefault();
                if (seller == null)
                {
                    logger.Error("SellerID : " + product.SellerID + " 查無此Seller資訊");
                    result.Msg = "SellerID : " + product.SellerID + " 查無此Seller資訊";
                    return result;
                }

                OrderDetail salesOrderDetail = new OrderDetail();
                // 會員身分別(Seller(S) or Vender(V))
                salesOrderDetail.AccountTypeCode = seller.AccountType.Trim().ToUpper();
                salesOrderDetail.ItemUrl = GetItemUrl(firstProcess.StoreID);
                // 出貨地址
                string sellerShippingAddress = string.Empty;
                string comZip = string.IsNullOrEmpty(SellerInfo.ComZipcode) ? string.Empty : SellerInfo.ComZipcode.Trim() + " ";

                sellerShippingAddress = comZip + SellerInfo.ComCity + SellerInfo.ComSellerAdd;

                // 出貨地址
                salesOrderDetail.SellerShippingAddress = sellerShippingAddress;
                // 訂單編號
                salesOrderDetail.CartID = cartID;
                // PurchaseOrder Code
                salesOrderDetail.POCode = poCode;
                // ProcessID List
                salesOrderDetail.ProcessIDList.AddRange(processList.Select(x => x.ID).ToList());
                // 產品編號
                salesOrderDetail.ProductID = product.ID;
                // 訂單產生日期
                salesOrderDetail.OrderCreateDate = ((DateTime)cart.CreateDate).ToString("yyyy/MM/dd HH:mm:ss");
                // 訂單狀態
                salesOrderDetail.Status = (int)cart.Status;
                // 訂單配送狀態
                salesOrderDetail.DelvStatus = cart.DelvStatus;
                // 訂購人名稱
                salesOrderDetail.CustomerName = cart.Username;
                // 訂購人手機
                salesOrderDetail.CustomerMobile = cart.TelDay;
                //  供貨通路/遞送服務/遞送方(代碼)
                salesOrderDetail.DelvType = cart.ShipType ?? -1;
                //供貨通路/遞送服務類別/遞送方
                salesOrderDetail.FulfillChannel = this.GetShipTypeName(cart.ShipType ?? -1);
                // 收件人姓名
                salesOrderDetail.ReceiverName = cart.Receiver;
                // 收件人市話
                salesOrderDetail.ReceiverPhone = cart.Phone;
                // 收件人地址
                salesOrderDetail.ReceiverAddress = cart.Zipcode + " " + cart.Location + cart.ADDR;
                // 收件人手機
                salesOrderDetail.ReceiverCellphone = cart.RecvMobile;
                // 備註
                salesOrderDetail.Note = cart.Note;
                // 商家商品編號
                salesOrderDetail.SellerProductID = product.SellerProductID;
                // 新蛋商品編號
                salesOrderDetail.NeweggPartNum = productID.ToString();
                // 廠商編號
                salesOrderDetail.ManufactureID = product.ManufactureID;
                // 廠商產品編號
                salesOrderDetail.MenufacturePartNum = string.IsNullOrEmpty(product.MenufacturePartNum) ? "N/A" : product.MenufacturePartNum;
                // UPC
                salesOrderDetail.UPC = product.UPC;
                // 商品說明
                salesOrderDetail.ItemTitle = firstProcess.Title;
                int getQty = processList.Sum(s => (int)s.Qty);
                // 訂購數量
                salesOrderDetail.Qty = getQty;
                // if cart.status = 1(訂單取消) then 已遞送數量 = 0
                // else
                // {
                //     if cart.DelvStatus = 6 || 0 (訂單狀態是"已成立"or"待出貨") then 
                //         已遞送數量 = 0 
                //     else 
                //         已遞送數量 = 訂購數量
                // }
                if (cart.Status == (int)Cart.status.取消)
                {
                    // 已遞送數量
                    salesOrderDetail.ShippedCount = 0;
                }
                else if (cart.DelvStatus == (int)Cart.cartstatus.已成立 || cart.DelvStatus == (int)Cart.cartstatus.待出貨)
                {
                    salesOrderDetail.ShippedCount = 0;
                }
                else
                {
                    salesOrderDetail.ShippedCount = processList.Sum(s => (int)s.Qty);
                }
                // 單價(S)
                salesOrderDetail.Price = firstProcess.Price ?? 0m;
                decimal unitCost = 0m;
                unitCost = db_Backend.PurchaseOrderitemTWBACK.Where(x => x.PurchaseorderCode == poCode).Select(y => y.SourcePrice).FirstOrDefault();
                // 單位成本(V)
                salesOrderDetail.UnitCost = unitCost;
                // 總成本(V)
                salesOrderDetail.TotalCost = unitCost * (decimal)getQty;
                // 希望到貨時間
                salesOrderDetail.DelvDate = firstProcess.OrderNote.Replace("</arrive>", "");
                int delvID = db_SellerPortal.Seller_DelvTrack.Where(x => x.ProcessID == firstProcess.ID).Select(x => x.DeliverID).FirstOrDefault();
                // 貨運公司
                salesOrderDetail.DelvCompanyName = db_Backend.Deliver.Where(x => x.code == delvID).Select(x => x.Name).FirstOrDefault();
                // 貨運編號
                salesOrderDetail.TrackingNumber = db_SellerPortal.Seller_DelvTrack.Where(x => x.ProcessID == firstProcess.ID).Select(x => x.TrackingNum).FirstOrDefault();
                #region 小計項目
                // 子項總計
                decimal subtotalPrice = 0;
                decimal shippingFee = processList.Sum(x => (decimal)x.ShippingExpense);
                // 運費
                salesOrderDetail.ShippingFee = shippingFee;
                decimal serviceFee = processList.Sum(x => (decimal)x.ServiceExpense);
                // 服務費
                salesOrderDetail.ServiceFee = serviceFee;
                // 訂購總額
                decimal totalPrice = 0;

                switch (seller.AccountType.Trim().ToUpper())
                {
                    default:
                    case "S":
                        {
                            subtotalPrice = processList.Sum(x => (decimal)x.Price);
                            totalPrice = subtotalPrice + shippingFee + serviceFee;
                            break;
                        }
                    case "V":
                        {
                            subtotalPrice = unitCost * getQty;
                            totalPrice = subtotalPrice + shippingFee + serviceFee;
                            break;
                        }
                }
                // 子項目總計
                salesOrderDetail.SubTotalPrice = subtotalPrice;
                // 訂購總額
                salesOrderDetail.TotalPrice = totalPrice;
                #endregion 小計項目
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Body = salesOrderDetail;
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號 : " + cartID + " 系統意外結束 : [ErrorMessage] " + ex.ToString());
                result.Msg = "訂單編號 : " + cartID + " 系統執行失敗，請聯繫客服";
            }
            #endregion

            return result;
        }

        #endregion

        /// <summary>
        /// 取得前台賣場頁網址
        /// </summary>
        /// <param name="itemID">賣場頁ID</param>
        /// <returns>返回前台賣場頁網址</returns>
        public string GetItemUrl(Nullable<int> itemID)
        {
            return webSite + "/item?itemid=" + (itemID ?? 0).ToString();
        }

        /// <summary>
        /// 取得訂單編號的完整資訊
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public ActionResponse<TinyCart> GetCartInfo(string cartID)
        {
            TWBackendDBContext db_BackEnd = new DB.TWBackendDBContext();
            ActionResponse<TinyCart> cartInfo = new ActionResponse<TinyCart>();
            try
            {
                Cart cart = db_BackEnd.Cart.Where(x => x.ID == cartID).FirstOrDefault();
                TinyCart tinyCart = new TinyCart();
                tinyCart.ID = cart.ID;
                tinyCart.ShipType = (int)cart.ShipType;
                tinyCart.DelvStatus = (int)cart.DelvStatus;
                if (cart == null)
                {
                    cartInfo.Finish(false, (int)ResponseCode.Error, "訂單編號 : " + cartID + " 不存在", null);
                }
                else
                {
                    cartInfo.Finish(true, (int)ResponseCode.Success, "", tinyCart);
                }
            }
            catch (Exception ex)
            {
                cartInfo.Finish(false, (int)ResponseCode.Error, GetExceptionMessage(ex), null);
            }

            return cartInfo;
        }

        /// <summary>
        /// 更新訂單遞送狀態
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <param name="delvStatus">訂單出貨狀態</param>
        /// <param name="updateUser">updateUser</param>
        /// <returns>返回結果</returns>
        public ActionResponse<TinyCart> UpdateCartDelvStatus(string cartID, int delvStatus, string updateUser)
        {
            TWBackendDBContext db_BackEnd = new DB.TWBackendDBContext();
            ActionResponse<TinyCart> cartInfo = new ActionResponse<TinyCart>();
            TinyCart tinyCart = new TinyCart();
            try
            {
                Cart cart = db_BackEnd.Cart.Where(x => x.ID == cartID).FirstOrDefault();
                if (cart == null)
                {
                    cartInfo.Finish(false, (int)ResponseCode.Error, "查無訂單編號 : " + cartID + " 資訊", null);
                }

                cart.DelvStatus = delvStatus;
                cart.DelvStatusDate = DateTime.UtcNow.AddHours(8);
                cart.UpdateUser = updateUser;
                cart.UpdateDate = DateTime.UtcNow.AddHours(8);
                if (string.IsNullOrEmpty(cart.UpdateNote))
                {
                    cart.UpdateNote = DateTime.Now + " " + Enum.GetName(typeof(Cart.cartstatus), delvStatus).ToString() + ".VendorPortal(" + updateUser + ")</br>";
                }
                else
                {
                    cart.UpdateNote += DateTime.Now + " " + Enum.GetName(typeof(Cart.cartstatus), delvStatus).ToString() + ".VendorPortal(" + updateUser + ")</br>";
                }

                db_BackEnd.SaveChanges();
                tinyCart.ID = cartID;
                tinyCart.DelvStatus = delvStatus;
                tinyCart.ShipType = (int)cart.ShipType;
                cartInfo.Finish(true, (int)ResponseCode.Success, "", tinyCart);
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號[" + cartID + "]更新狀態[" + Enum.GetName(typeof(Cart.cartstatus), delvStatus).ToString() + "]失敗 UpdateUser : " + updateUser);
                cartInfo.Finish(false, (int)ResponseCode.Error, GetExceptionMessage(ex), null);
            }

            return cartInfo;
        }
    }
}
