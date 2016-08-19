using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.Utility;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class ItemController : BaseController
    {
        // 滿額贈折價狀態設定，是否啟用正式機狀態的開關閥，正式機on、測試機off
        private string promotionGiftStatusTurnON = System.Configuration.ConfigurationManager.AppSettings["PromotionGiftStatusTurnON"];

        public ActionResult Index(int ItemId)
        {
            //考慮到資料傳輸與組合的效能, 麵包屑及所有父階的取得作業, 改由Client利用Ajax呼叫Api來完成
            ItemBasic objItemBasic = null;
            Dictionary<int, ItemUrl> itemUrls = new Dictionary<int, ItemUrl>();
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> dictItemMarketGroup = null;

            objItemBasic = Processor.Request<ItemBasic, ItemDetail>("ItemDetailService", "GetItemDetail", ItemId, promotionGiftStatusTurnON).results;
            //dictItemMarketGroup = this.GetTestData();
            if (objItemBasic == null)
                return RedirectToAction("Index", "Home");

            #region 加價購賣場不給看
            if (objItemBasic.ShowOrder < -1)
            {
                return RedirectToAction("Index", "Home");
            }
            #endregion

            #region 隱藏商品只能給有權限會員觀看
            if (objItemBasic.ShowOrder == -1)
            {
                if (!NEUser.IsAuthticated)
                    return RedirectToAction("Index", "Home");

                var result = Processor.Request<TWNewEgg.Models.ViewModels.Account.AccountVM, TWNewEgg.Models.DomainModels.Account.AccountDM>("AccountService", "GetAccountByEmail", NEUser.Email);
                if (!string.IsNullOrEmpty(result.error) || result.results == null || result.results.MemberAgreement == null || result.results.MemberAgreement.Value != 1)
                    return RedirectToAction("Index", "Home");
            }
            #endregion

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemId }).results;

            var listImgUrl = new List<string>();
            foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemId].Where(x => x.Size == 640))
            {
                if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    listImgUrl.Add(singleImgUrl.ImageUrl);
                else
                    listImgUrl.Add(ImageUtility.GetImagePath(singleImgUrl.ImageUrl));
            }
            objItemBasic.ImgUrlList = listImgUrl;

            //取得ItemGroup
            dictItemMarketGroup = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ItemMarketGroup>>>("ItemGroupService", "GetRelativeItemMarketGroupByItemId", ItemId).results;
            if (dictItemMarketGroup != null)
                objItemBasic.DictItemMarketGroup = dictItemMarketGroup;

            #region 取得可用的信用卡紅利折抵資料
            var _getBankBonusdata = this.getBankBonusdata();
            foreach (var item in _getBankBonusdata)
            {
                ViewBag.BankCount = item.Key;
                ViewBag.BankList = item.Value;
            }




            #endregion

            //若該商品為間配且商家為美蛋，則需要帶出告示：『本產品為原廠真品平行輸入之原裝進口產品』
            if (objItemBasic.DelvType == 1 && objItemBasic.SellerID == 2)
                objItemBasic.IsUSRemark = true;
            else
                objItemBasic.IsUSRemark = false;

            return View(objItemBasic);
        }

        public ActionResult ItemPreview(string p, string itemID, string UserName = "")
        {
            /*
            ItemBasic objTestData = this.GetTestData();
            return View(objTestData);
             */

            ItemBasic objItemBasic = null;
            if (string.IsNullOrEmpty(itemID))
            {
                //objItemBasic = getTestItemBasic();
                //return View(objItemBasic);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ResponsePacket<JsonResult> results = null;
                switch (p)
                {
                    case "ip":
                        //IPP
                        results = Processor.Request<JsonResult, JsonResult>("Controllers.ItemVerifyMaintainController", "GetPreViewInfo", itemID, UserName);
                        break;
                    //case "sp":
                    default:
                        //Seller Protal
                        results = Processor.Request<JsonResult, JsonResult>("Controllers.ItemPreviewController", "GetPreViewInfo", itemID);
                        break;
                }

                if (results == null) return RedirectToAction("Index", "Home");

                //var results = Processor.Request<JsonResult, JsonResult>("Controllers.ItemPreviewController", "GetPreViewInfo", itemID);
                if (!string.IsNullOrEmpty(results.error) || results.results == null)
                    return RedirectToAction("Index", "Home");

                //objItemBasic = results.results;

                //var convertData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ItemBasic>>(results.results.Data.ToString());
                var convertData = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemApi>(results.results.Data.ToString());

                if (convertData.Body == null)
                {
                    if (convertData.Msg == null)
                        return RedirectToAction("Index", "Home");
                    else
                        ViewBag.ErrMsg = convertData.Msg;
                }
                else
                {
                    objItemBasic = convertData.Body;
                    objItemBasic.Status = 0;
                    objItemBasic.DateStart = DateTime.Now.AddDays(-2);
                    objItemBasic.DateEnd = DateTime.Now.AddDays(2);
                }

                // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160530
                if (objItemBasic.Id > 0)
                {
                    var GetItemInfo_result = Processor.Request<TWNewEgg.Models.DomainModels.Item.ItemInfo,
                                   TWNewEgg.Models.DomainModels.Item.ItemInfo>("ItemInfoService", "GetItemInfo", objItemBasic.Id);
                    var item_base = GetItemInfo_result.results.ItemBase;
                    objItemBasic.Discard4 = item_base.Discard4;
                }
                // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160530

                return View(objItemBasic);
            }
        }

        //private Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> GetTestData()
        private ItemBasic GetTestData()
        {
            TWNewEgg.Models.ViewModels.Item.ItemBasic objItemBasic = new ItemBasic();
            List<TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption> listPaytype = null;
            List<TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption> listDelivery = null;
            List<string> listImgUrl = null;
            List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> listPromotion = null;
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> dictItemMarketGroup = null;
            List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup> listItemMarketGroup = null;
            TWNewEgg.Models.ViewModels.Item.ItemMarketGroup objItemMarketGroup = null;
            int numItemId = 1000;

            objItemBasic.Status = 0;
            objItemBasic.DateStart = DateTime.Now.AddDays(-2);
            objItemBasic.DateEnd = DateTime.Now.AddDays(2);
            objItemBasic.Amount = 10;
            objItemBasic.ArrivalTime = "5日";
            objItemBasic.Countdown = null;
            objItemBasic.Description = "<h1 class='itemName'>紅色全皮防刮全皮橢圓是牌手提包</h1><p>請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹<br>請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹請填入產品詳細解紹</p><img alt='' src='/Themes/preview/img/item2.jpg'>";
            objItemBasic.Id = numItemId;
            objItemBasic.ItemSource = ItemBasic.ItemSourceOption.中國;
            objItemBasic.Name = "紅色全皮防刮全皮橢圓是牌手提包";
            objItemBasic.Price = 13500;
            objItemBasic.Slogan = "<ul class='introduction'><li>美感質佳亮色防刮全皮</li><li>背面設一小開口袋便利擺放常用小物</li><li>半圓造型佳好搭配/可肩背可斜背</li></ul>";
            objItemBasic.SourceDescription = "SourceDescription SourceDescription SourceDescription";
            objItemBasic.Spec = "SPEC SPEC SPEC SPEC";
            objItemBasic.Title = "春夏最新款美國直送";
            objItemBasic.UserReviews = "User Reviews User Reviews User Reviews User Reviews";
            objItemBasic.Warranty = "Warranty Warranty Warranty Warranty";

            //Promotion

            //ImgUrlList
            listImgUrl = new List<string>();
            listImgUrl.Add("/Themes/preview/img/item2.jpg");
            listImgUrl.Add("/Themes/preview/img/item2_01.jpg");
            listImgUrl.Add("/Themes/preview/img/item2_02.jpg");
            listImgUrl.Add("/Themes/preview/img/item2_03.jpg");
            listImgUrl.Add("/Themes/preview/img/item2_04.jpg");
            listImgUrl.Add("/Themes/preview/img/logo2.jpg");
            objItemBasic.ImgUrlList = listImgUrl;

            //PaymentType
            listPaytype = new List<ItemBasic.ItemPaymentOption>();
            listPaytype.Add(ItemBasic.ItemPaymentOption.信用卡一次付清);
            listPaytype.Add(ItemBasic.ItemPaymentOption.三期零利率);
            listPaytype.Add(ItemBasic.ItemPaymentOption.十八期零利率);
            listPaytype.Add(ItemBasic.ItemPaymentOption.超商付款);
            listPaytype.Add(ItemBasic.ItemPaymentOption.貨到付款);
            objItemBasic.PaymentType = listPaytype;

            //DeliveryType
            listDelivery = new List<ItemBasic.ItemDeliveryOption>();
            listDelivery.Add(ItemBasic.ItemDeliveryOption.宅配);
            listDelivery.Add(ItemBasic.ItemDeliveryOption.超商取貨);
            objItemBasic.DeliveryType = listDelivery;

            //ItemGroup
            int numGroupId = 1;
            int numMasterPropertyId = 701;
            int numSecPropertyId = 601;
            string strMasterPropertyDisplay = "顏色";
            string strSecPropertyDisplay = "尺寸";
            dictItemMarketGroup = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>();
            listItemMarketGroup = new List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>();

            //紅+L
            objItemMarketGroup = new TWNewEgg.Models.ViewModels.Item.ItemMarketGroup();
            objItemMarketGroup.GroupId = numGroupId;
            objItemMarketGroup.MasterPropertyId = numMasterPropertyId;
            objItemMarketGroup.MasterPropertyDisplay = strMasterPropertyDisplay;
            objItemMarketGroup.SecondPropertyId = numSecPropertyId;
            objItemMarketGroup.SecondPropertyDisplay = strSecPropertyDisplay;
            objItemMarketGroup.ItemId = numItemId;
            objItemMarketGroup.MasterPropertyValueId = 7001;
            objItemMarketGroup.MasterPropertyValueDisplay = "紅";
            objItemMarketGroup.SecondPropertyValueId = 6001;
            objItemMarketGroup.SecondPropertyValueDisplay = "L";
            objItemMarketGroup.SellingQty = 10;
            listItemMarketGroup.Add(objItemMarketGroup);
            //藍+L
            objItemMarketGroup = new TWNewEgg.Models.ViewModels.Item.ItemMarketGroup();
            objItemMarketGroup.GroupId = numGroupId;
            objItemMarketGroup.MasterPropertyId = numMasterPropertyId;
            objItemMarketGroup.MasterPropertyDisplay = strMasterPropertyDisplay;
            objItemMarketGroup.SecondPropertyId = numSecPropertyId;
            objItemMarketGroup.SecondPropertyDisplay = strSecPropertyDisplay;
            objItemMarketGroup.ItemId = 2000;
            objItemMarketGroup.MasterPropertyValueId = 7002;
            objItemMarketGroup.MasterPropertyValueDisplay = "藍";
            objItemMarketGroup.SecondPropertyValueId = 6001;
            objItemMarketGroup.SecondPropertyValueDisplay = "L";
            objItemMarketGroup.SellingQty = 10;
            listItemMarketGroup.Add(objItemMarketGroup);
            //紅+M
            objItemMarketGroup = new TWNewEgg.Models.ViewModels.Item.ItemMarketGroup();
            objItemMarketGroup.GroupId = numGroupId;
            objItemMarketGroup.MasterPropertyId = numMasterPropertyId;
            objItemMarketGroup.MasterPropertyDisplay = strMasterPropertyDisplay;
            objItemMarketGroup.SecondPropertyId = numSecPropertyId;
            objItemMarketGroup.SecondPropertyDisplay = strSecPropertyDisplay;
            objItemMarketGroup.ItemId = 3000;
            objItemMarketGroup.MasterPropertyValueId = 7001;
            objItemMarketGroup.MasterPropertyValueDisplay = "紅";
            objItemMarketGroup.SecondPropertyValueId = 6002;
            objItemMarketGroup.SecondPropertyValueDisplay = "M";
            objItemMarketGroup.SellingQty = 10;
            listItemMarketGroup.Add(objItemMarketGroup);
            //藍+M
            objItemMarketGroup = new TWNewEgg.Models.ViewModels.Item.ItemMarketGroup();
            objItemMarketGroup.GroupId = numGroupId;
            objItemMarketGroup.MasterPropertyId = numMasterPropertyId;
            objItemMarketGroup.MasterPropertyDisplay = strMasterPropertyDisplay;
            objItemMarketGroup.SecondPropertyId = numSecPropertyId;
            objItemMarketGroup.SecondPropertyDisplay = strSecPropertyDisplay;
            objItemMarketGroup.ItemId = 4000;
            objItemMarketGroup.MasterPropertyValueId = 7002;
            objItemMarketGroup.MasterPropertyValueDisplay = "藍";
            objItemMarketGroup.SecondPropertyValueId = 6002;
            objItemMarketGroup.SecondPropertyValueDisplay = "M";
            objItemMarketGroup.SellingQty = 10;
            listItemMarketGroup.Add(objItemMarketGroup);

            dictItemMarketGroup.Add(numGroupId, listItemMarketGroup);
            objItemBasic.DictItemMarketGroup = dictItemMarketGroup;

            return objItemBasic;
        }

        public Dictionary<int, string> getBankBonusdata()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var getBankInfo = Processor.Request<TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>>>("BankBonusService", "GetAllEffectiveBankBonus");
            try
            {
                if (string.IsNullOrEmpty(getBankInfo.error) == false)
                {
                    result.Add(0, string.Empty);
                }
                else
                {
                    if (getBankInfo.results.IsSuccess == false)
                    {
                        result.Add(0, string.Empty);
                    }
                    else
                    {
                        if (getBankInfo.results.Data == null)
                        {
                            result.Add(0, string.Empty);
                        }
                        else
                        {
                            int count = getBankInfo.results.Data.Count;
                            string Banktemp = string.Empty;
                            int i = 1;
                            foreach (var item in getBankInfo.results.Data)
                            {
                                if (item.PublishBank == getBankInfo.results.Data.LastOrDefault().PublishBank)
                                {
                                    Banktemp += item.PublishBank;
                                }
                                else
                                {
                                    Banktemp += item.PublishBank + "、";
                                }
                                if (i % 5 == 0)
                                {
                                    Banktemp += "</br>";
                                }
                                i++;
                            }
                            Banktemp = Banktemp.Replace("銀行", "");
                            result.Add(count, Banktemp);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                result.Add(0, string.Empty);
            }
            return result;

        }

        public ActionResult Partial_AdditionalItemDetial(int ItemId)
        {
            //int ItemId = 473718;
            //考慮到資料傳輸與組合的效能, 麵包屑及所有父階的取得作業, 改由Client利用Ajax呼叫Api來完成
            ItemBasic objItemBasic = null;
            Dictionary<int, ItemUrl> itemUrls = new Dictionary<int, ItemUrl>();
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> dictItemMarketGroup = null;

            objItemBasic = Processor.Request<ItemBasic, ItemDetail>("ItemDetailService", "GetItemDetail", ItemId, promotionGiftStatusTurnON).results;
            //dictItemMarketGroup = this.GetTestData();
            if (objItemBasic == null)
                return RedirectToAction("Index", "Home");

            #region 隱藏商品只能給有權限會員觀看
            if (objItemBasic.ShowOrder == -1)
            {
                if (!NEUser.IsAuthticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                var result = Processor.Request<TWNewEgg.Models.ViewModels.Account.AccountVM, TWNewEgg.Models.DomainModels.Account.AccountDM>("AccountService", "GetAccountByEmail", NEUser.Email);
                if (!string.IsNullOrEmpty(result.error) ||
                    result.results == null ||
                    result.results.MemberAgreement == null ||
                    result.results.MemberAgreement.Value != 1)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            #endregion

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemId }).results;

            var listImgUrl = new List<string>();
            foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemId].Where(x => x.Size == 640))
            {
                if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    listImgUrl.Add(singleImgUrl.ImageUrl);
                else
                    listImgUrl.Add(ImageUtility.GetImagePath(singleImgUrl.ImageUrl));
            }
            objItemBasic.ImgUrlList = listImgUrl;

            //取得ItemGroup
            dictItemMarketGroup = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ItemMarketGroup>>>("ItemGroupService", "GetRelativeItemMarketGroupByItemId", ItemId).results;
            if (dictItemMarketGroup != null)
                objItemBasic.DictItemMarketGroup = dictItemMarketGroup;

            return PartialView("Partial_AdditionalItemDetial", objItemBasic);
            //return PartialView("Partial_AdditionalItemDetial");
        }
    }
}
