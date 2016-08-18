using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_ProductDetail")]
    public class Seller_ProductDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }       //2014.4.18 add SellerID by ice
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string NameTW { get; set; }
        public string UPC { get; set; }

        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SellerProductID { get; set; }
        public int ManufactureID { get; set; }
        public string ManufacturePartNum { get; set; }
        public int Condition { get; set; }
        public string ShipType { get; set; }
        public int Qty { get; set; }
        public int QtyReg { get; set; }
        public int SafeQty { get; set; }
        public string Status { get; set; }      //2014.4.23 審核狀態 add by ice
        public Nullable<int> InUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
