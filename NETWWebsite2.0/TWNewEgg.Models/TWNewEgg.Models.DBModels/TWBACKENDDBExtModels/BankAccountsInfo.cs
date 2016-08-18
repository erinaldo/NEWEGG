using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class BankAccountsInfo
    {
        public Bank Bank { get; set; }
        public GLAccounts Accounts { get; set; }
    }
}
