using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160604
    /// 行動設備的廣告設定
    /// </summary>
    [Table("DeviceAdSet")]
    public class DeviceAdSetDB
    {
        /// <summary>
        /// 系統流水編號
        /// IDENTITY(1000,1) NOT NULL
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [DisplayName("名稱")]
        [Column("Name")]
        //[MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 子層時是:標題文字
        /// 有父層時才會有標題文字
        /// </summary>
        [DisplayName("子層時是:標題文字")]
        [Column("SubName")]
        //[MaxLength(50)]
        public string SubName { get; set; }


        /// <summary>
        /// 值是NULL=沒有父親, 有值時父親是ID
        /// </summary>
        [DisplayName("值是NULL=沒有父親, 有值時父親是ID")]
        [Column("Parent")]
        public int Parent { get; set; }


        /// <summary>
        /// phone=手機, pad=平版, pc=桌機
        /// </summary>
        [DisplayName("phone=手機, pad=平版, pc=桌機")]
        [Column("Flag")]
        //[MaxLength(50)]
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
