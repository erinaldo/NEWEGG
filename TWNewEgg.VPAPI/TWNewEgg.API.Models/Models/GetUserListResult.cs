using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class GetUserListResult
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Purview 類型
        /// </summary>
        public string PurviewType { set; get; }

        /// <summary>
        /// User Status
        /// </summary>
        public string Status { set; get; }

        /// <summary>
        /// GroupID
        /// </summary>
        public int GroupID { set; get; }
    }
}
