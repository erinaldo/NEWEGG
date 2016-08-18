using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Category
{
    public class AdLayer3DM
    {
        public AdLayer3DM()
        {
            this.ItemList = new List<AdLayer3ItemDM>();
        }
        public enum adType
        {
            item = 1,
            image = 2
        };
        public enum showAll
        {
            不顯示 = 0,
            顯示 = 1
        };
        public int? ID { get; set; }
        /// <summary>
        /// 櫥窗名稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 第三層類別ID
        /// </summary>
        public int? CategoryID { get; set; }
        /// <summary>
        /// 廣告類型
        ///    1-	item
        ///    2-	image
        /// </summary>
        public int? AdType { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int? Showorder { get; set; }
        /// <summary>
        /// 0：不顯示；1：顯示
        /// </summary>
        public int? ShowAll { get; set; }
        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 圖片連結
        /// </summary>
        public string ImageLink { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime? UpdateDate { get; set; }

        public List<AdLayer3ItemDM> ItemList { get; set; }
    }
    
}
