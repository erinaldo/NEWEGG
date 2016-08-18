using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PublicApiModels;

namespace TWNewEgg.AuthTokenService.Interface
{
    public interface IAuthTokenService
    {
        TokenResponseModel getToken(string account, string fromIP);
    }
}
