using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.VotingActivity
{
    /// <summary>
    /// 投票活動
    /// </summary>
    public class VotingActivityGroup
    {
        /// <summary>
        /// 投票方式
        /// </summary>
        public enum RestrictTypeOption
        {
            /// <summary>
            /// 每小時限制
            /// </summary>
            PerHour = 1,

            /// <summary>
            /// 每日限制
            /// </summary>
            EveryDay = 2,

            /// <summary>
            /// 限制總票數
            /// </summary>
            Amount = 3
        }

        /// <summary>
        /// 重複對Item投票
        /// </summary>
        public enum VotingItemRepeateOption
        {
            /// <summary>
            /// 不可重複
            /// </summary>
            CantRepeate = 0,

            /// <summary>
            /// 可重複
            /// </summary>
            Repeate = 1
        }

        /// <summary>
        /// 投票帳號來源限制
        /// </summary>
        public enum RestrictAccountOption
        {
            /// <summary>
            /// 不限制
            /// </summary>
            Any = 0,

            /// <summary>
            /// 限制新蛋會員
            /// </summary>
            Newegg = 1
        }

        /// <summary>
        /// 流水編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活動說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 開始顯示活動時間
        /// </summary>
        public DateTime DisplayStartDate { get; set; }

        /// <summary>
        /// 結束顯示活動時間
        /// </summary>
        public DateTime DisplayEndDate { get; set; }

        /// <summary>
        /// 投票開始時間
        /// </summary>
        public DateTime VotingStartDate { get; set; }

        /// <summary>
        /// 投票結束時間
        /// </summary>
        public DateTime VotingEndDate { get; set; }

        /// <summary>
        /// 上線狀態, 0:下線, 1:上線, 2:Testing
        /// </summary>
        public int OnlineStatus { get; set; }

        /// <summary>
        /// 投票方式, 請參照RestrictTypeOption
        /// </summary>
        public int RestrictType { get; set; }

        /// <summary>
        /// 票數限制, 配合RestrictType做計數基準, 若RestrictType為3時, 將無視此欄位
        /// </summary>
        public int RestrictLimit { get; set; }

        /// <summary>
        /// 是否可以對同一個Item重複投票, 請參照VotingItemRepeateOption
        /// </summary>
        public int VotingItemRepeate { get; set; }

        /// <summary>
        /// 投票帳號限制, 請參照 RestrictAccountOption
        /// </summary>
        public int RestrictAccount { get; set; }

        /// <summary>
        /// 創建人
        /// </summary>
        public int CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改人
        /// </summary>
        public int? UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}
