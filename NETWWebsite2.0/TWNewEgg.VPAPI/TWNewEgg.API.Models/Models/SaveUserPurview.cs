using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SaveUserPurview
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 權限List
        /// </summary>
        public List<PurviewListInfo> PurviewList { set; get; }

        public class PurviewListInfo
        {
            /// <summary>
            /// 功能ID
            /// </summary>
            public int FunctionID { set; get; }

            /// <summary>
            /// 啟用狀態
            /// </summary>
            public string Enable { set; get; }
        }

        /// <summary>
        /// 建立/修改者UserID
        /// </summary>
        public int UpdateUserID { get; set; }
    }
}
