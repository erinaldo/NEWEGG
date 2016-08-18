using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class ExceptionService
    {
        public string InnerExceptionMessage(Exception error)
        {
            string returnResult = error.InnerException != null ? error.InnerException.Message : string.Empty;
            return returnResult;
        }
    }
}
