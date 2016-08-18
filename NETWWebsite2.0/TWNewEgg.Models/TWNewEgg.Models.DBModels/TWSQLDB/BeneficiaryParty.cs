using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("BeneficiaryParty")]
    public class BeneficiaryParty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID{ get; set; }
        public string Name { get; set; }
        public string BankCode { get; set; }
        public Nullable<int> Status { get; set; }
    }
}
