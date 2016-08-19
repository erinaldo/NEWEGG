using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class HotWordsController : ApiController
    {
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        // GET api/hotwords/5
        public List<TWNewEgg.Models.ViewModels.HotWords.HotWords> Get(int categoryId)
        {
            List<TWNewEgg.Models.ViewModels.HotWords.HotWords> listHotWords = null;
            listHotWords = Processor.Request<List<TWNewEgg.Models.ViewModels.HotWords.HotWords>, List<TWNewEgg.Models.DomainModels.HotWords.HotWords>>("HotWordsService", "GetActive", categoryId).results;
            return listHotWords;
        }

        // POST api/hotwords
        public void Post([FromBody]string value)
        {
        }

        // PUT api/hotwords/5
        public void Put(int id, [FromBody]string value)
        {
        }

        //[DisableCors]
        // DELETE api/hotwords/5
        public void Delete(int id)
        {
        }
    }
}
