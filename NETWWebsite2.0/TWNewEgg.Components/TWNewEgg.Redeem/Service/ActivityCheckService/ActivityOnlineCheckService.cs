using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.ActivityCheckService
{
    public class ActivityOnlineCheckService
    {
        /// <summary>
        /// 取得所需活動的執行狀態
        /// </summary>
        /// <param name="activityName">活動名稱</param>
        /// <returns>返回Dictionary格式，Key儲存活動是否尚在進行中，true:是，false:不是;Value則儲存失敗訊息</returns>
        public ActivityData GetActivityStatus(string activityName)
        {

            ActivityData activityStatus = new ActivityData();
            if (!string.IsNullOrEmpty(activityName))
            {
                switch (activityName.ToLower())
                {
                    // 三月會員招募 x Omusic
                    case "omusic":
                        activityStatus = ActivityDateCheck("omusic");
                        break;
                    // 四月會員招募 coupon
                    case "apriljoinus":
                        activityStatus = ActivityDateCheck("apriljoinus");
                        break;
                    // 無此資訊時
                    default:
                        activityStatus.StartDate = null;
                        activityStatus.EndDate = null;
                        activityStatus.Deadline = null;
                        activityStatus.ActivityName = activityName;
                        activityStatus.IsEffective = false;
                        activityStatus.ErrorMessage = "無[" + activityName + "]活動項目";
                        break;
                }
            }
            else
            {
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = "活動名稱輸入錯誤!";
            }

            return activityStatus;
        }

        /// <summary>
        /// 取得活動狀態
        /// </summary>
        /// <returns>返回執行結果</returns>
        public ActivityData ActivityDateCheck(string activityName)
        {
            ActivityData activityStatus = new ActivityData();
            string strStartDate = string.Empty;
            string strEndDate = string.Empty;
            string strDeadlineData = string.Empty;
            try
            {
                // 成功取得日期資訊
                strStartDate = System.Configuration.ConfigurationManager.AppSettings["RegisterActivityStartDate"];
                strEndDate = System.Configuration.ConfigurationManager.AppSettings["RegisterActivityEndDate"];
                strDeadlineData = System.Configuration.ConfigurationManager.AppSettings["ActivitiesDeadline"];
            }
            catch (Exception e)
            {
                // 日期資訊取得失敗
                //logger.Info("Account GetDateTime 日期資訊取得失敗!");
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = "活動尚未開始或已結束!";
            }
            // 取得活動起始時間
            ActivityData startDateData = GetDateTime("StartDate", activityName, "RegisterActivityStartDate", strStartDate);
            // 取得活動結束時間
            ActivityData endDateData = GetDateTime("EndDate", activityName, "RegisterActivityEndDate", strEndDate);
            // 取得活動截止日期
            ActivityData deadlineData = GetDateTime("Deadline", activityName, "ActivitiesDeadline", strDeadlineData);
            // check 起始時間
            if (startDateData.StartDate == null)
            {
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = startDateData.ErrorMessage;
                return activityStatus;
            }
            else
            {
                activityStatus.StartDate = startDateData.StartDate;
            }
            // Check 結束時間
            if (endDateData.EndDate == null)
            {
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = endDateData.ErrorMessage;
                return activityStatus;
            }
            else
            {
                activityStatus.EndDate = endDateData.EndDate;
            }
            // 序號兌換截止時間
            if (deadlineData.Deadline == null)
            {
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = deadlineData.ErrorMessage;
            }
            else
            {
                activityStatus.Deadline = deadlineData.Deadline;
            }
            // 檢視目前時間是否在活動有效時間內
            if ((DateTime)activityStatus.StartDate <= DateTime.Now && DateTime.Now < (DateTime)activityStatus.EndDate)
            {
                activityStatus.IsEffective = true;
                activityStatus.ErrorMessage = string.Empty;
            }
            else
            {
                activityStatus.IsEffective = false;
                activityStatus.ErrorMessage = "活動尚未開始或已結束!";
            }

            return activityStatus;
        }

        /// <summary>
        /// 活動日期取得與轉換string格式為Date(Dictionary<string, Dictionary<bool, Dictionary<DateTime, string>>>)
        /// </summary>
        /// <param name="action">所需的日期欄位名稱</param>
        /// <param name="activityName">活動名稱</param>
        /// <param name="dateName">日期名稱，RegisterActivityStartDate、RegisterActivityEndDate、ActivitiesDeadline</param>
        /// <param name="strDateData">字串格式的活動日期</param>
        /// <returns>返回轉換後日期資訊</returns>
        public ActivityData GetDateTime(string action, string activityName, string dateName, string strDateData)
        {
            ActivityData dateAndStatus = null;
            Nullable<DateTime> searchDateTime = null;

            List<string> dateList = strDateData.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList();
            foreach (string analysis in dateList)
            {
                List<string> findActivity = analysis.Split(':').Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (findActivity.Count == 2 && findActivity[0].ToLower() == activityName.ToLower())
                {
                    dateAndStatus = new ActivityData();
                    searchDateTime = new Nullable<DateTime>();
                    try
                    {
                        // 執行日期轉換成功
                        searchDateTime = Convert.ToDateTime(findActivity[1]);
                        switch (action.ToLower())
                        {
                            case "startdate":
                                dateAndStatus.StartDate = searchDateTime;
                                dateAndStatus.ActivityName = dateName.ToLower();
                                dateAndStatus.IsEffective = true;
                                dateAndStatus.ErrorMessage = string.Empty;
                                break;
                            case "enddate":
                                dateAndStatus.EndDate = searchDateTime;
                                dateAndStatus.ActivityName = dateName.ToLower();
                                dateAndStatus.IsEffective = true;
                                dateAndStatus.ErrorMessage = string.Empty;
                                break;
                            case "deadline":
                                dateAndStatus.Deadline = searchDateTime;
                                dateAndStatus.ActivityName = dateName.ToLower();
                                dateAndStatus.IsEffective = true;
                                dateAndStatus.ErrorMessage = string.Empty;
                                break;
                            default:
                                dateAndStatus.StartDate = null;
                                dateAndStatus.EndDate = null;
                                dateAndStatus.Deadline = null;
                                dateAndStatus.ActivityName = dateName.ToLower();
                                dateAndStatus.IsEffective = false;
                                dateAndStatus.ErrorMessage = "無[" + action + "]欄位";
                                break;
                        }

                        return dateAndStatus;
                    }
                    catch (Exception e)
                    {
                        // 執行日期轉換失敗
                        //logger.Info("Account GetDateTime 日期轉換失敗 : [ErrorMessage]" + e.Message + " [StrackTrace]" + e.StackTrace);
                        dateAndStatus.StartDate = null;
                        dateAndStatus.EndDate = null;
                        dateAndStatus.Deadline = null;
                        dateAndStatus.ActivityName = dateName.ToLower();
                        dateAndStatus.IsEffective = false;
                        dateAndStatus.ErrorMessage = "日期轉換失敗";

                        return dateAndStatus;
                    }
                }
            }

            if (dateAndStatus == null)
            {
                // 執行日期轉換失敗
                dateAndStatus = new ActivityData();
                dateAndStatus.StartDate = null;
                dateAndStatus.EndDate = null;
                dateAndStatus.Deadline = null;
                dateAndStatus.ActivityName = dateName.ToLower();
                dateAndStatus.IsEffective = false;
                dateAndStatus.ErrorMessage = "無日期可轉換";

                //logger.Info("Account GetDateTime 日期轉換失敗 : 無日期可轉換");
            }

            return dateAndStatus;
        }
    }
}
