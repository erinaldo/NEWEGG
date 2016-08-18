using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemCategoryTempRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemStockTempRepoAdapters.Interface;
using TWNewEgg.ItemTempRepoAdapters.Interface;
using TWNewEgg.ProductTempRepoAdapters.Interface;
using TWNewEgg.PublicAPI.BatchUpdata.Interface;
using TWNewEgg.PublicApiModels;

namespace TWNewEgg.PublicAPI.BatchUpdata
{
    public class BatchTempOfficial : IBatchTempOfficial
    {
        private string BatchDataCount = System.Configuration.ConfigurationManager.AppSettings["BatchDataCount"] == null ? "100" : System.Configuration.ConfigurationManager.AppSettings["BatchDataCount"].ToString();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private IItemRepoAdapter _iItemRepoAdapter;
        private IProductRepoAdapter _iProductRepoAdapter;
        private IItemcategoryRepoAdapter _iItemcategoryRepoAdapter;
        private IItemStockRepoAdapter _iItemStockRepoAdapter;
        private IItemTempRepoAdapter _iItemTempRepoAdapter;
        private IProductTempRepoAdapters _iProductTempRepoAdapter;
        private IItemCategoryTempRepoAdapters _iItemCategoryTempRepoAdapters;
        private IItemStockTempRepoAdapters _iItemStockTempRepoAdapters;

