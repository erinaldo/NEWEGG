using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Newtonsoft.Json;

namespace TWNewEgg.ECWeb.PrivilegeFilters
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string username)
        {
            this.Identity = new CustomIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated;
        }

    }
    public class CustomIdentity : IIdentity
    {
        private string _AccEmail;
        private string _Type;
        private string _AccName;
        private string _AccNickName;
        private DateTime _AccLoginTime;
        private int _AccID;
        private string _AccIPAddress;
        private string _AccBrowser;
        private string _AccScopes;
        private void insertData(Models.AccountInfo accountInfo)
        {
            _AccName = accountInfo.Name;
            _AccEmail = accountInfo.Email;
            _AccNickName = accountInfo.NickName;
            _AccID = accountInfo.ID;
            _AccScopes = accountInfo.Scopes;
            _AccLoginTime = accountInfo.Loginon;
            _AccIPAddress = accountInfo.IPAddress;
            _AccBrowser = accountInfo.Browser;
        }
        public CustomIdentity(string accInfo)
        {
            if (string.IsNullOrEmpty(accInfo))
            {
                throw new ArgumentException("No Account Information.");
            }
            Models.AccountInfo accountInfo = JsonConvert.DeserializeObject<Models.AccountInfo>(accInfo);
            if (accountInfo == null && string.IsNullOrEmpty(accountInfo.Email))
            {
                throw new ArgumentException("No Account Information.");
            }

            insertData(accountInfo);
            _Type = string.Empty;
        }
        public CustomIdentity(string accInfo, string type)
        {
            if (string.IsNullOrEmpty(accInfo))
            {
                throw new ArgumentException("No Account Information.");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("No Account Information Type.");
            }
            Models.AccountInfo accountInfo = JsonConvert.DeserializeObject<Models.AccountInfo>(accInfo);
            if (accountInfo == null && string.IsNullOrEmpty(accountInfo.Email))
            {
                throw new ArgumentException("No Account Information.");
            }

            insertData(accountInfo);
            _Type = type;
        }
        public string AuthenticationType
        {
            get { return _Type; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(_AccEmail); }
        }

        public string Name
        {
            get { return _AccName; }
        }
        public string NickName
        {
            get { return _AccNickName; }
        }
        public string Email
        {
            get { return _AccEmail; }
        }
        public int ID
        {
            get { return _AccID; }
        }
        public string Scopes
        {
            get { return _AccScopes; }
        }
        public string IPAddress
        {
            get { return _AccIPAddress; }
        }
        public string Browser
        {
            get { return _AccBrowser; }
        }
        public DateTime LoginTime
        {
            get { return _AccLoginTime; }
        }
    }
}
