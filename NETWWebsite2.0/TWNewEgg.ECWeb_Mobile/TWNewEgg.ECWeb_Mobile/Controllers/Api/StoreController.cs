using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class StoreController : ApiController
    {
        // GET api/store/5
        public StoreInfo Get(int? storeId)
        {
            StoreInfo storeInfo = new StoreInfo();
            storeInfo = TWNewEgg.Framework.ServiceApi.Processor.Request<StoreInfo, StoreInfo>("StoreService", "GetStoreInfo", storeId, new List<int>(new int[] { 10 })).results;
            return storeInfo;
        }

        // POST api/store
        public void Post([FromBody]string value)
        {
        }

        // PUT api/store/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/store/5
        public void Delete(int id)
        {
        }
    }
}
