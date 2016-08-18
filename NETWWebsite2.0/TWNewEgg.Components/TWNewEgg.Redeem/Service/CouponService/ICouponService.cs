using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.CouponService
{
    public interface ICouponService 
    {
        Models.DomainModels.Redeem.Coupon getCouponById(int arg_nCouponId);
        Models.DomainModels.Redeem.Coupon getCouponByNumber(string arg_strCouponNumber);
        List<Models.DomainModels.Redeem.Coupon> GetCouponByIdList(List<int> argListId);
        List<Models.DomainModels.Redeem.Coupon> GetCouponByOrdCode(List<string> argListOrdCode);
        List<Models.DomainModels.Redeem.Coupon> GetCouponByAccountIdAndEventId(string argStrAccountId, int argNumEventId);
        Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption addDynamicCouponByCouponMarketNumberAndUserAccount(string arg_strCouponNumber, string arg_strUserAccount);
        List<Models.DomainModels.Redeem.Coupon> getUsedCouponListByAccount(string arg_strAccount);
        List<Models.DomainModels.Redeem.Coupon> getUsedCouponListByAccountByDate(string arg_strAccount, string arg_strFromDate, string arg_strEndDate);
        List<Models.DomainModels.Redeem.Coupon> getUsedCouponIn3MonthListByAccount(string arg_strAccount);
        List<Models.DomainModels.Redeem.Coupon> getActiveCouponListByAccount(string arg_strAccount);
        List<Models.DomainModels.Redeem.Coupon> getActiveCouponListByAccountAndDate(string arg_strAccount, string arg_strFromDate, string arg_strEndDate);
        List<Models.DomainModels.Redeem.Coupon> getAllCouponListByAccount(string arg_strAccount);
        List<Models.DomainModels.Redeem.Coupon> getWaitingActiveCouponByAccount(string arg_strAccount);
        bool editCoupon(Models.DomainModels.Redeem.Coupon arg_oCoupon);
        bool editCouponList(List<Models.DomainModels.Redeem.Coupon> arg_listCoupons);
        List<Models.DomainModels.Redeem.Coupon> getExpiredCouponListByAccount(string arg_strAccount);
        bool cancelCoupon(Models.DomainModels.Redeem.Coupon arg_oCoupon);
        Models.DomainModels.Redeem.Coupon getActiveCouponByCouponId(int arg_nCouponId);
        Redeem.Service.CouponService.CouponServiceRepository.AddCouponStatusOption addDynamicCouponByEventIdAndUserAccount(int arg_nEventId, string arg_strAccount);
        List<Models.DomainModels.Redeem.CouponsUsedStatusReport> GetAllCouponUsedStatusReport();
        Models.DomainModels.Redeem.Coupon GetCouponByCouponId(int argNumCouponId);
        Models.DomainModels.Redeem.Coupon GetCouponByCouponNumber(string argStrCouponNumber);
        void ExecGrantCouponAllUser();
    }//end interface
}//end namespace
