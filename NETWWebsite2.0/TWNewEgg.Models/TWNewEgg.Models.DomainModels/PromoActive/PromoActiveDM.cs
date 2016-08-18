using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PromoActive
{
    public class PromoActiveDM
    {
        /// <summary>
        /// 行銷活動編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 館別名稱
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 館別連結
        /// </summary>
        public string CategoryLink { get; set; }

        /// <summary>
        /// 公佈類別
        /// </summary>
        /// <value>0:全部</value>
        /// <value>1:行銷活動</value>
        /// <value>2:中獎名單</value>
        public int FuncType { get; set; }

        /// <summary>
        /// 行銷活動名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 行銷活動連結
        /// </summary>
        public string NameLink { get; set; }

        /// <summary>
        /// 行銷活動贈品
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 行銷活動備註
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 行銷活動類型
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType</value>
        public int Type { get; set; }

        /// <summary>
        /// 行銷活動類型名稱
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveType</value>
        public string TypeName { get; set; }

        /// <summary>
        /// 是否顯示在前台
        /// </summary>
        /// <value>0:不顯示</value>
        /// <value>1:顯示</value>
        public int Status { get; set; }

        /// <summary>
        /// 領獎方式
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveTakeType</value>
        public Nullable<int> TakeType { get; set; }

        /// <summary>
        /// 領獎方式名稱
        /// </summary>
        /// <value>enum TWNewEgg.Models.DBModels.TWSQLDB.PromoActive.PromoActiveTakeType</value>
        public string TakeTypeName { get; set; }

        /// <summary>
        /// 得獎公告日
        /// </summary>
        public Nullable<System.DateTime> DeclareDate { get; set; }

        /// <summary>
        /// 行銷活動開始時間
        /// </summary>
        public Nullable<System.DateTime> StartDate { get; set; }

        /// <summary>
        /// 行銷活動結束時間
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }
       
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 是否已上傳中獎名單
        /// </summary>
        public bool IsImport { get; set; }

        /// <summary>
        /// 紀錄資料總比數
        /// </summary>
        public int totalDataCount { get; set; }
    }

    /// <summary>
    /// 中獎名單頁 DomainModel
    /// </summary>
    public class AwardDM
    {
        /// <summary>
        /// 行銷活動清單
        /// </summary>
        public List<PromoActiveDM> ActivityList { get; set; }

        /// <summary>
        /// 中獎名單頁最大頁數
        /// </summary>
        public int MaxPage { get; set; }

        /// <summary>
        /// 中獎名單頁排序方式
        /// </summary>
        /// <value>ActivityListOrderByType</value>
        public int OrderBy { get; set; }

        /// <summary>
        /// 中獎名單頁指定顯示頁數
        /// </summary>
        public int PageIndex { get; set; }

        public AwardDM()
        {
            PageIndex = 1;
            MaxPage = 1;
            ActivityList = new List<PromoActiveDM>();
            OrderBy = (int)ActivityListOrderByTypeDM.最新活動;
        }
    }

    /// <summary>
    /// 中獎名單頁排序方式
    /// </summary>
    public enum ActivityListOrderByTypeDM
    {
        最新活動 = 0,
        即將結束 = 1
    }
}
