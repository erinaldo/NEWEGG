using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemBatchServices.Models;
using TWNewEgg.NeweggUSARequestServices.Services;
using TWNewEgg.NeweggUSARequestServices.Models;
using TWNewEgg.NeweggUSARequestServices.Models.Pricing;
using TWNewEgg.SellerRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;


namespace TWNewEgg.ItemBatchServices.Service
{
    public class USAUpdateItemPriceService : IUSAUpdateItemPriceService, IDisposable
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;
        private ISellerRepoAdapter _sellerRepoAdapter;
        private IItemDisplayPriceService _itemDisplayPriceService;

        public USAUpdateItemPriceService
        (
            IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter productRepoAdapter, ISellerRepoAdapter sellerRepoAdapter, IItemDisplayPriceService itemDisplayPriceService
        )
        {
            this._itemRepoAdapter = itemRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
            this._sellerRepoAdapter = sellerRepoAdapter;
            this._itemDisplayPriceService = itemDisplayPriceService;
        }

        

        public ActionResponse<List<DomainResult>> DoWork(UpdateModel updateModel)
        {
            string UpdateUser = updateModel.UpdateUser;
            if (UpdateUser == null || UpdateUser == "")
            {
                UpdateUser = "System_UpdateItemPrice";
            }
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            result.Body = new List<DomainResult>();
            result.IsSuccess = false;
            List<string> sellerproductIDs = new List<string>();
            sellerproductIDs = updateModel.SellerProductIDsList;

            if (sellerproductIDs != null || sellerproductIDs.Count > 0)
            {
                sellerproductIDs = sellerproductIDs.OrderBy(x => x).ToList();
                int timenumber = 0;
                int itemnumber = sellerproductIDs.Count;
                int skipnumber = 20;
                while ((timenumber * skipnumber) < itemnumber)
                {
                    try
                    {
                        List<string> sellerproductIDstemp = sellerproductIDs.Skip(timenumber * skipnumber).Take(skipnumber).ToList();

                        List<Product> ProductList = new List<Product>();
                        List<string> productFromWS = new List<string>();
                        ProductList = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && sellerproductIDstemp.Contains(x.SellerProductID) && x.SellerID == 2).Distinct().ToList();

                        foreach (var temp in ProductList)
                        {
                            if (temp != null)
                            {
                                switch (temp.SourceTable.ToLower())
                                {
                                    case "productfromws":
                                        productFromWS.Add(temp.SellerProductID);
                                        break;
                                }
                            }
                        }
                        productFromWS = productFromWS.Distinct().ToList();
                        //Get Price
                        if (productFromWS != null && productFromWS.Count > 0)
                        {
                            ActionResponse<List<DomainResult>> GetPriceFromNeweggUSAResult = GetPriceFromNeweggUSA(productFromWS, UpdateUser);
                            result.Body.AddRange(GetPriceFromNeweggUSAResult.Body);
                            result.IsSuccess = true;
                            result.Msg = "執行成功";
                        }
                        else
                        {
                            result.IsSuccess = true;
                            result.Msg = "無需要執行資料";
                        }                      
                    }
                    catch(Exception e){
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                    timenumber++;
                    System.Threading.Thread.Sleep(100);
                }
            }
            //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_執行結束");
            result.Msg = result.Msg + "\r\n USAUpdateItemPriceService DoWork_執行結束";
            return result;
        }

