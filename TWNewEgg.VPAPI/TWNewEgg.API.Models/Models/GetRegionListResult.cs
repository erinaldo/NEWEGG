using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TWNewEgg.API.Models
{
    public class GetRegionListResult
    {
        /// <summary>
        /// 地區ID
        /// </summary>
        public string CountryCode { set; get; }

        /// <summary>
        /// 地區名稱
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 地區中文名
        /// </summary>
        public string NameTW { set; get; }
    }
}