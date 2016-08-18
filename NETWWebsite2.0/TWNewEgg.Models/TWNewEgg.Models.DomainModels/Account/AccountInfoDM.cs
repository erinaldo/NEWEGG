using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Account
{
    public class AccountInfoDM
    {
        public AccountInfoDM() {
            this.IsSuccess = true;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public AccountDM ADM { get; set; }
        public MemberDM MDM { get; set; }
    }
}
