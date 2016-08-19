using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.ECWeb.PrivilegeFilters;
using Newtonsoft.Json;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.ECWeb_Mobile.Auth;
using TWNewEgg.Models;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    [AllowNonSecures]
    public class CouponController : ApiController
    {
        /// <summary>
        /// 依照Coupon代碼取得Coupon
        /// </summary>
        /// <param name="CouponMarketNumber">Coupon的Marketnumber</param>
        /// <returns></returns>
        [HttpGet]
        public string addDynamicCouponByCouponMarketNumber(string CouponMarketNumber)
        {
            //if (NEUser.ID == null)
            //{
            //    return "";
            //}
            TWNewEgg.Models.ViewModels.Redeem.Coupon.AddCouponStatusOption oAddResult;
            string strResult = "";
            string strAccountId = "";

            strAccountId =  NEUser.ID.ToString();

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


    }
}
