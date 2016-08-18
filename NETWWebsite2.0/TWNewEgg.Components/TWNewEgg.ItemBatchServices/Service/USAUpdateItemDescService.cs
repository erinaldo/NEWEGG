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
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using TWNewEgg.ItemBatchServices.Interface;

namespace TWNewEgg.ItemBatchServices.Service
{
    public class USAUpdateItemDescService : IUSAUpdateItemDescService
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;
        private ISellerRepoAdapter _sellerRepoAdapter;

        public USAUpdateItemDescService
        (
            IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter productRepoAdapter, ISellerRepoAdapter sellerRepoAdapter
        )
        {
            this._itemRepoAdapter = itemRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
            this._sellerRepoAdapter = sellerRepoAdapter;
        }

        public ActionResponse<List<DomainResult>> DoWork(UpdateModel updateModel)
        {
            string UpdateUser = updateModel.UpdateUser;
            if (UpdateUser == null || UpdateUser == "") {
                UpdateUser = "System_UpdateItemDesc";
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
                                productIDs = _itemRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && AlltempIDstemp.Contains(x.ID) && (x.ItemDesc == null || x.ItemDesc == "")).Select(x => x.ProductID).Distinct().ToList();
                            }
                        }
                        else {
                            if (updateModel.ProductList != null && updateModel.ProductList.Count > 0)
                            {
                                productIDs = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && AlltempIDstemp.Contains(x.ID) && x.SellerID == 2 && (x.SPEC == null || x.SPEC == "") && x.SourceTable == "productfromws").Select(x => x.ID).Distinct().ToList();
                            }
                        }

                        if (productIDs != null && productIDs.Count > 0)
                        {
                            List<Product> ProductList = new List<Product>();
                            List<string> productFromWS = new List<string>();
                            List<int> productIDList = new List<int>();
                            productIDList = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && productIDs.Contains(x.ID) && x.SellerID == 2 && x.SourceTable == "productfromws").Select(x => x.ID).Distinct().ToList();
                            productFromWS = _productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && productIDs.Contains(x.ID) && x.SellerID == 2 && (x.SPEC == null || x.SPEC == "") && x.SourceTable == "productfromws").Select(x => x.SellerProductID).Distinct().ToList();

                            //foreach (var temp in ProductListtemp)
                            //{
                            //    if (temp != null)
                            //    {
                            //        switch (temp.SourceTable.ToLower())
                            //        {
                            //            case "productfromws":
                            //                productFromWS.Add(temp.SellerProductID);
                            //                productIDList.Add(temp.ID);
                            //                break;
                            //        }
                            //    }
                            //}

                            //Get Price
                            if (productFromWS.Count > 0)
                            {
                                ActionResponse<List<DomainResult>> GetDescFromNeweggUSAResult = GetDescFromNeweggUSA(productFromWS, productIDList, UpdateUser);
                                result = GetDescFromNeweggUSAResult;
                            }
                            else {
                                result.IsSuccess = true;
                                result.Msg = "無需要執行資料";
                            }
                        }
                        else
                        {
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
            result.Msg = result.Msg + "\r\n USAUpdateItemDescService DoWork_執行結束";
            return result;
        }

        private ActionResponse<List<DomainResult>> GetDescFromNeweggUSA(List<string> itemnumberList, List<int> productIDList, string UpdateUser)
        {
            ActionResponse<List<DomainResult>> result = new ActionResponse<List<DomainResult>>();
            result.Body = new List<DomainResult>();
            result.IsSuccess = false;
            try{
                List<DomainResult> DomainResult = new List<DomainResult>();
                List<Product> ProductList = this._productRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && productIDList.Contains(x.ID) && x.SellerID == 2 && (x.SPEC == null || x.SPEC == "")).ToList();
               
                NeweggRequest nr = new NeweggRequest();
                Q4S q4S = new Q4S();
                string[] itemnumberArray = itemnumberList.ToArray();

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Reset();
                watch.Start();

                if (ProductList != null)
                {
                    int changeNum = 0;
                    foreach (var Producttemp in ProductList)
                    {
                        string itemnumbertemp = Producttemp.SellerProductID;
                        int failCount = 0;
                        string htmlCode = "";
                        string itemDesc = "";

                        while (failCount < 3)
                        {
                            IWebProxy myProxy = GlobalProxySelection.GetEmptyWebProxy();
                            GlobalProxySelection.Select = myProxy;
                            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                            WebRequest req = HttpWebRequest.Create("http://www.newegg.com/Product/Product.aspx?Item=" + itemnumbertemp);
                            req.Method = "GET";
                            req.ContentType = "text/xml; charset=utf-8";

                            DomainResult DomainResulttemp = new DomainResult();
                            DomainResulttemp.ProductID = Producttemp.ID;
                            DomainResulttemp.SellerProductID = Producttemp.SellerProductID;
                            DomainResulttemp.IsSuccess = false;

                            try
                            {                                
                                using (WebResponse response = req.GetResponse())
                                {
                                    Stream stream = response.GetResponseStream();
                                    using (StreamReader sr = new StreamReader(stream))
                                    {
                                        htmlCode = sr.ReadToEnd();
                                        if (!string.IsNullOrEmpty(htmlCode))                ///有回傳資料
                                        {
                                            htmlCode = Regex.Match(htmlCode, @"<div\sid=\""Overview_Content(.|\n)+<div\sid=\""Details_Content").ToString();
                                            if (!string.IsNullOrEmpty(htmlCode))
                                            //回傳資料有值
                                            {
                                                htmlCode = Regex.Replace(htmlCode, @"[\r\n]+\s{0,}[\r\n]+", " ");
                                                itemDesc = htmlCode.Substring(0, htmlCode.LastIndexOf("<div"));
                                            }
                                            else
                                            {
                                                itemDesc = "";
                                            }
                                        }
                                        else
                                        {
                                            itemDesc = "";
                                        }
                                    }
                                }
                                if (itemDesc != "")
                                {
                                    Producttemp.SPEC = itemDesc;
                                    Producttemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                                    Producttemp.UpdateUser = UpdateUser;
                                    List<Item> ItemList = this._itemRepoAdapter.GetAll().Where(x => (x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || x.DelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.間配) && x.ProductID == Producttemp.ID && x.SellerID == 2 && (x.ItemDesc == null || x.ItemDesc == "")).ToList();
                                    foreach (var itemtemp in ItemList)
                                    {
                                        DomainResult DomainResultitemtemp = new DomainResult();
                                        DomainResultitemtemp.ProductID = Producttemp.ID;
                                        DomainResultitemtemp.SellerProductID = Producttemp.SellerProductID;
                                        DomainResultitemtemp.IsSuccess = false;

                                        itemtemp.ItemDesc = itemDesc;
                                        itemtemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                                        itemtemp.UpdateUser = UpdateUser;

                                        DomainResultitemtemp.ItemID = itemtemp.ID;
                                        DomainResultitemtemp.Log = "商品Item Desc更新成功 ItemDesc:" + itemDesc; ;
                                        DomainResultitemtemp.IsSuccess = true;
                                        result.Body.Add(DomainResultitemtemp);
                                    }
                                    _itemRepoAdapter.UpdateItemList(ItemList);


                                    changeNum++;
                                }
                                DomainResulttemp.ItemID = 0;
                                DomainResulttemp.Log = "商品Product Desc更新成功 ItemDesc:" + itemDesc; ;
                                DomainResulttemp.IsSuccess = true;
                                result.Body.Add(DomainResulttemp);
                                break;
                            }
                            catch (WebException e)
                            {
                                failCount ++ ;
                                DomainResulttemp.ItemID = 0;
                                DomainResulttemp.Log = "商品Product Desc更新失敗\r\n" + e.Message;
                                DomainResulttemp.IsSuccess = false;
                                result.Body.Add(DomainResulttemp);

                                continue;
                            }
                        }
                     _productRepoAdapter.Update(Producttemp);
                    }
                    //if (changeNum != 0)
                    //{
                    //    _productRepoAdapter.UpdateMany(ProductList);
                    //}
                }
                watch.Stop();
                result.IsSuccess = true;
                result.Msg = "GetDescFromNeweggUSA 執行完成";
                return result;
                //ProcessStatus(this, "Thread_" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_資料操作耗時(ms):" + watch.ElapsedMilliseconds);
            }
            catch(Exception e){
                result.Msg = result.Msg + "\r\n" + GetExceptionMessage(e) + e.InnerException.StackTrace;
                result.IsSuccess = false;
                return result;
            }
        }
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
    }
}
