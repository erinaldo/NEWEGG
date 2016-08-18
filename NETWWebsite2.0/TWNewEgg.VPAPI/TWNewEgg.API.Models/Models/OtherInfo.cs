using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Other Info (APIModel)
    /// <para>Website Page:Create Items</para>
    /// </summary>
    public class OtherInfo
    {
        /// <summary>
        /// upload picture start quantity
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public int PicStart { get; set; }

        /// <summary>
        /// upload picture final quantity
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public int PicEnd { get; set; }

        /// <summary>
        /// Manufacturer's name
        /// <para>DB Form:TWSQLDB.dbo.manufacture</para>
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Manufacturer's ID
        /// <para>DB Form:???</para>
        /// </summary>
        /// <remarks>
        /// It is a fake data.
        /// </remarks>
        public string ManufactureItemID { get; set; }

        /// <summary>
        /// Seller's commission
        /// <para>DB Form:TWSELLERPORTALDB.dbo.Seller_Charge</para>
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// Item's English name
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Item's TW name
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public string NameTW { get; set; }

        /// <summary>
        /// Item's status
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Seller item's ID
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// Item's Internet Price
        /// <para>DB Form:TWSQLDB.dbo.item</para>
        /// </summary>
        /// <remarks>
        /// Not sure it is the right data.
        /// </remarks>
        public decimal PriceCash { get; set; }

        /// <summary>
        /// Item's Market Price
        /// <para>DB Form:???</para>
        /// <remarks>
        /// It is a fake data.
        /// </remarks>
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// Item's Inventory
        /// <para>DB Form:???</para>
        /// <remarks>
        /// It is a fake data.
        /// </remarks>
        /// </summary>
        public int Inventory { get; set; }

        /// <summary>
        /// Item's Price ship
        /// <para>DB Form:???</para>
        /// <remarks>
        /// It is a fake data.
        /// </remarks>
        /// </summary>
        public decimal Priceship { get; set; }
    }
    /*---------- end by thisway ----------*/
}
