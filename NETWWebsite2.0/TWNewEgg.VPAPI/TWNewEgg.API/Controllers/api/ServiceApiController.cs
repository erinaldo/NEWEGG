using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.ServiceApi.Models;

namespace TWNewEgg.API.Controllers.api
{
    public class ServiceApiController : ApiController
    {
        [HttpPost]
        public ResponseMessage Process(RequestMessage requestMessage)
        {
            var response = Processor.Process(requestMessage);
            return response;
        }
    }
}
