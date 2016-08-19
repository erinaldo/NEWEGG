using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.EventRepoAdapters.Interface
{
    public interface ICouponRepoAdapter
    {

        /// <summary>
        /// 取得所有的Coupon
        /// </summary>
        /// <returns></returns>
        IQueryable<Coupon> Coupon_GetAll();

        /// <summary>
        /// 新增Coupon
        /// </summary>
        /// <param name="argObjCoupon">新增的Coupon</param>
        void CreateCoupon(Coupon argObjCoupon);

        /// <summary>
        /// 修改Coupon
        /// </summary>
        /// <param name="argObjCoupon">要修改的Coupon</param>
        /// <returns>update success return ture, else return false</returns>
        bool UpdateCoupon(Coupon argObjCoupon);

        /// <summary>
        /// 修改一列Coupon
        /// </summary>
        /// <param name="argListCoupon">list of coupon</param>
        /// <returns>update success return ture, else return false</returns>
        bool UpdateCouponRange(List<Coupon> argListCoupon);

        /// <summary>
        /// 根據Id取得Coupon
        /// </summary>
        /// <param name="argNumId">Coupon ID</param>
        /// <returns></returns>
        IQueryable<Coupon> GetCouponById(int argNumId);

        /// <summary>
        /// 新增動態coupon
        /// </summary>
        /// <param name="arg_nEventId"></param>
        /// <param name="arg_strAccount"></param>
        /// <returns></returns>
        int addDynamicCouponByEventIdAndUserAccount(int arg_nEventId, string arg_strAccount);

        /// <summary>
        /// 自動發送系統coupon的store procedure
        /// </summary>
        void ExecGrantCouponAllUser();

        IQueryable <CouponActivity> CouponActivity();
    }
}
