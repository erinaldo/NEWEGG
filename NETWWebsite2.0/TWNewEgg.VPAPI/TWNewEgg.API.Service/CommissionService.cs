using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class CommissionService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }
        private DB.TWSqlDBContext db = new DB.TWSqlDBContext();
        public Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.Category>> GetCategory()
        {
            Models.ActionResponse<List<TWNewEgg.DB.TWSQLDB.Models.Category>> result = new Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>();

            try
            {
                result.Code = (int)ResponseCode.Success;
                result.Body = db.Category.Where(x => x.Layer == 0).ToList();
                result.IsSuccess = true;
                result.Msg = "Success";
            }
            catch (Exception ex)
            {                
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = ex.Message ?? "" + ((ex.InnerException != null) ? ex.InnerException.Message : "");
            }

            return result;
        }
    }
}
