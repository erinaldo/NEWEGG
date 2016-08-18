using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.Auth;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class AuchkController : ApiController
    {
        // GET api/auchk
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

        //// GET api/auchk/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/auchk
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/auchk/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/auchk/5
        //public void Delete(int id)
        //{
        //}
    }
}
