using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PublicApiModels
{
    public enum CodeStatue
    {
        Success = 0,
        Account_is_invalid = 1,
        Account_is_not_registered = 2,
        IP_is_invalid = 3,
        AuthToken_error = 4,
        AuthToken_is_Expired = 5,
        Datas_error = 6,
        No_datas = 7,
        System_Error = 99,
    }
    public class TokenResponseModel
    {
        public string codeMessage { get; set; }
        public string codeNumber { get; set; }
        public string infoMessage { get; set; }
        public string token { get; set; }
    }
}
