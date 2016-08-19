using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class PageInfo
    {
        /// <summary>
        /// Number of rows in one page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Current page index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Total page count
        /// </summary>
        public int TotalPage { get; set; }        
    }
}
