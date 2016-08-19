using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ECWeb.PrivilegeFilters.Models;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Core
{
    public interface IEncapsulationAuthCookies
    {
        string Encapsulate(AccountInfo source, string path, string domain, DateTime? expires);
        string DeEncapsulate();
        void ClearAll(string domain);
    }
}
