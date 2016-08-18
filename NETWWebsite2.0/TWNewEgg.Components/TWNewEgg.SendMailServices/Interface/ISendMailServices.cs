using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.SendMail;

namespace TWNewEgg.SendMailServices.Interface
{
    public interface ISendMailServices
    {
        bool SendATMPaymentSuccessfulMail(SendMailDM SendMailDM);
        bool SendMail(SendMailDM SendMailDM);
    }
}
