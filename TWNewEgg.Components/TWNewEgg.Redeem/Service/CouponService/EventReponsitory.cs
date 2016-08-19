using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.CartRepoAdapters.Interface;


namespace TWNewEgg.Redeem.Service.CouponService
{
    public class EventReponsitory : IEvent
    {
        private IEventRepoAdapter _EventRepoAdapter = null;
        private ISORepoAdapter _iSORepoAdapter = null;

        public EventReponsitory(IEventRepoAdapter argEventRepo, ISORepoAdapter iSORepoAdapter)
        {
            this._EventRepoAdapter = argEventRepo;
            this._iSORepoAdapter = iSORepoAdapter;
        }
        /// <summary>
        /// 近增 Event
        /// </summary>
        /// <param name="arg_oEvent">Event物件</param>
        /// <returns>Event ID, 新增失敗時, 回傳0</returns>
        public int addEvent(Models.DomainModels.Redeem.Event arg_oEvent)
        {
            if (arg_oEvent == null)
                return 0;

            int nEventId = 0;
            Models.DBModels.TWSQLDB.Event objDbEvent = null;
            Models.DBModels.TWSQLDB.EventFile objDbEventFile = null;
            try
            {
                arg_oEvent.createdate = DateTime.Now;
                //進行資料轉型
                objDbEvent = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.Event>(arg_oEvent);
                this._EventRepoAdapter.CreateEvent(objDbEvent);
                nEventId = objDbEvent.id;

                if (nEventId > 0 && arg_oEvent.filterfileid > 0)
                {
                    objDbEventFile = this._EventRepoAdapter.EventFile_GetAll().Where(x => x.id == arg_oEvent.filterfileid).FirstOrDefault();
                    if (objDbEventFile != null && objDbEventFile.eventid != nEventId)
                    {
                        objDbEventFile.eventid = nEventId;
                        this._EventRepoAdapter.UpdateEventFile(objDbEventFile);
                    }
                }
            }
            catch
            {
                nEventId = 0;
            }

            return nEventId;
        }//end addEvent()

        /// <summary>
        /// 修改Event
        /// </summary>
        /// <param name="Event">Event 物件</param>
        /// <returns>true: 修改成功, false: 修改失敗</returns>
        public bool editEvent(Models.DomainModels.Redeem.Event arg_oEvent)
        {
            Models.DBModels.TWSQLDB.Event objDbEvent = this._EventRepoAdapter.Event_GetAll().Where(x => x.id == arg_oEvent.id).FirstOrDefault();
            Models.DBModels.TWSQLDB.EventFile objDbEventFile = null;
            bool bExec = false;

            if (objDbEvent == null)
                return false;

            if (!String.IsNullOrEmpty(arg_oEvent.items))
            {
                arg_oEvent.items = ";" + arg_oEvent.items.Replace(" ", "").Replace(",", ";").TrimStart(';').TrimEnd(';') + ";";
            }

            objDbEvent = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.Event>(arg_oEvent);

            objDbEvent.updated = objDbEvent.updated + 1;
            objDbEvent.updatedate = DateTime.Now;

            try
            {
                this._EventRepoAdapter.UpdateEvent(objDbEvent);
                bExec = true;

                if (objDbEvent.filterfileid > 0)
                {
                    objDbEventFile = this._EventRepoAdapter.EventFile_GetAll().Where(x => x.id == objDbEvent.filterfileid).FirstOrDefault();
                    if (objDbEventFile != null && objDbEventFile.eventid != objDbEvent.id)
                    {
                        objDbEventFile.eventid = objDbEvent.id;
                        this._EventRepoAdapter.UpdateEventFile(objDbEventFile);
                    }
                }
            }
            catch
            {
                bExec = false;
            }
            finally
            {
                objDbEvent = null;
            }
            
            return bExec;
        }//end editEvent()

        /// <summary>
        /// 根據Event Id取得Event
        /// </summary>
        /// <param name="EventId">Event Id</param>
        /// <returns>Event or null</returns>
        public Models.DomainModels.Redeem.Event getEventById(int arg_nEventId)
        {
            if (arg_nEventId <= 0)
                return null;

            Models.DBModels.TWSQLDB.Event objDbEvent = null;
            Models.DomainModels.Redeem.Event objResult = null;

            objDbEvent = this._EventRepoAdapter.Event_GetAll().Where(x => x.id == arg_nEventId).FirstOrDefault();
            if (objDbEvent != null)
            {
                objResult = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Event>(objDbEvent);
            }

            return objResult;
        }//end getEventById()

