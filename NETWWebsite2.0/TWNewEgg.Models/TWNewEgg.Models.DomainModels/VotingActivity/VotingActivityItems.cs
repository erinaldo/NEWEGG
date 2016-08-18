using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.VotingActivity
{
    /// <summary>
    /// 參加投票活動的Item
    /// </summary>
    public class VotingActivityItems
    {
        /// <summary>
        /// 活動Id, refer VotingActivityGroup.Id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 簡易說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 開始顯示項目時間, 若未設定, 將繼承活動的DisplayStartDate
        /// </summary>
        public DateTime DisplayStartDate { get; set; }

        /// <summary>
        /// 結束顯示項目時間, 若未設定, 將繼承活動的DisplayEndDate
        /// </summary>
        public DateTime DisplayEndDate { get; set; }

        /// <summary>
        /// 投票開始時間, 若未設定, 將繼承活動的VotingStartDate
        /// </summary>
        public DateTime VotingStartDate { get; set; }

        /// <summary>
        /// 投票結束時間, 若未設定, 將繼承活動的VotingEndDate
        /// </summary>
        public DateTime VotingEndDate { get; set; }

        /// <summary>
        /// 初始票數
        /// </summary>
        public int InitVoting { get; set; }

        /// <summary>
        /// 系統投票數
        /// </summary>
        public int SystemVoting { get; set; }

        /// <summary>
        /// 實際投票數
        /// </summary>
        public int RealVoting { get; set; }

        /// <summary>
        /// 上線狀態, 0:下線, 1:上線, 2:Testing
        /// </summary>
        public int OnlineStatus { get; set; }

        /// <summary>
        /// 顯示排序順序, 當值為0時為不顯示
        /// </summary>
        public int ShowOrder { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public int CreateUser { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public int? UpdateUser { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}
