using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 癈四機同意
    /// </summary>
    [Table("Discard4")]
    public class Discard4DB
    {

        /// <summary>
        /// 系統流水編號
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 購物車編號
        /// </summary>
        [DisplayName("購物車編號")]
        [Column("SalesOrderGroupID")]
        public int SalesOrderGroupID { get; set; }


        /// <summary>
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </summary>
        [DisplayName("同意癈四機回收")]
        [Column("AgreedDiscard4")]
        public string AgreedDiscard4 { get; set; }

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