        public BatchTempOfficial(IItemRepoAdapter iItemRepoAdapter, IProductRepoAdapter iProductRepoAdapter, IItemcategoryRepoAdapter iItemcategoryRepoAdapter, IItemStockRepoAdapter iItemStockRepoAdapter,
            IItemTempRepoAdapter iItemTempRepoAdapter, IProductTempRepoAdapters iProductTempRepoAdapter, IItemCategoryTempRepoAdapters iItemCategoryTempRepoAdapters, IItemStockTempRepoAdapters iItemStockTempRepoAdapters)
        {
            this._iItemRepoAdapter = iItemRepoAdapter;
            this._iProductRepoAdapter = iProductRepoAdapter;
            this._iItemcategoryRepoAdapter = iItemcategoryRepoAdapter;
            this._iItemStockRepoAdapter = iItemStockRepoAdapter;
            this._iItemTempRepoAdapter = iItemTempRepoAdapter;
            this._iProductTempRepoAdapter = iProductTempRepoAdapter;
            this._iItemCategoryTempRepoAdapters = iItemCategoryTempRepoAdapters;
            this._iItemStockTempRepoAdapters = iItemStockTempRepoAdapters;
        }
        public BatchResponse EditBatchUpdate(List<ItemSketchEdit> modelItemEdit, int userid)
        {
            BatchResponse _EditResultModelResult = new BatchResponse();
            List<DataCheckStatus> listDataCheckStatusResult = new List<DataCheckStatus>();
            int BatchDataCountInt = this.String2Int(BatchDataCount);
            int dataTotal = modelItemEdit.Count;
            int takeCount = dataTotal / BatchDataCountInt;
            modelItemEdit = modelItemEdit.OrderBy(p => p.dataCheckStatus.index).ToList();

            for (int i = 0; i <= takeCount; i++)
            {
                var editModelTemp = modelItemEdit.Skip(i * BatchDataCountInt).Take(BatchDataCountInt).ToList();
                #region official datas
                List<int> itemidList = editModelTemp.Where(p => p.dataCheckStatus.isCorrect == true).Select(p => p.ItemID.Value).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.Item> itemDataList = this._iItemRepoAdapter.GetAll().Where(p => itemidList.Contains(p.ID)).ToList();
                List<int> productIDList = itemDataList.Select(p => p.ProductID).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.Product> productDataList = this._iProductRepoAdapter.GetAll().Where(p => productIDList.Contains(p.ID)).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> itemCategoryList = this._iItemcategoryRepoAdapter.GetAll().Where(p => itemidList.Contains(p.ItemID)).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> itemStockList = this._iItemStockRepoAdapter.GetAll().Where(p => productIDList.Contains(p.ProductID)).ToList();
                #endregion
                #region temp datas
                List<int> itemTempIDList = itemDataList.Select(p => p.ItemtempID.Value).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp> itemTempDataList = this._iItemTempRepoAdapter.GetAll().Where(p => itemTempIDList.Contains(p.ID)).ToList();
                List<int> productTempIDList = itemTempDataList.Select(p => p.ProduttempID.Value).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp> productTempDataList = this._iProductTempRepoAdapter.GetAll().Where(p => productTempIDList.Contains(p.ID)).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> itemCategoryTempDataList = this._iItemCategoryTempRepoAdapters.GetAll().Where(p => itemTempIDList.Contains(p.itemtempID)).ToList();
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp> itemStocktempTempDataList = this._iItemStockTempRepoAdapters.GetAll().Where(p => productTempIDList.Contains(p.producttempID)).ToList();
                #endregion

                for (int j = 0; j < editModelTemp.Count; j++)
                {
                    if (editModelTemp[j].dataCheckStatus.isCorrect == false)
                    {
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    ststusChange _ststusChange = new ststusChange();
                    #region item and itemTemp
                    #region itemtemp
                    var itemTempDataTemp = itemTempDataList.Where(p => p.ItemID == editModelTemp[j].ItemID.Value).FirstOrDefault();
                    //statustemp for name
                    _ststusChange.Name = itemTempDataTemp.Name;
                    _ststusChange.PriceCash = itemTempDataTemp.PriceCash;
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp> _isEditItemTemp = this.isEditItemTemp(itemTempDataTemp, editModelTemp[j], userid);
                    if (_isEditItemTemp.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #region item
                    var itemDataTemp = itemDataList.Where(p => p.ID == editModelTemp[j].ItemID.Value).FirstOrDefault();
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Item> _isEditItemOfficial = this.isEditItemOfficial(itemDataTemp, editModelTemp[j], userid);
                    if (_isEditItemOfficial.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #endregion end of item and itemtemp
                    #region product and productTemp
                    #region productTemp
                    var productTempDataTemp = productTempDataList.Where(p => p.ProductID == itemDataTemp.ProductID).FirstOrDefault();
                    //statustemp for cost
                    _ststusChange.Cost = productTempDataTemp.Cost;
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp> _isEditProductTemp = this.isEditProductTemp(productTempDataTemp, editModelTemp[j], userid);
                    if (_isEditProductTemp.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #region product
                    var productDataTemp = productDataList.Where(p => p.ID == itemDataTemp.ProductID).FirstOrDefault();
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Product> _isEditProductOfficial = this.isEditProductOfficial(productDataTemp, editModelTemp[j], userid);
                    if (_isEditProductOfficial.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #endregion
                    #region itemCategory and itemCategoryTemp
                    //Temp model process
                    ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp>> _isEditItemCategoryTemp = new ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp>>();
                    _isEditItemCategoryTemp.Body = new List<Models.DBModels.TWSQLDB.ItemCategoryTemp>();
                    List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> need2DeleteTemp = null;
                    //Officail model process
                    ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory>> _isEditItemCategory = new ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory>>();
                    _isEditItemCategory.Body = new List<Models.DBModels.TWSQLDB.ItemCategory>();
                    List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> need2DeleteOfficial = null;
                    //判斷有無修改跨分類
                    if (editModelTemp[j].SubCategoryID_1_Layer2.HasValue == true || editModelTemp[j].SubCategoryID_2_Layer2.HasValue == true)
                    {
                        int itemTempid = itemTempDataTemp.ID;
                        #region temp
                        need2DeleteTemp = new List<Models.DBModels.TWSQLDB.ItemCategoryTemp>();
                        need2DeleteTemp = itemCategoryTempDataList.Where(p => p.ItemID == editModelTemp[j].ItemID.Value).ToList();
                        _isEditItemCategoryTemp = this.isEditItemCategoryTemp(itemTempid, editModelTemp[j], userid);
                        if (_isEditItemCategoryTemp.IsSuccess == false)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = false;
                            editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                            listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                            continue;
                        }
                        #endregion
                        #region official
                        need2DeleteOfficial = new List<Models.DBModels.TWSQLDB.ItemCategory>();
                        need2DeleteOfficial = itemCategoryList.Where(p => p.ItemID == editModelTemp[j].ItemID.Value).ToList();
                        _isEditItemCategory = this.isEditItemCategory(editModelTemp[j], userid);
                        if (_isEditItemCategory.IsSuccess == false)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = false;
                            editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                            listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                            continue;
                        }
                        #endregion
                    }
                    #endregion
                    #region Itemstock and Itemstocktemp
                    #region Itemstocktemp
                    var itemStockTempDataTemp = itemStocktempTempDataList.Where(p => p.ProductID == itemDataTemp.ProductID).FirstOrDefault();
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp> _isItemStockTemp = this.isItemStockTemp(itemStockTempDataTemp, editModelTemp[j], userid.ToString());
                    if (_isItemStockTemp.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #region Itemstock
                    var itemStockDataTemp = itemStockList.Where(p => p.ProductID == productDataTemp.ID).FirstOrDefault();
                    ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> _isItemStock = this.isItemStock(itemStockDataTemp, editModelTemp[j], userid.ToString());
                    if (_isItemStock.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    #endregion
                    #endregion
                    #region producttempstatus and Itemtempstatus
                    ActionResponse<bool> _itemTempStatus = this.TempStatus_DetailEdit(_ststusChange, editModelTemp[j]);
                    if (_itemTempStatus.IsSuccess == false)
                    {
                        editModelTemp[j].dataCheckStatus.isCorrect = false;
                        editModelTemp[j].dataCheckStatus.reason = "資料處理錯誤";
                        listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                        continue;
                    }
                    if (_itemTempStatus.Body == true)
                    {
                        #region Gross margin
                        decimal _grossMargin = this.GrossMargin(_isEditItemTemp.Body, _isEditProductTemp.Body);
                        #endregion
                        _isEditProductTemp.Body.Status = 1;
                        _isEditItemTemp.Body.Status = 1;
                        _isEditItemTemp.Body.GrossMargin = _grossMargin;
                        _isEditItemTemp.Body.SubmitMan = userid.ToString();
                        _isEditItemTemp.Body.SubmitDate = DateTime.Now;
                    }
                    #endregion
                    #region update temp and official
                    using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                    {
                        ActionResponse<string> updateTempResilt = this.UpdateTemp(_isEditItemTemp.Body, _isEditProductTemp.Body, _isEditItemCategoryTemp.Body, need2DeleteTemp, _isItemStockTemp.Body);
                        if (updateTempResilt.IsSuccess == false)
                        {
                            editModelTemp[j].dataCheckStatus.isCorrect = false;
                            editModelTemp[j].dataCheckStatus.reason = updateTempResilt.Msg;
                            listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                            scope.Dispose();
                        }
                        else
                        {
                            ActionResponse<string> updateOfficialTable = this.UpdateOfficialTable(_isEditItemOfficial.Body, _isEditProductOfficial.Body, _isEditItemCategory.Body, need2DeleteOfficial, _isItemStock.Body);
                            if (updateOfficialTable.IsSuccess == false)
                            {
                                editModelTemp[j].dataCheckStatus.isCorrect = false;
                                editModelTemp[j].dataCheckStatus.reason = updateOfficialTable.Msg;
                                listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                                scope.Dispose();
                            }
                            else
                            {
                                editModelTemp[j].dataCheckStatus.isCorrect = true;
                                editModelTemp[j].dataCheckStatus.reason = "Success";
                                listDataCheckStatusResult.Add(editModelTemp[j].dataCheckStatus);
                                scope.Complete();
                            }
                        }
                    }
                    #endregion
                }
                _EditResultModelResult.code = "0" + ((int)CodeStatue.Success).ToString();
                _EditResultModelResult.codeMessage = CodeStatue.Success.ToString().Replace("_", " ");
                _EditResultModelResult.message = "Request success";
                _EditResultModelResult.dataCheckStatus = listDataCheckStatusResult;
            }
            return _EditResultModelResult;
        }
        #region itemtemp data insert
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp> isEditItemTemp(TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp itemTempDataModel, ItemSketchEdit model, int userid)
        { 
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp> result = new ActionResponse<Models.DBModels.TWSQLDB.ItemTemp>();
            try
            {
                #region Name
                if (!string.IsNullOrEmpty(model.Name))
                {
                    itemTempDataModel.Name = model.Name;
                }
                #endregion
                #region Sdec
                string str_Sdec = this.createSdec(itemTempDataModel.Sdesc, model);
                itemTempDataModel.Sdesc = str_Sdec;
                #endregion
                #region Spechead
                if (string.IsNullOrEmpty(model.Spechead) == false)
                {
                    itemTempDataModel.Spechead = model.Spechead;
                }
                #endregion
                #region priceCash
                if (model.PriceCash.HasValue == true)
                {
                    itemTempDataModel.PriceCash = itemTempDataModel.PriceCard = model.PriceCash.Value;
                }
                #endregion
                #region MarketPrice
                if (model.MarketPrice.HasValue == true)
                {
                    itemTempDataModel.MarketPrice = model.MarketPrice.Value;
                }
                #endregion
                #region Description
                if (!string.IsNullOrEmpty(model.Description))
                {
                    itemTempDataModel.ItemTempDesc = itemTempDataModel.DescTW = model.Description;
                }
                #endregion
                itemTempDataModel.UpdateUser = userid.ToString();
                itemTempDataModel.UpdateDate = DateTime.Now;
                itemTempDataModel.Updated++;
                result.IsSuccess = true;
                result.Body = itemTempDataModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
                
            }
            return result;
        }
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Item> isEditItemOfficial(TWNewEgg.Models.DBModels.TWSQLDB.Item itemDataModel, ItemSketchEdit model, int userid)
        {
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Item> result = new ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Item>();
            try
            {
                #region Sdec
                string str_Sdec = this.createSdec(itemDataModel.Sdesc, model);
                itemDataModel.Sdesc = str_Sdec;
                #endregion
                #region Spechead
                if (string.IsNullOrEmpty(model.Spechead) == false)
                {
                    itemDataModel.Spechead = model.Spechead;
                }
                #endregion

                #region MarketPrice
                if (model.MarketPrice.HasValue == true)
                {
                    itemDataModel.MarketPrice = model.MarketPrice.Value;
                }
                #endregion
                #region Description
                if (!string.IsNullOrEmpty(model.Description))
                {
                    itemDataModel.ItemDesc = itemDataModel.DescTW = model.Description;
                }
                #endregion
                itemDataModel.UpdateUser = userid.ToString();
                itemDataModel.UpdateDate = DateTime.Now;
                itemDataModel.Updated++;
                result.IsSuccess = true;
                result.Body = itemDataModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
            }
            return result;
        }
        #endregion
        #region product and productTemp
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp> isEditProductTemp(TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp productTempDataModel, ItemSketchEdit model, int userid)
        {
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp> result = new ActionResponse<Models.DBModels.TWSQLDB.ProductTemp>();
            try
            {
                if (string.IsNullOrEmpty(model.SellerProductID) == false)
                {
                    productTempDataModel.SellerProductID = model.SellerProductID;
                }
                //ProductTemp Name 不須修改
                //if (string.IsNullOrEmpty(model.Name) == false)
                //{
                //    productTempDataModel.Name = productTempDataModel.NameTW = model.Name;
                //}
                if (string.IsNullOrEmpty(model.Description) == false)
                {
                    productTempDataModel.Description = productTempDataModel.DescriptionTW = model.Description;
                }
                if (model.Cost.HasValue == true)
                {
                    productTempDataModel.Cost = model.Cost.Value;
                }
                if (model.Warranty.HasValue == true)
                {
                    productTempDataModel.Warranty = model.Warranty.Value;
                }
                productTempDataModel.Updated++;
                productTempDataModel.UpdateDate = DateTime.Now;
                productTempDataModel.UpdateUser = userid.ToString();
                result.IsSuccess = true;
                result.Body = productTempDataModel;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
            }
            return result;
        }
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Product> isEditProductOfficial(TWNewEgg.Models.DBModels.TWSQLDB.Product productDataModel, ItemSketchEdit model, int userid)
        {
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.Product> result = new ActionResponse<Models.DBModels.TWSQLDB.Product>();
            try
            {
                if (string.IsNullOrEmpty(model.SellerProductID) == false)
                {
                    productDataModel.SellerProductID = model.SellerProductID;
                }

                if (string.IsNullOrEmpty(model.Description) == false)
                {
                    productDataModel.Description = productDataModel.DescriptionTW = model.Description;
                }

                if (model.Warranty.HasValue == true)
                {
                    productDataModel.Warranty = model.Warranty.Value;
                }
                productDataModel.Updated++;
                productDataModel.UpdateDate = DateTime.Now;
                productDataModel.UpdateUser = userid.ToString();
                result.IsSuccess = true;
                result.Body = productDataModel;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
            }
            return result;
        }
        #endregion
        #region ItemCategory and ItemCategoryTemp
        private ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp>> isEditItemCategoryTemp(int itemTempID, ItemSketchEdit model, int userid)
        { 
            ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp>> _result = new ActionResponse<List<Models.DBModels.TWSQLDB.ItemCategoryTemp>>();
            try
            {
                _result.Body = new List<Models.DBModels.TWSQLDB.ItemCategoryTemp>();
                if (model.SubCategoryID_1_Layer2.HasValue == true)
                {
                    TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp itemcategory = this.createItemCategoryTempModel(model.ItemID.Value, model.SubCategoryID_1_Layer2.Value, itemTempID, userid.ToString());
                    _result.Body.Add(itemcategory);
                }
                if (model.SubCategoryID_2_Layer2.HasValue == true)
                {
                    TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp itemcategory = this.createItemCategoryTempModel(model.ItemID.Value, model.SubCategoryID_2_Layer2.Value, itemTempID, userid.ToString());
                    _result.Body.Add(itemcategory);
                }
                _result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _result.IsSuccess = false;
                _result.Body = null;
                logger.Error(ex.ToString());
            }
            return _result;
            
        }
        private ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory>> isEditItemCategory(ItemSketchEdit model, int userid)
        {
            ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory>> _result = new ActionResponse<List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory>>();
            _result.Body = new List<Models.DBModels.TWSQLDB.ItemCategory>();
            try
            {
                if (model.SubCategoryID_1_Layer2.HasValue == true)
                {
                    TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory itemcategory = this.createItemCategoryModel(model.ItemID.Value, model.SubCategoryID_1_Layer2.Value, userid.ToString());
                    _result.Body.Add(itemcategory);
                }
                if (model.SubCategoryID_2_Layer2.HasValue == true)
                {
                    TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory itemcategory = this.createItemCategoryModel(model.ItemID.Value, model.SubCategoryID_2_Layer2.Value, userid.ToString());
                    _result.Body.Add(itemcategory);
                }
                _result.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _result.IsSuccess = false;
                _result.Body = null;
                logger.Error(ex.ToString());
            }
            return _result;
        }
        #endregion
        #region itemStock and itemStockTemp
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp> isItemStockTemp(TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp itemStockTempDataModel, ItemSketchEdit model, string userid)
        {
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp> result = new ActionResponse<Models.DBModels.TWSQLDB.ItemStocktemp>();
            try
            {
                if (model.InventorySafeQty.HasValue == true)
                {
                    itemStockTempDataModel.SafeQty = model.InventorySafeQty.Value;
                }
                if (model.CanSaleQty.HasValue == true)
                {
                    int QtyReg = itemStockTempDataModel.QtyReg;
                    itemStockTempDataModel.Qty = model.CanSaleQty.Value + QtyReg;
                }
                itemStockTempDataModel.Updated++;
                itemStockTempDataModel.UpdateDate = DateTime.Now;
                itemStockTempDataModel.UpdateUser = userid;
                result.IsSuccess = true;
                result.Body = itemStockTempDataModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
            }
            return result;
        }
        private ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> isItemStock(TWNewEgg.Models.DBModels.TWSQLDB.ItemStock itemStockDataModel, ItemSketchEdit model, string userid)
        {
            ActionResponse<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> result = new ActionResponse<Models.DBModels.TWSQLDB.ItemStock>();
            try
            {
                if (model.InventorySafeQty.HasValue == true)
                {
                    itemStockDataModel.SafeQty = model.InventorySafeQty.Value;
                }
                if (model.CanSaleQty.HasValue == true)
                {
                    int QtyReg = itemStockDataModel.QtyReg;
                    itemStockDataModel.Qty = model.CanSaleQty.Value + QtyReg;
                }
                itemStockDataModel.Updated++;
                itemStockDataModel.UpdateDate = DateTime.Now;
                itemStockDataModel.UpdateUser = userid;
                result.IsSuccess = true;
                result.Body = itemStockDataModel;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = null;
                logger.Error(ex.ToString());
            }
            return result;
        }
        #endregion
        #region 變更 名稱 成品 售價 需改商品審核狀態
        private ActionResponse<bool> TempStatus_DetailEdit(ststusChange _ststusChange, ItemSketchEdit model)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Body = false;
            bool isChangeValue = false;
            try
            {
                // 商品名稱
                if ((!string.IsNullOrEmpty(model.Name)) && model.Name != _ststusChange.Name)
                {
                    isChangeValue = true;
                }
                // 售價
                if (model.PriceCash.HasValue && model.PriceCash != _ststusChange.PriceCash)
                {

                    isChangeValue = true;
                }
                // 成本
                if (model.Cost.HasValue && model.Cost != _ststusChange.Cost)
                {

                    isChangeValue = true;
                }

                if (isChangeValue)
                {
                    result.Body = true;
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error(ex.ToString());
            }
            return result;
        }
        #endregion
        #region update temp and official
        #region temp
        private ActionResponse<string> UpdateTemp(TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp _itemtemp,
           TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp _productTemp,
           List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> _itemCategoryTempEdit,
           List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> _listNeed2Delete,
           TWNewEgg.Models.DBModels.TWSQLDB.ItemStocktemp _itemStockTemp)
        {
            ActionResponse<string> updateResult = new ActionResponse<string>();
            try
            {
                this._iItemTempRepoAdapter.Update(_itemtemp);
                this._iProductTempRepoAdapter.Update(_productTemp);

                //先判斷是否要新增的跨分類值都為-99
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp> tempMinus99 = _itemCategoryTempEdit.Where(p => p.CategoryID == -99).ToList();
                //兩個跨分類值都為-99表示要進行刪除所有跨分類
                if (tempMinus99.Count == 2)
                {
                    var deleteCateforyTemp = _listNeed2Delete.Where(p => p.FromSystem == "1").ToList();
                    this._iItemCategoryTempRepoAdapters.DeleteMany(deleteCateforyTemp);
                }
                else
                {
                    //必須刪除跨分類表示有新增跨分類
                    if (_listNeed2Delete != null)
                    {
                        //刪除原有的誇分類
                        _listNeed2Delete = _listNeed2Delete.Where(p => p.FromSystem == "1").ToList();
                        this._iItemCategoryTempRepoAdapters.DeleteMany(_listNeed2Delete);
                        //新增新的跨分類
                        if (_itemCategoryTempEdit != null)
                        {
                            _itemCategoryTempEdit = _itemCategoryTempEdit.Where(p => p.FromSystem == "1" && p.CategoryID != -99).ToList();
                            this._iItemCategoryTempRepoAdapters.CreateMany(_itemCategoryTempEdit);
                        }
                    }
                }
                this._iItemStockTempRepoAdapters.Update(_itemStockTemp);
                updateResult.IsSuccess = true;
                updateResult.Msg = updateResult.Body = "success";
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                updateResult.IsSuccess = false;
                updateResult.Msg = updateResult.Body = "資料處理失敗";
            }


            return updateResult;
        }
        #endregion
        #region official
        private ActionResponse<string> UpdateOfficialTable(
            TWNewEgg.Models.DBModels.TWSQLDB.Item _item,
            TWNewEgg.Models.DBModels.TWSQLDB.Product _product,
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> _itemCategory,
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> listNeed2Delete,
            TWNewEgg.Models.DBModels.TWSQLDB.ItemStock _itemStock)
        { 
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                this._iItemRepoAdapter.UpdateItem(_item);
                this._iProductRepoAdapter.Update(_product);
                //先判斷是否要新增的跨分類值都為-99
                List<TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory> tempMinus99 = _itemCategory.Where(p => p.CategoryID == -99).ToList();
                //兩個跨分類值都為-99表示要進行刪除所有跨分類
                if (tempMinus99.Count == 2)
                {
                    var _deleteCategory = listNeed2Delete.Where(p => p.FromSystem == "1").ToList();
                    this._iItemcategoryRepoAdapter.DeteleMany(_deleteCategory);
                }
                else
                {
                    //先判斷有沒有要刪除的誇分類，有表示有新增跨分類，所以要先刪除原有的
                    if (listNeed2Delete != null)
                    {
                        var _deleteCategory = listNeed2Delete.Where(p => p.FromSystem == "1").ToList();
                        this._iItemcategoryRepoAdapter.DeteleMany(_deleteCategory);
                        if (_itemCategory != null)
                        {
                            _itemCategory = _itemCategory.Where(p => p.FromSystem == "1" && p.CategoryID != -99).ToList();
                            this._iItemcategoryRepoAdapter.CreateMany(_itemCategory);
                        }
                    }
                }
                this._iItemStockRepoAdapter.Update(_itemStock);
                result.IsSuccess = true;
                result.Body = result.Msg = "Success";
            }
            catch (Exception ex)
            {
                result.Body = result.Code = "資料處理錯誤";
                logger.Error(ex.ToString());
            }

            return result;
        }
        #endregion
        #endregion
        private decimal GrossMargin(TWNewEgg.Models.DBModels.TWSQLDB.ItemTemp itemTempDataModel, TWNewEgg.Models.DBModels.TWSQLDB.ProductTemp productTempDataModel)
        {
            decimal grossMargin;
            if (itemTempDataModel.PriceCash == 0 || productTempDataModel.Cost == null || productTempDataModel.Cost.Value == 0)
            {
                grossMargin = 0;
            }
            else
            {
                grossMargin = (itemTempDataModel.PriceCash - productTempDataModel.Cost.Value) / itemTempDataModel.PriceCash * 100m;
                grossMargin = Decimal.Round(grossMargin, 0);
            }
            return grossMargin;
        }
        private TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp createItemCategoryTempModel(int itemid, int categoryid, int itemTempID, string userid)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.ItemCategoryTemp result = new Models.DBModels.TWSQLDB.ItemCategoryTemp();
            result.itemtempID = itemTempID;
            result.ItemID = itemid;
            result.CategoryID = categoryid;
            result.FromSystem = "1";
            result.CreateDate = DateTime.Now;
            result.CreateUser = userid;
            result.UpdateDate = DateTime.Now;
            result.UpdateUser = userid;
            return result;
        }
        private TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory createItemCategoryModel(int itemid, int categoryid, string userid)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.ItemCategory result = new Models.DBModels.TWSQLDB.ItemCategory();
            result.ItemID = itemid;
            result.CategoryID = categoryid;
            result.FromSystem = "1";
            result.CreateDate = DateTime.Now;
            result.CreateUser = userid;
            result.UpdateDate = DateTime.Now;
            result.UpdateUser = userid;
            return result;
        }
        private string createSdec(string itemDBSdec, ItemSketchEdit model)
        {
            string newSdec = string.Empty;
            if (string.IsNullOrEmpty(itemDBSdec) == false)
            {
                List<string> SdecItemDBSdecList = new List<string>();
                List<string> resultTempList = new List<string>();
                string[] stringSeparators = new string[] { "<li>" };
                string[] splitSdec = itemDBSdec.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                SdecItemDBSdecList = splitSdec.ToList().Select(p => p.Replace("</li>", "")).ToList();
                Dictionary<int, string> _DBSdecDicCreate = this.DBSdecDicCreate(SdecItemDBSdecList, 3);
                Dictionary<int, string> _UsersSdecDicCreate = this.FinalSdecDicCreate(model.Sdesc);
                for (int i = 1; i <= 3; i++)
                {
                    if (string.IsNullOrEmpty(_UsersSdecDicCreate[i]) == true)
                    {
                        resultTempList.Add(_DBSdecDicCreate[i]);
                    }
                    else
                    {
                        resultTempList.Add(_UsersSdecDicCreate[i]);
                    }
                }
                resultTempList.ForEach(p =>
                {
                    if (string.IsNullOrEmpty(p) == false)
                    {
                        if (p.ToLower().Equals("null") == true)
                        {

                        }
                        else
                        {
                            newSdec += "<li>" + p + "</li>";
                        }
                        //if (p == "[CLEAR]")
                        //{
                        //}
                        //else
                        //{
                        //    if (p.ToLower().Equals("null") == true)
                        //    {
                        //    }
                        //    else
                        //    {
                        //        newSdec += "<li>" + p + "</li>";
                        //    }
                        //}
                    }
                });
                return newSdec;
            }
            else
            {
                if (string.IsNullOrEmpty(model.Sdesc.Sdesc1) == false)
                {
                    newSdec += "<li>" + model.Sdesc.Sdesc1 + "</li>";
                }
                if (string.IsNullOrEmpty(model.Sdesc.Sdesc2) == false)
                {
                    if (model.Sdesc.Sdesc2.ToLower().Equals("null") == false)
                    {
                        newSdec += "<li>" + model.Sdesc.Sdesc2 + "</li>";
                    }
                    //if (model.Sdesc.Sdesc2 != "[CLEAR]")
                    //{
                    //    newSdec += "<li>" + model.Sdesc.Sdesc2 + "</li>";
                    //}
                }
                if (string.IsNullOrEmpty(model.Sdesc.Sdesc3) == false)
                {
                    if (model.Sdesc.Sdesc3.ToLower().Equals("null") == false)
                    {
                        newSdec += "<li>" + model.Sdesc.Sdesc3 + "</li>";
                    }
                    //if (model.Sdesc.Sdesc3 != "[CLEAR]")
                    //{
                    //    newSdec += "<li>" + model.Sdesc.Sdesc3 + "</li>";
                    //}
                }
                return newSdec;
            }
            

        }
        private Dictionary<int, string> FinalSdecDicCreate(Sdesc _sdec)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            result.Add(1, string.IsNullOrEmpty(_sdec.Sdesc1) == true ? string.Empty : _sdec.Sdesc1);
            result.Add(2, string.IsNullOrEmpty(_sdec.Sdesc2) == true ? string.Empty : _sdec.Sdesc2);
            result.Add(3, string.IsNullOrEmpty(_sdec.Sdesc3) == true ? string.Empty : _sdec.Sdesc3);
            return result;
        }
        private Dictionary<int, string> DBSdecDicCreate(List<string> listDdec, int _count)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            Dictionary<int, string> SdecListDic = listDdec.Select((s, i) => new { s, i }).ToDictionary(x => x.i + 1, x => x.s);
            int SdecListDicCount = SdecListDic.Count;
            int need2AddCount = _count - SdecListDicCount;
            for (int i = 0; i < need2AddCount; i++)
            {
                int MaxKey = SdecListDic.Keys.Max();
                SdecListDic.Add(MaxKey + 1, "NULL");
            }
            return SdecListDic;
        }
        private int String2Int(string converStr)
        {
            int result = 0;
            int.TryParse(converStr, out result);
            return result;
        }
    }
}
