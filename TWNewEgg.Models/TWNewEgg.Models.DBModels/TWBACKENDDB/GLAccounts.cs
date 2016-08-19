using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("GLAccounts")]
    public class GLAccounts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string AccNumber { get; set; }
        public string AccDescription { get; set; }
        public string UseFlag { get; set; }
    }
}
