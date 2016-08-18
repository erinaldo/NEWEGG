using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service.BlackList
{
    public class BlackList
    {
        public bool MobileBlackList(string _strPhoneNumber)
        {

            string _strBlackList = (System.Configuration.ConfigurationManager.AppSettings["PayMoneyPhoneNumber"]).ToString();
            string[] _strArrayBlackList = _strBlackList.Split(',');
            List<string> _listBlackList = new List<string>();
            for (int i = 0; i < _strArrayBlackList.Length; i++)
            {
                _listBlackList.Add(_strArrayBlackList[i].Trim());
            }
            bool isError = _listBlackList.Contains(_strPhoneNumber.Substring(0, 4));
            if (isError)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}