        private ActionResponse<List<DomainResult>> GetPriceFromNeweggUSA(List<string> itemnumberList, string UpdateUser)
        {
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            result.Body = new List<DomainResult>();
            result.IsSuccess = false;
            try
            {
                NeweggRequest nr = new NeweggRequest();

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Reset();
                watch.Start();

                Dictionary<string, ItemInfo> priceInfos = null;
                int retry = 0;
                while ((retry++) < 3)
                {
                    try
                    {
                        //ProcessStatus(this, "下載商品資訊...");
                        priceInfos = nr.GetPrice(itemnumberList);
                        if (priceInfos != null && priceInfos.Count > 0)
                        {
                            result.IsSuccess = true;
                            result.Msg = "GetPriceFromNeweggUSA 下載商品資訊:...完成";
                            break;
                        }
                        else
                        {
                            //ProcessStatus(this, ");
                            result.IsSuccess = false;
                            result.Msg = "GetPriceFromNeweggUSA 下載商品資訊:...重試 " + retry.ToString();
                        }

                    }
                    catch (Exception e)
                    {
                        watch.Stop();
                        //ProcessStatus(this, e.Message);
                    }
                }

                watch.Stop();
                //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_下載資訊耗時(ms):" + watch.ElapsedMilliseconds);

                watch.Reset();
                watch.Start();

                if (priceInfos != null)
                {
                    foreach (var priceInfo in priceInfos)
                    {
                        string itemnumber = priceInfo.Key;
                        decimal price = 0;
                        decimal shippingCharge = 0;
                        ItemInfo info = priceInfo.Value;
                        if (info != null && info.Available == true)
                        {
                            price = info.FinalPrice;
                            shippingCharge = info.ShippingCharge;
                            //依據美蛋的資料規則，運費若為0.01 則變更為0
                            if (shippingCharge <= 0.01m)
                            {
                                shippingCharge = 0m;
                            }
                        }
                        //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_ItemNumber:" + itemnumber + " Price:" + price.ToString());
                        result.Msg = "System.Threading.Thread.CurrentThread.ManagedThreadId itemnumber" + itemnumber;
                        List<Product> productList = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && x.SourceTable== "productfromws" && x.SellerID == 2 && x.SellerProductID == itemnumber).ToList();
                        List<int> itemchangeList = new List<int>();
                        if (price <= 0||price+shippingCharge<=0)
                        {
                            //售價異常，關閉賣場
                            List<int> productchangeList = productList.Select(x => x.ID).ToList();
                            CloseItem(productchangeList);
                            DomainResult DomainResulttemp = new DomainResult();
                            DomainResulttemp.ProductID = productList.FirstOrDefault().ID;
                            DomainResulttemp.SellerProductID = productList.FirstOrDefault().SellerProductID;
                            DomainResulttemp.IsSuccess = false;
                            DomainResulttemp.Log = "售價異常，關閉賣場 Price=" + price.ToString();
                            result.Body.Add(DomainResulttemp);
                        }
                        else
                        {
                            int countStock = 0;
                            //更新售價
                            foreach (var currentProduct in productList)
                            {
                                countStock++;
                                try
                                {
                                    DomainResult DomainResulttemp = new DomainResult();
                                    DomainResulttemp.ProductID = currentProduct.ID;
                                    DomainResulttemp.SellerProductID = currentProduct.SellerProductID;
                                    DomainResulttemp.IsSuccess = false;
                                    currentProduct.Cost = price + shippingCharge;
                                    currentProduct.SupplyShippingCharge = shippingCharge;
                                    currentProduct.UpdateUser = "System_UpdateItemPrice";
                                    currentProduct.UpdateDate = DateTime.UtcNow.AddHours(8);
                                    //所有販賣這個商品的賣場，跳過切貨賣場
                                    List<Item> items = _itemRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && x.ProductID == currentProduct.ID && x.SellerID == 2).ToList();

                                    //所有使用這個商品做為屬性
                                    //List<ItemList> itemlists = db.ItemList.Where(x => x.ItemlistProductID == currentProduct.ID).ToList();

                                    //搜尋匯率
                                    Seller seller = _sellerRepoAdapter.GetAll().Where(x => x.ID == 2).FirstOrDefault();
                                    if (seller != null)
                                    {
                                        string year = DateTime.Now.Year.ToString();
                                        string month = DateTime.Now.Month.ToString();
                                        Currency currency = this._sellerRepoAdapter.GetCurrency(year, month, (seller.CountryID ?? 2));
                                        if (currency != null)
                                        {
                                            //使用當期匯率重新計算售價
                                            //ProcessStatus(this, year + "/" + month + " 匯率: " + currency.BufferRate.ToString());
                                            result.Msg = "使用當期匯率重新計算售價" + currency.BufferRate.ToString();
                                            //BufferRate Default Value = AverageexchangeRate * 1.012
                                            if (currency.BufferRate == null || currency.BufferRate <= 0)
                                            {
                                                currency.BufferRate = currency.AverageexchangeRate * 1.012m;
                                                currency = this._sellerRepoAdapter.UpdateCurrency(currency);
                                            }
                                            decimal nt = (price + shippingCharge) * currency.BufferRate;
                                            nt = Math.Round(nt, 0, MidpointRounding.AwayFromZero);
                                            List<string> priceChangeItemID = new List<string>();
                                            foreach (var itemtemp in items)
                                            {
                                                DomainResult DomainResultitemtemp = new DomainResult();
                                                DomainResultitemtemp.ProductID = currentProduct.ID;
                                                DomainResultitemtemp.SellerProductID = currentProduct.SellerProductID;
                                                DomainResultitemtemp.IsSuccess = false;
                                                if (itemtemp.PriceCash != nt)
                                                {
                                                    //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_售價異動 ItemID:" + items[k].ID.ToString() + "ProductID:" + currentProduct.ID.ToString() + " ItemNumber:" + currentProduct.SellerProductID + " 原價NT:" + items[k].PriceCash.ToString() + " 現價NT:" + nt.ToString());
                                                    result.Msg = "System.Threading.Thread.CurrentThread.ManagedThreadId_售價異動" + itemtemp.ID.ToString();
                                                    //Add to changeList
                                                    itemchangeList.Add(itemtemp.ID);

                                                    //更新主商品售價
                                                    itemtemp.PriceCash = nt;
                                                    itemtemp.UpdateUser = UpdateUser;
                                                    itemtemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                                                }
                                                //因商品售價更新成功，如賣場狀態為關閉則自動開啟
                                                if (itemtemp.Status == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.系統下架)
                                                {
                                                    if (itemtemp.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.已上架)
                                                    {
                                                        //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_ItemID:" + items[k].ID.ToString() + "_OldStatus:" + items[k].Status.ToString() + "_轉換為上架");
                                                    }
                                                    itemtemp.Status = (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.已上架;
                                                }
                                                DomainResultitemtemp.ItemID = itemtemp.ID;
                                                DomainResultitemtemp.Log = "商品售價更新成功 NT:" + nt.ToString(); ;
                                                DomainResultitemtemp.IsSuccess = true;
                                                result.Body.Add(DomainResultitemtemp);
                                            }
                                            DomainResulttemp.ItemID = 0;
                                            DomainResulttemp.Log = "商品售價更新成功 USD:" + (currentProduct.Cost ?? 0).ToString();
                                            DomainResulttemp.IsSuccess = true;
                                        }
                                        else
                                        {
                                            //
                                            //ProcessStatus(this, "Currency Not Exist :" + year + "/" + month);
                                            DomainResulttemp.Log = "找不到當期匯率";
                                            DomainResulttemp.IsSuccess = false;
                                        }
                                    }
                                    else
                                    {
                                        //找不到 Seller
                                        //ProcessStatus(this, "Seller Not Exist => ProductID:" + currentProduct.ID.ToString() + " SellerID:" + currentProduct.SellerID.ToString());
                                        DomainResulttemp.Log = "找不到Seller";
                                        DomainResulttemp.IsSuccess = false;
                                    }
                                    try
                                    {
                                        _itemRepoAdapter.UpdateItemList(items);
                                        System.Threading.Thread.Sleep(100);
                                    }
                                    catch (Exception e)
                                    {
                                        var i = 5;
                                    }
                                    result.Body.Add(DomainResulttemp);
                                }
                                catch
                                {

                                }
                                if (countStock > 5)
                                {
                                    _productRepoAdapter.Update(currentProduct);
                                    System.Threading.Thread.Sleep(80);
                                    countStock = 0;
                                }
                            }
                            _productRepoAdapter.UpdateMany(productList);
                            System.Threading.Thread.Sleep(200);
                        }

                        if (itemchangeList.Count > 0)
                        {
                            //觸發賣場統一售價計算
                            try
                            {
                                result.Msg = this._itemDisplayPriceService.SetItemDisplayPriceByIDs(itemchangeList) + "_更新成功";
                                result.IsSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                result.Msg = ex.Message;
                                result.IsSuccess = false;
                            }
                            //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_賣場統一顯示售價計算:" + string.Join(",", changeList) + "_Msg:" + msg);
                        }
                        else
                        {
                            result.Msg = "無需要更新項目";
                            result.IsSuccess = true;
                        }
                    }
                }
                watch.Stop();
                result.IsSuccess = true;
                return result;
                //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_資料操作耗時(ms):" + watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                result.Msg = result.Msg + "\r\n" + e.Message;
                result.IsSuccess = false;
                return result;
            }
        }

        /// <summary>
        /// 關閉賣場
        /// </summary>
        public void CloseItem(List<int> productIDs)
        {
            //所有販賣這個商品的賣場，跳過切貨賣場
            List<Item> items = _itemRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && productIDs.Contains(x.ProductID) && x.SellerID == 2 && x.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.強制下架).ToList();
            //所有使用這個商品做為屬性
            //List<ItemList> itemlists = db.ItemList.Where(x => x.ItemlistProductID == productID).ToList();

            if (items.Count() <= 0)
            {
                //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_尚未建立相關賣場");
            }

            foreach (var temp in items)
            {
                if (temp.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.強制下架)
                {
                    if (temp.Status != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.系統下架)
                    {
                        //ProcessStatus(this, "關閉賣場 ItemID:" + items[i].ID.ToString() + " ProductID:" + items[i].ProductID + "_OldStatus:" + items[i].Status.ToString() + "_轉換為系統下架");
                    }
                    temp.Status = (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.status.系統下架;
                }

                temp.UpdateUser = "System_UpdateItemPrice_New";
                temp.StatusDate = DateTime.UtcNow.AddHours(8);
            }
            _itemRepoAdapter.UpdateItemList(items);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
        }
    }
}
