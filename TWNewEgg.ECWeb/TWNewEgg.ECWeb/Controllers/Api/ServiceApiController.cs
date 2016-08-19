using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.ServiceApi.Models;

namespace TWNewEgg.ECService.Controllers.api
{
    public class ServiceApiController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [AllowNonSecures]
        public ResponseMessage Process(RequestMessage requestMessage)
        {
            var response = Processor.Process(requestMessage);
            return response;
        }
    }
}
