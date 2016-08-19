using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("LotteryAward")]
    public class LotteryAward
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; }
        public string Description { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int LotteryID { get; set; }
        public string Name { get; set; }
        public int Probability { get; set; }
        public int ShowOrder { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public int? Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
