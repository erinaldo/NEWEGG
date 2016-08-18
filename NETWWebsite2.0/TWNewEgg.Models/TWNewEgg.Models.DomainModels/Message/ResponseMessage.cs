using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Message
{
    public class ResponseMessage<T>
    {
        public ResponseMessage()
        {
            this.Error = new ErrorMessage();
        }
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ErrorMessage Error { get; set; }
    }
}
