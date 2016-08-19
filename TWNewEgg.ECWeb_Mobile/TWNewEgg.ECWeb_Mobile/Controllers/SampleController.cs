using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Product;
using TWNewEgg.Models.ViewModels.Product;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.Models.DomainModels.MobileStore;

//Search
using TWNewEgg.Models.ViewModels.Search;
using TWNewEgg.Models.DomainModels.Search;
using TWNewEgg.Framework.ServiceApi;

//Register
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.ECWeb_Mobile.Services.Account;
using TWNewEgg.Models.ViewModels.Message;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.Models.ViewModels.Login;

//賣場頁 
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;

//登入後的
using TWNewEgg.ECWeb_Mobile.Auth;

//活動
//using TWNewEgg.Models.DomainModels.Event;

//Promotion
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.Models.ViewModels.Answer;
namespace TWNewEgg.ECWeb_Mobile.Controllers
{

    [AllowAnonymous]
    public class SampleController : Controller
    {
        //
        // GET: /Sample/
        private AccountService _accountService;
        public ActionResult Test(int iTest)
        {
            //ViewBag.TestStart = DateTime.Now;
            //ViewBag.test = Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report", number).results;
            //ViewBag.TestEnd = DateTime.Now;
            //http://localhost:62608/sample/test?iTest=7
            if (iTest == 1)
            {
                #region Search
                var searchword = "Acer";
                var order = 3;
                SearchConditionDM condition = new SearchConditionDM();
                if (!string.IsNullOrEmpty(searchword))
                {
                    searchword = searchword.Trim();
                }
                condition = SettingCondition(searchword, "", order, null, null, null, null, null, null, null, 8, 2, null, null, "");

                Api.SearchController searchService = new Api.SearchController();
                var results = searchService.Get(condition);

                //TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
                //ViewBag.KeyWords = searchword;
                //int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(results.resultCount) / condition.PageSize));
                //ViewBag.TotalPage = totalPage;
                //ViewBag.NowPage = condition.Page.Value;
                //ViewBag.ShowingPageList = CalculationsPage.getShowPages(totalPage, condition.Page.Value, 3);
                #endregion
            }
            if (iTest == 2)
            {
                #region Register

                RegisterVM saveAccountVM = new RegisterVM
                {
                    AgreePaper = 1,
                    Birthday = new DateTime(1984, 5, 1),
                    confirmPWD = "abcd1234",
                    Email = "aabbcc@email.com",
                    Firstname = "李",
                    Lastname = "小二",
                    MessagePaper = 1,
                    Mobile = "0910123123",
                    PWD = "abcd1234",
                    securitycode = false,
                    Sex = 1
                };

                ECWebResponse response = new ECWebResponse();
                AccountInfoVM accountInfo = new AccountInfoVM();
                ResponsePacket<AccountInfoVM> registerResult;
                this._accountService = new AccountService(AccountAuthFactory.AuthType.ecweb.ToString());
                //RegisterVM registerVM = new RegisterVM();
                RegistrationError errorMsg = this._accountService.CheckRegister(saveAccountVM);
                if (!errorMsg.error)
                {

                    TWNewEgg.Models.DomainModels.Account.AccountDM accountDM = new TWNewEgg.Models.DomainModels.Account.AccountDM();
                    AutoMapper.Mapper.Map(saveAccountVM, accountDM);
                    registerResult = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "Register", accountDM);
                    if (string.IsNullOrWhiteSpace(registerResult.error))
                    {
                        response.Data = registerResult.results;
                        response.Status = (int)ECWebResponse.StatusCode.成功;
                        Login model = new TWNewEgg.Models.ViewModels.Login.Login();
                        model.user = saveAccountVM.Email;
                        model.pass = saveAccountVM.PWDtxt;
                        var exist = _accountService.CheckLogin(model);
                        var loginStatus = (exist != null) ? true : false;
                        if (loginStatus)
                        {
                            return Json("0");
                        }
                        else
                        {
                            response.Error = new ErrorMessage() { Code = 1, Detail = "系統出現異常，請聯絡管理員" };
                            response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                            return Json("1");
                        }
                    }
                    else
                    {
                        response.Error = new ErrorMessage() { Code = 1, Detail = "系統出現異常，請聯絡管理員" };
                        response.Status = (int)ECWebResponse.StatusCode.系統錯誤;
                        return Json("1");
                    }
                }
                else
                {
                    return Json(errorMsg.errormessage);
                }
                #endregion
            }
            if (iTest == 3)
            {
                #region 首頁category
                List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;
                listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;
                #endregion
            }
            if (iTest == 4)
            {
                #region 賣場頁
                //var ItemId = 78640; //一般商品
                var ItemId = 57377;  //閃購
                //var ItemId = 78640; //美蛋商品 87788,
                //考慮到資料傳輸與組合的效能, 麵包屑及所有父階的取得作業, 改由Client利用Ajax呼叫Api來完成
                ItemBasic objItemBasic = null;
                Dictionary<int, ItemUrl> itemUrls = new Dictionary<int, ItemUrl>();

                objItemBasic = Processor.Request<ItemBasic, ItemDetail>("ItemDetailService", "GetItemDetail", ItemId, "on").results;
                if (objItemBasic == null)
                {
                    return View();
                }

                Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemId }).results;

