using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    /// <summary>
    /// 癈四機回收四聯單
    /// </summary>
    [Table("Discard4Item")]
    public class Discard4ItemDB
    {

        /// <summary>
        /// 系統流水編號
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }


        /// <summary>
        /// 購車商品編號
        /// </summary>
        [DisplayName("購車商品編號")]
        [Column("SalesorderCode")]
        public string SalesorderCode { get; set; }

        /// <summary>
        /// 購車數量編號
        /// </summary>
        [DisplayName("購車數量編號")]
        [Column("SalesorderitemCode")]
        public string SalesorderitemCode { get; set; }

        /// <summary>
        /// 賣場品編號
        /// </summary>
        [DisplayName("賣場品編號")]
        [Column("ItemID")]
        public int ItemID { get; set; }

         

        /// <summary>
        /// 安裝日期
        /// </summary>
        [DisplayName("安裝日期")]
        [Column("InstalledDate")]
        public DateTime? InstalledDate { get; set; }

        /// <summary>
        /// 回收四聯單號
        /// </summary>
        [DisplayName("回收四聯單號")]
        [Column("NumberCode")]
        public string NumberCode { get; set; }

        /// <summary>
        /// 回收狀態
        /// ''=未處理, Y=回收, N=不回收
        /// </summary>
        [DisplayName("回收狀態")]
        [Column("Discard4Flag")]
        public string Discard4Flag { get; set; }


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
