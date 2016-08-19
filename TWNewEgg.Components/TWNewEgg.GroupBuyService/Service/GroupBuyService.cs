using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.GroupBuy.Interface;
using TWNewEgg.Models.DomainModels.GroupBuy;

namespace TWNewEgg.GroupBuy
{
    /// <summary>
    /// GroupBuyService
    /// </summary>
    public class GroupBuyService : IGroupBuy
    {
        private string disableImageUrl = "/img/groupbuy/gbBuyDisable.png"; //disable 前往 Button Image
        private string enableImageUrl = "/img/groupbuy/gbBuy.png"; //enable 前往 Button Image
        private string endImageUrl = "/img/groupbuy/gbEnd.png"; //結束 Button Image
        private string soldOutImageUrl = "/img/groupbuy/gbSoldOut.png"; //結束 Button Image       
        private const string ITEMURLLINK = "/item?itemid={0}&categoryid={1}&StoreID={2}";
        TWNewEgg.ItemService.Service.IItemService itemService = new TWNewEgg.ItemService.Service.ItemServiceRepository();
        //private IGroupBuyService _GroupBuyService;

        //public GroupBuyService(IGroupBuyService GroupBuyService)
        //{
        //    this._GroupBuyService = GroupBuyService;
        //}

        /// <summary>
        /// Get GroupBuyDisableImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyDisableImage
        {
            get
            {
                return this.disableImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuyEnableImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyEnableImage
        {
            get
            {
                return this.enableImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuyEndImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyEndImage
        {
            get
            {
                return this.endImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuySoldOutImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuySoldOutImage
        {
            get
            {
                return this.soldOutImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuy information by GroupBuyID
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <returns>Data Model GroupBuyInfo</returns>
        public GroupBuyInfo GetInfo(int id)
        {
            GroupBuyInfo result = null;

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                result = FillInGroupBuyInfo(groupBuy);
            }

            return result;
        }

        /// <summary>
        /// Get GroupBuy information
        /// </summary>
        /// <param name="condition">搜尋條件</param>
        /// <returns>List<GroupBuyInfo></GroupBuyInfo></returns>
        public List<GroupBuyInfo> QueryInfo(GroupBuyQueryCondition condition)
        {
            List<GroupBuyInfo> infoList = new List<GroupBuyInfo>();

            if (condition.PageSize < 1)
            {
                condition.PageSize = 10;
            }

            if (condition.PageNumber < 1)
            {
                condition.PageNumber = 1;
            }

            int skip = condition.PageSize * (condition.PageNumber - 1);
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuyQuery = db.GroupBuy.AsQueryable();

            //Query Conditionss
            //GroupBuyID
            if (condition.GroupBuyID > 0)
            {
                groupBuyQuery = groupBuyQuery.Where(x => x.ID == condition.GroupBuyID).AsQueryable();
            }

            //ItemID
            if (condition.ItemID > 0)
            {
                groupBuyQuery = groupBuyQuery.Where(x => x.ItemID == condition.ItemID).AsQueryable();
            }

            //ItemName
            if (condition.ItemName != null && condition.ItemName.Length > 0)
            {
                groupBuyQuery = groupBuyQuery.Where(x => x.ItemName.IndexOf(condition.ItemName) >= 0).AsQueryable();
            }

            //BeginDate
            if (condition.BeginDate.Year > 2000)
            {
                groupBuyQuery = groupBuyQuery.Where(x => x.BeginDate >= condition.BeginDate).AsQueryable();
            }

            //EndDate
            if (condition.EndDate.Year > 2000)
            {
                groupBuyQuery = groupBuyQuery.Where(x => x.EndDate <= condition.EndDate).AsQueryable();
            }

            //GroupBuyStatus
                var now = DateTime.UtcNow.AddHours(8);
                switch ((GroupBuyStatus.StatusEnum)condition.Status)
                {
                    case GroupBuyStatus.StatusEnum.草稿:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == false && x.IsReject == false && x.IsHide == false).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.待審核:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == true && x.IsApprove == false && x.IsReject == false && x.IsHide == false).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.退回:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == false && x.IsReject == true && x.IsHide == false).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.待上檔:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == true && x.IsReject == false && x.IsHide == false && now < x.BeginDate).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.上檔中:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == true && x.IsReject == false && x.IsHide == false && now >= x.BeginDate && now <= x.EndDate).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.已下檔:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == true && x.IsReject == false && x.IsHide == false && now > x.EndDate).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.已售完:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == true && x.IsReject == false && x.IsHide == false && now >= x.BeginDate && now <= x.EndDate).AsQueryable();
                        break;
                    case GroupBuyStatus.StatusEnum.已移除:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsWaitingForApprove == false && x.IsApprove == true && x.IsReject == false && x.IsHide == true).AsQueryable();
                        break;
                    default:
                        groupBuyQuery = groupBuyQuery.Where(x => x.IsHide == false).AsQueryable();
                        break;
                }

