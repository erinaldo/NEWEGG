using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.Services.Account
{
    public class AccountAuthFactory
    {
        public enum AuthType
        {
            ecweb,
            facebook
        }
        public IAccountAuth SwitchAuthService(string type)
        {
            AuthType authType = (AuthType)Enum.Parse(typeof(AuthType), type);
            if (!Enum.IsDefined(typeof(AuthType), authType))
            {
                return new ECAccountAuth();
            }
            switch (authType)
            {
                case AuthType.ecweb:
                    return new ECAccountAuth();
                case AuthType.facebook:
                    return new FBAccountAuth();
                default:
                    return new ECAccountAuth();
            };
        }
    }
}