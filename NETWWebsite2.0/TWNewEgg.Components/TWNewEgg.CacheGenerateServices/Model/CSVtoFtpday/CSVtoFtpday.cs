using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace TWNewEgg.CacheGenerateServices.Model.CSVtoFtpday
{
    public class CSVtoFtpday
    {
        /// <summary>
        /// 取得 Item UpdateDate 的時間區間
        /// </summary>
        public string GetItemTime { get; set; }

        /// <summary>
        /// 要取 N 筆 Item
        /// </summary>
        public string NumPerCount { get; set; }

        /// <summary>
        /// 設定 Before 幾小時
        /// </summary>
        public string getItemHours { get; set; }

        /// <summary>
        /// 設定是否分檔案
        /// </summary>
        public string cutPage { get; set; }
    }
}
