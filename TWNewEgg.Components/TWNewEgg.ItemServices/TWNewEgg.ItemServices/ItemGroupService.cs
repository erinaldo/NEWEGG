using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.SellerServices.Interface;

namespace TWNewEgg.ItemServices
{
    public class ItemGroupService : IItemGroupService
    {
        private IItemDisplayPriceService _itemDisplayPriceService;
        private IItemRepoAdapter _itemRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;
        private ISellerServices _iSellerServices;
        private IItemGroupRepoAdapter _ItemGroupRepoAdapter;
        private IItemRepoAdapter _ItemDetailRepoAdapter;
        private IItemStockService _ItemStockService;

        public ItemGroupService(IItemDisplayPriceService itemDisplayPriceService, IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter productRepoAdapter, ISellerServices iSellerServices, IItemGroupRepoAdapter argItemGroupRepoAdapter, IItemRepoAdapter argItemDetailRepoAdapter, IItemStockService argItemStockServices)
        {
            this._itemDisplayPriceService = itemDisplayPriceService;
            this._itemRepoAdapter = itemRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
            this._iSellerServices = iSellerServices;
            this._ItemGroupRepoAdapter = argItemGroupRepoAdapter;
            this._ItemDetailRepoAdapter = argItemDetailRepoAdapter;
            this._ItemStockService = argItemStockServices;
        }

        public bool IsValid(List<int> itemIds)
        {
            throw new NotImplementedException();
        }

        public CartItemGroup BuildItemGroup(List<PostCartItemGroup> PostCartItemGroup)
        {
            List<int> itemIds = new List<int>();
            List<int> itemListIds = new List<int>();
            foreach (var item in PostCartItemGroup)
            {
                itemIds.AddRange(item.PostCartItem.Select(x => x.ItemID).ToList());
                foreach (var itemtemp in item.PostCartItem) {
                    itemListIds.AddRange(itemtemp.PostCartItemList.Select(x => x.ItemID).ToList());
                }
            }

            if (itemIds.Count == 0)
            {
                throw new Exception("No Item ID!!!");
            }
            // ItemDisplayPrice清單
            Dictionary<int, ItemPrice> ItemDisplayPriceList = this._itemDisplayPriceService.GetItemDisplayPrice(itemIds);
            if (ItemDisplayPriceList.Count == 0)
            {
                throw new Exception("No DisplayPriceList!!!");
            }

            // Item清單
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> ItemList = this._itemRepoAdapter.GetItemList(itemIds);
            if (ItemList.Count == 0)
            {
                throw new Exception("No ItemList!!!");
            }

            // Product清單
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Product> ProductList = this._productRepoAdapter.GetListAll(itemIds);
            if (ProductList.Count == 0)
            {
                throw new Exception("No ItemList!!!");
            }

            var ItemGroupBySeller = ItemList.GroupBy(x => x.Value.SellerID).ToList();
            // Seller清單

            List<int> SellerIDList = ItemList.GroupBy(x => x.Value.SellerID).Select(x => x.Key).ToList();
            Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> SellerBaseList = this._iSellerServices.GetSellerWithCountryList(SellerIDList);
            if (SellerBaseList.Count == 0)
            {
                throw new Exception("No SellerBaseList!!!");
            }

            foreach (var ItemGroupBySelleritemp in ItemGroupBySeller) {
                var ItemGroupByDelvType = ItemGroupBySelleritemp.GroupBy(x => x.Value.DelvType).ToList();
                foreach (var ItemGroupByDelvTypetemp in ItemGroupByDelvType) { 
                
                }
            }

            return null;
        }

