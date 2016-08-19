using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Models.ViewModels.Register;

namespace TWNewEgg.ECWeb_Mobile.Services.Account
{
    public interface IAccountAuth
    {
        AccountVM CheckAuth(Login model);
        PasswordError CheckPassword(RegisterVM RegisterVM);
        bool CheckAgreePaper(RegisterVM RegisterVM);
    }
}