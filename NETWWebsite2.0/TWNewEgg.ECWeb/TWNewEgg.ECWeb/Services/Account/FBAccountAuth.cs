using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb.Services.Account
{
    public class FBAccountAuth : IAccountAuth
    {
        public AccountVM CheckAuth(Login model)
        {
           
            throw new NotImplementedException();
        }

        public AccountVM CheckGuestAuth(Login model)
        {
            throw new NotImplementedException();
        }

        public PasswordError CheckPassword(RegisterVM RegisterVM)
        {
            throw new NotImplementedException();
        }

        public bool CheckAgreePaper(RegisterVM RegisterVM)
        {
            throw new NotImplementedException();
        }
    }
}