using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("BankCodeMessage")]
    public class BankCodeMessage
    {
        public enum TradeMethodOption
        {
            CredicCard = 1,
            WebATM = 2
        }
        public BankCodeMessage()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key][Column(Order=0)]
        public string BankCode { get; set; }
        [Key]
        [Column(Order = 1)]
        public int TradeMethod { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MsgCode { get; set; }
        public string MsgDescription { get; set; }
    }//end class
}//end namespace
