using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("AdditionalItemForCart")]
    public class AdditionalItemForCart
    {
        /// <summary>
        /// Related to Status
        /// </summary>
        public enum AdditionalItemStatus
        {
            Planning = -1,
            Disable = 0,
            Enable = 1
        };
        /// <summary>
        /// Related to Specific
        /// </summary>
        public enum SpecificStatus
        {
            AllAccount = 0,
            NeweggAccount = 1,
            VIPAccount = 2
        };
        /// <summary>
        /// Related to CartType
        /// </summary>
        public enum CartTypeStatus
        {
            全部 = -1,
            無定義 = 0,
            Domestic = 1,
            Internation = 2,
            ChooseAny = 3
        };
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("Item ID")]
        public int ItemID { get; set; }
        /// <summary>
        /// Related to AdditionalItemStatus
        /// </summary>
        [DisplayName("加價商品狀態")]
        public int Status { get; set; }
        [DisplayName("最低可加購金額")]
        public decimal LimitedPrice { get; set; }
        [DisplayName("規格品GroupID")]
        public Nullable<int> ItemGroupID { get; set; }
        /// <summary>
        /// Related to SpecificStatus
        /// </summary>
        [DisplayName("針對身分")]
        public int Specific { get; set; }
        [DisplayName("起始時間")]
        public DateTime StartDate { get; set; }
        [DisplayName("結束時間")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Related to CartTypeStatus
        /// </summary>
        [DisplayName("購物車型態")]
        public int CartType { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        /// <summary>
        /// 商品排序
        /// </summary>
        public int Sequence { get; set; }
    }
}
