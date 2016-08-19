using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class GetPurviewResult
    {
        /// <summary>
        /// 類別ID
        /// </summary>
        public int CategotyID { set; get; }

        /// <summary>
        /// 類別名稱
        /// </summary>
        public string CategotyName { set; get; }

        /// <summary>
        /// Function ID
        /// </summary>
        public int FunctionID { get; set; }

        /// <summary>
        /// Function 名稱
        /// </summary>
        public string FunctionName { set; get; }

        /// <summary>
        /// 啟用狀態
        /// </summary>
        public string Enable { set; get; }

        /// <summary>
        /// Purview 類型
        /// </summary>
        public string PurviewType { set; get; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserID { set; get; }

        /// <summary>
        /// User Email
        /// </summary>
        public string UserEmail { set; get; }
    }
}
