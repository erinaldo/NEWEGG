using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Specific Info Result (APIModelResult)
    /// <para>Website Page:Create Items(Step3 APImodel)</para>
    /// </summary>
    public class SpecificInfoResult
    {
        /// <summary>
        /// Item's length
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Item's width
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Item's height
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Item's weight
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Item's sale type
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public int SaleType { get; set; }

        /// <summary>
        /// Item's Description
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item's TW Description
        /// <para>DB Form:TWSQLDB.dbo.product</para>
        /// </summary>
        public string DescriptionTW { get; set; }

        /// <summary>
        /// Item's note
        /// <para>DB Form:???</para>
        /// </summary>
        /// <remarks>
        /// It is a fake data(TWSQLDB.dbo.product).
        /// </remarks>
        public string Note { get; set; }

        /// <summary>
        /// Ship danger items
        /// <para>DB Form:???</para>
        /// </summary>
        /// <remarks>
        /// It is a fake data(TWSQLDB.dbo.product).
        /// </remarks>
        public char ShipDangerItems { get; set; }

        /// <summary>
        /// Buy the item need age Prohibit
        /// <para>DB Form:???</para>
        /// </summary>
        /// <remarks>
        /// It is a fake data(TWSQLDB.dbo.product).
        /// </remarks>
        public char ProhibitAge { get; set; }

        /// <summary>
        /// Choking Danger item
        /// <para>DB Form:???</para>
        /// </summary>
        /// <remarks>
        /// It is a fake data(TWSQLDB.dbo.product).
        /// </remarks>
        public char ChokingDanger { get; set; }
    }
    /*---------- end by thisway ----------*/
}
