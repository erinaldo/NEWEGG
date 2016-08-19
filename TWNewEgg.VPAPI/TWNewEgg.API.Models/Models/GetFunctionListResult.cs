using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class GetFunctionListResult
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
    }
}
