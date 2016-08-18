using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemSketch")]
    public class ItemSketch
    {
        public ItemSketch()
        {
            DateTime defaultDate = DateTime.Now;
            DateStart = defaultDate;
            DateEnd = defaultDate.AddYears(10);
            DateDel = DateEnd.Value.AddDays(1);
            CreateDate = DateTime.Now;

            Tax = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> ItemTempGroupID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> ProducttempID { get; set; }
        public Nullable<int> itemtempID { get; set; }
        public string SourceTable { get; set; }
        public string SellerProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SPEC { get; set; }
        public Nullable<int> ManufactureID { get; set; }
        public string Model { get; set; }
        public string BarCode { get; set; }
        public Nullable<int> SellerID { get; set; }
        public Nullable<int> DelvType { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<int> PicEnd { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> TradeTax { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<int> Warranty { get; set; }
        public string UPC { get; set; }
        public string Note { get; set; }
        public string IsMarket { get; set; }
        public string Is18 { get; set; }
        public string IsShipDanger { get; set; }
        public string IsChokingDanger { get; set; }
        public string MenufacturePartNum { get; set; }
        public string Sdesc { get; set; }
        public string SpecDetail { get; set; }
        public string Spechead { get; set; }
        public string DelvDate { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<System.DateTime> DateDel { get; set; }
        public Nullable<decimal> PriceCard { get; set; }
        public Nullable<decimal> PriceCash { get; set; }
        public Nullable<decimal> ServicePrice { get; set; }
        public Nullable<decimal> PriceLocalship { get; set; }
        public Nullable<decimal> PriceGlobalship { get; set; }
        public Nullable<int> ItemQty { get; set; }
        public Nullable<int> ItemQtyReg { get; set; }
        public Nullable<int> ItemSafeQty { get; set; }
        public Nullable<int> QtyLimit { get; set; }
        public Nullable<int> ShowOrder { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public string ShipType { get; set; }
        public string ItemPackage { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string IsNew { get; set; }
        public Nullable<int> InventoryQty { get; set; }
        public Nullable<int> InventoryQtyReg { get; set; }
        public Nullable<int> InventorySafeQty { get; set; }
        public Nullable<decimal> GrossMargin { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// Discard4 
        /// Y:全新
        /// N:二手
        /// </summary>
        public string Discard4 { get; set; }
    }
}
