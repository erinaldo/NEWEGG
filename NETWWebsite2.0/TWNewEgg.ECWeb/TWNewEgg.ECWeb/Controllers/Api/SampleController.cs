using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    //[AllowNonSecures]
    //[AllowAnonymous]
    public class SampleController : ApiController
    {
        public string Get()
        {
            var ttt = User.Identity.IsAuthenticated;
            var tttt = NEUser.Email;
            return string.Empty;
        }
        //public List<string> Post(List<string> itemNumber)
        //{
        //    List<string> results = new List<string>();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        results.Add(i.ToString());
        //    }
        //    return results;
        //}
        public List<string> Post([FromBody]SampleRequest itemNumber)
        {
            List<string> results = new List<string>();
            for (int i = 100; i < 115; i++)
            {
                results.Add(i.ToString());
            }
            return results;
        }

        public List<string> Put([FromBody]SampleRequest itemNumber)
        {
            List<string> results = new List<string>();
            for (int i = 100; i > 80; i--)
            {
                results.Add(i.ToString());
            }
            return results;
        }

        public List<string> Delete([FromBody]int itemNumber)
        {
            List<string> results = new List<string>();
            for (int i = 10; i > 0; i--)
            {
                results.Add(i.ToString());
            }
            return results;
        }
    }
    public class SampleRequest
    {
        public List<string> itemNumber { get; set; }
        public List<int> itemQty { get; set; }
    }
}
