using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("ItemGroupDetailProperty")]
    public class ItemGroupDetailProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GroupID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ItemTempID { get; set; }
        public int SellerID { get; set; }

        /// <summary>
        /// Master Property Id
        /// </summary>
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MasterPropertyID { get; set; }


        /// <summary>
        /// Second Property Id
        /// </summary>
        public int? PropertyID { get; set; }

        /// <summary>
        /// Master Property Value Id
        /// </summary>
        [Key, Column(Order = 2)]
        public int GroupValueID { get; set; }

        /// <summary>
        /// Second Property Value Id
        /// </summary>
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ValueID { get; set; }

        /// <summary>
        /// Secton Property Value for Display
        /// </summary>
        public string ValueName { get; set; }

        /// <summary>
        /// Master Property Value for Display
        /// </summary>
        public string InputValue { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}