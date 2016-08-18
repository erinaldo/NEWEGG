using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

namespace TWNewEgg.AdvService.Service
{
    /// <summary>
    /// AdvEventDBService
    /// </summary>
    public class AdvEventDBService : IAdvEventDB
    {
        private TWSqlDBContext twsql = new TWSqlDBContext();

        #region AdvEvent

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">true or false</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                twsql.Dispose();
                //twsql = null;
            }
        }

        /// <summary>
        /// Get AdvEvent from db by AdvType and start date or end date
        /// </summary>
        /// <param name="advType">ENUM AdvType2 from AdvEvent.cs</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns>List of AdvEvent</returns>
        public List<AdvEvent> GetAdvEventByAdvType(int advType, DateTime? startDate, DateTime? endDate)
        {
            List<AdvEvent> advEvents = new List<AdvEvent>();
            DateTime dateTimeNow = DateTime.UtcNow.AddMonths(-1).AddHours(8);
            if (startDate != null && endDate != null)
            {
                advEvents = twsql.AdvEvent.Where(x => x.AdvType == advType && x.StartDate > startDate && x.EndDate < endDate && (x.DelDate > dateTimeNow || x.DelDate == null)).ToList();
            }
            else if (startDate != null && endDate == null)
            {
                advEvents = twsql.AdvEvent.Where(x => x.AdvType == advType && x.StartDate > startDate && (x.DelDate > dateTimeNow || x.DelDate == null)).ToList();
            }
            else if (startDate == null && endDate != null)
            {
                advEvents = twsql.AdvEvent.Where(x => x.AdvType == advType && x.EndDate < endDate && (x.DelDate > dateTimeNow || x.DelDate == null)).ToList();
            }
            else
            {
                advEvents = twsql.AdvEvent.Where(x => x.AdvType == advType && (x.DelDate > dateTimeNow || x.DelDate == null)).ToList();
            }

            return advEvents;
        }

        /// <summary>
        /// Get AdvEvent from db by AdvType and start date or end date
        /// </summary>
        /// <param name="advType">advType</param>
        /// <param name="closeDate">結束時間</param>
        /// <returns>AdvEvent object</returns>
        public AdvEvent GetAdvEventFromCloseDate(int advType, DateTime closeDate)
        {
            AdvEvent objAdvEvent = new AdvEvent();
            objAdvEvent = twsql.AdvEvent.Where(x => x.AdvType == advType && x.StartDate <= closeDate).OrderByDescending(x => x.StartDate).FirstOrDefault();
            return objAdvEvent;
        }

        /// <summary>
        /// Get AdvEventType   by Code.
        /// </summary>
        /// <param name="advEventTypeCodes">list of advEventTypeCodes</param>
        /// <param name="takeNumber">int</param>
        /// <param name="datetimeNow">datetime</param>
        /// <returns>List of AdvEventType</returns>
        public List<AdvEventType> GetAdvEventTypes(List<int> advEventTypeCodes, int takeNumber, DateTime? datetimeNow)
        {
            List<AdvEventType> advEventTypes = new List<AdvEventType>();
            if (advEventTypeCodes.Count != 0)
            {
                if (datetimeNow == null)
                {
                    advEventTypes.AddRange(twsql.AdvEventType.Where(x => advEventTypeCodes.Contains(x.AdvTypeCode)).Take(takeNumber).ToList());
                }
                else
                {
                    advEventTypes.AddRange(twsql.AdvEventType.Where(x => advEventTypeCodes.Contains(x.AdvTypeCode) && x.StartDate <= datetimeNow.Value).Take(takeNumber).ToList());
                }
            }

            return advEventTypes;
        }

        /// <summary>
        /// Add a new advevent into DB, and return the advevent which saved in DB
        /// </summary>
        /// <param name="newAdvEvent">新增的AdvEvent</param>
        /// <returns>已新增並含有ID的AdvEvent</returns>
        public AdvEvent AddNewAdvEvent(AdvEvent newAdvEvent)
        {
            AdvEvent objNewAdvEvent = new AdvEvent();

            objNewAdvEvent.HashCode = newAdvEvent.HashCode;
            objNewAdvEvent.ClickNumber = newAdvEvent.ClickNumber;
            objNewAdvEvent.AdvType = newAdvEvent.AdvType;
            objNewAdvEvent.StartDate = newAdvEvent.StartDate;
            objNewAdvEvent.EndDate = newAdvEvent.EndDate;
            objNewAdvEvent.DelDate = newAdvEvent.DelDate;
            objNewAdvEvent.StyleClassName1 = newAdvEvent.StyleClassName1;
            objNewAdvEvent.StyleClassName2 = newAdvEvent.StyleClassName2;
            objNewAdvEvent.SoldoutClassName = newAdvEvent.SoldoutClassName;
            objNewAdvEvent.ImgFilterClassName1 = newAdvEvent.ImgFilterClassName1;
            objNewAdvEvent.ImgFilterClassName2 = newAdvEvent.ImgFilterClassName2;
            objNewAdvEvent.BeforeTitle = newAdvEvent.BeforeTitle;
            objNewAdvEvent.BeforeSlogan = newAdvEvent.BeforeSlogan;
            objNewAdvEvent.BeforeLinkUrl = newAdvEvent.BeforeLinkUrl;
            objNewAdvEvent.BeforeImgUrl = newAdvEvent.BeforeImgUrl;
            objNewAdvEvent.BeforeImgAlt = newAdvEvent.BeforeImgAlt;
            objNewAdvEvent.StartTitle = newAdvEvent.StartTitle;
            objNewAdvEvent.StartSlogan = newAdvEvent.StartSlogan;
            objNewAdvEvent.StartLinkUrl = newAdvEvent.StartLinkUrl;
            objNewAdvEvent.StartImgUrl = newAdvEvent.StartImgUrl;
            objNewAdvEvent.StartImgAlt = newAdvEvent.StartImgAlt;
            objNewAdvEvent.EndTitle = newAdvEvent.EndTitle;
            objNewAdvEvent.EndSlogan = newAdvEvent.EndSlogan;
            objNewAdvEvent.EndLinkUrl = newAdvEvent.EndLinkUrl;
            objNewAdvEvent.EndImgUrl = newAdvEvent.EndImgUrl;
            objNewAdvEvent.EndImgAlt = newAdvEvent.EndImgAlt;
            objNewAdvEvent.ItemID = newAdvEvent.ItemID;
            objNewAdvEvent.RecommendItemIDs = newAdvEvent.RecommendItemIDs;
            objNewAdvEvent.ExtraApi1 = newAdvEvent.ExtraApi1;
            objNewAdvEvent.ExtraApiAction1 = newAdvEvent.ExtraApiAction1;
            objNewAdvEvent.ExtraApiParameters1 = newAdvEvent.ExtraApiParameters1;
            objNewAdvEvent.ExtraApi2 = newAdvEvent.ExtraApi2;
            objNewAdvEvent.ExtraApiAction2 = newAdvEvent.ExtraApiAction2;
            objNewAdvEvent.ExtraApiParameters2 = newAdvEvent.ExtraApiParameters2;
            objNewAdvEvent.ExtraApi3 = newAdvEvent.ExtraApi3;
            objNewAdvEvent.ExtraApiAction3 = newAdvEvent.ExtraApiAction3;
            objNewAdvEvent.ExtraApiParameters3 = newAdvEvent.ExtraApiParameters3;
            objNewAdvEvent.CreateDate = newAdvEvent.CreateDate;
            objNewAdvEvent.CreateUser = newAdvEvent.CreateUser;
            objNewAdvEvent.Updated = newAdvEvent.Updated;

            try
            {
                twsql.AdvEvent.Add(objNewAdvEvent);
                twsql.SaveChanges();
            }
            catch
            {
                objNewAdvEvent = null;
            }

            return objNewAdvEvent;
        }

        /// <summary>
        /// Update a advevent which already in DB.
        /// </summary>
        /// <param name="newAdvEvent">要修改AdvEvent</param>
        /// <returns>修改後的AdvEvent</returns>
        public AdvEvent UpdateAdvEvent(AdvEvent newAdvEvent)
        {
            var oriAdvEvent = GetAdvEventByIDs(new List<int> { newAdvEvent.ID }).FirstOrDefault();
            if (oriAdvEvent == null)
            {
                return oriAdvEvent;
            }

            oriAdvEvent.HashCode = newAdvEvent.HashCode;
            oriAdvEvent.ClickNumber = newAdvEvent.ClickNumber;
            oriAdvEvent.AdvType = newAdvEvent.AdvType;
            oriAdvEvent.StartDate = newAdvEvent.StartDate;
            oriAdvEvent.EndDate = newAdvEvent.EndDate;
            oriAdvEvent.DelDate = newAdvEvent.DelDate;
            oriAdvEvent.StyleClassName1 = newAdvEvent.StyleClassName1;
            oriAdvEvent.StyleClassName2 = newAdvEvent.StyleClassName2;
            oriAdvEvent.SoldoutClassName = newAdvEvent.SoldoutClassName;
            oriAdvEvent.ImgFilterClassName1 = newAdvEvent.ImgFilterClassName1;
            oriAdvEvent.ImgFilterClassName2 = newAdvEvent.ImgFilterClassName2;
            oriAdvEvent.BeforeTitle = newAdvEvent.BeforeTitle;
            oriAdvEvent.BeforeSlogan = newAdvEvent.BeforeSlogan;
            oriAdvEvent.BeforeLinkUrl = newAdvEvent.BeforeLinkUrl;
            oriAdvEvent.BeforeImgUrl = newAdvEvent.BeforeImgUrl;
            oriAdvEvent.BeforeImgAlt = newAdvEvent.BeforeImgAlt;
            oriAdvEvent.StartTitle = newAdvEvent.StartTitle;
            oriAdvEvent.StartSlogan = newAdvEvent.StartSlogan;
            oriAdvEvent.StartLinkUrl = newAdvEvent.StartLinkUrl;
            oriAdvEvent.StartImgUrl = newAdvEvent.StartImgUrl;
            oriAdvEvent.StartImgAlt = newAdvEvent.StartImgAlt;
            oriAdvEvent.EndTitle = newAdvEvent.EndTitle;
            oriAdvEvent.EndSlogan = newAdvEvent.EndSlogan;
            oriAdvEvent.EndLinkUrl = newAdvEvent.EndLinkUrl;
            oriAdvEvent.EndImgUrl = newAdvEvent.EndImgUrl;
            oriAdvEvent.EndImgAlt = newAdvEvent.EndImgAlt;
            oriAdvEvent.ItemID = newAdvEvent.ItemID;
            oriAdvEvent.RecommendItemIDs = newAdvEvent.RecommendItemIDs;
            oriAdvEvent.ExtraApi1 = newAdvEvent.ExtraApi1;
            oriAdvEvent.ExtraApiAction1 = newAdvEvent.ExtraApiAction1;
            oriAdvEvent.ExtraApiParameters1 = newAdvEvent.ExtraApiParameters1;
            oriAdvEvent.ExtraApi2 = newAdvEvent.ExtraApi2;
            oriAdvEvent.ExtraApiAction2 = newAdvEvent.ExtraApiAction2;
            oriAdvEvent.ExtraApiParameters2 = newAdvEvent.ExtraApiParameters2;
            oriAdvEvent.ExtraApi3 = newAdvEvent.ExtraApi3;
            oriAdvEvent.ExtraApiAction3 = newAdvEvent.ExtraApiAction3;
            oriAdvEvent.ExtraApiParameters3 = newAdvEvent.ExtraApiParameters3;
            oriAdvEvent.Updated = (oriAdvEvent.Updated++);
            oriAdvEvent.UpdateDate = newAdvEvent.UpdateDate;
            oriAdvEvent.UpdateUser = newAdvEvent.UpdateUser;

            try
            {
                twsql.SaveChanges();
            }
            catch
            {
                oriAdvEvent = null;
            }

            return oriAdvEvent;
        }

        /// <summary>
        /// Get advevents from DB by IDs.
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>AdvEvent的列表</returns>
        public List<AdvEvent> GetAdvEventByIDs(List<int> ids)
        {
            List<AdvEvent> advEventList = new List<AdvEvent>();
            advEventList.AddRange(twsql.AdvEvent.Where(x => ids.Contains(x.ID)).ToList());
            return advEventList;
        }

        /// <summary>
        /// Delete a advevent by ID.
        /// </summary>
        /// <param name="numId">id</param>
        /// <returns>true or false</returns>
        public bool DeleteAdvEvent(int numId)
        {
            return true;
        }

        /// <summary>
        /// Delete advevents by IDs
        /// </summary>
        /// <param name="ids">list of id</param>
        /// <returns>刪除結果</returns>
        public string DeleteAdvEvents(List<int> ids)
        {
            return "";
        }

        /// <summary>
        /// 根據AdvEventType取得旗下的所有AdvEvent
        /// </summary>
        /// <param name="arg_nAdvTypeId">AdvEventType.ID</param>
        /// <returns>null or List of AdvEvent</returns>
        public List<AdvEvent> GetAllAdvEventByAdvEventTypeId(int arg_nAdvTypeId)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<AdvEvent> listAdvEvent = null;
            DateTime objDateNow = DateTime.Now;

            // 根據AdvEventType取得旗下的所有AdvEvent
            oDb = new DB.TWSqlDBContext();
            listAdvEvent = oDb.AdvEvent.Where(x => x.AdvEventTypeId == arg_nAdvTypeId && (x.DelDate == null || x.DelDate > objDateNow)).OrderBy(x => x.ShowOrder).ToList();
            oDb.Dispose();

            return listAdvEvent;
        }

        /// <summary>
        /// 根據AdvEventType取得旗下的所有上線的AdvEvent
        /// </summary>
        /// <param name="arg_AdvEventTypeId">AdvEventId</param>
        /// <returns>null or List of AdvEvent</returns>
        public List<AdvEvent> GetActiveAdvEventByAdvEventTypeId(int arg_AdvEventTypeId)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<AdvEvent> listAdvEvent = null;
            DateTime objDateNow = DateTime.Now;

            oDb = new DB.TWSqlDBContext();
            // 取得可顯示於線上的AdvEvent, 條件為: 在起始時間內 && OnlineStatus = 1 && ShowOrder > 0
            listAdvEvent = oDb.AdvEvent.Where(x => x.AdvEventTypeId == arg_AdvEventTypeId && x.StartDate <= objDateNow && x.EndDate >= objDateNow && (x.DelDate == null || x.DelDate >= objDateNow) && x.OnlineStatus == 1 && x.ShowOrder > 0).OrderBy(x => x.ShowOrder).ToList();
            oDb.Dispose();

            return listAdvEvent;
        }

        /// <summary>
        /// 修改AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">要修改的AdvEvent</param>
        /// <param name="arg_numFlag">用來overload的flag</param>
        /// <returns>true:修改成功; false: 修改失敗</returns>
        public bool UpdateAdvEvent(AdvEvent arg_oAdvEvent, int arg_numFlag)
        {
            //arg_numFlag只是用來強制UpdateAdvEvent function override而已
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            AdvEvent objOldAdvEvent = null;
            bool boolError = false;

            //TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo <
            
            //如果可以查到這個物件, 才進行修改
            oDb = new DB.TWSqlDBContext();
            objOldAdvEvent = oDb.AdvEvent.FirstOrDefault(x => x.ID == arg_oAdvEvent.ID);
            if (objOldAdvEvent != null)
            {
                objOldAdvEvent.HashCode = arg_oAdvEvent.HashCode;
                objOldAdvEvent.ClickNumber = arg_oAdvEvent.ClickNumber;
                objOldAdvEvent.AdvType = arg_oAdvEvent.AdvType;
                objOldAdvEvent.StartDate = arg_oAdvEvent.StartDate;
                objOldAdvEvent.EndDate = arg_oAdvEvent.EndDate;
                if (arg_oAdvEvent.AdvType == 1)
                {
                    // 若AdvType為整點特賣, 廣告於結束時間一週後刪除
                    objOldAdvEvent.DelDate = Convert.ToDateTime(arg_oAdvEvent.EndDate).AddDays(7);
                }
                else
                {
                    // 若AdvType非整點特賣, 廣告於結束時間3日後刪除
                    objOldAdvEvent.DelDate = Convert.ToDateTime(arg_oAdvEvent.EndDate).AddDays(3);
                }

                objOldAdvEvent.StyleClassName1 = arg_oAdvEvent.StyleClassName1;
                objOldAdvEvent.StyleClassName2 = arg_oAdvEvent.StyleClassName2;
                objOldAdvEvent.OnlineStatus = arg_oAdvEvent.OnlineStatus;
                objOldAdvEvent.ShowOrder = arg_oAdvEvent.ShowOrder;
                objOldAdvEvent.SoldoutClassName = arg_oAdvEvent.SoldoutClassName;
                objOldAdvEvent.ImgFilterClassName1 = arg_oAdvEvent.ImgFilterClassName1;
                objOldAdvEvent.ImgFilterClassName2 = arg_oAdvEvent.ImgFilterClassName2;

                objOldAdvEvent.BeforeTitle = arg_oAdvEvent.BeforeTitle;
                objOldAdvEvent.BeforeSlogan = arg_oAdvEvent.BeforeSlogan;
                objOldAdvEvent.BeforeLinkUrl = arg_oAdvEvent.BeforeLinkUrl;
                objOldAdvEvent.BeforeImgUrl = arg_oAdvEvent.BeforeImgUrl;
                objOldAdvEvent.BeforeImgAlt = arg_oAdvEvent.BeforeImgAlt;

                objOldAdvEvent.StartTitle = arg_oAdvEvent.StartTitle;
                objOldAdvEvent.StartSlogan = arg_oAdvEvent.StartSlogan;
                objOldAdvEvent.StartLinkUrl = arg_oAdvEvent.StartLinkUrl;
                objOldAdvEvent.StartImgUrl = arg_oAdvEvent.StartImgUrl;
                objOldAdvEvent.StartImgAlt = arg_oAdvEvent.StartImgAlt;

                objOldAdvEvent.EndTitle = arg_oAdvEvent.EndTitle;
                objOldAdvEvent.EndSlogan = arg_oAdvEvent.EndSlogan;
                objOldAdvEvent.EndLinkUrl = arg_oAdvEvent.EndLinkUrl;
                objOldAdvEvent.EndImgUrl = arg_oAdvEvent.EndImgUrl;
                objOldAdvEvent.EndImgAlt = arg_oAdvEvent.EndImgAlt;

                objOldAdvEvent.ItemID = arg_oAdvEvent.ItemID;
                objOldAdvEvent.RecommendItemIDs = arg_oAdvEvent.RecommendItemIDs;

                objOldAdvEvent.ExtraApi1 = arg_oAdvEvent.ExtraApi1;
                objOldAdvEvent.ExtraApiAction1 = arg_oAdvEvent.ExtraApiAction1;
                objOldAdvEvent.ExtraApiParameters1 = arg_oAdvEvent.ExtraApiParameters1;

                objOldAdvEvent.ExtraApi2 = arg_oAdvEvent.ExtraApi2;
                objOldAdvEvent.ExtraApiAction2 = arg_oAdvEvent.ExtraApiAction2;
                objOldAdvEvent.ExtraApiParameters2 = arg_oAdvEvent.ExtraApiParameters2;

                objOldAdvEvent.ExtraApi3 = arg_oAdvEvent.ExtraApi3;
                objOldAdvEvent.ExtraApiAction3 = arg_oAdvEvent.ExtraApiAction3;
                objOldAdvEvent.ExtraApiParameters3 = arg_oAdvEvent.ExtraApiParameters3;

                objOldAdvEvent.Memo = arg_oAdvEvent.Memo;

                objOldAdvEvent.Updated = objOldAdvEvent.Updated + 1;
                objOldAdvEvent.UpdateDate = DateTime.Now;
                objOldAdvEvent.UpdateUser = arg_oAdvEvent.UpdateUser;

                try
                {
                    oDb.SaveChanges();
                }
                catch
                {
                    boolError = true;
                }
                finally
                {
                    if (oDb != null)
                    {
                        oDb.Dispose();
                    }

                    oDb = null;
                }
            }
            
            return !boolError;
        }

        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        public int AddAdvEvent(AdvEvent arg_oAdvEvent)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            int nId = 0;

            try
            {
                oDb = new DB.TWSqlDBContext();
                //決定DelDate
                if (arg_oAdvEvent.AdvType.Equals(1))
                {
                    // 若AdvType為整點特賣, 廣告於結束時間一週後刪除
                    arg_oAdvEvent.DelDate = Convert.ToDateTime(arg_oAdvEvent.EndDate).AddDays(7);
                }
                else
                {
                    // 若AdvType非整點特賣, 廣告於結束時間3日後刪除
                    arg_oAdvEvent.DelDate = Convert.ToDateTime(arg_oAdvEvent.EndDate).AddDays(3);
                }

                arg_oAdvEvent.CreateDate = DateTime.Now;

                oDb.AdvEvent.Add(arg_oAdvEvent);
                oDb.SaveChanges();
                nId = arg_oAdvEvent.ID;
            }
            catch
            {
                nId = 0;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }

                oDb = null;
            }

            return nId;
        }

        #endregion

        #region AdvEventType
        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEventType">新增的AdvEvent物件</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        public int AddAdvEventType(AdvEventType arg_oAdvEventType)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            int nId = 0;
            
            oDb = new DB.TWSqlDBContext();
            arg_oAdvEventType.CreateDate = DateTime.Now;
            oDb.AdvEventType.Add(arg_oAdvEventType);
            try
            {
                oDb.SaveChanges();
                nId = arg_oAdvEventType.ID;
            }
            catch
            {
                nId = 0;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }
            }

            return nId;
        }

        /// <summary>
        /// 取得所有的AdvEventType列表
        /// </summary>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeList()
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<AdvEventType> listAdvEventType = null;

            try
            {
                oDb = new DB.TWSqlDBContext();
                listAdvEventType = oDb.AdvEventType.OrderByDescending(x => x.ID).ToList();
            }
            catch
            {
                listAdvEventType = null;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }
            }

            return listAdvEventType;
        }

        /// <summary>
        /// 查詢Name含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strKeyword">關鍵字</param>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeListByName(string arg_strKeyword)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<AdvEventType> listAdvEventType = null;

            try
            {
                oDb = new DB.TWSqlDBContext();
                listAdvEventType = oDb.AdvEventType.Where(x => x.AdvTypeName.IndexOf(arg_strKeyword) >= 0).ToList();
            }
            catch
            {
                listAdvEventType = null;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }
            }

            return listAdvEventType;
        }

        /// <summary>
        /// 查詢CSS含有關鍵字的AdvEventType
        /// </summary>
        /// <param name="arg_strCSS">CSS</param>
        /// <returns>null或是AdvEventType的列表</returns>
        public List<AdvEventType> GetAdvEventTypeListByCSS(string arg_strCSS)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<AdvEventType> listAdvEventType = null;

            try
            {
                oDb = new DB.TWSqlDBContext();
                listAdvEventType = oDb.AdvEventType.Where(x => x.CSS.IndexOf(arg_strCSS) >= 0).ToList();
            }
            catch
            {
                listAdvEventType = null;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }
            }

            return listAdvEventType;
        }

        /// <summary>
        /// 根據ID取得AdvEventType物件
        /// </summary>
        /// <param name="arg_nAdvEventType">ID</param>
        /// <returns>null或AdvEventType物件</returns>
        public AdvEventType GetAdvEventTypeById(int arg_nAdvEventType)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            AdvEventType objAdvEventType = null;

            try
            {
                oDb = new DB.TWSqlDBContext();
                objAdvEventType = oDb.AdvEventType.Where(x => x.ID == arg_nAdvEventType).FirstOrDefault();
            }
            catch
            {
                objAdvEventType = null;
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }
            }

            return objAdvEventType;
        }

        /// <summary>
        /// 修改AdvEventType
        /// </summary>
        /// <param name="arg_oAdvEventType">修改的AdvEventType物件</param>
        /// <returns>true:修改成功, false:修改失敗</returns>
        public bool UpdateAdvEventType(AdvEventType arg_oAdvEventType)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            TWNewEgg.DB.TWSQLDB.Models.AdvEventType objOldAdvEventType = null;
            bool boolError = false;
            
            oDb = new DB.TWSqlDBContext();
            objOldAdvEventType = oDb.AdvEventType.Where(x => x.ID == arg_oAdvEventType.ID).FirstOrDefault();
            if (objOldAdvEventType != null)
            {
                // 因修改並非所有欄位都允許更動, 故以下為各別可更動欄位進行指定更動
                objOldAdvEventType.AdvTypeCode = arg_oAdvEventType.AdvTypeCode;
                objOldAdvEventType.AdvTypeName = arg_oAdvEventType.AdvTypeName;
                objOldAdvEventType.StartDate = arg_oAdvEventType.StartDate;
                objOldAdvEventType.EndDate = arg_oAdvEventType.EndDate;
                objOldAdvEventType.MaxAd = arg_oAdvEventType.MaxAd;
                objOldAdvEventType.CacheMins = arg_oAdvEventType.CacheMins;
                objOldAdvEventType.CSS = arg_oAdvEventType.CSS;
                objOldAdvEventType.Country = arg_oAdvEventType.Country;

                objOldAdvEventType.UpdateDate = DateTime.Now;
                objOldAdvEventType.UpdateUser = arg_oAdvEventType.UpdateUser;

                if (objOldAdvEventType.Updated == null)
                {
                    objOldAdvEventType.Updated = 1;
                }
                else
                {
                    objOldAdvEventType.Updated = objOldAdvEventType.Updated + 1;
                }

                try
                {
                    // 修改AdvEventType
                    oDb.SaveChanges();
                }
                catch
                {
                    boolError = true;
                }
                finally
                {
                    if (oDb != null)
                    {
                        oDb.Dispose();
                    }
                }
            }

            return !boolError;
        }

        /// <summary>
        /// 根據Country Code及AdvType取得AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">Country ID</param>
        /// <param name="arg_numAdvType">AdvType</param>
        /// <returns>AdvEventType列表或是null</returns>
        public List<AdvEventType> GetAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            // 根據Country Code及AdvType取得AdvEventType的列表
            objDb = new DB.TWSqlDBContext();
            listAdvEventType = objDb.AdvEventType.Where(x => x.Country == arg_numCountryId && x.AdvTypeCode == arg_numAdvType).ToList();

            objDb = null;

            return listAdvEventType;
        }

        /// <summary>
        /// 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numCountryId">Country Id</param>
        /// <param name="arg_numAdvType">Convert enum AdvEventType.AdvTypeOption</param>
        /// <returns>List of AdvEventType</returns>
        public List<AdvEventType> GetActiveAdvEventTypeListByCountryAndAdvType(int arg_numCountryId, int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            DateTime objDateNow = DateTime.Now;

            // 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
            objDb = new DB.TWSqlDBContext();
            listAdvEventType = objDb.AdvEventType.Where(x => x.Country == arg_numCountryId && x.AdvTypeCode == arg_numAdvType && x.StartDate <= objDateNow && x.EndDate >= objDateNow).ToList();

            objDb = null;

            return listAdvEventType;
        }

        /// <summary>
        /// 根據AdvType取得有效期間內的AdvEventType的列表
        /// </summary>
        /// <param name="arg_numAdvType">Convert enum AdvEventType.AdvTypeOption</param>
        /// <returns>List of AdvEventType</returns>
        public List<AdvEventType> GetActiveAdvEventTypeListByAdvType(int arg_numAdvType)
        {
            List<AdvEventType> listAdvEventType = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            DateTime objDateNow = DateTime.Now;

            // 根據Country Code及AdvType取得有效期間內的AdvEventType的列表
            objDb = new DB.TWSqlDBContext();
            listAdvEventType = objDb.AdvEventType.Where(x => x.AdvTypeCode == arg_numAdvType && x.StartDate <= objDateNow && x.EndDate >= objDateNow).ToList();

            objDb = null;

            return listAdvEventType;
        }
        #endregion
    }
}
