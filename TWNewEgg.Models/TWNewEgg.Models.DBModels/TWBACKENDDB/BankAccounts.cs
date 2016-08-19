using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    public class BankAccounts
    {
        [Key]
        public int BankID { get; set; }
        [Key]
        public string AccNumber { get; set; }
    }
}
