using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.EDM;

namespace TWNewEgg.EDMServices.Interface
{
    public interface IEDMService
    {
        EDMBookDM GetLatestEDM();
    }
}