        /// <summary>
        /// 根據GroupId取得所有的ItemMarketGroup
        /// </summary>
        /// <param name="numGroupId">Group Id</param>
        /// <returns>List of ItemMarketGroup</returns>
        public List<ItemMarketGroup> GetItemMarketGroupByGroupId(int numGroupId)
        {
            if (numGroupId <= 0)
            {
                return null;
            }

            List<Models.DomainModels.Item.ItemMarketGroup> listResult = null;
            Models.DomainModels.Item.ItemMarketGroup objTempMarketGroup = null;
            List<Models.DBModels.TWSQLDB.ItemGroupDetailProperty> listItemGroupDetail = null;
            List<Models.DBModels.TWSQLDB.ItemGroupProperty> listItemGroupProperty = null;
            List<Models.DBModels.TWSQLDB.ItemPropertyName> listPropertyName = null;
            Models.DBModels.TWSQLDB.ItemPropertyName objPropertyName = null;
            List<Models.DBModels.TWSQLDB.ItemPropertyValue> listPropertyValue = null;
            Dictionary<int, string> dictPropertyDisplay = null;
            Dictionary<int, string> dictValueDisplay = null;
            List<int> listTempInt = null;
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> dictItemInfoDetail = null;
            TWNewEgg.Models.DBModels.TWSQLDB.Item objItemInfoDetail = null;
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty = null;
            int i = 0;

            //取得Group
            listItemGroupDetail = this._ItemGroupRepoAdapter.GetItemGroupDetailPropertyByGroupId(numGroupId).Where(x=>x.ItemID != null).ToList();
            if (listItemGroupDetail == null || listItemGroupDetail.Count <= 0)
            {
                return null;
            }

            //過濾掉不符合的賣場
            listTempInt = listItemGroupDetail.Select(x => x.ItemID).Cast<int>().ToList();
            dictItemInfoDetail = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item>();
            foreach (int numItemId in listTempInt)
            {
                objItemInfoDetail = this._ItemDetailRepoAdapter.GetIfAvailable(numItemId);
                if (objItemInfoDetail != null)
                {
                    dictItemInfoDetail.Add(numItemId, objItemInfoDetail);
                }
            }

            if (dictItemInfoDetail == null || dictItemInfoDetail.Count <= 0)
            {
                return null;
            }
            for (i = 0; i < listItemGroupDetail.Count; i++)
            {
                if (dictItemInfoDetail.Count > 0 && dictItemInfoDetail.ContainsKey(Convert.ToInt32(listItemGroupDetail[i].ItemID)))
                {
                    objItemInfoDetail = dictItemInfoDetail[Convert.ToInt32(listItemGroupDetail[i].ItemID)];
                }
                
                if (!dictItemInfoDetail.ContainsKey(Convert.ToInt32(listItemGroupDetail[i].ItemID)) || objItemInfoDetail == null || objItemInfoDetail.Status != 0)
                {
                    listItemGroupDetail.Remove(listItemGroupDetail[i]);
                    i--;
                }
            }

            //取得GroupProperty, 若GroupProperty的PropertyName(自訂顯示名稱)為Null, 就要取ItemPropertyName.Id的值來做
            listItemGroupProperty = this._ItemGroupRepoAdapter.GetItemGroupPropertyByGroupId(numGroupId).OrderBy(x => x.Order).ToList();
            if (listItemGroupProperty != null && listItemGroupProperty.Count > 0)
            {
                listTempInt = listItemGroupProperty.Select(x => x.PropertyID).ToList();
                //取得PropertyGroup預設顯示的名稱
                listPropertyName = this._ItemGroupRepoAdapter.GetAllPropertyName().Where(x => listTempInt.Contains(x.ID)).ToList();
                if (listPropertyName != null && listPropertyName.Count > 0)
                {
                    dictPropertyDisplay = new Dictionary<int, string>();
                    foreach (Models.DBModels.TWSQLDB.ItemGroupProperty objGroupProperty in listItemGroupProperty)
                    {
                        if (dictPropertyDisplay.ContainsKey(objGroupProperty.PropertyID))
                        {
                            continue;
                        }

                        if (objGroupProperty.PropertyName != null && objGroupProperty.PropertyName.Length > 0)
                        {
                            dictPropertyDisplay.Add(objGroupProperty.PropertyID, objGroupProperty.PropertyName);
                        }
                        else
                        {
                            objPropertyName = listPropertyName.Where(x => x.ID == objGroupProperty.PropertyID).FirstOrDefault();
                            if (objPropertyName != null && objPropertyName.PropertyNameTW != null && objPropertyName.PropertyNameTW.Length > 0)
                            {
                                dictPropertyDisplay.Add(objPropertyName.ID, objPropertyName.PropertyNameTW);
                            }
                            else if (objPropertyName != null)
                            {
                                dictPropertyDisplay.Add(objPropertyName.ID, objPropertyName.PropertyName);
                            }
                        } 
                    } ////end foreach
                }
            } ////end if

            //取得PropertyValue 及預設的顯示名稱
            listTempInt = new List<int>();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty objDetail in listItemGroupDetail)
            {
                if (!listTempInt.Contains(objDetail.GroupValueID))
                {
                    listTempInt.Add(objDetail.GroupValueID);
                }
                if (objDetail.ValueID > 0 && !listTempInt.Contains(objDetail.ValueID))
                {
                    listTempInt.Add(objDetail.ValueID);
                }
            }
            if (listTempInt.Count > 0)
            {
                listPropertyValue = this._ItemGroupRepoAdapter.GetAllPropertyValue().Where(x => listTempInt.Contains(x.ID)).ToList();
                if (listPropertyValue != null && listPropertyValue.Count > 0)
                {
                    dictValueDisplay = new Dictionary<int, string>();
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue objValue in listPropertyValue)
                    {
                        if (dictValueDisplay.ContainsKey(objValue.ID))
                        {
                            continue;
                        }

                        if (objValue.PropertyValueTW != null && objValue.PropertyValueTW.Length > 0)
                        {
                            dictValueDisplay.Add(objValue.ID, objValue.PropertyValueTW);
                        }
                        else
                        {
                            dictValueDisplay.Add(objValue.ID, objValue.PropertyValue);
                        }
                    }
                }
            }

