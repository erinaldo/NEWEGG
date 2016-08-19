using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.ItemBatchServices.Models
{
    public class ActionResponse<T>
    {
        public ActionResponse()
        {
            this.IsSuccess = false;
            this.Code = 0;
            this.Msg = string.Empty;
            this.Body = default(T);
        }

        public void Finish(bool IsSuccess, int Code, string Msg, T Body)
        {
            this.IsSuccess = IsSuccess;
            this.Code = Code;
            this.Msg = Msg;
            this.Body = Body;
        }

        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public T Body { get; set; }


    }

    public enum ResponseCode
    {
        Success = 0,
        Error = 1,
        AccessError = 2,
        IPPUpdateFailed = 3,
    }
}
