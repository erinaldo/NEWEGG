using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("warehouse")]
    public class Warehouse
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 倉別代碼
        /// Ex.8801、8802
        /// </summary>
        public string WarehouseID { get; set; }
        /// <summary>
        /// 倉庫名
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 地區名
        /// </summary>
        public string WMSWarehouseLocationName { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Dist { get; set; }
        public string PhoneRegion { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
    }
}
