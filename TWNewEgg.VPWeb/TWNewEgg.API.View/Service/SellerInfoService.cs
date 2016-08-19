using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.View.Service
{
    /// <summary>
    /// 取得登入者 cookie 內資料
    /// </summary>
    public class SellerInfoService
    {
        private int localSellerID;

        /// <summary>
        /// SellerID
        /// </summary>
        /// <value>SellerID in Seller_User </value>
        public int SellerID 
        {
            get
            {
                return localSellerID;
            }
        }

        private int localGroupID;

        /// <summary>
        /// GroupID
        /// </summary>
        /// <value>Group in Seller_User</value>
        public int GroupID
        {
            get
            {
                return localGroupID;
            }
        }

        private int localUserID;

        /// <summary>
        /// UserID
        /// </summary>
        /// <value>UserID in Seller_User</value>
        public int UserID
        {
            get
            {
                return localUserID;
            }
        }

        private string localAccessToken;

        /// <summary>
        /// 存取權限
        /// </summary>
        /// <value>Seller_User.AccessToken</value>
        public string AccessToken
        {
            get
            {
                return localAccessToken;
            }
        }

        private string localAccountTypeCode;

        /// <summary>
        /// Seller or Vendor
        /// </summary>
        /// <value>S: Seller, V: Vendor</value>
        public string AccountTypeCode
        {
            get
            {
                return localAccountTypeCode;
            }
        }

        private string localUserEmail;

        /// <summary>
        /// Login UserEmail
        /// </summary>
        /// <value>UserEmail</value>
        public string UserEmail
        {
            get
            {
                return localUserEmail;
            }
        }

        private int localcurrentSellerID;

        /// <summary>
        /// 登入後使用的sellerID
        /// </summary>
        /// <value>目前選取的 SellerID </value>
        public int currentSellerID
        {
            get
            {
                return localcurrentSellerID;
            }
        }

        private bool localisAdmin;

        /// <summary>
        /// 判斷是否為管理者
        /// </summary>
        /// <value>由GroupID判斷是否為管理者身分，可選取商家 </value>
        public bool IsAdmin
        {
            get
            {
                return localisAdmin;
            }
        }

        private string localLoginAccountType;

        /// <summary>
        /// 登入者的AccountType
        /// </summary>
        /// <value>登入者的AccountType</value>
        public string LoginAccountType
        {
            get
            {
                return localLoginAccountType;
            }
        }

        private string localLoginSellerStatus;

        /// <summary>
        /// 登入者的商家權限
        /// </summary>
        /// <value>判斷商家權限(登入、上下架商品)，Active or Inactive(A or I) </value>
        public string LoginSellerStatus
        {
            get
            {
                return localLoginSellerStatus;
            }
        }

        private string localSellerStatus;

        /// <summary>
        /// 判斷Admin選取的Seller商家權限(登入、上下架商品)
        /// </summary>
        /// <value>判斷商家權限(登入、上下架商品)，Active or Inactive(A or I) </value>
        public string SellerStatus
        {
            get
            {
                return localSellerStatus;
            }
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <value>SellerInfo 建構子</value>
        public SellerInfoService()
        {
            int changeInt = 0;

            if (int.TryParse(AesDecrypt("UD"), out changeInt))
            {
                localUserID = changeInt;
            }

            localUserID = string.IsNullOrEmpty(AesDecrypt("UD")) && int.TryParse(AesDecrypt("UD"), out changeInt) 
                ? 0 : Convert.ToInt32(AesDecrypt("UD"));

            localSellerID = string.IsNullOrEmpty(AesDecrypt("SD")) && int.TryParse(AesDecrypt("SD"), out changeInt) 
                ? 0 : Convert.ToInt32(AesDecrypt("SD"));

            if (System.Web.HttpContext.Current.Request.Cookies["AT"] != null)
            {
                localAccessToken = System.Web.HttpContext.Current.Request.Cookies["AT"].Value;
            }

            if (System.Web.HttpContext.Current.Request.Cookies["LATC"] != null)
            {
                localLoginAccountType = AesDecrypt("LATC");
            }

            if (System.Web.HttpContext.Current.Request.Cookies["LSS"] != null)
            {
                localLoginSellerStatus = AesDecrypt("LSS");
            }

            if (System.Web.HttpContext.Current.Request.Cookies["SS"] != null)
            {
                localSellerStatus = AesDecrypt("SS");
            }
            
            localAccountTypeCode = string.IsNullOrEmpty(AesDecrypt("ATC"))
                ? string.Empty : AesDecrypt("ATC");

            localUserEmail = string.IsNullOrEmpty(AesDecrypt("UEM"))  
                ? string.Empty : AesDecrypt("UEM");

            localGroupID = string.IsNullOrEmpty(AesDecrypt("GD")) && int.TryParse(AesDecrypt("GD"), out changeInt) 
                ? 0 : Convert.ToInt32(AesDecrypt("GD"));

            localcurrentSellerID = string.IsNullOrEmpty(AesDecrypt("CSD")) && int.TryParse(AesDecrypt("CSD"), out changeInt) 
                ? 0 : Convert.ToInt32(AesDecrypt("CSD"));

            //判斷是否管理者
            //目前採取
            if (localGroupID == (int)User_Group.Internal_User || localGroupID == (int)User_Group.PM)
            {
                localisAdmin = true; 
            }
            else
            {
                localisAdmin = false;
            }
        }

        AES aesservice = new AES();
            
        private string AesDecrypt(string key)
        {
            if (System.Web.HttpContext.Current.Request.Cookies[key] != null)
            {
                return aesservice.AesDecrypt(System.Web.HttpContext.Current.Request.Cookies[key].Value);
            }

            return null;
        }

        private enum User_Group
        {
            //商家
            Seller_Owner = 1,
            //一般User
            Normal_User = 2,
            //管理者
            Internal_User = 3,
            //倉儲
            CM = 4,
            //新蛋 PM
            PM = 5,
            //由製造商新增
            ManufatureAdd = 6,
            //是Seller & Vendor 雙重身分
            SellerandVendor = 7,
            //供應商
            Vendor = 8
        }
    }
}
