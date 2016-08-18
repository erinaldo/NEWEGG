using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ItemService.Models;
//using TWNewEgg.Redeem.Service.PromotionGiftService;
using TWNewEgg.Website.ECWeb.Controllers;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.Website.ECWeb.Service;
using System.Net.Mail;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.SMSService.Service;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.ViewModels.Cart;
using TWNewEgg.Framework.Autofac;
using AutoMapper;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TWNewEgg.Models.ViewModels.Message;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.ECWeb.Services.OldCart.CartService;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels.SendMail;
using TWNewEgg.ECWeb.Utility;
using System.Threading;

namespace TWNewEgg.ECWeb.Controllers
{
    public class CartController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private TWNewEgg.Website.ECWeb.Service.AesCookies aesEnc = new TWNewEgg.Website.ECWeb.Service.AesCookies();
        private string _soFlowSelect = System.Configuration.ConfigurationManager.AppSettings["SOFlowSelect"];
        private string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        private string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
        private string NoticeMail = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SendPOFailNoticeMail"];
        private string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        private string ServiceAccount = System.Configuration.ConfigurationManager.AppSettings["ECWeb_ServiceAccount"];
        private string imageDomain = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"];
        // 滿額贈折價狀態設定，是否啟用正式機狀態的開關閥，正式機on、測試機off
        private string promotionGiftStatusTurnON = System.Configuration.ConfigurationManager.AppSettings["PromotionGiftStatusTurnON"];
        private string invoidException = System.Configuration.ConfigurationManager.AppSettings["InvoidException"];
        private string checkItemAndPriceTurnON = "OFF";
        private string payTypeGatewayTurnON = "on";
        private string crediteAuthTurnON = "on";
        private string OTPServiceGateway = "on";
        private HttpCookieCollection test;
        public CartController()
        {
            try
            {
                OTPServiceGateway = System.Configuration.ConfigurationManager.AppSettings["OTPServiceGateway"];
                checkItemAndPriceTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_PayTypeControllerCheckItemStatusPriceStock"];
                payTypeGatewayTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_PayTypeGateway"];
                crediteAuthTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CrediteAuth"];
            }
            catch
            {
            }
        }

        public CartController(HttpContext context)
        {
            try
            {
                OTPServiceGateway = System.Configuration.ConfigurationManager.AppSettings["OTPServiceGateway"];
                checkItemAndPriceTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_PayTypeControllerCheckItemStatusPriceStock"];
                payTypeGatewayTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_PayTypeGateway"];
                crediteAuthTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CrediteAuth"];
                this.ControllerContext = new System.Web.Mvc.ControllerContext(context.Request.RequestContext, this);
                //test = context.Request.Cookies;
            }
            catch
            {
            }
        }

        #region Trackitem
        public ActionResult Trackitem(int id)
        {
            string xmlPath = @"C:\trackitem\" + id + "\trackitem.xml";
            XmlDocument trackxml = new XmlDocument();

            //判断文件是否存在
            if (System.IO.File.Exists(xmlPath))
            {
                trackxml.LoadXml(xmlPath);
            }

            return View();
        }
        #endregion

        #region Trackflow
        public ActionResult Trackflow()
        {
            return View();
        }
        #endregion

        #region Index

        public ActionResult Index(int? TypeID, string errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                ViewBag.ErrMsg = errMsg;
            }

            ViewBag.TypeID = TypeID;
            int accID = NEUser.ID;
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listActiveCoupon = null;
            Dictionary<string, object> dictResult = null;
            if (accID > 0)
            {
                Processor.Request<bool, bool>("TrackService", "CleanOldAndUpdateTracks", accID, TWNewEgg.Framework.ServiceApi.Configuration.ConfigurationManager.GetTaiwanTime().AddDays(-30));
                listActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", accID.ToString()).results;
                dictResult = new Dictionary<string, object>();
                dictResult.Add("CouponList", listActiveCoupon);

                return View(dictResult);
            }
            else
            {
                return RedirectToAction("Login", "MyAccount", new { returnUrl = "/Cart/Index" });
            }
        }
        #region 讀取對應的 bank 相關資料
        /// <summary>
        /// 取得合法 bank 相關資料
        /// </summary>
        /// <param name="_CartPayType_View"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View getBankBonusesInfo(TWNewEgg.Models.ViewModels.Cart.CartPayType_View _CartPayType_View = null)
        {
            var getBankInfo = Processor.Request<ResponseMessage<List<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>>, ResponseMessage<List<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>>>("BankBonusService", "GetAllEffectiveBankBonus");
            if (string.IsNullOrEmpty(getBankInfo.error) == false)
            {
                return null;
            }
            if (getBankInfo.results.Data == null)
            {
                return null;
            }
            if (_CartPayType_View == null)
            {
                return null;
            }
            _CartPayType_View.BankIDwithName.Clear();
            foreach (var item in getBankInfo.results.Data)
            {
                _CartPayType_View.BankIDwithName.Add(new CartPayTypeBankIDwithName_View
                {
                    BankID = (item.BankBonusID.Equals(null)) ? 0 : item.BankID,
                    BankName = string.IsNullOrEmpty(item.PublishBank) == true ? string.Empty : item.PublishBank,
                    ConsumeMax = item.OffsetMax.Equals(null) ? string.Empty : string.Format("{0:#}", item.OffsetMax),
                    ConsumeMin = item.ConsumeLimit.Equals(null) ? string.Empty : string.Format("{0:#}", item.ConsumeLimit)
                });
                //CartPayTypeBankIDwithName_View _cartPayTypeBankIDwithName_View = new CartPayTypeBankIDwithName_View();
                //_cartPayTypeBankIDwithName_View.BankID = item.BankBonusID == null ? 0 : item.BankBonusID;
                //_cartPayTypeBankIDwithName_View.BankName = string.IsNullOrEmpty(item.PublishBank) == true ? string.Empty : item.PublishBank;
                //_cartPayTypeBankIDwithName_View.ConsumeMax = item.OffsetMax.ToString();
                //_cartPayTypeBankIDwithName_View.ConsumeMin = item.ConsumeLimit.ToString();
                //_CartPayType_View.BankIDwithName.Add(_cartPayTypeBankIDwithName_View);
            }
            return _CartPayType_View;
        }
        #endregion
        public ActionResult NewEggCartList(int? TypeID)
        {
            int accID = NEUser.ID;

            var GetCartAllList = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);

            if (GetCartAllList.error != null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }

            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();

            // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516
            ViewBag.Discard4 = string.Empty;  //確認廢四機商品

            if (TypeID == null)
            {
                foreach (var GetCartAllListresulttemp in GetCartAllList.results)
                {

                    if (GetCartAllListresulttemp.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車 && GetCartAllListresulttemp.Qty > 0)
                    {
                        ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車);
                        string sn = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車, ItemGroup_ViewList);
                        ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車;
                        ViewBag.SerialNumber = sn;
                        //ItemGroup_ViewList.ForEach(p =>
                        //{
                        //    p.CartPayTypeGroupList.ForEach(q =>
                        //    {
                        //        q.CartPayType_View.ForEach(r =>
                        //        {
                        //            r.BankIDwithName = this.GetBackBonuses(r.BankIDwithName);
                        //        });
                        //        //q.CartPayType_View = this.GetBackBonuses(q.CartPayType_View);
                        //    });// = this.GetBackBonuses(p.CartPayTypeBankIDwithName_View);
                        //});
                        //GetBackBonuses


                        // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516
                        foreach (var each_group in ItemGroup_ViewList)    //取得購車商品清單內容
                        {
                            foreach (var each_item in each_group.CartItemList)
                            {
                                int itemId = each_item.ItemID;
                                var GetItemInfo_result = Processor.Request<TWNewEgg.Models.DomainModels.Item.ItemInfo,
                                TWNewEgg.Models.DomainModels.Item.ItemInfo>("ItemInfoService", "GetItemInfo", itemId);
                                if (GetItemInfo_result.error != null) continue;
                                var item_base = GetItemInfo_result.results.ItemBase;
                                if (string.IsNullOrEmpty(item_base.Discard4)) item_base.Discard4 = string.Empty;
                                ViewBag.Discard4 = item_base.Discard4.ToUpper();
                                //break;
                                if (ViewBag.Discard4 != string.Empty)
                                {
                                    if (item_base.Discard4.ToUpper() == "Y") //是廢四機才給值--------20160525
                                    {
                                        ViewBag.Discard4 = item_base.Discard4.ToUpper();
                                        break;
                                    }
                                }
                                continue;
                            }
                            if (ViewBag.Discard4 != string.Empty) break;
                        }
                        // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516



                        return PartialView("Partial_NewEggCartList", ItemGroup_ViewList);
                    }
                    else if (GetCartAllListresulttemp.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車 && GetCartAllListresulttemp.Qty > 0)
                    {
                        ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車);
                        string sn = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車, ItemGroup_ViewList);
                        ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車;
                        ViewBag.SerialNumber = sn;
                        return PartialView("Partial_NewEggCartList", ItemGroup_ViewList);
                    }
                    else if (GetCartAllListresulttemp.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車 && GetCartAllListresulttemp.Qty > 0)
                    {
                        ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車);
                        string sn = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車, ItemGroup_ViewList);
                        ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車;
                        ViewBag.SerialNumber = sn;
                        return PartialView("Partial_NewEggChooseCartList", ItemGroup_ViewList);
                    }
                }

                ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車);
                string sn2 = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車, ItemGroup_ViewList);
                ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車;
                ViewBag.SerialNumber = sn2;
                return PartialView("Partial_NewEggCartList", ItemGroup_ViewList);


            }
            else if (TypeID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車)
            {
                ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車);
                string sn = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車, ItemGroup_ViewList);
                ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車;
                ViewBag.SerialNumber = sn;
                return PartialView("Partial_NewEggChooseCartList", ItemGroup_ViewList);
            }
            else
            {
                ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, TypeID ?? (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車);
                string sn = CreateCartTemp(TypeID.Value, ItemGroup_ViewList);
                ViewBag.TypeID = TypeID;
                ViewBag.SerialNumber = sn;
                return PartialView("Partial_NewEggCartList", ItemGroup_ViewList);
            }
        }

        //購物車推薦加購品
        public ActionResult Partial_AdditionalItem(int CartType)
        {
            var AllAIForCartList = Processor.Request<List<TWNewEgg.Models.DomainModels.AdditionalItem.AllAIForCart>, List<TWNewEgg.Models.DomainModels.AdditionalItem.AllAIForCart>>("AIForCartService", "GetAdditionalItemDetailforShopByCartType", CartType);

            List<AdditionalItem_View> AdditionalItemList = new List<AdditionalItem_View>();
            foreach (var temp in AllAIForCartList.results)
            {
                AdditionalItem_View AdditionalItem_Viewtemp = new AdditionalItem_View();
                AutoMapper.Mapper.Map(temp, AdditionalItem_Viewtemp);
                if (temp.itemGroup != null && temp.itemGroup.Count > 0)
                {
                    List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup> ItemMarketGroupListtemp = new List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>();
                    AutoMapper.Mapper.Map(temp.itemGroup, ItemMarketGroupListtemp);
                    AdditionalItem_Viewtemp.ItemMarketGroup.AddRange(ItemMarketGroupListtemp);
                }
                AdditionalItemList.Add(AdditionalItem_Viewtemp);
            }
            if (AdditionalItemList.Count != 0)
            {
                return PartialView("Partial_AdditionalItem", AdditionalItemList);
            }
            else
            {
                return PartialView("Partial_AdditionalItem", null);
            }
        }

        private string CreateCartTemp(int cartTypeID, List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> itemGroupList)
        {
            List<CartItem_View> CartItemList = new List<CartItem_View>();
            if (itemGroupList != null && itemGroupList.Count > 0)
            {
                foreach (var CartItemClass_ViewListtemp in itemGroupList[cartTypeID - 1].CartItemClass_ViewList)
                {
                    CartItemList.AddRange(CartItemClass_ViewListtemp.CartItemList);
                }
            }

            List<CartItemTempDM> cartItemTempList = ModelConverter.ConvertTo<List<CartItemTempDM>>(CartItemList);
            // 商品總價化
            //List<int> itemIDList = new List<int>();
            //cartItemTempList.ForEach(x =>
            //{
            //    itemIDList.Add(x.ItemID);
            //});
            //if (itemIDList.Count > 0)
            //{
            //    var getDisplayItemResult = Processor.Request<string, string>("ItemDisplayPriceService", "SetItemDisplayPriceByIDs", itemIDList);
            //    if (getDisplayItemResult.error != null)
            //    {
            //        throw new Exception(getDisplayItemResult.error);
            //    }
            //}

            var genSNResult = Processor.Request<string, string>("CartTempServices.CartTempService", "GenerateSerialNumber", NEUser.ID, cartTypeID);
            if (genSNResult.error != null)
            {
                throw new Exception(genSNResult.error);
            }

            string serialNumber = genSNResult.results;

            CartTempDM InitialCartTempDM = new CartTempDM
            {
                SerialNumber = serialNumber,
                AccountID = NEUser.ID,
                CartTypeID = cartTypeID,
                Status = (int)CartTempDM.StatusEnum.Initial,
                IPAddress = NEUser.IPAddress,
                CartItemTempDMs = cartItemTempList
            };

            var createCartTempResult = Processor.Request<CartTempDM, CartTempDM>("CartTempServices.CartTempService", "CreateCartTemp", InitialCartTempDM);
            if (createCartTempResult.error != null)
            {
                throw new Exception(createCartTempResult.error);
            }

            CartTempDM cartTemp = createCartTempResult.results;
            return serialNumber;
        }
        #endregion

        public ActionResult NewEggChooseCartList(int TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車)
        {
            int accID = NEUser.ID;

            var GetCartAllList = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);

            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();
            string sn = CreateCartTemp((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車, ItemGroup_ViewList);
            ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車);
            ViewBag.TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車;
            ViewBag.SerialNumber = sn;

            return View(ItemGroup_ViewList);
        }

        public ActionResult NextCartListMenu(int TypeID = 1)
        {
            int accID = NEUser.ID;

            var GetCartAllList = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);

            if (GetCartAllList.error != null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }

            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = GetItemGroupList(GetCartAllList.results, TypeID);
            string sn = CreateCartTemp(TypeID, ItemGroup_ViewList);
            string resultString = "";
            using (StringWriter sw = new StringWriter())
            {
                //ItemGroup_ViewList.ForEach(p =>
                //{
                //    p.CartPayTypeGroupList.ForEach(q =>
                //    {
                //        q.CartPayType_View.ForEach(r =>
                //        {
                //            r.BankIDwithName = this.GetBackBonuses(r.BankIDwithName);
                //        });
                //        //q.CartPayType_View = this.GetBackBonuses(q.CartPayType_View);
                //    });// = this.GetBackBonuses(p.CartPayTypeBankIDwithName_View);
                //});
                ViewBag.SerialNumber = sn;
                ViewData.Model = ItemGroup_ViewList;
                ViewBag.TypeID = TypeID;

                //處理加購商品時也要處理-------------------add by bruce 20160526

                // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516
                foreach (var each_group in ItemGroup_ViewList)    //取得購車商品清單內容
                {
                    foreach (var each_item in each_group.CartItemList)
                    {
                        int itemId = each_item.ItemID;
                        var GetItemInfo_result = Processor.Request<TWNewEgg.Models.DomainModels.Item.ItemInfo,
                        TWNewEgg.Models.DomainModels.Item.ItemInfo>("ItemInfoService", "GetItemInfo", itemId);
                        if (GetItemInfo_result.error != null) continue;
                        var item_base = GetItemInfo_result.results.ItemBase;
                        if (string.IsNullOrEmpty(item_base.Discard4)) item_base.Discard4 = string.Empty;
                        ViewBag.Discard4 = item_base.Discard4.ToUpper();
                        //break;
                        if (ViewBag.Discard4 != string.Empty)
                        {
                            if (item_base.Discard4.ToUpper() == "Y") //是廢四機才給值--------20160525
                            {
                                ViewBag.Discard4 = item_base.Discard4.ToUpper();
                                break;
                            }
                        }
                        continue;
                    }
                    if (ViewBag.Discard4 != string.Empty) break;
                }
                // 依據 BSATW-173 廢四機需求增加 // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516

                //處理加購商品時也要處理-------------------add by bruce 20160526

                ViewEngineResult viewResult = null;
                if (TypeID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車)
                {
                    viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_NewEggChooseCartList");
                }
                else
                {
                    viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_NewEggCartList");
                }
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                resultString = sw.GetStringBuilder().ToString();
            }
            if (Request.IsAjaxRequest())
            {
                return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
        }

        // 追蹤清單初始化
        public ActionResult NewEggWishCartList(int? TypeID, int ViewPage = 1, int PageNumber = 10, string OrderBy = "DescCreatDate")
        {
            ViewBag.TypeID = TypeID ?? (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車;
            ViewBag.TypeName = Enum.GetName(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType), ViewBag.TypeID).ToString();

            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = GetWishGroupList((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車, ViewPage, PageNumber, OrderBy);
            return PartialView("Partial_NewEggWishList", ItemGroup_ViewList);
        }

        // 換頁追蹤清單
        public ActionResult NextWishCartListMenu(int? TypeID, int ViewPage = 1, int PageNumber = 10, string OrderBy = "DescCreatDate")
        {
            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = GetWishGroupList((int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車, ViewPage, PageNumber, OrderBy);

            string resultString = "";
            using (StringWriter sw = new StringWriter())
            {
                ViewData.Model = ItemGroup_ViewList;
                ViewBag.TypeID = TypeID ?? (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車;
                ViewBag.TypeName = Enum.GetName(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType), ViewBag.TypeID).ToString();
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Partial_NewEggWishList");
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                resultString = sw.GetStringBuilder().ToString();
            }
            if (Request.IsAjaxRequest())
            {
                return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
        }

        /// <summary>
        /// 購物車明細
        /// </summary>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> GetItemGroupListold(int TypeID = 1)
        {
            int accID = NEUser.ID;
            List<int> TypeIDList = new List<int>() { 1, 7 };
            List<int> TypeIDList_SingleQty = new List<int>() { 3, 5, 7, 8, 9 };
            TWNewEgg.ECWeb.Controllers.Api.CheckoutCartController CheckoutCartController = new TWNewEgg.ECWeb.Controllers.Api.CheckoutCartController();
            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();
            foreach (var TypeIDtemp in TypeIDList)
            {
                List<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems> CartItems = CheckoutCartController.Get(accID.ToString(), TypeIDtemp, 1).ToList();
                #region CartItems
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.TypeID = TypeIDtemp;
                ItemGroup_View.TypeQty = CartItems.Count;
                if (TypeID == TypeIDtemp)
                {
                    List<int> SellerIDList = CartItems.Select(x => x.ItemSellerID).GroupBy(x => x).Select(x => x.Key).ToList();
                    List<int> ItemIDList = CartItems.Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList();
                    var Sellerresult = Processor.Request<Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase>, Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase>>("SellerServices", "GetSellerWithCountryList", SellerIDList);
                    var ItemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", ItemIDList);
                    var ItemInforesult = Processor.Request<Dictionary<int, TWNewEgg.Models.DomainModels.Item.ItemInfo>, Dictionary<int, TWNewEgg.Models.DomainModels.Item.ItemInfo>>("ItemInfoService", "GetItemInfoList", ItemIDList).results;
                    Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIDList).results;
                    Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>> TrackItem_Viewresults = Processor.Request<Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>>, Dictionary<string, List<TWNewEgg.Models.DomainModels.Track.TrackItem>>>("TrackService", "GetAllTracksItemQty", accID).results;
                    List<TWNewEgg.Models.ViewModels.Track.TrackItem_View> TrackItem_ViewList = new List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>();
                    if (TrackItem_Viewresults != null)
                    {
                        TrackItem_ViewList = TrackItem_Viewresults.Where(x => x.Key == "All").Select(x => x.Value).FirstOrDefault();
                    }
                    var PromotionGiftDetailList = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>>("Service.PromotionGiftService.PromotionGiftRepository", "getItemPromotionGiftListInfo", accID, ItemIDList, promotionGiftStatusTurnON).results;
                    foreach (var item in CartItems)
                    {
                        TWNewEgg.Models.ViewModels.Cart.CartItem_View CartItem_View = new TWNewEgg.Models.ViewModels.Cart.CartItem_View();

                        AutoMapper.Mapper.Map(item, CartItem_View);

                        if (ItemInforesult != null && ItemInforesult.Count > 0)
                        {
                            if (ItemInforesult.Select(x => x.Value.ItemBase.ID == item.ItemID) != null)
                            {
                                AutoMapper.Mapper.Map(ItemInforesult[item.ItemID].ItemBase, CartItem_View.CartItemBase);
                            }
                        }
                        AutoMapper.Mapper.Map(item, CartItem_View.CartItemBase);
                        if (Sellerresult.results != null && Sellerresult.results.Where(x => x.Key == item.ItemSellerID).ToList().Count != 0)
                        {
                            CartItem_View.CountryofOriginID = Sellerresult.results[item.ItemSellerID].CountryID;
                            CartItem_View.CountryofOrigin = Sellerresult.results[item.ItemSellerID].CountryNameCHT;
                        }
                        if (ItemDisplayPriceresult.results != null && ItemDisplayPriceresult.results.Where(x => x.Key == item.ItemID).ToList().Count != 0)
                        {
                            CartItem_View.NTPrice = Convert.ToInt32(Math.Floor((double)ItemDisplayPriceresult.results[item.ItemID].DisplayPrice + 0.5));
                            CartItem_View.CartItemBase.PriceCash = Convert.ToInt32(Math.Floor((double)ItemDisplayPriceresult.results[item.ItemID].DisplayPrice + 0.5));
                            CartItem_View.CartItemBase.MarketPrice = Convert.ToInt32(Math.Floor((double)ItemDisplayPriceresult.results[item.ItemID].ItemCostTW + 0.5));
                            CartItem_View.OriginPrice = ItemDisplayPriceresult.results[item.ItemID].ItemCost;
                            CartItem_View.Taxes.TotalTaxes = ItemDisplayPriceresult.results[item.ItemID].DisplayTax;
                            CartItem_View.Qty = 1;
                            CartItem_View.MaxQty = ItemDisplayPriceresult.results[item.ItemID].MaxNumber;

                            if (TrackItem_ViewList.Where(x => x.ItemID == item.ItemID).FirstOrDefault() != null && TrackItem_ViewList.Where(x => x.ItemID == item.ItemID).FirstOrDefault().ItemQty != 0)
                            {
                                CartItem_View.Qty = TrackItem_ViewList.Where(x => x.ItemID == item.ItemID).FirstOrDefault().ItemQty;
                            }
                            if (CartItem_View.Qty > CartItem_View.MaxQty)
                            {
                                CartItem_View.Qty = CartItem_View.MaxQty;
                            }

                        }
                        try
                        {
                            if (PromotionGiftDetailList != null && PromotionGiftDetailList.Count > 0 && PromotionGiftDetailList.Where(x => x.Key == item.ItemID).ToList().Count != 0)
                            {
                                CartItem_View.GroupDiscount = PromotionGiftDetailList[item.ItemID];
                            }
                            if (itemUrlDictionary != null)
                            {
                                if (itemUrlDictionary.Count > 0 && itemUrlDictionary.Where(x => x.Key == item.ItemID).FirstOrDefault().Value.Count != 0)
                                {
                                    CartItem_View.ImagePath = itemUrlDictionary[item.ItemID].FirstOrDefault().ImageUrl;
                                    if (itemUrlDictionary[item.ItemID].Where(x => x.Size == 60).FirstOrDefault() != null)
                                    {
                                        CartItem_View.ImagePath = itemUrlDictionary[item.ItemID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                                    }
                                    else
                                    {
                                        if (itemUrlDictionary[item.ItemID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                        {
                                            CartItem_View.ImagePath = itemUrlDictionary[item.ItemID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                        }
                                        else
                                        {
                                            if (itemUrlDictionary[item.ItemID].Where(x => x.Size == 300).FirstOrDefault() != null)
                                            {
                                                CartItem_View.ImagePath = itemUrlDictionary[item.ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                                            }
                                        }
                                    }

                                    if (CartItem_View.ImagePath.IndexOf("newegg.com/") >= 0)
                                    {
                                    }
                                    else
                                    {
                                        CartItem_View.ImagePath = ImageUtility.GetImagePath(CartItem_View.ImagePath);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }

                        if (TypeIDList_SingleQty.Contains(TypeIDtemp))
                        {
                            CartItem_View.MaxQty = 1;
                            CartItem_View.Qty = 1;
                        }

                        ItemGroup_View.CartItemList.Add(CartItem_View);
                    }
                    ItemGroup_View.CartPayTypeGroupList = GetCartPayTypeListold(CartItems);

                    // 滿額折優惠
                    List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput> PromotionInputList = new List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput>();
                    foreach (var CartItemListtemp in ItemGroup_View.CartItemList)
                    {
                        TWNewEgg.Models.ViewModels.Redeem.PromotionInput PromotionInputTemp = new TWNewEgg.Models.ViewModels.Redeem.PromotionInput();
                        PromotionInputTemp.ItemID = CartItemListtemp.CartItemBase.ID ?? 0;
                        PromotionInputTemp.Price = CartItemListtemp.NTPrice;
                        PromotionInputTemp.Qty = CartItemListtemp.Qty;
                        PromotionInputTemp.SumPrice = CartItemListtemp.NTPrice * CartItemListtemp.Qty;
                        PromotionInputList.Add(PromotionInputTemp);
                    }
                    //TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository promotionGift = new TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository();
                    //List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = promotionGift.PromotionGiftParsingV2(PromotionInputList, promotionGiftStatusTurnON);
                    List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>>("Service.PromotionGiftService.PromotionGiftRepository", "PromotionGiftParsingV2", accID, PromotionInputList, promotionGiftStatusTurnON).results;
                    List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> PromotionDetail_View = new List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View>();
                    AutoMapper.Mapper.Map(PromotionDetail, PromotionDetail_View);
                    if (PromotionDetail_View != null)
                    {
                        ItemGroup_View.DiscountSum = Convert.ToInt32(Math.Floor((double)PromotionDetail_View.Sum(x => x.ApportionedAmount) + 0.5));
                        foreach (var PromotionDetail_Viewtemp in PromotionDetail_View)
                        {
                            if (PromotionDetail_Viewtemp.ApportionedAmount > 0)
                            {
                                ItemGroup_View.PromotionItemIDList.AddRange(PromotionDetail_Viewtemp.AcceptedItems);
                            }
                        }
                    }
                }
                ItemGroup_ViewList.Add(ItemGroup_View);
                #endregion
            }
            return ItemGroup_ViewList;
        }

        /// <summary>
        /// 購物車明細
        /// </summary>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> GetItemGroupList(List<ShoppingCart_View> ShoppingCart_ViewList, int TypeID = (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車)
        {
            int accID = NEUser.ID;
            List<int> TypeIDList = new List<int>() { (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車, (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車 };
            TWNewEgg.ECWeb.Controllers.Api.CheckoutCartController CheckoutCartController = new TWNewEgg.ECWeb.Controllers.Api.CheckoutCartController();
            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();
            foreach (var TypeIDtemp in TypeIDList)
            {
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.TypeID = TypeIDtemp;
                ItemGroup_View.TypeQty = ShoppingCart_ViewList.Where(x => x.ID == TypeIDtemp).FirstOrDefault().Qty;
                ItemGroup_View.CartItemClass_ViewList = ShoppingCart_ViewList.Where(x => x.ID == TypeIDtemp).FirstOrDefault().CartItemClassList;

                //List<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems> CartItems = CheckoutCartController.Get(accID.ToString(), TypeIDtemp, 1).ToList();
                #region CartItems

                if (TypeID == TypeIDtemp)
                {
                    List<CartItem_View> CartItemList = new List<CartItem_View>();
                    List<int> ItemIDList = new List<int>();
                    // 滿額折優惠
                    List<TWNewEgg.Models.DomainModels.Redeem.PromotionInput> PromotionInputList = new List<TWNewEgg.Models.DomainModels.Redeem.PromotionInput>();

                    foreach (var CartItemClass_ViewListtemp in ItemGroup_View.CartItemClass_ViewList)
                    {
                        CartItemList.AddRange(CartItemClass_ViewListtemp.CartItemList);

                        if (CartItemClass_ViewListtemp.CartItemList.Where(x => (x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart && x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem && x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem)).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList().Count > 0)
                        {
                            ItemIDList.AddRange(CartItemClass_ViewListtemp.CartItemList.Where(x => (x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart && x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem && x.ShowOrder != (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem)).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList());
                        }
                    }

                    ItemGroup_View.CartItemList = CartItemList;

                    Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>> PromotionGiftDetailList = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>>();

                    if (ItemIDList.Count() > 0)
                    {
                        PromotionGiftDetailList = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>>("Service.PromotionGiftService.PromotionGiftRepository", "getItemPromotionGiftListInfo", accID, ItemIDList, promotionGiftStatusTurnON).results;
                    }

                    foreach (var CartItemClass_ViewListtemp in ItemGroup_View.CartItemClass_ViewList)
                    {
                        var ItemIDListStringtemp = "";
                        foreach (var CartItemListtemp in CartItemClass_ViewListtemp.CartItemList)
                        {
                            if (ItemIDListStringtemp != "")
                            {
                                ItemIDListStringtemp = ItemIDListStringtemp + "," + CartItemListtemp.ItemID.ToString();
                            }
                            else
                            {
                                ItemIDListStringtemp = CartItemListtemp.ItemID.ToString();
                            }

                            try
                            {
                                if (PromotionGiftDetailList != null && PromotionGiftDetailList.Count > 0 && PromotionGiftDetailList.Where(x => x.Key == CartItemListtemp.ItemID).ToList().Count != 0)
                                {
                                    CartItemListtemp.GroupDiscount = PromotionGiftDetailList[CartItemListtemp.ItemID];
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            TWNewEgg.Models.DomainModels.Redeem.PromotionInput PromotionInputTemp = new TWNewEgg.Models.DomainModels.Redeem.PromotionInput();
                            PromotionInputTemp.ItemID = CartItemListtemp.ItemID;
                            PromotionInputTemp.Price = CartItemListtemp.NTPrice;
                            PromotionInputTemp.Qty = CartItemListtemp.Qty;
                            PromotionInputTemp.SumPrice = CartItemListtemp.NTPrice * CartItemListtemp.Qty;
                            PromotionInputList.Add(PromotionInputTemp);
                        }
                        CartItemClass_ViewListtemp.ItemIDListString = ItemIDListStringtemp;
                    }
                    ItemGroup_View.CartPayTypeGroupList = GetCartPayTypeList(CartItemList);

                    //TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository promotionGift = new TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository();
                    //List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = promotionGift.PromotionGiftParsingV2(PromotionInputList, promotionGiftStatusTurnON);
                    List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>>("Service.PromotionGiftService.PromotionGiftRepository", "PromotionGiftParsingV2", accID, PromotionInputList, promotionGiftStatusTurnON).results;
                    List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> PromotionDetail_View = new List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View>();
                    AutoMapper.Mapper.Map(PromotionDetail, PromotionDetail_View);
                    if (PromotionDetail_View != null)
                    {
                        ItemGroup_View.DiscountSum = Convert.ToInt32(Math.Floor((double)PromotionDetail_View.Sum(x => x.ApportionedAmount) + 0.5));
                        foreach (var PromotionDetail_Viewtemp in PromotionDetail_View)
                        {
                            if (PromotionDetail_Viewtemp.ApportionedAmount > 0)
                            {
                                ItemGroup_View.PromotionItemIDList.AddRange(PromotionDetail_Viewtemp.AcceptedItems);
                            }
                        }
                    }
                }
                ItemGroup_ViewList.Add(ItemGroup_View);
                #endregion
            }
            return ItemGroup_ViewList;
        }

        /// <summary>
        /// 我的最愛明細
        /// </summary>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> GetWishGroupListold(string TypeName = "wish", int ViewPage = 1, int PageNumber = 10, string OrderBy = "DescCreatDate")
        {
            int accID = NEUser.ID;
            List<string> TypeNameList = new List<string>() { "wish" };
            List<int> TypeIDList_SingleQty = new List<int>() { 3, 5, 7, 8, 9 };
            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();
            List<TWNewEgg.Models.ViewModels.Track.TrackItem_View> TrackItem_ViewList = new List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>();
            Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>> TrackItemresults = Processor.Request<Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>>, Dictionary<string, List<TWNewEgg.Models.DomainModels.Track.TrackItem>>>("TrackService", "GetTracksDetial", accID).results;

            if (TrackItemresults.Where(x => x.Key == "wish").FirstOrDefault().Value.Count > 0)
            {
                TrackItem_ViewList = TrackItemresults.Where(x => x.Key == "wish").FirstOrDefault().Value;
            }

            foreach (var TypeNametemp in TypeNameList)
            {
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.TypeName = TypeNametemp;
                ItemGroup_View.TypeQty = 0;
                ItemGroup_View.ViewPage = 1;
                ItemGroup_View.TotalPage = 0;

                // 計算個數
                if (TrackItemresults.Where(x => x.Key == TypeNametemp).FirstOrDefault().Value.Count > 0)
                {
                    TrackItem_ViewList = TrackItemresults.Where(x => x.Key == TypeNametemp).FirstOrDefault().Value;
                    ItemGroup_View.TypeQty = TrackItem_ViewList.Count;
                    ItemGroup_View.TotalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(TrackItem_ViewList.Count) / PageNumber)); ;
                }

                if (TypeNametemp == TypeName && TrackItem_ViewList.Count > 0)
                {
                    ItemGroup_View.OrderBy = OrderBy;
                    switch (OrderBy)
                    {
                        case "LowPrice":
                            TrackItem_ViewList = TrackItem_ViewList.OrderBy(x => x.ItemPrice).ToList();
                            break;
                        case "HighPrice":
                            TrackItem_ViewList = TrackItem_ViewList.OrderByDescending(x => x.ItemPrice).ToList();
                            break;
                        case "CreatDate":
                            TrackItem_ViewList = TrackItem_ViewList.OrderBy(x => x.CreateDate).ToList();
                            break;
                        case "DescCreatDate":
                            TrackItem_ViewList = TrackItem_ViewList.OrderByDescending(x => x.CreateDate).ToList();
                            break;
                        default:
                            TrackItem_ViewList = TrackItem_ViewList.OrderByDescending(x => x.CreateDate).ToList();
                            break;
                    }
                }

                int TrackItem_ViewListtempCount = 1;
                foreach (var TrackItem_ViewListtemp in TrackItem_ViewList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartItem_View CartItem_View = new TWNewEgg.Models.ViewModels.Cart.CartItem_View();
                    AutoMapper.Mapper.Map(TrackItem_ViewListtemp, CartItem_View);
                    CartItem_View.Page = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(TrackItem_ViewListtempCount) / PageNumber));
                    ItemGroup_View.CartItemList.Add(CartItem_View);
                    TrackItem_ViewListtempCount++;
                }

                List<CartItem_View> ItemGroup_ViewCartItemListtemp = new List<CartItem_View>();
                ItemGroup_ViewCartItemListtemp = ItemGroup_View.CartItemList.Where(x => x.Page == ViewPage).ToList();

                while (ItemGroup_ViewCartItemListtemp.Count == 0 && ViewPage > 1)
                {
                    ViewPage = ViewPage - 1;
                    ItemGroup_ViewCartItemListtemp = ItemGroup_View.CartItemList.Where(x => x.Page == ViewPage).ToList();
                }
                ItemGroup_View.ViewPage = ViewPage;
                ItemGroup_View.CartItemList = ItemGroup_ViewCartItemListtemp;

                List<int> ItemIDList = ItemGroup_View.CartItemList.Select(x => x.ItemID).ToList();
                // 圖片路徑
                Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIDList).results;
                // Item參語活動
                var PromotionGiftDetailList = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>>("Service.PromotionGiftService.PromotionGiftRepository", "getItemPromotionGiftListInfo", accID, ItemIDList, promotionGiftStatusTurnON).results;

                foreach (var CartItemListtemp in ItemGroup_View.CartItemList)
                {
                    int ItemID = CartItemListtemp.ItemID;
                    try
                    {
                        if (PromotionGiftDetailList != null && PromotionGiftDetailList.Count > 0 && PromotionGiftDetailList.Where(x => x.Key == ItemID).ToList().Count != 0)
                        {
                            CartItemListtemp.GroupDiscount = PromotionGiftDetailList[ItemID];
                        }
                        if (itemUrlDictionary.Count > 0 && itemUrlDictionary.Where(x => x.Key == ItemID).FirstOrDefault().Value.Count != 0)
                        {
                            CartItemListtemp.ImagePath = imageDomain + itemUrlDictionary[ItemID].FirstOrDefault().ImageUrl;
                            if (itemUrlDictionary[ItemID].Where(x => x.Size == 60).FirstOrDefault() != null)
                            {
                                CartItemListtemp.ImagePath = itemUrlDictionary[ItemID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                            }
                            else
                            {
                                if (itemUrlDictionary[ItemID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                {
                                    CartItemListtemp.ImagePath = itemUrlDictionary[ItemID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (itemUrlDictionary[ItemID].Where(x => x.Size == 300).FirstOrDefault() != null)
                                    {
                                        CartItemListtemp.ImagePath = itemUrlDictionary[ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                                    }
                                }
                            }

                            if (CartItemListtemp.ImagePath.IndexOf("newegg.com/") >= 0)
                            {
                            }
                            else
                            {
                                CartItemListtemp.ImagePath = ImageUtility.GetImagePath(CartItemListtemp.ImagePath);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
                ItemGroup_View.ShowPageList = CalculationsPage.getShowPages(ItemGroup_View.TotalPage, ViewPage, 3);
                ItemGroup_ViewList.Add(ItemGroup_View);
            }
            return ItemGroup_ViewList;
        }

        /// <summary>
        /// 我的最愛明細
        /// </summary>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> GetWishGroupList(int? TypeID, int ViewPage = 1, int PageNumber = 10, string OrderBy = "DescCreatDate")
        {
            int accID = NEUser.ID;

            List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View> ItemGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.ItemGroup_View>();
            var GetCartAllList = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);

            if (GetCartAllList.error != null)
            {
                return null;
            }

            ShoppingCart_View ShoppingCart_Viewtemp = GetCartAllList.results.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault();
            CartItemClass_View CartItemClass_Viewtemp = ShoppingCart_Viewtemp.CartItemClassList.FirstOrDefault();

            //if (TrackItemresults.Where(x => x.Key == "wish").FirstOrDefault().Value.Count > 0)
            //{
            //    TrackItem_ViewList = TrackItemresults.Where(x => x.Key == "wish").FirstOrDefault().Value;
            //}

            //foreach (var TypeNametemp in TypeNameList)
            //{
            List<CartItem_View> CartItemList = new List<CartItem_View>();
            foreach (var CartItemClass_ViewListtemp in ShoppingCart_Viewtemp.CartItemClassList)
            {
                CartItemList.AddRange(CartItemClass_ViewListtemp.CartItemList);
            }

            if (CartItemList != null)
            {
                // 計算個數
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View();
                ItemGroup_View.TypeName = TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車.ToString();
                ItemGroup_View.TypeQty = CartItemList.Count;
                ItemGroup_View.ViewPage = 1;
                ItemGroup_View.TotalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(CartItemList.Count) / PageNumber));

                // 排序
                ItemGroup_View.OrderBy = OrderBy;
                switch (OrderBy)
                {
                    case "LowPrice":
                        CartItemList = CartItemList.OrderBy(x => x.NTPrice).ToList();
                        break;
                    case "HighPrice":
                        CartItemList = CartItemList.OrderByDescending(x => x.NTPrice).ToList();
                        break;
                    case "CreatDate":
                        CartItemList = CartItemList.OrderBy(x => x.CreateDate).ToList();
                        break;
                    case "DescCreatDate":
                        CartItemList = CartItemList.OrderByDescending(x => x.CreateDate).ToList();
                        break;
                    default:
                        CartItemList = CartItemList.OrderByDescending(x => x.CreateDate).ToList();
                        break;
                }

                int CartItemListtempCount = 1;
                foreach (var CartItemtemp in CartItemList)
                {
                    CartItemtemp.Page = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(CartItemListtempCount) / PageNumber));
                    CartItemListtempCount++;
                }
                CartItemList = CartItemList.Where(x => x.Page == ViewPage).ToList();

                // Item參語活動
                List<int> ItemIDList = CartItemList.Select(x => x.ItemID).ToList();
                var PromotionGiftDetailList = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Cart.GroupDiscount_View>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>>("Service.PromotionGiftService.PromotionGiftRepository", "getItemPromotionGiftListInfo", accID, ItemIDList, promotionGiftStatusTurnON).results;

                foreach (var CartItemListtemp in CartItemList)
                {
                    int ItemID = CartItemListtemp.ItemID;
                    try
                    {
                        if (PromotionGiftDetailList != null && PromotionGiftDetailList.Count > 0 && PromotionGiftDetailList.Where(x => x.Key == ItemID).ToList().Count != 0)
                        {
                            CartItemListtemp.GroupDiscount = PromotionGiftDetailList[ItemID];
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                ItemGroup_View.ViewPage = ViewPage;
                ItemGroup_View.CartItemList = CartItemList;
                TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
                ItemGroup_View.ShowPageList = CalculationsPage.getShowPages(ItemGroup_View.TotalPage, ViewPage, 3);
                ItemGroup_ViewList.Add(ItemGroup_View);
            }
            return ItemGroup_ViewList;
        }


        [HttpPost]
        public JsonResult GetPromotionDetailPrice(CartStep1Data postdata)
        {
            GetPromotionDetail_View GetPromotionDetail_View = new GetPromotionDetail_View();
            // 滿額折優惠
            GetPromotionDetail_View.Price = 0;
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput> PromotionInputList = new List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput>();
            foreach (var postdatatemp in postdata.CartItemDetailList_View)
            {
                TWNewEgg.Models.ViewModels.Redeem.PromotionInput PromotionInputTemp = new TWNewEgg.Models.ViewModels.Redeem.PromotionInput();
                PromotionInputTemp.ItemID = postdatatemp.ItemID;
                PromotionInputTemp.Price = postdatatemp.Price;
                PromotionInputTemp.Qty = postdatatemp.Qty;
                PromotionInputTemp.SumPrice = postdatatemp.Price * postdatatemp.Qty;
                PromotionInputList.Add(PromotionInputTemp);
            }
            //TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository promotionGift = new TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository();
            //List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = promotionGift.PromotionGiftParsingV2(PromotionInputList, promotionGiftStatusTurnON);
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> PromotionDetail = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>>("Service.PromotionGiftService.PromotionGiftRepository", "PromotionGiftParsingV2", NEUser.ID, PromotionInputList, promotionGiftStatusTurnON).results;
            List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> PromotionDetail_View = new List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View>();
            AutoMapper.Mapper.Map(PromotionDetail, PromotionDetail_View);
            if (PromotionDetail_View != null)
            {
                GetPromotionDetail_View.Price = Convert.ToInt32(Math.Floor((double)PromotionDetail_View.Sum(x => x.ApportionedAmount) + 0.5));

                foreach (var PromotionDetail_Viewtemp in PromotionDetail_View)
                {
                    if (PromotionDetail_Viewtemp.ApportionedAmount > 0)
                    {
                        GetPromotionDetail_View.ItemIDList.AddRange(PromotionDetail_Viewtemp.AcceptedItems);
                    }
                }
            }
            return Json(GetPromotionDetail_View, JsonRequestBehavior.AllowGet);
        }

        public List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View> GetCartPayTypeListold(List<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems> CartItemsList)
        {
            string _strOpenPaytypeForTest = "";

            try
            {
                _strOpenPaytypeForTest = (System.Configuration.ConfigurationManager.AppSettings["OpenPaytypeForTest"]).ToString();
            }
            catch (Exception e)
            {

            }

            List<string> _strOpenPaytypeForTestList = _strOpenPaytypeForTest.Split(',').ToList();
            List<int> _intOpenPaytypeForTestList = new List<int>();

            foreach (var temp in _strOpenPaytypeForTestList)
            {
                _intOpenPaytypeForTestList.Add(Convert.ToInt32(temp.Trim()));
            }

            TWSqlDBContext db_before = new TWSqlDBContext();
            List<PayType> DBpayTypeList = db_before.PayType.Where(x => x.Status == (int)PayType.PayTypeStatus.啟動 && _intOpenPaytypeForTestList.Contains(x.ID)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList = db_before.Bank.ToList();
            List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View> CartPayTypeGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View>();

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum)))
            {
                TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View CartPayTypeGroup_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View();
                CartPayTypeGroup_View.CartPayType_View = new List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View>();
                CartPayTypeGroup_View.PayTypeGroupID = item;
                CartPayTypeGroup_View.PayTypeGroupName = Enum.GetName(typeof(TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum), item).ToString();
                if (item == 1)
                {
                    CartPayTypeGroup_View.PayTypeGroupName = "信用卡(一次或分期)";
                }
                CartPayTypeGroup_ViewList.Add(CartPayTypeGroup_View);
            }


            int? PayTypeGroupID = null;

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType)))
            {
                List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View> CartPayTypeList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View>();
                List<PayType> DBpayTypeListtemp = DBpayTypeList.Where(x => x.PayType0rateNum == item).ToList();
                TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_Viewtemp = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();

                switch (item)
                {
                    case ((int)PayType.nPayType.信用卡一次付清):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "一次付清";
                        }
                        break;
                    case ((int)PayType.nPayType.三期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "3期(0利率)";
                            CartPayType_Viewtemp.Installments = 3;
                        }
                        break;
                    case ((int)PayType.nPayType.六期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "6期(0利率)";
                            CartPayType_Viewtemp.Installments = 6;
                        }
                        break;
                    case ((int)PayType.nPayType.十二期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "12期(0利率)";
                            CartPayType_Viewtemp.Installments = 12;
                        }
                        break;
                    case ((int)PayType.nPayType.十八期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "18期(0利率)";
                            CartPayType_Viewtemp.Installments = 18;
                        }
                        break;
                    case ((int)PayType.nPayType.十期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                            {
                                CartPayType_Viewtemp.Name = "10期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                            }
                            else
                            {
                                CartPayType_Viewtemp.Name = "10期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                            }
                            CartPayType_Viewtemp.Installments = 10;
                        }
                        break;
                    case ((int)PayType.nPayType.十二期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                            {
                                CartPayType_Viewtemp.Name = "12期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                            }
                            else
                            {
                                CartPayType_Viewtemp.Name = "12期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                            }
                            CartPayType_Viewtemp.Installments = 12;
                        }
                        break;
                    case ((int)PayType.nPayType.十八期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                            {
                                CartPayType_Viewtemp.Name = "18期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                            }
                            else
                            {
                                CartPayType_Viewtemp.Name = "18期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                            }
                            CartPayType_Viewtemp.Installments = 18;
                        }
                        break;
                    case ((int)PayType.nPayType.二十四期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                            {
                                CartPayType_Viewtemp.Name = "24期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                            }
                            else
                            {
                                CartPayType_Viewtemp.Name = "24期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                            }
                            CartPayType_Viewtemp.Installments = 24;
                        }
                        break;
                    case ((int)PayType.nPayType.信用卡紅利折抵):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡紅利折抵;
                        CartPayType_Viewtemp = null;
                        break;
                    case ((int)PayType.nPayType.貨到付款):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.貨到付款;
                        if (CartItemsList.Where(x => x.ItemDelvType != (int)Item.tradestatus.切貨 && x.ItemDelvType != (int)Item.tradestatus.MKPL寄倉 && x.ItemDelvType != (int)Item.tradestatus.B2c寄倉).ToList().Count == 0)
                        {
                            List<int> ItemIDList = CartItemsList.Select(x => x.ItemID).ToList();
                            List<int> PayTypeIDList = db_before.PayType.Where(x => x.PayType0rateNum == (int)TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType.貨到付款).Select(x => x.ID).ToList();
                            if (db_before.ItemDeliverBlack.Where(x => ItemIDList.Contains(x.ItemID) && PayTypeIDList.Contains(x.PayTypeID)).ToList().Count == 0)
                            {
                                CartPayType_Viewtemp = GetCashonDelivery(DBpayTypeListtemp, BankList);
                                if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                                {
                                    CartPayType_Viewtemp.Name = "貨到付款";
                                }
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.超商付款):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.超商付款;
                        CartPayType_Viewtemp = GetStorePayments(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "超商付款";
                        }
                        break;
                    case ((int)PayType.nPayType.實體ATM):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.實體ATM;
                        CartPayType_Viewtemp = GetEntityATMpayment(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "實體ATM";
                        }
                        break;
                    case ((int)PayType.nPayType.網路ATM):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.網路ATM;
                        CartPayType_Viewtemp = GetWebATMpayment(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "網路ATM";
                        }
                        break;
                    case ((int)PayType.nPayType.歐付寶儲值支付):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.儲值支付;
                        CartPayType_Viewtemp = GetallPaypayment(DBpayTypeListtemp, BankList);
                        if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                        {
                            CartPayType_Viewtemp.Name = "儲值支付";
                        }
                        break;

                }

                if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                {
                    CartPayTypeGroup_ViewList.Where(x => x.PayTypeGroupID == PayTypeGroupID).FirstOrDefault().CartPayType_View.Add(CartPayType_Viewtemp);
                }
                //CartPayTypeGroup_ViewList.Where(x => x.PayTypeGroupID == PayTypeGroupID).FirstOrDefault().CartPayType_View = CartPayTypeList;
            }
            return CartPayTypeGroup_ViewList;
        }

        public List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View> GetCartPayTypeList(List<TWNewEgg.Models.ViewModels.Cart.CartItem_View> CartItemsList)
        {
            string _strOpenPaytypeForTest = "";

            try
            {
                _strOpenPaytypeForTest = (System.Configuration.ConfigurationManager.AppSettings["OpenPaytypeForTest"]).ToString();
            }
            catch (Exception e)
            {

            }

            List<string> _strOpenPaytypeForTestList = _strOpenPaytypeForTest.Split(',').ToList();
            List<int> _intOpenPaytypeForTestList = new List<int>();

            foreach (var temp in _strOpenPaytypeForTestList)
            {
                _intOpenPaytypeForTestList.Add(Convert.ToInt32(temp.Trim()));
            }
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<PayType> DBpayTypeList = db_before.PayType.Where(x => x.Status == (int)PayType.PayTypeStatus.啟動 && _intOpenPaytypeForTestList.Contains(x.ID)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList = db_before.Bank.ToList();
            List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View> CartPayTypeGroup_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View>();

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum)))
            {
                TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View CartPayTypeGroup_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroup_View();
                CartPayTypeGroup_View.CartPayType_View = new List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View>();
                CartPayTypeGroup_View.PayTypeGroupID = item;
                CartPayTypeGroup_View.PayTypeGroupName = Enum.GetName(typeof(TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum), item).ToString();
                if (item == 1)
                {
                    CartPayTypeGroup_View.PayTypeGroupName = "信用卡(一次或分期)";
                }
                CartPayTypeGroup_ViewList.Add(CartPayTypeGroup_View);
            }


            int? PayTypeGroupID = null;
            List<int> ItemIDList = CartItemsList.Select(x => x.ItemID).ToList();

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.DB.TWSQLDB.Models.PayType.nPayType)))
            {
                List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View> CartPayTypeList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayType_View>();
                List<PayType> DBpayTypeListtemp = DBpayTypeList.Where(x => x.PayType0rateNum == item).ToList();
                TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_Viewtemp = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();

                List<int> PayTypeIDList = db_before.PayType.Where(x => x.PayType0rateNum == item).Select(x => x.ID).ToList();
                List<ItemDeliverWhite> ItemDeliverWhiteList = db_before.ItemDeliverWhite.Where(x => ItemIDList.Contains(x.ItemID) && PayTypeIDList.Contains(x.PayTypeID)).ToList();
                List<int> ItemDeliverWhiteIDList = ItemDeliverWhiteList.Select(x => x.ItemID).ToList();
                List<ItemDeliverBlack> ItemDeliverBlackList = db_before.ItemDeliverBlack.Where(x => ItemIDList.Contains(x.ItemID) && PayTypeIDList.Contains(x.PayTypeID)).ToList();
                List<int> ItemDeliverBlackIDList = ItemDeliverBlackList.Select(x => x.ItemID).ToList();
                List<string> ServiceAccountList = ServiceAccount.ToLower().Trim().Replace(" ", "").Split(',').ToList();
                ItemDeliverWhiteIDList.Add(0);

                switch (item)
                {
                    case ((int)PayType.nPayType.信用卡一次付清):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "一次付清";
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.三期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "3期(0利率)";
                                CartPayType_Viewtemp.Installments = 3;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.六期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "6期(0利率)";
                                CartPayType_Viewtemp.Installments = 6;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.九期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "9期(0利率)";
                                CartPayType_Viewtemp.Installments = 9;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "10期(0利率)";
                                CartPayType_Viewtemp.Installments = 10;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十二期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "12期(0利率)";
                                CartPayType_Viewtemp.Installments = 12;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十八期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "18期(0利率)";
                                CartPayType_Viewtemp.Installments = 18;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.二十四期零利率):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "24期(0利率)";
                                CartPayType_Viewtemp.Installments = 24;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.三期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "3期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "3期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 3;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.六期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "6期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "6期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 6;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.九期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "9期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "9期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 9;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "10期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "10期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 10;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十二期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "12期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "12期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 12;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.十八期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "18期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "18期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 18;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.二十四期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "24期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "24期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 24;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.三十期分期):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                if (((CartPayType_Viewtemp.InsRate ?? 0) * 100 % 1) != 0)
                                {
                                    CartPayType_Viewtemp.Name = "30期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0.0") + "%利率)";
                                }
                                else
                                {
                                    CartPayType_Viewtemp.Name = "30期(" + ((CartPayType_Viewtemp.InsRate * 100) ?? 0).ToString("0") + "%利率)";
                                }
                                CartPayType_Viewtemp.Installments = 30;
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.信用卡紅利折抵):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                        //DBpayTypeListtemp = DBpayTypeList.Where(x => x.PayType0rateNum == PayTypeGroupID).ToList();
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetPayTypeBankList(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "信用卡紅利折抵";
                                CartPayType_Viewtemp.Installments = 201;
                                this.getBankBonusesInfo(CartPayType_Viewtemp);
                            }

                        }
                        //PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡紅利折抵;
                        //CartPayType_Viewtemp = null;
                        break;
                    case ((int)PayType.nPayType.貨到付款):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.貨到付款;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            if (CartItemsList.Where(x => x.ItemDelvType == (int)Item.tradestatus.切貨 || x.ItemDelvType == (int)Item.tradestatus.MKPL寄倉 || x.ItemDelvType == (int)Item.tradestatus.B2c寄倉 || ItemDeliverWhiteIDList.Contains(x.ItemID)).ToList().Count == CartItemsList.ToList().Count)
                            {
                                CartPayType_Viewtemp = GetCashonDelivery(DBpayTypeListtemp, BankList);
                                if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                                {
                                    CartPayType_Viewtemp.Name = "貨到付款";
                                }
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.超商付款):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.超商付款;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetStorePayments(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "超商付款";
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.實體ATM):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.實體ATM;
                        if (ItemDeliverBlackIDList.Count == 0 && (CartItemsList.Where(x => x.ItemDelvType == (int)Item.tradestatus.間配 || x.ItemDelvType == (int)Item.tradestatus.三角 || x.ItemDelvType == (int)Item.tradestatus.海外切貨).ToList().Count == 0))
                        {
                            CartPayType_Viewtemp = GetEntityATMpayment(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "實體ATM";
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.網路ATM):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.網路ATM;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            CartPayType_Viewtemp = GetWebATMpayment(DBpayTypeListtemp, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "網路ATM";
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.歐付寶儲值支付):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.儲值支付;
                        if (ItemDeliverBlackIDList.Count == 0)
                        {
                            if (ItemDeliverBlackIDList.Count == 0)
                            {
                                CartPayType_Viewtemp = GetallPaypayment(DBpayTypeListtemp, BankList);
                                if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                                {
                                    CartPayType_Viewtemp.Name = "儲值支付";
                                }
                            }
                        }
                        break;
                    case ((int)PayType.nPayType.電匯):
                        PayTypeGroupID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.電匯;
                        string email = NEUser.Email.ToLower();
                        List<PayType> DBpayTypeListtempforService = db_before.PayType.Where(x => x.PayType0rateNum == item).ToList();
                        if (ServiceAccountList.Where(x => x == email).ToList().Count != 0)
                        {
                            CartPayType_Viewtemp = GetEntityATMpayment(DBpayTypeListtempforService, BankList);
                            if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                            {
                                CartPayType_Viewtemp.Name = "電匯";
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (CartPayType_Viewtemp != null && CartPayType_Viewtemp.PayType0rateNum != null)
                {
                    CartPayTypeGroup_ViewList.Where(x => x.PayTypeGroupID == PayTypeGroupID).FirstOrDefault().CartPayType_View.Add(CartPayType_Viewtemp);
                }
                //CartPayTypeGroup_ViewList.Where(x => x.PayTypeGroupID == PayTypeGroupID).FirstOrDefault().CartPayType_View = CartPayTypeList;
            }
            return CartPayTypeGroup_ViewList;
        }


        /// <summary>
        /// 信用卡判斷
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetPayTypeBankList(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                string BankNamestringList = "";
                string BankIDstringList = "";
                foreach (var DBpayTypetemp in DBpayTypeListtemp)
                {
                    if (DBpayTypetemp.BankIDList != null && DBpayTypetemp.BankIDList.Trim() != "")
                    {
                        if (BankIDstringList != null && BankIDstringList.Trim() != "")
                        {
                            BankIDstringList = BankIDstringList + ",";
                        }
                        BankIDstringList = BankIDstringList + DBpayTypetemp.BankIDList;

                        DBpayTypetemp.BankIDList = DBpayTypetemp.BankIDList + ",";
                    }
                }
                List<string> BankNameList = BankNamestringList.Split('、').GroupBy(x => x).Select(x => x.Key).ToList();
                List<string> BankIDList_string = BankIDstringList.Split(',').GroupBy(x => x).Select(x => x.Key).ToList();
                List<int> BankIDList_int = new List<int>();

                foreach (var BankIDList_stringtemp in BankIDList_string)
                {
                    int BankIDnumber;
                    bool result = Int32.TryParse(BankIDList_stringtemp, out BankIDnumber);
                    if (result)
                    {
                        BankIDList_int.Add(BankIDnumber);
                    }
                }

                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankIDList_inttemp in BankIDList_int)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    TWNewEgg.DB.TWSQLDB.Models.Bank BankTemp = BankList.Where(x => x.ID == BankIDList_inttemp).FirstOrDefault();
                    if (BankTemp != null)
                    {
                        string BankIDList_inttempString = BankIDList_inttemp.ToString() + ",";
                        PayType PayTypetemp = DBpayTypeListtemp.Where(x => x.BankIDList.IndexOf(BankIDList_inttempString) >= 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();

                        CartPayTypeBankIDwithName_View.BankID = BankIDList_inttemp;
                        CartPayTypeBankIDwithName_View.BankName = BankTemp.Referred;
                        CartPayTypeBankIDwithName_View.PaymentVerification = PayTypetemp.Verification ?? 0;

                        CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                    }
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        /// <summary>
        /// 貨到付款
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetCashonDelivery(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                List<int> BankISList = DBpayTypeListtemp.Select(x => x.BankID ?? 0).ToList();
                BankList = BankList.Where(x => BankISList.Contains(x.ID)).ToList();
                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankNametemp in BankList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    CartPayTypeBankIDwithName_View.BankID = BankNametemp.ID;
                    CartPayTypeBankIDwithName_View.BankName = BankNametemp.Name;
                    CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        /// <summary>
        /// 超商付款
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetStorePayments(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                List<int> BankISList = DBpayTypeListtemp.Select(x => x.BankID ?? 0).ToList();
                BankList = BankList.Where(x => BankISList.Contains(x.ID)).ToList();
                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankNametemp in BankList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    CartPayTypeBankIDwithName_View.BankID = BankNametemp.ID;
                    CartPayTypeBankIDwithName_View.BankName = BankNametemp.Name;
                    CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        /// <summary>
        /// 實體ATM
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetEntityATMpayment(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                List<int> BankISList = DBpayTypeListtemp.Select(x => x.BankID ?? 0).ToList();
                BankList = BankList.Where(x => BankISList.Contains(x.ID)).ToList();
                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankNametemp in BankList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    CartPayTypeBankIDwithName_View.BankID = BankNametemp.ID;
                    CartPayTypeBankIDwithName_View.BankName = BankNametemp.Name;
                    CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        /// <summary>
        /// WebATM
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetWebATMpayment(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                List<int> BankISList = DBpayTypeListtemp.Select(x => x.BankID ?? 0).ToList();
                BankList = BankList.Where(x => BankISList.Contains(x.ID)).ToList();
                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankNametemp in BankList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    CartPayTypeBankIDwithName_View.BankID = BankNametemp.ID;
                    CartPayTypeBankIDwithName_View.BankName = BankNametemp.Name;
                    CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        /// <summary>
        /// 歐付寶allPay
        /// </summary>
        /// <param name="DBpayTypeListtemp"></param>
        /// <returns></returns>
        public TWNewEgg.Models.ViewModels.Cart.CartPayType_View GetallPaypayment(List<PayType> DBpayTypeListtemp, List<TWNewEgg.DB.TWSQLDB.Models.Bank> BankList)
        {
            TWNewEgg.Models.ViewModels.Cart.CartPayType_View CartPayType_View = new TWNewEgg.Models.ViewModels.Cart.CartPayType_View();
            if (DBpayTypeListtemp.Count > 0)
            {
                AutoMapper.Mapper.Map(DBpayTypeListtemp.FirstOrDefault(), CartPayType_View);
                List<int> BankISList = DBpayTypeListtemp.Select(x => x.BankID ?? 0).ToList();
                BankList = BankList.Where(x => BankISList.Contains(x.ID)).ToList();
                List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View> CartPayTypeBankIDwithName_ViewList = new List<TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View>();
                foreach (var BankNametemp in BankList)
                {
                    TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View CartPayTypeBankIDwithName_View = new TWNewEgg.Models.ViewModels.Cart.CartPayTypeBankIDwithName_View();
                    CartPayTypeBankIDwithName_View.BankID = BankNametemp.ID;
                    CartPayTypeBankIDwithName_View.BankName = BankNametemp.Name;
                    CartPayTypeBankIDwithName_ViewList.Add(CartPayTypeBankIDwithName_View);
                }
                CartPayType_View.BankIDwithName = CartPayTypeBankIDwithName_ViewList;
            }
            else
            {
                return null;
            }
            return CartPayType_View;
        }

        //        int WhiteListStatus = (int)TWNewEgg.DB.TWSQLDB.Models.PromotionGiftWhiteList.WhiteListStatus.Used;
        //        int TwoForOneOffer = 0;
        //        int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TwoForOneOffer"], out TwoForOneOffer);
        //        List<int> TwoForOneOfferListtemp = db.PromotionGiftBasic.Where(x => x.ID == TwoForOneOffer).Select(x => x.ID).ToList();
        //        List<int> BuytwoListtemp = db.PromotionGiftWhiteList.Where(x => TwoForOneOfferListtemp.Contains(x.PromotionGiftBasicID) && x.Status == WhiteListStatus).Select(x => x.ItemID).ToList();
        //        string BuytwoListString = "";
        //        foreach (var item in BuytwoListtemp)
        //        {
        //            if (BuytwoListString != "")
        //            {
        //                BuytwoListString = BuytwoListString + ",";
        //            }
        //            BuytwoListString = BuytwoListString + "\"" + item.ToString() + "\":\"T\"";
        //        }
        //        ViewBag.BuytwoList = "{" + BuytwoListString + "}";
        //        return View();
        //    }
        //    else
        //    {
        //        //return RedirectToAction("Login", "Account", new { returnUrl = "/Cart/index" });
        //        return RedirectToAction("GuestLogin", "Account", new { returnUrl = "/Cart/Index" });
        //    }
        //    //int accID = NEUser.ID;
        //    int accID = 8;
        //    if (accID > 0)
        //    {
        //        TWNewEgg.Website.ECWeb.Controllers.Api.CheckoutCartController CheckoutCartController = new TWNewEgg.Website.ECWeb.Controllers.Api.CheckoutCartController();
        //        List<CartItems> CartItems = CheckoutCartController.Get("8", 1, 1).ToList();
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "MyAccount", new { returnUrl = "/Cart/Index" });
        //    }
        //} 

        //#endregion

        #region SsCart
        public ActionResult SsCart(string err)
        {
            ViewBag.err = err;
            return View();
        }
        #endregion

        #region Wishshoppingcart
        public ActionResult Wishshoppingcart()
        {
            int accID = NEUser.ID;
            if (accID > 0)
            {
                return View();
            }
            else
            {
                //return RedirectToAction("Login", "Account", new { returnUrl = "/Cart/wishshoppingcart" });
                return RedirectToAction("Login", "MyAccount", new { returnUrl = "/Cart/wishshoppingcart" });
            }
        }
        #endregion

        #region Wishtrackflow
        public ActionResult Wishtrackflow()
        {
            return View();
        }
        #endregion

        #region Wishtrackflowtest
        public ActionResult Wishtrackflowtest()
        {
            return View();
        }
        #endregion

        #region ItemTWtodetail
        public void ItemTWtodetail(string password, int qty = 1)
        {
            if (password == "newegg05")
            {
                string mainwriteStringUS = "";
                string mainwriteStringCH = "";
                string mainpath = Server.MapPath("~/Log/TransMain");

                TWSqlDBContext db = new TWSqlDBContext();

                int[] delvTypeArray = { 0 };
                int delvTypeArrayCount = delvTypeArray.Length;

                for (int i = 0; i < delvTypeArrayCount; i++)
                {
                    Response.Write("<br>DelvTypeArrayCount: " + delvTypeArrayCount + " i : " + i.ToString());
                    Response.Flush();
                    int delvTypetemp = delvTypeArray[i];
                    var itemList = (from I in db.Item.Where(x => x.DelvType == delvTypetemp && x.Status == 0)
                                    join P in db.Product on I.ProductID equals P.ID
                                    join Cate in db.Category on I.CategoryID equals Cate.ID
                                    select new { ItemID = I.ID, ItemProductSellerID = P.SellerID, ItemCategoryID3 = I.CategoryID, ItemCategory3 = Cate.Description, ItemCategoryID2 = Cate.ParentID, ItemCategory2 = "", ItemCategory1 = 0, ItemCategoryID1 = "", DelvType = I.DelvType, SellerID = I.SellerID, ItemName = I.Name, ItemNameEN = P.Name, ProductPrice = P.Cost, ItemPrice = I.PriceCash, ProductID = P.ID, Length = P.Length, Width = P.Width, Height = P.Height, Weight = P.Weight, CCCcode = "切貨", Dutyrate = "切貨", ImportRule = "切貨", ProductTax = P.Tax, TradeTax = P.TradeTax, TWTradeTaxRule = "切貨" }).ToList();

                    List<Category> categoryList = db.Category.ToList();

                    mainwriteStringUS = "";

                    itemList = itemList.OrderBy(x => x.ItemCategoryID3).ToList();

                    TWNewEgg.Website.ECWeb.Service.CartsRepository cartsRepository = new TWNewEgg.Website.ECWeb.Service.CartsRepository();

                    Response.Write("<br>ItemList.Count : " + itemList.Count.ToString());
                    Response.Flush();
                    if (itemList.Count > 0)
                    {
                        int itemp = 0;
                        string path = Server.MapPath("~/content/itemtodetail/DelvType" + delvTypetemp + "/");
                        Response.Write("<br>Path : " + path);
                        Response.Flush();

                        string current = "";
                        for (int fuck = 0; fuck < itemList.Count; fuck++)
                        {
                            try
                            {
                                Response.Write("<br>Count : " + fuck.ToString());
                                Response.Flush();
                                var item = itemList[fuck];

                                int itemID = item.ItemID;
                                int productID = item.ProductID;
                                int itemCategoryID3temp = item.ItemCategoryID3;
                                int itemCategoryID2temp = item.ItemCategoryID2;
                                int itemCategoryID1ID = categoryList.Where(x => x.ID == itemCategoryID2temp).Select(x => x.ParentID).FirstOrDefault();
                                var itemCategory3 = categoryList.Where(x => x.ID == itemCategoryID3temp).Select(x => x.Description).FirstOrDefault();
                                var itemCategory2 = categoryList.Where(x => x.ID == itemCategoryID2temp).Select(x => x.Description).FirstOrDefault();
                                var itemCategory1 = categoryList.Where(x => x.ID == itemCategoryID1ID).Select(x => x.Description).FirstOrDefault();
                                string itemNameENtemp = item.ItemNameEN.Replace(",", " ");
                                string itemNametemp = item.ItemName.Replace(",", " ");
                                string v0 = "";
                                string v1 = "";
                                string v2 = "";
                                string v3 = "";
                                string v4 = "";
                                string v5 = "";
                                string v6 = "";
                                string v7 = "";
                                string v8 = "";

                                string itemCategoryALL = itemCategoryID3temp + "," + itemCategory3 + "," + itemCategoryID2temp + "," + itemCategory2 + "," + itemCategoryID1ID + "," + itemCategory1;

                                string vw = "";
                                try
                                {
                                    if (delvTypetemp == 0 || delvTypetemp == 1 || delvTypetemp == 2 || delvTypetemp == 3 || delvTypetemp == 6)
                                    {
                                        vw = item.Length + "*" + item.Width + "*" + item.Height + "/5000=" + (item.Length * item.Width * item.Height / 5000).ToString() + "," + item.Length + "," + item.Width + "," + item.Height + "," + item.Weight;
                                    }
                                    else
                                    {
                                        vw = item.Length + "*" + item.Width + "*" + item.Height + "/6000=" + (item.Length * item.Width * item.Height / 6000).ToString() + "," + item.Length + "," + item.Width + "," + item.Height + "," + item.Weight;
                                    }
                                }
                                catch
                                {
                                    vw = "NULL" + "*" + "NULL" + "*" + "NULL" + "/6000= NULL" + "," + "NULL" + "," + "NULL" + "," + "NULL" + ",NULL";
                                }

                                current = itemID.ToString() + "," + productID.ToString() + "," + itemNametemp.Replace(",", ".") + "," + item.ItemPrice.ToString() + "," + item.ProductPrice.ToString() + "," + vw + "," + v8 + "," + item.CCCcode.Replace(",", ".") + "," + item.TradeTax.ToString() + "," + item.ProductTax.ToString() + "," + item.ImportRule.Replace(",", ".") + "," + item.TWTradeTaxRule.Replace(",", ".") + "," + v2 + "," + itemCategoryALL + "," + item.SellerID + "," + item.ItemProductSellerID;

                                List<TWNewEgg.ItemService.Models.BuyingItems> buyingItemList = new List<TWNewEgg.ItemService.Models.BuyingItems>();
                                TWNewEgg.ItemService.Models.BuyingItems buyingItemmodel = new TWNewEgg.ItemService.Models.BuyingItems();

                                buyingItemmodel.buyingNumber = 1;
                                buyingItemmodel.buyItemID = item.ItemID;
                                buyingItemmodel.buyItemID_DelvType = item.DelvType;
                                buyingItemmodel.buyItemID_Seller = item.SellerID;
                                buyingItemmodel.buyItemLists = new List<TWNewEgg.ItemService.Models.BuyingItemList>();

                                buyingItemList.Add(buyingItemmodel);
                                //decimal tempWeight = cartsRepository.getTotalWeight(buyingItemslist);
                                Response.Write("<br>GO: " + DateTime.Now.ToString() + " ItemID: " + item.ItemID.ToString());
                                Response.Flush();

                                TWNewEgg.ItemService.Models.ShipTaxService shippingCosts = cartsRepository.ShippingCosts(buyingItemList, "index");
                                Response.Write("<br>Done: " + DateTime.Now.ToString());
                                Response.Flush();

                                //原產地
                                v0 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[0];
                                //台幣售價
                                v1 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[1];
                                //稅賦
                                v2 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[2];
                                //服務費
                                v3 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[3];
                                //關稅
                                v4 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[4];
                                //VAT
                                v5 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[5];
                                //貨物稅
                                v6 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[6];
                                //推廣貿易服務費
                                v7 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[7];
                                //運費
                                v8 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[8];

                                int totalPrice = 0;
                                totalPrice = (((int)(item.ItemPrice)) * qty) + ((int)decimal.Parse(v2 ?? "0") * qty) + ((int)decimal.Parse(v3 ?? "0") * qty) + ((int)decimal.Parse(v8 ?? "0"));

                                mainwriteStringUS = mainwriteStringUS + itemID.ToString() + "," + productID.ToString() + "," + itemNametemp.Replace(",", ".") + "," + item.ItemPrice.ToString() + "," + item.ProductPrice.ToString() + "," + vw + "," + v8 + "," + item.CCCcode.Replace(",", ".") + "," + item.TradeTax.ToString() + "," + item.ProductTax.ToString() + "," + item.ImportRule.Replace(",", ".") + "," + item.TWTradeTaxRule.Replace(",", ".") + "," + v2 + "," + itemCategoryALL + "," + item.SellerID + "," + item.ItemProductSellerID + "," + totalPrice.ToString() + "," + v4.ToString() + "," + v5.ToString() + "," + v6.ToString() + "," + v7.ToString() + Environment.NewLine;
                                itemp++;
                                if (itemp >= 20000)
                                {
                                    Response.Write("<br>寫入檔案...");
                                    Response.Flush();
                                    if (delvTypeArray[i] == 0 || delvTypeArray[i] == 1 || delvTypeArray[i] == 2 || delvTypeArray[i] == 3 || delvTypeArray[i] == 6)
                                    {
                                        LogtoFileWrite(path, mainwriteStringUS, "US");
                                        itemp = 0;
                                        mainwriteStringUS = "";
                                    }
                                    else
                                    {
                                        LogtoFileWrite(path, mainwriteStringUS, "CN");
                                        itemp = 0;
                                        mainwriteStringUS = "";
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                //cartsRepository = new Service.cartsRepository();
                                //GC.Collect();
                                string msg = e.Message;
                                if (e.InnerException != null)
                                {
                                    msg += " " + e.InnerException.Message;
                                }

                                mainwriteStringUS += current.ToString() + ", _建立資料有誤" + Environment.NewLine;

                                Response.Write("<hr/><h2 style=\"color:red;\">Item: " + current.ToString() + "</h2><hr/>");
                                Response.Flush();

                                Response.Write("<br>Exception: " + e.Message);
                                Response.Flush();
                            }
                        }

                        Response.Write("<br>寫入檔案...");
                        Response.Flush();
                        if (delvTypetemp == 0 || delvTypetemp == 1 || delvTypetemp == 2 || delvTypetemp == 3 || delvTypetemp == 4 || delvTypetemp == 6)
                        {
                            LogtoFileWrite(path, mainwriteStringUS, "US");
                        }
                        else
                        {
                            LogtoFileWrite(path, mainwriteStringUS, "CN");
                        }
                    }
                }
            }
        }
        #endregion

        #region Itemtodetail
        public void Itemtodetail(string password, int? categoryID, int? parentID, int? parentsParentID, int? update, int? itemStatus, int qty = 1)
        {
            if (password == "newegg05")
            {
                string mainwriteStringUS = "";
                string mainwriteStringCH = "";
                string mainpath = Server.MapPath("~/Log/TransMain");

                TWSqlDBContext db = new TWSqlDBContext();

                int[] delvTypeArray = { 1, 2, 3, 5, 6 };

                int delvTypeArrayCount = delvTypeArray.Length;

                for (int i = 0; i < delvTypeArrayCount; i++)
                {
                    Response.Write("<br>DelvTypeArrayCount: " + delvTypeArrayCount + " i : " + i.ToString());
                    Response.Flush();
                    int delvTypetemp = delvTypeArray[i];
                    var itemList = (from I in db.Item.Where(x => x.DelvType == delvTypetemp)
                                    join P in db.Product on I.ProductID equals P.ID
                                    join PWS in db.ProductFromWS on P.SellerProductID equals PWS.ItemNumber
                                    join Cate in db.Category on I.CategoryID equals Cate.ID
                                    join Cate2 in db.Category on Cate.ParentID equals Cate2.ID
                                    select new { ItemID = I.ID, ItemStatus = I.Status, ItemProductSellerID = P.SellerID, ItemCategoryID3 = I.CategoryID, ItemCategory3 = Cate.Description, ItemCategoryID2 = Cate.ParentID, ItemCategory2 = "", ItemCategory1 = 0, ItemCategoryID1 = Cate2.ParentID, DelvType = I.DelvType, SellerID = I.SellerID, ItemName = I.Name, ItemNameEN = P.Name, ProductPrice = P.Cost, ItemPrice = I.PriceCash, ProductID = P.ID, Length = P.Length, Width = P.Width, Height = P.Height, Weight = P.Weight, CCCcode = PWS.CCC, Dutyrate = "TTAX.Rate1", ImportRule = "TTAX.ImportRule", ProductTax = P.Tax, TradeTax = P.TradeTax, TWTradeTaxRule = "TTAX.TWTradeTaxRule", Createdate = I.CreateDate, SellerProductID = P.SellerProductID ?? "Null" }).ToList();

                    itemList = itemList.OrderBy(x => x.ItemID).ToList();

                    if (itemStatus != null)
                    {
                        itemList = itemList.Where(x => x.ItemStatus == itemStatus).ToList();
                    }

                    if (categoryID != null)
                    {
                        itemList = itemList.Where(x => x.ItemCategoryID3 == categoryID.Value).ToList();
                    }

                    if (parentID != null)
                    {
                        itemList = itemList.Where(x => x.ItemCategoryID2 == parentID.Value).ToList();
                    }

                    if (parentsParentID != null)
                    {
                        itemList = itemList.Where(x => x.ItemCategoryID1 == parentsParentID.Value).ToList();
                    }

                    List<Category> categoryList = db.Category.ToList();

                    mainwriteStringUS = "";

                    itemList = itemList.OrderBy(x => x.ItemCategoryID3).ToList();

                    TWNewEgg.Website.ECWeb.Service.CartsRepository cartsRepository = new TWNewEgg.Website.ECWeb.Service.CartsRepository();

                    Response.Write("<br>ItemList.Count : " + itemList.Count.ToString());
                    Response.Flush();
                    if (itemList.Count > 0)
                    {
                        int itemp = 0;
                        string path = Server.MapPath("~/content/itemtodetail/DelvType" + delvTypetemp + "/");
                        Response.Write("<br>Path : " + path);
                        Response.Flush();

                        string current = "";
                        for (int fuck = 0; fuck < itemList.Count; fuck++)
                        {
                            try
                            {
                                string item_Dutyrate = "";
                                string item_ImportRule = "";
                                string item_TWTradeTaxRule = "";

                                Response.Write("<br>Count : " + fuck.ToString());
                                Response.Flush();
                                var item = itemList[fuck];

                                string pws_CCCtemp = item.CCCcode;
                                if (pws_CCCtemp != null)
                                {
                                    TWNewEgg.DB.TWSQLDB.Models.TwTradeTax twtradeTaxtemp = db.TwTradeTax.Where(x => x.ID == pws_CCCtemp).FirstOrDefault();

                                    if (twtradeTaxtemp != null)
                                    {
                                        item_Dutyrate = twtradeTaxtemp.Rate1;
                                        item_ImportRule = twtradeTaxtemp.ImportRule;
                                        item_TWTradeTaxRule = twtradeTaxtemp.TWTradeTaxRule;
                                    }
                                }

                                int itemCategoryID3temp = item.ItemCategoryID3;
                                int itemCategoryID2temp = item.ItemCategoryID2;
                                int itemCategoryID1ID = 0;
                                if (categoryList.Where(x => x.ID == itemCategoryID2temp).Select(x => x.ParentID).Count() > 0)
                                {
                                    itemCategoryID1ID = categoryList.Where(x => x.ID == itemCategoryID2temp).Select(x => x.ParentID).FirstOrDefault();
                                }
                                var itemCategory3 = (categoryList.Where(x => x.ID == itemCategoryID3temp).Select(x => x.Description).FirstOrDefault() ?? "Null").Replace(",", " ");
                                var itemCategory2 = (categoryList.Where(x => x.ID == itemCategoryID2temp).Select(x => x.Description).FirstOrDefault() ?? "Null").Replace(",", " ");
                                var itemCategory1 = (categoryList.Where(x => x.ID == itemCategoryID1ID).Select(x => x.Description).FirstOrDefault() ?? "Null").Replace(",", " ");
                                string itemNameENtemp = item.ItemNameEN.Replace(",", " ");
                                string itemNametemp = item.ItemName.Replace(",", " ");
                                string v0 = "0";
                                string v1 = "0";
                                string v2 = "0";
                                string v3 = "0";
                                string v4 = "0";
                                string v5 = "0";
                                string v6 = "0";
                                string v7 = "0";
                                string v8 = "0";

                                int itemID = item.ItemID;
                                int productID = item.ProductID;

                                string itemCategoryALL = itemCategoryID3temp + "," + itemCategory3 + "," + itemCategoryID2temp + "," + itemCategory2 + "," + itemCategoryID1ID + "," + itemCategory1;

                                string vw = "";
                                try
                                {
                                    if (delvTypetemp == 0 || delvTypetemp == 1 || delvTypetemp == 2 || delvTypetemp == 3 || delvTypetemp == 6)
                                    {
                                        vw = item.Length + "*" + item.Width + "*" + item.Height + "/5000=" + (item.Length * item.Width * item.Height / 5000).ToString() + "," + item.Length + "," + item.Width + "," + item.Height + "," + item.Weight;
                                    }
                                    else
                                    {
                                        vw = item.Length + "*" + item.Width + "*" + item.Height + "/6000=" + (item.Length * item.Width * item.Height / 6000).ToString() + "," + item.Length + "," + item.Width + "," + item.Height + "," + item.Weight;
                                    }
                                }
                                catch
                                {
                                    vw = "NULL" + "*" + "NULL" + "*" + "NULL" + "/6000= NULL" + "," + "NULL" + "," + "NULL" + "," + "NULL" + ",NULL";
                                }

                                current = itemID.ToString() + "," + productID.ToString() + "," + itemNametemp.Replace(",", ".") + "," + item.ItemPrice.ToString() + "," + item.ProductPrice.ToString() + "," + vw + "," + v8 + "," + (item.CCCcode ?? "NULL").Replace(",", ".") + "," + (item.TradeTax ?? 0m).ToString() + "," + (item.ProductTax ?? 0m).ToString() + "," + item_ImportRule.Replace(",", ".") + "," + item_TWTradeTaxRule.Replace(",", ".") + "," + v2 + "," + itemCategoryALL + "," + item.SellerID + "," + item.ItemProductSellerID;

                                List<TWNewEgg.ItemService.Models.BuyingItems> buyingItems = new List<TWNewEgg.ItemService.Models.BuyingItems>();
                                TWNewEgg.ItemService.Models.BuyingItems buyingItemmodel = new TWNewEgg.ItemService.Models.BuyingItems();

                                buyingItemmodel.buyingNumber = qty;
                                buyingItemmodel.buyItemID = item.ItemID;
                                buyingItemmodel.buyItemID_DelvType = item.DelvType;
                                buyingItemmodel.buyItemID_Seller = item.SellerID;
                                buyingItemmodel.buyItemLists = new List<TWNewEgg.ItemService.Models.BuyingItemList>();

                                buyingItems.Add(buyingItemmodel);
                                //decimal tempWeight = cartsRepository.getTotalWeight(buyingItemslist);
                                Response.Write("<br>GO: " + DateTime.Now.ToString() + " ItemID: " + item.ItemID.ToString());
                                Response.Flush();

                                TWNewEgg.ItemService.Models.ShipTaxService shippingCosts = new TWNewEgg.ItemService.Models.ShipTaxService();
                                try
                                {
                                    shippingCosts = cartsRepository.ShippingCosts(buyingItems, "index");
                                }
                                catch (Exception e)
                                {

                                }
                                Response.Write("<br>Done: " + DateTime.Now.ToString());
                                Response.Flush();

                                if (shippingCosts.ShippingTaxCost != null && shippingCosts.ShippingTaxCost.Count > 0)
                                {
                                    //原產地
                                    v0 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[0];
                                    //台幣售價
                                    v1 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[1];
                                    //稅賦
                                    v2 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[2];
                                    //服務費
                                    v3 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[3];
                                    //關稅
                                    v4 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[4];
                                    //VAT
                                    v5 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[5];
                                    //貨物稅
                                    v6 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[6];
                                    //推廣貿易服務費
                                    v7 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[7];
                                    //運費
                                    v8 = shippingCosts.ShippingTaxCost.First().Value.First().pricetaxdetail.Split(',')[8];
                                }

                                int totalPrice = 0;
                                totalPrice = (((int)(item.ItemPrice)) * qty) + ((int)decimal.Parse(v2 ?? "0") * qty) + ((int)decimal.Parse(v3 ?? "0") * qty) + ((int)decimal.Parse(v8 ?? "0"));

                                mainwriteStringUS = mainwriteStringUS + itemID.ToString() + "," + productID.ToString() + "," + itemNametemp.Replace(",", ".") + "," + item.ItemPrice.ToString() + "," + item.ProductPrice.ToString() + "," + vw + "," + v8 + "," + (item.CCCcode ?? "NULL").Replace(",", ".") + "," + (item.TradeTax ?? 0m).ToString() + "," + (item.ProductTax ?? 0m).ToString() + "," + item_ImportRule.Replace(",", ".") + "," + item_TWTradeTaxRule.Replace(",", ".") + "," + v2 + "," + itemCategoryALL + "," + item.SellerID + "," + item.ItemProductSellerID + "," + totalPrice.ToString() + "," + v4.ToString() + "," + v5.ToString() + "," + v6.ToString() + "," + v7.ToString() + "," + item.Createdate.ToLongDateString() + "," + (item.ItemStatus.ToString()) + "," + (item.ItemProductSellerID.ToString()) + Environment.NewLine;
                                itemp++;
                                if (itemp >= 10000)
                                {
                                    Response.Write("<br>寫入檔案...");
                                    Response.Flush();
                                    if (delvTypeArray[i] == 0 || delvTypeArray[i] == 1 || delvTypeArray[i] == 2 || delvTypeArray[i] == 3 || delvTypeArray[i] == 6)
                                    {
                                        LogtoFileWrite(path, mainwriteStringUS, "US");
                                        itemp = 0;
                                        mainwriteStringUS = "";
                                    }
                                    else
                                    {
                                        LogtoFileWrite(path, mainwriteStringUS, "CN");
                                        itemp = 0;
                                        mainwriteStringUS = "";
                                    }
                                }

                                if (update != null && update == 1)
                                {
                                    Item itemitem = new Item();
                                    int itemIDtemp = item.ItemID;
                                    itemitem = db.Item.Where(x => x.ID == itemIDtemp).FirstOrDefault();

                                    itemitem.PriceGlobalship = (int)(Math.Floor(Convert.ToDouble(v8) + 0.6));
                                    itemitem.Taxfee = (int)(Math.Floor(Convert.ToDouble(v2) + 0.6));

                                    int tax = (int)(Math.Floor(Convert.ToDouble(v2) + 0.6));
                                    int shippingFee = (int)(Math.Floor(Convert.ToDouble(v8) + 0.6)) + (int)(Math.Floor(Convert.ToDouble(v3) + 0.6));

                                    if (item.DelvType == 1)
                                    {
                                        //itemitem.Finalprice = item.ItemPrice + tax + shippingFee;
                                    }
                                    else if (item.DelvType == 6 || item.DelvType == 5)
                                    {
                                        //itemitem.Finalprice = item.ItemPrice + tax;
                                    }
                                    else if (item.DelvType == 3)
                                    {
                                        //itemitem.Finalprice = item.ItemPrice + shippingFee;
                                    }

                                    if ((itemp / 200) == 0)
                                    {
                                        //db.SaveChanges();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = e.Message;
                                if (e.InnerException != null)
                                {
                                    msg += " " + e.InnerException.Message;
                                }

                                mainwriteStringUS += current.ToString() + ", _建立資料有誤" + Environment.NewLine;
                                Response.Write("<hr/><h2 style=\"color:red;\">Item: " + current.ToString() + "</h2><hr/>");
                                Response.Flush();
                                Response.Write("<br>Exception: " + e.Message);
                                Response.Flush();
                            }

                            //db.SaveChanges();
                        }

                        Response.Write("<br>寫入檔案...");
                        Response.Flush();
                        if (delvTypetemp == 0 || delvTypetemp == 1 || delvTypetemp == 2 || delvTypetemp == 3 || delvTypetemp == 4 || delvTypetemp == 6)
                        {
                            LogtoFileWrite(path, mainwriteStringUS, "US");
                        }
                        else
                        {
                            LogtoFileWrite(path, mainwriteStringUS, "CN");
                        }
                    }
                }
            }
        }
        #endregion

        #region LogtoFileWrite
        public void LogtoFileWrite(string path, string writeStringendtoFile, string country)
        {
            if (country == "US")
            {
                writeStringendtoFile = "item_id,product_id,item_name,台幣售價,當地售價,材積重=長*寬*高/5000,長,寬,高,重量,國際運費(購物車的試算金額),CCC code, Duty rate,ProductTax,ImportRule,TWTradeTaxRule,Duty & Tax(稅賦),ItemCategoryID3,ItemCategoryID3Name,ItemCategoryID2,ItemCategoryID2Name,ItemCategoryID1,ItemCategoryID1Name,ItemSellerID,ProductSellerID,Total,關稅,VAT,貨物稅,推廣貿易服務費" + Environment.NewLine + writeStringendtoFile;
            }
            else
            {
                writeStringendtoFile = "item_id,product_id,item_name,台幣售價,當地售價,材積重=長*寬*高/6000,長,寬,高,重量,國際運費(購物車的試算金額),CCC code, Duty rate,ProductTax,ImportRule,TWTradeTaxRule,Duty & Tax(稅賦),ItemCategoryID3,ItemCategoryID3Name,ItemCategoryID2,ItemCategoryID2Name,ItemCategoryID1,ItemCategoryID1Name,ItemSellerID,ProductSellerID,Total,關稅,VAT,貨物稅,推廣貿易服務費" + Environment.NewLine + writeStringendtoFile;
            }

            string filename = path + string.Format("\\{0:yyyy-MM-dd-HH-mm-ss-ffff}.txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);
        }
        #endregion

        #region WishList
        public ActionResult WishCart()
        {
            return View();
        }
        #endregion

        #region CheckTools
        #region FindBuyingCart
        private List<ShoppingCartItems> FindBuyingCart(string accountID, string postData, string isOverSea)
        {
            string userEmail = NEUser.Email;
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"66\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"62\",\"buyingNumber\":\"2\"}]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"76\",\"buyingNumber\":\"6\"}]}]";
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]}]";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<BuyingItems> buyingItemPostData = new List<BuyingItems>();
            buyingItemPostData = serializer.Deserialize<List<BuyingItems>>(postData); //translate json string to model
            CartsRepository buyingICart = new CartsRepository();
            List<ShoppingCartItems> buyingCarts = new List<ShoppingCartItems>();
            int accID = NEUser.ID;
            bool boolPhase = Int32.TryParse(accountID, out accID);
            if (boolPhase)
            {
                try
                {
                    if (isOverSea == "False")
                    {
                        logger.Info("[userEmail] [" + userEmail + "]" + "GetBuyingCart [isOverSea] " + isOverSea);
                        buyingCarts = buyingICart.GetBuyingCart(accID, 0, buyingItemPostData, "False").ToList(); //Get Complete buying items
                        logger.Info("[userEmail] [" + userEmail + "]" + "[GetBuyingCart] end");
                    }
                    else if (isOverSea == "True")
                    {
                        logger.Info("[userEmail] [" + userEmail + "]" + "GetBuyingCart [isOverSea] " + isOverSea);
                        buyingCarts = buyingICart.GetBuyingCart(accID, 0, buyingItemPostData, "True").ToList(); //Get Complete buying items
                        logger.Info("[userEmail] [" + userEmail + "]" + "[GetBuyingCart] end");
                    }

                    return buyingCarts;
                }
                catch (Exception e)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "FindBuyingCart [ErrorMsg]" + e.ToString());
                    return null;
                }
            }

            return null;
        }
        #endregion

        #region TwoForOneOfferQtyCheck
        /// <summary>
        /// 檢查買一送一商品數量是否為2
        /// </summary>
        /// <param name="buyingItemPostData">購物車商品資訊</param>
        /// <returns>返回執行結果，若無買一送一商品或者具有買一送一商品但檢查結果商品數量皆為2，則返回true，否則返回false</returns>
        public bool TwoForOneOfferQtyCheck(List<BuyingItems> buyingItemPostData)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            string twoForOneOffer = System.Configuration.ConfigurationManager.AppSettings["TwoForOneOffer"];
            int intTwoForOneOffer = 0;
            int.TryParse(twoForOneOffer, out intTwoForOneOffer);
            // 執行結果
            bool execResult = true;
            List<int> twoForOneOfferItemIDList = null;
            // 找出買一送一的活動資訊
            TWNewEgg.Models.ViewModels.Redeem.PromotionGiftBasic promotionGiftBasic = null;
            //promotionGiftBasic = db_before.PromotionGiftBasic.Where(x => x.ID == intTwoForOneOffer && x.StartDate <= DateTime.Now && x.EndDate > DateTime.Now).FirstOrDefault();
            promotionGiftBasic = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftBasic, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic>("Service.PromotionGiftService.PromotionGiftRepository", "GetPromotionGiftBasicByBasicId", intTwoForOneOffer).results;

            // 若活動不存在或不在有效期限直接返回
            if (promotionGiftBasic == null || promotionGiftBasic.StartDate > DateTime.Now || promotionGiftBasic.EndDate < DateTime.Now)
            {
                return execResult;
            }
            // 取得買一送一活動的ID
            int promotionGiftBasicID = promotionGiftBasic.ID;
            // 找出買一送一優惠商品ID
            twoForOneOfferItemIDList = db_before.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == promotionGiftBasicID).Select(x => x.ItemID).ToList();
            List<Item> list2for1OfferItem = db_before.Item.Where(x => twoForOneOfferItemIDList.Contains(x.ID)).ToList();
            // 找出買一送一商品庫存數量
            //PromotionGiftRepository promotionGiftRepository = new PromotionGiftRepository();
            //Dictionary<int, int> sellingQtyList = promotionGiftRepository.GetSellingQtyList(list2for1OfferItem);
            List<int> listNumTemp = list2for1OfferItem.Select(x => x.ID).ToList();
            Dictionary<int, int> sellingQtyList = Processor.Request<Dictionary<int, int>, Dictionary<int, int>>("ItemStockService", "GetSellingQtyByItemList", listNumTemp).results;
            // 找出購買的商品內是否有需要加入檢查買一送一數量的商品
            twoForOneOfferItemIDList.ForEach(x =>
            {
                BuyingItems subBuyingItem = buyingItemPostData.Where(b => b.buyItemID == x).FirstOrDefault();
                if (subBuyingItem != null)
                {
                    var sellingQty = sellingQtyList.Where(s => s.Key == x).FirstOrDefault();
                    // 若購物車中買一送一商品數量不足2或者商品庫存數量不足2，則返回執行失敗訊息
                    if (subBuyingItem.buyingNumber < 2 || sellingQty.Value < 2)
                    {
                        execResult = false;
                    }
                }
            });

            db_before.Dispose();
            return execResult;
        }
        #endregion

        #region GetItemRootCategoryId
        /// <summary>
        /// 取得Item的最高Category類別ID
        /// </summary>
        /// <param name="arg_nCategoryId"></param>
        /// <returns></returns>
        private int GetItemRootCategoryId(int arg_nCategoryId, int arg_nGate, ref List<string> listResult)
        {
            if (arg_nCategoryId <= 0 || arg_nGate >= 20)
            {
                return -1;
            }

            arg_nGate++;

            TWSqlDBContext oDb = null;
            Category objCategory = null;
            int numParentCategoryId = -1;

            oDb = new TWSqlDBContext();
            objCategory = oDb.Category.Where(x => x.ID == arg_nCategoryId).SingleOrDefault();
            oDb.Dispose();
            oDb = null;

            if (objCategory == null)
            {
                return -1;
            }

            numParentCategoryId = objCategory.ParentID;
            objCategory = null;
            if (numParentCategoryId == 0)
            {
                return arg_nCategoryId;
            }
            else
            {
                listResult.Add(numParentCategoryId.ToString());
                return this.GetItemRootCategoryId(numParentCategoryId, arg_nGate, ref listResult);
            }
        } //end getItemRootCategoryId 
        #endregion

        #region checkCredit
        /// <summary>
        /// 判斷消費者選擇的付費方式是否為信用卡付款
        /// </summary>
        /// <param name="paytype">付費方式代號</param>
        /// <returns>如果是信用卡付費:回傳true   如果不是信用卡付費:回傳false</returns>
        public bool checkCredit(int paytype)
        {
            bool exec = false;
            if (paytype == (int)PayType.nPayType.信用卡一次付清
                || paytype == (int)PayType.nPayType.三期零利率
                || paytype == (int)PayType.nPayType.六期零利率
                || paytype == (int)PayType.nPayType.十期零利率
                || paytype == (int)PayType.nPayType.十二期零利率
                || paytype == (int)PayType.nPayType.十八期零利率
                || paytype == (int)PayType.nPayType.二十四期零利率
                || paytype == (int)PayType.nPayType.十期分期
                || paytype == (int)PayType.nPayType.十二期分期
                || paytype == (int)PayType.nPayType.十八期分期
                || paytype == (int)PayType.nPayType.二十四期分期)
            {
                exec = true;
            }
            return exec;
        }
        #endregion

        #region InvoidCheck
        /// <summary>
        /// 驗證統一編號是否是被禁止的
        /// </summary>
        /// <param name="verifyCode">需要驗證的統一編號</param>
        /// <returns>若是被禁止的則返回true，反之則false</returns>
        public bool InvoidCheck(string verifyCode)
        {
            // 驗證統一編號是否是被禁止的布林參數
            bool invoidIsIllegal = false;
            // 若驗證的統一編號為NULL則不驗證
            if (string.IsNullOrEmpty(verifyCode))
            {
                return invoidIsIllegal;
            }
            // 將AppSetting中，被禁止的統一編號撈出並儲存為List
            List<string> invoidList = invoidException.Split(',').ToList();
            // 當需驗證的統一編號不為NULL時，執行驗證
            if (invoidList.Count > 0)
            {
                foreach (string subInvoid in invoidList)
                {
                    // 若被設定為禁止使用的統一編號 等於 驗證的統一編號 則返回的布林參數設定為true
                    if (subInvoid.Trim() == verifyCode.Trim())
                    {
                        invoidIsIllegal = true;
                    }
                }
            }

            return invoidIsIllegal;
        }
        #endregion

        #region SOReSendMail
        /// <summary>
        /// 訂單失敗通知信，金流失敗通知信
        /// </summary>
        /// <param name="needReSendList"></param>
        /// <param name="payFail">當PayFail為F則為金流失敗通知信</param>
        /// <param name="rtnCode">rtncode</param>
        /// <param name="rtnMsg">rtnmsg</param>
        /// <returns>是否成功發送</returns>
        public bool SOReSendMail(List<string> needReSendList, string payFail = "", string rtnCode = "", string rtnMsg = "")
        {
            string userEmail = NEUser.Email;
            try
            {
                logger.Info("Email [" + userEmail + "] SOReSendMail : [start]");
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                // 序列化
                string szJson = jsonSerializer.Serialize(needReSendList);
                logger.Info("Email [" + userEmail + "] needReSendList [" + szJson + "] payFail [" + payFail + "] rtnCode [" + rtnCode + "] rtnMsg [" + rtnMsg + "]");
                TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
                string messageresult = "";
                bool defaultFlag = false;
                ViewBag.NewLinkTitle = NewLinkTitle;
                //logger.Info("Email [" + userEmail + "] SOReSendMail_1");
                List<SalesOrder> salesOrderList = db_before.SalesOrder.Where(x => needReSendList.Contains(x.Code)).Distinct().ToList();
                List<SalesOrderItem> salesOrderItemList = db_before.SalesOrderItem.Where(x => needReSendList.Contains(x.SalesorderCode)).Distinct().ToList();
                List<PurchaseOrder> purchaseOrderList = db_before.PurchaseOrder.Where(x => needReSendList.Contains(x.SalesorderCode)).Distinct().ToList();
                List<SOReSend> salesOrderReSendList = new List<SOReSend>();
                //logger.Info("Email [" + userEmail + "] SOReSendMail_2");
                foreach (SalesOrder row in salesOrderList)
                {
                    SOReSend send = new SOReSend();
                    // 該訂單所在環境 DEV、GQC、PRD
                    send.Environment = "[ " + environment.ToUpper() + " ]";
                    send.Email = row.Email;
                    send.Name = row.Name;
                    if (row.PayType == 999520999)
                    {
                        defaultFlag = true;
                    }
                    SalesOrderItem searchSOItems = salesOrderItemList.Where(x => x.SalesorderCode == row.Code).FirstOrDefault();
                    send.SalesOrderCode = row.Code;
                    send.PurchaseOrderCode = purchaseOrderList.Where(x => x.SalesorderCode == row.Code).Select(x => x.Code).FirstOrDefault();
                    send.ItemID = searchSOItems.ItemID.ToString();
                    Product searchProduct = db_before.Product.Where(x => x.ID == searchSOItems.ProductID).FirstOrDefault();
                    send.ProductID = searchProduct.ID.ToString();
                    send.SellerProductID = searchProduct.SellerProductID;
                    send.ItemName = searchSOItems.Name;
                    send.DelvType = ((Item.tradestatus)row.DelivType).ToString();
                    Bank bankInfo = db_before.Bank.Where(x => x.Code == row.CardBank).FirstOrDefault();
                    string paytype = bankInfo.Name + "(" + db_before.PayType.Where(x => x.PayType0rateNum == row.PayType && x.BankID == bankInfo.ID).Select(x => x.Name).FirstOrDefault() + ")";
                    send.PayType = paytype;
                    send.Status = ((SalesOrder.status)row.Status.Value).ToString();
                    salesOrderReSendList.Add(send);
                }
                if (salesOrderReSendList.Count == 0)
                {
                    return false;
                }
                //logger.Info("Email [" + userEmail + "] SOReSendMail_3");
                ViewBag.SOReSendList = salesOrderReSendList; // 需重新拋單的訂單
                if (payFail != "")
                {
                    ViewBag.SalesOrderGroupID = salesOrderList[0].SalesOrderGroupID;
                    ViewBag.RtnCode = rtnCode;
                    ViewBag.RtnMsg = rtnMsg;
                }
                //logger.Info("Email [" + userEmail + "] SOReSendMail_4");
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "~/Views/Cart/Mail_SOReSend.cshtml");
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    messageresult = sw.GetStringBuilder().ToString();
                }
                logger.Info("Email [" + userEmail + "] SOReSendMail_5 messageresult[" + messageresult + "]");
                string recipient = NoticeMail;
                if (payTypeGatewayTurnON.ToLower() == "on")
                {
                    if (payFail == "")
                    {
                        if (defaultFlag == true)
                        {
                            return SendEmail(messageresult, "[金流為Default]需要重新拋單的訂單[ " + environment.ToUpper() + " ]", recipient);
                        }
                        else
                        {
                            return SendEmail(messageresult, "需要重新拋單的訂單[ " + environment.ToUpper() + " ]", recipient);
                        }
                    }
                    else
                    {
                        if (defaultFlag == true)
                        {
                            return SendEmail(messageresult, "[金流為Default](Issue) 金流失敗 [ " + environment.ToUpper() + " ]", recipient);
                        }
                        else
                        {
                            return SendEmail(messageresult, "(Issue) 金流失敗 [ " + environment.ToUpper() + " ]", recipient);
                        }
                    }
                }
                else
                {
                    if (payFail == "")
                    {
                        return SendEmail(messageresult, "[金流關閉]需要重新拋單的訂單[ " + environment.ToUpper() + " ]", recipient);
                    }
                    else
                    {
                        return SendEmail(messageresult, "[金流關閉](Issue) 金流失敗 [ " + environment.ToUpper() + " ]", recipient);
                    }
                }
                logger.Info("Email [" + userEmail + "] SOReSendMail : [End]");
            }
            catch (Exception ex)
            {
                logger.Info("AllPayCredic:(寄發金流發生錯誤通知信失敗) : [Email] " + userEmail + " 寄發金流發生錯誤通知信失敗 [ErrorMessage] " + ex.ToString());
                return false;
            }
        }
        #endregion

        #region SendEmail
        /// <summary>
        /// 寄發信件用Function
        /// </summary>
        /// <param name="mailMessage">信件內容</param>
        /// <param name="subject">信件主旨</param>
        /// <param name="recipient">收件人</param>
        /// <returns>是否成功寄出信件</returns>
        public bool SendEmail(string mailMessage, string subject, string recipient)
        {
            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收件者
                msg.To.Add(recipient);
                // msg.CC.Add("c@msn.com"); // 副本
                // msg.Bcc.Add("d@msn.com"); // 密件副本
                // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
                msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
                msg.Subject = subject; // 郵件主旨
                msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
                msg.Body = mailMessage; // 郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
                msg.IsBodyHtml = true; // 是否為HTML郵件
                msg.Priority = MailPriority.Normal; // 郵件優先等級
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient mySmtp = new SmtpClient(ECWeb_SMTP, 25);
                // 設定妳的帳號密碼
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                mySmtp.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region HitrustPay
        /// <summary>
        /// 此函式專門處理透過Hitrust SSL Auth(直接授權)的信用卡交易
        /// </summary>
        /// <param name="arg_oHitrustSsl"></param>
        /// <returns></returns>
        //public ActionResult HitrustPay(HitrustSSL arg_oHitrustSsl, HitrustTradeType arg_oHitrastTradeType)
        public TWNewEgg.Models.ViewModels.Cart.CashFlowResult HitrustPay(ref List<InsertSalesOrdersBySellerOutput> argSend, ref InsertSalesOrdersBySellerInput argData, PayType.nPayType argPayTypeMode, InstallmentInfo argSaveInstallmentInfo, int argNumPromotionGiftAmount)
        {
            logger.Info("HitrustPay:(Start) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            HitrustService objHitrustService = null;
            HitrustResult objResult = null;
            MessageOfTradeWithBank objMessage = null;
            TWNewEgg.DB.TWSQLDB.Models.Auth objAuth = null;
            HitrustSSL oHitrustSsl = null;
            decimal numCouponValue = 0;
            decimal numInstallmentInfo = 0;
            TWNewEgg.Models.ViewModels.Cart.CashFlowResult objCashFlowResult = null;//金流交易結果的統一性訊息

            //設定金流交易結果的初始值
            objCashFlowResult = new CashFlowResult();
            objCashFlowResult.TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard;
            objCashFlowResult.Paytype = "Hitrust";

            logger.Info("HitrustPay:(oHitrastTradeType) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            HitrustTradeType oHitrastTradeType = HitrustTradeType.WebATM;
            // 是否啟用Coupon Function
            logger.Info("HitrustPay:(是否啟用Coupon Function) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
            {
                numCouponValue = Convert.ToDecimal(argData.TotalCouponValue);
            }
            else
            {
                numCouponValue = 0;
            }
            // 分期金額設定
            if (argSaveInstallmentInfo != null)
            {
                numInstallmentInfo = argSaveInstallmentInfo.TotalInsRateFees;
            }
            logger.Info("HitrustPay:(Save oHitrustSsl) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            oHitrustSsl = new HitrustSSL();
            oHitrustSsl.ordernumber = argSend.First().salesorder_code;
            oHitrustSsl.orderdesc = "index No: " + Convert.ToString(argSend.First().salesorder_salesordergroupid) + "_" + argSend.First().salesorder_code.Substring(9);
            oHitrustSsl.amount = Convert.ToInt32(argData.pricesum + numInstallmentInfo - numCouponValue - argNumPromotionGiftAmount) * 100;
            oHitrustSsl.ticketno = "";
            oHitrustSsl.pan = argData.salesorder_cardno;
            oHitrustSsl.expiry = argData.salesorder_cardexpire;
            oHitrustSsl.E01 = argData.auth_code_3;
            logger.Info("HitrustPay:(分期期數) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            //分期期數
            oHitrustSsl.E03 = "1"; // 
            switch (argPayTypeMode)
            {
                case PayType.nPayType.網路ATM:
                    oHitrastTradeType = HitrustTradeType.WebATM;
                    break;
                case PayType.nPayType.信用卡一次付清:
                    oHitrastTradeType = HitrustTradeType.CredicOnce;
                    oHitrustSsl.E03 = "1";
                    break;
                case PayType.nPayType.三期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "3";
                    break;
                case PayType.nPayType.六期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "6";
                    break;
                case PayType.nPayType.十期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "010";
                    break;
                case PayType.nPayType.十二期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "12";
                    break;
                case PayType.nPayType.十八期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "018";
                    break;
                case PayType.nPayType.二十四期零利率:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "024";
                    break;
                case PayType.nPayType.十期分期:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "010";
                    break;
                case PayType.nPayType.十二期分期:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "12";
                    break;
                case PayType.nPayType.十八期分期:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "018";
                    break;
                case PayType.nPayType.二十四期分期:
                    oHitrastTradeType = HitrustTradeType.CredicInstallments;
                    oHitrustSsl.E03 = "024";
                    break;
            }

            objHitrustService = new HitrustService();
            logger.Info("HitrustPay:(createCreditSslTransaction) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            objResult = objHitrustService.createCreditSslTransaction(oHitrustSsl, oHitrastTradeType);

            objHitrustService = null;
            logger.Info("HitrustPay:(產生PO & Auth) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            //產生PO & Auth
            objAuth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
            if (objResult.retcode.Equals("00"))
            {
                objAuth.SuccessFlag = "1";
                objCashFlowResult.TradeResult = true;
            }
            else
            {
                objAuth.SuccessFlag = "0";
                objCashFlowResult.TradeResult = false;
                objCashFlowResult.PaytypeReturnCode = Convert.ToString(objResult.retcode);
                objCashFlowResult.PaytypeReturnMsg = "";
                objCashFlowResult.PaytypeAuthCode = Convert.ToString(objResult.authCode);
                objCashFlowResult.SystemMessage = "信用卡付款失敗";
            }

            objAuth.AcqBank = "021";
            objAuth.CustomerID = "Hitrust";
            objAuth.OrderNO = objResult.ordernumber;
            objAuth.AuthCode = objResult.authCode;
            try
            {
                objAuth.AuthDate = Convert.ToDateTime(objResult.orderdate);
            }
            catch
            {
                objAuth.AuthDate = DateTime.Now;
            }

            try
            {
                objAuth.Amount = Convert.ToInt32(objResult.depositamount) / 100;
            }
            catch
            {
                objAuth.Amount = 0;
            }

            try
            {
                objAuth.AuthSN = Convert.ToString(objResult.authRRN);
                objAuth.AmountSelf = Convert.ToInt32(objResult.E06); //分期期數
                objAuth.Bonus = 0;
                objAuth.PriceFirst = Convert.ToInt32(objResult.E07);
                objAuth.PriceOther = Convert.ToInt32(objResult.E08);
                objAuth.RspCode = objResult.retcode;  //授權回應碼
                objAuth.RspMSG = objResult.retcode;   //花旗沒有授權回應訊息
                objAuth.RspOther = "";   //額外資料
                objAuth.CreateUser = "lynn.p";
                objAuth.AgreementID = "";
            }
            catch
            {
                objAuth.AmountSelf = 0; //分期期數
                objAuth.Bonus = 0;
                objAuth.PriceFirst = 0;
                objAuth.PriceOther = 0;
                objAuth.AuthSN = Convert.ToString(objResult.authRRN);
                objAuth.RspCode = objResult.retcode;  //授權回應碼
                objAuth.RspMSG = objResult.retcode;   //花旗沒有授權回應訊息
                objAuth.RspOther = "";   //額外資料
                objAuth.CreateUser = "lynn.p";
                objAuth.AgreementID = "";
            }
            logger.Info("HitrustPay:(產生Auth記錄及PO單) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
            //temp_sw.WriteLine("start create po");
            //產生Auth記錄及PO單
            this.CreatePOAndAuth(argSend[0].salesorder_salesordergroupid.Value.ToString(), objAuth, objResult.retcode, "HITRUST");

            if (objResult.retcode.Equals("00"))
            {
                logger.Info("HitrustPay:(交易成功) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                //交易成功
                return objCashFlowResult;
            }
            else
            {
                logger.Info("HitrustPay:(交易失敗) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                //交易失敗
                TWNewEgg.DB.TWSqlDBContext objTradeDb = null;
                int numSalesOrderGroupId = 0;
                SalesOrder msgSalesOrder = null;
                List<SalesOrder> listMsgSalesOrder = null;
                List<string> payFailList = new List<string>();
                objTradeDb = new DB.TWSqlDBContext();
                msgSalesOrder = objTradeDb.SalesOrder.Where(x => x.Code == objResult.ordernumber).FirstOrDefault();
                if (msgSalesOrder != null)
                {
                    try
                    {
                        numSalesOrderGroupId = Convert.ToInt32(msgSalesOrder.SalesOrderGroupID);
                        listMsgSalesOrder = objTradeDb.SalesOrder.Where(x => x.SalesOrderGroupID == numSalesOrderGroupId).ToList();
                        if (listMsgSalesOrder != null)
                        {
                            foreach (SalesOrder objSubSalesOrder in listMsgSalesOrder)
                            {
                                payFailList.Add(objSubSalesOrder.Code);
                            }

                            listMsgSalesOrder.Clear();
                            listMsgSalesOrder = null;
                        }
                    }
                    catch
                    {
                    }
                }

                objTradeDb.Dispose();

                if (payFailList != null && payFailList.Count > 0)
                {
                    logger.Info("HitrustPay:(寄發金流發生錯誤通知信) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                    SOReSendMail(payFailList, "F", objResult.retcode, ""); // 寄發金流發生錯誤通知信
                }
                logger.Info("HitrustPay:(利用錯誤碼查詢客製化訊息) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                //利用錯誤碼查詢客製化訊息
                TWNewEgg.DB.TWSQLDB.Models.BankCodeMessage objBankCodeMessage = null;
                TWNewEgg.DB.TWSqlDBContext objBankDb = null;
                objBankDb = new DB.TWSqlDBContext();
                if (oHitrustSsl.Equals(HitrustTradeType.WebATM))
                {
                    logger.Info("HitrustPay:(oHitrustSsl.Equals(HitrustTradeType.WebATM)) : [Email] " + argSend[0].salesorder_email + " [Hitrust-WebATM " + argSend[0].salesorder_cardbank + "]");
                    objBankCodeMessage = objBankDb.BankCodeMessage.Where(x => x.BankCode == "021" && x.TradeMethod == (int)BankCodeMessage.TradeMethodOption.WebATM && x.MsgCode == objResult.retcode).FirstOrDefault();
                }
                else
                {
                    logger.Info("HitrustPay:(非oHitrustSsl.Equals(HitrustTradeType.WebATM)) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                    objBankCodeMessage = objBankDb.BankCodeMessage.Where(x => x.BankCode == "021" && x.TradeMethod == (int)BankCodeMessage.TradeMethodOption.CredicCard && x.MsgCode == objResult.retcode).FirstOrDefault();
                }

                objBankDb.Dispose();

                objMessage = new MessageOfTradeWithBank();
                objMessage.AuthAmt = objResult.depositamount.ToString();
                objMessage.AuthCode = objResult.authCode.ToString();
                if (objBankCodeMessage != null)
                {
                    objMessage.ErrDesc = objBankCodeMessage.MsgDescription;
                    objCashFlowResult.SystemMessage = objBankCodeMessage.MsgDescription;
                }
                else
                {
                    logger.Info("HitrustPay:(請詢問銀行客服失敗代碼) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                    objMessage.ErrDesc = "請詢問銀行客服失敗代碼";
                    objCashFlowResult.SystemMessage = "付款失敗，請詢問銀行客服失敗代碼";
                }

                objMessage.ErrCode = objResult.retcode.ToString();
                objMessage.Fee = "0";
                objMessage.PayerLastPin4Code = "";  //花旗不回傳末4碼
                try
                {
                    objMessage.OrderNumber = objResult.ordernumber;
                }
                catch
                {
                    objMessage.OrderNumber = "";
                }
                logger.Info("HitrustPay:(View('Chinatrust_ReturnMessage', objMessage)前) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "期分期" + argSend[0].salesorder_cardbank + "]");
                return objCashFlowResult;
            }

            return objCashFlowResult;
        } //end HitrustPay 
        #endregion

        #region ChinatrustPay
        /// <summary>
        /// 此函式為中國信託WebAtm及信用卡使用
        /// </summary>
        /// <param name="OrderNo">訂單編號</param>
        /// <param name="AuthAmt">訂單總金額</param>
        /// <param name="AuthResURL">完成後導向網址</param>
        /// <param name="BillshortDesc">訂單簡述(50字位元內), 可空白</param>
        /// <param name="txType">交易方式:Credic Card, WebATM...etc</param>
        /// <param name="AutoCap">自動請款, 預設值為0(非信用卡交易請給空白)</param>
        /// <param name="ProdCode">紅利積點產品代碼</param>
        /// <param name="NumberOfPay">信用卡分期數</param>
        /// <returns></returns>
        public ActionResult ChinatrustPay(string OrderNo, string AuthAmt, string BillshortDesc, Chinatrust_txType txType, string AutoCap, string ProdCode, string NumberOfPay, DateTime OrderDateTime)
        {
            //檢查參數
            if (OrderNo == null || AuthAmt == null)
            {
                return View();
            }

            try
            {
                Convert.ToInt32(AuthAmt);
            }
            catch
            {
                return View();
            }

            Dictionary<string, string> objDictResult = null;
            string strURLResEnc = "";
            Chinatrust objChinatrust = null;
            string strServerUrl = System.Configuration.ConfigurationManager.AppSettings.Get("ChinatrustFeedback");

            objDictResult = new Dictionary<string, string>();
            objChinatrust = new Chinatrust();

            objDictResult = new Dictionary<string, string>();
            if (txType.Equals(Chinatrust_txType.WebATM))
            {
                //strURLResEnc = oChinatrust.getURLResEncOfWebAtm(OrderNo, AuthAmt, "http://10.16.131.41/PayType/Chinatrust_ReturnMessage", BillshortDesc);
                strURLResEnc = objChinatrust.getURLResEncOfWebAtm(OrderNo, AuthAmt, strServerUrl + "/PayType/Chinatrust_ReturnMessage", BillshortDesc, OrderDateTime);
                if (System.Configuration.ConfigurationManager.AppSettings.Get("AllPayLogFunction").ToUpper().Equals("ON"))
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/AllPayLog")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/AllPayLog"));
                    }

                    StreamWriter sw = new StreamWriter(Server.MapPath("~/AllPayLog") + "\\" + OrderNo + ".txt", false);
                    sw.Write(strURLResEnc);
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }

                objDictResult.Add("merID", objChinatrust.WebAtm_merID);
                objDictResult.Add("ActionLink", objChinatrust.WebAtmUrl);
            }
            else
            {
                //strURLResEnc = oChinatrust.getURLResEncOfCredicCard(OrderNo, AuthAmt, "http://10.16.131.41/PayType/Chinatrust_ReturnMessage", BillshortDesc, AutoCap, ProdCode, NumberOfPay, txType);
                strURLResEnc = objChinatrust.getURLResEncOfCredicCard(OrderNo, AuthAmt, strServerUrl + "/cart/Chinatrust_CredicReturnMessage", BillshortDesc, AutoCap, ProdCode, NumberOfPay, txType);
                if (System.Configuration.ConfigurationManager.AppSettings.Get("AllPayLogFunction").ToUpper().Equals("ON"))
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/AllPayLog")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/AllPayLog"));
                    }

                    StreamWriter sw = new StreamWriter(Server.MapPath("~/AllPayLog") + "\\" + OrderNo + ".txt", false);
                    sw.Write(strURLResEnc);
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }

                if (txType.Equals(Chinatrust_txType.Credic_Normal) || txType.Equals(Chinatrust_txType.Credic_BonusNormal))
                {
                    objDictResult.Add("merID", objChinatrust.CredicNormal_merID);
                }
                else if (txType.Equals(Chinatrust_txType.Credic_Installments) || txType.Equals(Chinatrust_txType.Credic_BonusInstallments))
                {
                    objDictResult.Add("merID", objChinatrust.CredicIntallments_merID);
                }

                objDictResult.Add("ActionLink", objChinatrust.CredicUrl);
            }

            objDictResult.Add("URLEnc", strURLResEnc);

            objChinatrust = null;

            return View("ChinatrustPay", objDictResult);
            //return RedirectToAction("ChinatrustPay", oDictResult);
        } //end ChinatrustPay
        #endregion

        #region AllPayCredic
        /// <summary>
        /// 此函式專門處理透過WebService的AllPay的信用卡交易 
        /// </summary>
        /// <param name="arg_AllPayCreditAio"></param>
        public TWNewEgg.Models.ViewModels.Cart.CashFlowResult AllPayCredic(ref List<InsertSalesOrdersBySellerOutput> argSend, ref InsertSalesOrdersBySellerInput argData, PayType.nPayType argPayTypeMode, InstallmentInfo argSaveInstallmentInfo, int argNumPromotionGiftAmount)
        //public ActionResult AllPayCredic(ref AllPayCredicAio arg_AllPayCreditAio)
        {
            logger.Info("AllPayCredic:(Start) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
            string strEncrypt = "";
            string strDecrypt = "";
            decimal numCouponValue = 0;
            decimal numInstallmentInfo = 0;
            TWNewEgg.Models.ViewModels.Cart.CashFlowResult objCashFlowResult = null;//金流交易結果的統一性訊息

            //設定金流交易結果的初始值
            objCashFlowResult = new CashFlowResult();
            objCashFlowResult.TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard;
            objCashFlowResult.Paytype = "Allpay";

            logger.Info("AllPayCredic:(是否啟用Coupon Function) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
            // 是否啟用Coupon Function
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
            {
                numCouponValue = Convert.ToDecimal(argData.TotalCouponValue);
            }
            else
            {
                numCouponValue = 0;
            }
            logger.Info("AllPayCredic:(分期金額設定) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
            // 分期金額設定
            if (argSaveInstallmentInfo != null)
            {
                numInstallmentInfo = argSaveInstallmentInfo.TotalInsRateFees;
            }
            logger.Info("AllPayCredic:(Save AllPayCredicAio) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
            AllPayCredicAio oCredicAio = new AllPayCredicAio();
            //oCredicAio.MerchantTradeNo = send.First().salesorder_code.ToString();
            oCredicAio.MerchantTradeNo = Convert.ToString(argSend.First().salesorder_salesordergroupid);   //歐付寶為了對帳方便,使用CartNo
            oCredicAio.MerchantTradeDate = String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);
            oCredicAio.TotalAmount = Convert.ToInt32(argData.pricesum + numInstallmentInfo - numCouponValue - argNumPromotionGiftAmount);
            oCredicAio.TradeDesc = "indexNo: " + Convert.ToString(argSend.First().salesorder_salesordergroupid) + "_" + argSend.First().salesorder_code.Substring(9);
            //oCredicAio.CardNo = salesorder_cardno;
            oCredicAio.CardNo = argData.salesorder_cardno;
            oCredicAio.CardValidMM = argData.salesorder_cardexpire.Substring(2, 2);
            oCredicAio.CardValidYY = argData.salesorder_cardexpire.Substring(0, 2);
            oCredicAio.CardCVV2 = argData.auth_code_3;
            //期數
            oCredicAio.Installment = 0;
            switch (argPayTypeMode)
            {
                case PayType.nPayType.信用卡一次付清:
                    oCredicAio.Installment = 0;
                    break;
                case PayType.nPayType.三期零利率:
                    oCredicAio.Installment = 3;
                    break;
                case PayType.nPayType.六期零利率:
                    oCredicAio.Installment = 6;
                    break;
                case PayType.nPayType.十期零利率:
                    oCredicAio.Installment = 10;
                    break;
                case PayType.nPayType.十二期零利率:
                    oCredicAio.Installment = 12;
                    break;
                case PayType.nPayType.十八期零利率:
                    oCredicAio.Installment = 18;
                    break;
                case PayType.nPayType.二十四期零利率:
                    oCredicAio.Installment = 24;
                    break;
                case PayType.nPayType.十期分期:
                    oCredicAio.Installment = 10;
                    break;
                case PayType.nPayType.十二期分期:
                    oCredicAio.Installment = 12;
                    break;
                case PayType.nPayType.十八期分期:
                    oCredicAio.Installment = 18;
                    break;
                case PayType.nPayType.二十四期分期:
                    oCredicAio.Installment = 24;
                    break;
            }
            oCredicAio.ThreeD = 0;
            oCredicAio.Enn = "";
            oCredicAio.BankOnly = "";
            oCredicAio.Redeem = "";
            oCredicAio.PhoneNumber = "";
            oCredicAio.AddMember = "0";
            oCredicAio.CName = "";
            oCredicAio.Email = "";
            oCredicAio.Remark = "";
            logger.Info("AllPayCredic:(Save AllPayCredicAio 1) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
            try
            {
                //AllPay.Service.Credic. oCtrDo = null;
                TWNewEgg.ECWeb.AllPayCredic.creditcard objCtrDo = null;
                strEncrypt = oCredicAio.getEncryptInput();

                if (System.Configuration.ConfigurationManager.AppSettings.Get("AllPayLogFunction").ToUpper().Equals("ON"))
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/AllPayLog")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/AllPayLog"));
                    }

                    StreamWriter sw = new StreamWriter(Server.MapPath("~/AllPayLog") + "\\" + oCredicAio.MerchantTradeNo + ".txt", false);
                    sw.Write(strEncrypt);
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
                logger.Info("AllPayCredic:(Save AllPayCredicAio 2) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                objCtrDo = new TWNewEgg.ECWeb.AllPayCredic.creditcard();
                strDecrypt = objCtrDo.CreateTrade(Convert.ToInt32(oCredicAio.MerchantID), strEncrypt);
                logger.Info("AllPayCredic:(Save AllPayCredicAio 3) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                oCredicAio.Decrypt(strDecrypt);

                TWNewEgg.DB.TWSQLDB.Models.Auth objAuth = null;

                //僅RtnCode為1時表示付款成功,其餘皆為失敗
                objAuth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
                if (oCredicAio.RtnCode == 1)
                {
                    objAuth.SuccessFlag = "1";
                    objCashFlowResult.TradeResult = true;
                }
                else
                {
                    objAuth.SuccessFlag = "0";
                    objCashFlowResult.TradeResult = false;
                    objCashFlowResult.PaytypeReturnCode = Convert.ToString(oCredicAio.RtnCode);
                    objCashFlowResult.PaytypeReturnMsg = oCredicAio.RtnMsg;
                    objCashFlowResult.PaytypeAuthCode = Convert.ToString(oCredicAio.auth_code);
                    objCashFlowResult.SystemMessage = "信用卡付款失敗";
                }

                objAuth.AcqBank = "10006";

                objAuth.CustomerID = "AllPay";
                objAuth.OrderNO = oCredicAio.MerchantTradeNo;
                //oAuth.AuthCode = arg_AllPayCreditAio.RtnCode.ToString();
                objAuth.AuthCode = oCredicAio.auth_code;
                objAuth.AuthDate = DateTime.Now;
                try
                {
                    if (oCredicAio.TotalAmount > 0)
                    {
                        objAuth.Amount = Convert.ToInt32(oCredicAio.TotalAmount);
                    }
                    else
                    {
                        objAuth.Amount = Convert.ToInt32(oCredicAio.amount);
                    }
                }
                catch
                {
                    objAuth.Amount = 0;
                }
                logger.Info("AllPayCredic:(Save AllPayCredicAio 4) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                objAuth.AmountSelf = oCredicAio.stage;
                objAuth.Bonus = 0;
                objAuth.PriceFirst = oCredicAio.stast;  //因抓不到首期金額, 先暫設為0
                objAuth.PriceOther = oCredicAio.staed;  //因抓不到分期金額, 先暫設為0
                objAuth.AuthSN = oCredicAio.TradeNo;
                objAuth.RspCode = oCredicAio.RtnCode.ToString();  //授權回應碼
                objAuth.RspMSG = oCredicAio.RtnMsg;   //授權回應訊息
                objAuth.RspOther = "";   //額外資料
                objAuth.CreateUser = "lynn.p";
                objAuth.AgreementID = "";

                //temp_sw.WriteLine("start create po");
                //產生Auth記錄及PO單(若匯款成功才會產生PO單)
                logger.Info("AllPayCredic:(產生Auth記錄及PO單(若匯款成功才會產生PO單)) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                this.CreatePOAndAuth(oCredicAio.MerchantTradeNo, objAuth, oCredicAio.RtnCode.ToString(), "ALLPAY");
                logger.Info("AllPayCredic:(oCredicAio.RtnCode.Equals(1)) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                logger.Info("AllPayCredic:oCredicAio.MerchantTradeNo [" + oCredicAio.MerchantTradeNo + "]");
                if (oCredicAio.RtnCode.Equals(1))
                {
                    logger.Info("AllPayCredic:(成功的畫面) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    //成功的畫面
                    //return Redirect("/PayType/Results?orderNumber=" + arg_AllPayCreditAio.MerchantTradeNo);
                    return objCashFlowResult;
                }
                else
                {
                    logger.Info("AllPayCredic:(失敗的頁面) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    //失敗的頁面
                    TWSqlDBContext db_before = new TWSqlDBContext();
                    int salesOrderGroupID = Convert.ToInt32(oCredicAio.MerchantTradeNo);
                    List<string> payFailList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).Select(x => x.Code).ToList();
                    logger.Info("AllPayCredic:(失敗的頁面1) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    if (payFailList != null && payFailList.Count > 0)
                    {
                        SOReSendMail(payFailList, "F", oCredicAio.RtnCode.ToString(), oCredicAio.RtnMsg); // 寄發金流發生錯誤通知信
                    }
                    logger.Info("AllPayCredic:(失敗的頁面2) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    //利用錯誤碼查詢客製化訊息
                    TWNewEgg.DB.TWSQLDB.Models.BankCodeMessage objBankCodeMessage = null;
                    TWNewEgg.DB.TWSqlDBContext objBankDb = null;
                    string strRtnCode = oCredicAio.RtnCode.ToString();
                    objBankDb = new DB.TWSqlDBContext();
                    objBankCodeMessage = objBankDb.BankCodeMessage.Where(x => x.BankCode == "10006" && x.TradeMethod == (int)BankCodeMessage.TradeMethodOption.CredicCard && x.MsgCode == strRtnCode).FirstOrDefault();
                    objBankDb.Dispose();
                    logger.Info("AllPayCredic:(失敗的頁面3) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    MessageOfTradeWithBank objMessage = new MessageOfTradeWithBank();
                    objMessage.AuthAmt = oCredicAio.amount.ToString();
                    objMessage.AuthCode = oCredicAio.RtnCode.ToString();
                    if (objBankCodeMessage != null)
                    {
                        objMessage.ErrDesc = objBankCodeMessage.MsgDescription;
                        objCashFlowResult.SystemMessage = objBankCodeMessage.MsgDescription;
                    }
                    else
                    {
                        objMessage.ErrDesc = oCredicAio.RtnMsg;
                    }
                    logger.Info("AllPayCredic:(失敗的頁面4) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    objMessage.ErrCode = oCredicAio.RtnCode.ToString();
                    objMessage.Fee = "0";
                    objMessage.PayerLastPin4Code = oCredicAio.card4no;
                    objMessage.OrderNumber = oCredicAio.MerchantTradeNo;
                    logger.Info("AllPayCredic:(失敗的頁面5) : [Email] " + argSend[0].salesorder_email + " [" + argSend[0].salesorder_paytype + "分期" + argSend[0].salesorder_cardbank + "]");
                    return objCashFlowResult;
                }
            }
            catch (Exception e)
            {
                TWSqlDBContext db_before = new TWSqlDBContext();
                int salesOrderGroupID = Convert.ToInt32(oCredicAio.MerchantTradeNo);
                List<string> payFailList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).Select(x => x.Code).ToList();
                if (payFailList != null && payFailList.Count > 0)
                {
                    SOReSendMail(payFailList, "F"); // 寄發金流發生錯誤通知信
                }

                logger.Info("AllPayCredic : " + e.Message + "_:_" + e.StackTrace);
                InternalSendMail.Service.MailSender objMailSender = null;
                objMailSender = new InternalSendMail.Service.MailSender();
                objMailSender.SendMail("Allpay Credic 連線錯誤<br>" + e.InnerException, "lynn.y.yeh@newegg.com", "", "Allpay Credic 連線錯誤", null, null);
                objMailSender = null;
                objCashFlowResult.SystemMessage = "信用卡收單行連線中斷，我們已通知客服人員，將儘快回復服務，造成您的不便敬請見諒！";
                return objCashFlowResult;
                //return RedirectToAction("index", "cart", new { message = "信用卡收單行連線中斷，我們已通知客服人員，將儘快回復服務，造成您的不便敬請見諒！" });
                //return RedirectToAction("index", "cart", new { message = "AllPayCredic error" });
            }
        } //end AllPayCredic 
        #endregion

        /// <summary>
        /// 產生PO單及存入Auth記錄
        /// </summary>
        /// <param name="ordernumber">訂單編號</param>
        /// <param name="arg_oAuth">含銀行回傳的授權資訊的授權物件</param>
        private void CreatePOAndAuth(string arg_strOrderNumber, TWNewEgg.DB.TWSQLDB.Models.Auth arg_oAuth, string arg_strErrCode, string arg_strPlat)
        {
            AfterPayService afterService = new AfterPayService();
            afterService.CreatePOAndAuth(arg_strOrderNumber, arg_oAuth, arg_strErrCode, arg_strPlat);
        }

        #region Mail_WebMessage
        /// <summary>
        /// 訂購成功通知信-ATM、刷卡
        /// </summary>
        /// <param name="salesOredrCode"></param>
        public void Mail_WebMessage(string salesOredrCode)
        {
            TWSqlDBContext db = new TWSqlDBContext();
            string path = Server.MapPath("~/Log/Mail/");
            ViewBag.NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
            string messageresult = "";
            // salesorders 除國際運費及服務費的訂單
            string strSalesOrderCode = salesOredrCode;
            SalesOrder salesorder = (from p in db.SalesOrder where p.Code == strSalesOrderCode select p).FirstOrDefault();
            ViewBag.salesorder_name = salesorder.Name;
            ViewBag.salesorder_createdate = salesorder.CreateDate;
            int salesOrderGroupId = (int)salesorder.SalesOrderGroupID;
            List<SalesOrder> salesOrderList = (from p in db.SalesOrder where p.SalesOrderGroupID == salesOrderGroupId select p).ToList();
            string salesOrderList_SOCode = "";
            foreach (var so in salesOrderList)
            {
                if (so.Note != "國際運費" && so.Note != "服務費")
                {
                    salesOrderList_SOCode += so.Code + ",";
                }
            }

            salesOrderList_SOCode = salesOrderList_SOCode.Substring(0, salesOrderList_SOCode.Length - 1);
            ViewBag.SalesOrderCodes = salesOrderList_SOCode;
            // salesorders 除國際運費及服務費的訂單
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_Success_WebATMandCard");

                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                messageresult = sw.GetStringBuilder().ToString();
            }

            LogtoFileWrite(path, "Messageresult", messageresult);

            string subject = salesorder.Name + " 您好，Newegg的購物清單-訂購成功通知信";
            string recipient = salesorder.Email;

            SendEmail(messageresult, subject, recipient);
        }
        #endregion

        #region InsertSupplyShippingCharge
        /// <summary>
        /// 將Product中的SupplyShippingCharge insert 到 SalesOrderItem的SupplyShippingCharge欄位中
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <returns>成功執行則返回true, 否則false</returns>
        public bool InsertSupplyShippingCharge(int salesOrderGroupID)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            bool boolExec = false;
            List<string> salesOrderCodeList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).ToList().Select(x => x.Code).ToList();
            List<SalesOrderItem> salesOrderItemList = db_before.SalesOrderItem.Where(x => salesOrderCodeList.Contains(x.SalesorderCode)).ToList();
            List<int> itemIDs = salesOrderItemList.Select(x => x.ItemID).Distinct().ToList();
            List<Item> items = db_before.Item.Where(x => itemIDs.Contains(x.ID)).ToList();
            List<int> productIDs = items.Select(x => x.ProductID).ToList();
            List<Product> productList = db_before.Product.Where(x => productIDs.Contains(x.ID)).ToList();
            //  將Product中的SupplyShippingCharge insert 到 SalesOrderItem的SupplyShippingCharge欄位中
            salesOrderItemList.ForEach(x =>
            {
                Item item = items.Where(i => i.ID == x.ItemID).FirstOrDefault();
                Product product = productList.Where(p => p.ID == item.ProductID).FirstOrDefault();
                x.SupplyShippingCharge = product.SupplyShippingCharge;
            });

            try
            {
                db_before.SaveChanges();
                boolExec = true;
            }
            catch
            {
                boolExec = false;
            }

            return boolExec;
        }
        #endregion

        #region CheckCoupon
        /// <summary>
        /// 此function 用於驗證User使用的Coupon是否合法
        /// </summary>
        /// <param name="arg_listBuyItem">購買的產品項</param>
        /// <param name="arg_listUsedCoupon">使用的Coupon</param>
        /// <param name="arg_strAccountId">User帳號</param>
        /// <param name="arg_nShippingExpense">ShippingExpense</param>
        /// <returns>true:驗證成功; false:驗證失敗</returns>
        public bool CheckCoupon(List<UsedCoupon> arg_listBuyItem, List<UsedCoupon> arg_listUsedCoupon, string arg_strAccountId, decimal arg_nTotalCouponValue)
        {
            UsedCoupon objBuyItem = null;
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listUserActiveCoupon = null;
            TWNewEgg.Models.ViewModels.Redeem.Coupon objCoupon = null;
            bool boolErrorCoupon = false;
            List<string> listItemCategories = null;
            List<Item> listItem = null;
            Item objItem = null;
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            bool boolCheck = false;
            decimal numTempTotalCouponValue = 0;
            decimal numItemPrice = 0;
            List<int> ary_nItemId = null;

            if (arg_listUsedCoupon != null && arg_listUsedCoupon.Count > 0)
            {
                //取得該User有效的Coupon
                //listUserActiveCoupon = objCouponService.getActiveCouponListByAccount(arg_strAccountId);
                listUserActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", arg_strAccountId).results;

                //比對Coupon使用總數
                if (arg_listUsedCoupon.Sum(x => x.Coupons.Count) > listUserActiveCoupon.Count)
                {
                    listUserActiveCoupon = null;
                    boolErrorCoupon = true;
                    return !boolErrorCoupon;
                }

                if (!boolErrorCoupon)
                {
                    oDb = new DB.TWSqlDBContext();
                    ary_nItemId = arg_listBuyItem.Select(y => y.ItemId).ToList<int>();
                    listItem = (from x in oDb.Item where ary_nItemId.Contains(x.ID) select x).ToList<Item>();
                    oDb.Dispose();
                    foreach (UsedCoupon objSubCoupon in arg_listUsedCoupon)
                    {
                        objBuyItem = arg_listBuyItem.SingleOrDefault(x => x.ItemId == objSubCoupon.ItemId);
                        objItem = listItem.FirstOrDefault(x => x.ID == objSubCoupon.ItemId);
                        numItemPrice = Convert.ToDecimal(objBuyItem.DisplayPrice);

                        if (objSubCoupon.Coupons.Count == 0)
                        {
                            continue; //沒使用Coupon就不比對
                        }

                        //比對購買品項
                        if (objBuyItem == null || objItem == null)
                        {
                            listUserActiveCoupon = null;
                            return false;
                        }

                        //比對使用數量
                        if (objSubCoupon.Coupons.Count > objBuyItem.BuyNumber)
                        {
                            listUserActiveCoupon = null;
                            return false;
                        }
                        //比對使用類別
                        listItemCategories = new List<string>();
                        listItemCategories.Add(objItem.CategoryID.ToString());
                        this.GetItemRootCategoryId(objItem.CategoryID, 1, ref listItemCategories); //取得該Item的所有父階ID
                        foreach (UsedCouponItem objUsedCouponItem in objSubCoupon.Coupons)
                        {
                            boolCheck = false;
                            objCoupon = listUserActiveCoupon.SingleOrDefault(x => x.id == objUsedCouponItem.CouponId);
                            if (objCoupon == null)
                            {
                                boolErrorCoupon = true;
                                break; //有任何錯誤就跳出
                            }

                            //比對類別與金額
                            if (objCoupon.categories.IndexOf(";0;") >= 0 && (objCoupon.limit == 0 || objCoupon.limit <= numItemPrice))
                            {
                                numTempTotalCouponValue += objCoupon.value;
                                boolCheck = true;
                            }
                            else
                            {
                                foreach (string strCategoryId in listItemCategories)
                                {
                                    if (objCoupon.categories.IndexOf(";" + strCategoryId + ";") >= 0)
                                    {
                                        if (objCoupon.limit == 0 || objCoupon.limit <= numItemPrice)
                                        {
                                            numTempTotalCouponValue += objCoupon.value;
                                            boolCheck = true;
                                        }

                                        break;
                                    }
                                } //end foreach
                            }
                            //若沒有比對到
                            if (!boolCheck)
                            {
                                boolErrorCoupon = true;
                                break;
                            }

                            if (boolErrorCoupon)
                            {
                                break;
                            }
                        } //end foreach(UsedCouponItem oUsedCouponItem in oSubCoupon.coupons)
                        if (boolErrorCoupon)
                        {
                            break;
                        }
                    } //end foreach (UsedCoupon oSubCoupon in arg_listUsedCoupon)
                }

                //比對最後的總值
                if (!numTempTotalCouponValue.Equals(arg_nTotalCouponValue))
                {
                    boolErrorCoupon = true;
                }
            } //end if (arg_listUsedCoupon != null && arg_listUsedCoupon.Count > 0)

            //釋放Coupon相關的記憶體
            listUserActiveCoupon = null;
            objCoupon = null;
            listItem = null;
            if (oDb != null)
            {
                oDb.Dispose();
            }

            oDb = null;
            return !boolErrorCoupon;
        }
        #endregion

        /// <summary>
        /// 設定消費者使用Coupon的狀態
        /// </summary>
        /// <param name="arg_nSalesOrderGroupId"></param>
        /// <param name="arg_strAccountId"></param>
        private void AddSalesOrderCoupon(int arg_nSalesOrderGroupId, string arg_strAccountId)
        {
            AfterPayService afterService = new AfterPayService();
            afterService.AddSalesOrderCoupon(arg_nSalesOrderGroupId, arg_strAccountId);
        }
        #endregion

        #region Step2
        /// <summary>
        /// 重新導向Step2()
        /// </summary>
        /// <returns>Step2預設畫面</returns>
        [HttpPost]
        public ActionResult Step2(CartStep1Data postdata)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            int getAccID = NEUser.ID;
            string userEmail = NEUser.Email;
            ECWebResponse response;
            try
            {
                ViewBag.CartTypeID = postdata.CartTypeID;
                ViewBag.AccountID = getAccID;
                ViewBag.SerialNumber = postdata.SerialNumber;
                //List<int> itemIDList = new List<int>();
                //postdata.CartItemDetailList_View.ForEach(x =>
                //{
                //    itemIDList.Add(x.ItemID);
                //});
                //// 執行總價化
                //string getDisplayItemResult = Processor.Request<string, string>("ItemDisplayPriceService", "SetItemDisplayPriceByIDs", itemIDList).results;
                //if (getDisplayItemResult.Length > 0)
                //{
                //    logger.Error("[UserEmail] [" + userEmail + "] " + "Step2 執行總價化function [ErrorMsg] " + getDisplayItemResult);
                //    //return Json(new { message = "商品金額錯誤無法購買，請與客服聯繫，謝謝" }, JsonRequestBehavior.AllowGet);
                //    response = new ECWebResponse()
                //    {
                //        Status = (int)ECWebResponse.StatusCode.返回購物車頁,
                //        Message = "商品金額錯誤無法購買，請與客服聯繫，謝謝"
                //    };
                //    return Json(response);
                //}

                ViewBag.PayType = 0;
                if (postdata != null && postdata.CartPaytype_View != null)
                {
                    ViewBag.PayType = postdata.CartPaytype_View.PayType0rateNum;
                }
                // 取得客戶相關資訊
                var getResult = Processor.Request<CartMemberInfoVM, CartMemberInfoDM>("GetMemberService", "GetCartMemberInfo", getAccID);
                CartMemberInfoVM getCartMemberInfo = getResult.results;
                if (getCartMemberInfo != null)
                {
                    if (getCartMemberInfo.DeliverAddressBookList.Count == 0)
                    {
                        ViewBag.AddressbookFlag = 1;
                    }
                }
                ViewBag.HideCardFlag = 0;
                // 當不是信用卡付款，或是台新信用卡付款，不顯示信用卡form (台新信用卡需跳轉頁面)
                Bank searchBank = (from p in db_before.Bank where p.ID == postdata.CartPaytype_View.BankID select p).FirstOrDefault();
                PayType payTypeCheck = null;
                int? nullBankId = null;
                payTypeCheck = this.GetCurrentPatyType((PayType.nPayType)postdata.CartPaytype_View.PayType0rateNum, searchBank != null ? searchBank.ID : nullBankId);
                if ((postdata.CartPaytype_View.PayTypeGroupID != (int)CartPayTypeGroupenum.信用卡 && postdata.CartPaytype_View.PayTypeGroupID != (int)CartPayTypeGroupenum.信用卡紅利折抵)
                    || payTypeCheck.Verification == (int)PayType.PaymentVerification.三Ｄ驗證)
                {
                    ViewBag.HideCardFlag = 1;
                }

                if (OTPServiceGateway.ToLower() == "on")
                {
                    ViewBag.OTPServiceGateway = true;
                }
                else
                {
                    ViewBag.OTPServiceGateway = false;
                }
                // 客戶基本資訊
                ViewBag.GetCartMemberInfo = getCartMemberInfo;
                ViewBag.MemberAddressBook = RenderView("MemberAddressBook");
                // 收件人紀錄本
                ViewBag.RecipientAddressBook = RenderView("RecipientAddressBook");
                // 統編紀錄本
                ViewBag.CompanyBook = RenderView("CompanyBook");
                string result = new JavaScriptSerializer().Serialize(postdata);
                return PartialView("Step2", result);
            }
            catch (Exception ex)
            {
                logger.Error("[UserEmail] [" + userEmail + "] " + "Step2 Error [ErrorMsg] " + ex.ToString() + " [StackTrace] " + ex.StackTrace + " [InnerException] " + (ex.InnerException) ?? "");
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.返回購物車頁,
                    Message = "資料錯誤，請與客服聯繫，謝謝"
                };
                return Json(response);
            }
        }

        private enum OTPStauts
        {
            // 初始化
            init = 0,
            // 通過
            byPass = 1,
            // 原地不動
            Stagnant = 2,
            // 顯示驗證頁面
            Verification = 3,
            // 嚴重錯誤返回購物車頁
            returnToCart = 4,
        }

        public JsonResult OTPCheck(string step1Data, string mobile)
        {
            try
            {
                int accID = NEUser.ID;
                string email = NEUser.Email;
                string itemIDs = string.Empty;
                logger.Info("[Email] " + email + " [OTPCheck] start");
                logger.Info("[Email] " + email + " [step1Data] " + step1Data + " [Mobile] " + mobile);
                if (string.IsNullOrEmpty(step1Data) || string.IsNullOrEmpty(mobile))
                {
                    return Json(new { IsSuccess = (int)OTPStauts.Stagnant, Msg = "資料缺漏! 請重檢查訂購人手機號碼是否未填或重新進入購物車" }, JsonRequestBehavior.AllowGet);
                }

                TWNewEgg.DB.TWSqlDBContext sDB = new DB.TWSqlDBContext();
                OTPRecord otr = sDB.OTPRecord.Where(x => x.UserID == accID).FirstOrDefault();
                //logger.Info("[Email] " + email + " [OTPRecord取得] " + otr);
                DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
                if (otr != null)
                {
                    //判斷是否jo6BLOCK且已經超過Block時間，若已超過把Status改成Null，並把FailCount歸零
                    if (datetimeNow > otr.StatusDate.AddDays(1) && otr.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block)
                    {
                        otr.Status = (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Null;
                        otr.FailCount = 0;
                        otr.StatusDate = datetimeNow;
                        sDB.SaveChanges();
                    }
                }
                sDB.Dispose();
                //logger.Info("[Email] " + email + " [OTPRecord End] ");
                TWSqlDBContext db_before = new TWSqlDBContext();
                OTPService otpService = new OTPService();
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

                #region CustomerInfo 產生密碼 send sms
                CartStep1Data postdata = jsSerializer.Deserialize<CartStep1Data>(step1Data);
                //logger.Info("[Email] " + email + " [CartStep1Data 取得] " + postdata);
                //logger.Info("[Email] " + email + " [searchBank ID] " + postdata.CartPaytype_View.BankID);
                // 檢查付款方式有無被使用者不當修改
                Bank searchBank = (from p in db_before.Bank where p.ID == postdata.CartPaytype_View.BankID select p).FirstOrDefault();
                PayType payTypeCheck = null;
                //logger.Info("[Email] " + email + " [payTypeCheck start] ");
                if (searchBank != null)
                {
                    payTypeCheck = this.GetCurrentPatyType((PayType.nPayType)postdata.CartPaytype_View.PayType0rateNum, searchBank.ID);
                }
                else
                {
                    payTypeCheck = this.GetCurrentPatyType((PayType.nPayType)postdata.CartPaytype_View.PayType0rateNum, 0);
                }
                //logger.Info("[Email] " + email + " [payTypeCheck ID] " + payTypeCheck.ID);
                postdata.CartItemDetailList_View.ForEach(x =>
                {
                    itemIDs += x.ItemID + ",";
                });
                //logger.Info("[Email] " + email + " [itemIDs] " + itemIDs);
                itemIDs = "'" + itemIDs.Substring(0, itemIDs.Length - 1) + "'";
                //logger.Info("[Email] " + email + " [itemIDs Substring] " + itemIDs);
                decimal insRate = 0;
                if (payTypeCheck != null)
                {
                    insRate = (decimal)payTypeCheck.InsRate;
                }
                int cumulativePrice = Convert.ToInt32(Math.Floor(0.5m + postdata.AmountsPayable * (1 + insRate)));
                // 判斷購物金額是否大於10000及是否選擇信用卡付款方式，且非使用3D驗證的皆需驗證OTP
                if (payTypeCheck != null
                    && payTypeCheck.Verification != 3
                    && cumulativePrice >= 10000
                    && (postdata.CartPaytype_View.PayTypeGroupID == 1 || postdata.CartPaytype_View.PayTypeGroupID == 2))
                {
                    //判斷該UID是否被Block住,若被Block住就導回購物車首頁,若被Block住就導回購物車首頁
                    if (otpService.IsSMSBlock(accID, cumulativePrice))
                    {
                        return Json(new { IsSuccess = (int)OTPStauts.returnToCart, Msg = "親愛的" + WebSiteData.Abbreviation + "顧客，您已錯誤輸入超過三次認證碼，您將於24小時內無法進行1萬元以上的購物，若還需購買一萬元以上商品，歡迎來電洽詢客服中心。" }, JsonRequestBehavior.AllowGet);
                    }
                    OTPRecord DBRecord = null;
                    DBRecord = new OTPRecord();
                    //logger.Info("[Email] " + email + " [BlackList new ]");
                    TWNewEgg.Website.ECWeb.Service.BlackList.BlackList _blackList = new TWNewEgg.Website.ECWeb.Service.BlackList.BlackList();
                    //logger.Info("[Email] " + email + " [isMobileError] moblie : " + mobile);
                    bool isMobileError = _blackList.MobileBlackList(mobile);
                    //logger.Info("[Email] " + email + " [isMobileError] " + isMobileError);
                    if (isMobileError)
                    {
                        return Json(new { IsSuccess = (int)OTPStauts.Stagnant, Msg = "此電話無法認證，請提供可認證之手機號碼" }, JsonRequestBehavior.AllowGet);
                    }

                    DBRecord = db_before.OTPRecord.Where(x => x.UserID == accID).FirstOrDefault();
                    if (DBRecord != null && DBRecord.Phone.Trim() != mobile)
                    {
                        //logger.Info("[Email] " + email + " [otpService.GenerateSMSCode start]");
                        string SMSpaasword = otpService.GenerateSMSCode(accID, cumulativePrice, mobile, itemIDs);
                        //logger.Info("[Email] " + email + " [otpService.GenerateSMSCode] " + SMSpaasword);
                        if (SMSpaasword.Trim().Count() != 0)
                        {
                            //logger.Info("[Email] " + email + " [SendSMS send 1] ");
                            otpService.SendSMS(mobile, SMSpaasword);
                            //logger.Info("[Email] " + email + " [SendSMS end 1] ");
                        }
                        else
                        {
                            logger.Info("[Email] " + email + " [SMSpaasword 錯誤密碼] ");
                            return Json(new { IsSuccess = (int)OTPStauts.returnToCart, Msg = "SendSMS錯誤密碼" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //logger.Info("[Email] " + email + " [otpService.IsSMSSuccess(accID)] " + accID);
                        if (otpService.IsSMSSuccess(accID) != true)
                        {
                            //logger.Info("[Email] " + email + " [(DBRecord != null && DateTime.Now >= DBRecord.StatusDate.AddMinutes(10)) || DBRecord == null)] ");
                            if ((DBRecord != null && DateTime.UtcNow.AddHours(8) >= DBRecord.StatusDate.AddMinutes(10)) || DBRecord == null)
                            {
                                string SMSpaasword = otpService.GenerateSMSCode(accID, cumulativePrice, mobile, itemIDs);
                                //logger.Info("[Email] " + email + " [otpService.GenerateSMSCode 2] " + SMSpaasword);
                                if (SMSpaasword.Trim().Count() != 0)
                                {
                                    //logger.Info("[Email] " + email + " [SendSMS send 2] ");
                                    otpService.SendSMS(mobile, SMSpaasword);
                                    //logger.Info("[Email] " + email + " [SendSMS end 2] ");
                                }
                                else
                                {
                                    logger.Info("[Email] " + email + " [SMSpaasword 錯誤密碼 2] ");
                                    return Json(new { IsSuccess = (int)OTPStauts.returnToCart, Msg = "SendSMS錯誤密碼" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    DBRecord = db_before.OTPRecord.Where(x => x.UserID == accID).FirstOrDefault();
                    //logger.Info("[Email] " + email + " [return Json] ");
                    if (DBRecord.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Success)
                    {
                        logger.Info("[Email] " + email + " [return Json] 1");
                        return Json(new { IsSuccess = (int)OTPStauts.byPass, Msg = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (DBRecord.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block)
                    {
                        logger.Info("[Email] " + email + " [return Json] 2");
                        return Json(new { IsSuccess = (int)OTPStauts.returnToCart, Msg = "親愛的" + WebSiteData.Abbreviation + "顧客，您已錯誤輸入超過三次認證碼，您將於24小時內無法進行1萬元以上的購物，若還需購買一萬元以上商品，歡迎來電洽詢客服中心。" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        logger.Info("[Email] " + email + " [return Json 3] ");
                        OTPInfo otpModel = GetOTPTimeLimit(accID);
                        logger.Info("[Email] " + email + " [return Json 3 otpModel] " + otpModel);
                        return Json(new { IsSuccess = (int)OTPStauts.Verification, Msg = "getTimeOut", DeadLine = otpModel.DeadLine, MinutesLeft = otpModel.MinutesLeft }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                return Json(new { IsSuccess = (int)OTPStauts.byPass, Msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("OTP執行產生錯誤[ErrorMsg] " + ex.ToString() + " [StackTrace] " + ex.StackTrace + "   [Inner] " + (ex.InnerException) ?? "");
                return Json(new { IsSuccess = (int)OTPStauts.Stagnant, Msg = "OTP資料錯誤，請與客服人員聯繫，謝謝" }, JsonRequestBehavior.AllowGet);
            }
        }

        private class OTPInfo
        {
            // 到期時間
            public string DeadLine { get; set; }
            // 剩餘時間
            public double MinutesLeft { get; set; }
        }

        private OTPInfo GetOTPTimeLimit(int accID)
        {
            string email = NEUser.Email;
            logger.Info("[Email] " + email + " [GetOTPTimeLimit] ");
            TWSqlDBContext db_before = new TWSqlDBContext();
            OTPInfo otpModel = new OTPInfo();
            OTPRecord dbRecord = new OTPRecord();
            logger.Info("[Email] " + email + " [GetOTPTimeLimit dbRecord query] ");
            dbRecord = db_before.OTPRecord.Where(x => x.UserID == accID).FirstOrDefault();
            if (dbRecord != null)
            {
                logger.Info("[Email] " + email + " [GetOTPTimeLimit dbRecord query 非空 UserID] " + dbRecord.UserID);
            }
            else
            {
                logger.Info("[Email] " + email + " [GetOTPTimeLimit dbRecord query 為空]");
            }
            string otpTimeOutDate = dbRecord.StatusDate.AddMinutes(10).Hour.ToString("00") + ":" + dbRecord.StatusDate.AddMinutes(10).Minute.ToString("00"); // otp到期時間
            //logger.Info("[Email] " + email + " [GetOTPTimeLimit otpTimeOutDate] " + otpTimeOutDate + " [dbRecord.StatusDate] " + dbRecord.StatusDate);
            TimeSpan getTimeout = new TimeSpan(dbRecord.StatusDate.AddMinutes(10).Ticks - DateTime.UtcNow.AddHours(8).Ticks);
            //logger.Info("[Email] " + email + " [GetOTPTimeLimit getTimeout] " + getTimeout );
            double timeOutMinutes = getTimeout.TotalMinutes;
            //logger.Info("[Email] " + email + " [GetOTPTimeLimit timeOutMinutes] " + timeOutMinutes);
            logger.Info("[Email] " + email + " [GetOTPTimeLimit DeadLine] " + otpTimeOutDate + " [MinutesLeft] " + timeOutMinutes);
            return new OTPInfo { DeadLine = otpTimeOutDate, MinutesLeft = timeOutMinutes };
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        private string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }

        /// <summary>
        /// OverseaStep2
        /// </summary>
        /// <returns>OverseaStep2預設畫面</returns>
        [HttpPost]
        public ActionResult OverseaStep2(CartStep1Data postdata)
        {
            string result = new JavaScriptSerializer().Serialize(postdata);
            return PartialView("OverseaStep2", result);
        }

        #endregion Step2

        private List<TWNewEgg.ItemService.Models.BuyingItems> GetStrPostData(CartStep1Data step1Data)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<TWNewEgg.ItemService.Models.BuyingItems> buyingItemPostData = new List<TWNewEgg.ItemService.Models.BuyingItems>();
            List<int> itemIDList = new List<int>();
            itemIDList = step1Data.CartItemDetailList_View.Select(x => x.ItemID).ToList();
            List<Item> allCartItem = db_before.Item.Where(x => itemIDList.Contains(x.ID)).ToList();
            foreach (CartItemDetail_View cartItemDetailView in step1Data.CartItemDetailList_View)
            {
                BuyingItems itemDetail = new BuyingItems();
                Item searchItem = allCartItem.Where(x => x.ID == cartItemDetailView.ItemID).FirstOrDefault();
                itemDetail.buyItemID = cartItemDetailView.ItemID;
                itemDetail.buyingNumber = cartItemDetailView.Qty;
                itemDetail.buyItemID_DelvType = searchItem.DelvType;
                itemDetail.buyItemID_Seller = searchItem.SellerID;
                buyingItemPostData.Add(itemDetail);
            }

            db_before.Dispose();
            return buyingItemPostData;
        }

        public string Serialize(object model)
        {
            XmlSerializer ser = new XmlSerializer(model.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, model);
            return sb.ToString();
        }

        /// <summary>
        /// 組合PromotionInput清單資訊
        /// </summary>
        /// <param name="itemDisplayPriceList">賣場詳細資訊清單</param>
        /// <param name="step1Data">購物車頁傳入資訊</param>
        /// <returns>返回PromotionInput清單資訊</returns>
        private List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput> GetPromotionInputList(List<ItemDisplayPrice> itemDisplayPriceList, CartStep1Data step1Data)
        {
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput> promotionInputList = new List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput>();
            foreach (ItemDisplayPrice displayPrice in itemDisplayPriceList)
            {
                TWNewEgg.Models.ViewModels.Redeem.PromotionInput PromotionInputTemp = new TWNewEgg.Models.ViewModels.Redeem.PromotionInput();

                PromotionInputTemp.ItemID = displayPrice.ItemID;
                PromotionInputTemp.Price = Convert.ToInt32(Math.Floor(displayPrice.DisplayPrice + 0.5m));
                PromotionInputTemp.Qty = step1Data.CartItemDetailList_View.Where(x => x.ItemID == displayPrice.ItemID).FirstOrDefault().Qty;
                PromotionInputTemp.SumPrice = PromotionInputTemp.Price * PromotionInputTemp.Qty;
                promotionInputList.Add(PromotionInputTemp);
            }

            return promotionInputList;
        }

        private string GetTelDay(string telZip, string telNumber, string telExt)
        {
            string telDay = string.Empty;
            if (!string.IsNullOrEmpty(telZip))
            {
                telDay += "(" + telZip.Trim() + ")";
            }
            if (!string.IsNullOrEmpty(telNumber))
            {
                telDay += telNumber.Trim();
            }
            if (!string.IsNullOrEmpty(telExt))
            {
                telDay += "#" + telExt.Trim();
            }

            return telDay;
        }

        #region Step3

        [HttpPost]
        public JsonResult Step3(CartStep2Data data)
        {
            ECWebResponse response;
            int account_id = NEUser.ID;
            string userEmail = NEUser.Email;
            //logger.Info("[UserEmail] [" + userEmail + "] " + "Step3 start");
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<int> itemIDList = new List<int>();
            List<ItemDisplayPrice> getItemDisplayPriceList = new List<ItemDisplayPrice>();
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<BuyingItems> buyingItemPostData = new List<BuyingItems>();
            SalesOrderService salesOrderService = new SalesOrderService();
            //PromotionGiftRepository promotionGift = new PromotionGiftRepository();
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> promotionGiftDetailV2 = null;
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput> promotionInputList = null;
            string postData = string.Empty;
            List<ShoppingCartItems> sendcheckout = new List<ShoppingCartItems>();
            string errorMessage = string.Empty;
            InsertSalesOrdersBySellerInput _data = new InsertSalesOrdersBySellerInput();
            // 活動折抵總額
            int getDiscountAmount = 0;
            logger.Info("[UserEmail] [" + userEmail + "] " + "Step3 step1Data");
            CartStep1Data step1Data = null;
            try
            {
                step1Data = jsSerializer.Deserialize<CartStep1Data>(data.step1Data);
            }
            catch (Exception ex)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(取得CartStep1Data) : [ErrorMessage]" + ex.Message + " [StrackTrace]" + ex.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }

            #region 會員資料新增與修改&新增訂購人紀錄本
            try
            {
                #region Member新建與更新
                // 檢查member資料是否已存在
                MemberVM searchMember = Processor.Request<MemberVM, MemberDM>("AccountService", "GetMember", account_id).results;
                if (searchMember == null || data.memberUpdate == true)
                {
                    MemberVM SaveMemberVM = new MemberVM();
                    SaveMemberVM.AccID = account_id;
                    SaveMemberVM.Firstname = data.member_firstname;
                    SaveMemberVM.Lastname = data.member_lastname;
                    SaveMemberVM.Mobile = data.salesorder_mobile;
                    SaveMemberVM.Sex = Convert.ToInt32(data.member_sex);
                    SaveMemberVM.TelZip = data.teldayzip;
                    SaveMemberVM.TelDay = data.teldaynumber;
                    SaveMemberVM.TelExtension = data.teldayext;
                    SaveMemberVM.Zip = data.salesorder_cardzip;
                    SaveMemberVM.Loc = data.salesorder_cardloc;
                    SaveMemberVM.Zipname = data.cardCity.Split(' ').Length == 2 ? data.cardCity.Split(' ')[1] : data.cardCity;
                    SaveMemberVM.Address = data.cardaddr;
                    MemberDM memberDM = ModelConverter.ConvertTo<MemberDM>(SaveMemberVM);
                    // 若member資料不存在則新建
                    if (searchMember == null)
                    {
                        MemberVM addResult = Processor.Request<MemberVM, MemberDM>("AccountService", "CreateMember", memberDM).results;
                    }
                    else if (searchMember != null && data.memberUpdate == true)
                    {
                        memberDM.Birthday = searchMember.Birthday;
                        // 若member已存在，且使用者有勾選更新member資料則執行下列程式
                        bool updateResult = Processor.Request<bool, bool>("AccountService", "UpdateMemberInfo", memberDM).results;
                    }

                    AccountDM accountDM = Processor.Request<AccountDM, AccountDM>("AccountService", "GetAccountByEmail", userEmail).results;
                    if (accountDM != null)
                    {
                        accountDM.Name = SaveMemberVM.Lastname + SaveMemberVM.Firstname;
                        accountDM.TelDay = GetTelDay(SaveMemberVM.TelZip, SaveMemberVM.TelDay, SaveMemberVM.TelExtension);
                        accountDM.Mobile = SaveMemberVM.Mobile;
                        accountDM.Sex = SaveMemberVM.Sex;
                        accountDM.Loc = SaveMemberVM.Loc;
                        accountDM.Zip = SaveMemberVM.Zip;
                        accountDM.Address = SaveMemberVM.Zipname + SaveMemberVM.Address;
                        accountDM.UpdateDate = DateTime.UtcNow.AddHours(8);
                        AccountVM updateResult = Processor.Request<AccountVM, AccountDM>("AccountService", "UpdateAccount", accountDM).results;
                    }
                }
                #endregion
                #region 新增訂購人紀錄本
                // 新增訂購人紀錄本是否有被勾選
                if (data.memberRecords == true)
                {
                    AddressBookDM addMemberAddressBook = new AddressBookDM();
                    addMemberAddressBook.AccountID = account_id;
                    addMemberAddressBook.AccountEmail = userEmail;
                    addMemberAddressBook.RecvSex = Convert.ToInt32(data.member_sex);
                    addMemberAddressBook.RecvName = data.member_lastname + data.member_firstname;
                    addMemberAddressBook.RecvFirstName = data.member_firstname;
                    addMemberAddressBook.RecvLastName = data.member_lastname;
                    addMemberAddressBook.RecvTelDay = GetTelDay(data.teldayzip, data.teldaynumber, data.teldayext);
                    addMemberAddressBook.RecvMobile = data.salesorder_mobile;
                    addMemberAddressBook.DelivZip = data.salesorder_cardzip;
                    addMemberAddressBook.DelivLOC = data.salesorder_cardloc;
                    addMemberAddressBook.DelivZipName = data.cardCity.Split(' ').Length == 2 ? data.cardCity.Split(' ')[1] : data.cardCity;
                    addMemberAddressBook.DelivAddr = addMemberAddressBook.DelivZipName + data.cardaddr;
                    addMemberAddressBook.DelivAddress = data.cardaddr;
                    addMemberAddressBook.IsMemberBook = 1;
                    addMemberAddressBook.CreateDate = DateTime.UtcNow.AddHours(8);
                    addMemberAddressBook.TelZip = data.teldayzip;
                    addMemberAddressBook.TelDay = data.teldaynumber;
                    addMemberAddressBook.TelExtension = data.teldayext;
                    addMemberAddressBook.DefaultSetting = 1; // 設定為預設Address

                    AddressBookVM addAddressBookResult = Processor.Request<AddressBookVM, AddressBookDM>("GetMemberService", "AddAddressBook", addMemberAddressBook).results;
                }
                #endregion
            }
            catch (Exception e)
            {
                logger.Info("Step3(會員資料新增與修改&新增訂購人紀錄本失敗) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
            }
            #endregion 會員資料新增與修改&新增訂購人紀錄本
            #region 新增收件人紀錄
            try
            {
                // 新增收件人紀錄本是否有被勾選
                if (data.delivRecords == true)
                {
                    AddressBookDM addDelivAddressBook = new AddressBookDM();
                    addDelivAddressBook.AccountID = account_id;
                    addDelivAddressBook.AccountEmail = userEmail;
                    addDelivAddressBook.RecvSex = Convert.ToInt32(data.member_recvsex);
                    addDelivAddressBook.RecvName = data.member_recvlastname + data.member_recvfirstname;
                    addDelivAddressBook.RecvFirstName = data.member_recvfirstname;
                    addDelivAddressBook.RecvLastName = data.member_recvlastname;
                    addDelivAddressBook.RecvTelDay = GetTelDay(data.recvteldayzip, data.recvteldaynumber, data.recvteldayext);
                    addDelivAddressBook.RecvMobile = data.salesorder_recvmobile;
                    addDelivAddressBook.DelivZip = data.salesorder_delivzip;
                    addDelivAddressBook.DelivLOC = data.salesorder_delivloc;
                    addDelivAddressBook.DelivZipName = data.delivCity.Split(' ').Length == 2 ? data.delivCity.Split(' ')[1] : data.delivCity;
                    addDelivAddressBook.DelivAddr = addDelivAddressBook.DelivZipName + data.delivaddr;
                    addDelivAddressBook.DelivAddress = data.delivaddr;
                    addDelivAddressBook.IsMemberBook = 0;
                    addDelivAddressBook.CreateDate = DateTime.UtcNow.AddHours(8);
                    addDelivAddressBook.TelZip = data.teldayzip;
                    addDelivAddressBook.TelDay = data.teldaynumber;
                    addDelivAddressBook.TelExtension = data.teldayext;
                    addDelivAddressBook.DefaultSetting = 1; // 設定為預設Address

                    AddressBookVM addAddressBookResult = Processor.Request<AddressBookVM, AddressBookDM>("GetMemberService", "AddAddressBook", addDelivAddressBook).results;
                }
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(新增收件人紀錄本失敗) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
            }
            #endregion 新增收件人紀錄
            #region 新增統編紀錄
            try
            {
                if (data.invoRecords == true && !string.IsNullOrEmpty(data.salesorder_invotitle) && !string.IsNullOrEmpty(data.salesorder_invoid))
                {
                    CompanyBookDM cbook = new CompanyBookDM();
                    cbook.Accountid = account_id;
                    cbook.Title = data.salesorder_invotitle;
                    cbook.Number = data.salesorder_invoid;
                    cbook.Delivloc = "";
                    cbook.Delivzip = "";
                    cbook.DelivZipName = "";
                    cbook.Delivaddr = "";

                    CompanyBookVM addCompanyBook = Processor.Request<CompanyBookVM, CompanyBookDM>("GetMemberService", "AddCompanyBook", cbook).results;
                }
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(新增統編紀錄失敗) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
            }
            #endregion 新增統編紀錄
            step1Data.CartItemDetailList_View.ForEach(x => itemIDList.Add(x.ItemID));
            getItemDisplayPriceList = db_before.ItemDisplayPrice.Where(x => itemIDList.Contains(x.ItemID)).ToList();
            List<Item> itemList = db_before.Item.Where(x => itemIDList.Contains(x.ID)).ToList();
            step1Data.CartItemDetailList_View.ForEach(x =>
            {
                if (x.Category == 0)
                {
                    x.Category = itemList.Where(y => y.ID == x.ItemID).FirstOrDefault().CategoryID;
                }
            });

            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon = step1Data.CouponList;
            try
            {
                // 取得List<BuyingItems>
                buyingItemPostData = GetStrPostData(step1Data);
                // 取得PostData
                postData = jsSerializer.Serialize(buyingItemPostData);
            }
            catch (Exception ex)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(取得List<BuyingItems>) : [ErrorMessage]" + ex.Message + " [StrackTrace]" + ex.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #region 消費者購買次數SP、購物車資訊SP
            int getNumByDate = 0;
            DbQuery nDb = null;
            SqlParameter[] narySqlParameter = null; //getSalesOrderNumByDate
            DataSet ndsResult = null; //getSalesOrderNumByDate
            string combine = "";
            DataTable ndtItem = null; //getSalesOrderNumByDate
            //List<ShoppingCartItems> sendcheckout = new List<ShoppingCartItems>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                nDb = new TWNewEgg.Website.ECWeb.Models.DbQuery();
                //iDb.setConnectionString("10.16.131.43", "TWSQLDB", "twec", "ABS301egg");
                narySqlParameter = new SqlParameter[1]; // 如果要增加參數則SqlParameter[2]、SqlParameter[3]...
                narySqlParameter[0] = new SqlParameter("@accunt_id", SqlDbType.Int);
                narySqlParameter[0].Value = account_id;
                combine = "declare @kk int exec [UP_EC_GetSalesOrderNumByDate] " + narySqlParameter[0].Value + ",@kk out select @kk";
                ndsResult = nDb.Query(combine, narySqlParameter);
                if (ndsResult != null && ndsResult.Tables.Count > 0)
                {
                    ndtItem = ndsResult.Tables[0];

                    foreach (DataRow dr in ndtItem.Rows)
                    {
                        getNumByDate = Convert.ToInt32(dr[0]);
                    } //end foreach
                } //end if (dsResult != null && dsResult.Tables.Count > 0)
                nDb.Dispose();
            }
            catch (Exception ex)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(消費者購買次數SP、購物車資訊SP) : [ErrorMessage]" + ex.Message + " [StrackTrace]" + ex.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion 消費者購買次數SP、購物車資訊SP
            #region 買一送一商品數量檢驗
            try
            {
                if (!this.TwoForOneOfferQtyCheck(buyingItemPostData))
                {
                    logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(買一送一商品數量檢驗) : [ErrorMessage] 買一送一商品數量檢驗錯誤，單品項商品不為2");
                    response = new ECWebResponse()
                    {
                        Status = (int)ECWebResponse.StatusCode.系統錯誤,
                        Message = "部分買一送一商品數量不足，請移除該商品並重新選購，若有疑問請洽詢客服單位，謝謝"
                    };
                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(買一送一商品數量檢驗) : [ErrorMessage]" + ex.Message + " [StrackTrace]" + ex.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion 買一送一商品數量檢驗
            #region 符合滿額贈條件的賣場ID與滿額贈折價加金額、運費計算
            try
            {
                promotionInputList = new List<TWNewEgg.Models.ViewModels.Redeem.PromotionInput>();
                promotionGiftDetailV2 = new List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail>();
                promotionInputList = GetPromotionInputList(getItemDisplayPriceList, step1Data);
                //promotionGiftDetailV2 = promotionGift.PromotionGiftParsingV2(promotionInputList, promotionGiftStatusTurnON);
                promotionGiftDetailV2 = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>>("Service.PromotionGiftService.PromotionGiftRepository", "PromotionGiftParsingV2", account_id, promotionInputList, promotionGiftStatusTurnON).results;
                // 累加滿額贈所有活動總金額
                if (promotionGiftDetailV2 != null)
                {
                    promotionGiftDetailV2.ForEach(x => getDiscountAmount += Convert.ToInt32(Math.Floor(0.5m + x.ApportionedAmount)));
                    // DelvType是否為間配或三角等必定有運費的付款類型
                    bool boolShip = false;
                    // 運費金額
                    decimal totalShippingFee = 0m;
                    buyingItemPostData.ForEach(x =>
                    {
                        if (x.buyItemID_DelvType == (int)Item.tradestatus.間配 || x.buyItemID_DelvType == (int)Item.tradestatus.三角)
                        {
                            boolShip = true;
                            Item searchItemInfo = itemList.Where(a => a.ID == x.buyItemID).FirstOrDefault();
                            totalShippingFee += getItemDisplayPriceList.Where(y => y.ItemID == x.buyItemID).Select(y => y.DisplayShipping).FirstOrDefault() - searchItemInfo.ServicePrice;
                        }
                    });

                    if (boolShip == true && totalShippingFee == 0m)
                    {
                        logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(運費計算) : 系統資料設定錯誤，此商品運費不應為0");
                        response = new ECWebResponse()
                        {
                            Status = (int)ECWebResponse.StatusCode.系統錯誤,
                            Message = "商品費用計算錯誤請重新選擇商品!"
                        };
                        return Json(response);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(運費計算) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion 符合滿額贈條件的賣場ID與滿額贈折價加金額、運費計算
            #region 取得加價購商品清單
            // 取得加價購商品清單
            TWNewEgg.Framework.ServiceApi.ResponsePacket<List<ShoppingCartItems>> getIncreasePurchaseItemList = null;
            try
            {
                getIncreasePurchaseItemList = Processor.Request<List<ShoppingCartItems>, List<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems>>("ShoppingCartService", "GetIncreasePurchaseItemList", account_id, step1Data.CartTypeID);
                if (getIncreasePurchaseItemList.error != null)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "取得加價購商品清單失敗");
                    throw new Exception(getIncreasePurchaseItemList.error);
                }
            }
            catch (Exception ex)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(取得加價購商品清單失敗) : [ErrorMessage]" + ex.Message + " [StrackTrace]" + ex.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion
            #region CouponFunction & SO資料組合 (同時檢查item 與 product的資料一致性(Delvtype與SellerID))
            try
            {
                #region CouponFunction
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listTempCoupon = null;
                bool boolCouponError = false;
                TWNewEgg.Models.ViewModels.Redeem.Coupon objCoupon = null;
                decimal numCouponAmount = 0;
                listCoupon = step1Data.CouponList;
                //var test = Processor.Request<TWNewEgg.Models.DomainModels.Message.ResponseMessage<string>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<string>>("CouponService", "CheckCouponForCart", account_id, step1Data);
                if (listCoupon != null && listCoupon.Count > 0)
                {
                    // 先取得資料庫中使用者有效的折價券
                    listTempCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", NEUser.ID.ToString()).results;
                    if (listTempCoupon == null || listTempCoupon.Count <= 0 || listTempCoupon.Count != listCoupon.Count)
                    {
                        boolCouponError = true;
                    }
                    // 比對折價券的內容是否有誤, 有任何錯誤就跳出
                    if (!boolCouponError)
                    {
                        foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon objSubCoupon in listCoupon)
                        {
                            objCoupon = listTempCoupon.Where(x => x.id == objSubCoupon.id).FirstOrDefault();
                            if (objCoupon == null)
                            {
                                boolCouponError = true;
                                break;
                            }
                            // 增加 items 檢查 2015/11/16
                            if (objSubCoupon.value != objCoupon.value || objSubCoupon.limit != objCoupon.limit || objSubCoupon.categories != objCoupon.categories || objSubCoupon.items != objCoupon.items)
                            {
                                boolCouponError = true;
                                break;
                            }
                        }
                    }
                    // 比對商品使用是否正確
                    if (!boolCouponError)
                    {
                        numCouponAmount = 0;
                        listCoupon = listCoupon.Where(x => x.ItemId != null).ToList();
                        listTempCoupon = null;
                        foreach (TWNewEgg.Models.ViewModels.Cart.CartItemDetail_View objSubItemData in step1Data.CartItemDetailList_View)
                        {
                            listTempCoupon = listCoupon.Where(x => x.ItemId == objSubItemData.ItemID).ToList();
                            if (listTempCoupon == null || listTempCoupon.Count <= 0)
                            {
                                continue;
                            }
                            // 比對數量
                            if (listTempCoupon.Count > objSubItemData.Qty)
                            {
                                boolCouponError = true;
                                break;
                            }
                            // 比對使用類別與金額限制與Promotion限制
                            foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon objSubCoupon in listTempCoupon)
                            {
                                if (String.IsNullOrEmpty(objSubCoupon.categories) && String.IsNullOrEmpty(objSubCoupon.items))
                                {
                                    boolCouponError = true;
                                    break;
                                }
                                // 比對使用類別及品項
                                if (!String.IsNullOrEmpty(objSubCoupon.categories) && objSubCoupon.categories.Equals(";0;"))
                                {
                                    // 全館可用, 不比對
                                }
                                else if ((!String.IsNullOrEmpty(objSubCoupon.items) && objSubCoupon.items.IndexOf(";" + objSubItemData.ItemID.ToString().Trim() + ";") < 0) && (!String.IsNullOrEmpty(objSubCoupon.categories) && objSubCoupon.categories.IndexOf(";" + objSubItemData.Category.ToString().Trim() + ";") < 0))
                                {
                                    // 比對品項與類別
                                    boolCouponError = true;
                                    break;
                                }
                                // 比對使用金額限制
                                if (objSubCoupon.limit != null && objSubCoupon.limit > 0)
                                {
                                    if (objSubItemData.Price < objSubCoupon.limit)
                                    {
                                        boolCouponError = true;
                                        break;
                                    }
                                }
                                // 比對Promotion活動限制, 若有Promotion, 就不能用Coupon
                                if (promotionGiftDetailV2 != null && promotionGiftDetailV2.Count > 0)
                                {
                                    promotionGiftDetailV2.ForEach(x =>
                                    {
                                        int getItemID = 0;
                                        //getItemID = x.AcceptedItems.Where(y => y == objCoupon.ItemId).FirstOrDefault();
                                        getItemID = x.AcceptedItems.Where(y => y == objSubCoupon.ItemId).FirstOrDefault();
                                        if (getItemID != 0)
                                        {
                                            boolCouponError = true;
                                        }
                                    });
                                    if (boolCouponError)
                                    {
                                        break;
                                    }
                                }
                                // 若無錯誤會執行到此行
                                numCouponAmount += objSubCoupon.value;
                            }
                            // 有任何錯錯就跳出迴圈
                            if (boolCouponError)
                            {
                                break;
                            }
                        }
                    }

                    if (boolCouponError)
                    {
                        response = new ECWebResponse()
                        {
                            Status = (int)ECWebResponse.StatusCode.系統錯誤,
                            Message = "折價券使用數量有誤，請洽詢客服單位"
                        };
                        return Json(response);
                    }
                    else
                    {
                        _data = salesOrderService.SODataCombine(getIncreasePurchaseItemList.results, buyingItemPostData, getItemDisplayPriceList, account_id, getNumByDate);
                        // 將資料以_data規定的格式, 依序填入Coupon相關的資料
                        string[] aryBuyItemId = _data.item_id.Split(',');
                        string strCouponTempId = "";
                        string strCouponTempValue = "";
                        listTempCoupon = listCoupon;
                        int numTemp = 0;

                        foreach (string strItemId in aryBuyItemId)
                        {
                            Int32.TryParse(strItemId.Replace("'", "").Trim(), out numTemp);
                            objCoupon = listTempCoupon.Where(x => x.ItemId == numTemp).FirstOrDefault();
                            if (objCoupon != null)
                            {
                                strCouponTempId += objCoupon.id.ToString() + ",";
                                strCouponTempValue += objCoupon.value.ToString() + ",";
                                listTempCoupon.Remove(objCoupon);
                            }
                            else
                            {
                                strCouponTempId += ",";
                                strCouponTempValue += "0,";
                            }
                        }

                        _data.salesorderitems_pricecoupon = "'" + strCouponTempValue.TrimEnd(',') + "'";
                        _data.salesorderitems_coupons = "'" + strCouponTempId.TrimEnd(',') + "'";
                        _data.TotalCouponValue = numCouponAmount;
                        _data.CouponJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(listCoupon);
                    }
                }
                else
                {
                    _data = salesOrderService.SODataCombine(getIncreasePurchaseItemList.results, buyingItemPostData, getItemDisplayPriceList, account_id, getNumByDate);
                    _data.TotalCouponValue = 0;
                    _data.CouponJsonString = "";
                }
                #endregion
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(SO資料組合) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                if (e.Message == "商品價格錯誤請重新選擇商品")
                {
                    response = new ECWebResponse()
                    {
                        Status = (int)ECWebResponse.StatusCode.系統錯誤,
                        Message = "商品價格錯誤請重新選擇商品!"
                    };
                    return Json(response);
                }
                else
                {
                    response = new ECWebResponse()
                    {
                        Status = (int)ECWebResponse.StatusCode.系統錯誤,
                        Message = "因商品傳送資料有問題，請洽詢客服單位"
                    };
                    return Json(response);
                }
            }
            #endregion
            #region sendcheckout資料取得
            try
            {
                _data.buyingcartinfo = postData;
                sendcheckout.Clear(); //I Need This (Bill)
                logger.Info("[userEmail] [" + userEmail + "]" + "[_data.buyingcartinfo] " + postData);
                if (step1Data.CartTypeID != (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車)
                {
                    sendcheckout = FindBuyingCart(account_id.ToString(), postData, "False"); //I Need This (Bill)
                }
                else
                {
                    sendcheckout = FindBuyingCart(account_id.ToString(), postData, "True"); //I Need This (Bill)
                }

                if (sendcheckout == null)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail sendcheckout為空 請檢查 function FindBuyingCart 與SP");
                }
                else if (sendcheckout.Count == 0)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail sendcheckout List Count == 0");
                    response = new ECWebResponse()
                    {
                        Status = (int)ECWebResponse.StatusCode.系統錯誤,
                        Message = "商品已完售，請重新選擇商品!"
                    };
                    return Json(response);
                }
                else
                {
                    // 將加價購商品加入計算
                    if (getIncreasePurchaseItemList.results != null)
                    {
                        sendcheckout.AddRange(getIncreasePurchaseItemList.results);
                    }

                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail sendcheckout非空");
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    // 序列化
                    string szJson = jsonSerializer.Serialize(sendcheckout);
                    logger.Info("[userEmail] [" + userEmail + "]" + "sendcheckout info [" + szJson + "]");
                }
            }
            catch (Exception e)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail(購物車資訊收集) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }

            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail sendcheckout資料取得 end");
            #endregion
            #region 攔截美蛋沒有的商品-若因為變價產生的金額不一致問題，則執行總價化流程
            TWNewEgg.Website.ECWeb.Service.SalesOrderService.CheckNewEggResult checkNewEggResult = null;
            try
            {
                checkNewEggResult = salesOrderService.CheckNeweggStock(buyingItemPostData, sendcheckout, checkItemAndPriceTurnON);
            }
            catch (Exception e)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(攔截美蛋沒有的商品) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }

            if (checkNewEggResult != null && checkNewEggResult.EventFlag)
            {
                if (!string.IsNullOrWhiteSpace(checkNewEggResult.messageWSStatus))
                {
                    errorMessage = checkNewEggResult.messageWSStatus;
                }
                else if (!string.IsNullOrWhiteSpace(checkNewEggResult.messageQty))
                {
                    errorMessage = checkNewEggResult.messageQty;
                }
                else if (checkNewEggResult.EventFlag)
                {
                    errorMessage = checkNewEggResult.messagePrice;
                }
                else
                {
                    throw new Exception("因商品傳送資料有問題，請洽詢客服單位");
                }

                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.返回購物車頁,
                    Message = errorMessage
                };
                return Json(response);
            }
            #endregion 攔截美蛋沒有的商品
            #region 總金額確認-防止因為變價不完全產生的金額不一致問題，若發生不一致則執行總價化流程
            try
            {
                decimal getTotalPrice = this.GetTotalPrice(_data.salesorderitems_price);
                decimal getTotalShipping = this.GetTotalPrice(_data.salesorderitems_shippingexpense);
                decimal getTotalServicefees = this.GetTotalPrice(_data.salesorderitems_serviceexpense);
                decimal getTatalItemPriceSum = this.GetTotalPrice(_data.salesorderitems_itempricesum);
                decimal getDisplayPriceSum = this.GetTotalPrice(_data.salesorderitems_displayprice);
                // 防止因為變價不完全產生的金額不一致問題
                if (_data.pricesum != (getTotalPrice + getTotalShipping + getTotalServicefees) || _data.pricesum != getTatalItemPriceSum || getDisplayPriceSum != getTatalItemPriceSum)
                {
                    logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(總金額不一致，執行總價化)");
                    // 因總金額不一致故執行總價化流程
                    List<int> productIDList = itemList.Select(x => x.ProductID).ToList();
                    List<int> changePriceItemIDList = db_before.Item.Where(x => productIDList.Contains(x.ProductID)).Select(x => x.ID).ToList();
                    var getDisplayItemResult = Processor.Request<string, string>("ItemDisplayPriceService", "SetItemDisplayPriceByIDs", changePriceItemIDList);
                    if (getDisplayItemResult.error != null)
                    {
                        throw new Exception(getDisplayItemResult.error);
                    }

                    response = new ECWebResponse()
                    {
                        Status = (int)ECWebResponse.StatusCode.系統錯誤,
                        Message = "商品價格錯誤請重新選擇商品"
                    };
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(總金額確認) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion
            try
            {
                AutoMapper.Mapper.Map(data, _data);
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3 AutoMapper : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }

            int expire_year = 0;
            int.TryParse(data.expire_year, out expire_year);
            int expire_month = 0;
            int.TryParse(data.expire_month, out expire_month);
            _data.salesorder_cardexpire = (expire_year - 2000).ToString() + expire_month.ToString("00");
            // 檢查付款方式有無被使用者不當修改
            _data.salesorder_paytype = step1Data.CartPaytype_View.PayType0rateNum;
            Bank searchBank = (from p in db_before.Bank where p.ID == step1Data.CartPaytype_View.BankID select p).FirstOrDefault();
            PayType payTypeCheck = null;
            if (searchBank != null)
            {
                payTypeCheck = this.GetCurrentPatyType((PayType.nPayType)_data.salesorder_paytype, searchBank.ID);
            }
            else
            {
                payTypeCheck = this.GetCurrentPatyType((PayType.nPayType)_data.salesorder_paytype, 0);
            }

            if (payTypeCheck == null)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderDetail(付款選擇錯誤)");
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "付款選擇錯誤請重新選擇"
                };
                return Json(response);
            }

            _data.salesorder_cardbank = db_before.Bank.Where(x => x.ID == payTypeCheck.BankID).Select(x => x.Code).FirstOrDefault();
            _data.salesorder_paytypeid = payTypeCheck.ID;
            // 2015-07-15 暫時設定為新竹貨運宅配
            _data.DeliverCode = (int)TWNewEgg.DB.TWBACKENDDB.Models.Deliver.ShippingCompany.HCT新竹貨運;

            #region 利息計算
            InstallmentController installmentController = new InstallmentController();
            try
            {
                // 取得利息總額
                _data.insRateFees = installmentController.GetInsRateFee(_data.salesorder_paytypeid, _data.pricesum, _data.TotalCouponValue, getDiscountAmount);
            }
            catch (Exception e)
            {
                logger.Info("[UserEmail] [" + userEmail + "] " + "Step3(利息計算) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = "因商品傳送資料有問題，請洽詢客服單位"
                };
                return Json(response);
            }
            #endregion 利息計算
            #region HiLife超商取貨線上付款與線下付款條件判斷
            //TWNewEgg.Website.ECWeb.Service.O2OHiLifeService o2OHiLifeService = new TWNewEgg.Website.ECWeb.Service.O2OHiLifeService();
            //List<TWNewEgg.Website.ECWeb.Models.PayTypeDeliverInfo> payTypeDeliverInfoList = o2OHiLifeService.StoreDeliveryCheck(buyingItemPostData, _data.CouponJsonString, promotionGiftDetail);
            //ViewBag.payTypeDeliverInfoList = payTypeDeliverInfoList;
            //List<PayTypeDeliverInfo> storePayTypeDeliverList = payTypeDeliverInfoList.Where(x => x.PayType == (int)PayType.nPayType.超商付款).ToList();
            //List<int> notAllowStorePayTypeID = storePayTypeDeliverList.Where(x => x.DeliverWay.PickupByStore == false).Select(x => x.PayTypeID).ToList();
            #endregion
            #region 判斷該UID是否被Block住,若被Block住就導回購物車首頁
            // 累計總金額
            int cumulativePrice = 0;
            getItemDisplayPriceList.ForEach(x =>
            {
                int price = Convert.ToInt32(Math.Floor(x.DisplayPrice + 0.5m));
                int qty = step1Data.CartItemDetailList_View.Where(y => y.ItemID == x.ItemID).FirstOrDefault().Qty;
                int sumPrice = price * qty;
                cumulativePrice += sumPrice;
            });

            cumulativePrice = Convert.ToInt32(Math.Floor(0.5m + ((cumulativePrice - _data.TotalCouponValue - getDiscountAmount) * (1m + (decimal)payTypeCheck.InsRate))));
            if (OTPServiceGateway.ToLower() == "on")
            {
                if (payTypeCheck.Verification != 3
                    && cumulativePrice >= 10000
                    && (step1Data.CartPaytype_View.PayTypeGroupID == 1 || step1Data.CartPaytype_View.PayTypeGroupID == 2))
                {
                    TWNewEgg.Website.ECWeb.Service.OTPService otpService = new TWNewEgg.Website.ECWeb.Service.OTPService();
                    // 判斷該UID是否被Block住,若被Block住就導回購物車首頁
                    if (otpService.IsSMSBlock(account_id, cumulativePrice))
                    {
                        response = new ECWebResponse()
                        {
                            Status = (int)ECWebResponse.StatusCode.系統錯誤,
                            Message = "親愛的" + WebSiteData.Abbreviation + "顧客，您已錯誤輸入超過三次認證碼，您將於24小時內無法進行1萬元以上的購物，若還需購買一萬元以上商品，歡迎來電洽詢客服中心。"
                        };
                        return Json(response);
                    }
                }
            }
            #endregion 判斷該UID是否被Block住,若被Block住就導回購物車首頁
            SalesOrderDetailResult SODetailResult = SalesOrderDetail(getIncreasePurchaseItemList.results, _data, getItemDisplayPriceList, promotionGiftDetailV2);
            if (SODetailResult.State == "ok")
            {
                int soGroupId = SODetailResult.SOGroupId;
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.成功,
                    Data = soGroupId
                };
            }
            else if (SODetailResult.State == "notFinished")
            {
                int soGroupId = SODetailResult.SOGroupId;
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.轉頁付款,
                    Data = new { SOGroupId = soGroupId, PayType = SODetailResult.PaymentResult.Paytype, TradeMethod = SODetailResult.PaymentResult.TradeMethod }
                };
            }
            else
            {
                string msg = "";
                if (SODetailResult.PaymentResult != null)
                {
                    msg = SODetailResult.PaymentResult.SystemMessage;
                }
                else
                {
                    msg = SODetailResult.SystemMessage;
                }
                response = new ECWebResponse()
                {
                    Status = (int)ECWebResponse.StatusCode.系統錯誤,
                    Message = msg
                };
            }

            return Json(response);
        }

        #endregion

        /// <summary>
        /// 取得String所記錄的總金額
        /// </summary>
        /// <param name="strPrice">紀錄金額的String</param>
        /// <returns>返回累積總額</returns>
        private decimal GetTotalPrice(string strPrice)
        {
            decimal totalPrice = 0;
            if (!string.IsNullOrEmpty(strPrice))
            {
                strPrice.Replace("'", "").Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList().ForEach(x =>
                {
                    decimal getPrice = 0;
                    decimal.TryParse(x, out getPrice);
                    totalPrice += getPrice;
                });
            }

            return totalPrice;
        }

        private int GetPayTypeCount(List<PayType> checkPayType, int payTypeID)
        {
            int payTypeCount = 0;
            PayType searchPayType = null;
            if (checkPayType != null)
            {
                searchPayType = checkPayType.Where(x => x.ID == payTypeID).FirstOrDefault();
                if (searchPayType != null)
                {
                    payTypeCount = 1;
                }
            }

            return payTypeCount;
        }

        /// <summary>
        /// 結果頁
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="cartNumber"></param>
        /// <param name="Mode"></param>
        /// <returns顯示結果頁面></returns>
        public ActionResult Results(string orderNumber, string Mode, string cartNumber = "")
        {
            ResultsService callService = new ResultsService();
            TWSqlDBContext db_before = new TWSqlDBContext();
            string mainDomain = "";
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (environment == "DEV")
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["DEV_MainDomain"];
            }
            else
            {
                mainDomain = System.Configuration.ConfigurationManager.AppSettings["ECWeb_MainDomain"];
            }

            //JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            //List<TWNewEgg.ItemService.Models.BuyingItems> buyingItemPostData = new List<TWNewEgg.ItemService.Models.BuyingItems>();
            //TWNewEgg.Website.ECWeb.Service.SalesOrderService salesOrderService = new TWNewEgg.Website.ECWeb.Service.SalesOrderService();
            //TWNewEgg.Website.ECWeb.Models.SaleAndDelvTypeCheck salesDelvTypeCheck = null;
            //TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository promotionGift = new TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository();
            //List<GetItemTaxDetail> promotionGiftDetail = null;

            //CartStep1Data step1Data = jsSerializer.Deserialize<CartStep1Data>(data.step1Data);
            //Mapper.CreateMap<CartStep2Data, InsertSalesOrdersBySellerInput>();
            //InsertSalesOrdersBySellerInput totalData = ModelConverter.ConvertTo<InsertSalesOrdersBySellerInput>(data);
            // 取得List<BuyingItems>
            //buyingItemPostData = GetStrPostData(step1Data);
            // 取得PostData
            //string postData = Serialize(buyingItemPostData);
            #region 驗證登入狀態
            int accountId = NEUser.ID;
            //string[] plainText;

            //// Check Login Status.
            //if (Request.Cookies["Accountid"] != null && Request.Cookies["LoginStatus"] != null)
            //{
            //    IGetInfo checkID = new GetInfoRepository();
            //    plainText = checkID.Decoder(Request.Cookies["Accountid"].Value, false);
            //    if (plainText.Length < 2)
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }

            //    bool check = Int32.TryParse(plainText[0], out accountId); //Get accid .
            //    if (check == true)
            //    {
            //        //Set AccountID into this repository class.
            //        if (checkID.CheckAccount(accountId) == 1)
            //        {
            //            //return "account Error";
            //        }
            //        else
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            #endregion


            // 原result頁傳入值須修改成正常值
            //string orderNumber = string.Empty;
            //string cartNumber = string.Empty;
            //string Mode = string.Empty;
            // 原result頁傳入值須修改成正常值
            var clientIP = NEUser.IPAddress;

            if (cartNumber != null && cartNumber != "" && (orderNumber == "" || orderNumber == null))
            {
                int cartID = new int();
                bool cartFlag = int.TryParse(cartNumber, out cartID);
                using (var db = new TWSqlDBContext())
                {
                    var isNullorNot = db.SalesOrder.Where(x => x.SalesOrderGroupID == cartID).Select(x => x.Code).FirstOrDefault();
                    if (isNullorNot != null && isNullorNot != "")
                    {
                        orderNumber = isNullorNot;
                    }
                    else
                    {
                        orderNumber = "";
                    }
                }
            }

            List<ResultCart> result = new List<ResultCart>();
            result = SetResultCart(orderNumber, Mode, accountId);
            //bool removestauts = RemoveCartItem(Result, accountId);
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //if (clientIP != "0.0.0.0")
            //{
            //    var setIPFlag = SetSOIP(orderNumber, clientIP);
            //}

            if (Request.Cookies["itemnumber"] != null)
            {
                HttpCookie itemnumbercookie = new HttpCookie("itemnumber");
                itemnumbercookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(itemnumbercookie);
            }

            if (Request.Cookies["cartNumberDetail"] != null)
            {
                HttpCookie cartNumberDetailcookie = new HttpCookie("cartNumberDetail");
                cartNumberDetailcookie.Domain = mainDomain;
                cartNumberDetailcookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cartNumberDetailcookie);
            }

            //while (RemoveCartItem(result, accountId, plainText[1]) == false)
            //{
            //    //Do nothing;
            //}

            #region 根據購買品項取得Promotion及Coupon組成貼紙
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftBasic> listPromotionBasic = null;
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords> listPromotionRecords = null;
            List<string> listStrLbo = null;
            List<string> listStrLbs = null;
            List<SalesOrderItem> listItem = null;
            TWSqlDBContext objDb = null;
            List<int> listBasicId = null;
            List<int> listCouponId = null;
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon = null;
            List<string> listTemp = null;
            SalesOrderItem objSItem = null;
            TWNewEgg.Models.ViewModels.Redeem.Coupon objCoupon = null;
            TWNewEgg.Models.ViewModels.Redeem.PromotionGiftBasic objBasic = null;
            TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords objRecord = null;


            listStrLbo = new List<string>(result.Select(x => x.itemSONumber).ToList());
            if (listStrLbo != null)
            {
                objDb = new TWSqlDBContext();
                listItem = objDb.SalesOrderItem.Where(x => listStrLbo.Contains(x.SalesorderCode)).ToList();
                if (listItem != null && listItem.Count > 0)
                {
                    listStrLbs = listItem.Select(x => x.Code).ToList();
                    if (listStrLbs != null)
                    {
                        //查Promotion
                        //listPromotionRecords = objDb.PromotionGiftRecords.Where(x => listStrLbs.Contains(x.SalesOrderItemCode)).ToList();
                        listPromotionRecords = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftRecords>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>>("Service.PromotionGiftService.PromotionGiftRecordRepository", "GetGiftRecordsByListSalesOrderItemCode", listStrLbs).results;
                        if (listPromotionRecords != null && listPromotionRecords.Count > 0)
                        {
                            listBasicId = listPromotionRecords.Select(x => x.PromotionGiftBasicID).ToList();
                            //listPromotionBasic = objDb.PromotionGiftBasic.Where(x => listBasicId.Contains(x.ID)).ToList();
                            listPromotionBasic = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.PromotionGiftBasic>, List<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic>>("Service.PromotionGiftService.PromotionGiftRepository", "GetPromotionGiftBasicByIdList", listBasicId).results;
                        }

                        //查coupon
                        listTemp = listItem.Select(x => x.Coupons).ToList();
                        listCouponId = new List<int>();
                        foreach (string strItem in listTemp)
                        {
                            try
                            {
                                listCouponId.Add(Convert.ToInt32(strItem));
                            }
                            catch
                            {
                            }
                        }

                        if (listCouponId != null)
                        {
                            //listCoupon = objDb.Coupon.Where(x => listCouponId.Contains(x.id)).ToList();
                            listCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "GetCouponByIdList", listCouponId).results;
                        }

                    }
                }

                //整合所有result資訊
                foreach (ResultCart oSubResult in result)
                {
                    objSItem = listItem.Where(x => x.SalesorderCode == oSubResult.itemSONumber).FirstOrDefault();
                    if (objSItem == null)
                        continue;

                    objRecord = listPromotionRecords.Where(x => x.SalesOrderItemCode == objSItem.Code).FirstOrDefault();
                    if (objRecord != null)
                    {
                        objBasic = listPromotionBasic.Where(x => x.ID == objRecord.PromotionGiftBasicID).FirstOrDefault();
                        if (objBasic != null)
                        {
                            oSubResult.PromtionGiftString = "<div class='cartDiscountBox'><input class='" + objBasic.CSS + "' type='button' onclick='return false;' value='" + objBasic.ShowDesc + "'>";
                            if (objBasic.HighLight != null && objBasic.HighLight.Length > 0)
                            {
                                oSubResult.PromtionGiftString += "<div class='tips'>" + objBasic.HighLight + "</div>";
                            }
                            oSubResult.PromtionGiftString += "</div>";
                        }
                    }
                    else
                    {
                        if (objSItem.Coupons.Length > 0)
                        {
                            objCoupon = listCoupon.Where(x => x.id == Convert.ToInt32(objSItem.Coupons)).FirstOrDefault();
                            if (objCoupon != null)
                            {
                                oSubResult.CouponString = "<div class='cartDiscountBox'><div style='float:left; cursor:default;' class='iconB useCoupon'></div>活動折抵：$<span class='red'>" + Convert.ToString(objCoupon.value) + "</span></div>";
                            }
                        }
                    }
                }
                objDb.Dispose();

                listPromotionBasic = null;
                listPromotionRecords = null;
                listStrLbo = null;
                listStrLbs = null;
                listItem = null;
                objDb = null;
                listBasicId = null;
                listCouponId = null;
                listCoupon = null;
                listTemp = null;
                objSItem = null;
                objCoupon = null;
                objBasic = null;
                objRecord = null;
            }
            #endregion
            return View(result);
        }

        private string ReSetInputData(string inputData)
        {
            string resultStr = string.Empty;
            if (!string.IsNullOrEmpty(inputData))
            {
                resultStr = inputData.Replace("'", "");
                resultStr = resultStr.Replace("<apostrophe>", "''");
                //resultStr = resultStr.Replace(",", "repdot");
                //resultStr = resultStr.Replace("，", "repdot");
            }

            return "N'" + resultStr + "'";
        }

        /// <summary>
        /// 訂單組成頁面
        /// </summary>
        /// <param name="_data">產生訂單所需相關資料</param>
        /// <returns>重新導向其他設定頁面</returns>
        public SalesOrderDetailResult SalesOrderDetail(
            List<ShoppingCartItems> increasePurchaseItemList,
            InsertSalesOrdersBySellerInput _data,
            List<ItemDisplayPrice> itemDisplayPriceList,
            List<TWNewEgg.Models.ViewModels.Redeem.PromotionDetail> promotionGiftDetailV2)
        {
            string userEmail = NEUser.Email;
            int accountID = NEUser.ID;
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail start");
            SalesOrderDetailResult result = new SalesOrderDetailResult() { State = "ok" };
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<BuyingItems> buyingItemPostData = new List<BuyingItems>();
            SalesOrderService salesOrderService = new SalesOrderService();
            //PromotionGiftRepository promotionGift = new PromotionGiftRepository();
            TWNewEgg.Models.ViewModels.Cart.CashFlowResult objCashFlowResult = null;//金流交易結果的統一性訊息
            _data.salesorder_cardbank = _data.salesorder_cardbank.Trim();
            _data.salesorder_email = _data.salesorder_email.Trim();
            if (_data.pricesum <= 0)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + " SalesOrderDetail(商品價格錯誤 PriceSum <= 0)");
                result.SystemMessage = "商品價格錯誤請重新選擇商品!";
                result.State = "err";
                return result;
            }
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 驗證收件人地址是否包含不在允許範圍內的特殊符號、檢核收件人地址是否不符合設定規則、驗證統一編號是否是被禁止的");
            #region 驗證收件人地址是否包含不在允許範圍內的特殊符號、檢核收件人地址是否不符合設定規則、驗證統一編號是否是被禁止的
            AddressRestrictions checkingAddress = new AddressRestrictions();
            // 驗證收件人地址是否包含不在允許範圍內的特殊符號
            if (!checkingAddress.CheckingSpecialSymbols(_data.salesorder_delivloc, _data.salesorder_delivaddr, "-").FirstOrDefault().Key)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail(收件人地址包含不在規則內的特殊符號) [ " + _data.salesorder_delivloc + _data.salesorder_delivaddr + " ]");
                result.SystemMessage = "地址不可包含特殊符號，請重新填寫";
                result.State = "err";
                return result;

            }
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 檢核收件人地址是否不符合設定規則");
            // 檢核收件人地址是否不符合設定規則
            string addressChecking = checkingAddress.CheckingAddressKeyword(_data.salesorder_delivloc, _data.salesorder_delivaddr, "");
            if (addressChecking.Length > 0)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail(收件人地址不符合設定規則) [ " + _data.salesorder_delivloc + _data.salesorder_delivaddr + " ]");
                result.SystemMessage = addressChecking;
                result.State = "err";
                return result;
            }
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 驗證統一編號是否是被禁止的");
            // 驗證統一編號是否是被禁止的
            if (InvoidCheck(_data.salesorder_invoid))
            {
                result.SystemMessage = "資料錯誤請重新填寫";
                result.State = "err";
                return result;
            }
            #endregion
            //TWNewEgg.Logistics.HiLife.Service.HiLifeForNewEggAPI hiLifeNeweggApi = null; // 2015-07-15 超商暫時移除
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 付款方式");

            if (_data.salesorder_email == null)
            {
                _data.salesorder_email = userEmail;
            }

            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail [zip]" + _data.salesorder_cardzip);

            // 將收件人姓名、地址轉存到發票接收人姓名與地址
            _data.salesorder_invoreceiver = _data.salesorder_recvname;
            _data.salesorder_involoc = _data.salesorder_delivloc;
            _data.salesorder_invoaddr = _data.salesorder_delivaddr;
            _data.salesorder_cardzip = _data.salesorder_cardzip.Trim() + "00"; // 因為國外郵遞區號需要5碼
            _data.salesorder_delivzip = _data.salesorder_delivzip.Trim() + "00";
            _data.salesorder_invozip = _data.salesorder_delivzip;
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail ordernumber_temp");
            string ordernumber_temp = "";
            string salesorder_cardno = "";
            DateTime salesorder_expire = DateTime.Now;
            DbQuery oDb = null;
            SqlParameter[] arySqlParameter = null;
            DataSet dataSetResult = null;
            DataTable dataTableItem = null;
            List<InsertSalesOrdersBySellerOutput> send = new List<InsertSalesOrdersBySellerOutput>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            bool boolExec = false;
            //decimal pricesumcheck = 0;
            //ICarts repository = new CartsRepository();
            int numPromotionGiftAmount = 0;
            //bool flag = false;
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail get ItemIDList");
            List<int> ItemIDList = new List<int>();
            _data.item_id.Split(',').ToList().ForEach(x =>
            {
                int tempItemID = 0;
                int.TryParse(x, out tempItemID);
                ItemIDList.Add(tempItemID);
            });
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail ItemDisplayPriceresult");
            var ItemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", ItemIDList);
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 付款方式對錯確認");

            if (promotionGiftDetailV2.Count > 0)
            {
                promotionGiftDetailV2.ForEach(x =>
                {
                    numPromotionGiftAmount += Convert.ToInt32(Math.Floor(0.5m + x.ApportionedAmount));
                });
            }

            PayType paytypeInfo = db_before.PayType.Where(x => x.ID == _data.salesorder_paytypeid).FirstOrDefault();
            if (paytypeInfo == null)
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail: [ErrorMessage] 無法取得PayType資訊");
                result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                result.State = "err";
                return result;
            }
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail 簡訊驗證碼驗證");
            #region 簡訊驗證碼驗證
            if (OTPServiceGateway.ToLower() == "on")
            {
                // 利息
                decimal sumInsRateFees = Math.Floor(0.5m + (_data.pricesum - _data.TotalCouponValue - Convert.ToDecimal(numPromotionGiftAmount)) * ((decimal)paytypeInfo.InsRate));
                // 購物車扣除折價劵與活動折抵金額後再加上利息的總金額
                decimal priceSumCheck = _data.pricesum + sumInsRateFees - _data.TotalCouponValue - Convert.ToDecimal(numPromotionGiftAmount);
                OTPService otpService = new OTPService();
                if (paytypeInfo.Verification != 3
                    && priceSumCheck >= 10000
                    && !otpService.IsSMSSuccess(_data.salesorder_accountid)
                    && this.checkCredit(_data.salesorder_paytype))
                {
                    result.SystemMessage = "簡訊驗證錯誤";
                    result.State = "err";
                    return result;
                }
            }
            #endregion 簡訊驗證碼驗證
            //ViewBag.salesorder = _data;

            MakeData(_data);

            if (this._soFlowSelect.ToString() == "Parallel")
            {
                Thread newThread = new Thread(() => this.CreateSO(_data));
                newThread.Start();
            }
            else if (this._soFlowSelect.ToString() == "New")
            {
                send = this.CreateSO(_data);
            }

            if (this._soFlowSelect.ToString() != "New")
            {
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail SP InsertSalesOrdersBySeller資料組合");
                #region SP InsertSalesOrdersBySeller資料組合
                try
                {
                    oDb = new DbQuery();
                    arySqlParameter = new SqlParameter[87]; // 如果要增加參數則SqlParameter[2]、SqlParameter[3]...
                    arySqlParameter[0] = new SqlParameter("@instype", SqlDbType.Int);
                    arySqlParameter[0].Value = _data.instype;
                    arySqlParameter[1] = new SqlParameter("@item_id", SqlDbType.NVarChar);
                    arySqlParameter[1].Value = ReSetInputData(_data.item_id);
                    arySqlParameter[2] = new SqlParameter("@salesorderPrefix", SqlDbType.NVarChar);
                    arySqlParameter[2].Value = ReSetInputData(_data.salesorderPrefix);
                    arySqlParameter[3] = new SqlParameter("@salesorderitemPrefix", SqlDbType.NVarChar);
                    arySqlParameter[3].Value = ReSetInputData(_data.salesorderitemPrefix);
                    arySqlParameter[4] = new SqlParameter("@pricesum", SqlDbType.Int);
                    arySqlParameter[4].Value = Math.Floor(0.5m + _data.pricesum);
                    arySqlParameter[5] = new SqlParameter("@ordernum", SqlDbType.Int);
                    arySqlParameter[5].Value = _data.ordernum;
                    arySqlParameter[6] = new SqlParameter("@note", SqlDbType.NVarChar);
                    arySqlParameter[6].Value = ReSetInputData(_data.note);
                    arySqlParameter[7] = new SqlParameter("@item_attribid", SqlDbType.NVarChar);
                    arySqlParameter[7].Value = ReSetInputData(_data.item_attribid);
                    arySqlParameter[8] = new SqlParameter("@salesorder_telday", SqlDbType.NVarChar);
                    arySqlParameter[8].Value = ReSetInputData(_data.salesorder_telday);
                    arySqlParameter[9] = new SqlParameter("@salesorder_invoreceiver", SqlDbType.NVarChar);
                    arySqlParameter[9].Value = ReSetInputData(_data.salesorder_invoreceiver);
                    arySqlParameter[10] = new SqlParameter("@salesorder_invoid", SqlDbType.NVarChar);
                    arySqlParameter[10].Value = ReSetInputData(_data.salesorder_invoid);
                    arySqlParameter[11] = new SqlParameter("@salesorder_invotitle", SqlDbType.NVarChar);
                    arySqlParameter[11].Value = ReSetInputData(_data.salesorder_invotitle);
                    arySqlParameter[12] = new SqlParameter("@salesorder_involoc", SqlDbType.NVarChar);
                    arySqlParameter[12].Value = ReSetInputData(_data.salesorder_involoc);
                    arySqlParameter[13] = new SqlParameter("@salesorder_invozip", SqlDbType.NVarChar);
                    arySqlParameter[13].Value = ReSetInputData(_data.salesorder_invozip);
                    arySqlParameter[14] = new SqlParameter("@salesorder_invoaddr", SqlDbType.NVarChar);
                    arySqlParameter[14].Value = ReSetInputData(_data.salesorder_invoaddr);
                    arySqlParameter[15] = new SqlParameter("@salesorder_name", SqlDbType.NVarChar);
                    arySqlParameter[15].Value = ReSetInputData(_data.salesorder_name);
                    arySqlParameter[16] = new SqlParameter("@salesorder_paytypeid", SqlDbType.Int);
                    arySqlParameter[16].Value = _data.salesorder_paytypeid;
                    arySqlParameter[17] = new SqlParameter("@salesorder_paytype", SqlDbType.Int);
                    arySqlParameter[17].Value = _data.salesorder_paytype;
                    arySqlParameter[18] = new SqlParameter("@salesorder_email", SqlDbType.NVarChar);
                    arySqlParameter[18].Value = ReSetInputData(_data.salesorder_email);
                    arySqlParameter[19] = new SqlParameter("@salesorder_delivloc", SqlDbType.NVarChar);
                    arySqlParameter[19].Value = ReSetInputData(_data.salesorder_delivloc);
                    arySqlParameter[20] = new SqlParameter("@salesorder_delivzip", SqlDbType.NVarChar);
                    arySqlParameter[20].Value = ReSetInputData(_data.salesorder_delivzip);
                    arySqlParameter[21] = new SqlParameter("@salesorder_delivaddr", SqlDbType.NVarChar);
                    arySqlParameter[21].Value = ReSetInputData(_data.salesorder_delivaddr);
                    arySqlParameter[22] = new SqlParameter("@salesorder_delivengaddr", SqlDbType.NVarChar);
                    arySqlParameter[22].Value = ReSetInputData(_data.salesorder_delivengaddr);
                    arySqlParameter[23] = new SqlParameter("@salesorder_idno", SqlDbType.NVarChar);
                    arySqlParameter[23].Value = ReSetInputData(_data.salesorder_idno);
                    arySqlParameter[24] = new SqlParameter("@salesorder_mobile", SqlDbType.NVarChar);
                    arySqlParameter[24].Value = ReSetInputData(_data.salesorder_mobile);
                    arySqlParameter[25] = new SqlParameter("@salesorder_accountid", SqlDbType.Int);
                    arySqlParameter[25].Value = _data.salesorder_accountid;
                    arySqlParameter[26] = new SqlParameter("@salesorder_recvname", SqlDbType.NVarChar);
                    arySqlParameter[26].Value = ReSetInputData(_data.salesorder_recvname);
                    arySqlParameter[27] = new SqlParameter("@salesorder_recvengname", SqlDbType.NVarChar);
                    arySqlParameter[27].Value = ReSetInputData(_data.salesorder_recvengname);
                    arySqlParameter[28] = new SqlParameter("@salesorder_recvmobile", SqlDbType.NVarChar);
                    arySqlParameter[28].Value = ReSetInputData(_data.salesorder_recvmobile);
                    arySqlParameter[29] = new SqlParameter("@salesorder_recvtelday", SqlDbType.NVarChar);
                    arySqlParameter[29].Value = ReSetInputData(_data.salesorder_recvtelday);
                    arySqlParameter[30] = new SqlParameter("@salesorder_cardno", SqlDbType.NVarChar);
                    arySqlParameter[30].Value = "''";
                    arySqlParameter[31] = new SqlParameter("@salesorder_cardtype", SqlDbType.NVarChar);
                    arySqlParameter[31].Value = ReSetInputData(_data.salesorder_cardtype);
                    arySqlParameter[32] = new SqlParameter("@salesorder_cardbank", SqlDbType.NVarChar);
                    arySqlParameter[32].Value = ReSetInputData(_data.salesorder_cardbank);
                    arySqlParameter[33] = new SqlParameter("@salesorder_cardexpire", SqlDbType.NVarChar);
                    arySqlParameter[33].Value = "''";
                    arySqlParameter[34] = new SqlParameter("@salesorder_cardbirthday", SqlDbType.DateTime);
                    arySqlParameter[34].Value = _data.salesorder_cardbirthday.ToShortDateString();
                    arySqlParameter[35] = new SqlParameter("@salesorder_cardloc", SqlDbType.NVarChar);
                    arySqlParameter[35].Value = ReSetInputData(_data.salesorder_cardloc);
                    arySqlParameter[36] = new SqlParameter("@salesorder_cardzip", SqlDbType.NVarChar);
                    arySqlParameter[36].Value = ReSetInputData(_data.salesorder_cardzip.Trim());
                    arySqlParameter[37] = new SqlParameter("@salesorder_cardaddr", SqlDbType.NVarChar);
                    arySqlParameter[37].Value = ReSetInputData(_data.salesorder_cardaddr);
                    arySqlParameter[38] = new SqlParameter("@salesorder_status", SqlDbType.Int);
                    arySqlParameter[38].Value = _data.salesorder_status;
                    // 備註欄位
                    arySqlParameter[39] = new SqlParameter("@salesorders_note", SqlDbType.NVarChar);
                    //_data.salesorders_note = _data.note + _data.salesorders_note;
                    arySqlParameter[39].Value = ReSetInputData(_data.salesorders_note);
                    arySqlParameter[40] = new SqlParameter("@salesorders_delivtype", SqlDbType.NVarChar);
                    arySqlParameter[40].Value = ReSetInputData(_data.salesorders_delivtype);
                    arySqlParameter[41] = new SqlParameter("@salesorders_delivdata", SqlDbType.NVarChar);
                    arySqlParameter[41].Value = ReSetInputData(_data.salesorders_delivdata);
                    arySqlParameter[42] = new SqlParameter("@salesorder_remoteip", SqlDbType.NVarChar);
                    arySqlParameter[42].Value = ReSetInputData(_data.salesorder_remoteip);
                    arySqlParameter[43] = new SqlParameter("@salesorder_coservername", SqlDbType.NVarChar);
                    arySqlParameter[43].Value = ReSetInputData(_data.salesorder_coservername);
                    arySqlParameter[44] = new SqlParameter("@salesorder_servername", SqlDbType.NVarChar);
                    arySqlParameter[44].Value = ReSetInputData(_data.salesorder_servername);
                    arySqlParameter[45] = new SqlParameter("@salesorder_authcode", SqlDbType.NVarChar);
                    arySqlParameter[45].Value = ReSetInputData(_data.salesorder_authcode);
                    arySqlParameter[46] = new SqlParameter("@salesorder_authdate", SqlDbType.DateTime);
                    arySqlParameter[46].Value = _data.salesorder_authdate.ToShortDateString();
                    arySqlParameter[47] = new SqlParameter("@salesorder_authnote", SqlDbType.NVarChar);
                    arySqlParameter[47].Value = ReSetInputData(_data.salesorder_authnote);
                    arySqlParameter[48] = new SqlParameter("@salesorder_updateuser", SqlDbType.NVarChar);
                    arySqlParameter[48].Value = ReSetInputData(_data.salesorder_updateuser);
                    arySqlParameter[49] = new SqlParameter("@salesorders_itemname", SqlDbType.NVarChar);
                    arySqlParameter[49].Value = ReSetInputData(_data.salesorders_itemname);
                    arySqlParameter[50] = new SqlParameter("@salesorderitems_itemlistid", SqlDbType.NVarChar);
                    arySqlParameter[50].Value = ReSetInputData(_data.salesorderitems_itemlistid);
                    arySqlParameter[51] = new SqlParameter("@salesorderitems_qty", SqlDbType.NVarChar);
                    arySqlParameter[51].Value = ReSetInputData(_data.salesorderitems_qty);
                    // 到達時間欄位
                    arySqlParameter[52] = new SqlParameter("@salesorderitems_note", SqlDbType.NVarChar);
                    arySqlParameter[52].Value = ReSetInputData(_data.salesorderitems_note);
                    arySqlParameter[53] = new SqlParameter("@salesorderitems_price", SqlDbType.NVarChar);
                    arySqlParameter[53].Value = ReSetInputData(_data.salesorderitems_price);
                    arySqlParameter[54] = new SqlParameter("@salesorderitems_displayprice", SqlDbType.NVarChar); // 顯示單一價格
                    arySqlParameter[54].Value = ReSetInputData(_data.salesorderitems_displayprice);
                    arySqlParameter[55] = new SqlParameter("@salesorderitems_discountprice", SqlDbType.NVarChar); // 折扣金額
                    arySqlParameter[55].Value = ReSetInputData(_data.salesorderitems_discountprice);
                    arySqlParameter[56] = new SqlParameter("@salesorderitems_shippingexpense", SqlDbType.NVarChar); // 部分運費
                    arySqlParameter[56].Value = ReSetInputData(_data.salesorderitems_shippingexpense);
                    arySqlParameter[57] = new SqlParameter("@salesorderitems_serviceexpense", SqlDbType.NVarChar); // 部分服務費
                    arySqlParameter[57].Value = ReSetInputData(_data.salesorderitems_serviceexpense);
                    arySqlParameter[58] = new SqlParameter("@salesorderitems_tax", SqlDbType.NVarChar); // 部分稅金
                    arySqlParameter[58].Value = ReSetInputData(_data.salesorderitems_tax);
                    arySqlParameter[59] = new SqlParameter("@salesorderitems_itempricesum", SqlDbType.NVarChar); // 部分商品總額
                    arySqlParameter[59].Value = ReSetInputData(_data.salesorderitems_itempricesum);
                    arySqlParameter[60] = new SqlParameter("@salesorderitems_installmentfee", SqlDbType.NVarChar); // 部分商品利息
                    arySqlParameter[60].Value = ReSetInputData(_data.salesorderitems_installmentfee);
                    arySqlParameter[61] = new SqlParameter("@salesorderitems_priceinst", SqlDbType.NVarChar);
                    arySqlParameter[61].Value = ReSetInputData(_data.salesorderitems_priceinst);
                    arySqlParameter[62] = new SqlParameter("@salesorderitems_pricecoupon", SqlDbType.NVarChar);
                    arySqlParameter[62].Value = ReSetInputData(_data.salesorderitems_pricecoupon);
                    arySqlParameter[63] = new SqlParameter("@salesorderitems_coupons", SqlDbType.NVarChar);
                    arySqlParameter[63].Value = ReSetInputData(_data.salesorderitems_coupons);
                    arySqlParameter[64] = new SqlParameter("@salesorderitems_redmbln", SqlDbType.NVarChar);
                    arySqlParameter[64].Value = ReSetInputData(_data.salesorderitems_redmbln);
                    arySqlParameter[65] = new SqlParameter("@salesorderitems_redmtkout", SqlDbType.NVarChar);
                    arySqlParameter[65].Value = ReSetInputData(_data.salesorderitems_redmtkout);
                    arySqlParameter[66] = new SqlParameter("@salesorderitems_redmfdbck", SqlDbType.NVarChar);
                    arySqlParameter[66].Value = ReSetInputData(_data.salesorderitems_redmfdbck);
                    arySqlParameter[67] = new SqlParameter("@salesorderitems_wfbln", SqlDbType.NVarChar);
                    arySqlParameter[67].Value = ReSetInputData(_data.salesorderitems_wfbln);
                    arySqlParameter[68] = new SqlParameter("@salesorderitems_wftkout", SqlDbType.NVarChar);
                    arySqlParameter[68].Value = ReSetInputData(_data.salesorderitems_wftkout);
                    arySqlParameter[69] = new SqlParameter("@salesorderitems_actid", SqlDbType.NVarChar);
                    arySqlParameter[69].Value = ReSetInputData(_data.salesorderitems_actid);
                    arySqlParameter[70] = new SqlParameter("@salesorderitems_acttkout", SqlDbType.NVarChar);
                    arySqlParameter[70].Value = ReSetInputData(_data.salesorderitems_acttkout);
                    arySqlParameter[71] = new SqlParameter("@salesorderitems_isnew", SqlDbType.NVarChar);
                    arySqlParameter[71].Value = ReSetInputData(_data.salesorderitems_isnew);
                    arySqlParameter[72] = new SqlParameter("@itemlist_attribid", SqlDbType.NVarChar);
                    arySqlParameter[72].Value = ReSetInputData(_data.itemlist_attribid);
                    arySqlParameter[73] = new SqlParameter("@salesordergroupext_pscartid", SqlDbType.Int);
                    arySqlParameter[73].Value = _data.salesordergroupext_pscartid;
                    arySqlParameter[74] = new SqlParameter("@salesordergroupext_pssellerid", SqlDbType.NVarChar);
                    arySqlParameter[74].Value = ReSetInputData(_data.salesordergroupext_pssellerid);
                    arySqlParameter[75] = new SqlParameter("@salesordergroupext_pscarrynote", SqlDbType.NVarChar);
                    arySqlParameter[75].Value = ReSetInputData(_data.salesordergroupext_pscarrynote);
                    arySqlParameter[76] = new SqlParameter("@salesordergroupext_pshasact", SqlDbType.Int);
                    arySqlParameter[76].Value = _data.salesordergroupext_pshasact;
                    arySqlParameter[77] = new SqlParameter("@salesordergroupext_pshaspartialauth", SqlDbType.Int);
                    arySqlParameter[77].Value = _data.salesordergroupext_pshaspartialauth;
                    arySqlParameter[78] = new SqlParameter("@salesorderitemexts_psproductid", SqlDbType.NVarChar);
                    arySqlParameter[78].Value = ReSetInputData(_data.salesorderitemexts_psproductid);
                    arySqlParameter[79] = new SqlParameter("@salesorderitemexts_psmproductid", SqlDbType.NVarChar);
                    arySqlParameter[79].Value = ReSetInputData(_data.salesorderitemexts_psmproductid);
                    arySqlParameter[80] = new SqlParameter("@salesorderitemexts_psoriprice", SqlDbType.NVarChar);
                    arySqlParameter[80].Value = ReSetInputData(_data.salesorderitemexts_psoriprice);
                    arySqlParameter[81] = new SqlParameter("@salesorderitemexts_pssellcatid", SqlDbType.NVarChar);
                    arySqlParameter[81].Value = ReSetInputData(_data.salesorderitemexts_pssellcatid);
                    arySqlParameter[82] = new SqlParameter("@salesorderitemexts_psattribname", SqlDbType.NVarChar);
                    arySqlParameter[82].Value = ReSetInputData(_data.salesorderitemexts_psattribname);
                    arySqlParameter[83] = new SqlParameter("@salesorderitemexts_psmodelno", SqlDbType.NVarChar);
                    arySqlParameter[83].Value = ReSetInputData(_data.salesorderitemexts_psmodelno);
                    arySqlParameter[84] = new SqlParameter("@salesorderitemexts_pscost", SqlDbType.NVarChar);
                    arySqlParameter[84].Value = ReSetInputData(_data.salesorderitemexts_pscost);
                    arySqlParameter[85] = new SqlParameter("@salesorderitemexts_psfvf", SqlDbType.NVarChar);
                    arySqlParameter[85].Value = ReSetInputData(_data.salesorderitemexts_psfvf);
                    arySqlParameter[86] = new SqlParameter("@salesorderitemexts_psproducttype", SqlDbType.NVarChar);
                    arySqlParameter[86].Value = ReSetInputData(_data.salesorderitemexts_psproducttype);
                }
                catch (Exception e)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SP InsertSalesOrdersBySeller資料組合) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                    result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                    result.State = "err";
                    return result;
                }
                #endregion SP InsertSalesOrdersBySeller資料組合
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail SP InsertSalesOrdersBySeller執行");
                #region SP InsertSalesOrdersBySeller執行
                string temp = "", combine = "";
                try
                {
                    for (int i = 0; i < 87; i++)
                    {
                        if (i == 34 || i == 46)
                        {
                            temp += "'" + arySqlParameter[i].Value + "',";
                        }
                        else
                        {
                            temp = temp + arySqlParameter[i].Value + ",";
                        }
                    }

                    combine = "declare @outputstr nvarchar(max) set @outputstr='' exec [dbo].[UP_EC_InsertSalesOrdersBySellerV3] " + temp + "@outputstr out select @outputstr";
                    logger.Info("[userEmail] [" + userEmail + "]" + "[sp]" + combine);
                    //dsResult = oDb.Query("exec index_getitem @account_id", arySqlParameter);]
                    bool executeSP = false;
                    try
                    {
                        dataSetResult = oDb.Query(combine, arySqlParameter);
                        executeSP = true;
                    }
                    catch (Exception e)
                    {
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SP InsertSalesOrdersBySeller第一次失敗) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                    }
                    if (!executeSP)
                    {
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SP InsertSalesOrdersBySeller第二次執行)");
                        oDb = null;
                        dataSetResult = oDb.Query(combine, arySqlParameter);
                    }
                }
                catch (Exception e)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SP InsertSalesOrdersBySeller第二次失敗) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                    result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                    result.State = "err";
                    return result;

                }

                send.Clear();
                #endregion SP InsertSalesOrdersBySeller執行
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail SP InsertSalesOrdersBySeller資料取得");
                #region SP InsertSalesOrdersBySeller資料取得
                try
                {
                    if (dataSetResult != null && dataSetResult.Tables.Count > 0)
                    {
                        dataTableItem = dataSetResult.Tables[0];
                        int dataSetTableCount = dataSetResult.Tables.Count;  // 計算DataSet中DataTable的個數
                        int dataTableRowCount = dataSetResult.Tables[0].Rows.Count; // 計算DataTable中Tables[0]的Rows的個數
                        int rowCount = 0;
                        ViewBag.dsTcount = dataSetTableCount;
                        ViewBag.dsRcount = dataTableRowCount;
                        InsertSalesOrdersBySellerOutput[] sp = new InsertSalesOrdersBySellerOutput[dataTableRowCount];
                        foreach (DataRow dr in dataTableItem.Rows)
                        {
                            try
                            {
                                //----------public partial class salesorder----------//
                                sp[rowCount] = new InsertSalesOrdersBySellerOutput();
                                sp[rowCount].salesorder_code = Convert.ToString(dr["Code"]);
                                sp[rowCount].salesorder_salesordergroupid = Convert.ToInt32(dr["SalesOrderGroupID"]);
                                sp[rowCount].salesorder_idno = Convert.ToString(dr["IDNO"]);
                                sp[rowCount].salesorder_name = Convert.ToString(dr["Name"]);
                                sp[rowCount].salesorder_accountid = Convert.ToInt32(dr["AccountID"]);
                                sp[rowCount].salesorder_telday = Convert.ToString(dr["TelDay"]);
                                sp[rowCount].salesorder_telnight = Convert.ToString(dr["TelNight"]);
                                sp[rowCount].salesorder_mobile = Convert.ToString(dr["Mobile"]);
                                sp[rowCount].salesorder_email = Convert.ToString(dr["Email"]);
                                sp[rowCount].salesorder_paytypeid = Convert.ToInt32(dr["PayTypeID"]);
                                sp[rowCount].salesorder_paytype = Convert.ToInt32(dr["PayType"]);
                                //--------------------------------------//
                                DateTime dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["StarvlDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_starvldate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_cardholder = Convert.ToString(dr["CardHolder"]);
                                sp[rowCount].salesorder_cardtelday = Convert.ToString(dr["CardTelDay"]);
                                sp[rowCount].salesorder_cardtelnight = Convert.ToString(dr["CardTelNight"]);
                                sp[rowCount].salesorder_cardmobile = Convert.ToString(dr["CardMobile"]);
                                sp[rowCount].salesorder_cardloc = Convert.ToString(dr["CardLOC"]);
                                sp[rowCount].salesorder_cardzip = Convert.ToString(dr["CardZip"]);
                                sp[rowCount].salesorder_cardaddr = Convert.ToString(dr["CardADDR"]);
                                sp[rowCount].salesorder_cardno = Convert.ToString(dr["CardNo"]);
                                salesorder_cardno = Convert.ToString(dr["CardNo"]);
                                sp[rowCount].salesorder_cardnochk = Convert.ToString(dr["CardNochk"]);
                                sp[rowCount].salesorder_cardtype = Convert.ToString(dr["CardType"]);
                                sp[rowCount].salesorder_cardbank = Convert.ToString(dr["CardBank"]);
                                sp[rowCount].salesorder_cardexpire = Convert.ToString(dr["CardExpire"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["CardBirthday"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_cardbirthday = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_invoreceiver = Convert.ToString(dr["InvoiceReceiver"]);
                                sp[rowCount].salesorder_invoid = Convert.ToString(dr["InvoiceID"]);
                                sp[rowCount].salesorder_invotitle = Convert.ToString(dr["InvoiceTitle"]);
                                sp[rowCount].salesorder_involoc = Convert.ToString(dr["InvoiceLoc"]);
                                sp[rowCount].salesorder_invozip = Convert.ToString(dr["InvoiceZip"]);
                                sp[rowCount].salesorder_invoaddr = Convert.ToString(dr["InvoiceAddr"]);
                                sp[rowCount].salesorder_recvname = Convert.ToString(dr["RecvName"]);
                                sp[rowCount].salesorder_recvengname = Convert.ToString(dr["RecvEngName"]);
                                sp[rowCount].salesorder_recvtelday = Convert.ToString(dr["RecvTelDay"]);
                                sp[rowCount].salesorder_recvtelnight = Convert.ToString(dr["RecvTelNight"]);
                                sp[rowCount].salesorder_recvmobile = Convert.ToString(dr["RecvMobile"]);
                                sp[rowCount].salesorder_delivtype = Convert.ToByte(dr["DelivType"]);
                                sp[rowCount].salesorder_delivdata = Convert.ToString(dr["DelivData"]);
                                sp[rowCount].salesorder_delivloc = Convert.ToString(dr["DelivLOC"]);
                                sp[rowCount].salesorder_delivzip = Convert.ToString(dr["DelivZip"]);
                                sp[rowCount].salesorder_delivaddr = Convert.ToString(dr["DelivADDR"]);
                                sp[rowCount].salesorder_delivengaddr = Convert.ToString(dr["DelivEngADDR"]);
                                sp[rowCount].salesorder_delivhitnote = Convert.ToString(dr["DelivHitNote"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["ConfirmDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_confirmdate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_confirmnote = Convert.ToString(dr["ConfirmNote"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["AuthDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_authdate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_authcode = Convert.ToString(dr["AuthCode"]);
                                sp[rowCount].salesorder_authnote = Convert.ToString(dr["AuthNote"]);
                                int dint = 0;
                                try
                                {
                                    dint = Convert.ToInt32(dr["HpType"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_hptype = dint;
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["RcptDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_rcptdate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_rcptnote = Convert.ToString(dr["RcptNote"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["Expire"]);
                                    salesorder_expire = dt;
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_expire = dt;
                                //--------------------------------------//
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["DateDEL"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_datedel = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_coservername = Convert.ToString(dr["CoServerName"]);
                                sp[rowCount].salesorder_servername = Convert.ToString(dr["ServerName"]);
                                sp[rowCount].salesorder_actcode = Convert.ToString(dr["ActCode"]);
                                sp[rowCount].salesorder_status = Convert.ToByte(dr["Status"]);
                                sp[rowCount].salesorder_statusnote = Convert.ToString(dr["StatusNote"]);
                                sp[rowCount].salesorder_remoteip = Convert.ToString(dr["RemoteIP"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["Date"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_date = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_note = Convert.ToString(dr["Note"]);
                                sp[rowCount].salesorder_note2 = Convert.ToString(dr["Note2"]);
                                sp[rowCount].salesorder_createuser = Convert.ToString(dr["CreateUser"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["CreateDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_createdate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorder_updated = Convert.ToByte(dr["Updated"]);
                                sp[rowCount].salesorder_updateuser = Convert.ToString(dr["UpdateUser"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["UpdateDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorder_updatedate = dt;
                                //--------------------------------------//
                                //----------public partial class salesorderitem----------//
                                sp[rowCount].salesorderitem_code = Convert.ToString(dr["SalesorderItem_Code"]);
                                sp[rowCount].salesorderitem_salesordercode = Convert.ToString(dr["SalesorderItem_SalesorderCode"]);
                                sp[rowCount].salesorderitem_itemid = Convert.ToInt32(dr["SalesorderItem_ItemID"]);
                                sp[rowCount].salesorderitem_itemlistid = Convert.ToInt32(dr["SalesorderItem_ItemlistID"]);
                                sp[rowCount].salesorderitem_productid = Convert.ToInt32(dr["SalesorderItem_ProductID"]);
                                sp[rowCount].salesorderitem_productlistid = Convert.ToInt32(dr["SalesorderItem_ProductlistID"]);
                                sp[rowCount].salesorderitem_name = Convert.ToString(dr["SalesorderItem_Name"]);
                                sp[rowCount].salesorderitem_price = Convert.ToDecimal(dr["SalesorderItem_Price"]);
                                sp[rowCount].salesorderitem_priceinst = Convert.ToDecimal(dr["SalesorderItem_Priceinst"]);
                                sp[rowCount].salesorderitem_qty = Convert.ToInt32(dr["SalesorderItem_Qty"]);
                                sp[rowCount].salesorderitem_pricecoupon = Convert.ToDecimal(dr["SalesorderItem_Pricecoupon"]);
                                sp[rowCount].salesorderitem_coupons = Convert.ToString(dr["SalesorderItem_Coupons"]);
                                sp[rowCount].salesorderitem_redmtkout = Convert.ToInt32(dr["SalesorderItem_RedmtkOut"]);
                                sp[rowCount].salesorderitem_redmbln = Convert.ToInt32(dr["SalesorderItem_RedmBLN"]);
                                sp[rowCount].salesorderitem_redmfdbck = Convert.ToInt32(dr["SalesorderItem_Redmfdbck"]);
                                dint = 0;
                                try
                                {
                                    dint = Convert.ToByte(dr["SalesorderItem_Status"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_status = dint;
                                sp[rowCount].salesorderitem_statusnote = Convert.ToString(dr["SalesorderItem_StatusNote"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["SalesorderItem_Date"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_date = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorderitem_attribs = Convert.ToString(dr["SalesorderItem_Attribs"]);
                                sp[rowCount].salesorderitem_note = Convert.ToString(dr["SalesorderItem_Note"]);
                                sp[rowCount].salesorderitem_wftkout = Convert.ToInt32(dr["SalesorderItem_WftkOut"]);
                                sp[rowCount].salesorderitem_wfbln = Convert.ToInt32(dr["SalesorderItem_WfBLN"]);
                                dint = 0;
                                try
                                {
                                    dint = Convert.ToInt32(dr["SalesorderItem_AdjPrice"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_adjprice = dint;
                                sp[rowCount].salesorderitem_actid = Convert.ToString(dr["SalesorderItem_ActID"]);
                                sp[rowCount].salesorderitem_acttkout = Convert.ToInt32(dr["SalesorderItem_ActtkOut"]);
                                sp[rowCount].salesorderitem_isnew = Convert.ToString(dr["SalesorderItem_IsNew"]);
                                dint = 0;
                                try
                                {
                                    dint = Convert.ToInt32(dr["SalesorderItem_ProdcutCostID"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_prodcutcostid = dint;
                                sp[rowCount].salesorderitem_createuser = Convert.ToString(dr["SalesorderItem_CreateUser"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["SalesorderItem_CreateDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_createdate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorderitem_updated = Convert.ToByte(dr["SalesorderItem_Updated"]);
                                //--------------------------------------//
                                dt = DateTime.Parse("1990/01/01");
                                try
                                {
                                    dt = Convert.ToDateTime(dr["SalesorderItem_UpdateDate"]);
                                }
                                catch
                                {
                                }

                                sp[rowCount].salesorderitem_updatedate = dt;
                                //--------------------------------------//
                                sp[rowCount].salesorderitem_updateuser = Convert.ToString(dr["SalesorderItem_UpdateUser"]);
                                sp[rowCount].salesorderitem_displayprice = Convert.ToDecimal(dr["SalesorderItem_DisplayPrice"]); // 單一化價格
                                sp[rowCount].salesorderitem_discountprice = Convert.ToDecimal(dr["SalesorderItem_DiscountPrice"]); // 折扣金額
                                sp[rowCount].salesorderitem_shippingexpense = Convert.ToDecimal(dr["SalesorderItem_ShippingExpense"]); // 部分運費
                                sp[rowCount].salesorderitem_serviceexpense = Convert.ToDecimal(dr["SalesorderItem_ServiceExpense"]); // 部分服務費
                                sp[rowCount].salesorderitem_tax = Convert.ToDecimal(dr["SalesorderItem_Tax"]); // 部分稅金
                                sp[rowCount].salesorderitem_itempricesum = Convert.ToDecimal(dr["SalesorderItem_ItemPriceSum"]); // 部分商品總額
                                sp[rowCount].salesorderitem_installmentfee = Convert.ToDecimal(dr["SalesorderItem_InstallmentFee"]); // 部分利息
                                //sp[rowCount].salesorderitem_isnew = Convert.ToString(dr["SalesorderItem_IsNew"]);
                                //sp[rowCount].salesorderitem_apportionedamount = Convert.ToDecimal(dr["SalesorderItem_ApportionedAmount"]);
                                //----------public partial class salesorderitemext----------//
                                sp[rowCount].salesorderitemext_id = Convert.ToInt32(dr["SalesorderItemExt_ID"]);
                                sp[rowCount].salesorderitemext_salesorderitemcode = Convert.ToString(dr["SalesorderItemExt_SalesorderitemCode"]);
                                sp[rowCount].salesorderitemext_psproductid = Convert.ToString(dr["SalesorderItemExt_PsProductID"]);
                                sp[rowCount].salesorderitemext_psmproductid = Convert.ToString(dr["SalesorderItemExt_PsmProductID"]);
                                sp[rowCount].salesorderitemext_psoriprice = Convert.ToInt32(dr["SalesorderItemExt_PsoriPrice"]);
                                sp[rowCount].salesorderitemext_pssellcatid = Convert.ToString(dr["SalesorderItemExt_PsSellcatID"]);
                                sp[rowCount].salesorderitemext_psattribname = Convert.ToString(dr["SalesorderItemExt_PsAttribName"]);
                                sp[rowCount].salesorderitemext_psmodelno = Convert.ToString(dr["SalesorderItemExt_PsModelNO"]);
                                sp[rowCount].salesorderitemext_pscost = Convert.ToInt32(dr["SalesorderItemExt_PsCost"]);
                                sp[rowCount].salesorderitemext_psfvf = Convert.ToInt32(dr["SalesorderItemExt_Psfvf"]);
                            }
                            catch (Exception e)
                            {
                                logger.Info("[userEmail] [" + userEmail + "] [ErrorMsg]" + e.Message + "_:_" + e.StackTrace);
                                string str = "";
                                int index = 0;
                                while (true)
                                {
                                    try
                                    {
                                        str += dr[index++] + "__";
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }

                                logger.Info("[userEmail] [" + userEmail + "]" + str);
                                // 檢查 運費、服務費的Item Qty 是否不足
                                result.SystemMessage = "該商品數不足請重新選擇商品";
                                result.State = "err";
                                return result;
                            }
                            //ViewBag.sp = sp[_dsRcount];
                            send.Add(sp[rowCount]);
                            rowCount++;
                        } //end foreach
                    } //end if (dsResult != null && dsResult.Tables.Count > 0)
                    //send.First().salesorder_note = _data.note;
                    ViewBag.pricesum = Math.Floor(0.5m + _data.pricesum);
                    ViewBag.sd = send;
                }
                catch (Exception e)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SP InsertSalesOrdersBySeller資料取得) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                    result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                    result.State = "err";
                    return result;
                }
                #endregion SP InsertSalesOrdersBySeller資料取得
                oDb.Dispose();
            }
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(SalesOrder_Coupon金流前修改狀態) : [Email] " + _data.salesorder_email);
            #region SalesOrder_Coupon驗證
            if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON") && _data.CouponJsonString != null)
            {
                //修改LBS的Coupon狀態
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listTempCoupon = null;
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon = null;
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listEditCoupon = null;
                TWSqlDBContext objCouponDb = null;
                List<SalesOrderItem> listSalesOrderItem = null;
                TWNewEgg.Models.ViewModels.Redeem.Coupon objNowUseCoupon = null;
                int numSalesOrderGroupId = Convert.ToInt32(send.First().salesorder_salesordergroupid);

                listCoupon = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>>(_data.CouponJsonString);
                //listActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", NEUser.ID.ToString()).results;
                if (listCoupon != null && listCoupon.Count > 0)
                {
                    listTempCoupon = listCoupon.ToList();
                    objCouponDb = new TWSqlDBContext();
                    listSalesOrderItem = (from p in objCouponDb.SalesOrder
                                          join x in objCouponDb.SalesOrderItem on p.Code equals x.SalesorderCode
                                          where p.SalesOrderGroupID == numSalesOrderGroupId
                                          select x).ToList();

                    listEditCoupon = new List<TWNewEgg.Models.ViewModels.Redeem.Coupon>();
                    foreach (SalesOrderItem objSubItem in listSalesOrderItem)
                    {
                        objNowUseCoupon = listTempCoupon.Where(x => x.ItemId == objSubItem.ItemID).FirstOrDefault();
                        //設定LBO單
                        if (objNowUseCoupon == null)
                            continue;
                        objSubItem.Coupons = objNowUseCoupon.id.ToString();
                        objSubItem.Pricecoupon = Convert.ToDecimal(objNowUseCoupon.value);
                        //設定Coupon為暫時使用狀態
                        objNowUseCoupon.usestatus = (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.SetTempUsedForCheckout;
                        objNowUseCoupon.ordcode = objSubItem.Code;
                        listEditCoupon.Add(objNowUseCoupon);
                        listTempCoupon.Remove(objNowUseCoupon);
                    }

                    //一次更新Coupon狀態
                    Processor.Request<bool, bool>("Service.CouponService.CouponServiceRepository", "editCouponList", listEditCoupon);

                    //釋放記憶體
                    if (objCouponDb != null)
                    {
                        objCouponDb.Dispose();
                    }

                    //釋放Coupon相關的記憶體, 但不釋放listCoupon, 留給金流使用
                    listEditCoupon = null;
                    listTempCoupon = null;
                    listSalesOrderItem = null;
                    objNowUseCoupon = null;


                }
            }
            #endregion
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(超商取貨驗證) : [Email] " + _data.salesorder_email);
            #region 超商取貨驗證
            /*
            if (cartTypeCheck != (int)Item.tradestatus.三角 && cartTypeCheck != (int)Item.tradestatus.海外切貨) // 2015-07-15 超商暫時移除
            {
                #region 超商取貨驗證
                //if (_data.DeliverCode != (int)Deliver.ShippingCompany.HCT新竹貨運)
                //{
                //    O2OHiLifeService o2OHiLifeService = new O2OHiLifeService();
                //    List<PayTypeDeliverInfo> getStoreDeliveryList = o2OHiLifeService.StoreDeliveryCheck(buyingItemPostData, _data.CouponJsonString, promotionGiftDetail);
                //    PayTypeDeliverInfo searchPayTypeDeliverInfo = getStoreDeliveryList.Where(x =>
                //        x.PayTypeID == _data.salesorder_paytypeid
                //        && x.PayType == _data.salesorder_paytype
                //            //&& x.DeliverCode == _data.DeliverCode 
                //        && x.DeliverWay.PickupByStore == true
                //        && x.ConvenienceStore.HiLife == true).FirstOrDefault();
                //    if (searchPayTypeDeliverInfo == null)
                //    {
                //        logger.Info("SalesOrderDetail:(超商取貨驗證) : [ErrorMessage] 資料錯誤，HiLife取貨方式驗證錯誤! 訂單編號[" + send[0].salesorder_code + "]");
                //        message = "超商取貨驗證錯誤!";
                //        ViewBag.message = message;
                //        return RedirectToAction("index", "cart", new { message = message });
                //    }
                //}
                #endregion 超商取貨驗證
                logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(超商取貨資料儲存) : [Email] " + _data.salesorder_email);
                #region 超商取貨資料儲存
                //if (_data.salesorder_paytype == (int)PayType.nPayType.超商付款)
                //{
                #region HiLife
                if (_data.DeliverCode == (int)Deliver.ConvenienceStoreCode.HiLife)
                {
                    if (_data.storeStno.Length > 0 && _data.ConvenienceStoreName.Length > 0 && _data.storeRoute.Length > 0)
                    {
                        try
                        {
                            //    hiLifeNeweggApi = new Logistics.HiLife.Service.HiLifeForNewEggAPI();
                            //    List<string> salesOrderCodeList = send.Select(x => x.salesorder_code).Distinct().ToList();
                            //    foreach (var subSalesOrderCode in salesOrderCodeList)
                            //    {
                            //        string odNo = hiLifeNeweggApi.GetNewHiLifeNumber(subSalesOrderCode);
                            //        // HiLife訂單編號為11碼
                            //        if (odNo.Length == 11)
                            //        {
                            //            HiLifeOrderInfo hiLifeOrderInfoRecord = new HiLifeOrderInfo();
                            //            // Stno(取貨門市編號)
                            //            hiLifeOrderInfoRecord.StoreNumber = _data.storeStno;
                            //            // Stnm(取貨店名)
                            //            hiLifeOrderInfoRecord.StoreName = _data.ConvenienceStoreName;
                            //            // ODNO(EC訂單編號)
                            //            hiLifeOrderInfoRecord.ODNumber = odNo;
                            //            // DCRONO(路線路順)
                            //            hiLifeOrderInfoRecord.DirectRouteNO = _data.storeRoute;
                            //            // Prodnm(商品類型)(一般商品類型預設為0)
                            //            hiLifeOrderInfoRecord.ProductNumber = "0";
                            //            hiLifeOrderInfoRecord.CreateUser = "system";
                            //            hiLifeOrderInfoRecord.CreateDate = DateTime.Now;

                            //            db_before.HiLifeOrderInfo.Add(hiLifeOrderInfoRecord);
                            //        }
                            //        else
                            //        {
                            //            // 若無法正常取號則
                            //            logger.Info("SalesOrderDetail:(超商取貨資料儲存失敗) : [ErrorMessage] 無法正常取號! 訂單編號[" + send[0].salesorder_code + "]");
                            //            message = "超商取貨資訊錯誤!";
                            //            ViewBag.message = message;
                            //            return RedirectToAction("index", "cart", new { message = message });
                            //        }
                            //    }

                            //    db_before.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(超商取貨資料儲存失敗) : [ErrorMessage] " + e.Message + "[ErrorStackTrace] " + e.StackTrace + " 訂單編號[" + send[0].salesorder_code + "]");
                            result.SystemMessage = "超商取貨資訊錯誤!";
                            result.State = "err";
                            return result;
                        }
                    }
                    else
                    {
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(超商取貨資料儲存) : [ErrorMessage] 超商取貨資訊缺漏! 訂單編號[" + send[0].salesorder_code + "] StoreNumber:[" + _data.storeStno + "] StoreName:[" + _data.ConvenienceStoreName + "] DirectRouteNO:[" + _data.storeRoute + "]");
                        result.SystemMessage = "超商取貨資訊錯誤!";
                        result.State = "err";
                        return result;
                    }
                }
                #endregion
                #region SevenEleven
                else if (_data.DeliverCode == (int)Deliver.ConvenienceStoreCode.SevenEleven)
                {

                }
                #endregion
                #region FamilyMart
                else if (_data.DeliverCode == (int)Deliver.ConvenienceStoreCode.FamilyMart)
                {

                }
                #endregion
                #region OKMart
                else if (_data.DeliverCode == (int)Deliver.ConvenienceStoreCode.OKMart)
                {

                }
                #endregion
                //}
                #endregion
            }
            */
            #endregion
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(滿額贈折價加金額與符合滿額贈條件的賣場ID並執行滿額贈金額分攤至SalesOrderItems) : [Email] " + _data.salesorder_email);
            #region 滿額贈折價加金額與符合滿額贈條件的賣場ID並執行滿額贈金額分攤至SalesOrderItems
            if (promotionGiftDetailV2.Count > 0)
            {
                // 傳入購物車ID與商品資訊，會自動拆單更新SalesOrderItems並建立滿額贈的拆單記錄, 若拆單未成功, 
                //if (!promotionGift.CreatePromotionGiftRecordV2((int)send.First().salesorder_salesordergroupid, promotionGiftDetailV2, promotionGiftStatusTurnON))
                boolExec = Processor.Request<bool, bool>("Service.PromotionGiftService.PromotionGiftRepository", "CreatePromotionGiftRecordV2", (int)send.First().salesorder_salesordergroupid, promotionGiftDetailV2, promotionGiftStatusTurnON).results;
                if (!boolExec)
                {
                    logger.Info("[userEmail] [" + userEmail + "]" + "滿額折抵折價優惠-建立失敗");
                    result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                    result.State = "err";
                    return result;
                }
            }
            #endregion 滿額贈折價加金額與符合滿額贈條件的賣場ID並執行滿額贈金額分攤至SalesOrderItems
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(將Product中的SupplyShippingCharge insert 到 SalesOrderItem的SupplyShippingCharge欄位中) : [Email] " + _data.salesorder_email);
            #region 將Product中的SupplyShippingCharge insert 到 SalesOrderItem的SupplyShippingCharge欄位中
            this.InsertSupplyShippingCharge((int)send.First().salesorder_salesordergroupid);
            #endregion
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(利息分攤) : [Email] " + _data.salesorder_email);
            #region 利息分攤
            InstallmentController installmentController = new InstallmentController();
            InstallmentInfo saveInstallmentInfo = null;
            try
            {
                saveInstallmentInfo = installmentController.InstallmentInsert(Convert.ToInt32(send.First().salesorder_salesordergroupid));
            }
            catch (Exception e)
            {
                logger.Info("SalesOrderDetail(利息分攤錯誤) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
            }
            #endregion 利息分攤
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(儲存iChannels通路王需要的訂單資料) : [Email] " + _data.salesorder_email);
            #region 儲存iChannels通路王需要的訂單資料
            //var oeyaInfo = Request.Cookies.Get("OEYA");
            //if (oeyaInfo != null)
            //{
            //    TWNewEgg.OeyaIChannelsService.Service.OeyaIChannelsService oeyaService = new OeyaIChannelsService.Service.OeyaIChannelsService();
            //    send.ForEach(orderInfo =>
            //    {
            //        oeyaService.SaveOrderInfoToOeyaIChannelsOrderInfo(orderInfo.salesorderitem_salesordercode, Request.Cookies["OEYA"].Value);
            //    });
            //}
            #endregion
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(買一送一優惠訊息填入) : [Email] " + _data.salesorder_email);
            #region 買一送一優惠訊息填入
            //string commentInsertResult = promotionGift.SalesOrderComment(Convert.ToInt32(send.First().salesorder_salesordergroupid), buyingItemPostData.Select(x => x.buyItemID).ToList());
            string commentInsertResult = Processor.Request<string, string>("Service.PromotionGiftService.PromotionGiftRepository", "SalesOrderComment", Convert.ToInt32(send.First().salesorder_salesordergroupid), buyingItemPostData.Select(x => x.buyItemID).ToList()).results;
            if (commentInsertResult.Length > 0)
            {
                logger.Info("[userEmail] [" + userEmail + "] commentInsertResult" + commentInsertResult);
                result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                result.State = "err";
                return result;
            }
            #endregion 買一送一優惠訊息填入
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email);
            #region START 填入Invoice Data START
            salesOrderService.SettingItemInvoiceData(send, _data.invore3_2, _data.invoiceCarType, _data.invoiceCarCell, _data.invoiceCarNatu, _data.invoiceDonCode);
            #endregion END 填入Invoice Data END
            #region START 填入Item Category START
            salesOrderService.SettingItemCategory(send, _data.itemCategoey);
            #endregion END 填入Item Category END
            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) start : [Email] " + _data.salesorder_email);
            #region 金流
            string recvMessage = "";
            //string authrr = "";
            //string PayTypeName = "";
            HiTrust c = new HiTrust();
            object[] mm1 = new object[7];
            string mm = "";
            string authcode = "";
            int salesOrderPayType = 0;
            decimal numCouponValue = 0;
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
                {
                    numCouponValue = Convert.ToDecimal(_data.TotalCouponValue);
                }
                else
                {
                    numCouponValue = 0;
                }

                recvMessage = _data.salesorder_email + "," + _data.ArrivalTime;
                salesOrderPayType = (int)paytypeInfo.PayType0rateNum;
                payTypeGatewayTurnON = payTypeGatewayTurnON.ToLower();
                if (payTypeGatewayTurnON != "on")
                {
                    salesOrderPayType = (int)PayType.nPayType.電匯;
                }

                switch (salesOrderPayType)
                {
                    case (int)PayType.nPayType.實體ATM:
                        result.State = "notFinished";
                        objCashFlowResult = new CashFlowResult
                        {
                            Paytype = "AllPay",
                            TradeMethod = (int)CashFlowResult.TradeMethodOption.ATM,
                            TradeResult = true
                        };
                        logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用All Pay 實體ATM付款。");
                        this.AddSalesOrderCoupon(send[0].salesorder_salesordergroupid.Value, send[0].salesorder_accountid.ToString());
                        break;
                    //WebATM
                    case (int)PayType.nPayType.網路ATM:
                        ordernumber_temp = send.First().salesorder_code;
                        //中國信託WebATM
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue).ToString(), "", Chinatrust_txType.WebATM, "", "", "", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "013")
                        {
                            //國泰世華WebATM
                            TWNewEgg.Website.ECWeb.Controllers.WebATMController WebATM = new TWNewEgg.Website.ECWeb.Controllers.WebATMController();
                            CathayUnitedBank WATM = new CathayUnitedBank();
                            WATM.salesorder_code = send.First().salesorder_code;
                            WATM.ItemNo = "TWNE " + send.First().salesorder_code.Substring(9);
                            WATM.PurQuantity = 1;
                            WATM.amount = _data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount);

                            WebATM.WebATMConfirm(WATM);
                            //WebATM.
                            //return PartialView("Test", WATM);
                        }
                        //使用歐付寶接口的WebATM:台新、玉山、華南、台灣、富邦、第一、中信、國泰、兆豐、元大、土地
                        //else if (_data.salesorder_cardbank.Equals("812") || _data.salesorder_cardbank.Equals("808") || _data.salesorder_cardbank.Equals("008") || _data.salesorder_cardbank.Equals("004") || _data.salesorder_cardbank.Equals("012") || _data.salesorder_cardbank.Equals("007"))
                        else if (paytypeInfo.PayTypeCode == "10006")
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "AllPay",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.WebATM,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用All Pay Web ATM付款。");
                        }

                        break;
                    //信用卡-一般交易(一次付清)
                    case (int)PayType.nPayType.信用卡紅利折抵:
                        if (int.Parse(paytypeInfo.PayTypeCode) >= 20000 && int.Parse(paytypeInfo.PayTypeCode) < 30000)
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "NCCC",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用NCCC信用卡紅利折抵。");
                        }
                        break;
                    case (int)PayType.nPayType.信用卡一次付清:
                        //為了測試一次付清而設的AppSetting，若有值則使用該值代表的PaymentGateway
                        string gate = System.Configuration.ConfigurationManager.AppSettings["PPCredicOnceGate"];
                        if (gate != null)
                        {
                            paytypeInfo.PayTypeCode = gate;
                        }

                        //中國信託
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Normal, "", "", "", send.First().salesorder_createdate);
                            //return View("ChinatrustPay", new { OrderNo = send.First().salesorder_actcode.ToString(), AuthAmt = _data.pricesum.ToString(), BillshortDesc = "", txType = Chinatrust_txType.WebATM});
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:信用卡一次付清
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.信用卡一次付清, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //AllPay歐付寶接口:信用卡一次付清
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.信用卡一次付清, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("812"))
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_TaiShin",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用台新銀行信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("20000"))
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "NCCC",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用NCCC信用卡付款。");
                        }

                        break;
                    //信用卡-3期零利率
                    case (int)PayType.nPayType.三期零利率:
                        /* _data.salesorder_cardno =>信用卡號碼,
                         * 之後要改為輸入卡號即刻辨識
                         * 決定要走哪一個金流
                         */
                        //中國信託3期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "03", send.First().salesorder_createdate);
                        }
                        else if (int.Parse(paytypeInfo.PayTypeCode) >= 20000 && int.Parse(paytypeInfo.PayTypeCode) < 30000)
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "NCCC",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用NCCC信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_CitiBank",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用花旗銀行信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //AllPay歐付寶
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.三期零利率, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("812"))
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_TaiShin",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用台新銀行信用卡付款。");
                        }
                        break;
                    //信用卡-6期零利率
                    case (int)PayType.nPayType.六期零利率:
                        /* _data.salesorder_cardno =>信用卡號碼,
                         * 之後要改為輸入卡號即刻辨識
                         * 決定要走哪一個金流
                         */
                        //中國信託6期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "06", send.First().salesorder_createdate);
                        }
                        else if (int.Parse(paytypeInfo.PayTypeCode) >= 20000 && int.Parse(paytypeInfo.PayTypeCode) < 30000)
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "NCCC",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用NCCC信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:6期零利率
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_CitiBank",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用花旗銀行信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //歐付寶接口:6期零利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.六期零利率, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("812"))
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_TaiShin",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用台新銀行信用卡付款。");
                        }
                        break;
                    //信用卡-10期零利率
                    case (int)PayType.nPayType.十期零利率:
                        //中國信託10期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "10", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:10期零利率
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_CitiBank",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用花旗銀行信用卡付款。");
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //歐付寶接口:10期零利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十期零利率, null, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-12期零利率
                    case (int)PayType.nPayType.十二期零利率:
                        //中國信託12期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "12", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:12期零利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.十二期零利率, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //AllPay歐付寶接口:12期零利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十二期零利率, null, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-18期零利率
                    case (int)PayType.nPayType.十八期零利率:
                        //中國信託18期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "18", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:18期零利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.十八期零利率, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十八期零利率, null, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-24分期零利率
                    case (int)PayType.nPayType.二十四期零利率:
                        //中國信託24期零利率
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "24", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:24期零利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.二十四期零利率, null, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            //歐付寶接口: 24期零利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.二十四期零利率, null, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-10期分期
                    case (int)PayType.nPayType.十期分期:
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [10期分期]");
                        //中國信託10期分期
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum + saveInstallmentInfo.TotalInsRateFees - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "10", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:10期分期有利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.十期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [10期分期10006]");
                            //歐付寶接口:10期分期有利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-12期分期
                    case (int)PayType.nPayType.十二期分期:
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [12期分期]");
                        //中國信託12期分期
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum + saveInstallmentInfo.TotalInsRateFees - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "12", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:12期分期有利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.十二期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [12期分期10006]");
                            //歐付寶接口:12期分期有利率
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十二期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("812"))
                        {
                            result.State = "notFinished";
                            objCashFlowResult = new CashFlowResult
                            {
                                Paytype = "HiTrust_TaiShin",
                                TradeMethod = (int)CashFlowResult.TradeMethodOption.CredicCard,
                                TradeResult = true
                            };
                            logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用台新銀行信用卡付款。");
                        }
                        break;
                    //信用卡-18期分期
                    case (int)PayType.nPayType.十八期分期:
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [18期分期]");
                        //中國信託18期分期
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum + saveInstallmentInfo.TotalInsRateFees - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "18", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            //花旗接口:18期分期有利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.十八期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [18期分期10006]");
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.十八期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        break;
                    //信用卡-24分期有利率
                    case (int)PayType.nPayType.二十四期分期:
                        logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [24期分期]");
                        //中國信託24期分期
                        if (paytypeInfo.PayTypeCode == "822")
                        {
                            ChinatrustPay(send.First().salesorder_code.ToString(), (_data.pricesum + saveInstallmentInfo.TotalInsRateFees - numCouponValue - Convert.ToDecimal(numPromotionGiftAmount)).ToString(), "", Chinatrust_txType.Credic_Installments, "", "", "24", send.First().salesorder_createdate);
                        }
                        else if (paytypeInfo.PayTypeCode == "021")
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [24期分期" + _data.salesorder_cardbank + "]");
                            //花旗接口:24期分期有利率
                            objCashFlowResult = HitrustPay(ref send, ref _data, PayType.nPayType.二十四期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        else if (paytypeInfo.PayTypeCode.Equals("10006"))
                        {
                            logger.Info("[userEmail] [" + userEmail + "]" + "SalesOrderDetail:(金流) : [Email] " + _data.salesorder_email + " [24期分期" + _data.salesorder_cardbank + "]");
                            objCashFlowResult = AllPayCredic(ref send, ref _data, PayType.nPayType.二十四期分期, saveInstallmentInfo, numPromotionGiftAmount);
                        }
                        break;
                    // 貨到付款(因貨到付款只用於切貨商品所以不執行拋單)
                    case (int)PayType.nPayType.貨到付款:
                        //ViewBag.sd = send;
                        //return View("SalesOrderDetail", ViewBag.sd);
                        ordernumber_temp = send.First().salesorder_code;
                        //貨到付款 直接變更 訂單狀態 為 正常
                        using (DB.TWSqlDBContext dbContext = new DB.TWSqlDBContext())
                        {
                            var so = dbContext.SalesOrder.Where(x => x.Code == ordernumber_temp).FirstOrDefault();
                            if (so != null)
                            {
                                var salesOrderList = dbContext.SalesOrder.Where(x => x.SalesOrderGroupID == so.SalesOrderGroupID).ToList();
                                foreach (var s in salesOrderList)
                                {
                                    s.Status = (int)DB.TWSQLDB.Models.SalesOrder.status.付款成功;
                                }

                                dbContext.SaveChanges();

                                //是否使用Coupon券
                                if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
                                {
                                    string str31Account = "";
                                    int n31SalesOrderGroupId = 0;
                                    try
                                    {
                                        str31Account = Convert.ToString(so.AccountID);
                                        n31SalesOrderGroupId = Convert.ToInt32(so.SalesOrderGroupID);
                                    }
                                    catch
                                    {
                                        str31Account = "0";
                                        n31SalesOrderGroupId = 0;
                                    }

                                    this.AddSalesOrderCoupon(n31SalesOrderGroupId, str31Account);
                                }

                                //end if 是否使用Coupon券
                            }
                        }
                        objCashFlowResult = new CashFlowResult();
                        objCashFlowResult.TradeMethod = (int)CashFlowResult.TradeMethodOption.貨到付款;
                        objCashFlowResult.TradeResult = true;
                        //return RedirectToAction("Results", new { ordernumber = ordernumber_temp, Mode = recvMessage });
                        break;
                    // 超商付款
                    case (int)PayType.nPayType.超商付款:
                        //ViewBag.sd = send;
                        //return View("SalesOrderDetail", ViewBag.sd);
                        ordernumber_temp = send.First().salesorder_code;
                        //貨到付款 直接變更 訂單狀態 為 正常
                        using (DB.TWSqlDBContext dbContext = new DB.TWSqlDBContext())
                        {
                            var so = dbContext.SalesOrder.Where(x => x.Code == ordernumber_temp).FirstOrDefault();
                            if (so != null)
                            {
                                var salesOrderList = dbContext.SalesOrder.Where(x => x.SalesOrderGroupID == so.SalesOrderGroupID).ToList();
                                foreach (var s in salesOrderList)
                                {
                                    s.Status = (int)DB.TWSQLDB.Models.SalesOrder.status.付款成功;
                                }

                                dbContext.SaveChanges();
                                //是否使用Coupon券
                                if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
                                {
                                    string str31Account = "";
                                    int n31SalesOrderGroupId = 0;
                                    try
                                    {
                                        str31Account = Convert.ToString(so.AccountID);
                                        n31SalesOrderGroupId = Convert.ToInt32(so.SalesOrderGroupID);
                                    }
                                    catch
                                    {
                                        str31Account = "0";
                                        n31SalesOrderGroupId = 0;
                                    }

                                    this.AddSalesOrderCoupon(n31SalesOrderGroupId, str31Account);
                                }

                                //end if 是否使用Coupon券
                            }
                        }
                        objCashFlowResult = new CashFlowResult();
                        objCashFlowResult.TradeMethod = (int)CashFlowResult.TradeMethodOption.超商付款;
                        objCashFlowResult.TradeResult = true;
                        break;
                    //歐付寶
                    case (int)PayType.nPayType.歐付寶儲值支付:
                        result.State = "notFinished";
                        objCashFlowResult = new CashFlowResult
                        {
                            Paytype = "AllPay",
                            TradeMethod = (int)CashFlowResult.TradeMethodOption.儲值支付,
                            TradeResult = true
                        };
                        logger.Info("SalseOrderGroup ID:" + send[0].salesorder_salesordergroupid + "，使用All Pay儲值支付。");
                        break;
                    // 預設值(貨到付款模式但執行拋單) & 電匯
                    case (int)PayType.nPayType.電匯:
                        //ViewBag.sd = send;
                        //return View("SalesOrderDetail", ViewBag.sd);
                        ordernumber_temp = send.First().salesorder_code;
                        //貨到付款 & 電匯 直接變更 訂單狀態 為 正常
                        using (DB.TWSqlDBContext dbContext = new DB.TWSqlDBContext())
                        {
                            var so = dbContext.SalesOrder.Where(x => x.Code == ordernumber_temp).FirstOrDefault();
                            if (so != null)
                            {
                                var salesOrderList = dbContext.SalesOrder.Where(x => x.SalesOrderGroupID == so.SalesOrderGroupID).ToList();
                                TWNewEgg.Website.ECWeb.Service.PlaceOrder po = new TWNewEgg.Website.ECWeb.Service.PlaceOrder();
                                List<string> needReSendList = new List<string>();
                                foreach (SalesOrder a2 in salesOrderList)
                                {
                                    try
                                    {
                                        //如果產生海外切貨DelvType=6的訂單，通知營服主管
                                        SpexService.SendMail(a2.Code);
                                        string purchaseOrderCode = po.SendPlaceOrder((int)a2.SalesOrderGroupID, a2.Code);
                                        a2.Status = (int)DB.TWSQLDB.Models.SalesOrder.status.付款成功; //  貨到付款是否要新增不同的status?來跟其他的已付款的status做區隔?
                                        dbContext.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        a2.Status = (int)SalesOrder.status.付款成功拋單失敗;
                                        dbContext.SaveChanges();
                                        //MyNeweggController MyNewegg = new MyNeweggController();
                                        //MyNewegg.resetpost(a2.Code, 4, "交易失敗", "", "", "", true); // 因為會寄信通知工程師此問題，所以取消訂單動作暫時拿掉
                                        // 新增需要重新拋送的訂單編號
                                        needReSendList.Add(a2.Code);
                                        logger.Error("[userEmail] [" + userEmail + "]" + a2.Code + ":" + e.Message + "_:_" + e.StackTrace);
                                    }
                                }

                                if (needReSendList.Count > 0)
                                {
                                    SOReSendMail(needReSendList); // 寄發訂單失敗通知信
                                }

                                //是否使用Coupon券
                                if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
                                {
                                    string str99AccountId = "";
                                    int n99SalesOrderGroupId = 0;
                                    try
                                    {
                                        str99AccountId = Convert.ToString(so.AccountID);
                                        n99SalesOrderGroupId = Convert.ToInt32(so.SalesOrderGroupID);
                                    }
                                    catch
                                    {
                                        str99AccountId = "0";
                                        n99SalesOrderGroupId = 0;
                                    }

                                    this.AddSalesOrderCoupon(n99SalesOrderGroupId, str99AccountId);
                                }
                                //end if 是否使用Coupon券
                            }
                        }
                        objCashFlowResult = new CashFlowResult();
                        objCashFlowResult.TradeMethod = (int)CashFlowResult.TradeMethodOption.電匯;
                        objCashFlowResult.TradeResult = true;
                        //return RedirectToAction("Results", new { ordernumber = ordernumber_temp, Mode = recvMessage });
                        break;
                }
            }
            catch (Exception e)
            {
                logger.Info("SalesOrderDetail:(金流) : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                //result.SystemMessage = "因商品傳送資料有問題，請洽詢客服單位";
                //result.State = "err";
                //return result;
            }
            #endregion 金流
            if (oDb != null) { oDb.Dispose(); }

            logger.Info("SalesOrderDetail:(End) : [Email] " + _data.salesorder_email);

            var itemDate = send.First().salesorder_createdate;
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];

            //if (objCashFlowResult.TradeResult == true)
            //{
            //    if (environment == "PRD")
            //    {
            //        if (itemDate != null)
            //        {
            //            var timeDiff = itemDate.Subtract(DateTime.UtcNow.AddHours(8)).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
            //            if (timeDiff.Minutes < 5)
            //            {
            //                var smsFlag = SendSMS(send.First().salesorder_mobile, send.First().salesorder_code);
            //            }
            //        }
            //    }
            //    try
            //    {
            //        MailManageController mailManage = new MailManageController(this.ControllerContext);
            //        mailManage.DealSuccess(send); // 寄發訂單成功通知信
            //    }
            //    catch (Exception e)
            //    {
            //        logger.Error("[userEmail] [" + userEmail + "]" + "Mail Error:" + e.ToString());
            //    }
            //}

            if (objCashFlowResult != null && objCashFlowResult.TradeResult == false)
            {
                result.State = "err";
                result.PaymentResult = objCashFlowResult;
            }
            else
            {
                result.PaymentResult = objCashFlowResult;
                result.SOGroupId = send[0].salesorder_salesordergroupid.Value;
            }

            return result;
        }

        private void MakeData(InsertSalesOrdersBySellerInput _data)
        {
            _data.salesorders_note = _data.salesorders_note.Replace("'", "");
            if (_data.note != null)
            {
                _data.note = _data.note.Replace(",", "repdot");
                _data.note = _data.note.Replace("，", "repdot");
            }
            if (_data.salesorder_delivengaddr != null)
            {
                _data.salesorder_delivengaddr = _data.salesorder_delivengaddr.Replace(",", "repdot");
                _data.salesorder_delivengaddr = _data.salesorder_delivengaddr.Replace("，", "repdot");
            }
            if (_data.salesorders_note.Split(',').Length <= 0)
            {
                _data.salesorders_note += _data.note;
            }
            else
            {
                bool isContainA = _data.salesorders_note.IndexOf("國際運費", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isContainB = _data.salesorders_note.IndexOf("服務費", StringComparison.OrdinalIgnoreCase) >= 0;
                if (isContainA == true || isContainB == true)
                {
                    _data.salesorders_note = _data.salesorders_note.Replace("國際運費,", "國際運費||");
                    _data.salesorders_note = _data.salesorders_note.Replace("服務費,", "服務費||");
                    _data.salesorders_note = _data.salesorders_note.Replace(",", _data.note + ",");
                    _data.salesorders_note = _data.salesorders_note.Replace("||", ",");
                }
                else
                {
                    _data.salesorders_note = _data.salesorders_note.Replace(",", _data.note + ",");
                    _data.salesorders_note = _data.salesorders_note + _data.note;
                }
            }
            _data.salesorderitems_note = _data.salesorderitems_note.Replace("'", "");
            if (_data.salesorderitems_note.Split(',').Length <= 0)
            {
                _data.salesorderitems_note += _data.ArrivalTime + "</arrive>";
            }
            else
            {
                bool isContainA = _data.salesorderitems_note.IndexOf("國際運費", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isContainB = _data.salesorderitems_note.IndexOf("服務費", StringComparison.OrdinalIgnoreCase) >= 0;
                if (isContainA == true || isContainB == true)
                {
                    _data.salesorderitems_note = _data.salesorderitems_note.Replace("國際運費,", "國際運費||");
                    _data.salesorderitems_note = _data.salesorderitems_note.Replace("服務費,", "服務費||");
                    _data.salesorderitems_note = _data.salesorderitems_note.Replace(",", _data.ArrivalTime + "</arrive>" + ",");
                    _data.salesorderitems_note = _data.salesorderitems_note.Replace("||", ",");
                }
                else
                {
                    _data.salesorderitems_note = _data.salesorderitems_note.Replace(",", _data.ArrivalTime + "</arrive>" + ",");
                    _data.salesorderitems_note = _data.salesorderitems_note + _data.ArrivalTime + "</arrive>";
                }
            }
        }

        private List<InsertSalesOrdersBySellerOutput> CreateSO(InsertSalesOrdersBySellerInput _data)
        {
            var createSOInfoResult = Processor.Request<List<InsertSalesOrdersBySellerOutput>, List<DemainInsertSalesOrdersBySellerOutput>>("SOServices.SalesOrderInfoService", "CreateSoInfo", _data, _soFlowSelect);
            if (createSOInfoResult.error != null)
            {
                logger.Error(createSOInfoResult.error);
                if (this._soFlowSelect == "New")
                {
                    throw new Exception(createSOInfoResult.error);
                }
            }

            return createSOInfoResult.results;
        }

        private bool SetSOIP(int soGroupID, string clientIP)
        {
            bool flag = false;
            TWSqlDBContext db = new TWSqlDBContext();
            var allSO = db.SalesOrder.Where(x => x.SalesOrderGroupID == soGroupID).ToList();
            foreach (var singleSO in allSO)
            {
                singleSO.RemoteIP = clientIP;
            }
            try
            {
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }

        public List<ResultCart> SetResultCart(string orderNumber, string recvMessage, int? accountID)
        {
            string userEmail = NEUser.Email;
            bool isOverSea = false;
            ResultsService callService = new ResultsService();
            TWSqlDBContext db = new TWSqlDBContext();

            Dictionary<string, int> rowSpan = new Dictionary<string, int>();
            List<InsertSalesOrdersBySellerOutput> resultsOri = callService.GetSalesOrders(orderNumber); //Get Stored procedure return data from DB.
            List<decimal> interShip = new List<decimal>();
            List<ResultCart> result = new List<ResultCart>();
            List<string> allSOItemCodes = new List<string>();

            if (resultsOri == null || resultsOri.Count == 0)
            {
                result = null;
                return result;
            }

            if (resultsOri.FirstOrDefault().salesorder_accountid != accountID.Value)
            {
                result = null;
                return result;
            }

            resultsOri.Remove(resultsOri.Where(x => x.salesorder_note == "國際運費").FirstOrDefault()); //find cart's International shipping cost.
            resultsOri.Remove(resultsOri.Where(x => x.salesorder_note == "服務費").FirstOrDefault()); //find cart's International shipping cost.
            //if (ResultsOri.First().salesorder_delivtype == 3) //check is the order from Oversea.
            //{
            //    isOverSea = true;
            //}
            allSOItemCodes = resultsOri.Select(x => x.salesorderitem_code).ToList();
            isOverSea = callService.GetIsOverSeaByDelvType(resultsOri.First().salesorder_delivtype); //check is the order from Oversea.

            Int32? payType = new int();
            string cardBank = resultsOri[0].salesorder_cardbank;
            if (resultsOri.Count != 0)
            {
                payType = resultsOri[0].salesorder_paytype; //Find buying type.
                cardBank = db.Bank.Where(x => x.Code == cardBank).Select(y => y.Name).FirstOrDefault(); //Find buying type.
                var payTypeName = db.PayType.Where(x => x.PayType0rateNum == payType).Select(x => x.Name).FirstOrDefault();
                SetPayTypeText(payType, cardBank, payTypeName); //and set the variable to View (ex: WebATM or Credit Card).
            }

            List<BuyingItems> resultItems = new List<BuyingItems>();

            var allSalesOrder = resultsOri.Select(x => new { x.salesorder_code, x.salesorderitem_itemid }).Distinct().ToList(); //Get and find how many Seller IDs in this cart.
            List<int> allSalesOrderItemID = new List<int>();

            //allSalesOrderItemID = ResultsOri.Where(x => allSalesOrderCodes.Contains(x.salesorder_code)).Select(y => y.salesorderitem_itemid).Distinct().ToList();
            allSalesOrderItemID = allSalesOrder.Select(x => x.salesorderitem_itemid).Distinct().ToList(); //Get and Find all itemIDs in one SellerID.

            var itemIDSellerID = db.Item.Where(y => allSalesOrderItemID.Contains(y.ID)).Select(z => new { item_sellerid = z.SellerID, item_id = z.ID }).ToList(); //Find SellerID and which's CountryID.
            Dictionary<string, int> itemcategoryList = new Dictionary<string, int>();
            foreach (var itemIDS in allSalesOrderItemID)
            {
                int categoryIDS = db.Item.Where(x => x.ID == itemIDS).Select(x => x.CategoryID).FirstOrDefault();
                itemcategoryList.Add(itemIDS.ToString(), categoryIDS);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();

            ViewBag.category = jss.Serialize(itemcategoryList);
            List<int> allSellerID = itemIDSellerID.Select(x => x.item_sellerid).ToList(); //Read all SellerID.
            List<Seller> allSellers = db.Seller.Where(x => allSellerID.Contains(x.ID)).ToList(); //Get all SellerID's Detail Data.
            List<Nullable<int>> allCountryID = allSellers.Select(y => y.CountryID).ToList(); //Read all CountryID.
            List<Country> allCountries = db.Country.Where(x => allCountryID.Contains(x.ID)).ToList(); //Get all CountryID's Detail Data.
            //Caculate all item in each Seller.
            foreach (var sellerID in allSellers)
            {
                List<int> sameSellerItem = itemIDSellerID.Where(x => x.item_sellerid == sellerID.ID).Select(y => y.item_id).ToList(); //Find all items which in same seller.
                List<string> sameSellerCode = allSalesOrder.Where(x => sameSellerItem.Contains(x.salesorderitem_itemid)).Select(y => y.salesorder_code).ToList(); //Find the items in same seller's salesorder_code.
                List<InsertSalesOrdersBySellerOutput> resultsTemp = resultsOri.Where(x => sameSellerCode.Contains(x.salesorder_code)).ToList(); //Using salesorder_code to Find Result's Item.
                callService.SetInResults(resultsTemp, result, sameSellerCode, sellerID, rowSpan, resultItems, allCountries); //Start to Caculate and find this Result's Data.
            }

            /*
            foreach(var salesOrderCode in allSalesOrderCodes)
            {

                List<InsertSalesOrdersBySellerOutput> ResultsTemp = GetSalesOrders(orderNumber);
                ResultsTemp.AddRange(GetSameOrderCode(salesOrderCode, ResultsOri));

            }
            */
            //ViewBag.SalesOrders = ResultsOri;
            ICarts countShipping = new CartsRepository();
            TWNewEgg.ItemService.Models.ShipTaxService taxShipService = new TWNewEgg.ItemService.Models.ShipTaxService();
            if (isOverSea == true)
            {
                //TaxShipService = CountShipping.ShippingCosts(ResultItems, "True", "index"); //Call and Get oversea's product Local price, Tax and service fee from stored procedure in DB.
                taxShipService = countShipping.ShippingCosts(resultItems, "True", "cart"); //Call and Get oversea's product Local price, Tax and service fee from stored procedure in DB.
                ViewBag.isOverSea = true;
            }
            else
            {
                taxShipService = countShipping.ShippingCosts(resultItems); //Call and Get Local price, Tax and service fee from stored procedure in DB.
                ViewBag.isOverSea = false;
            }

            Dictionary<string, decimal> shippingCosts = new Dictionary<string, decimal>();
            shippingCosts = taxShipService.ShippingCost; //Set shipping cost in a variable.
            Dictionary<string, List<TWNewEgg.DB.TWSQLDB.Models.ExtModels.GetItemTaxDetail>> itemTaxDetail = new Dictionary<string, List<TWNewEgg.DB.TWSQLDB.Models.ExtModels.GetItemTaxDetail>>();
            itemTaxDetail = taxShipService.ShippingTaxCost; //Set it in a variable.

            decimal totalShipping = new decimal();
            foreach (var shipingcost in shippingCosts)
            {
                totalShipping += shipingcost.Value; //Count Total shipping cost.
            }

            interShip.Add(Convert.ToInt32(totalShipping));

            if (isOverSea == true)
            {
                ViewBag.Service = callService.SetResultsItem(result, itemTaxDetail, shippingCosts, "True"); //Combine SP and Result. Also count total service fee.
            }
            else
            {
                ViewBag.Service = callService.SetResultsItem(result, itemTaxDetail, shippingCosts); //Combine SP and Result. Also count total service fee.
            }

            decimal totalCost = new decimal();
            foreach (var resultItem in result)
            {
                //totalCost += ResultItem.salesorderitem_price; //Count Total item Price.
                totalCost += resultItem.priceSum.Value; //Count Total item Price.
                if (resultItem.itemList.Count != 0)
                {
                    foreach (var resultItemList in resultItem.itemList)
                    {
                        totalCost += resultItemList.priceSum.Value; //Count Total item Price.
                    }
                }
            }

            interShip.Add(Convert.ToInt32(totalCost));
            InsertSalesOrdersBySellerOutput userData = new InsertSalesOrdersBySellerOutput();
            userData = resultsOri.FirstOrDefault();
            ViewBag.UserData = userData; //Find and set User Data.
            ViewBag.InterShip = interShip;
            //ViewBag.Results = Result;
            ViewBag.rowSpan = rowSpan;

            ViewBag.shippingCosts = shippingCosts;
            ViewBag.arriveDate = db.SalesOrderItem.Where(x => x.SalesorderCode == userData.salesorder_code).Select(y => y.Note).FirstOrDefault().ToString().Replace("</arrive>", "");
            ///////////////////////////////

            var itemDate = db.SalesOrder.Where(x => x.Code == userData.salesorder_code).Select(y => y.CreateDate).FirstOrDefault(); //get item date

            // Get discount price Start
            ViewBag.DiscountData = callService.GetDiscountData(allSOItemCodes);
            // Get discount price End

            // Send SMS Start

            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (environment == "PRD")
            {
                if (itemDate != null)
                {
                    var timeDiff = itemDate.Subtract(DateTime.UtcNow.AddHours(8)).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
                    if (timeDiff.Minutes < 5)
                    {
                        var smsFlag = SendSMS(userData.salesorder_mobile, orderNumber);
                    }
                }
            }

            // Send SMS End

            var nowDate = DateTime.Now; //get now
            try
            {
                //所有SO狀態都為正常，才判定為訂購完整成功，寄送郵件通知
                List<string> needReSendList = new List<string>();
                //掃描訂單狀態
                string salesOrderCode = userData.salesorder_code;
                Nullable<int> groupID = db.SalesOrder.Where(x => x.Code == salesOrderCode).Select(x => x.SalesOrderGroupID).FirstOrDefault();
                ViewBag.salceordergorupid = groupID;
                if (groupID != null && groupID > 0)
                {
                    List<SalesOrder> salesOrderList = db.SalesOrder.Where(x => x.SalesOrderGroupID == groupID).ToList();
                    needReSendList = salesOrderList.Where(x => x.Status != (int)SalesOrder.status.付款成功 && x.Status != (int)SalesOrder.status.完成).Select(x => x.Code).ToList();
                }

                //寄送郵件通知
                MailManageController mailManage = new MailManageController(this.ControllerContext);
                if (itemDate != null)
                {
                    var timeDiff = itemDate.Subtract(nowDate).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
                    if (timeDiff.Minutes < 3)
                    {
                        if (needReSendList.Count == 0)
                        {
                            mailManage.DealSuccess(resultsOri); // 寄發訂單成功通知信
                        }
                        //else // 需要拋單的在之前應該都會拋送了，所以暫時拿掉在此時的重拋訂單通知信
                        //{
                        //    payTypeGatewayTurnON = payTypeGatewayTurnON.ToLower();
                        //    if (payTypeGatewayTurnON == "on")
                        //    {
                        //        SOReSendMail(needReSendList); // 寄發訂單失敗通知信
                        //    }
                        //}
                    }
                }

                logger.Info("[userEmail] [" + userEmail + "]" + "結果頁 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            }
            catch (Exception e)
            {
                logger.Error("[userEmail] [" + userEmail + "]" + userData.salesorder_code + ":" + e.Message + "_:_" + e.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// This function should be delete, coz this should be call from DB or File.
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="cardBank"></param>
        /// <param name="payTypeName"></param>
        private void SetPayTypeText(Nullable<Int32> payType, string cardBank, string payTypeName)
        {
            if (payType != null && payType != 0 && cardBank != null && cardBank != "")
            {
                //var payType = 
                //string displayPayType = "";
                string displayPayType = payTypeName;
                switch (payType.Value)
                {
                    case (int)PayType.nPayType.網路ATM:
                        //displayPayType = "Web ATM轉帳";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 1;
                        break;
                    case (int)PayType.nPayType.信用卡一次付清:
                        //displayPayType = "信用卡付款 (一次付清)";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.三期零利率:
                        //displayPayType = "3 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 3;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.六期零利率:
                        //displayPayType = "6 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 6;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十期零利率:
                        //displayPayType = "10 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 10;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十二期零利率:
                        //displayPayType = "12 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 12;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十八期零利率:
                        //displayPayType = "18 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 18;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.二十四期零利率:
                        //displayPayType = "24 期 0利率";
                        //ViewBag.pricesumtemp = pricesum / 24;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十期分期:
                        //displayPayType = "10 期 分期";
                        //ViewBag.pricesumtemp = pricesum / 10;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十二期分期:
                        //displayPayType = "12 期 分期";
                        //ViewBag.pricesumtemp = pricesum / 12;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.十八期分期:
                        //displayPayType = "18 期 分期";
                        //ViewBag.pricesumtemp = pricesum / 18;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.二十四期分期:
                        //displayPayType = "24 期 分期";
                        //ViewBag.pricesumtemp = pricesum / 24;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 2;
                        break;
                    case (int)PayType.nPayType.貨到付款:
                        //displayPayType = "新竹貨運貨到付款";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，我們將會委由新竹貨運送貨及貨到付款的收款，煩請在指定期間內確定有人可以在收件人的地點收貨及付款，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 3;
                        break;
                    case (int)PayType.nPayType.超商付款:
                        //displayPayType = "超商付款 取貨";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>請注意簡訊和e-mail的貨到通知訊息，並且在貨到通知日起７天內前，請完成繳款取貨。";
                        ViewBag.statusChoose = 4;
                        break;
                    case (int)PayType.nPayType.電匯:
                        //displayPayType = "Newegg 電匯";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>請注意簡訊和e-mail的貨到通知訊息，並且在貨到通知日起７天內前，請完成繳款取貨。";
                        ViewBag.statusChoose = 3;
                        break;
                    case (int)PayType.nPayType.歐付寶儲值支付:
                        //displayPayType = "歐付寶付款";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>您所訂購的商品明細和收件人資訊如下，若有任何問題煩請聯絡客服中心。";
                        ViewBag.statusChoose = 1;
                        break;
                    default:
                        //displayPayType = "ATM";
                        //ViewBag.pricesumtemp = pricesum;
                        ViewBag.readme = "感謝您的購買<br/>請於ＯＯＯＯ年ＯＯ月ＯＯ日ＯＯ時ＯＯ分前至「ATM自動提款機」轉帳，輸入下方１６碼的轉帳帳號及金額，就可順利完成此訂購！若有任何問題，煩請聯絡客服中心。";
                        ViewBag.statusChoose = 5;
                        break;
                }
                //displayPayType = cardBank + " " + displayPayType;
                ViewBag.payTypeName = displayPayType;
            }
        }

        /// <summary>
        /// RemoveCartItem
        /// </summary>
        /// <param name="result"></param>
        /// <param name="accountId"></param>
        /// <param name="dateTime"></param>
        /// <returns>是否成功執行</returns>
        public bool RemoveCartItem(List<ResultCart> result, int accountId, string dateTime)
        {
            ICarts removeCart = new CartsRepository();
            //OutputMessage OMessage = new OutputMessage();
            bool accStatus = removeCart.SetTrackAll(accountId, dateTime);
            if (accStatus)
            {
                List<int> itemIDs = new List<int>();
                foreach (var itemID in result)
                {
                    itemIDs.Add(itemID.itemID);
                }

                if (removeCart.RemoveTrack(itemIDs) == OutputMessage.removeSuccess || removeCart.RemoveTrack(itemIDs) == OutputMessage.noData)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 取得Client端IP位址
        /// </summary>
        /// <returns>返回IP位置</returns>
        private string GetIPAddress(HttpRequestBase request)
        {

            #region Get IP from API
            //if (Request.Cookies["cuipadd"] != null)
            //{
            //    string fromCookie = Request.Cookies["cuipadd"].Value;
            //    fromCookie = fromCookie.Replace("[\"", "");
            //    fromCookie = fromCookie.Replace("\"]", "");
            //    fromCookie = fromCookie.Trim();
            //    string[] splitString = new string[] { "." };
            //    string[] ipFormat = fromCookie.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            //    if (ipFormat.Length > 3)
            //    {
            //        return fromCookie;
            //    }
            //    else
            //    {
            //        return "";
            //    }
            //}
            //else
            //{
            //    return "";
            //}
            #endregion Get IP from API

            #region get client ip
            string remoteIPAddr = request.Headers.ToString();
            if (request.Headers["ClientIP"] != null)
            {
                logger.Info(request.Headers["ClientIP"].Trim());
                return request.Headers["ClientIP"].Trim();
            }
            else
            {
                return "";
            }
            #endregion get client ip
        }

        private bool SendSMS(string cellPhoneNo, string orderNumber)
        {
            bool flag = false;
            HiNetSolution SendSMS = new HiNetSolution();

            var sendMessage = String.Format(WebSiteData.SiteName + "已接到您的訂單：{0} 可至網站「我的訂單」查詢。提醒您，" + WebSiteData.SiteName + "不會以電話通知變更付款條件。", orderNumber);

            var response = SendSMS.EasySendSMSMessage(cellPhoneNo, sendMessage);

            return flag;
        }

        [HttpGet]
        public ActionResult Step3ResultPage(int salesOrderGroupID)
        {
            int accID = NEUser.ID;
            CartResults_View CartResults_View = GetCartResults_View(salesOrderGroupID);
            if (CartResults_View.Status.Equals("付款失敗"))
            {
                return RedirectToAction("Index", "Cart", new { SalesOrderGroupID = salesOrderGroupID, errMsg = "付款失敗，已取消訂單，請重新確認您的付款資訊是否正確。" });
            }
            //取得 auth 對應的信用卡相關資料
            var _GetPaymentDataResult = this.GetPaymentData(salesOrderGroupID);
            if (_GetPaymentDataResult == null)
            {
                return RedirectToAction("Index", "Cart", new { SalesOrderGroupID = salesOrderGroupID, errMsg = "系統連線異常，請洽詢客服人員。" });
            }
            //20: NCCC, 1006: 歐富寶
            int bankCodeLength = CartResults_View.CartPayType_View.BankCode.Length - 3;
            string bankCode = CartResults_View.CartPayType_View.BankCode.Substring(bankCodeLength, 3);
            //string bankCode = CartResults_View.CartPayType_View.BankCode.Replace("20", "").Replace("10006", "");
            //呼叫 service 取得銀行名稱
            var bankInfo = Processor.Request<string, string>("BankService", "BankName", bankCode);
            //判斷 service 是否連線錯誤或是 service 進入 exception
            if (string.IsNullOrEmpty(bankInfo.error) == false)
            {
                return RedirectToAction("Index", "Cart", new { SalesOrderGroupID = salesOrderGroupID, errMsg = "系統連線異常，請洽詢客服人員。" });
            }
            //判斷 service 回傳的訊息中是否有錯誤判斷
            if (bankInfo.results.IndexOf("[Error]") >= 0)
            {
                return RedirectToAction("Index", "Cart", new { SalesOrderGroupID = salesOrderGroupID, errMsg = "系統連線異常，請洽詢客服人員。" });
            }
            //以上無誤後把相關的信用卡紅利資料填入 model
            RedeemInfo redeeminfo = new RedeemInfo();
            redeeminfo.BankRedeemName = bankInfo.results;
            redeeminfo.BonusUsed = _GetPaymentDataResult.FirstOrDefault() == null ? 0 : _GetPaymentDataResult.FirstOrDefault().Bonus;
            redeeminfo.BonusBLN = _GetPaymentDataResult.FirstOrDefault() == null ? 0 : _GetPaymentDataResult.FirstOrDefault().BonusBLN;
            redeeminfo.BonusBLN = _GetPaymentDataResult.FirstOrDefault() == null ? 0 : _GetPaymentDataResult.FirstOrDefault().AmountSelf;
            CartResults_View.redeemInfo = redeeminfo;


            // 依據 BSATW-173 廢四機需求增加
            // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160517
            string Discard4 = string.Empty;
            foreach (var each_order in CartResults_View.SalesOrder_ViewList)
            {
                foreach (var each_item in each_order.SalesOrderItem_ViewList)
                {
                    Discard4 = each_item.Discard4;
                    break;
                }
                if (!string.IsNullOrEmpty(Discard4)) break;
            }
            //購車有廢四機商品就建立同意廢四機
            if (!string.IsNullOrEmpty(Discard4))
            {
                int my_salesOrderGroupID = salesOrderGroupID;
                string agreedDiscard4 = "Y";
                string user_name = NEUser.Email;
                if (CartResults_View.SalesOrder_ViewList.Count() > 0)
                    CartResults_View.SalesOrder_ViewList[0].AccountID.ToString();
                TWNewEgg.Models.DomainModels.Discard4.Discard4DM dd4_info = new TWNewEgg.Models.DomainModels.Discard4.Discard4DM();
                var dd4_result = Processor.Request<TWNewEgg.Models.DomainModels.Discard4.Discard4DM
                    , TWNewEgg.Models.DomainModels.Discard4.Discard4DM>("Discard4Service", "InitData", my_salesOrderGroupID, agreedDiscard4, user_name);
                if (dd4_result.error == null) dd4_info = dd4_result.results;
            }



            return View("Step3", CartResults_View);
        }

        public List<TWNewEgg.Models.ViewModels.Auth.AuthVM> GetPaymentData(int salesOrderGroupID)
        {
            var cartProxyResult = Processor.Request<List<TWNewEgg.Models.ViewModels.Auth.AuthVM>, List<TWNewEgg.Models.ViewModels.Auth.AuthVM>>("SOServices.AuthService", "GetAuthBySalesOrderGroupID", salesOrderGroupID);
            if (string.IsNullOrEmpty(cartProxyResult.error) == false)
            {
                return null;
            }
            return cartProxyResult.results;
        }


        public CartResults_View GetCartResults_View(int SalesOrderGroupID)
        {
            int accID = NEUser.ID;
            TWSqlDBContext db_before = new TWSqlDBContext();
            CartResults_View CartResults_View = new CartResults_View();

            TWNewEgg.DB.TWSQLDB.Models.SalesOrderGroup SalesOrderGroup = db_before.SalesOrderGroup.Where(x => x.ID == SalesOrderGroupID).FirstOrDefault();

            if (SalesOrderGroup == null)
            {
                return null;
            }

            List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder> SalesOrderList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == SalesOrderGroupID).ToList();

            if (SalesOrderList == null)
            {
                return null;
            }
            if (SalesOrderList.FirstOrDefault().AccountID != accID)
            {
                return null;
            }

            bool findFailSO = true;
            SalesOrderList.ForEach(x =>
            {
                if (x.Status != (int)SalesOrder.status.付款成功 && x.Status != (int)SalesOrder.status.完成)
                {
                    findFailSO = false;
                }
            });

            if (SalesOrderList.FirstOrDefault().PayType == (int)PayType.nPayType.實體ATM || findFailSO == true)
            {
                #region clientIP寫入
                var clientIP = NEUser.IPAddress;
                if (clientIP != "0.0.0.0")
                {
                    var setIPFlag = SetSOIP(SalesOrderGroupID, clientIP);
                }
                #endregion
                #region 寄發訂單成立通知信
                try
                {
                    string orderNumber = string.Empty;
                    orderNumber = SalesOrderList.FirstOrDefault().Code;
                    string SMSAndMail = System.Configuration.ConfigurationManager.AppSettings["Step3SendSMSAndMail"];
                    if (!string.IsNullOrEmpty(SMSAndMail) && SMSAndMail.ToLower() == "on")
                    {
                        ResultsService callService = new ResultsService();
                        List<InsertSalesOrdersBySellerOutput> resultsOri = callService.GetSalesOrders(orderNumber); //Get Stored procedure return data from DB.
                        var itemDate = resultsOri.First().salesorder_createdate;
                        if (itemDate != null)
                        {
                            var timeDiff = itemDate.Subtract(DateTime.UtcNow.AddHours(8)).Duration(); // 時間相減，避免因為使用者使用不當造成短時間大量重複寄信
                            if (timeDiff.Minutes < 5)
                            {
                                var smsFlag = SendSMS(resultsOri.First().salesorder_mobile, resultsOri.First().salesorder_code);
                                MailManageController mailManage = new MailManageController(this.ControllerContext);
                                mailManage.DealSuccess(resultsOri); // 寄發訂單成功通知信
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("[userEmail] [" + NEUser.Email + "]" + "Mail Error:" + e.ToString());
                }
                #endregion
            }

            List<string> SalesOrderIDList = SalesOrderList.Select(x => x.Code).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem> SalesOrderItemList = db_before.SalesOrderItem.Where(x => SalesOrderIDList.Contains(x.SalesorderCode)).ToList();
            List<int> ItemIDList = SalesOrderItemList.Select(x => x.ItemID).Distinct().ToList();
            // Item資料
            var ItemInforesult = Processor.Request<Dictionary<int, TWNewEgg.Models.DomainModels.Item.ItemInfo>, Dictionary<int, TWNewEgg.Models.DomainModels.Item.ItemInfo>>("ItemInfoService", "GetItemInfoList", ItemIDList).results;
            List<int> SellerIDList = ItemInforesult.Select(x => x.Value.ItemBase.SellerID).GroupBy(x => x).Select(x => x.Key).ToList();
            // Seller資料
            var Sellerresult = Processor.Request<Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase>, Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase>>("SellerServices", "GetSellerWithCountryList", SellerIDList);
            // Item圖片
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIDList).results;

            AutoMapper.Mapper.Map(SalesOrderGroup, CartResults_View.SalesOrderGroup_View);

            foreach (var SalesOrderListtemp in SalesOrderList)
            {
                TWNewEgg.Models.ViewModels.Cart.SalesOrder_View SalesOrder_View = new TWNewEgg.Models.ViewModels.Cart.SalesOrder_View();
                AutoMapper.Mapper.Map(SalesOrderListtemp, SalesOrder_View);
                List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem> SalesOrderItemListtemp = SalesOrderItemList.Where(x => x.SalesorderCode == SalesOrderListtemp.Code).ToList();

                foreach (var SalesOrderItemListtemptemp in SalesOrderItemListtemp)
                {
                    TWNewEgg.Models.ViewModels.Cart.SalesOrderItem_View SalesOrderItem_View = new TWNewEgg.Models.ViewModels.Cart.SalesOrderItem_View();
                    AutoMapper.Mapper.Map(SalesOrderItemListtemptemp, SalesOrderItem_View);

                    CartItem_View CartItem_View = new CartItem_View();
                    int SellerID = 0;
                    if (ItemInforesult != null && ItemInforesult.Count > 0)
                    {
                        if (ItemInforesult.Select(x => x.Value.ItemBase.ID == SalesOrderItemListtemptemp.ItemID) != null)
                        {
                            SellerID = ItemInforesult[SalesOrderItemListtemptemp.ItemID].ItemBase.SellerID;
                            AutoMapper.Mapper.Map(ItemInforesult[SalesOrderItemListtemptemp.ItemID].ItemBase, CartItem_View.CartItemBase);
                            CartItem_View.ShowOrder = ItemInforesult[SalesOrderItemListtemptemp.ItemID].ItemBase.ShowOrder;



                        }

                    }
                    if (Sellerresult.results != null && Sellerresult.results.Where(x => x.Key == CartItem_View.CartItemBase.SellerID).ToList().Count != 0)
                    {
                        CartItem_View.CountryofOriginID = Sellerresult.results[SellerID].CountryID;
                        CartItem_View.CountryofOrigin = Sellerresult.results[SellerID].CountryNameCHT;
                    }



                    var requestResult = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("ItemGroupService", "GetItemMarketGroupNameByItemId", SalesOrderItemListtemptemp.ItemID);
                    if (string.IsNullOrEmpty(requestResult.error))
                    {
                        if (!string.IsNullOrEmpty(requestResult.results))
                        {
                            SalesOrderItem_View.Attribs = (requestResult.results.StartsWith("-")) ? requestResult.results.Substring(1) : requestResult.results;
                        }
                    }
                    try
                    {
                        if (itemUrlDictionary.Count > 0 && itemUrlDictionary.Where(x => x.Key == SalesOrderItemListtemptemp.ItemID).FirstOrDefault().Value.Count != 0)
                        {
                            if (itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].FirstOrDefault() != null)
                            {
                                CartItem_View.ImagePath = imageDomain + itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].FirstOrDefault().ImageUrl;
                                if (itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 60).FirstOrDefault() != null)
                                {
                                    CartItem_View.ImagePath = imageDomain + itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                    {
                                        CartItem_View.ImagePath = imageDomain + itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                    }
                                    else
                                    {
                                        if (itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 300).FirstOrDefault() != null)
                                        {
                                            CartItem_View.ImagePath = imageDomain + itemUrlDictionary[SalesOrderItemListtemptemp.ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    SalesOrderItem_View.CartItem_View = CartItem_View;

                    SalesOrder_View.SalesOrderItem_ViewList.Add(SalesOrderItem_View);

                    CartResults_View.TimeofReceipt = (SalesOrderItemListtemptemp.Note.Replace("</arrive>", ",")).Split(',')[0];

                    CartResults_View.TotalPrice = CartResults_View.TotalPrice + Convert.ToInt32(Math.Floor((double)((SalesOrderItemListtemptemp.DisplayPrice ?? 0m) * SalesOrderItemListtemptemp.Qty) + 0.5));
                    CartResults_View.PromotionPriceSum = CartResults_View.PromotionPriceSum + Convert.ToInt32(Math.Floor((double)(SalesOrderItemListtemptemp.DiscountPrice ?? 0) + 0.5)) + Convert.ToInt32(Math.Floor((double)SalesOrderItemListtemptemp.ApportionedAmount + 0.5));
                    CartResults_View.CouponePriceSum = CartResults_View.CouponePriceSum + Convert.ToInt32(Math.Floor((double)(SalesOrderItemListtemptemp.Pricecoupon ?? 0) + 0.5));
                    CartResults_View.InstallmentFeeSum = CartResults_View.InstallmentFeeSum + Convert.ToInt32(Math.Floor((double)SalesOrderItemListtemptemp.InstallmentFee + 0.5));
                    CartResults_View.NeedPayMoneyPriceSum = CartResults_View.NeedPayMoneyPriceSum + Convert.ToInt32(Math.Floor((double)((SalesOrderItemListtemptemp.DisplayPrice ?? 0) * SalesOrderItemListtemptemp.Qty) + 0.5)) + Convert.ToInt32(Math.Floor((double)SalesOrderItemListtemptemp.InstallmentFee + 0.5)) - Convert.ToInt32(Math.Floor((double)(SalesOrderItemListtemptemp.DiscountPrice ?? 0) + 0.5)) - Convert.ToInt32(Math.Floor((double)SalesOrderItemListtemptemp.ApportionedAmount + 0.5)) - Convert.ToInt32(Math.Floor((double)(SalesOrderItemListtemptemp.Pricecoupon ?? 0) + 0.5));
                }
                CartResults_View.SalesOrder_ViewList.Add(SalesOrder_View);
            }

            if (SalesOrderList != null && SalesOrderList.Count > 0)
            {
                SalesOrder so = SalesOrderList.FirstOrDefault();
                CartResults_View.CartPayType_View = GetCartPayType_View(SalesOrderList.FirstOrDefault().PayType ?? 1);
                CartResults_View.CartPayType_View.BankCode = so.CardBank;
                CartResults_View.CartPayType_View.vAccount = so.CardNo;
                if (so.Expire.HasValue)
                {
                    CartResults_View.CartPayType_View.ExpireDate = so.Expire.Value;
                }

                CartResults_View.OtherCartNumber = 0;

                int[] notPayedPayTypes = new int[2] { ((int)PayType.nPayType.貨到付款), ((int)PayType.nPayType.實體ATM) };
                if (so.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.被動取消 || so.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.初始狀態)
                {
                    CartResults_View.Status = "付款失敗";
                }
                else if (so.Status == (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.尚未付款
                    || (notPayedPayTypes.Contains(so.PayType.Value) && so.Status != (int)TWNewEgg.DB.TWBACKENDDB.Models.Cart.status.完成))
                {
                    CartResults_View.Status = "尚未付款";
                }

                // 清理購物車
                if (CartResults_View.Status != "付款失敗")
                {

                    List<TWNewEgg.Models.DomainModels.Track.TrackDM> TrackDMList = new List<TWNewEgg.Models.DomainModels.Track.TrackDM>();
                    foreach (var temp in ItemIDList)
                    {
                        TWNewEgg.Models.DomainModels.Track.TrackDM newTrack = new TWNewEgg.Models.DomainModels.Track.TrackDM();
                        newTrack.ItemID = temp;
                        newTrack.Status = 0;
                        newTrack.Qty = 0;
                        newTrack.CategoryID = 0;
                        newTrack.CategoryType = 0;
                        TrackDMList.Add(newTrack);
                    }
                    if (TrackDMList.Count > 0)
                    {
                        List<string> DeleteFromTracksresults = Processor.Request<List<string>, List<string>>("TrackService", "DeleteFromTracks", accID, TrackDMList).results;
                    }
                }

                var GetCartAllList = Processor.Request<List<ShoppingCart_View>, List<ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", accID);
                int ShoppingCart_ViewListQty = GetCartAllList.results.Where(x => (x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車 || x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車 || x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車) && x.Qty > 0).ToList().Count;
                CartResults_View.OtherCartNumber = ShoppingCart_ViewListQty;

                if ((CartResults_View.CartPayType_View.ID == (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡 || CartResults_View.CartPayType_View.ID == (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡紅利折抵 || CartResults_View.CartPayType_View.ID == (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.儲值支付 || CartResults_View.CartPayType_View.ID == (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.網路ATM))
                {
                    CartResults_View.IsConsignee = 1;
                }
                if ((CartResults_View.CartPayType_View.ID == (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.貨到付款))
                {
                    CartResults_View.IsConsignee = 1;
                }
            }

            // 依據 BSATW-173 廢四機需求增加
            // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160517
            foreach (var each_order in CartResults_View.SalesOrder_ViewList)
            {
                foreach (var each_item in each_order.SalesOrderItem_ViewList)
                {
                    if (string.IsNullOrEmpty(ItemInforesult[each_item.ItemID].ItemBase.Discard4)) continue;
                    each_item.Discard4 = ItemInforesult[each_item.ItemID].ItemBase.Discard4; //取得廢四機值
                    break;
                }
            }

            return CartResults_View;
        }

        public CartPayType_View GetCartPayType_View(int PatTypeID = 1)
        {
            CartPayType_View CartPayType_Viewtemp = new CartPayType_View();
            switch (PatTypeID)
            {
                case ((int)PayType.nPayType.信用卡一次付清):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "一次付清";
                    CartPayType_Viewtemp.Installments = 1;
                    break;
                case ((int)PayType.nPayType.三期零利率):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "3期(0利率)";
                    CartPayType_Viewtemp.Installments = 3;
                    break;
                case ((int)PayType.nPayType.六期零利率):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "6期(0利率)";
                    CartPayType_Viewtemp.Installments = 6;
                    break;
                case ((int)PayType.nPayType.十期零利率):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "10期(0利率)";
                    CartPayType_Viewtemp.Installments = 10;
                    break;
                case ((int)PayType.nPayType.十二期零利率):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "12期(0利率)";
                    CartPayType_Viewtemp.Installments = 12;
                    break;
                case ((int)PayType.nPayType.十八期零利率):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "18期(0利率)";
                    CartPayType_Viewtemp.Installments = 18;
                    break;
                case ((int)PayType.nPayType.十期分期):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "10期";
                    CartPayType_Viewtemp.Installments = 10;
                    break;
                case ((int)PayType.nPayType.十二期分期):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "12期";
                    CartPayType_Viewtemp.Installments = 12;
                    break;
                case ((int)PayType.nPayType.十八期分期):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "18期";
                    CartPayType_Viewtemp.Installments = 18;
                    break;
                case ((int)PayType.nPayType.二十四期分期):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡.ToString();
                    CartPayType_Viewtemp.Name = "24期";
                    CartPayType_Viewtemp.Installments = 24;
                    break;
                case ((int)PayType.nPayType.信用卡紅利折抵):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡紅利折抵;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.信用卡紅利折抵.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                case ((int)PayType.nPayType.貨到付款):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.貨到付款;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.貨到付款.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                case ((int)PayType.nPayType.超商付款):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.超商付款;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.超商付款.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                case ((int)PayType.nPayType.實體ATM):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.實體ATM;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.實體ATM.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                case ((int)PayType.nPayType.網路ATM):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.網路ATM;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.網路ATM.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                case ((int)PayType.nPayType.歐付寶儲值支付):
                    CartPayType_Viewtemp.ID = (int)TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.儲值支付;
                    CartPayType_Viewtemp.GroupName = TWNewEgg.Models.ViewModels.Cart.CartPayTypeGroupenum.儲值支付.ToString();
                    CartPayType_Viewtemp.Name = "";
                    break;
                default:
                    CartPayType_Viewtemp.ID = 0;
                    CartPayType_Viewtemp.GroupName = "尚未定義";
                    CartPayType_Viewtemp.Name = "";
                    break;
            }
            return CartPayType_Viewtemp;
        }

        /// <summary>
        /// 根據付款方式及使用者的銀行卡號, 選擇最優先的金流設定
        /// </summary>
        /// <param name="argPayType">PayType.nPayType</param>
        /// <param name="argUserCardBankId"></param>
        /// <returns>Paytype or null</returns>
        private TWNewEgg.DB.TWSQLDB.Models.PayType GetCurrentPatyType(PayType.nPayType argPayType, int? argUserCardBankId)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.PayType objPayTypeResult = null;
            string strUserCardBankId = "";

            #region 信用卡一次付清
            //信用卡有支援銀行卡號的差異, 故要特別選擇
            if (argPayType.Equals(PayType.nPayType.信用卡一次付清))
            {
                if (argUserCardBankId != null && argUserCardBankId > 0)
                {
                    //取得支援目前此卡的金流
                    strUserCardBankId = "," + Convert.ToString(argUserCardBankId) + ",";
                    //先判斷有沒有指定金流銀行的資料
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && x.BankID == argUserCardBankId).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    //若沒有指定銀行金流, 就判斷有支援這張卡片的金流
                    if (objPayTypeResult == null)
                    {
                        objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && ("," + x.BankIDList + ",").IndexOf(strUserCardBankId) >= 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    }
                }
                //若沒有指定BankId, 或是上述判斷式也無法找到支援銀行, 就用系統優先值最高的
                if (objPayTypeResult == null)
                {
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                }

            }
            #endregion

            #region 紅利折抵
            if (argPayType.Equals(PayType.nPayType.信用卡紅利折抵))
            {
                strUserCardBankId = "," + Convert.ToString(argUserCardBankId) + ",";
                objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && ("," + x.BankIDList + ",").IndexOf(strUserCardBankId) >= 0 && x.Status == 0).FirstOrDefault();
            }
            #endregion

            #region 信用卡(有息無息邏輯皆同)
            if (argPayType.Equals(PayType.nPayType.三期零利率)
                || argPayType.Equals(PayType.nPayType.六期零利率)
                || argPayType.Equals(PayType.nPayType.十期零利率)
                || argPayType.Equals(PayType.nPayType.十二期零利率)
                || argPayType.Equals(PayType.nPayType.十八期零利率)
                || argPayType.Equals(PayType.nPayType.二十四期零利率)
                || argPayType.Equals(PayType.nPayType.十期分期)
                || argPayType.Equals(PayType.nPayType.十二期分期)
                || argPayType.Equals(PayType.nPayType.十八期分期)
                || argPayType.Equals(PayType.nPayType.二十四期分期))
            {
                //分期信用卡有支援銀行卡號的差異, 故要BankId選擇
                if (argUserCardBankId != null && argUserCardBankId > 0)
                {
                    //取得支援目前此卡的結帳方式的清單
                    strUserCardBankId = "," + Convert.ToString(argUserCardBankId) + ",";
                    //先判斷有沒有指定金流銀行的資料
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && x.BankID == argUserCardBankId).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    //若沒有指定銀行金流, 就判斷有支援這張卡片的金流
                    if (objPayTypeResult == null)
                    {
                        objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && ("," + x.BankIDList + ",").IndexOf(strUserCardBankId) >= 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    }
                    //若沒有支援的金流, 就不作任何事
                }
            }
            #endregion

            #region WebATM
            //WebATM可支援銀行卡或可不支援
            if (argPayType.Equals(PayType.nPayType.網路ATM))
            {
                if (argUserCardBankId != null && argUserCardBankId > 0)
                {
                    //取得支援設定銀行的WebATM
                    strUserCardBankId = "," + Convert.ToString(argUserCardBankId) + ",";
                    //先判斷有沒有指定金流銀行的資料
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && x.BankID == argUserCardBankId).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    //若沒有指定銀行金流, 就判斷有支援這張卡片的金流
                    if (objPayTypeResult == null)
                    {
                        objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && ("," + x.BankIDList + ",").IndexOf(strUserCardBankId) >= 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                    }
                }
                if (objPayTypeResult == null)
                {
                    //若沒有指定BankId, 或是上述的設定都沒有資料,直接抓系統優先的預設值
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                }
            }
            #endregion

            #region 歐付寶儲值支付/超商/電匯/貨到付款
            if (argPayType.Equals(PayType.nPayType.歐付寶儲值支付)
                || argPayType.Equals(PayType.nPayType.超商付款)
                || argPayType.Equals(PayType.nPayType.電匯)
                || argPayType.Equals(PayType.nPayType.貨到付款)
                || argPayType.Equals(PayType.nPayType.實體ATM))
            {
                if (argUserCardBankId != null && argUserCardBankId != 0)
                {
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0 && x.BankID == argUserCardBankId).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                }
                else
                {
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType && x.Status == 0).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                }
            }
            #endregion

            #region 內部Service登入則不論何種交易模式都可使用貨到付款與電匯
            if (argPayType.Equals(PayType.nPayType.貨到付款)
                || argPayType.Equals(PayType.nPayType.電匯))
            {
                string innerAccount = NEUser.Email.ToLower();
                ServiceAccount = ServiceAccount.Replace(" ", "").ToLower();
                List<string> serviceAccountList = ServiceAccount.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
                string serviceCheck = serviceAccountList.Where(x => x == innerAccount).FirstOrDefault();
                // 不論是否付款方式上下架，客服帳號皆可看到貨到付款與電匯的付款方式
                if (!string.IsNullOrEmpty(serviceCheck))
                {
                    objPayTypeResult = db_before.PayType.Where(x => x.PayType0rateNum == (int)argPayType).OrderBy(x => x.ChooseOrder).FirstOrDefault();
                }
            }
            #endregion

            if (System.Configuration.ConfigurationManager.AppSettings["TestPayType"] != null)
            {
                string paytype = System.Configuration.ConfigurationManager.AppSettings["TestPayType"];
                int paytypeid = int.Parse(paytype);
                objPayTypeResult = db_before.PayType.Where(x => x.ID == paytypeid).FirstOrDefault();
            }

            if (db_before != null)
            {
                db_before.Dispose();
                db_before = null;
            }
            return objPayTypeResult;
        }

        #region 呼叫 service 取得銀行紅利最低消費和最高折扣
        public List<CartPayTypeBankIDwithName_View> GetBackBonuses(List<CartPayTypeBankIDwithName_View> _CartPayTypeBankIDwithName_View)
        {
            if (_CartPayTypeBankIDwithName_View == null)
            {
                return null;
            }
            _CartPayTypeBankIDwithName_View.ForEach(p =>
            {
                p.ConsumeMax = "100";
                p.ConsumeMin = "60000";
            });

            return _CartPayTypeBankIDwithName_View;
        }
        #endregion
    }
}
