using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.ECWeb_Mobile.Auth;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class AuchkController : ApiController
    {
#if DEBUG
        [AllowNonSecures]
#endif
        // GET api/auchk
        [HttpGet]
        public bool Get()
        {
            if (!NEUser.IsAuthticated)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