                var listImgUrl = new List<string>();
                foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemId].Where(x => x.Size == 640))
                {
                    if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    {
                        listImgUrl.Add(singleImgUrl.ImageUrl);
                    }
                    else
                    {
                        listImgUrl.Add(string.Format("{0}{1}", "https://ssl-images.newegg.com.tw", singleImgUrl.ImageUrl));
                    }
                }
                objItemBasic.ImgUrlList = listImgUrl;
                #endregion
            }
            if (iTest == 5)
            {
                #region 閃購清單
                //GroupBuyService.Service.GroupBuyService gpbService = new GroupBuyService.Service.GroupBuyService();
                TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition condition = new TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyQueryCondition();
                condition.PageSize = 8;
                condition.PageNumber = 1;
                condition.GroupBuyID = 0;

                //infoList = gpbService.QueryViewInfo(condition);
                var result = Processor.Request<List<TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>, List<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo>>("GroupBuyService", "QueryViewInfo", condition, (condition.PageSize.ToString() + "_" + condition.PageNumber.ToString()));
                #endregion
            }
            if (iTest == 6)
            {
                #region 折扣
                //研究一下再搬,桌機版web直接連db沒透過service
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listAllCoupon = null;
                string strAccountId = "";
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listActiveCoupon = null;   //已生效可使用的Coupon
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listUsedCoupon = null; //已經使用過的Coupon
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listExpiredCoupon = null; //過期的Coupon
                List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listWaitingForActivecoupon = null; //待生效的Coupon
                //List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem> listSalesOrderItem = null;
                List<string> listSalesOrderItemCode = null;
                //TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem oSalesOrderItem = null;
                //TWNewEgg.DB.TWSqlDBContext oDb = null;
                Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>> oDictResult = null;
                DateTime dateTimeNow = DateTime.Now;

                strAccountId = "8";// NEUser.ID.ToString();
                oDictResult = new Dictionary<int, List<TWNewEgg.Models.ViewModels.Redeem.Coupon>>();

                // 將Coupon分類
                // 已生效但未使用
                listActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", strAccountId).results;
                // 修改Categories的說明
                if (listActiveCoupon != null)
                {
                    this.getCouponCategoriesDesc(ref listActiveCoupon);
                }
                oDictResult.Add(1, listActiveCoupon);

                // 已使用:僅列出3個月內的消費記錄
                listUsedCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getUsedCouponIn3MonthListByAccount", strAccountId).results;
                if (listUsedCoupon != null && listUsedCoupon.Count > 0)
                {
                    // 置換x.ordcode為顯示商品名稱, 而非消費者看不懂的FBS序號
                    //listSalesOrderItemCode = listUsedCoupon.Select(x => x.ordcode).ToList();
                    //oDb = new DB.TWSqlDBContext();
                    //listSalesOrderItem = oDb.SalesOrderItem.Where(x => listSalesOrderItemCode.Contains(x.Code)).ToList();
                    //if (listSalesOrderItem != null)
                    //{
                    //    foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listUsedCoupon)
                    //    {
                    //        oSalesOrderItem = listSalesOrderItem.FirstOrDefault(x => x.Code == oSubCoupon.ordcode);
                    //        if (oSalesOrderItem != null)
                    //            oSubCoupon.ordcode = "<div style='display:none;'>" + oSubCoupon.ordcode + "</div>" + oSalesOrderItem.Name;
                    //        // 判斷coupon的Used狀況, 若是「使用後訂單取消」也要特別顯示
                    //        if (oSubCoupon.usestatus == (int)TWNewEgg.Models.ViewModels.Redeem.Coupon.CouponUsedStatusOption.UsedButCancel)
                    //            oSubCoupon.ordcode += "<br><span style='color:red'>此項商品已取消訂購</span>";
                    //    }// end foreach
                    //}
                    //oDb.Dispose();
                    // 修改Categories的說明
                    this.getCouponCategoriesDesc(ref listUsedCoupon);
                }
                oDictResult.Add(2, listUsedCoupon);
                // 未使用但過期
                listExpiredCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getExpiredCouponListByAccount", strAccountId).results;
                // 修改Categories的說明
                if (listExpiredCoupon != null)
                {
                    //縮減為一年內的記錄
                    listExpiredCoupon = listExpiredCoupon.Where(x => x.validend >= DateTime.Now.AddDays(-365)).ToList();
                    if (listExpiredCoupon.Count > 0)
                    {
                        this.getCouponCategoriesDesc(ref listExpiredCoupon);
                    }
                    else
                    {
                        listExpiredCoupon = null;
                    }
                }
                oDictResult.Add(3, listExpiredCoupon);

                // 待生效 : Event設定可以使用coupon
                listWaitingForActivecoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getWaitingActiveCouponByAccount", strAccountId).results;
                // 修改Categories的說明
                if (listWaitingForActivecoupon != null)
                {
                    this.getCouponCategoriesDesc(ref listWaitingForActivecoupon);
                }
                oDictResult.Add(4, listWaitingForActivecoupon);

                //釋放記憶體
                //listSalesOrderItem = null;
                listSalesOrderItemCode = null;
                //oSalesOrderItem = null;

                return View(oDictResult);
                #endregion
            }
            if (iTest == 7)
            {
                #region 修改帳戶GET資料
                string email = "d@d.com";// NEUser.Email;
                var memberInfo = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "EditPersonInfo", email);
                memberInfo.results.AVM.PWD = "";
                memberInfo.results.AVM.Loginon = null;
                #endregion
            }
            if (iTest == 8)
            {
                #region 修改帳戶POST資料
                string email = "d@d.com";// NEUser.Email;
                MemberVM SaveMemberVM = new MemberVM();
                int EDM = 1;
                var result = Processor.Request<bool, bool>("AccountService", "EditPersonInformation", SaveMemberVM, EDM, email);
                #endregion
            }
            if (iTest == 9)
            {
                #region 購物車
                int accID = 8; //NEUser.ID
                string TypeNameList = "cart";
                string OrderBy = "LowPrice";
                List<int> TypeIDList_SingleQty = new List<int>() { 3, 5, 7, 8, 9 };
                Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>> TrackItemresults = Processor.Request<Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>>, Dictionary<string, List<TWNewEgg.Models.DomainModels.Track.TrackItem>>>("TrackService", "GetTracksDetial", accID).results;
                //購物車和我的最愛差這邊而已,TrackItemresults兩者都回來了...
                List<TWNewEgg.Models.ViewModels.Track.TrackItem_View> TrackItem_ViewList = TrackItemresults.Where(x => x.Key == TypeNameList).FirstOrDefault().Value;
                //取圖片
                List<int> ItemIDList = TrackItem_ViewList.Select(x => x.ItemID).ToList();
                Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIDList).results;
                // Item參語活動
                //TWNewEgg.ECWeb_Mobile.Services.Cart.getPromotionGiftDetail getPromotionGiftDetail = new TWNewEgg.ECWeb_Mobile.Services.Cart.getPromotionGiftDetail();
                //var PromotionGiftDetailList = getPromotionGiftDetail.getItemPromotionGiftListInfo(ItemIDList).Data;
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View
                {
                    TypeName = TypeNameList,
                    TypeQty = 0,
                    ViewPage = 1,
                    TotalPage = 0
                };
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
                foreach (TWNewEgg.Models.ViewModels.Track.TrackItem_View item in TrackItem_ViewList)
                {
                    var viewitem = new TWNewEgg.Models.ViewModels.Cart.CartItem_View();
                    AutoMapper.Mapper.Map(item, viewitem);
                    viewitem.ImagePath = getImgURL(itemUrlDictionary, item.ItemID);
                    ItemGroup_View.CartItemList.Add(viewitem);
                }
                #endregion
            }
            if (iTest == 10)
            {
                #region 我的最愛
                int accID = 8; //NEUser.ID
                string TypeNameList = "wish";
                string OrderBy = "LowPrice";
                List<int> TypeIDList_SingleQty = new List<int>() { 3, 5, 7, 8, 9 };//只有宣告沒使用???
                Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>> TrackItemresults = Processor.Request<Dictionary<string, List<TWNewEgg.Models.ViewModels.Track.TrackItem_View>>, Dictionary<string, List<TWNewEgg.Models.DomainModels.Track.TrackItem>>>("TrackService", "GetTracksDetial", accID).results;
                List<TWNewEgg.Models.ViewModels.Track.TrackItem_View> TrackItem_ViewList = TrackItemresults.Where(x => x.Key == TypeNameList).FirstOrDefault().Value;
                //取圖片
                List<int> ItemIDList = TrackItem_ViewList.Select(x => x.ItemID).ToList();
                Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIDList).results;
                // Item參語活動,該service直接連DB了!!稍後,等和Bill確認
                //TWNewEgg.ECWeb_Mobile.Services.Cart.getPromotionGiftDetail getPromotionGiftDetail = new TWNewEgg.ECWeb_Mobile.Services.Cart.getPromotionGiftDetail();
                //var PromotionGiftDetailList = getPromotionGiftDetail.getItemPromotionGiftListInfo(ItemIDList).Data;

                //PS:不一定要用這ItemGroup_View 這個viewmodel,主要差異是getImgURL(itemUrlDictionary,item.ItemID);拿圖檔路徑,和上面的Item參語活動
                //以及部分和分頁有關的,實際上TrackItem_ViewList都已經有需要的分頁資料
                TWNewEgg.Models.ViewModels.Cart.ItemGroup_View ItemGroup_View = new TWNewEgg.Models.ViewModels.Cart.ItemGroup_View
                {
                    TypeName = TypeNameList,
                    TypeQty = 0,
                    ViewPage = 1,
                    TotalPage = 0
                };
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
                foreach (TWNewEgg.Models.ViewModels.Track.TrackItem_View item in TrackItem_ViewList)
                {
                    var viewitem = new TWNewEgg.Models.ViewModels.Cart.CartItem_View();
                    AutoMapper.Mapper.Map(item, viewitem);
                    viewitem.ImagePath = getImgURL(itemUrlDictionary, item.ItemID);
                    ItemGroup_View.CartItemList.Add(viewitem);
                }
                #endregion
            }
            if (iTest == 11)
            {
                #region 活動
                //var events = Processor.Request<List<Event>, List<Event>>("EventService", "GetEvents", new int[]{221,215});
                #endregion
            }
            if (iTest == 12)
            {
                #region 商品促銷活動 
                var result = Processor.Request<TWNewEgg.Models.DomainModels.Message.ResponseMessage<Dictionary<int, List<GroupDiscount>>>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<Dictionary<int, List<GroupDiscount>>>>("PromotionService", "getItemPromotionGiftListInfo", new List<int>(new int[] { 132146,76555,134541 })).results;
                #endregion
            }
            if (iTest == 13)
            {
                #region 取得商品子單
                var result = Processor.Request<List<SalesOrderItemInfo>, List<SalesOrderItemInfo>>("EventService", "GetSOItemByCodes", new List<string>(new string[] { "LBS131122000018", "LBS131122000017", "LBS131122000010" })).results;                
                #endregion
            }
            return View();
        }
        private void getCouponCategoriesDesc(ref List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon)
        {
            //TWNewEgg.DB.TWSqlDBContext oDb = null;
            //Redeem.Service.CouponService.CouponServiceRepository oCouponService = null;
            //List<int> listEventId = null;
            //List<TWNewEgg.DB.TWSQLDB.Models.Event> listEvent = null;
            //TWNewEgg.DB.TWSQLDB.Models.Event oEvent = null;

            //if (listCoupon == null)
            //    return;

            //listEventId = listCoupon.Select(x => x.eventid).Distinct().ToList<int>();

            //oDb = new DB.TWSqlDBContext();
            //listEvent = oDb.Event.Where(x => listEventId.Contains(x.id)).ToList<TWNewEgg.DB.TWSQLDB.Models.Event>();
            //oDb = null;

            //if (listEvent != null)
            //{
            //    foreach (TWNewEgg.Models.ViewModels.Redeem.Coupon oSubCoupon in listCoupon)
            //    {
            //        oEvent = listEvent.FirstOrDefault(x => x.id == oSubCoupon.eventid);
            //        if (oEvent == null)
            //            continue;

            //        if (oEvent.limitdescription.Length > 0)
            //            oSubCoupon.categories = oEvent.limitdescription;
            //        else if (oSubCoupon.categories.Equals(";0;"))
            //            oSubCoupon.categories = "全館";
            //        else
            //            oSubCoupon.categories = "部份商品";

            //        if (oSubCoupon.title == null || oSubCoupon.title.Length <= 0)
            //        {
            //            oSubCoupon.title = oEvent.name;
            //        }
            //    }//end foreach
            //}

            ////釋放記憶體
            //if (oDb != null)
            //{
            //    oDb.Dispose();
            //}

            //listEvent = null;
            //listEventId = null;
        }
        private string imageDomain = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"];
        private string getImgURL(Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemUrl>> itemUrlDictionary, int itemID)
        {
            if (itemUrlDictionary.ContainsKey(itemID) == false) return "";
            int[] imgSizes = new int[]{60,125,300};
            foreach (int iSize in imgSizes)
            {
                var imgset = itemUrlDictionary[itemID].Where(x => x.Size == iSize).FirstOrDefault();
                if (imgset != null)
                {
                    if (imageDomain.IndexOf("newegg.com/") >= 0) return imageDomain + imgset.ImageUrl;
                    else return "https://ssl-images.newegg.com.tw" + imgset.ImageUrl;
                }
            }
            return "";
        }
        private SearchConditionDM SettingCondition(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats)
        {
            SearchConditionDM condition = new SearchConditionDM();
            if (string.IsNullOrEmpty(searchword))
            {
                searchword = null;
            }
            condition.SearchWord = searchword;

            if (string.IsNullOrEmpty(srchin))
            {
                srchin = null;
            }
            condition.SrchIn = srchin;

            if (page == null || page < 0)
            {
                page = 0;
            }
            condition.Page = page.Value;

            if (pagesize == null || pagesize < 8 || pagesize > 1000)
            {
                pagesize = 8;
            }
            condition.PageSize = pagesize.Value;

            if (order != null)
            {
                condition.Order = order.Value;
            }

            if (cat != null)
            {
                condition.Cat = cat.Value;
            }

            if (lid != null)
            {
                condition.LID = lid.Value;
            }

            if (cty != null)
            {
                condition.Cty = cty.Value;
            }

            if (bid != null)
            {
                condition.BID = bid.Value;
            }

            if (sid != null)
            {
                condition.SID = sid.Value;
            }

            if (minprice != null)
            {
                condition.minPrice = minprice.Value;
            }

            if (maxprice != null)
            {
                condition.maxPrice = maxprice.Value;
            }

            if (!string.IsNullOrEmpty(mode))
            {
                condition.Mode = mode;
            }

            if (!string.IsNullOrEmpty(submit))
            {
                condition.Submit = submit;
            }

            if (!string.IsNullOrEmpty(orderCats))
            {
                condition.orderCats = orderCats;
            }

            return condition;
        }
        public ActionResult Test2(int number)
        {
            ViewBag.TestStart = DateTime.Now;
            ViewBag.test = Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService2", "report", number).results;
            ViewBag.TestEnd = DateTime.Now;
            return View("Test");
        }
        public ActionResult Test3()//string comID, string methodName
        {
            //ViewBag.TestStart = DateTime.Now;
            //ViewBag.test = Processor.Request<string, string>(comID, methodName).results;
            //ViewBag.TestEnd = DateTime.Now;
            var searchword = "Acer";
            var order = 3;
            SearchConditionDM condition = new SearchConditionDM();
            if (!string.IsNullOrEmpty(searchword))
            {
                searchword = searchword.Trim();
            }
            condition = SettingCondition(searchword, "", null, null, null, null, null, null, null, null, 8, 0, null, null, "");

            Api.SearchController searchService = new Api.SearchController();
            var results = searchService.Get(condition);

            //TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
            //ViewBag.KeyWords = searchword;
            //int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(results.resultCount) / condition.PageSize));
            //ViewBag.TotalPage = totalPage;
            //ViewBag.NowPage = condition.Page.Value;
            //ViewBag.ShowingPageList = CalculationsPage.getShowPages(totalPage, condition.Page.Value, 3);
            return View();
        }
        //private SearchConditionDM SettingCondition(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats)
        //{
        //    SearchConditionDM condition = new SearchConditionDM();
        //    if (string.IsNullOrEmpty(searchword))
        //    {
        //        searchword = null;
        //    }
        //    condition.SearchWord = searchword;

        //    if (string.IsNullOrEmpty(srchin))
        //    {
        //        srchin = null;
        //    }
        //    condition.SrchIn = srchin;

        //    if (page == null || page < 0)
        //    {
        //        page = 0;
        //    }
        //    condition.Page = page.Value;

        //    if (pagesize == null || pagesize < 8 || pagesize > 1000)
        //    {
        //        pagesize = 8;
        //    }
        //    condition.PageSize = pagesize.Value;

        //    if (order != null)
        //    {
        //        condition.Order = order.Value;
        //    }

        //    if (cat != null)
        //    {
        //        condition.Cat = cat.Value;
        //    }

        //    if (lid != null)
        //    {
        //        condition.LID = lid.Value;
        //    }

        //    if (cty != null)
        //    {
        //        condition.Cty = cty.Value;
        //    }

        //    if (bid != null)
        //    {
        //        condition.BID = bid.Value;
        //    }

        //    if (sid != null)
        //    {
        //        condition.SID = sid.Value;
        //    }

        //    if (minprice != null)
        //    {
        //        condition.minPrice = minprice.Value;
        //    }

        //    if (maxprice != null)
        //    {
        //        condition.maxPrice = maxprice.Value;
        //    }

        //    if (!string.IsNullOrEmpty(mode))
        //    {
        //        condition.Mode = mode;
        //    }

        //    if (!string.IsNullOrEmpty(submit))
        //    {
        //        condition.Submit = submit;
        //    }

        //    if (!string.IsNullOrEmpty(orderCats))
        //    {
        //        condition.orderCats = orderCats;
        //    }

        //    return condition;
        //}
        public string MultiRequest(int number)
        {

            var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report", number);

            //int a = 5;
            //string b = "total : ";
            //List<int> aaa = new List<int>() { 0, 2, 3 };
            //TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            //pp.ManufactureID = 999;
            //pp.Name = "3roijogiorjgrweg";
            //List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp);
            return testtt.error + " - results: " + testtt.results.Count;
        }
        public string MultiRequest2(int number)
        {

            int a = number;
            string b = "total : ";
            List<int> aaa = new List<int>() { 0, 2, 3 };
            TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            pp.ManufactureID = 999;
            pp.Name = "3roijogiorjgrweg";
            List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test1.ManufactureID = 8888;
            test1.Name = "654948321389981";
            TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            test2.ManufactureID = 9849846;
            test2.Name = "asegfwgrwegherg";
            test.Add(test1);
            test.Add(test2);
            var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test5", a, b, aaa, test);
            return testtt.error + " - results: " + testtt.results.Count;
        }
        public string MultiRequest3()
        {

            //var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<List<ProductDetail>, List<ProductDetailDM>>("TestService", "report2", number);

            //int a = 5;
            //string b = "total : ";
            //List<int> aaa = new List<int>() { 0, 2, 3 };
            //TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            //pp.ManufactureID = 999;
            //pp.Name = "3roijogiorjgrweg";
            //List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //var testtt = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp);
            return "";// testtt.error + " - results: " + testtt.results.Count;
        }

        public ActionResult TestYiting(int? ID, int? ID2)
        {
            //List<int> indexList = new List<int>();
            //indexList.Add(1);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<MStoreItemCell>, List<MStoreItemCell>>("MobileStoreService", "GetMobileStoreItems", ID, ID2);
            //var ret2 = TWNewEgg.Framework.ServiceApi.Processor.Request<List<MStoreItemCell>, List<MStoreItemCell>>("MobileStoreService", "GetMobileStoreItems",7, ID);

            //ViewBag.Result = ret2.results;
            ViewBag.Result = null;
            return View();
        }

        public ActionResult Index()
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;

            listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;

            //int a = 5;
            //string b = "total : ";
            //List<int> aaa = new List<int>() { 0, 2, 3 };
            //TWNewEgg.Models.ViewModels.Product.ProductDetail pp = new Models.ViewModels.Product.ProductDetail();
            //pp.ManufactureID = 999;
            //pp.Name = "3roijogiorjgrweg";
            //List<TWNewEgg.Models.ViewModels.Product.ProductDetail> test = new List<TWNewEgg.Models.ViewModels.Product.ProductDetail>();
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test1 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //TWNewEgg.Models.ViewModels.Product.ProductDetail test2 = new TWNewEgg.Models.ViewModels.Product.ProductDetail();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //Random randdd = new Random();
            //List<ComplexProduntDM> test3 = new List<ComplexProduntDM>();
            //for (int i = 0; i < 5; i++)
            //{
            //    ComplexProduntDM newPDM = new ComplexProduntDM();
            //    newPDM.byteTest = Convert.ToByte(randdd.Next(00, 99));
            //    newPDM.shortTest = Convert.ToInt16(randdd.Next(00, 99));
            //    newPDM.intTest = randdd.Next(00, 99);
            //    newPDM.longTest = Convert.ToInt64(randdd.Next(00, 99));
            //    newPDM.floatTest = Convert.ToSingle(randdd.Next(00, 99));
            //    newPDM.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
            //    newPDM.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
            //    newPDM.stringTest = randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString();
            //    newPDM.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
            //    newPDM.dateTimeTest = DateTime.Now;

            //    newPDM.byteNullTest = null;
            //    newPDM.shortNullTest = null;
            //    newPDM.intNullTest = null;
            //    newPDM.longNullTest = null;
            //    newPDM.floatNullTest = null;
            //    newPDM.doubleNullTest = null;
            //    newPDM.decimalNullTest = null;
            //    newPDM.boolNullTest = null;
            //    newPDM.dateTimeNullTest = null;

            //    newPDM.complexProductDMTest = new List<ComplexProduntDM>();
            //    newPDM.complexProductDMNullTest = null;

            //    ComplexProduntDM newPDM2 = new ComplexProduntDM();
            //    newPDM2.byteTest = Convert.ToByte(randdd.Next(00, 99));
            //    newPDM2.shortTest = Convert.ToInt16(randdd.Next(00, 99));
            //    newPDM2.intTest = randdd.Next(00, 99);
            //    newPDM2.longTest = Convert.ToInt64(randdd.Next(00, 99));
            //    newPDM2.floatTest = Convert.ToSingle(randdd.Next(00, 99));
            //    newPDM2.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
            //    newPDM2.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
            //    newPDM2.stringTest = randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString() + randdd.Next(00, 99).ToString();
            //    newPDM2.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
            //    newPDM2.dateTimeTest = DateTime.Now;

            //    newPDM2.byteNullTest = null;
            //    newPDM2.shortNullTest = null;
            //    newPDM2.intNullTest = null;
            //    newPDM2.longNullTest = null;
            //    newPDM2.floatNullTest = null;
            //    newPDM2.doubleNullTest = null;
            //    newPDM2.decimalNullTest = null;
            //    newPDM2.boolNullTest = null;
            //    newPDM2.dateTimeNullTest = null;

            //    newPDM2.complexProductDMTest = new List<ComplexProduntDM>();
            //    newPDM2.complexProductDMNullTest = null;

            //    newPDM.complexProductDMTest.Add(newPDM2);
            //    test3.Add(newPDM);
            //}
            ////var c = new { e = 9, f = "10" };
            //ViewBag.ccTimeStart = DateTime.Now;
            //ViewBag.cc = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test", a, b, aaa, pp).results;
            //ViewBag.ccTimeEnd = DateTime.Now;
            //ViewBag.ddTimeStart = DateTime.Now;
            //ViewBag.dd = TWNewEgg.Framework.ServiceApi.Processor.Request<List<string>, List<string>>("TestService", "test2").results;
            //ViewBag.ddTimeEnd = DateTime.Now;
            //ViewBag.eeTimeStart = DateTime.Now;
            //ViewBag.ee = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.DomainModels.Product.ProductDetailDM, TWNewEgg.Models.DomainModels.Product.ProductDetailDM>("TestService", "test3", a, b, aaa, pp).results;
            //ViewBag.eeTimeEnd = DateTime.Now;
            //ViewBag.ffTimeStart = DateTime.Now;
            //ViewBag.ff = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test4", a, b, aaa, pp).results;
            //ViewBag.ffTimeEnd = DateTime.Now;
            //ViewBag.ggTimeStart = DateTime.Now;
            //ViewBag.gg = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TestService", "test5", a, b, aaa, test).results;
            //ViewBag.ggTimeEnd = DateTime.Now;
            //ViewBag.hhTimeStart = DateTime.Now;
            //ViewBag.hh = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("TestService", "test6", null).results;
            //ViewBag.hhTimeEnd = DateTime.Now;
            //ViewBag.iiTimeStart = DateTime.Now;
            //ViewBag.ii = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.DomainModels.Product.ComplexProduntDM>, List<TWNewEgg.Models.DomainModels.Product.ComplexProduntDM>>("TestService", "test7", a, b, aaa, test3).results;
            //ViewBag.iiTimeEnd = DateTime.Now;
            //var ee = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.ViewModels.Product.ProductDetail, TWNewEgg.Models.DomainModels.Product.ProductDetailDM>("TWNewEgg.Services.TestService", "test3", a, b, aaa, pp);
            //var ff = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.ViewModels.Product.ProductDetail>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TWNewEgg.Services.TestService", "test4", a, b, aaa, pp);
            //var gg = TWNewEgg.Framework.ServiceApi.Processor.Request<List<TWNewEgg.Models.ViewModels.Product.ProductDetail>, List<TWNewEgg.Models.DomainModels.Product.ProductDetailDM>>("TWNewEgg.Services.TestService", "test5", a, b, aaa, test);
            return View();
        }

        public string aa { get; set; }
        public string bb { get; set; }

    }
}