            List<DB.TWSQLDB.Models.GroupBuy> groupBuyList;
            if (condition.Status == (int)GroupBuyStatus.StatusEnum.上檔中)
            {
                //排除掉已售完
                groupBuyList = groupBuyQuery.ToList();
                var tempList = new List<DB.TWSQLDB.Models.GroupBuy>();
                groupBuyList.ForEach(x =>
                {
                    int soRealCount = GetSoRealCount(x.ID);
                    //SalesOrderLimit <= 0 代表不限制團購銷售數量
                    if (x.SalesOrderLimit <= 0 || soRealCount < x.SalesOrderLimit)
                    {
                        tempList.Add(x);
                    }
                });
                groupBuyList = tempList.OrderBy(x => x.ID).Skip(skip).Take(condition.PageSize).ToList();
            }
            else if (condition.Status == (int)GroupBuyStatus.StatusEnum.已售完)
            {
                //只取已售完
                groupBuyList = groupBuyQuery.ToList();
                var tempList = new List<DB.TWSQLDB.Models.GroupBuy>();
                groupBuyList.ForEach(x =>
                {
                    int soRealCount = GetSoRealCount(x.ID);
                    if (x.SalesOrderLimit > 0 && soRealCount >= x.SalesOrderLimit)
                    {
                        tempList.Add(x);
                    }
                });
                groupBuyList = tempList.OrderBy(x => x.ID).Skip(skip).Take(condition.PageSize).ToList();
            }
            else
            {
                groupBuyList = groupBuyQuery.OrderBy(x => x.ID).Skip(skip).Take(condition.PageSize).ToList();
            }

            groupBuyList.ForEach(x =>
            {
                infoList.Add(FillInGroupBuyInfo(x));
            });

            return infoList;
        }

        /// <summary>
        /// Get GroupBuy information for front end view By GroupBuyID
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <returns>Data Model GroupBuyInfo</returns>
        public GroupBuyViewInfo GetViewInfo(int id)
        {
            GroupBuyViewInfo result = null;

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                result = FillInGroupBuyViewInfo(groupBuy);
            }

