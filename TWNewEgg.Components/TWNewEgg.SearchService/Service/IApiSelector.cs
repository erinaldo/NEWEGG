using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.SearchService.Models;

namespace TWNewEgg.SearchService.Service
{
    public interface IApiSelector
    {
        //bool FindApiName(string apiName);
        SearchApiModel SetSearchApiModel(string apiArgs);
        DealApiModel SetDealApiModel(string apiArgs);
        string ReturnURLWithApi(string apiName, string apiArgs);
    }
}
