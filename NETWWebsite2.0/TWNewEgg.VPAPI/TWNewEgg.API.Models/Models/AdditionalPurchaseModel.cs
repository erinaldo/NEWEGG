using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TWNewEgg.API.Models
{
    public class AdditionalPurchase
    {
        /// <summary>
        /// 商家 ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品ItemTempID
        /// </summary>
        public int ItemTempID { get; set; }

        /// <summary>
        /// 商品ItemID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 商品ItemSketchID
        /// </summary>
        public int ItemSketchID { get; set; }

        /// <summary>
        /// 商品顯示狀態
        /// </summary>
        public int? ShowOrder { get; set; }

        /// <summary>
        /// 修改搜尋目標
        /// </summary>
        public AdditionalPurchaseSearchTarget SearchTarget { get; set; }

        /// <summary>
        /// 建立及更新資訊
        /// </summary>
        public CreateAndUpdateIfno CreateAndUpdate { get; set; }

        public AdditionalPurchase()
        {
            this.CreateAndUpdate = new CreateAndUpdateIfno();
        }
        /// <summary>
        /// 修改加價購目標
        /// </summary>
        public enum AdditionalPurchaseSearchTarget
        {
            /// <summary>
            /// 商家商品編號
            /// </summary>
            SellerProductID = 0,

            /// <summary>
            /// 廠商產品編號
            /// </summary>
            MenufacturePartNum = 1,

            /// <summary>
            /// 草稿 ID
            /// </summary>
            ItemSketchID = 2,

            /// <summary>
            /// 商品名稱
            /// </summary>
            ProductName = 3,

            /// <summary>
            /// 新蛋賣場編號
            /// </summary>
            ItemID = 4,

            /// <summary>
            /// 新蛋 Item 待審區 ID
            /// </summary>
            ItemTempID = 5,

            /// <summary>
            /// Groupid
            /// </summary>
            GroupId = 6,

            All
        }

        /// <summary>
        /// 修改加價購目標
        /// </summary>
        public enum ShowOrderType
        {
            /// <summary>
            /// 不顯示
            /// </summary>
            不顯示 = 0,

            /// <summary>
            /// 顯示
            /// </summary>
            顯示 = 1,

            /// <summary>
            /// 加價購
            /// </summary>
            加價購 = -3,

        }
        /// <summary>
        /// 建立及更新資訊
        /// </summary>
        public class CreateAndUpdateIfno
        {
            /// <summary>
            /// 建立者
            /// </summary>
            public int CreateUser { get; set; }

            /// <summary>
            /// 建檔日期
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 更新者
            /// </summary>
            public int UpdateUser { get; set; }

            /// <summary>
            /// 更新日期
            /// </summary>
            public DateTime UpdateDate { get; set; }
        }
    }
}