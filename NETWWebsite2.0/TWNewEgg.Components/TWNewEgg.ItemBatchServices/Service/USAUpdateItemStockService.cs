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
    public class USAUpdateItemStockService : IUSAUpdateItemStockService
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;
        private IItemStockRepoAdapter _itemStockRepoAdapter;

        public USAUpdateItemStockService
        (
            IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter productRepoAdapter, IItemStockRepoAdapter itemStockRepoAdapter
        )
        {
            this._itemRepoAdapter = itemRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
            this._itemStockRepoAdapter = itemStockRepoAdapter;
        }

        public ActionResponse<List<DomainResult>> DoWork(UpdateModel updateModel)
        {
            string UpdateUser = updateModel.UpdateUser;
            if (UpdateUser == null || UpdateUser == "")
            {
                UpdateUser = "System_UpdateItemStock";
            }
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            result.Body = new List<DomainResult>();
            result.IsSuccess = false;
            List<int> AlltempIDs = new List<int>();
            List<int> productIDs = new List<int>();

            AlltempIDs.AddRange(updateModel.Itemlist);
            AlltempIDs.AddRange(updateModel.ProductList);

            if (AlltempIDs != null || AlltempIDs.Count > 0)
            {
                AlltempIDs = AlltempIDs.OrderBy(x => x).ToList();
                int timenumber = 0;
                int itemnumber = AlltempIDs.Count;
                int skipnumber = 30;
                while ((timenumber * skipnumber) < itemnumber)
                {
                    try
                    {
                        List<int> AlltempIDstemp = AlltempIDs.Skip(timenumber * skipnumber).Take(skipnumber).ToList();
                        if (updateModel.UpdateListType == (int)UpdateModel.UpdateListTypestatus.Itemlist)
                        {
                            if (updateModel.Itemlist != null && updateModel.Itemlist.Count > 0)
                            {
                                productIDs = _itemRepoAdapter.GetAll().Where(x => AlltempIDstemp.Contains(x.ID)).Select(x => x.ProductID).Distinct().ToList();
                            }
                        }
                        else
                        {
                            if (updateModel.ProductList != null && updateModel.ProductList.Count > 0)
                            {
                                productIDs = AlltempIDstemp;
                            }
                        }

                        if (productIDs != null && productIDs.Count > 0)
                        {
                            List<Product> ProductList = new List<Product>();
                            List<string> productFromWS = new List<string>();
                            ProductList = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && productIDs.Contains(x.ID) && x.SellerID == 2).Take(110).Distinct().ToList();

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

                            //Get Price
                            if (productFromWS.Count > 0)
                            {
                                ActionResponse<List<DomainResult>> GetQuantityFromNeweggUSAResult = GetQuantityFromNeweggUSA(productFromWS, UpdateUser);
                                result.Body.AddRange(GetQuantityFromNeweggUSAResult.Body);
                                result.IsSuccess = true;
                                result.Msg = "執行成功";
                            }
                            else
                            {
                                result.IsSuccess = true;
                                result.Msg = "無需要執行資料";
                            }
                        }
                        else {
                            result.IsSuccess = true;
                            result.Msg = "無需要執行資料";
                        }
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                    timenumber++;
                    System.Threading.Thread.Sleep(100);
                }
            }
            //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_執行結束");
            result.Msg = result.Msg + "\r\n USAUpdateItemStockService DoWork_執行結束";
            return result;
        }

        private ActionResponse<List<DomainResult>> GetQuantityFromNeweggUSA(List<string> itemnumberList, string UpdateUser)
        {
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            result.Body = new List<DomainResult>();
            result.IsSuccess = false;
            try{
               
            NeweggRequest nr = new NeweggRequest();
            Q4S q4S = new Q4S();
            string[] itemnumberArray = itemnumberList.ToArray();

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Reset();
            watch.Start();

            //Dictionary<string, ItemInfo> priceInfos = null;
            Dictionary<string, int> quantityresult = null;
            int retry = 0;
            while ((retry++) < 3)
            {
                try
                {
                    //ProcessStatus(this, "下載商品資訊...");

                    quantityresult = q4S.GetWarehouseQuantity(itemnumberArray, "07");
                    if (quantityresult != null && quantityresult.Count > 0)
                    {
                        result.IsSuccess = true;
                        result.Msg = "GetWarehouseQuantity 下載商品資訊:...完成";
                        break;
                    }
                    else
                    {
                        //ProcessStatus(this, ");
                        result.IsSuccess = false;
                        result.Msg = "GetWarehouseQuantity 下載商品資訊:...重試 " + retry.ToString();                       
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

            if (quantityresult != null)
            {
                List<string> SellerProductIDList = new List<string>();
                SellerProductIDList = quantityresult.Select(x => x.Key).ToList();

                List<Product> productList = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && x.SourceTable == "productfromws" && x.SellerID == 2 && SellerProductIDList.Contains(x.SellerProductID)).ToList();
                List<int> productIDList = productList.Select(X => X.ID).ToList();
                List<ItemStock> itemStockList = _itemStockRepoAdapter.GetAll().Where(x => productIDList.Contains(x.ProductID)).ToList();
                int countStock = 0;
                //更新庫存
                foreach (var currentStock in itemStockList)
                {
                    countStock++;
                    DomainResult DomainResulttemp = new DomainResult();
                    DomainResulttemp.ProductID = currentStock.ProductID;
                    DomainResulttemp.SellerProductID = productList.Where(X => X.ID == currentStock.ProductID).FirstOrDefault().SellerProductID;
                    DomainResulttemp.IsSuccess = false;
                    try
                    {                       
                        int qty = currentStock.QtyReg;
                        qty = qty + quantityresult[DomainResulttemp.SellerProductID];
                        if (qty > 32767)
                        {
                            qty = 32766;
                        }
                        if (currentStock.Qty < 32767 && qty >= currentStock.QtyReg)
                        {
                            currentStock.Qty = qty;
                            DomainResulttemp.Log = "更新庫存 ProductID:" + DomainResulttemp.ProductID + "__QTY:" + qty.ToString();
                        }
                        else {
                            DomainResulttemp.Log = "不更新庫存 ProductID:" + DomainResulttemp.ProductID + "__QTY:" + qty.ToString();
                        }
                        DomainResulttemp.IsSuccess = true;
                        currentStock.UpdateDate = DateTime.UtcNow.AddHours(8);
                        currentStock.UpdateUser = UpdateUser;
                        result.Body.Add(DomainResulttemp);
                    }
                    catch(Exception e) {
                        DomainResulttemp.IsSuccess = false;
                        DomainResulttemp.Log = e.Message;
                        result.Body.Add(DomainResulttemp);
                    }
                    //result.Msg = this._itemStockRepoAdapter.UpdateforModel(currentStock) + "_更新成功";
                    //System.Threading.Thread.Sleep(60);
                    if (countStock > 5)
                    {
                        result.Msg = this._itemStockRepoAdapter.UpdateAll(itemStockList) + "_更新成功";
                        System.Threading.Thread.Sleep(160);
                        countStock = 0;
                    }
                }

                if (itemStockList.Count > 0)
                {
                    //觸發賣場統一售價計算
                    try
                    {
                        result.Msg = this._itemStockRepoAdapter.UpdateAll(itemStockList) + "_更新成功";
                        System.Threading.Thread.Sleep(100);
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.Msg = ex.Message;
                        result.IsSuccess = false;
                    }
                    //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_賣場統一顯示售價計算:" + string.Join(",", changeList) + "_Msg:" + msg);
                }

            }
            watch.Stop();
            return result;
            //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_資料操作耗時(ms):" + watch.ElapsedMilliseconds);
            }
            catch(Exception e){
                result.Msg = result.Msg + "\r\n" + e.Message;
                result.IsSuccess = false;
                return result;
            }
        }
    }
}
