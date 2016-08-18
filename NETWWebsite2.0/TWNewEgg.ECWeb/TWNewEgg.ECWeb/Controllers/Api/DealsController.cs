using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Lottery;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Redeem.Service.CouponService;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Models.ViewModels.Redeem;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class DealsController : ApiController
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        /// <summary>
        /// 取得抽獎結果
        /// </summary>
        /// <returns>null or DrawResult</returns>
        [HttpPost]
        public TWNewEgg.Models.DomainModels.Lottery.DrawResult GetLotteryResult()
        {
            if (NEUser.ID == null || NEUser.ID <= 0)
            {
                return null;
            }

            DrawResult objResult = null;
            ICouponService objCouponService = null;

            //以下已在抽獎
            //function: Processor.Request<DrawResult, DrawResult>("LotteryService", "DrawAwrad", AccountId, GameId);
            var res = Processor.Request<DrawResult, DrawResult>("LotteryService", "DrawAwrad", NEUser.ID, 4);
            CouponServiceRepository.AddCouponStatusOption optCouponResult = CouponServiceRepository.AddCouponStatusOption.未處理;

            //判斷Coupon歸戶
            if (res.results.Award != null)
            {
                try
                {
                    //1000元的Coupon
                    switch (res.results.Award.Type)
                    {
                        case 5:
                            //1000元折價券=500元折價券2張
                            /*
                            optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_1000X2", NEUser.ID.ToString());
                            optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_1000X2", NEUser.ID.ToString());*/
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_1000X2", NEUser.ID.ToString()).results;
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_1000X2", NEUser.ID.ToString()).results;
                            break;
                        case 4:
                            //500元折價券
                            //optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_500X2", NEUser.ID.ToString());
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_500X2", NEUser.ID.ToString()).results;
                            break;
                        case 3:
                            //300元折價券
                            //optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_300X2", NEUser.ID.ToString());
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_300X2", NEUser.ID.ToString()).results;
                            break;
                        case 2:
                            //200元折價券
                            //optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_200X2", NEUser.ID.ToString());
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_200X2", NEUser.ID.ToString()).results;
                            break;
                        case 1:
                            //100元折價券
                            //optCouponResult = objCouponService.addDynamicCouponByCouponMarketNumberAndUserAccount("F_COOKIE_100X2", NEUser.ID.ToString());
                            optCouponResult = Processor.Request<CouponServiceRepository.AddCouponStatusOption, CouponServiceRepository.AddCouponStatusOption>("Service.CouponService.CouponServiceRepository", "addDynamicCouponByCouponMarketNumberAndUserAccount", "F_COOKIE_100X2", NEUser.ID.ToString()).results;
                            break;

                    }////end switch

                    objCouponService = null;
                }
                catch (Exception ex)
                {
                    string strError = "\n\n【api/DealsController.cs】\r\n";
                    strError += String.Format("DateTime: {0:u}", DateTime.Now) + "\r\n";
                    strError += "UserId:" + NEUser.ID.ToString() + "\r\n";
                    strError += "Award ID:" + res.results.Award.ID + "\r\n";
                    strError += "Award Name:" + res.results.Award.Name + "\r\n";
                    strError += "Award Type:" + res.results.Award.Type.ToString() + "\r\n";
                    strError += "Error Message:" + ex.Message + "\r\n" + ex.StackTrace + "\r\n";
                    logger.Info(strError);
                }
            }

            //寄通知信
            if (res.results.Award != null)
            {
                //SendEmail(account, res.results.Award);
            }

            objResult = res.results;
            return objResult;
        }

    }
}
