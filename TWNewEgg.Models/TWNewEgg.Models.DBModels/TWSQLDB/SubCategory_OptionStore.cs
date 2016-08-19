using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("SubCategory_OptionStore")]
    public class SubCategory_OptionStore
    {
        public enum ConstShowAll
        {
            Hide = 0,
            Show = 1
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Title { get; set; }
        public Nullable<bool> IsFormat { get; set; }
        public Nullable<decimal> FreeCost { get; set; }
        public Nullable<int> SellerID { get; set; }
        // ConstShowAll
        public Nullable<int> ShowAll { get; set; }
        public Nullable<int> Showorder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }

        /// <summary>
        /// 是否為任選館
        /// </summary>
        public int IsSpecOption { get; set; }
    }
}