            return result;
        }

        /// <summary>
        /// Get GroupBuy information for front end view
        /// </summary>
        /// <param name="condition">搜尋條件</param>
        /// <returns>List<GroupBuyInfo></GroupBuyInfo></returns>
        public List<GroupBuyViewInfo> QueryViewInfo(GroupBuyQueryCondition condition)
        {
            List<GroupBuyViewInfo> infoList = new List<GroupBuyViewInfo>();

            if (condition.PageSize < 1)
            {
                condition.PageSize = 10;
            }

            if (condition.PageNumber < 1)
            {
                condition.PageNumber = 1;
            }

            int skip = condition.PageSize * (condition.PageNumber - 1);
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var query = db.GroupBuy.Where(x => x.IsApprove == true && x.IsHide == false).OrderBy(x => x.ID).Skip(skip).Take(condition.PageSize).AsQueryable();
            if (condition.GroupBuyID > 0)
            {
                query = query.Where(x => x.ID == condition.GroupBuyID).AsQueryable();
            }

            var groupBuyList = query.ToList();
            groupBuyList.ForEach(x =>
            {
                infoList.Add(FillInGroupBuyViewInfo(x));
            });
            var now = DateTime.UtcNow.AddHours(8);
            //已開賣團購
            var beginGroup = infoList.Where(x => now >= DateTime.Parse(x.BeginDate) && x.IsExpired == false && x.IsSoldOut == false).OrderByDescending(x => DateTime.Parse(x.BeginDate)).ToList();
            //等待開賣團購
            var waitGroup = infoList.Where(x => now < DateTime.Parse(x.BeginDate) && x.IsExpired == false && x.IsSoldOut == false).OrderBy(x => DateTime.Parse(x.BeginDate)).ToList();
            //已失效團購
            var secondGroup = infoList.Where(x => x.IsExpired == true || x.IsSoldOut == true).OrderByDescending(x => DateTime.Parse(x.EndDate)).ToList();
            //團購排序=>已開賣團購 .. 等待開賣團購 .. 已失效團購
            infoList.Clear();
            infoList.AddRange(beginGroup);
            infoList.AddRange(waitGroup);
            infoList.AddRange(secondGroup);
            return infoList;
        }

        /// <summary>
        /// Save GroupBuy into database
        /// </summary>
        /// <param name="userName">Editor</param>
        /// <param name="groupBuy">Data Model GroupBuyInfo</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult Save(string userName, DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();
            result.IsSuccess = true;

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var oldGroupBuy = db.GroupBuy.Where(x => x.ID == groupBuy.ID).FirstOrDefault();
            if (oldGroupBuy != null)
            {
                //update
                var checkResult = CheckBeforeUpdate(groupBuy);
                if (checkResult.IsSuccess)
                {
                    oldGroupBuy.EditUser = userName;

                    if (IsAllowEdit(oldGroupBuy))
                    {
                        var currentStatus = CalculateStatus(oldGroupBuy);
                        if (currentStatus == GroupBuyStatus.StatusEnum.草稿
                            || currentStatus == GroupBuyStatus.StatusEnum.退回
                            || currentStatus == GroupBuyStatus.StatusEnum.待上檔)
                        {
                            //草稿、退回、待上檔，才允許做完整修改
                            oldGroupBuy.ItemID = groupBuy.ItemID;
                            var item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();
                            if (item != null)
                            {
                                oldGroupBuy.ItemName = item.Name;
                            }

                            oldGroupBuy.OriginalPrice = groupBuy.OriginalPrice;
                            oldGroupBuy.GroupBuyPrice = groupBuy.GroupBuyPrice;
                            oldGroupBuy.ProductCost = groupBuy.ProductCost;
                            oldGroupBuy.ShippingCost = groupBuy.ShippingCost;
                            oldGroupBuy.ImgUrl = groupBuy.ImgUrl;
                            oldGroupBuy.BeginDate = groupBuy.BeginDate;
                        }

                        oldGroupBuy.PromoText = groupBuy.PromoText;
                        oldGroupBuy.SalesOrderLimit = groupBuy.SalesOrderLimit;
                        oldGroupBuy.SalesOrderBase = groupBuy.SalesOrderBase;
                        oldGroupBuy.IsExclusive = groupBuy.IsExclusive;
                        oldGroupBuy.IsNeweggUSASync = groupBuy.IsNeweggUSASync;
                        oldGroupBuy.EndDate = groupBuy.EndDate;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = CalculateStatus(oldGroupBuy).ToString() + " is not allow edit";
                    }
                }
                else
                {
                    result = checkResult;
                }
            }
            else
            {
                //insert
                var checkResult = CheckBeforeInsert(groupBuy);
                if (checkResult.IsSuccess)
                {
                    var item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();
                    if (item != null)
                    {
                        groupBuy.ItemName = item.Name;
                    }

                    groupBuy.InUser = userName;
                    groupBuy.InDate = DateTime.UtcNow.AddHours(8);
                    db.GroupBuy.Add(groupBuy);
                }
                else
                {
                    result = checkResult;
                }
            }

            if (result.IsSuccess)
            {
                try
                {
                    db.SaveChanges();
                    if (oldGroupBuy != null)
                    {
                        result.Data = oldGroupBuy;
                    }
                    else
                    {
                        result.Data = groupBuy;
                    }
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.Msg = e.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// 修改資料前檢查團購資料設定是否符合規範限制，通過所有檢查則狀態為True
        /// </summary>
        /// <param name="groupBuy">Data Model GroupBuy</param>
        /// <returns>Data Model OperateResult</returns>
        private GroupBuyOperateResult CheckBeforeUpdate(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();
            result.IsSuccess = true;
            result.Msg = "";

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();

            if (item == null)
            {
                result.IsSuccess = false;
                result.Msg += " ItemID=" + groupBuy.ItemID.ToString() + " is not exist;";
            }

            if (groupBuy.OriginalPrice < 0)
            {
                result.IsSuccess = false;
                result.Msg += " OriginalPrice < 0;";
            }

            if (groupBuy.GroupBuyPrice < 0)
            {
                result.IsSuccess = false;
                result.Msg += " GroupBuyPrice < 0;";
            }

            if (groupBuy.PromoText == null)
            {
                groupBuy.PromoText = "";
            }

            if (groupBuy.ImgUrl == null)
            {
                groupBuy.ImgUrl = "";
            }

            if (groupBuy.BeginDate >= groupBuy.EndDate)
            {
                result.IsSuccess = false;
                result.Msg += " BeginDate >= EndDate;";
            }

            if (groupBuy.RejectCause == null)
            {
                groupBuy.RejectCause = "";
            }

            return result;
        }

        /// <summary>
        /// 新增資料前檢查團購資料設定是否符合規範限制，通過所有檢查則狀態為True
        /// </summary>
        /// <param name="groupBuy">新增資料 Data Model GroupBuy</param>
        /// <returns>Data Model OperateResult</returns>
        private GroupBuyOperateResult CheckBeforeInsert(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();
            result.IsSuccess = true;
            result.Msg = "";

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();

            if (item == null)
            {
                result.IsSuccess = false;
                result.Msg += " ItemID=" + groupBuy.ItemID.ToString() + " is not exist;";
            }

            if (groupBuy.OriginalPrice < 0)
            {
                result.IsSuccess = false;
                result.Msg += " OriginalPrice < 0;";
            }

            if (groupBuy.GroupBuyPrice < 0)
            {
                result.IsSuccess = false;
                result.Msg += " GroupBuyPrice < 0;";
            }

            if (groupBuy.PromoText == null)
            {
                groupBuy.PromoText = "";
            }

            if (groupBuy.ImgUrl == null)
            {
                groupBuy.ImgUrl = "";
            }

            if (groupBuy.BeginDate < DateTime.UtcNow.AddHours(8))
            {
                result.IsSuccess = false;
                result.Msg += " BeginDate < Now;";
            }

            if (groupBuy.BeginDate >= groupBuy.EndDate)
            {
                result.IsSuccess = false;
                result.Msg += " BeginDate >= EndDate;";
            }

            if (groupBuy.RejectCause == null)
            {
                groupBuy.RejectCause = "";
            }

            return result;
        }

        /// <summary>
        /// Send GroupBuy to audit by GroupBuyID
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <param name="userName">Editor</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult SendToAudit(int id, string userName)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                var newStatus = GroupBuyStatus.StatusEnum.待審核;
                if (IsAllowChangeStatus(groupBuy, newStatus))
                {
                    SetGroupBuyStatus(ref groupBuy, newStatus);
                    try
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "GroupBuyID=" + id.ToString() + " Status=" + CalculateStatus(groupBuy).ToString() + " is not allow change status to " + newStatus.ToString();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "GroupBuyID=" + id.ToString() + " is not exist";
            }

            return result;
        }

        /// <summary>
        /// Approve GroupBuy by GroupBuyID
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult Approve(int id)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                var newStatus = GroupBuyStatus.StatusEnum.待上檔;
                if (IsAllowChangeStatus(groupBuy, newStatus))
                {
                    groupBuy.RejectCause = "";
                    SetGroupBuyStatus(ref groupBuy, newStatus);
                    try
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "GroupBuyID=" + id.ToString() + " Status=" + CalculateStatus(groupBuy).ToString() + " is not allow change status to " + newStatus.ToString();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "GroupBuyID=" + id.ToString() + " is not exist";
            }

            return result;
        }

        /// <summary>
        /// Reject GroupBuy by GroupBuyID
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <param name="rejectCause">Reject cause</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult Reject(int id, string rejectCause)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                var newStatus = GroupBuyStatus.StatusEnum.退回;
                if (IsAllowChangeStatus(groupBuy, newStatus))
                {
                    SetGroupBuyStatus(ref groupBuy, newStatus);
                    groupBuy.RejectCause = rejectCause;
                    try
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "GroupBuyID=" + id.ToString() + " Status=" + CalculateStatus(groupBuy).ToString() + " is not allow change status to " + newStatus.ToString();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "GroupBuyID=" + id.ToString() + " is not exist";
            }

            return result;
        }

        /// <summary>
        /// Hide GroupBuy then it will not show on website
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult Hide(int id)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                var newStatus = GroupBuyStatus.StatusEnum.已移除;
                if (IsAllowChangeStatus(groupBuy, newStatus))
                {
                    SetGroupBuyStatus(ref groupBuy, newStatus);
                    try
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "GroupBuyID=" + id.ToString() + " Status=" + CalculateStatus(groupBuy).ToString() + " is not allow change status to " + newStatus.ToString();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "GroupBuyID=" + id.ToString() + " is not exist";
            }

            return result;
        }

        /// <summary>
        /// Delete GroupBuy. Only when this GroupBuy's status is not on sale can be delete.
        /// </summary>
        /// <param name="id">GroupBuyID</param>
        /// <returns>Data Model OperateResult</returns>
        public GroupBuyOperateResult Delete(int id)
        {
            GroupBuyOperateResult result = new GroupBuyOperateResult();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == id).FirstOrDefault();
            if (groupBuy != null)
            {
                if (IsAllowDelete(groupBuy))
                {
                    db.GroupBuy.Remove(groupBuy);
                    try
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    catch (Exception e)
                    {
                        result.IsSuccess = false;
                        result.Msg = e.Message;
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "GroupBuyID=" + id.ToString() + " is not exist";
            }

            return result;
        }

        /// <summary>
        /// Fill data from GroupBuy to GroupBuyInfo
        /// </summary>
        /// <param name="groupBuy">GroupBuy</param>
        /// <returns>Data Model GroupBuyInfo</returns>
        public GroupBuyInfo FillInGroupBuyInfo(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyInfo info = new GroupBuyInfo();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.Item item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();

            info.ID = groupBuy.ID;
            info.ItemID = groupBuy.ItemID;
            info.Title = item.Name;
            info.OriginalPrice = groupBuy.OriginalPrice;
            info.GroupBuyPrice = groupBuy.GroupBuyPrice;
            info.ProductCost = groupBuy.ProductCost;
            info.ShippingCost = groupBuy.ShippingCost;
            info.SalesOrderLimit = groupBuy.SalesOrderLimit;
            info.SalesOrderBase = groupBuy.SalesOrderBase;
            info.SalesOrderCurrentBuffer = GetSoBufferCount(groupBuy.ID) - GetSoRealCount(groupBuy.ID);
            info.IsExclusive = groupBuy.IsExclusive;
            info.IsNeweggUSASync = groupBuy.IsNeweggUSASync;
            info.PromoText = groupBuy.PromoText;
            info.ImgUrl = groupBuy.ImgUrl;
            info.BeginDate = groupBuy.BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            info.EndDate = groupBuy.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
            info.RejectCause = groupBuy.RejectCause;
            info.Status = CalculateStatus(groupBuy).ToString();
            info.InUser = groupBuy.InUser;
            info.InDate = groupBuy.InDate.ToString("yyyy-MM-dd HH:mm:ss");
            info.Sdesc = item.Sdesc;
            if (groupBuy.EditUser != null)
            {
                info.EditUser = groupBuy.EditUser;
            }
            else
            {
                info.EditUser = "";
            }

            if (groupBuy.EditDate.HasValue)
            {
                info.EditDate = groupBuy.EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                info.EditDate = "";
            }

            info.Discount = (groupBuy.OriginalPrice - groupBuy.GroupBuyPrice).ToString();
            if (groupBuy.OriginalPrice > 0)
            {
                info.DiscountPercentage = (Math.Floor(groupBuy.GroupBuyPrice / groupBuy.OriginalPrice * 10000) / 100).ToString();
            }
            else
            {
                info.DiscountPercentage = "0";
            }

            info.AvailableSO = GetSoRealCount(groupBuy.ID).ToString();
            info.IsAllowEdit = IsAllowEdit(groupBuy);
            info.ISAllowDelete = IsAllowDelete(groupBuy);
            info.IsAllowHide = IsAllowChangeStatus(groupBuy, GroupBuyStatus.StatusEnum.已移除);
            info.IsApproved = groupBuy.IsApprove;
            info.IsHide = groupBuy.IsHide;

            return info;
        }

        /// <summary>
        /// Fill data from GroupBuy to view model GroupBuyViewInfo
        /// </summary>
        /// <param name="groupBuy">GroupBuy</param>
        /// <returns>GroupBuyViewInfo</returns>
        public GroupBuyViewInfo FillInGroupBuyViewInfo(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyViewInfo info = new GroupBuyViewInfo();

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            TWNewEgg.DB.TWSQLDB.Models.Item item = db.Item.Where(x => x.ID == groupBuy.ItemID).FirstOrDefault();
            
            TWNewEgg.DB.TWSQLDB.Models.Category category = db.Category.Where(x => x.ID == item.CategoryID).FirstOrDefault();
            TWNewEgg.DB.TWSQLDB.Models.Category second = null;
            TWNewEgg.DB.TWSQLDB.Models.Category store = null;
            int categoryID = new int(), storeID = new int();

            if (category != null)
            {
                categoryID = category.ID;
                second = db.Category.Where(x => x.ID == category.ParentID).FirstOrDefault();
                if (second != null)
                {
                    storeID = second.ID;
                    store = db.Category.Where(x => x.ID == second.ParentID).FirstOrDefault();
                }
            }
            if (store != null)
            {
                storeID = store.ID;
            }

            int salesOrderRealCount = GetSoRealCount(groupBuy.ID);
            int salesOrderBufferCount = GetSoBufferCount(groupBuy.ID);

            info.ID = groupBuy.ID;
            info.IsAvailable = IsAvailable(groupBuy);
            DateTime now = DateTime.UtcNow.AddHours(8);
            if (now <= groupBuy.EndDate)
            {
                info.IsExpired = false;
            }
            else
            {
                info.IsExpired = true;
            }

            if (groupBuy.SalesOrderLimit > 0 && salesOrderRealCount >= groupBuy.SalesOrderLimit)
            {
                info.IsSoldOut = true;
            }
            else
            {
                info.IsSoldOut = false;
            }

            double startHours = (DateTime.UtcNow.AddHours(8) - groupBuy.BeginDate).TotalHours;
            if (startHours > 0 && startHours < 72)
            {
                info.IsShowNew = true;
            }
            else
            {
                info.IsShowNew = false;
            }

            if (salesOrderBufferCount > 50)
            {
                info.IsShowHot = true;
            }
            else
            {
                info.IsShowHot = false;
            }

            info.IsShowExclusive = groupBuy.IsExclusive;
            info.IsShowNeweggUSASync = groupBuy.IsNeweggUSASync;
            info.Title = item.Name;
            info.PromoText = groupBuy.PromoText;
            info.BeginDate = groupBuy.BeginDate.ToString("yyyy/MM/dd hh:mm:ss tt"); //.ToString("yyyy-MM-dd HH:mm:ss");
            info.BeginDate = info.BeginDate.Replace("上午", "AM");
            info.BeginDate = info.BeginDate.Replace("下午", "PM");
            info.EndDate = groupBuy.EndDate.ToString("yyyy/MM/dd hh:mm:ss tt"); //.ToString("yyyy-MM-dd HH:mm:ss");
            info.EndDate = info.EndDate.Replace("上午", "AM");
            info.EndDate = info.EndDate.Replace("下午", "PM");
            info.OriginalPrice = Math.Floor(groupBuy.OriginalPrice).ToString();
            info.Sdesc = item.Sdesc;
            info.ItemID = groupBuy.ItemID;

            //TWNewEgg.ItemService.Service.IItemService itemService = new TWNewEgg.ItemService.Service.ItemServiceRepository();
            info.SellQuantity = itemService.GetSellingQty(item.ID, "Item").ToString();

            //Get Display Price
            List<int> itemIDs = new List<int>();
            itemIDs.Add(item.ID);
            TWNewEgg.ItemService.Service.IItemPrice itemPriceData = new TWNewEgg.ItemService.Service.ItemPriceRepository();
            var getItemPrice = itemPriceData.GetItemDisplayPriceByIDs(itemIDs);
            var itemDisplayPrice = new DB.TWSQLDB.Models.ItemDisplayPrice();
            if (getItemPrice.TryGetValue(item.ID, out itemDisplayPrice))
            {
                decimal displayPrice = 0;
                try
                {
                    displayPrice = Math.Floor(itemDisplayPrice.DisplayPrice);
                }
                catch 
                { 
                }

                decimal discountPrice = 0;
                try
                {
                    discountPrice = Math.Floor(groupBuy.OriginalPrice - itemDisplayPrice.DisplayPrice);
                }
                catch 
                { 
                }

                decimal percent = 0;
                try
                {
                    percent = Math.Floor(Math.Floor(itemDisplayPrice.DisplayPrice / groupBuy.OriginalPrice * 10000) / 100);
                }
                catch 
                {
                }

                if (percent < 0 || percent >= 100)
                {
                    percent = 0;
                }

                switch (CalculateStatus(groupBuy))
                {
                    case GroupBuyStatus.StatusEnum.草稿:
                    case GroupBuyStatus.StatusEnum.待審核:
                    case GroupBuyStatus.StatusEnum.退回:
                    case GroupBuyStatus.StatusEnum.待上檔:
                        /*取消金額遮罩，所有狀態皆直接顯示金額
                        info.GroupBuyPrice = MaskNumber(displayPrice.ToString());
                        info.Discount = MaskNumber(discountPrice.ToString());
                        info.DiscountPercentage = MaskNumber(percent.ToString());
                        break;*/
                    case GroupBuyStatus.StatusEnum.上檔中:
                    case GroupBuyStatus.StatusEnum.已售完:
                    case GroupBuyStatus.StatusEnum.已下檔:
                    case GroupBuyStatus.StatusEnum.已移除:
                        info.GroupBuyPrice = displayPrice.ToString();
                        info.Discount = discountPrice.ToString();
                        info.DiscountPercentage = percent.ToString();
                        break;
                }
            }
            else
            {
                info.GroupBuyPrice = "XX";
                info.Discount = "XX";
                info.DiscountPercentage = "XX";
            }

            info.SalesOrderCount = salesOrderBufferCount.ToString();
            switch (CalculateStatus(groupBuy))
            {
                case GroupBuyStatus.StatusEnum.待上檔:
                    info.ItemLinkButtonText = "前往";
                    info.ItemLinkButtonImageUrl = enableImageUrl;
                    info.ItemLink = string.Format(ITEMURLLINK, item.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    break;
                case GroupBuyStatus.StatusEnum.上檔中:
                    info.ItemLinkButtonText = "前往";
                    info.ItemLinkButtonImageUrl = enableImageUrl;
                    info.ItemLink = string.Format(ITEMURLLINK, item.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    break;
                case GroupBuyStatus.StatusEnum.已售完:
                    info.ItemLinkButtonText = "已售完";
                    info.ItemLinkButtonImageUrl = endImageUrl;
                    info.ItemLink = string.Format(ITEMURLLINK, item.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    break;
                case GroupBuyStatus.StatusEnum.已下檔:
                    info.ItemLinkButtonText = "已結束";
                    info.ItemLinkButtonImageUrl = endImageUrl;
                    info.ItemLink = string.Format(ITEMURLLINK, item.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    break;
            }

            //TWNewEgg.ItemService.Service.IItemService itemService = new TWNewEgg.ItemService.Service.ItemServiceRepository();
            info.SellQuantity = itemService.GetSellingQty(item.ID, "Item").ToString();
            if (itemService.GetSellingQty(item.ID, "Item") < 1)
            {
                info.SellQuantity = "0";
                info.ItemLinkButtonText = "已售完";
                info.ItemLinkButtonImageUrl = endImageUrl;
                info.ItemLink = "#";
            }
            string id = item.ID.ToString("00000000");
            info.ImgUrl = "/pic/item/" + id.Substring(0, 4) + "/" + id.Substring(4) + "_1_640.jpg";

            return info;
        }

        /// <summary>
        /// 取得團購訂單數量，真實有效訂單 + 基礎數量
        /// </summary>
        /// <param name="groupBuyID">GroupBuy ID</param>
        /// <returns>訂單數量</returns>
        public int GetSoBufferCount(int groupBuyID)
        {
            int salesOrderCount = 0;

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == groupBuyID).FirstOrDefault();
            if (groupBuy != null)
            {
                var currentStatus = CalculateStatus(groupBuy);
                if (currentStatus == GroupBuyStatus.StatusEnum.上檔中
                    || currentStatus == GroupBuyStatus.StatusEnum.已售完
                    || currentStatus == GroupBuyStatus.StatusEnum.已下檔)
                {
                    //y = (8a^3) / (x^2 + 4a^2)
                    //a = 25
                    //y 為一個 50 ~ 0 的漸進線
                    //x = 50 時 y = 25
                    //x設定為已販售的分鐘數，利用y作為百分比，則50分鐘後達到50%
                    //基礎數量 = GroupBuy.SalesOrderBase * (y/50)
                    //訂單總數 = 真實有效訂單數 + 基礎數量
                    double expendMinutes = (DateTime.UtcNow.AddHours(8) - groupBuy.BeginDate).TotalMinutes;
                    decimal bufferPercent = 1m - ((125000m / (((decimal)expendMinutes * (decimal)expendMinutes) + 2500m)) / 50m);
                    salesOrderCount = int.Parse(Math.Floor((decimal)groupBuy.SalesOrderBase * bufferPercent).ToString());
                }

                salesOrderCount = salesOrderCount + GetSoRealCount(groupBuy.ID);
            }

            return salesOrderCount;
        }

        /// <summary>
        /// 取得團購訂單數量，真實有效訂單
        /// </summary>
        /// <param name="groupBuyID">GroupBuy ID</param>
        /// <returns>訂單數量</returns>
        private int GetSoRealCount(int groupBuyID)
        {
            int salesOrderCount = 0;

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var groupBuy = db.GroupBuy.Where(x => x.ID == groupBuyID).FirstOrDefault();
            if (groupBuy != null)
            {
                DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
                DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
                List<int> frontQtyList = frontend.SalesOrder.Where(x => x.CreateDate >= groupBuy.BeginDate && x.CreateDate <= groupBuy.EndDate && x.Status == (int)DB.TWBACKENDDB.Models.Cart.status.正常).Join(frontend.SalesOrderItem, x => x.Code, y => y.SalesorderCode, (x, y) => y).Where(x => x.ItemID == groupBuy.ItemID && x.Qty != null).Select(x => x.Qty).ToList();
                int frontCount = frontQtyList.Sum();
                List<Nullable<int>> qtyList = backend.Cart.Where(x => x.CreateDate >= groupBuy.BeginDate && x.CreateDate <= groupBuy.EndDate && (x.Status == (int)DB.TWBACKENDDB.Models.Cart.status.正常 || x.Status == (int)DB.TWBACKENDDB.Models.Cart.status.完成)).Join(backend.Process, x => x.ID, y => y.CartID, (x, y) => y).Where(x => x.StoreID == groupBuy.ItemID && x.Qty != null).Select(x => x.Qty).ToList();
                qtyList = qtyList.Where(x => x.HasValue).ToList();
                int backCount = (int)qtyList.Sum();
                if (frontCount > backCount)
                {
                    salesOrderCount = frontCount;
                }
                else
                {
                    salesOrderCount = backCount;
                }
            }

            return salesOrderCount;
        }

        private string MaskNumber(string original)
        {
            string result = "";

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\d]");
            result = regex.Replace(original, "X");

            return result;
        }

        private bool IsAllowChangeStatus(DB.TWSQLDB.Models.GroupBuy groupBuy, GroupBuyStatus.StatusEnum newStatus)
        {
            bool result = false;

            switch (CalculateStatus(groupBuy))
            {
                case GroupBuyStatus.StatusEnum.草稿:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.退回:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.待審核:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待上檔:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.退回:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待審核:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.待上檔:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.待審核:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.上檔中:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待上檔:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.已售完:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.上檔中:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.已下檔:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.已下檔:
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                            result = false;
                            break;
                    }

                    break;
                case GroupBuyStatus.StatusEnum.已移除:
                    switch (newStatus)
                    {
                        case GroupBuyStatus.StatusEnum.已移除:
                            result = true;
                            break;
                        case GroupBuyStatus.StatusEnum.草稿:
                        case GroupBuyStatus.StatusEnum.待審核:
                        case GroupBuyStatus.StatusEnum.退回:
                        case GroupBuyStatus.StatusEnum.待上檔:
                        case GroupBuyStatus.StatusEnum.上檔中:
                        case GroupBuyStatus.StatusEnum.已售完:
                        case GroupBuyStatus.StatusEnum.已下檔:
                            result = false;
                            break;
                    }

                    break;
            }

            return result;
        }

        private bool IsAllowEdit(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            bool result = false;

            switch (CalculateStatus(groupBuy))
            {
                case GroupBuyStatus.StatusEnum.草稿:
                case GroupBuyStatus.StatusEnum.退回:
                case GroupBuyStatus.StatusEnum.待上檔:
                case GroupBuyStatus.StatusEnum.上檔中:
                    result = true;
                    break;
                case GroupBuyStatus.StatusEnum.待審核:
                case GroupBuyStatus.StatusEnum.已售完:
                case GroupBuyStatus.StatusEnum.已下檔:
                case GroupBuyStatus.StatusEnum.已移除:
                    result = false;
                    break;
            }

            return result;
        }

        private bool IsAllowDelete(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            bool result = false;

            switch (CalculateStatus(groupBuy))
            {
                case GroupBuyStatus.StatusEnum.草稿:
                case GroupBuyStatus.StatusEnum.退回:
                    result = true;
                    break;
                case GroupBuyStatus.StatusEnum.待審核:
                case GroupBuyStatus.StatusEnum.待上檔:
                case GroupBuyStatus.StatusEnum.上檔中:
                case GroupBuyStatus.StatusEnum.已售完:
                case GroupBuyStatus.StatusEnum.已下檔:
                case GroupBuyStatus.StatusEnum.已移除:
                    result = false;
                    break;
            }

            return result;
        }

        private bool IsAvailable(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            bool result = false;

            if (CalculateStatus(groupBuy) == GroupBuyStatus.StatusEnum.上檔中)
            {
                result = true;
            }

            return result;
        }

        private void SetGroupBuyStatus(ref DB.TWSQLDB.Models.GroupBuy groupBuy, GroupBuyStatus.StatusEnum newStatus)
        {
            /*      IsWaitingForApprove IsReject    IsHide    IsApprove	

            草稿	    False			    False	    False     False		
            待審	    True			    False	    False     False		
            退回	    False			    True	    False     False		
            待上  	False			    False	    False     True		
            上檔	    False			    False	    False     True		
            售完	    False			    False	    False     True		
            下檔	    False			    False	    False     True		
            移除	    False			    False	    True      True		
             */
            if (IsAllowChangeStatus(groupBuy, newStatus))
            {
                switch (newStatus)
                {
                    case GroupBuyStatus.StatusEnum.草稿:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = false;
                        break;
                    case GroupBuyStatus.StatusEnum.退回:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = true;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = false;
                        break;
                    case GroupBuyStatus.StatusEnum.待審核:
                        groupBuy.IsWaitingForApprove = true;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = false;
                        break;
                    case GroupBuyStatus.StatusEnum.待上檔:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = true;
                        break;
                    case GroupBuyStatus.StatusEnum.上檔中:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = true;
                        break;
                    case GroupBuyStatus.StatusEnum.已售完:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = true;
                        break;
                    case GroupBuyStatus.StatusEnum.已下檔:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = false;
                        groupBuy.IsApprove = true;
                        break;
                    case GroupBuyStatus.StatusEnum.已移除:
                        groupBuy.IsWaitingForApprove = false;
                        groupBuy.IsReject = false;
                        groupBuy.IsHide = true;
                        groupBuy.IsApprove = true;
                        break;
                }
            }
        }

        private GroupBuyStatus.StatusEnum CalculateStatus(DB.TWSQLDB.Models.GroupBuy groupBuy)
        {
            GroupBuyStatus.StatusEnum result = GroupBuyStatus.StatusEnum.草稿;

            /*      IsWaitingForApprove IsReject    IsHide    IsApprove	

            草稿	    False			    False	    False     False		
            待審	    True			    False	    False     False		
            退回	    False			    True	    False     False		
            待上  	False			    False	    False     True		
            上檔	    False			    False	    False     True		
            售完	    False			    False	    False     True		
            下檔	    False			    False	    False     True		
            移除	    False			    False	    True      True		
             */

            if (groupBuy.IsWaitingForApprove)
            {
                result = GroupBuyStatus.StatusEnum.待審核;
            }
            else
            {
                if (groupBuy.IsReject)
                {
                    result = GroupBuyStatus.StatusEnum.退回;
                }
                else
                {
                    if (groupBuy.IsHide)
                    {
                        result = GroupBuyStatus.StatusEnum.已移除;
                    }
                    else
                    {
                        if (groupBuy.IsApprove)
                        {
                            var now = DateTime.UtcNow.AddHours(8);
                            if (now < groupBuy.BeginDate)
                            {
                                result = GroupBuyStatus.StatusEnum.待上檔;
                            }
                            else
                            {
                                if (now <= groupBuy.EndDate)
                                {
                                    int salesOrderRealCount = GetSoRealCount(groupBuy.ID);
                                    if (groupBuy.SalesOrderLimit > 0 && salesOrderRealCount >= groupBuy.SalesOrderLimit)
                                    {
                                        result = GroupBuyStatus.StatusEnum.已售完;
                                    }
                                    else
                                    {
                                        result = GroupBuyStatus.StatusEnum.上檔中;
                                    }
                                }
                                else
                                {
                                    result = GroupBuyStatus.StatusEnum.已下檔;
                                }
                            }
                        }
                        else
                        {
                            result = GroupBuyStatus.StatusEnum.草稿;
                        }
                    }
                }
            }

            return result;
        }
    }
}