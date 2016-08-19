using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.Utility
{
    public static class AmountUtility
    {
        private const string commaFormat = "###,###,###,###,###,###,###,###";

        /// <summary>
        /// Add commas and converts to dollar format.
        /// </summary>
        /// <param name="me">Current object.</param>
        /// <returns>Decimal value.</returns>
        public static string AddCommas(this decimal me, string formatter = commaFormat)
        {
            return string.Format("${0:" + commaFormat + "}", me); ;
        }
    }
}