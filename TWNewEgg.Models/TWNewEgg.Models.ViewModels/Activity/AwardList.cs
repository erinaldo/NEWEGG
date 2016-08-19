using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Activity
{
    /// <summary>
    /// 中獎名單頁 ViewModel
    /// </summary>
    public class Award
    {
        /// <summary>
        /// 行銷活動清單
        /// </summary>
        public List<PromoActive> ActivityList { get; set; }

        /// <summary>
        /// 中獎名單頁最大頁數
        /// </summary>
        public int MaxPage { get; set; }

        /// <summary>
        /// 中獎名單頁排序方式
        /// </summary>
        /// <value>enum ActivityListOrderByType</value>
        public int OrderBy { get; set; }

        /// <summary>
        /// 目前頁數
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分頁列表
        /// </summary>
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> ShowPage { get; set; }
        
        public Award()
        {
            ActivityList = new List<PromoActive>();
            MaxPage = 1;
            OrderBy = (int)ActivityListOrderByType.最新活動;
            PageIndex = 1;
            ShowPage = new List<Page.ShowPage>();
        }
    }

    /// <summary>
    /// 行銷活動
    /// </summary>
    public class PromoActive
    {
        /// <summary>
        /// 行銷活動編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 行銷活動類型名稱
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType</value>
        public string TypeName { get; set; }

        /// <summary>
        /// 館別名稱
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 館別連結
        /// </summary>
        public string CategoryLink { get; set; }

        /// <summary>
        /// 行銷活動名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 行銷活動連結
        /// </summary>
        public string NameLink { get; set; }

        /// <summary>
        /// 行銷活動開始時間
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 行銷活動結束時間
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 得獎公告日
        /// </summary>
        public DateTime DeclareDate { get; set; }

        /// <summary>
        /// 領獎方式名稱
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveTakeType</value>
        public string TakeTypeName { get; set; }

        /// <summary>
        /// 行銷活動備註
        /// </summary>
        public string Note { get; set; }
    }

    public class AwardListSearchCondition
    {
        /// <summary>
        /// 中獎清單排序方式
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 目前頁數
        /// </summary>
        public int PageIndex { get; set; }

        public AwardListSearchCondition()
        {
            OrderBy = 0;
            PageIndex = 1;
        }
    }

    /// <summary>
    /// 中獎清單排序方式
    /// </summary>
    public enum ActivityListOrderByType
    { 
        最新活動 = 0,
        即將結束 = 1
    }
}
