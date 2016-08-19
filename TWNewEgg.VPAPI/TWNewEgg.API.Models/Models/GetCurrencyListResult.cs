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
    public class GetCurrencyListResult
    {
        /// <summary>
        /// 幣別ID
        /// </summary>
        public string CurrencyCode { set; get; }

        /// <summary>
        /// 幣別名稱
        /// </summary>
        public string Name { set; get; }
    }
}