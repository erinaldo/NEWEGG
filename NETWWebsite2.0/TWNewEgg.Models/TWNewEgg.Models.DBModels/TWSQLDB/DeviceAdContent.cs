using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160604
    /// 行動設備的廣告內文
    /// </summary>
    [Table("DeviceAdContent")]
    public class DeviceAdContentDB
    {
        /// <summary>
        /// 系統流水編號
        /// IDENTITY(2000,1) NOT NULL
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 廣告位置
        /// 是屬於哪一個行動設備的廣告設定, DeviceAdSet.ID
        /// 若有值CategoryID會為0
        /// </summary>
        //[Key]
        [DisplayName("行動設備廣告設定的系統流水編號, DeviceAdSet.ID")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("DeviceAdSetID")]
        //[ForeignKey("DeviceAdSet")]
        public int DeviceAdSetID { get; set; }

        /// <summary>
        /// 名稱, 輪播時叫:標題．生活提案時叫:大標題．美國直購時叫:說明．
        /// 促案時叫:文字內容．全館分類叫:館別名稱．
        /// </summary>
        [Required]
        [DisplayName("名稱, 輪播時叫:標題．生活提案時叫:大標題．美國直購時叫:說明．促案時叫:文字內容．全館分類叫:館別名稱．")]
        //[MaxLength(50)]
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 名稱2, 生活提案時叫:小標題
        /// </summary>
        [Required]
        [DisplayName("名稱2, 生活提案時叫:小標題")]
        //[MaxLength(50)]
        [Column("Name2")]
        public string Name2 { get; set; }


        /// <summary>
        /// 開始時間
        /// </summary>
        [DisplayName("開始時間")]
        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        [DisplayName("結束時間")]
        [Column("EndDate")]
        public DateTime EndDate { get; set; }


        /// <summary>
        /// del=已刪除了
        /// ---------------------------------------------add by bruce 20160623
        /// 若CategoryID有值這裡代表是屬於這個CategoryID的index
        /// </summary>
        [DisplayName("del=已刪除了, 若CategoryID有值這裡代表是屬於這個CategoryID的index")]
        [Column("Flag")]
        public string Flag { get; set; }

        /// <summary>
        /// 顯示順序, 依此內容排序
        /// </summary>
        [DisplayName("顯示順序, 依此內容排序")]
        [Column("Showorder")]
        public int Showorder { get; set; }

        /// <summary>
        /// 是否顯示, 顯示:show|1, 不顯示:hide|0
        /// </summary>
        [DisplayName("是否顯示, 顯示:show|1, 不顯示:hide|0")]
        [Column("ShowAll")]
        public string ShowAll { get; set; }

        /// <summary>
        /// 要點選的連結
        /// </summary>
        [DisplayName("要點選的連結")]
        [Column("Clickpath")]
        public string Clickpath { get; set; }

        /// <summary>
        /// 圖片位置
        /// </summary>
        [DisplayName("圖片位置")]
        [Column("ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// ---------------------------------------------add by bruce 20160623
        /// 來自SubCategory_NormalStore與Category的ID
        /// 若有值DeviceAdSetID會為0
        /// </summary>
        [DisplayName("來自SubCategory_NormalStore與Category的ID")]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        /// <summary>
        /// 創建者
        /// </summary>
        [DisplayName("創建者")]
        [Column("CreateUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        [DisplayName("創建日期")]
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        [DisplayName("最後修改者")]
        [Column("UpdateUser")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        [DisplayName("最後修改日期")]
        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }

    }
}