            //整理所有列表的資料
            listResult = new List<ItemMarketGroup>();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty objDetail in listItemGroupDetail)
            {
                if (objDetail.ItemID == null)
                {
                    continue;
                }

                objTempMarketGroup = new ItemMarketGroup();
                objTempMarketGroup.GroupId = objDetail.GroupID;
                objTempMarketGroup.ItemId = Convert.ToInt32(objDetail.ItemID);
                //Master相關參數
                objTempMarketGroup.MasterPropertyId = objDetail.MasterPropertyID;
                objTempMarketGroup.MasterPropertyValueId = objDetail.GroupValueID;
                objTempMarketGroup.MasterPropertyDisplay = (string)dictPropertyDisplay[objDetail.MasterPropertyID];
                if (objDetail.InputValue != null && objDetail.InputValue.Length > 0)
                {
                    objTempMarketGroup.MasterPropertyValueDisplay = objDetail.InputValue;
                }
                else
                {
                    objTempMarketGroup.MasterPropertyValueDisplay = (string)dictValueDisplay[objDetail.GroupValueID];
                }

                //Second相關參數
                if (objDetail.PropertyID != null && objDetail.PropertyID > 0)
                {
                    objTempMarketGroup.SecondPropertyId = Convert.ToInt32(objDetail.PropertyID);
                    objTempMarketGroup.SecondPropertyValueId = objDetail.ValueID;
                    objTempMarketGroup.SecondPropertyDisplay = (string)dictPropertyDisplay[objTempMarketGroup.SecondPropertyId];
                    if (objDetail.ValueName != null && objDetail.ValueName.Length > 0)
                    {
                        objTempMarketGroup.SecondPropertyValueDisplay = objDetail.ValueName;
                    }
                    else
                    {
                        objTempMarketGroup.SecondPropertyValueDisplay = (string)dictValueDisplay[objDetail.ValueID];
                    }
                }
                else
                {
                    objTempMarketGroup.SecondPropertyId = 0;
                    objTempMarketGroup.SecondPropertyValueId = 0;
                    objTempMarketGroup.SecondPropertyDisplay = "";
                    objTempMarketGroup.SecondPropertyValueDisplay = "";
                }

                //可銷售數量
                objItemInfoDetail = dictItemInfoDetail[Convert.ToInt32(objDetail.ItemID)];
                objItemSellingQty = this._ItemStockService.GetItemSellingQtyByItemId(Convert.ToInt32(objDetail.ItemID));
                //Do not use this line below...because the data(objItemInfoDetail) query from database, what if someone modify it's data and call SaveChange() in a same DAL scope, the data(objItemInfoDetail) will be changed.
                //objItemInfoDetail.Qty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                int leftQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                //if (objItemInfoDetail.Qty > 0)
                if (leftQty > 0)
                {
                    objTempMarketGroup.SellingQty = 10;
                }
                else
                {
                    objTempMarketGroup.SellingQty = 0;
                }

                listResult.Add(objTempMarketGroup);
            }

            //釋放記憶體
            objTempMarketGroup = null;
            listItemGroupDetail = null;
            listItemGroupProperty = null;
            listPropertyName = null;
            objPropertyName = null;
            listPropertyValue = null;
            dictPropertyDisplay = null;
            dictValueDisplay = null;
            listTempInt = null;
            dictItemInfoDetail = null;

            return listResult;
        }

        /// <summary>
        /// 根據ItemId取得與Item相關的所有ItemGroup
        /// </summary>
        /// <param name="numItemId">Item Id</param>
        /// <returns>Dictionary<GroupId, List of ItemMarketGroup></returns>
        public Dictionary<int, List<ItemMarketGroup>> GetRelativeItemMarketGroupByItemId(int numItemId)
        {
            if (numItemId <= 0)
            {
                return null;
            }

            Dictionary<int, List<ItemMarketGroup>> dictResult = null;
            List<ItemMarketGroup> listItemMarketGroup = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> listItemDetail = null;
            List<int> listTempInt = null;

            listItemDetail = this._ItemGroupRepoAdapter.GetAllItemGroupDetailProperty().Where(x => x.ItemID == numItemId).ToList();
            if (listItemDetail != null && listItemDetail.Count > 0)
            {
                listTempInt = listItemDetail.Select(x => x.GroupID).Distinct().ToList();
                if (listTempInt != null && listTempInt.Count > 0)
                {
                    dictResult = new Dictionary<int, List<ItemMarketGroup>>();
                    foreach (int numGroupId in listTempInt)
                    {
                        if(dictResult.ContainsKey(numGroupId))
                        {
                            continue;
                        }

                        listItemMarketGroup = this.GetItemMarketGroupByGroupId(numGroupId);
                        dictResult.Add(numGroupId, listItemMarketGroup);
                    }
                }
            }

            //釋放記憶體
            listItemDetail = null;
            listTempInt = null;

            return dictResult;
        }

        /// <summary>
        /// 取得ItemGroup
        /// </summary>
        /// <param name="numItemId"></param>
        /// <returns>Dictionary<int of ItemId, list of ItemMarketGroup></returns>
        public Dictionary<int, List<ItemMarketGroup>> GetItemMarketGroupByItemId(int argNumItemId)
        {
            if (argNumItemId <= 0)
            {
                return null;
            }

            List<int> listItemId = null;
            Dictionary<int, List<ItemMarketGroup>> dictResult = null;

            listItemId = new List<int>();
            listItemId.Add(argNumItemId);
            dictResult = this.GetItemMarketGroupByItemId(listItemId, false);

            return dictResult;
        }

        /// <summary>
        /// 取得屬性物件
        /// </summary>
        /// <param name="argListItemId">List of ItemId</param>
        /// /// <param name="argFilterCancelItem">過濾掉下線的賣場, true:過濾, false:不過濾</param>
        /// <returns>null or Dictionary<int of ItemId, Dictionary<PropertyGroupId, ItemMarketGroup>></returns>
        public Dictionary<int, List<ItemMarketGroup>> GetItemMarketGroupByItemId(List<int> argListItemId, bool argFilterCancelItem)
        {
            if (argListItemId == null || argListItemId.Count <= 0)
            {
                return null;
            }

            List<Models.DomainModels.Item.ItemMarketGroup> listMarketGroup = null;
            Models.DomainModels.Item.ItemMarketGroup objTempMarketGroup = null;
            List<Models.DBModels.TWSQLDB.ItemGroupDetailProperty> listItemGroupDetail = null;
            List<Models.DBModels.TWSQLDB.ItemGroupDetailProperty> listTempItemGroupDetail = null;
            List<Models.DBModels.TWSQLDB.ItemGroupProperty> listItemGroupProperty = null;
            List<Models.DBModels.TWSQLDB.ItemPropertyName> listPropertyName = null;
            Models.DBModels.TWSQLDB.ItemPropertyName objPropertyName = null;
            List<Models.DBModels.TWSQLDB.ItemPropertyValue> listPropertyValue = null;
            Dictionary<int, string> dictPropertyDisplay = null;
            Dictionary<int, string> dictValueDisplay = null;
            List<int> listTempInt = null;
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> dictItemInfoDetail = null;
            TWNewEgg.Models.DBModels.TWSQLDB.Item objItemInfoDetail = null;
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty = null;
            int i = 0;
            Dictionary<int, List<ItemMarketGroup>> dictResult = null;

            #region 取得所有ItemGroupPropertyDetail
            //取得ItemGroupPropertyDetail
            listItemGroupDetail = this._ItemGroupRepoAdapter.GetAllItemGroupDetailProperty().Where(x => x.ItemID != null && argListItemId.Contains(x.ItemID.Value)).OrderBy(x=>x.ItemID).ToList();
            if (listItemGroupDetail == null || listItemGroupDetail.Count <= 0)
            {
                return null;
            }

            
            listTempInt = listItemGroupDetail.Select(x => x.ItemID).Cast<int>().ToList();
            dictItemInfoDetail = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item>();
            foreach (int numItemId in listTempInt)
            {
                objItemInfoDetail = this._ItemDetailRepoAdapter.GetIfAvailable(numItemId);
                if (objItemInfoDetail != null)
                {
                    dictItemInfoDetail.Add(numItemId, objItemInfoDetail);
                }
            }

            if (dictItemInfoDetail == null || dictItemInfoDetail.Count <= 0)
            {
                return null;
            }

            //過濾未上線的賣場
            if (argFilterCancelItem)
            {
                for (i = 0; i < listItemGroupDetail.Count; i++)
                {
                    objItemInfoDetail = dictItemInfoDetail[Convert.ToInt32(listItemGroupDetail[i].ItemID)];

                    if (objItemInfoDetail == null || objItemInfoDetail.Status != 0)
                    {
                        listItemGroupDetail.Remove(listItemGroupDetail[i]);
                        i--;
                    }

                }
            }
            #endregion

            #region 取得所有GrouProperty的顯示名稱
            //取得GroupProperty, 若GroupProperty的PropertyName(自訂顯示名稱)為Null, 就要取ItemPropertyName.Id的值來做
            listTempInt = listItemGroupDetail.Select(x => x.GroupID).Distinct().ToList();
            listItemGroupProperty = this._ItemGroupRepoAdapter.GetAllItemGroupProperty().Where(x => listTempInt.Contains(x.GroupID)).OrderBy(x => x.Order).ToList();
            if (listItemGroupProperty != null && listItemGroupProperty.Count > 0)
            {
                listTempInt = listItemGroupProperty.Select(x => x.PropertyID).ToList();
                //取得PropertyGroup預設顯示的名稱
                listPropertyName = this._ItemGroupRepoAdapter.GetAllPropertyName().Where(x => listTempInt.Contains(x.ID)).ToList();
                if (listPropertyName != null && listPropertyName.Count > 0)
                {
                    dictPropertyDisplay = new Dictionary<int, string>();
                    foreach (Models.DBModels.TWSQLDB.ItemGroupProperty objGroupProperty in listItemGroupProperty)
                    {
                        if (dictPropertyDisplay.ContainsKey(objGroupProperty.PropertyID))
                        {
                            continue;
                        }

                        if (objGroupProperty.PropertyName != null && objGroupProperty.PropertyName.Length > 0)
                        {
                            dictPropertyDisplay.Add(objGroupProperty.PropertyID, objGroupProperty.PropertyName);
                        }
                        else
                        {
                            objPropertyName = listPropertyName.Where(x => x.ID == objGroupProperty.PropertyID).FirstOrDefault();
                            if (objPropertyName != null && objPropertyName.PropertyNameTW != null && objPropertyName.PropertyNameTW.Length > 0)
                            {
                                dictPropertyDisplay.Add(objPropertyName.ID, objPropertyName.PropertyNameTW);
                            }
                            else if (objPropertyName != null)
                            {
                                dictPropertyDisplay.Add(objPropertyName.ID, objPropertyName.PropertyName);
                            }
                        }
                    } ////end foreach
                }
            } ////end if

            //取得PropertyValue 及預設的顯示名稱
            listTempInt = new List<int>();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty objDetail in listItemGroupDetail)
            {
                if (!listTempInt.Contains(objDetail.GroupValueID))
                {
                    listTempInt.Add(objDetail.GroupValueID);
                }
                if (objDetail.ValueID > 0 && !listTempInt.Contains(objDetail.ValueID))
                {
                    listTempInt.Add(objDetail.ValueID);
                }
            }
            if (listTempInt.Count > 0)
            {
                listPropertyValue = this._ItemGroupRepoAdapter.GetAllPropertyValue().Where(x => listTempInt.Contains(x.ID)).ToList();
                if (listPropertyValue != null && listPropertyValue.Count > 0)
                {
                    dictValueDisplay = new Dictionary<int, string>();
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue objValue in listPropertyValue)
                    {
                        if (dictValueDisplay.ContainsKey(objValue.ID))
                        {
                            continue;
                        }

                        if (objValue.PropertyValueTW != null && objValue.PropertyValueTW.Length > 0)
                        {
                            dictValueDisplay.Add(objValue.ID, objValue.PropertyValueTW);
                        }
                        else
                        {
                            dictValueDisplay.Add(objValue.ID, objValue.PropertyValue);
                        }
                    }
                }
            }

            #endregion

            #region 整理所有列表資料
            //整理所有列表的資料
            dictResult = new Dictionary<int,  List<ItemMarketGroup>>();
            foreach (int numItemId in argListItemId)
            {
                listTempItemGroupDetail = listItemGroupDetail.Where(x => x.ItemID != null && Convert.ToInt32(x.ItemID) == numItemId).ToList();

                //若沒有GrouPropertyDetailt資料就直接給予null
                if (listTempItemGroupDetail == null || listTempItemGroupDetail.Count <= 0)
                {
                    if (!dictResult.Keys.Contains(numItemId))
                    {
                        dictResult.Add(numItemId, null);
                    }

                    continue;
                }

                //若有資料就建立ItemMarketGroup
                listMarketGroup = new List<ItemMarketGroup>();
                foreach (TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty objDetail in listTempItemGroupDetail)
                {
                    if (objDetail.ItemID == null)
                    {
                        continue;
                    }

                    objTempMarketGroup = new ItemMarketGroup();
                    objTempMarketGroup.GroupId = objDetail.GroupID;
                    objTempMarketGroup.ItemId = Convert.ToInt32(objDetail.ItemID);
                    //Master相關參數
                    objTempMarketGroup.MasterPropertyId = objDetail.MasterPropertyID;
                    objTempMarketGroup.MasterPropertyValueId = objDetail.GroupValueID;
                    objTempMarketGroup.MasterPropertyDisplay = (string)dictPropertyDisplay[objDetail.MasterPropertyID];
                    if (objDetail.InputValue != null && objDetail.InputValue.Length > 0)
                    {
                        objTempMarketGroup.MasterPropertyValueDisplay = objDetail.InputValue;
                    }
                    else
                    {
                        objTempMarketGroup.MasterPropertyValueDisplay = (string)dictValueDisplay[objDetail.GroupValueID];
                    }

                    //Second相關參數
                    if (objDetail.PropertyID != null && objDetail.PropertyID > 0)
                    {
                        objTempMarketGroup.SecondPropertyId = Convert.ToInt32(objDetail.PropertyID);
                        objTempMarketGroup.SecondPropertyValueId = objDetail.ValueID;
                        objTempMarketGroup.SecondPropertyDisplay = (string)dictPropertyDisplay[objTempMarketGroup.SecondPropertyId];
                        if (objDetail.ValueName != null && objDetail.ValueName.Length > 0)
                        {
                            objTempMarketGroup.SecondPropertyValueDisplay = objDetail.ValueName;
                        }
                        else
                        {
                            objTempMarketGroup.SecondPropertyValueDisplay = (string)dictValueDisplay[objDetail.ValueID];
                        }
                    }
                    else
                    {
                        objTempMarketGroup.SecondPropertyId = 0;
                        objTempMarketGroup.SecondPropertyValueId = 0;
                        objTempMarketGroup.SecondPropertyDisplay = "";
                        objTempMarketGroup.SecondPropertyValueDisplay = "";
                    }

                    //可銷售數量
                    objItemInfoDetail = dictItemInfoDetail[Convert.ToInt32(objDetail.ItemID)];
                    objItemSellingQty = this._ItemStockService.GetItemSellingQtyByItemId(Convert.ToInt32(objDetail.ItemID));
                    //Do not use this line below...because the data(objItemInfoDetail) query from database, what if someone modify it's data and call SaveChange() in a same DAL scope, the data(objItemInfoDetail) will be changed.
                    //objItemInfoDetail.Qty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                    int leftQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                    //if (objItemInfoDetail.Qty > 0)
                    if (leftQty > 0)
                    {
                        objTempMarketGroup.SellingQty = 10;
                    }
                    else
                    {
                        objTempMarketGroup.SellingQty = 0;
                    }

                    listMarketGroup.Add(objTempMarketGroup);
                }
                //// end foreach - listTempItemGroupDetail
                if (!dictResult.Keys.Contains(numItemId))
                {
                    dictResult.Add(numItemId, listMarketGroup);
                }
            }
            ////end foreach - argListItemId
            #endregion

            //釋放記憶體
            listItemGroupDetail = null;
            listTempItemGroupDetail = null;
            listItemGroupProperty = null;
            listPropertyName = null;
            objPropertyName = null;
            listPropertyValue = null;
            dictPropertyDisplay = null;
            dictValueDisplay = null;
            listTempInt = null;
            dictItemInfoDetail = null;
            objItemInfoDetail = null;

            return dictResult;
        }

        /// <summary>
        /// 取得屬性名稱
        /// </summary>
        /// <param name="NumItemId">int of Item Id</param>
        /// <returns>string of GroupName, if not has GroupName then return empty</returns>
        public string GetItemMarketGroupNameByItemId(int argNumItemId)
        {
            string strResult = "";
            Dictionary<int, List<ItemMarketGroup>> dictSearch = null;
            ItemMarketGroup objItemMarketGroup = null;

            if (argNumItemId > 0)
            {
                dictSearch = this.GetItemMarketGroupByItemId(argNumItemId);
                if (dictSearch != null && dictSearch.ContainsKey(argNumItemId) && dictSearch[argNumItemId] != null)
                {
                    objItemMarketGroup = dictSearch[argNumItemId].FirstOrDefault();

                    if (objItemMarketGroup != null)
                    {
                        //Master Property Name
                        strResult = "-" + objItemMarketGroup.MasterPropertyValueDisplay;

                        //Second Property Name
                        if (objItemMarketGroup.SecondPropertyValueId > 0 && objItemMarketGroup.SecondPropertyValueDisplay.Length > 0)
                        {
                            strResult += "(" + objItemMarketGroup.SecondPropertyValueDisplay + ")";
                        }
                    }
                }
            }

            dictSearch = null;
            objItemMarketGroup = null; ;

            return strResult;
        }

        /// <summary>
        /// 取得屬性名稱
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns>Dictionary<int of ItemId, string of GroupName>, if not has GroupName then return empty</returns>
        public Dictionary<int, string> GetItemMarketGroupNameByItemIds(List<int> argListItemId)
        {
            Dictionary<int, string> dictResult = null;
            Dictionary<int, List<ItemMarketGroup>> dictSearch = null;
            ItemMarketGroup objItemMarketGroup = null;
            bool boolDefault = true;
            string strGroupName = "";

            dictResult = new Dictionary<int, string>();

            if (argListItemId != null && argListItemId.Count > 0)
            {
                dictSearch = this.GetItemMarketGroupByItemId(argListItemId, false);
                if (dictSearch != null)
                {
                    boolDefault = false;
                }
            }

            if (boolDefault)
            {
                //給預設空白值
                foreach (int numItemId in argListItemId)
                {
                    if (!dictResult.ContainsKey(numItemId))
                    {
                        dictResult.Add(numItemId, "");
                    }
                }
                //// end foreach
            }
            else
            {
                foreach (int numItemId in argListItemId)
                {
                    strGroupName = "";
                    if (dictSearch.ContainsKey(numItemId) && dictSearch[numItemId] != null && dictSearch[numItemId].Count > 0)
                    {
                        objItemMarketGroup = dictSearch[numItemId].FirstOrDefault();
                        if (objItemMarketGroup != null)
                        {
                            //Master Property Name
                            strGroupName = "-" + objItemMarketGroup.MasterPropertyValueDisplay;

                            //Second Property Name
                            if (objItemMarketGroup.SecondPropertyValueId > 0 && objItemMarketGroup.SecondPropertyValueDisplay.Length > 0)
                            {
                                strGroupName += "(" + objItemMarketGroup.SecondPropertyValueDisplay + ")";
                            }
                        }
                    }

                    if (!dictResult.ContainsKey(numItemId))
                    {
                        dictResult.Add(numItemId, strGroupName);
                    }
                }
                //// end foreach
            }


            dictResult = null;
            dictSearch = null;
            objItemMarketGroup = null;

            return dictResult;
        }

    }
}
