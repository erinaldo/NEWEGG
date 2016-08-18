using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("PromoActive")]
    public class PromoActive
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryLink { get; set; }
        public int FuncType { get; set; }
        public string Name { get; set; }
        public string NameLink { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int TakeType { get; set; }
        public System.DateTime? DeclareDate { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public enum Types
        {
            滿額折_現折 = 1,
            折價券 = 2,
            回饋金 = 3,
            紅利點數 = 4,
            抽獎 = 5,
            贈獎_獎品 = 6,
            銀行 = 7,
            折扣 = 8,
        }
        public enum TakeTypes
        {
            親領 = 1,
            寄送 = 2,
            歸戶 = 3,
            
        }
    }
}
