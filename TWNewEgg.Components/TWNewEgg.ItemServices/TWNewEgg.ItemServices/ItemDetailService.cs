using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemService.Service;
using TWNewEgg.GroupBuyServices.Interface;
using TWNewEgg.PromotionServices.Interface;
using TWNewEgg.ManufactureServices.Interface;

namespace TWNewEgg.ItemServices
{
    public class ItemDetailService : IItemDetailService
    {
        private IItemInfoService _itemInfoService;
        private IItemDisplayPriceService _itemDisplayPricePrice;
        private IItemStockRepoAdapter _itemStockRepoAdapter;
        private IGroupBuyService _groupBuyService;
        private IItemGroupService _ItemGroupService;
        private IPromotionService _PromotionService;
        private IManufactureService _ManufactureService;

        public ItemDetailService(IItemInfoService itemInfoService, IItemDisplayPriceService itemDisplayPriceService, IItemStockRepoAdapter itemStockRepoAdapter, IGroupBuyService groupBuyService, IItemGroupService argItemGroupService, IPromotionService argPromotionService, IManufactureService argManufactureService)
        {
            this._itemInfoService = itemInfoService;
            this._itemDisplayPricePrice = itemDisplayPriceService;
            this._itemStockRepoAdapter = itemStockRepoAdapter;
            this._groupBuyService = groupBuyService;
            this._ItemGroupService = argItemGroupService;
            this._PromotionService = argPromotionService;
            this._ManufactureService = argManufactureService;
        }

        public ItemDetail GetItemDetail(int itemId, string turnOn = "on")
        {
            // TODO: 等購物車時要將此service替換掉
            O2OHiLifeService hiLifeService = new O2OHiLifeService();
            var payAndDeliverTypes = hiLifeService.getPaymentandDelivery(itemId, 1);

            ItemInfo itemInfo = this._itemInfoService.GetItemInfo(itemId);
            if (itemInfo == null)
            {
                return null;
            }

            var priceDictionary = this._itemDisplayPricePrice.GetItemDisplayPrice(new List<int> { itemId });
            ItemPrice itemDisplayPrice = priceDictionary[itemId];
            //Promotion的參數
            List<Models.DomainModels.Redeem.PromotionDetail> listPromotionBasic = null;
            Models.DomainModels.Redeem.PromotionDetail objPromotionBasic = null;
            Models.DomainModels.Message.ResponseMessage<Models.DBModels.TWSQLDBExtModels.DbPromotionInfo> objPromotionInfo = null;
            //BrandStory的參數
            Models.DomainModels.Manufacture.Manufacture objManufacture = null;
            string strBrandStory = "";
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objItemSellingQty = null;

            //取得Promotion
            objPromotionInfo = this._PromotionService.HasOverPurchaseDiscount(itemId, turnOn);
            if (objPromotionInfo != null && objPromotionInfo.Data != null)
            {
                //進行資料轉換
                listPromotionBasic = new List<Models.DomainModels.Redeem.PromotionDetail>();
                foreach (TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBasic objDbBasic in objPromotionInfo.Data.promotionGiftBasicList)
                {
                    //因此商品為賣場頁用, 故僅需要基本顯示資訊即可
                    objPromotionBasic = new Models.DomainModels.Redeem.PromotionDetail();
                    objPromotionBasic.CSS = objDbBasic.CSS;
                    objPromotionBasic.Description = objDbBasic.Description;
                    objPromotionBasic.ShowDesc = objDbBasic.ShowDesc;
                    objPromotionBasic.HighLight = objDbBasic.HighLight;

                    listPromotionBasic.Add(objPromotionBasic);
                }
            }

            //取得BrandStory
            objManufacture = this._ManufactureService.GetById(itemInfo.ItemBase.ManufactureID);
            if (objManufacture != null && !String.IsNullOrEmpty(objManufacture.BrandStory))
            {
                strBrandStory = objManufacture.BrandStory;
            }
            
            //尋找規格品名稱, 若為規格品則要加上規格品的名稱
            string strItemGroupName = null;
            strItemGroupName = this._ItemGroupService.GetItemMarketGroupNameByItemId(itemId);
            itemInfo.ItemBase.Name += strItemGroupName;

            objItemSellingQty = this._itemStockRepoAdapter.GetItemSellingQtyByItemId(itemId).FirstOrDefault();
            ItemDetail itemDetail = new ItemDetail()
            {
                Main = itemInfo,
                Price = itemDisplayPrice,
                SellingQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0,
                DeliverTypes = payAndDeliverTypes.Select(x => x.DeliverType).Distinct().ToList(),
                PayTypes = payAndDeliverTypes.Select(x => x.PayType0rateNum).Distinct().ToList(),
                GroupBuyEndDate = this._groupBuyService.GetEndDate(0, itemId),
                PromotionBasicList = listPromotionBasic,
                BrandStory = strBrandStory
            };
                

            return itemDetail;
        }


        public List<ItemDetail> GetItemDetails(List<int> itemIds, string turnOn)
        {
            if (string.IsNullOrEmpty(turnOn))
            {
                turnOn = "on";
            }

            List<ItemDetail> results = new List<ItemDetail>();
            for (int i = 0; i < itemIds.Count; i++)
            {
                ItemDetail result = new ItemDetail();
                try
                {
                    result = this.GetItemDetail(itemIds[i], turnOn);
                }
                catch (Exception ex)
                {
                    result = null;
                }
                if (result != null)
                {
                    results.Add(result);
                }
            }
            return results;
        }
    }
}
