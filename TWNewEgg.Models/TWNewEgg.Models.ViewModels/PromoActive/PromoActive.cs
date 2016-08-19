using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.PromoActive
{
    public class PromoActiveView
    {
        public PromoActiveView()
        {
            this.listPromoActive = new List<PromoActive>();
            this.showPage = new List<Page.ShowPage>();
        }
        public List<PromoActive> listPromoActive { get; set; }
        public int totalPage { get; set; }
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> showPage { get; set; }
        public string searchFrom { get; set; }
    }
    public class PromoActive
    {
        public PromoActive()
        {
            this.totalDataCount = 0;
        }
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLink { get; set; }
        public int FuncType { get; set; }
        public string Name { get; set; }
        public string NameLink { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public Nullable<int> TakeType { get; set; }
        public Nullable<System.DateTime> DeclareDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        //活動是否進行中
        public int ActivityOrNot { get; set; }
        ////類型
        //public string Type_str { get; set; }
        ////館別名稱
        //public string categoryDescription { get; set; }
        //活動進行中的剩餘時間
        public string ActivityRemainDate { get; set; }
        //所有資料的總數
        public int totalDataCount { get; set; }
    }
    public enum ActivityType
    {
        滿額折_現折 = 1,
        折價券 = 2,
        回饋金 = 3,
        紅利點數 = 4,
        抽獎 = 5,
        贈品_獎品 = 6,
        銀行 = 7,
        折扣 = 8
    }
    public enum ActivityStatus
    {
        進行中 = 1, 
        已結束 = 0
    }
    
}
