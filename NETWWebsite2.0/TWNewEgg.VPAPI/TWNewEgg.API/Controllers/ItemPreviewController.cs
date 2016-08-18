using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Attributes;
using System.Transactions;
using TWNewEgg.API.Models;
using TWNewEgg.Models.ViewModels.Item;
//using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.API.Controllers
{
    public class ItemPreviewController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Service.ItemSketchService sketchService = new ItemSketchService();
        TWNewEgg.API.Service.TempService tempService = new TempService();
        TWNewEgg.API.Service.ItemSketchPropertyService Service = new ItemSketchPropertyService();
        TWNewEgg.API.Service.PropertyService PropertyService = new PropertyService();
        /// <summary>
        /// 供預覽拿Item資料使用，參數格式為 Type_SellerID_ItemID_Time
        /// Type: 區分 Temp or Sketch or Item正式
        /// SellerID: 搜尋時需要使用
        /// ItemID: ItemTempID or ItemSketchID or ItemID
        /// Time: 時間要做為預覽失效的判斷，以 10 分鐘為限
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPreViewInfo(string previewInfo)
        {
            //string testString = "temp_47_72654_" + DateTime.Now.AddMinutes(-2).ToString("yyyy/MM/dd HH:mm:ss");
            //previewInfo = AesEncryptor.AesEncrypt(testString);
            logger.Info("PreviewInfo: " + previewInfo);
            ActionResponse<TWNewEgg.Models.ViewModels.Item.ItemBasic> result = new ActionResponse<TWNewEgg.Models.ViewModels.Item.ItemBasic>();
            string getParmaInfo = AesEncryptor.AesDecrypt(previewInfo);
            
            if (getParmaInfo == "Decrypt Fail.")
            {
                logger.Error("Decrypt Fail.");
                throw new Exception("Drcrypt Fail.");
            }
            logger.Info("Parma: " + getParmaInfo);
            
            string[] parmaInfo = getParmaInfo.Split('_');
            DateTime previewTime = new DateTime();
            // 判斷時間有沒有超過 10 分鐘
            if (!string.IsNullOrEmpty(previewInfo) && parmaInfo.Count() == 4 && DateTime.TryParse(parmaInfo[3], out previewTime))
            {
                DateTime getTime = DateTime.UtcNow.AddHours(8);
                System.TimeSpan diff1 = getTime.Subtract(previewTime).Duration();
                if (diff1.Minutes < 10 && diff1.Minutes >= 0)
                {
                    int sellerID = 0;
                    int.TryParse(parmaInfo[1], out sellerID);
                    result = this.getPreviewModel(parmaInfo[0], sellerID, parmaInfo[2]);
                }
                else
                {
                    logger.Info("連結超過10分鐘已失效");
                    result.Msg = "連結已失效請重新查詢";
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                }
            }
            else
            {
                logger.Info("連結解密失敗");
                result.Code = (int)ResponseCode.AccessError;
                result.Msg = "連結已失效請重新查詢!";
                result.IsSuccess = false;
            }
            //var json_Preview = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            //logger.Info(json_Preview);

            return Json(result , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 利用 ItemType 用不同Service 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="id"></param>
        private ActionResponse<TWNewEgg.Models.ViewModels.Item.ItemBasic> getPreviewModel(string itemType, int sellerID, string id)
        {
            ActionResponse<TWNewEgg.Models.ViewModels.Item.ItemBasic> result = new ActionResponse<TWNewEgg.Models.ViewModels.Item.ItemBasic>();
            TWNewEgg.API.Models.ItemSketchSearchCondition itemSearch = new TWNewEgg.API.Models.ItemSketchSearchCondition();
            ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>> TwoItemSearch = new ActionResponse<List<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData>>();
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> TwoItemProductDetailSearch = new ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>>();
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> TwoItemProductSearch = new ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> TwoItemProductSketchDetailSearch = new ActionResponse<List<TWNewEgg.API.Models.ItemSketch>>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> twoSketchSearchResult = new ActionResponse<List<TWNewEgg.API.Models.ItemSketch>>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> searchResult = new ActionResponse<List<TWNewEgg.API.Models.ItemSketch>>();
            TWNewEgg.API.Service.PropertyService propertyService = new PropertyService();
            TWNewEgg.API.Service.ItemSketchPropertyService twosketchService = new TWNewEgg.API.Service.ItemSketchPropertyService();
            List<ItemMarketGroup> ListMarketGroup;
            //List<TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption> listPaytype = null;
            //List<TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption> listDelivery = null;
            //List<string> listPromotion = null;

            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            ItemBasic itemBasic = new ItemBasic();
            Item searchItem = new Item();
            Product searchProduct = new Product();
            Seller searchSeller = new Seller();
            Country searchCountry = new Country();

            itemSearch.KeyWord = id;
            itemSearch.SellerID = sellerID;
            //二維或一維商品  
            int type = 1;
            // 取得搜尋結果
            try
            {
                switch (itemType.ToLower())
                {
                    case "temp":
                        // 找出此 Seller 的規格品待審 ID
                        int itemtempidval = int.Parse(itemSearch.KeyWord);
                        bool ItemTempGroup = dbFront.ItemGroupDetailProperty.Where(x => x.SellerID == itemSearch.SellerID && x.ItemTempID == itemtempidval).Any();

                        int val = int.Parse(itemSearch.KeyWord);
                        if (ItemTempGroup == false)
                        {
                            itemSearch.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemTempID;
                            searchResult = tempService.ItemList(itemSearch, true);
                        }
                        else
                        {
                            logger.Info("二維品規格待審預覽開始");
                            type = 2;
                            //規格品群組data
                            itemSearch.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemTempID;
                            TwoItemSearch = Service.ItemSketchDetailPropertyListSearch(itemSearch, true);
                            //點擊商品基本資料
                            TwoItemProductDetailSearch = PropertyService.PropertyServiceList(itemSearch);
                        }
                        break;
                    case "sketch":
                        int checkid = int.Parse(itemSearch.KeyWord);
                        List<DB.TWSQLDB.Models.ItemSketchProperty> ItemSketchGroup = dbFront.ItemSketchProperty.Where(x => x.ItemSketchID == checkid).ToList();

                        if (ItemSketchGroup.Count==0)
                        {
                            itemSearch.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemSketchID;
                            searchResult = sketchService.GetItemSketchList(itemSearch);
                        }
                        else
                        {
                            logger.Info("二維品規格草搞預覽開始");
                            type = 2 ;
                            itemSearch.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemSketchID;
                            //規格品群組data
                            TwoItemSearch = Service.ItemSketchDetailSearch(itemSearch, false);
                            //點擊商品基本資料
                            TwoItemProductSketchDetailSearch = Service.ItemSketchPreviewSearch(itemSearch, false);                            
                        }
                        break;
                    // 預備未來給 Item 預覽使用
                    //case "Item":
                    //    break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                logger.Error("ItemType[ " + itemType + " ] 資料取得失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
            }
            if (type == 2)
            {
                switch (itemType.ToLower())
                {
                    case "temp":
                        try
                        {
                            TWNewEgg.API.Models.ItemSketch itemSketchTemp = TwoItemProductDetailSearch.Body[0];
                            searchItem = dbFront.Item.Where(x => x.ID == itemSketchTemp.Item.ItemID).FirstOrDefault();
                            searchProduct = dbFront.Product.Where(x => x.ID == searchItem.ProductID).FirstOrDefault();
                            searchSeller = dbFront.Seller.Where(x => x.ID == searchItem.SellerID).FirstOrDefault();
                            searchCountry = dbFront.Country.Where(x => x.ID == searchSeller.CountryID).FirstOrDefault();
                            itemBasic.ItemSource = (ItemBasic.ItemSourceOption)searchSeller.CountryID;
                            //itemBasic.Name = searchItem.Name;
                            itemBasic.SourceDescription = searchProduct.Description;
                            itemBasic.Name = itemSketchTemp.Product.Name;

                            itemBasic.Amount = (itemSketchTemp.ItemStock.InventoryQty ?? 0) - (itemSketchTemp.ItemStock.InventoryQtyReg ?? 0);
                            itemBasic.ArrivalTime = searchItem.DelvDate;
                            itemBasic.Countdown = null;
                            //itemBasic.DeliveryType =;
                            itemBasic.Description = searchProduct.Description;
                            itemBasic.Id = itemSketchTemp.Item.ID;
                            itemBasic.ImgUrlList = itemSketchTemp.Product.PicPatch_Edit;
                            //itemBasic.ItemGroupDetail =;
                            //itemBasic.PaymentType =
                            itemBasic.Price = itemSketchTemp.Item.MarketPrice ?? 0m;
                            //itemBasic.PromotionList =;
                            //itemBasic.PromotionMessage =;
                            itemBasic.PromotionPrice = itemSketchTemp.Item.PriceCash ?? 0m;
                            itemBasic.Slogan = searchItem.Sdesc;
                            itemBasic.Spec = "";
                            itemBasic.Title = itemSketchTemp.Item.Spechead;
                            //itemBasic.UserReviews =;
                            itemBasic.Warranty = itemSketchTemp.Product.Warranty.ToString();

                            //    //Promotion
                            //    listPromotion = new List<string>();
                            //    listPromotion.Add("折價券");
                            //    listPromotion.Add("抽獎");
                            //    itemBasic.PromotionList = listPromotion;
                            //    itemBasic.PromotionMessage = "限時優惠 , 錯過不再 !";

                            //    //PaymentType
                            //    listPaytype = new List<ItemBasic.ItemPaymentOption>();
                            //    listPaytype.Add(ItemBasic.ItemPaymentOption.信用卡一次付清);
                            //    listPaytype.Add(ItemBasic.ItemPaymentOption.三期零利率);
                            //    listPaytype.Add(ItemBasic.ItemPaymentOption.十八期零利率);
                            //    listPaytype.Add(ItemBasic.ItemPaymentOption.超商付款);
                            //    listPaytype.Add(ItemBasic.ItemPaymentOption.貨到付款);
                            //    itemBasic.PaymentType = listPaytype;

                            //    //DeliveryType
                            //    listDelivery = new List<ItemBasic.ItemDeliveryOption>();
                            //    listDelivery.Add(ItemBasic.ItemDeliveryOption.宅配);
                            //    listDelivery.Add(ItemBasic.ItemDeliveryOption.超商取貨);
                            //    itemBasic.DeliveryType = listDelivery;                            
                        }
                        catch (Exception e)
                        {
                            logger.Error("ItemType[ " + itemType + " ] 資料撈取失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                            result.Msg = "資料查詢錯誤，查無此賣場資訊";
                            result.IsSuccess = false;
                        }
                        ListMarketGroup = new List<ItemMarketGroup>();
                        foreach (var searchresultitem in TwoItemSearch.Body)
                        {
                            ItemMarketGroup mapresult = new ItemMarketGroup();
                            AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData, ItemMarketGroup>()
                                .ForMember(x => x.GroupId, opt => opt.MapFrom(src => src.groupID))
                                .ForMember(x => x.ItemId, opt => opt.MapFrom(src => src.ItemTempId))
                                .ForMember(x => x.MasterPropertyId, opt => opt.MapFrom(src => src.MasterPropertyID))
                                .ForMember(x => x.SecondPropertyId, opt => opt.MapFrom(src => src.PropertyID))
                                .ForMember(x => x.MasterPropertyValueId, opt => opt.MapFrom(src => src.GroupValueID))
                                .ForMember(x => x.MasterPropertyValueDisplay, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.definitions) == true ? src.propertyValue : src.definitions))
                                .ForMember(x => x.SecondPropertyValueId, opt => opt.MapFrom(src => src.ValueID))
                                .ForMember(x => x.SecondPropertyValueDisplay, opt => opt.MapFrom(src => src.ValueName));
                            AutoMapper.Mapper.Map(searchresultitem, mapresult);
                            mapresult.MasterPropertyDisplay = "顏色";
                            mapresult.SecondPropertyDisplay = "尺寸";
                            mapresult.SellingQty = 10;
                            ListMarketGroup.Add(mapresult);
                        }
                        Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> dictItemMarketGroup = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>();
                        dictItemMarketGroup.Add(itemBasic.Id, ListMarketGroup);
                        itemBasic.DictItemMarketGroup = dictItemMarketGroup;
                        result.Body = itemBasic;
                        result.IsSuccess = true;
                        logger.Info("規格品待審API查詢完全結束");
                        break;

                    case "sketch":
                        try
                        {
                            TWNewEgg.API.Models.ItemSketch itemSketchTemp = TwoItemProductSketchDetailSearch.Body[0];
                            //把GroupValueID 跟 ValueID 一樣 拿出PropertyID


                            itemBasic.Name = itemSketchTemp.Product.Name;

                            itemBasic.Amount = (itemSketchTemp.ItemStock.InventoryQty ?? 0) - (itemSketchTemp.ItemStock.InventoryQtyReg ?? 0);
                            itemBasic.ArrivalTime = itemSketchTemp.Item.DelvDate;
                            itemBasic.Countdown = null;
                            //itemBasic.DeliveryType =;
                            itemBasic.Description = itemSketchTemp.Product.Description;
                            itemBasic.Id = itemSketchTemp.ID;
                            itemBasic.ImgUrlList = itemSketchTemp.Product.PicPatch_Edit;
                            //itemBasic.ItemGroupDetail =;
                            //itemBasic.PaymentType =
                            itemBasic.Price = itemSketchTemp.Item.MarketPrice ?? 0m;
                            //itemBasic.PromotionList =;
                            //itemBasic.PromotionMessage =;
                            itemBasic.PromotionPrice = itemSketchTemp.Item.PriceCash ?? 0m;
                            itemBasic.Slogan = itemSketchTemp.Item.Sdesc;
                            itemBasic.Spec = "";
                            itemBasic.Title = itemSketchTemp.Item.Spechead;
                            //itemBasic.UserReviews =;
                            itemBasic.Warranty = itemSketchTemp.Product.Warranty.ToString();

                            ////Promotion
                            //listPromotion = new List<string>();
                            //listPromotion.Add("折價券");
                            //listPromotion.Add("抽獎");
                            //itemBasic.PromotionList = listPromotion;
                            //itemBasic.PromotionMessage = "限時優惠 , 錯過不再 !";

                            ////PaymentType
                            //listPaytype = new List<ItemBasic.ItemPaymentOption>();
                            //listPaytype.Add(ItemBasic.ItemPaymentOption.信用卡一次付清);
                            //listPaytype.Add(ItemBasic.ItemPaymentOption.三期零利率);
                            //listPaytype.Add(ItemBasic.ItemPaymentOption.十八期零利率);
                            //listPaytype.Add(ItemBasic.ItemPaymentOption.超商付款);
                            //listPaytype.Add(ItemBasic.ItemPaymentOption.貨到付款);
                            //itemBasic.PaymentType = listPaytype;

                            ////DeliveryType
                            //listDelivery = new List<ItemBasic.ItemDeliveryOption>();
                            //listDelivery.Add(ItemBasic.ItemDeliveryOption.宅配);
                            //listDelivery.Add(ItemBasic.ItemDeliveryOption.超商取貨);
                            //itemBasic.DeliveryType = listDelivery;                            
                        }
                        catch (Exception e)
                        {
                            logger.Error("ItemType[ " + itemType + " ] 資料撈取失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                            result.Msg = "資料查詢錯誤，查無此賣場資訊";
                            result.IsSuccess = false;
                        }
                        ListMarketGroup = new List<ItemMarketGroup>();
                        foreach (var searchresultitem in TwoItemSearch.Body)
                        {
                            ItemMarketGroup mapresult = new ItemMarketGroup();
                            AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemGroupJoinitemsketchListData, ItemMarketGroup>()
                                .ForMember(x => x.GroupId, opt => opt.MapFrom(src => src.groupID))
                                .ForMember(x => x.ItemId, opt => opt.MapFrom(src => src.ItemTempId))
                                .ForMember(x => x.MasterPropertyId, opt => opt.MapFrom(src => src.MasterPropertyID))
                                .ForMember(x => x.SecondPropertyId, opt => opt.MapFrom(src => src.PropertyID))
                                .ForMember(x => x.MasterPropertyValueId, opt => opt.MapFrom(src => src.GroupValueID))
                                .ForMember(x => x.MasterPropertyValueDisplay, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.definitions) == true ? src.propertyValue : src.definitions))
                                .ForMember(x => x.SecondPropertyValueId, opt => opt.MapFrom(src => src.ValueID))
                                .ForMember(x => x.SecondPropertyValueDisplay, opt => opt.MapFrom(src => src.ValueName));
                            AutoMapper.Mapper.Map(searchresultitem, mapresult);
                            mapresult.MasterPropertyDisplay = "顏色";
                            mapresult.SecondPropertyDisplay = "尺寸";
                            mapresult.SellingQty = 10;
                            ListMarketGroup.Add(mapresult);
                        }

                        Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> sketchdictItemMarketGroup = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>();
                        sketchdictItemMarketGroup.Add(ListMarketGroup[0].GroupId, ListMarketGroup);
                        itemBasic.DictItemMarketGroup = sketchdictItemMarketGroup;
                        result.Body = itemBasic;
                        result.IsSuccess = true;
                        logger.Info("規格品草稿API查詢完全結束");
                        break;

                    default:
                        break;
                }

            }
            else if (searchResult.IsSuccess == true)
            {
                try
                {
                   
                    TWNewEgg.API.Models.ItemSketch itemSketchTemp = searchResult.Body[0];
                    if (itemType.ToLower() == "temp")
                    {
                        searchItem = dbFront.Item.Where(x => x.ID == itemSketchTemp.Item.ItemID).FirstOrDefault();
                        searchProduct = dbFront.Product.Where(x => x.ID == searchItem.ProductID).FirstOrDefault();
                        searchSeller = dbFront.Seller.Where(x => x.ID == searchItem.SellerID).FirstOrDefault();
                        searchCountry = dbFront.Country.Where(x => x.ID == searchSeller.CountryID).FirstOrDefault();

                        //itemBasic.ItemSource = (ItemBasic.ItemSourceOption)searchSeller.CountryID;
                        //itemBasic.Name = searchItem.Name;
                        itemBasic.SourceDescription = searchProduct.Description;
                    }

                    itemBasic.Name = itemSketchTemp.Product.Name;

                    itemBasic.Amount = (itemSketchTemp.ItemStock.InventoryQty ?? 0) - (itemSketchTemp.ItemStock.InventoryQtyReg ?? 0);
                    itemBasic.ArrivalTime = itemSketchTemp.Item.DelvDate;
                    itemBasic.Countdown = null;
                    //itemBasic.DeliveryType =;
                    itemBasic.Description = itemSketchTemp.Product.Description;
                    itemBasic.Id = itemSketchTemp.Item.ItemID ?? 0;
                    itemBasic.ImgUrlList = itemSketchTemp.Product.PicPatch_Edit;
                    //itemBasic.ItemGroupDetail =;
                    //itemBasic.PaymentType =
                    itemBasic.Price = itemSketchTemp.Item.MarketPrice ?? 0m;
                    //itemBasic.PromotionList =;
                    //itemBasic.PromotionMessage =;
                    itemBasic.PromotionPrice = itemSketchTemp.Item.PriceCash ?? 0m;
                    itemBasic.Slogan = itemSketchTemp.Item.Sdesc;
                    itemBasic.Spec = "";
                    itemBasic.Title = itemSketchTemp.Item.Spechead;
                    //itemBasic.UserReviews =;
                    itemBasic.Warranty = itemSketchTemp.Product.Warranty.ToString();

                    result.Body = itemBasic;
                    result.IsSuccess = true;
                }
                catch (Exception e)
                {
                    logger.Error("ItemType[ " + itemType + " ] 資料撈取失敗 [ErrorMsg] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                    result.Msg = "資料查詢錯誤，查無此賣場資訊";
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.Msg = searchResult.Msg;
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
