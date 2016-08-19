using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.AccountServices.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.Redeem.Service.CouponService
{
    public class CouponServiceRepository : ICouponService
    {

        private ICouponRepoAdapter _CouponRepo = null;
        private IEventRepoAdapter _EventRepo = null;
        private IAccountService _AccountService = null;
        private ICouponService _couponService = null;
        public CouponServiceRepository(ICouponRepoAdapter argCouponRepo, IEventRepoAdapter argEventRepo, IAccountService argAccountService)
        {
            this._CouponRepo = argCouponRepo;
            this._EventRepo = argEventRepo;
            this._AccountService = argAccountService;
        }

        public enum AddCouponStatusOption
        {
            未處理 = 0,
            活動尚未開始或活動已截止 = 1,
            Coupon發送已達上限 = 2,
            User已領過 = 3,
            發送成功 = 4,
            發送失敗 = 5
        }

        /// <summary>
        /// 根據coupon ip取得coupon
        /// </summary>
        /// <param name="arg_nCouponId"></param>
        /// <returns></returns>
        public Models.DomainModels.Redeem.Coupon getCouponById(int arg_nCouponId)
        {
            Models.DBModels.TWSQLDB.Coupon objDbCoupon = null;
            Models.DomainModels.Redeem.Coupon oCoupon = null;


            objDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.id == arg_nCouponId).SingleOrDefault();
            if (objDbCoupon != null)
            {
                oCoupon = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Coupon>(objDbCoupon);
            }

            return oCoupon;
        }//end getCouponById()

        /// <summary>
        /// 根據Coupon的Number取得Coupon
        /// </summary>
        /// <param name="arg_strCouponNumber">CouponNumber</param>
        /// <returns></returns>
        public Models.DomainModels.Redeem.Coupon getCouponByNumber(string arg_strCouponNumber)
        {
            Models.DBModels.TWSQLDB.Coupon objDbCoupon = null;
            Models.DomainModels.Redeem.Coupon oCoupon = null;


            objDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.number == arg_strCouponNumber).SingleOrDefault();
            if (objDbCoupon != null)
            {
                oCoupon = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Coupon>(objDbCoupon);
            }

            return oCoupon;
        }

        public List<Models.DomainModels.Redeem.Coupon> GetCouponByIdList(List<int> argListId)
        {
            if (argListId == null || argListId.Count <= 0)
            {
                return null;
            }

            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;

            listDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => argListId.Contains(x.id)).ToList();

            if (listDbCoupon != null && listDbCoupon.Count > 0)
            {
                listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
            }

            return listCoupon;
        }

        public List<Models.DomainModels.Redeem.Coupon> GetCouponByOrdCode(List<string> argListOrdCode)
        {
            if (argListOrdCode == null || argListOrdCode.Count <= 0)
            {
                return null;
            }

            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;

            listDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => argListOrdCode.Contains(x.ordcode)).ToList();

            if (listDbCoupon != null && listDbCoupon.Count > 0)
            {
                listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
            }

            return listCoupon;
        }


        /// <summary>
        /// 取得該User所有Coupon EventId為傳入參數的列表
        /// </summary>
        /// <param name="argStrAccountId">User Account Id</param>
        /// <param name="argNumEventId">Evnet Id</param>
        /// <returns>Coupon列表</returns>
        public List<Models.DomainModels.Redeem.Coupon> GetCouponByAccountIdAndEventId(string argStrAccountId, int argNumEventId)
        {
            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;

            if (argStrAccountId == null || argStrAccountId.Length <= 0 || argNumEventId == 0)
                return null;

            int accountID = new int();
            bool bAccountCheck = int.TryParse(argStrAccountId, out accountID);

            if (bAccountCheck)
            {
                listDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == argStrAccountId && x.eventid == argNumEventId).ToList();
                if (listDbCoupon != null && listDbCoupon.Count > 0)
                {
                    listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
                }
            }

            return listCoupon;
        }

        /// <summary>
        /// 根據coupon Market number 取得coupon
        /// </summary>
        /// <param name="CouponNumber">coupon number</param>
        /// <returns>coupon object, or null</returns>
        public AddCouponStatusOption addDynamicCouponByCouponMarketNumberAndUserAccount(string arg_strCouponNumber, string arg_strUserAccount)
        {
            Models.DBModels.TWSQLDB.Event objDbEvent = null;
            DateTime oDateNow = DateTime.Now;
            AddCouponStatusOption oResultOption = AddCouponStatusOption.未處理;
            Models.DomainModels.Account.MemberDM objAccount = null;

            //檢查User Account是否Active
            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strUserAccount, out accountID);
            if (!bAccountCheck)
            {
                objAccount = this._AccountService.GetMemberDMByEmail(arg_strUserAccount);
            }
            else
            {
                objAccount = this._AccountService.GetMember(accountID);
            }

            if (objAccount == null)
            {
                return AddCouponStatusOption.發送失敗;
            }

            objDbEvent = this._EventRepo.Event_GetAll().Where(x => x.couponmarketnumber == arg_strCouponNumber && x.datestart <= oDateNow && x.dateend >= oDateNow).FirstOrDefault();
            if (objDbEvent == null)
            {
                return AddCouponStatusOption.活動尚未開始或活動已截止;
            }

            oResultOption = this.addDynamicCouponByEventIdAndUserAccount(objDbEvent.id, objAccount.AccID.ToString());


            objDbEvent = null;

            return oResultOption;
        }//end addDynamicCouponByCouponMarketNumberAndUserAccount

        /// <summary>
        /// 取得這個帳號所有已使用過的coupon
        /// </summary>
        /// <param name="Account">user account</param>
        /// <returns>list of coupon , or null</returns>
        public List<Models.DomainModels.Redeem.Coupon> getUsedCouponListByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listUsedCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbUsedCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;

            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listUsedCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listUsedCoupon;
            }

            DateTime dateTimeNow = DateTime.Now;

            listDbUsedCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.Used || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.UsedButCancel)).ToList();
            if (listDbUsedCoupon != null && listDbUsedCoupon.Count > 0)
            {
                listUsedCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbUsedCoupon);
                replaceCouponTitle(ref listUsedCoupon);
            }

            return listUsedCoupon;
        }//end getUsedCouponListByAccount

        /// <summary>
        /// 根據日期區間取得這個帳號所有已使用過的coupon
        /// </summary>
        /// <param name="arg_strAccount">user account</param>
        /// <param name="arg_strFromDate">From Date</param>
        /// <param name="arg_strEndDate">End Date</param>
        /// <returns>list of coupon , or null</returns>
        public List<Models.DomainModels.Redeem.Coupon> getUsedCouponListByAccountByDate(string arg_strAccount, string arg_strFromDate, string arg_strEndDate)
        {
            List<Models.DomainModels.Redeem.Coupon> listUsedCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbUsedCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            DateTime dateFrom = DateTime.Now;
            DateTime dateEnd = DateTime.Now;

            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listUsedCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listUsedCoupon;
            }

            //進行日期格式轉換
            try
            {
                dateFrom = Convert.ToDateTime(arg_strFromDate);
                dateEnd = Convert.ToDateTime(arg_strEndDate);
            }
            catch
            {
            }

            listDbUsedCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount 
                && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.Used || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.UsedButCancel) 
                && x.validstart >= dateFrom && x.validend <= dateEnd).ToList();

            if (listDbUsedCoupon != null && listDbUsedCoupon.Count > 0)
            {
                listUsedCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbUsedCoupon);
                replaceCouponTitle(ref listUsedCoupon);
            }

            return listUsedCoupon;
        }//end getUsedCouponListByAccount

        /// <summary>
        /// 取得這個帳號最近三個月所有已使用過的coupon
        /// </summary>
        /// <param name="Account">user account</param>
        /// <returns>list of coupon , or null</returns>
        public List<Models.DomainModels.Redeem.Coupon> getUsedCouponIn3MonthListByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listUsedCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbUsedCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            DateTime oDateTime = DateTime.Now.AddMonths(-3);

            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listUsedCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listUsedCoupon;
            }

            DateTime dateTimeNow = DateTime.Now;

            //只列出3個月內的消費記錄
            listDbUsedCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount 
                && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.Used || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.UsedButCancel) 
                && x.usedate >= oDateTime).ToList();
            if (listDbUsedCoupon != null && listDbUsedCoupon.Count > 0)
            {
                listUsedCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbUsedCoupon);
                replaceCouponTitle(ref listUsedCoupon);
            }

            return listUsedCoupon;
        }//end getUsedCouponListByAccount

        /// <summary>
        /// 取得這個帳號目前有效可以使用的coupon
        /// </summary>
        /// <param name="Account">user account</param>
        /// <returns>list of coupon, or null</returns>
        public List<Models.DomainModels.Redeem.Coupon> getActiveCouponListByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listActiveCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbActiveCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listActiveCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listActiveCoupon;
            }


            DateTime dateTimeNow = DateTime.Now;
            /*
             * activeCouponList = (from x in twSql.Coupon
                                join e in twSql.Event on x.eventid equals e.id
                                where x.accountid == arg_strAccount
                                && (x.usestatus == (int)Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Coupon.CouponUsedStatusOption.SetTempUsedForCheckout) && x.validstart <= dateTimeNow && x.validend >= dateTimeNow && (x.validtype == (int)Coupon.ValidTypeOption.System || x.validtype == (int)Coupon.ValidTypeOption.ByUser) && (x.activetype == (int)Coupon.CouponActiveTypeOption.SystemAutoActive || x.activetype == (int)Coupon.CouponActiveTypeOption.UserActive)
                                && e.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive
                                select x).ToList();
             */
            queryEventSearch = this._EventRepo.Event_GetAll().Where(x => x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);
            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount
                                && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.SetTempUsedForCheckout)
                                && x.validstart <= dateTimeNow && x.validend >= dateTimeNow
                                && (x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.System || x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.ByUser)
                                && (x.activetype == (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.SystemAutoActive || x.activetype == (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.UserActive));

            listDbActiveCoupon = (from x in queryCouponSearch join e in queryEventSearch on x.eventid equals e.id select x).ToList();

            //設定coupon的title
            if (listDbActiveCoupon != null && listDbActiveCoupon.Count > 0)
            {
                listActiveCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbActiveCoupon);
                this.replaceCouponTitle(ref listActiveCoupon);
            }
            return listActiveCoupon;
        }//end getActiveCouponListByAccount()

        /// <summary>
        /// 根據日期區間取得這個帳號目前有效可以使用的coupon
        /// </summary>
        /// <param name="arg_strAccount"></param>
        /// <param name="arg_strFromDate"></param>
        /// <param name="arg_strEndDate"></param>
        /// <returns></returns>
        public List<Models.DomainModels.Redeem.Coupon> getActiveCouponListByAccountAndDate(string arg_strAccount, string arg_strFromDate, string arg_strEndDate)
        {
            List<Models.DomainModels.Redeem.Coupon> listActiveCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbActiveCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            DateTime dateTimeFrom = DateTime.Now;
            DateTime dateTimeEnd = DateTime.Now;
            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listActiveCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listActiveCoupon;
            }

            try
            {
                dateTimeFrom = Convert.ToDateTime(arg_strFromDate);
                dateTimeEnd = Convert.ToDateTime(arg_strEndDate);
            }
            catch
            {
            }

            /*
            activeCouponList = (from x in twSql.Coupon
                                join e in twSql.Event on x.eventid equals e.id
                                where x.accountid == arg_strAccount
                                && (x.usestatus == (int)Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Coupon.CouponUsedStatusOption.SetTempUsedForCheckout) && x.validstart <= dateTimeEnd && x.validend >= dateTimeFrom && (x.validtype == (int)Coupon.ValidTypeOption.System || x.validtype == (int)Coupon.ValidTypeOption.ByUser) && (x.activetype == (int)Coupon.CouponActiveTypeOption.SystemAutoActive || x.activetype == (int)Coupon.CouponActiveTypeOption.UserActive)
                                && e.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive
                                select x).ToList();*/ 
            queryEventSearch = this._EventRepo.Event_GetAll().Where(x=>x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);
            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount
                                && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.SetTempUsedForCheckout)
                                && x.validstart <= dateTimeEnd && x.validend >= dateTimeFrom 
                                && (x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.System || x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.ByUser) 
                                && (x.activetype == (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.SystemAutoActive || x.activetype == (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.UserActive));

            listDbActiveCoupon = (from x in queryCouponSearch join e in queryEventSearch on x.eventid equals e.id select x).ToList();
            
            //設定coupon的title
            if (listDbActiveCoupon != null && listDbActiveCoupon.Count > 0)
            {
                listActiveCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbActiveCoupon);
                this.replaceCouponTitle(ref listActiveCoupon);
            }

            return listActiveCoupon;
        }

        /// <summary>
        /// 取得這個帳戶所有的Coupon(含未使用的, 但不包含系統一開始就不啟動的:Event.CouponActiveTypeOption.NotActive)
        /// </summary>
        /// <param name="Account">user account</param>
        /// <returns>list of coupon, or null</returns>
        public List<Models.DomainModels.Redeem.Coupon> getAllCouponListByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;

            if (!bAccountCheck)
            {
                return listCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {;
                return listCoupon;
            }


            DateTime dateTimeNow = DateTime.Now;

            /*
            listCoupon = (from x in twSql.Coupon
                          join e in twSql.Event on x.eventid equals e.id
                          where x.accountid == arg_strAccount
                          && x.validtype != (int)Coupon.ValidTypeOption.NotActive
                          && e.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive
                          select x).ToList();*/

            queryEventSearch = this._EventRepo.Event_GetAll().Where(x => x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);
            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount
                          && x.validtype != (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.NotActive);

            listDbCoupon = (from x in queryCouponSearch join e in queryEventSearch on x.eventid equals e.id select x).ToList();

            if (listDbCoupon != null && listDbCoupon.Count > 0)
            {
                listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
                //取代coupon title
                this.replaceCouponTitle(ref listCoupon);
            }

            return listCoupon;
        }//end getAllCouponListByAccount()

        /// <summary>
        /// 取得這個帳戶待生效的Coupon
        /// </summary>
        /// <param name="arg_strAccount"></param>
        /// <returns></returns>
        public List<Models.DomainModels.Redeem.Coupon> getWaitingActiveCouponByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            Models.DomainModels.Account.MemberDM objAccount = null;
            DateTime oDateTimeNow = DateTime.Now;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;

            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listCoupon;
            }

            DateTime dateTimeNow = DateTime.Now;

            /*
            listCoupon = (from x in twSql.Coupon join p in twSql.Event on x.eventid equals p.id
                              where x.accountid == arg_strAccount
                              && p.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive
                              && x.validstart > oDateTimeNow 
                              && (
                                    (x.usestatus == (int)Coupon.CouponUsedStatusOption.ActiveButNotUsed && x.validtype == (int)Coupon.ValidTypeOption.System || x.validtype == (int)Coupon.ValidTypeOption.WaitingForSystemActive)
                                || (x.validtype == (int)Coupon.ValidTypeOption.ByUser && x.activetype == (int)Coupon.CouponActiveTypeOption.UserActive)
                                )
                              select x).ToList();*/

            queryEventSearch = this._EventRepo.Event_GetAll().Where(x => x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);
            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount
                              && x.validstart > oDateTimeNow
                              && (
                                    (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.ActiveButNotUsed && x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.System || x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.WaitingForSystemActive)
                                || (x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.ByUser && x.activetype == (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.UserActive)
                                ));

            listDbCoupon = (from x in queryCouponSearch join e in queryEventSearch on x.eventid equals e.id select x).ToList();

            if (listDbCoupon != null && listDbCoupon.Count > 0)
            {
                listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
                replaceCouponTitle(ref listCoupon);
            }

            return listCoupon;
        }

        /// <summary>
        /// 修改coupon
        /// </summary>
        /// <param name="Coupon">Coupon物件</param>
        /// <returns>true: 修改成功; false: 修改失敗</returns>
        public bool editCoupon(Models.DomainModels.Redeem.Coupon arg_oCoupon)
        {
            return false;
        }//end editCoupon

        /// <summary>
        /// 修改一列coupon
        /// </summary>
        /// <param name="arg_listCoupons"></param>
        /// <returns></returns>
        public bool editCouponList(List<Models.DomainModels.Redeem.Coupon> arg_listCoupons)
        {
            bool bError = false;
            List<int> aryCouponId = null;
            List<Models.DBModels.TWSQLDB.Coupon> listOldDbCoupon = null;
            Models.DomainModels.Redeem.Coupon oNewCoupon = null;

            if (arg_listCoupons == null || arg_listCoupons.Count <= 0)
                return false;

            aryCouponId = arg_listCoupons.Select(x => x.id).ToList();
            listOldDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => aryCouponId.Contains(x.id)).ToList();

            if (listOldDbCoupon != null && listOldDbCoupon.Count > 0)
            {
                foreach (Models.DBModels.TWSQLDB.Coupon oOldDbCoupon in listOldDbCoupon)
                {
                    oNewCoupon = arg_listCoupons.FirstOrDefault(x => x.id == oOldDbCoupon.id);
                    if (oNewCoupon == null)
                        continue;

                    oOldDbCoupon.activetype = oNewCoupon.activetype;
                    oOldDbCoupon.ordcode = oNewCoupon.ordcode;
                    oOldDbCoupon.updated = oOldDbCoupon.updated + 1;
                    oOldDbCoupon.updatedate = DateTime.Now;
                    oOldDbCoupon.updateuser = oNewCoupon.updateuser;
                    oOldDbCoupon.usedate = oNewCoupon.usedate;
                    oOldDbCoupon.usestatus = oNewCoupon.usestatus;
                }//end foreach

                this._CouponRepo.UpdateCouponRange(listOldDbCoupon);
            }
            else
            {
                bError = true;
            }

            return bError;
        }

        /// <summary>
        /// 取得已過期的Coupon
        /// </summary>
        /// <param name="arg_strAccount"></param>
        /// <returns></returns>
        public List<Models.DomainModels.Redeem.Coupon> getExpiredCouponListByAccount(string arg_strAccount)
        {
            List<Models.DomainModels.Redeem.Coupon> listCoupon = null;
            List<Models.DBModels.TWSQLDB.Coupon> listDbCoupon = null;
            DateTime oDateTimeNow = DateTime.Now;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;
            Models.DomainModels.Account.MemberDM objAccount = null;

            int accountID = new int();
            bool bAccountCheck = int.TryParse(arg_strAccount, out accountID);
            if (!bAccountCheck)
            {
                return listCoupon;
            }

            //檢查User Account是否Active
            objAccount = this._AccountService.GetMember(accountID);
            if (objAccount == null)
            {
                return listCoupon;
            }

            /*
            listCoupon = twSql.Coupon.Where(x => x.accountid == arg_strAccount
                && (x.validtype == (int)Coupon.ValidTypeOption.System || x.validtype == (int)Coupon.ValidTypeOption.ByUser) 
                && (x.usestatus == (int)Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Coupon.CouponUsedStatusOption.NotUsedButExpired || x.usestatus == (int)Coupon.CouponUsedStatusOption.SetTempUsedForCheckout)
                && x.validend < oDateTimeNow).ToList();*/

            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.accountid == arg_strAccount
                && (x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.System || x.validtype == (int)Models.DBModels.TWSQLDB.Coupon.ValidTypeOption.ByUser)
                && (x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.ActiveButNotUsed || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.NotUsedButExpired || x.usestatus == (int)Models.DBModels.TWSQLDB.Coupon.CouponUsedStatusOption.SetTempUsedForCheckout)
                && x.validend < oDateTimeNow);

            listDbCoupon = queryCouponSearch.ToList();

            if (listDbCoupon != null && listDbCoupon.Count > 0)
            {
                listCoupon = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Coupon>>(listDbCoupon);
                this.replaceCouponTitle(ref listCoupon);
            }

            return listCoupon;
        }

        /// <summary>
        /// 僅修改useStatus、updateuser、upatedate欄位,及附加note欄位, 若需要修改更多資料, 請使用editCoupon功能
        /// </summary>
        /// <param name="Coupon"></param>
        /// <returns></returns>
        public bool cancelCoupon(Models.DomainModels.Redeem.Coupon arg_oCoupon)
        {
            return false;
        }//end cancelCoupon

        /// <summary>
        /// 根據CouponID取得Active的Coupon物件
        /// </summary>
        /// <param name="arg_nCouponId"></param>
        /// <returns></returns>
        public Models.DomainModels.Redeem.Coupon getActiveCouponByCouponId(int arg_nCouponId)
        {
            Models.DomainModels.Redeem.Coupon oCoupon = null;
            Models.DBModels.TWSQLDB.Coupon objDbCoupon = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;

            /*
            oCoupon = (from x in twSql.Coupon join e in twSql.Event on x.eventid equals e.id
                       where x.id == arg_nCouponId && x.activetype != (int)Coupon.CouponActiveTypeOption.NotActive
                       && e.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive select x).SingleOrDefault();*/

            queryEventSearch = this._EventRepo.Event_GetAll().Where(x => x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);
            queryCouponSearch = this._CouponRepo.Coupon_GetAll().Where(x => x.activetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive);

            objDbCoupon = (from x in queryCouponSearch join e in queryEventSearch on x.eventid equals e.id where x.id == arg_nCouponId select x).FirstOrDefault();

            if (objDbCoupon != null)
            {
                oCoupon = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Coupon>(objDbCoupon);
            }

            return oCoupon;
        }

        private void replaceCouponTitle(ref List<Models.DomainModels.Redeem.Coupon> arg_listCoupon)
        {
            List<int> listEventId = null;
            List<Models.DBModels.TWSQLDB.Event> listDbEvent = null;
            Models.DBModels.TWSQLDB.Event objDbEvent = null;

            if (arg_listCoupon == null || arg_listCoupon.Count <= 0)
                return;

            listEventId = arg_listCoupon.Select(x => x.eventid).Distinct().ToList();
            //listEvent = twSql.Event.Where(x => x.couponactivetype != (int)Coupon.CouponActiveTypeOption.NotActive && listEventId.Contains(x.id)).ToList();
            listDbEvent = this._EventRepo.Event_GetAll().Where(x => x.couponactivetype != (int)Models.DBModels.TWSQLDB.Coupon.CouponActiveTypeOption.NotActive && listEventId.Contains(x.id)).ToList();

            foreach (Models.DomainModels.Redeem.Coupon oCoupon in arg_listCoupon)
            {
                objDbEvent = listDbEvent.FirstOrDefault(x => x.id == oCoupon.eventid);
                if (objDbEvent == null)
                {
                    oCoupon.title = "折價券";
                    continue;
                }

                //顯示順序:Coupon Market Description > Event Market Description > "折價券"
                if (objDbEvent.coupondescription != null && objDbEvent.coupondescription.Length > 0)
                    oCoupon.title = objDbEvent.coupondescription;
                else if (objDbEvent.eventdescription != null && objDbEvent.eventdescription.Length > 0)
                    oCoupon.title = objDbEvent.eventdescription;
                else
                    oCoupon.title = "折價券";
            }//end foreach

            //釋放記憶體

            if(listDbEvent != null)
            {
                listDbEvent.Clear();
                listDbEvent = null;
            }
            if (listEventId != null)
            {
                listEventId.Clear();
                listEventId = null;
            }
            objDbEvent = null;
        }

        public AddCouponStatusOption GetCouponActivity(string arg_strAccount)
        {          
            //IQueryable<CouponActivity> couponActivity = null;
            List<CouponActivity> couponActivityList = new List<CouponActivity>();
            //couponActivity = this._CouponRepo.CouponActivity();
            couponActivityList = this._CouponRepo.CouponActivity().ToList();

            AddCouponStatusOption oReturnMessage = AddCouponStatusOption.未處理;

            try
            {
                foreach (var couponActivitys in couponActivityList)
                {
                    if (DateTime.Compare(DateTime.Now, couponActivitys.ActivityStart ) >= 0 && DateTime.Compare(DateTime.Now, couponActivitys.ActivityEnd) < 0)
                    {
                        for (int i = 0; i < couponActivitys.Qty; i++)
                        {
                            oReturnMessage = addDynamicCouponByEventIdAndUserAccount(couponActivitys.EventID, arg_strAccount);
                        }
                    }
                }
                return oReturnMessage;
            }
            catch (Exception ex)
            {
                oReturnMessage = AddCouponStatusOption.發送失敗;
            }

            return oReturnMessage;
        }


        public AddCouponStatusOption addDynamicCouponByEventIdAndUserAccount(int arg_nEventId, string arg_strAccount)
        {
            AddCouponStatusOption oReturnMessage = AddCouponStatusOption.未處理;
            int numResult = 0;

            numResult = this._CouponRepo.addDynamicCouponByEventIdAndUserAccount(arg_nEventId, arg_strAccount);

            oReturnMessage = (AddCouponStatusOption)numResult;

            return oReturnMessage;
        }

        public List<Models.DomainModels.Redeem.CouponsUsedStatusReport> GetAllCouponUsedStatusReport()
        {
            List<Models.DomainModels.Redeem.CouponsUsedStatusReport> listCouponUsedStatusReport = null;
            Models.DomainModels.Redeem.CouponsUsedStatusReport objCouponUsedStatusReport = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;
            IQueryable<Models.DBModels.TWSQLDB.Coupon> queryCouponSearch = null;

            /*
            listCouponUsedStatusReport = (from p in objTwSql.Coupon
                                          join x in objTwSql.Event on p.eventid equals x.id into xp
                                          from xp1 in xp.DefaultIfEmpty()
                                          group xp1 by new { xp1.id, xp1.name, p.usestatus }
                                              into xp2
                                              select new CouponsUsedStatusReport() {
                                                  EventId = xp2.Key.id, 
                                                  EventName = xp2.Key.name, 
                                                  CouponUsedStatusType = (Coupon.CouponUsedStatusOption)xp2.Key.usestatus, Count = xp2.Count() 
                                              }).ToList();*/

            queryEventSearch = this._EventRepo.Event_GetAll();
            queryCouponSearch = this._CouponRepo.Coupon_GetAll();

            //因使用.Net Framework5.0, 不支援LINQ內Enum的轉型, 故使用迴圈硬轉
            var tempResult = (from p in queryCouponSearch
                              join x in queryEventSearch on p.eventid equals x.id into xp
                              from xp1 in xp.DefaultIfEmpty()
                              group xp1 by new { xp1.id, xp1.name, p.usestatus }
                                  into xp2
                                  select new 
                                  {
                                      EventId = xp2.Key.id,
                                      EventName = xp2.Key.name,
                                      CouponUsedStatusType = xp2.Key.usestatus,
                                      //CouponUsedStatusType = xp2.Key.usestatus,
                                      Count = xp2.Count()
                                  }).ToList();

            if (tempResult != null)
            {
                //listCouponUsedStatusReport = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.CouponsUsedStatusReport>>(tempResult);
                ////因使用.Net Framework5.0, 不支援LINQ內Enum的轉型, 故使用迴圈硬轉
                listCouponUsedStatusReport = new List<Models.DomainModels.Redeem.CouponsUsedStatusReport>();
                foreach (object objTemp in tempResult)
                {
                    objCouponUsedStatusReport = new Models.DomainModels.Redeem.CouponsUsedStatusReport();
                    objCouponUsedStatusReport.EventId = (int)objTemp.GetType().GetProperty("EventId").GetValue(objTemp);
                    objCouponUsedStatusReport.EventName = (string)objTemp.GetType().GetProperty("EventName").GetValue(objTemp);
                    objCouponUsedStatusReport.Count = (int)objTemp.GetType().GetProperty("Count").GetValue(objTemp);
                    objCouponUsedStatusReport.CouponUsedStatusType = (Models.DomainModels.Redeem.Coupon.CouponUsedStatusOption)objTemp.GetType().GetProperty("CouponUsedStatusType").GetValue(objTemp);

                    listCouponUsedStatusReport.Add(objCouponUsedStatusReport);
                }
                
            }
 
            return listCouponUsedStatusReport;

        }

        /// <summary>
        /// 根據Coupon Id取得Coupon
        /// </summary>
        /// <param name="argNumCouponId">Coupon Id</param>
        /// <returns>Coupon Object</returns>
        public Models.DomainModels.Redeem.Coupon GetCouponByCouponId(int argNumCouponId)
        {
            if (argNumCouponId <= 0)
                return null;

            Models.DomainModels.Redeem.Coupon objCoupon = null;
            Models.DBModels.TWSQLDB.Coupon objDbCoupon = null;

            objDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.id == argNumCouponId).FirstOrDefault();
            if (objDbCoupon != null)
            {
                objCoupon = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Coupon>(objDbCoupon);
            }
           
            return objCoupon;
        }

        /// <summary>
        /// 根據Coupon Number取得Coupon
        /// </summary>
        /// <param name="argStrCouponNumber">Coupon Number</param>
        /// <returns>Coupon Object</returns>
        public Models.DomainModels.Redeem.Coupon GetCouponByCouponNumber(string argStrCouponNumber)
        {
            if (argStrCouponNumber.Length <= 0)
                return null;

            Models.DomainModels.Redeem.Coupon objCoupon = null;
            Models.DBModels.TWSQLDB.Coupon objDbCoupon = null;

            objDbCoupon = this._CouponRepo.Coupon_GetAll().Where(x => x.number == argStrCouponNumber).FirstOrDefault();
            if (objDbCoupon != null)
            {
                objCoupon = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Coupon>(objDbCoupon);
            }

            return objCoupon;
        }

        public void ExecGrantCouponAllUser()
        {
            this._CouponRepo.ExecGrantCouponAllUser();
        }

    }//end class
}//end namespace