        /// <summary>
        /// 取得上線中或最近半年的Event列表
        /// </summary>
        /// <returns>List of Events or null</returns>
        public List<Models.DomainModels.Redeem.Event> getEventList()
        {
            IQueryable<Models.DBModels.TWSQLDB.Event> querySearch = null;
            List<Models.DomainModels.Redeem.Event> listEvent = null;
            List<Models.DBModels.TWSQLDB.Event> listSearch = null;
            DateTime dateFilter = DateTime.Now.AddMonths(-3);

            querySearch = this._EventRepoAdapter.Event_GetAll().Where(x => x.createdate >= dateFilter || x.couponvalidend >= dateFilter || x.datestart >= dateFilter).OrderByDescending(x=>x.createdate);
            listSearch = querySearch.ToList();

            if (listSearch != null && listSearch.Count > 0)
            {
                listEvent = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Event>>(listSearch);
            }

            return listEvent;
        }//end getEventList()

        /// <summary>
        /// 取得EventName含有關鍵字的EventList
        /// </summary>
        /// <param name="arg_strEventName">EventName鍵字</param>
        /// <returns>list of event, or null</returns>
        public List<Models.DomainModels.Redeem.Event> getEventListByEventName(string arg_strEventName)
        {
            List<Models.DomainModels.Redeem.Event> listEvent = null;
            List<Models.DBModels.TWSQLDB.Event> listDbEvent = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> querySearch = null;

            querySearch = this._EventRepoAdapter.Event_GetAll().Where(x => x.name.Contains(arg_strEventName));
            listDbEvent = querySearch.ToList();

            if (listDbEvent != null && listDbEvent.Count > 0)
            {
                listEvent = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Event>>(listDbEvent);
            }

            return listEvent;
        }//end getEventListByEventName()

        /// <summary>
        /// 取得該Manager(活動發起人)負責的Event列表
        /// </summary>
        /// <param name="Manager">Manager</param>
        /// <returns>List of Event or null</returns>
        public List<Models.DomainModels.Redeem.Event> getEventListByManager(string arg_strManager)
        {
            /* Manager使用的欄位為creator, 非createuser */

            List<Models.DomainModels.Redeem.Event> listEvent = null;
            List<Models.DBModels.TWSQLDB.Event> listDbEvent = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> querySearch = null;

            querySearch = this._EventRepoAdapter.Event_GetAll().Where(x => x.creator == arg_strManager);
            listDbEvent = querySearch.ToList();

            if (listDbEvent != null && listDbEvent.Count > 0)
            {
                listEvent = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Event>>(listDbEvent);
            }

            return listEvent;
        }//end getEventListByManager()


        /// <summary>
        /// 取得目前是否有Active的活動使用同一個CouponNumber
        /// </summary>
        /// <param name="arg_strCouponNumber">Coupon行銷代碼</param>
        /// <returns></returns>
        public bool checkEventCouponNumber(string arg_strCouponNumber)
        {
            Models.DBModels.TWSQLDB.Event oEvent = null;
            bool bCheck = false;
            DateTime oDateNow = DateTime.Now;

            if (arg_strCouponNumber == null || arg_strCouponNumber.Length <= 0)
                return true;

            oEvent = this._EventRepoAdapter.Event_GetAll().Where(x => x.datestart <= oDateNow && x.dateend >= oDateNow && x.couponmarketnumber == arg_strCouponNumber).FirstOrDefault();

            if(oEvent == null)
                bCheck = true;

            oEvent = null;

            return bCheck;
        }

        /// <summary>
        /// 取得目前的Event所使用的CouponNumber是否可以使用
        /// </summary>
        /// <param name="arg_strCouponNumber"></param>
        /// <param name="arg_nEventId"></param>
        /// <returns></returns>
        public bool checkEventCouponNumberByEventId(string arg_strCouponNumber, int arg_nEventId)
        {
            Models.DBModels.TWSQLDB.Event oEvent = null;
            bool bCheck = false;
            DateTime oDateNow = DateTime.Now;

            if (arg_strCouponNumber == null || arg_strCouponNumber.Length <= 0)
                return true;

            oEvent = this._EventRepoAdapter.Event_GetAll().Where(x => x.datestart <= oDateNow && x.dateend >= oDateNow && x.couponmarketnumber == arg_strCouponNumber).FirstOrDefault();

            if (oEvent == null)
                bCheck = true;
            else if (oEvent.id == arg_nEventId)
                bCheck = true;

            oEvent = null;

            return bCheck;
        }

