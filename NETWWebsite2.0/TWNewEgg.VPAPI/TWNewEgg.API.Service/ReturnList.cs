using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class ReturnList
    {
        public Models.ActionResponse<List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>> Getretgood()
        {
            TWNewEgg.DB.TWBackendDBContext db = new DB.TWBackendDBContext();
            TWNewEgg.API.Models.ActionResponse<List<DB.TWBACKENDDB.Models.Retgood>> res = new API.Models.ActionResponse<List<DB.TWBACKENDDB.Models.Retgood>>();
            res.Body = db.Retgood.ToList();
            return res;
        }
        
    }
}
