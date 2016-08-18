using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Track
{
    /// <summary>
    /// cookies key is "sc"
    /// </summary>
    public class CartTrack
    {
        /// <summary>
        /// item id
        /// </summary>
        public int iid { get; set; }
        /// <summary>
        /// item qty
        /// </summary>
        public int qty { get; set; }
        /// <summary>
        /// item status
        /// </summary>
        public int stu { get; set; }
        /// <summary>
        /// category id
        /// </summary>
        public int? cid { get; set; }
        /// <summary>
        /// category type
        /// </summary>
        public int? cty { get; set; }
        /// <summary>
        /// coupon id
        /// </summary>
        public string cpd { get; set; }
    }
}
