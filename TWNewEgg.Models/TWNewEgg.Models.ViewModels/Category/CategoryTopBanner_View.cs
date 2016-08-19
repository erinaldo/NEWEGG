using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Category
{
    public class CategoryTopBanner_View
    {
        public CategoryTopBanner_View()
        {
            this.ItemList = new List<CategoryTopBanner_ItemView>();
        }
        public int ID { get; set; }
        /// <summary>
        /// 櫥窗名稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 第三層類別ID
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// 第一層類別ID
        /// </summary>
        public int StoreID { get; set; }
        /// <summary>
        /// 廣告類型
        ///    1-	item
        ///    2-	image
        /// </summary>
        public int AdType { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int Showorder { get; set; }
        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 圖片連結
        /// </summary>
        public string ImageLink { get; set; }

        public List<CategoryTopBanner_ItemView> ItemList { get; set; }
    }
}
