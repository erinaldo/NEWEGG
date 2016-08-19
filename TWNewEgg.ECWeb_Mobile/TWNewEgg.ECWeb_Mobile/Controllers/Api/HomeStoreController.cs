using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Home;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class HomeStoreController : ApiController
    {
        // GET api/homestore
        public List<HomeShopWindow> Get([FromUri]List<int> index)
        {
            List<HomeShopWindow> result = new List<HomeShopWindow>();
            var results = Processor.Request<List<HomeShopWindow>, List<HomeShopWindow>>("HomeStoreService", "GetHomeShopWindows", index);
            if (string.IsNullOrEmpty(results.error))
            {
                result = results.results;
            }
            return result;
        }

        // POST api/homestore
        public void Post([FromBody]string value)
        {
        }

        // PUT api/homestore/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/homestore/5
        public void Delete(int id)
        {
        }
    }
}
