using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public enum CodeStatue
    {
        Success = 00,
        Account_Error = 01,
        IP_Error = 02,
        Token_Expired = 03,
        Token_Error = 04,
        Data_Error = 05,
        System_Error = 99

    }
    public class AuthTokenCheck
    {
        public bool isCurrent { get; set; }
        public string message { get; set; }
    }
}
