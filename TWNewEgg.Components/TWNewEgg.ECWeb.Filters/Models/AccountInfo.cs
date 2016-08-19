using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Models
{
    public class AccountInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string Scopes { get; set; }
        public string IPAddress { get; set; }
        public string Browser { get; set; }
        public DateTime Loginon { get; set; }
    }
}
