using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [System.ComponentModel.DataAnnotations.Schema.Table("PromoActive")]
    public class PromoActive
    {
        /// <summary>
        /// 行銷活動類型
        /// </summary>
        /// <remarks>數值的部份由 TWNewegg DB schema 文件訂定</remarks>
        public enum PromoActiveType
        {
            滿額折 = 1,
            折價券 = 2,
            回饋金 = 3,
            紅利點數 = 4,
            抽獎 = 5,
            贈獎 = 6,
            銀行 = 7,
            折扣 = 8
        }

        /// <summary>
        /// 領獎方式
        /// </summary>
        /// <remarks>數值的部份由 TWNewegg DB schema 文件訂定</remarks>
        public enum PromoActiveTakeType
        {
            親領 = 1,
            寄送 = 2,
            歸戶 = 3
        }

        /// <summary>
        /// 行銷活動編號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        /// <value>enum PromoActiveType</value>
        public int Type { get; set; }

        /// <summary>
        /// 是否顯示於前台
        /// </summary>
        /// <value>0:不顯示</value>
        /// <value>1:顯示</value>
        public int Status { get; set; }

        /// <summary>
        /// 領獎方式
        /// </summary>
        /// <value>enum PromoActiveTakeType</value>
        public Nullable<int> TakeType { get; set; }

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
        /// 建立人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

}
