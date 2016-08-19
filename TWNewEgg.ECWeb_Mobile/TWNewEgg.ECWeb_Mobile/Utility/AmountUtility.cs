using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb_Mobile.Utility
{
    public static class AmountUtility
    {
        private const string commaFormat = "###,###,###,###,###,###,###,###";

        public static string AddCommas(this decimal me, string formatter = commaFormat)
        {
            if (me <= 0)
            {
                return string.Empty;
            }

            return string.Format("${0:" + commaFormat + "}", me); ;
        }


        public static string AddCommas(string text, string formatter = commaFormat)
        {
            decimal me = 0;
            if (decimal.TryParse(text, out me))
            {
                return me.AddCommas();
            }

            return text;
        }
    }
}