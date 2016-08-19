using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PublicApiModels
{
    public class ActionResponse<T>
    {
        public ActionResponse()
        {
            Body = default(T);
            Code = string.Empty;
            IsSuccess = false;
            Msg = string.Empty;
        }
        public T Body { get; set; }
        public string Code { get; set; }
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
    }
}
