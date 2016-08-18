using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.Redeem.Service.CouponService;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.ECWeb.PrivilegeFilters;
using Newtonsoft.Json;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Models;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    [AllowNonSecures]
    public class CouponController : ApiController
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //private ICouponService couponService = new CouponServiceRepository();
        //private IEvent eventService = new EventReponsitory();
        private string loginStatus = "1"; //0 = false, 1 = true

        private static DateTime _cacheTime = new DateTime();
        private static Dictionary<int, int> _couponNumber = new Dictionary<int, int>();
        private static string _environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        private static int _loginEventID = new int();


        //[RequireHttps]
        public TWNewEgg.Models.ViewModels.Redeem.UserCouponsProducts getCouponsProducts()
        {

            ICarts repository = new CartsRepository();   
            IEnumerable<CartItems> userOverSeaBuyNow = null;
            Dictionary<string, string> itemParentCategory = null;


            userOverSeaBuyNow = repository.GetTrackAll(11, 0); //GET all final shopping cart items from Stored procedure
            var itemIDs = userOverSeaBuyNow.Where(x => x.ItemListID == null).ToList();
            itemParentCategory = repository.GetRootCategorybyItemIDs(itemIDs.Select(x => x.ItemID).ToList());
            TWNewEgg.Models.ViewModels.Redeem.UserCouponsProducts userCouponsProducts = new TWNewEgg.Models.ViewModels.Redeem.UserCouponsProducts();
            userCouponsProducts.coupons = new List<TWNewEgg.Models.ViewModels.Redeem.CouponsLite>();
            userCouponsProducts.items = new List<TWNewEgg.Models.ViewModels.Redeem.ItemLite>();
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> userCoupons = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", NEUser.ID.ToString()).results;
            if (userCoupons == null || userCoupons.Count == 0)
            {
                userCoupons = new List<TWNewEgg.Models.ViewModels.Redeem.Coupon>();
            }
            userCouponsProducts.coupons.AddRange(userCoupons.Select(
                x => new TWNewEgg.Models.ViewModels.Redeem.CouponsLite {
                    couponID = x.id.ToString(), 
                    couponName = x.title,
                    couponValue = x.value.ToString(), 
                    couponCategories = x.categories.ToString(), 
                    couponProductID = "", couponLimit = x.limit.ToString(),
                    couponEndDate = x.validend.ToString("yyyy/MM/dd"),
                    couponItems = x.items
                }).ToList());
            //userCouponsProducts.items.AddRange(itemIDs.Select(x => new itemLite { itemID = x.ItemID.ToString(), itemParentCategoryID = "0" }));
            userCouponsProducts.items.AddRange(itemParentCategory.Select(x => new TWNewEgg.Models.ViewModels.Redeem.ItemLite { itemID = x.Key, itemParentCategoryID = x.Value }));
            return userCouponsProducts;
        }//end of getCouponsProducts

        /// <summary>
        /// 依照Coupon代碼取得Coupon
        /// </summary>
        /// <param name="CouponMarketNumber">Coupon的Marketnumber</param>
        /// <returns></returns>
        [HttpGet]
        public string addDynamicCouponByCouponMarketNumber(string CouponMarketNumber)
        {
            if(NEUser.ID == null)
            {
                return "";
            }
            TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption oAddResult;
            string strResult = "";
            TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository oCouponService = null;
            string strAccountId = "";

            strAccountId = NEUser.ID.ToString();

            if (strAccountId.Equals("0"))
            {
                strResult = "登入帳號錯誤，請重新登入！";
            }
            else
            {
                oAddResult = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption, TWNewEgg.Models.DomainModels.Redeem.Coupon.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", CouponMarketNumber, strAccountId).results;

                switch (oAddResult)
                {
                    case TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption.活動尚未開始或活動已截止:
                        strResult = "活動尚未開始或活動已截止";
                        break;
                    case TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption.User已領過:
                        strResult = "您已經兌換過囉！";
                        break;
                    case TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption.Coupon發送已達上限:
                        strResult = "折價券已經發完囉！";
                        break;
                    case TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption.發送失敗:
                        strResult = "兌換失敗！";
                        break;
                    case TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption.發送成功:
                        strResult = "兌換成功！";
                        break;
                    default:
                        strResult = "兌換失敗！";
                        break;
                }//end switch

            }

            return strResult;
        }//return getDynamicCoupon

        /// <summary>
        /// 根據UserID及EventId取得Coupon列表
        /// </summary>
        /// <param name="EventId">Event Id</param>
        /// <returns>Coupon列表或是null</returns>
        [HttpGet]
        public List<TWNewEgg.Models.ViewModels.Redeem.Coupon> GetCouponByAccountAndCouponMarketNumber(int EventId)
        {
            if (NEUser.ID == null)
            {
                return null;
            }

            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listCoupon = null;
            string strAccountId = "";

            //驗證登入,未登入或非本人都不可查詢
            strAccountId = Convert.ToString(NEUser.ID).Trim();
            if (!strAccountId.Equals("0"))
            {
                listCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", strAccountId, EventId).results;
            }

            return listCoupon;
        }//end of GetCouponByAccountAndCouponMarketNumber


        [HttpGet]
        public string GetCouponNumber(int EventId)
        {
            EventReponsitory objEventService = null;
            var today = DateTime.UtcNow.AddHours(8);
            int couponNumber = new int();
            bool dicFlag = new bool();
            TWNewEgg.Models.ViewModels.Redeem.Event objEvent = null;

            dicFlag = _couponNumber.ContainsKey(EventId);

            if (DateTime.Compare(_cacheTime.AddSeconds(30), today) < 0)
            {
                logger.Info("DateTime Coupon : [CT]" + _cacheTime.ToString() + " [Count]" + _couponNumber.Count.ToString());
                _cacheTime = today;
                _loginEventID = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[_environment + "_CouponID"]);

                objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.CouponServiceRepository", "getEventById", EventId).results;
                if (objEvent != null)
                {
                    if (dicFlag == false)
                    {
                        _couponNumber.Add(EventId, 0);
                    }
                    _couponNumber[EventId] = objEvent.couponmax.Value - objEvent.couponsum;

                    // Event ID = 189 then -100 start
                    if (EventId == _loginEventID)
                    {
                        _couponNumber[EventId] -= 100;
                    }
                    // Event ID = 189 then -100 end


                    if (_couponNumber[EventId] < 0)
                    {
                        _couponNumber[EventId] = 0;
                    }
                    return _couponNumber[EventId].ToString();
                }
                else
                {
                    return "0";
                }


            }
            else
            {
                if (dicFlag)
                {
                    return _couponNumber[EventId].ToString();
                }
                else
                {
                    objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.CouponServiceRepository", "getEventById", EventId).results;
                    if (objEvent != null && today >= objEvent.datestart && today < objEvent.dateend)
                    {
                        if (dicFlag == false)
                        {
                            _couponNumber.Add(EventId, 0);
                        }
                        _couponNumber[EventId] = objEvent.couponmax.Value - objEvent.couponsum;

                        // Event ID = 189 then -100 start
                        if (EventId == _loginEventID)
                        {
                            _couponNumber[EventId] -= 100;
                        }
                        // Event ID = 189 then -100 end


                        if (_couponNumber[EventId] < 0)
                        {
                            _couponNumber[EventId] = 0;
                        }
                        return _couponNumber[EventId].ToString();
                    }
                    else
                    {
                        return "0";
                    }
                }
            }

            return "0";
        }

        [HttpGet]
        public TWNewEgg.Models.ViewModels.Redeem.Event GetEventById(int EventId)
        {
            if (NEUser.ID == null)
            {
                return null;
            }
            TWNewEgg.Models.ViewModels.Redeem.Event objEvent = null;
            string strAccountId = "";

            //驗證登入,未登入或非本人都不可查詢
            strAccountId = Convert.ToString(NEUser.ID);
            if (!strAccountId.Equals("0"))
            {
                objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.CouponServiceRepository", "getEventById", EventId).results;
            }

            return objEvent;
        }

        [HttpGet]
        public TWNewEgg.Models.ViewModels.Redeem.Event GetActiveEventById(int EventId)
        {
            TWNewEgg.Models.ViewModels.Redeem.Event objEvent = null;
            string strAccountId = "";

            //驗證登入,未登入或非本人都不可查詢
            strAccountId = Convert.ToString(NEUser.ID).Trim();
            if (!strAccountId.Equals("0"))
            {
                objEvent = Processor.Request<TWNewEgg.Models.ViewModels.Redeem.Event, TWNewEgg.Models.DomainModels.Redeem.Event>("Service.CouponService.CouponServiceRepository", "GetActiveEventById", EventId).results;
            }

            return objEvent;
        }

    }
}