        /// <summary>
        /// 取得線上的活動
        /// </summary>
        /// <param name="arg_nEventId">活動ID</param>
        /// <returns>Event object of null</returns>
        public Models.DomainModels.Redeem.Event GetActiveEventById(int arg_nEventId)
        {
            if (arg_nEventId <= 0)
                return null;

            Models.DomainModels.Redeem.Event oEvent = null;
            Models.DBModels.TWSQLDB.Event objDbEvent = null;
            DateTime objDateNow = DateTime.Now;

            objDbEvent = this._EventRepoAdapter.Event_GetAll().Where(x => x.id == arg_nEventId && x.datestart <= objDateNow && x.dateend >= objDateNow).FirstOrDefault();

            if (objDbEvent != null)
            {
                oEvent = ModelConverter.ConvertTo<Models.DomainModels.Redeem.Event>(objDbEvent);
            }

            return oEvent;
        }

        public List<Models.DomainModels.Redeem.Event> GetEvents(List<int> eventIDs)
        {
            List<Models.DomainModels.Redeem.Event> rtn = new List<Models.DomainModels.Redeem.Event>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.Event> data = this._EventRepoAdapter.Event_GetAll().Where(x => eventIDs.Contains(x.id)).ToList();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.Event item in data)
            {
                rtn.Add(new Models.DomainModels.Redeem.Event
                {
                    id = item.id,
                    name = item.name,
                    creator = item.creator,
                    couponcategories = item.couponcategories,
                    couponmarketnumber = item.couponmarketnumber,
                    couponmax = item.couponmax,
                    couponsum = item.couponsum,
                    couponreget = item.couponreget,
                    datestart = item.datestart,
                    dateend = item.dateend,
                    couponactiveusagedays = item.couponactiveusagedays,
                    visible = item.visible,
                    note = item.note,
                    grantstart = item.grantstart,
                    grantend = item.grantend,
                    grantstatus = item.grantstatus,
                    couponvalidstart = item.couponvalidstart,
                    couponvalidend = item.couponvalidend,
                    value = item.value,
                    couponactivetype = item.couponactivetype,
                    filter = item.filter,
                    filterfileusage = item.filterfileusage,
                    filterfileid = item.filterfileid,
                    createdate = item.createdate,
                    createuser = item.createuser,
                    updated = item.updated,
                    updatedate = item.updatedate,
                    updateuser = item.updateuser,
                    limit = item.limit,
                    limitdescription = item.limitdescription,
                    coupondescription = item.coupondescription,
                    eventdescription = item.eventdescription,
                    items = item.items
                });
            }
            return rtn;
        }
        public List<Models.DomainModels.Answer.SalesOrderItemInfo> GetSOItemByCodes(List<string> salesOrderCodes)
        {
            List<Models.DomainModels.Answer.SalesOrderItemInfo> rtn = new List<Models.DomainModels.Answer.SalesOrderItemInfo>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> data = _iSORepoAdapter.GetSOItemsByCodes(salesOrderCodes).ToList();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem item in data)
            {
                rtn.Add(new Models.DomainModels.Answer.SalesOrderItemInfo
                {
                    Code = item.Code,
                    SalesorderCode = item.SalesorderCode,
                    ProductID = item.ProductID,
                    ProductlistID = item.ProductlistID,
                    Name = item.Name,
                    ActID = item.ActID,
                    ItemID = item.ItemID,
                    ItemlistID = item.ItemlistID
                });
            }             
            return rtn;
        }

        public List<Models.DomainModels.Redeem.Event> GetWaitingForGrantingList()
        {
            List<Models.DomainModels.Redeem.Event> listEvent = null;
            List<Models.DBModels.TWSQLDB.Event> listDbEvent = null;
            IQueryable<Models.DBModels.TWSQLDB.Event> queryEventSearch = null;

            queryEventSearch = this._EventRepoAdapter.Event_GetAll().Where(x => x.grantstatus == (int)TWNewEgg.Models.DBModels.TWSQLDB.Event.GrantStatusOption.WaitingForGranting
                    && x.datestart <= DateTime.Now && x.dateend >= DateTime.Now
                    && x.couponactivetype == 1);

            listDbEvent = queryEventSearch.ToList();
            if (listDbEvent != null && listDbEvent.Count > 0)
            {
                listEvent = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.Event>>(listDbEvent);
            }
            return listEvent;
        }
    }//end class
}//namespace
